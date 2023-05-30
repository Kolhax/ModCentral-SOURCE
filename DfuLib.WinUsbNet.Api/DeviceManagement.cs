using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DfuLib.WinUsbNet.Api;

internal static class DeviceManagement
{
	[StructLayout(LayoutKind.Sequential)]
	private class DEV_BROADCAST_DEVICEINTERFACE
	{
		internal int dbcc_size;

		internal int dbcc_devicetype;

		internal int dbcc_reserved;

		internal Guid dbcc_classguid;

		internal short dbcc_name;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	private class DEV_BROADCAST_DEVICEINTERFACE_1
	{
		internal int dbcc_size;

		internal int dbcc_devicetype;

		internal int dbcc_reserved;

		internal Guid dbcc_classguid;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
		internal char[] dbcc_name;
	}

	[StructLayout(LayoutKind.Sequential)]
	private class DEV_BROADCAST_HDR
	{
		internal int dbch_size;

		internal int dbch_devicetype;

		internal int dbch_reserved;
	}

	private struct SP_DEVICE_INTERFACE_DATA
	{
		internal int cbSize;

		internal Guid InterfaceClassGuid;

		internal int Flags;

		internal IntPtr Reserved;
	}

	private struct SP_DEVINFO_DATA
	{
		internal int cbSize;

		internal Guid ClassGuid;

		internal int DevInst;

		internal IntPtr Reserved;
	}

	private enum SPDRP : uint
	{
		SPDRP_DEVICEDESC = 0u,
		SPDRP_HARDWAREID = 1u,
		SPDRP_COMPATIBLEIDS = 2u,
		SPDRP_NTDEVICEPATHS = 3u,
		SPDRP_SERVICE = 4u,
		SPDRP_CONFIGURATION = 5u,
		SPDRP_CONFIGURATIONVECTOR = 6u,
		SPDRP_CLASS = 7u,
		SPDRP_CLASSGUID = 8u,
		SPDRP_DRIVER = 9u,
		SPDRP_CONFIGFLAGS = 10u,
		SPDRP_MFG = 11u,
		SPDRP_FRIENDLYNAME = 12u,
		SPDRP_LOCATION_INFORMATION = 13u,
		SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 14u,
		SPDRP_CAPABILITIES = 15u,
		SPDRP_UI_NUMBER = 16u,
		SPDRP_UPPERFILTERS = 17u,
		SPDRP_LOWERFILTERS = 18u,
		SPDRP_MAXIMUM_PROPERTY = 19u,
		SPDRP_ENUMERATOR_NAME = 22u,
		SPDRP_BASE_CONTAINERID = 36u
	}

	private enum RegTypes
	{
		REG_SZ = 1,
		REG_MULTI_SZ = 7
	}

	internal const int DBT_DEVICEARRIVAL = 32768;

	internal const int DBT_DEVICEREMOVECOMPLETE = 32772;

	private const int DBT_DEVTYP_DEVICEINTERFACE = 5;

	private const int DBT_DEVTYP_HANDLE = 6;

	private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;

	private const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;

	private const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;

	internal const int WM_DEVICECHANGE = 537;

	private const int DIGCF_PRESENT = 2;

	private const int DIGCF_DEVICEINTERFACE = 16;

	private const int ERROR_NO_MORE_ITEMS = 259;

	private const int ERROR_INSUFFICIENT_BUFFER = 122;

	public static string GetNotifyMessageDeviceName(Message m, Guid checkGuid)
	{
		DEV_BROADCAST_DEVICEINTERFACE_1 dEV_BROADCAST_DEVICEINTERFACE_ = new DEV_BROADCAST_DEVICEINTERFACE_1();
		DEV_BROADCAST_HDR dEV_BROADCAST_HDR = new DEV_BROADCAST_HDR();
		Marshal.PtrToStructure(m.LParam, (object)dEV_BROADCAST_HDR);
		if (dEV_BROADCAST_HDR.dbch_devicetype == 5)
		{
			int num = Convert.ToInt32((dEV_BROADCAST_HDR.dbch_size - 32) / 2);
			dEV_BROADCAST_DEVICEINTERFACE_.dbcc_name = new char[num + 1];
			Marshal.PtrToStructure(m.LParam, (object)dEV_BROADCAST_DEVICEINTERFACE_);
			if (dEV_BROADCAST_DEVICEINTERFACE_.dbcc_classguid != checkGuid)
			{
				return null;
			}
			return new string(dEV_BROADCAST_DEVICEINTERFACE_.dbcc_name, 0, num);
		}
		return null;
	}

