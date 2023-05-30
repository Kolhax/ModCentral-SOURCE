using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace DfuLib.WinUsbNet.Api;

internal class WinUsbDevice : IDisposable
{
	private enum USB_DEVICE_SPEED
	{
		UsbLowSpeed = 1,
		UsbFullSpeed,
		UsbHighSpeed
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	private struct WINUSB_SETUP_PACKET
	{
		public byte RequestType;

		public byte Request;

		public ushort Value;

		public ushort Index;

		public ushort Length;
	}

	private bool _disposed;

	private SafeFileHandle _deviceHandle;

	private IntPtr _winUsbHandle = IntPtr.Zero;

	private IntPtr[] _addInterfaces;

	private const uint DEVICE_SPEED = 1u;

	private const int USB_DEVICE_DESCRIPTOR_TYPE = 1;

	private const int USB_CONFIGURATION_DESCRIPTOR_TYPE = 2;

	private const int USB_STRING_DESCRIPTOR_TYPE = 3;

	private const int ERROR_NO_MORE_ITEMS = 259;

	public int InterfaceCount => 1 + ((_addInterfaces != null) ? _addInterfaces.Length : 0);

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~WinUsbDevice()
	{
		Dispose(disposing: false);
	}

	private void CheckNotDisposed()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("USB device object has been disposed.");
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}
		if (disposing)
		{
			if (_deviceHandle != null && !_deviceHandle.IsInvalid)
			{
				_deviceHandle.Dispose();
			}
			_deviceHandle = null;
		}
		FreeWinUSB();
		_disposed = true;
	}

	private void FreeWinUSB()
	{
		if (_addInterfaces != null)
		{
			for (int i = 0; i < _addInterfaces.Length; i++)
			{
				WinUsb_Free(_addInterfaces[i]);
			}
			_addInterfaces = null;
		}
		if (_winUsbHandle != IntPtr.Zero)
		{
			WinUsb_Free(_winUsbHandle);
		}
		_winUsbHandle = IntPtr.Zero;
	}

	public USB_DEVICE_DESCRIPTOR GetDeviceDescriptor()
	{
		uint num = (uint)Marshal.SizeOf(typeof(USB_DEVICE_DESCRIPTOR));
		if (!WinUsb_GetDescriptor(_winUsbHandle, (byte)1, (byte)0, (ushort)0, out USB_DEVICE_DESCRIPTOR deviceDesc, num, out uint LengthTransfered))
		{
			throw ApiException.Win32("Failed to get USB device descriptor.");
		}
		if (LengthTransfered != num)
		{
			throw ApiException.Win32("Incomplete USB device descriptor.");
		}
		return deviceDesc;
	}

	public string GetStringDescriptor(byte index)
	{
		byte[] array = new byte[256];
		if (!WinUsb_GetDescriptor(_winUsbHandle, 3, index, 0, array, (uint)array.Length, out var _))
		{
			throw ApiException.Win32("Failed to get USB string descriptor (" + index + ").");
		}
		int num = array[0] - 2;
		if (num <= 0)
		{
			return null;
		}
		return new string(Encoding.Unicode.GetChars(array, 2, num));
	}

	public void ControlTransfer(byte requestType, byte request, ushort value, ushort index, ushort length, byte[] data)
	{
		uint LengthTransferred = 0u;
		WINUSB_SETUP_PACKET setupPacket = default(WINUSB_SETUP_PACKET);
		setupPacket.RequestType = requestType;
		setupPacket.Request = request;
		setupPacket.Value = value;
		setupPacket.Index = index;
		setupPacket.Length = length;
		if (!WinUsb_ControlTransfer(_winUsbHandle, setupPacket, data, length, ref LengthTransferred, IntPtr.Zero))
		{
			throw ApiException.Win32("Control transfer on WinUSB device failed.");
		}
	}

	public void OpenDevice(string devicePathName)
	{
		try
		{
			_deviceHandle = FileApi.CreateFile(devicePathName, 3221225472u, 3, IntPtr.Zero, 3, 1073741952, 0);
			if (_deviceHandle.IsInvalid)
			{
				throw ApiException.Win32("Failed to open WinUSB device handle.");
			}
			InitializeDevice();
		}
		catch (Exception)
		{
			if (_deviceHandle != null)
			{
				_deviceHandle.Dispose();
				_deviceHandle = null;
			}
			FreeWinUSB();
			throw;
		}
	}

	private IntPtr InterfaceHandle(int index)
	{
		if (index == 0)
		{
			return _winUsbHandle;
		}
		return _addInterfaces[index - 1];
	}

	public void GetInterfaceInfo(int interfaceIndex, out USB_INTERFACE_DESCRIPTOR descriptor, out WINUSB_PIPE_INFORMATION[] pipes)
	{
		List<WINUSB_PIPE_INFORMATION> list = new List<WINUSB_PIPE_INFORMATION>();
		if (!WinUsb_QueryInterfaceSettings(InterfaceHandle(interfaceIndex), 0, out descriptor))
		{
			throw ApiException.Win32("Failed to get WinUSB device interface descriptor.");
		}
		IntPtr interfaceHandle = InterfaceHandle(interfaceIndex);
		for (byte b = 0; b < descriptor.bNumEndpoints; b = (byte)(b + 1))
		{
			WINUSB_PIPE_INFORMATION PipeInformation;
			bool num = WinUsb_QueryPipe(interfaceHandle, 0, b, out PipeInformation);
			list.Add(PipeInformation);
			if (!num)
			{
				throw ApiException.Win32("Failed to get WinUSB device pipe information.");
			}
		}
		pipes = list.ToArray();
	}

	private void InitializeDevice()
	{
		if (!WinUsb_Initialize(_deviceHandle, ref _winUsbHandle))
		{
			throw ApiException.Win32("Failed to initialize WinUSB handle. Device might not be connected.");
		}
		List<IntPtr> list = new List<IntPtr>();
		byte b = 0;
		byte b2 = 0;
		try
		{
			while (true)
			{
				IntPtr AssociatedInterfaceHandle = IntPtr.Zero;
				if (!WinUsb_GetAssociatedInterface(_winUsbHandle, b2, out AssociatedInterfaceHandle))
				{
					break;
				}
				list.Add(AssociatedInterfaceHandle);
				b2 = (byte)(b2 + 1);
				b = (byte)(b + 1);
			}
			if (Marshal.GetLastWin32Error() != 259)
			{
				throw ApiException.Win32("Failed to enumerate interfaces for WinUSB device.");
			}
		}
		finally
		{
			_addInterfaces = list.ToArray();
		}
		ThreadPool.BindHandle(_deviceHandle);
	}

	public unsafe void ReadPipe(int ifaceIndex, byte pipeID, byte[] buffer, int offset, int bytesToRead, out uint bytesRead)
	{
		bool num;
		fixed (byte* ptr = buffer)
		{
			num = WinUsb_ReadPipe(InterfaceHandle(ifaceIndex), pipeID, ptr + offset, (uint)bytesToRead, out bytesRead, IntPtr.Zero);
		}
		if (!num)
		{
			throw ApiException.Win32("Failed to read pipe on WinUSB device.");
		}
	}

	private unsafe void HandleOverlappedAPI(bool success, string errorMessage, NativeOverlapped* pOverlapped, UsbAsyncResult result, int bytesTransfered)
	{
		if (!success)
		{
			if (Marshal.GetLastWin32Error() != 997)
			{
				Overlapped.Unpack(pOverlapped);
				Overlapped.Free(pOverlapped);
				throw ApiException.Win32(errorMessage);
			}
		}
		else
		{
			Overlapped.Unpack(pOverlapped);
			Overlapped.Free(pOverlapped);
			result.OnCompletion(completedSynchronously: true, null, bytesTransfered, synchronousCallback: false);
		}
	}

	public unsafe void ReadPipeOverlapped(int ifaceIndex, byte pipeID, byte[] buffer, int offset, int bytesToRead, UsbAsyncResult result)
	{
		Overlapped obj = new Overlapped
		{
			AsyncResult = result
		};
		NativeOverlapped* ptr = null;
		ptr = obj.Pack(PipeIOCallback, buffer);
		bool success;
		uint LengthTransferred;
		fixed (byte* ptr2 = buffer)
		{
			success = WinUsb_ReadPipe(InterfaceHandle(ifaceIndex), pipeID, ptr2 + offset, (uint)bytesToRead, out LengthTransferred, ptr);
		}
		HandleOverlappedAPI(success, "Failed to asynchronously read pipe on WinUSB device.", ptr, result, (int)LengthTransferred);
	}

	public unsafe void WriteOverlapped(int ifaceIndex, byte pipeID, byte[] buffer, int offset, int bytesToWrite, UsbAsyncResult result)
	{
		Overlapped obj = new Overlapped
		{
			AsyncResult = result
		};
		NativeOverlapped* ptr = null;
		ptr = obj.Pack(PipeIOCallback, buffer);
		bool success;
		uint LengthTransferred;
		fixed (byte* ptr2 = buffer)
		{
			success = WinUsb_WritePipe(InterfaceHandle(ifaceIndex), pipeID, ptr2 + offset, (uint)bytesToWrite, out LengthTransferred, ptr);
		}
		HandleOverlappedAPI(success, "Failed to asynchronously write pipe on WinUSB device.", ptr, result, (int)LengthTransferred);
	}

	public unsafe void ControlTransferOverlapped(byte requestType, byte request, ushort value, ushort index, ushort length, byte[] data, UsbAsyncResult result)
	{
		uint LengthTransferred = 0u;
		WINUSB_SETUP_PACKET setupPacket = default(WINUSB_SETUP_PACKET);
		setupPacket.RequestType = requestType;
		setupPacket.Request = request;
		setupPacket.Value = value;
		setupPacket.Index = index;
		setupPacket.Length = length;
		Overlapped obj = new Overlapped
		{
			AsyncResult = result
		};
		NativeOverlapped* ptr = null;
		ptr = obj.Pack(PipeIOCallback, data);
		bool success = WinUsb_ControlTransfer(_winUsbHandle, setupPacket, data, length, ref LengthTransferred, ptr);
		HandleOverlappedAPI(success, "Asynchronous control transfer on WinUSB device failed.", ptr, result, (int)LengthTransferred);
	}

	private unsafe void PipeIOCallback(uint errorCode, uint numBytes, NativeOverlapped* pOverlapped)
	{
		try
		{
			Exception error = null;
			if (errorCode != 0)
			{
				error = ApiException.Win32("Asynchronous operation on WinUSB device failed.", (int)errorCode);
			}
			UsbAsyncResult obj = (UsbAsyncResult)Overlapped.Unpack(pOverlapped).AsyncResult;
			Overlapped.Free(pOverlapped);
			pOverlapped = null;
			obj.OnCompletion(completedSynchronously: false, error, (int)numBytes, synchronousCallback: true);
		}
		finally
		{
			if (pOverlapped != null)
			{
				Overlapped.Unpack(pOverlapped);
				Overlapped.Free(pOverlapped);
			}
		}
	}

	public void AbortPipe(int ifaceIndex, byte pipeID)
	{
		if (!WinUsb_AbortPipe(InterfaceHandle(ifaceIndex), pipeID))
		{
			throw ApiException.Win32("Failed to abort pipe on WinUSB device.");
		}
	}

	public unsafe void WritePipe(int ifaceIndex, byte pipeID, byte[] buffer, int offset, int length)
	{
		bool num;
		uint LengthTransferred;
		fixed (byte* ptr = buffer)
		{
			num = WinUsb_WritePipe(InterfaceHandle(ifaceIndex), pipeID, ptr + offset, (uint)length, out LengthTransferred, IntPtr.Zero);
		}
		if (!num || LengthTransferred != length)
		{
			throw ApiException.Win32("Failed to write pipe on WinUSB device.");
		}
	}

	public void FlushPipe(int ifaceIndex, byte pipeID)
	{
		if (!WinUsb_FlushPipe(InterfaceHandle(ifaceIndex), pipeID))
		{
			throw ApiException.Win32("Failed to flush pipe on WinUSB device.");
		}
	}

	public void SetPipePolicy(int ifaceIndex, byte pipeID, POLICY_TYPE policyType, bool value)
	{
		byte Value = (byte)(value ? 1u : 0u);
		if (!WinUsb_SetPipePolicy(InterfaceHandle(ifaceIndex), pipeID, (uint)policyType, 1u, ref Value))
		{
			throw ApiException.Win32("Failed to set WinUSB pipe policy.");
		}
	}

	public void SetPipePolicy(int ifaceIndex, byte pipeID, POLICY_TYPE policyType, uint value)
	{
		if (!WinUsb_SetPipePolicy(InterfaceHandle(ifaceIndex), pipeID, (uint)policyType, 4u, ref value))
		{
			throw ApiException.Win32("Failed to set WinUSB pipe policy.");
		}
	}

	public bool GetPipePolicyBool(int ifaceIndex, byte pipeID, POLICY_TYPE policyType)
	{
		uint ValueLength = 1u;
		if (!WinUsb_GetPipePolicy(InterfaceHandle(ifaceIndex), pipeID, (uint)policyType, ref ValueLength, out byte Value) || ValueLength != 1)
		{
			throw ApiException.Win32("Failed to get WinUSB pipe policy.");
		}
		return Value != 0;
	}

	public uint GetPipePolicyUInt(int ifaceIndex, byte pipeID, POLICY_TYPE policyType)
	{
		uint ValueLength = 4u;
		if (!WinUsb_GetPipePolicy(InterfaceHandle(ifaceIndex), pipeID, (uint)policyType, ref ValueLength, out uint Value) || ValueLength != 4)
		{
			throw ApiException.Win32("Failed to get WinUSB pipe policy.");
		}
		return Value;
	}

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_ControlTransfer(IntPtr InterfaceHandle, WINUSB_SETUP_PACKET SetupPacket, byte[] Buffer, uint BufferLength, ref uint LengthTransferred, IntPtr Overlapped);

	[DllImport("winusb.dll", SetLastError = true)]
	private unsafe static extern bool WinUsb_ControlTransfer(IntPtr InterfaceHandle, WINUSB_SETUP_PACKET SetupPacket, byte[] Buffer, uint BufferLength, ref uint LengthTransferred, NativeOverlapped* pOverlapped);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_Free(IntPtr InterfaceHandle);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_Initialize(SafeFileHandle DeviceHandle, ref IntPtr InterfaceHandle);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_QueryDeviceInformation(IntPtr InterfaceHandle, uint InformationType, ref uint BufferLength, out byte Buffer);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_QueryInterfaceSettings(IntPtr InterfaceHandle, byte AlternateInterfaceNumber, out USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_QueryPipe(IntPtr InterfaceHandle, byte AlternateInterfaceNumber, byte PipeIndex, out WINUSB_PIPE_INFORMATION PipeInformation);

	[DllImport("winusb.dll", SetLastError = true)]
	private unsafe static extern bool WinUsb_ReadPipe(IntPtr InterfaceHandle, byte PipeID, byte* pBuffer, uint BufferLength, out uint LengthTransferred, IntPtr Overlapped);

	[DllImport("winusb.dll", SetLastError = true)]
	private unsafe static extern bool WinUsb_ReadPipe(IntPtr InterfaceHandle, byte PipeID, byte* pBuffer, uint BufferLength, out uint LengthTransferred, NativeOverlapped* pOverlapped);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_AbortPipe(IntPtr InterfaceHandle, byte PipeID);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_SetPipePolicy(IntPtr InterfaceHandle, byte PipeID, uint PolicyType, uint ValueLength, ref byte Value);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_GetPipePolicy(IntPtr InterfaceHandle, byte PipeID, uint PolicyType, ref uint ValueLength, out byte Value);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_SetPipePolicy(IntPtr InterfaceHandle, byte PipeID, uint PolicyType, uint ValueLength, ref uint Value);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_GetPipePolicy(IntPtr InterfaceHandle, byte PipeID, uint PolicyType, ref uint ValueLength, out uint Value);

	[DllImport("winusb.dll", SetLastError = true)]
	private unsafe static extern bool WinUsb_WritePipe(IntPtr InterfaceHandle, byte PipeID, byte* pBuffer, uint BufferLength, out uint LengthTransferred, IntPtr Overlapped);

	[DllImport("winusb.dll", SetLastError = true)]
	private unsafe static extern bool WinUsb_WritePipe(IntPtr InterfaceHandle, byte PipeID, byte* pBuffer, uint BufferLength, out uint LengthTransferred, NativeOverlapped* pOverlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool CancelIo(IntPtr hFile);

	[DllImport("kernel32.dll", SetLastError = true)]
	private unsafe static extern bool CancelIoEx(IntPtr hFile, NativeOverlapped* pOverlapped);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_FlushPipe(IntPtr InterfaceHandle, byte PipeID);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_GetDescriptor(IntPtr InterfaceHandle, byte DescriptorType, byte Index, ushort LanguageID, byte[] Buffer, uint BufferLength, out uint LengthTransfered);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_GetDescriptor(IntPtr InterfaceHandle, byte DescriptorType, byte Index, ushort LanguageID, out USB_DEVICE_DESCRIPTOR deviceDesc, uint BufferLength, out uint LengthTransfered);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_GetDescriptor(IntPtr InterfaceHandle, byte DescriptorType, byte Index, ushort LanguageID, out USB_CONFIGURATION_DESCRIPTOR deviceDesc, uint BufferLength, out uint LengthTransfered);

	[DllImport("winusb.dll", SetLastError = true)]
	private static extern bool WinUsb_GetAssociatedInterface(IntPtr InterfaceHandle, byte AssociatedInterfaceIndex, out IntPtr AssociatedInterfaceHandle);
}