	private static byte[] GetProperty(IntPtr deviceInfoSet, SP_DEVINFO_DATA deviceInfoData, SPDRP property, out int regType)
	{
		if (!SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, property, IntPtr.Zero, IntPtr.Zero, 0u, out var RequiredSize) && Marshal.GetLastWin32Error() != 122)
		{
			throw ApiException.Win32("Failed to get buffer size for device registry property.");
		}
		byte[] array = new byte[RequiredSize];
		if (!SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref deviceInfoData, property, out regType, array, (uint)array.Length, out RequiredSize))
		{
			throw ApiException.Win32("Failed to get device registry property.");
		}
		return array;
	}

	private static string GetStringProperty(IntPtr deviceInfoSet, SP_DEVINFO_DATA deviceInfoData, SPDRP property)
	{
		int regType;
		byte[] property2 = GetProperty(deviceInfoSet, deviceInfoData, property, out regType);
		if (regType != 1)
		{
			throw new ApiException("Invalid registry type returned for device property.");
		}
		return Encoding.Unicode.GetString(property2, 0, property2.Length - 2);
	}

	private static string[] GetMultiStringProperty(IntPtr deviceInfoSet, SP_DEVINFO_DATA deviceInfoData, SPDRP property)
	{
		int regType;
		byte[] property2 = GetProperty(deviceInfoSet, deviceInfoData, property, out regType);
		if (regType != 7)
		{
			throw new ApiException("Invalid registry type returned for device property.");
		}
		return Encoding.Unicode.GetString(property2).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
	}

	private static DeviceDetails GetDeviceDetails(string devicePath, IntPtr deviceInfoSet, SP_DEVINFO_DATA deviceInfoData)
	{
		DeviceDetails result = default(DeviceDetails);
		result.DevicePath = devicePath;
		result.DeviceDescription = GetStringProperty(deviceInfoSet, deviceInfoData, SPDRP.SPDRP_DEVICEDESC);
		result.Manufacturer = GetStringProperty(deviceInfoSet, deviceInfoData, SPDRP.SPDRP_MFG);
		result.ContainerID = GetStringProperty(deviceInfoSet, deviceInfoData, SPDRP.SPDRP_BASE_CONTAINERID);
		string[] multiStringProperty = GetMultiStringProperty(deviceInfoSet, deviceInfoData, SPDRP.SPDRP_HARDWAREID);
		Regex regex = new Regex("^USB\\\\VID_([0-9A-F]{4})&PID_([0-9A-F]{4})", RegexOptions.IgnoreCase);
		bool flag = false;
		string[] array = multiStringProperty;
		foreach (string input in array)
		{
			Match match = regex.Match(input);
			if (match.Success)
			{
				result.VID = ushort.Parse(match.Groups[1].Value, NumberStyles.AllowHexSpecifier);
				result.PID = ushort.Parse(match.Groups[2].Value, NumberStyles.AllowHexSpecifier);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			throw new ApiException("Failed to find VID and PID for USB device. No hardware ID could be parsed.");
		}
		return result;
	}

	public static DeviceDetails[] FindDevicesFromGuid(Guid guid)
	{
		IntPtr intPtr = IntPtr.Zero;
		List<DeviceDetails> list = new List<DeviceDetails>();
		try
		{
			intPtr = SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, 18);
			if (intPtr == FileApi.INVALID_HANDLE_VALUE)
			{
				throw ApiException.Win32("Failed to enumerate devices.");
			}
			int num = 0;
			while (true)
			{
				SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = default(SP_DEVICE_INTERFACE_DATA);
				DeviceInterfaceData.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA));
				if (!SetupDiEnumDeviceInterfaces(intPtr, IntPtr.Zero, ref guid, num, ref DeviceInterfaceData))
				{
					break;
				}
				int RequiredSize = 0;
				if (!SetupDiGetDeviceInterfaceDetail(intPtr, ref DeviceInterfaceData, IntPtr.Zero, 0, ref RequiredSize, IntPtr.Zero) && Marshal.GetLastWin32Error() != 122)
				{
					throw ApiException.Win32("Failed to get interface details buffer size.");
				}
				IntPtr intPtr2 = IntPtr.Zero;
				try
				{
					intPtr2 = Marshal.AllocHGlobal(RequiredSize);
					Marshal.WriteInt32(intPtr2, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
					SP_DEVINFO_DATA DeviceInfoData = default(SP_DEVINFO_DATA);
					DeviceInfoData.cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
					if (!SetupDiGetDeviceInterfaceDetail(intPtr, ref DeviceInterfaceData, intPtr2, RequiredSize, ref RequiredSize, ref DeviceInfoData))
					{
						throw ApiException.Win32("Failed to get device interface details.");
					}
					DeviceDetails deviceDetails = GetDeviceDetails(Marshal.PtrToStringUni(new IntPtr(intPtr2.ToInt64() + 4)), intPtr, DeviceInfoData);
					list.Add(deviceDetails);
				}
				finally
				{
					if (intPtr2 != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(intPtr2);
						intPtr2 = IntPtr.Zero;
					}
				}
				num++;
			}
			if (Marshal.GetLastWin32Error() != 259)
			{
				throw ApiException.Win32("Failed to get device interface.");
			}
		}
		finally
		{
			if (intPtr != IntPtr.Zero && intPtr != FileApi.INVALID_HANDLE_VALUE)
			{
				SetupDiDestroyDeviceInfoList(intPtr);
			}
		}
		return list.ToArray();
	}

	public static void RegisterForDeviceNotifications(IntPtr controlHandle, Guid classGuid, ref IntPtr deviceNotificationHandle)
	{
		DEV_BROADCAST_DEVICEINTERFACE dEV_BROADCAST_DEVICEINTERFACE = new DEV_BROADCAST_DEVICEINTERFACE();
		IntPtr intPtr = IntPtr.Zero;
		try
		{
			int cb = (dEV_BROADCAST_DEVICEINTERFACE.dbcc_size = Marshal.SizeOf(typeof(DEV_BROADCAST_DEVICEINTERFACE)));
			dEV_BROADCAST_DEVICEINTERFACE.dbcc_devicetype = 5;
			dEV_BROADCAST_DEVICEINTERFACE.dbcc_reserved = 0;
			dEV_BROADCAST_DEVICEINTERFACE.dbcc_classguid = classGuid;
			intPtr = Marshal.AllocHGlobal(cb);
			Marshal.StructureToPtr((object)dEV_BROADCAST_DEVICEINTERFACE, intPtr, fDeleteOld: true);
			deviceNotificationHandle = RegisterDeviceNotification(controlHandle, intPtr, 0);
			if (deviceNotificationHandle == IntPtr.Zero)
			{
				throw ApiException.Win32("Failed to register device notification");
			}
			Marshal.PtrToStructure(intPtr, (object)dEV_BROADCAST_DEVICEINTERFACE);
		}
		finally
		{
			if (intPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}
	}

	public static void StopDeviceDeviceNotifications(IntPtr deviceNotificationHandle)
	{
		if (!UnregisterDeviceNotification(deviceNotificationHandle))
		{
			throw ApiException.Win32("Failed to unregister device notification");
		}
	}

	[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, int Flags);

	[DllImport("setupapi.dll", SetLastError = true)]
	private static extern int SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

	[DllImport("setupapi.dll", SetLastError = true)]
	private static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, int MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, SPDRP Property, out int PropertyRegDataType, byte[] PropertyBuffer, uint PropertyBufferSize, out uint RequiredSize);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, SPDRP Property, IntPtr PropertyRegDataType, IntPtr PropertyBuffer, uint PropertyBufferSize, out uint RequiredSize);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, ref SP_DEVINFO_DATA DeviceInfoData);

	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr DeviceInfoData);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool UnregisterDeviceNotification(IntPtr Handle);
}
