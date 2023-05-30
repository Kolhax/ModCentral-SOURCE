using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using DfuLib.WinUsbNet.Api;

namespace DfuLib.WinUsbNet;

internal class DeviceNotifyHookWpf : IDeviceNotifyHook, IDisposable
{
	private Window _parent;

	private UsbNotifier _notifier;

	private Guid _guid;

	private IntPtr _notifyHandle;

	public DeviceNotifyHookWpf(UsbNotifier notifier, Window parent, Guid guid)
	{
		_parent = parent;
		_guid = guid;
		_notifier = notifier;
		IntPtr intPtr = new WindowInteropHelper(parent).EnsureHandle();
		HwndSource.FromHwnd(intPtr).AddHook(WndProc);
		try
		{
			if (_notifyHandle != IntPtr.Zero)
			{
				DeviceManagement.StopDeviceDeviceNotifications(_notifyHandle);
				_notifyHandle = IntPtr.Zero;
			}
			DeviceManagement.RegisterForDeviceNotifications(intPtr, _guid, ref _notifyHandle);
		}
		catch (ApiException innerException)
		{
			throw new UsbException("Failed to register new window handle for device notification.", innerException);
		}
	}

	private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		if (msg == 537)
		{
			_notifier.HandleDeviceChange(Message.Create(hwnd, msg, wParam, lParam));
		}
		return IntPtr.Zero;
	}

	~DeviceNotifyHookWpf()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_notifyHandle != IntPtr.Zero)
		{
			DeviceManagement.StopDeviceDeviceNotifications(_notifyHandle);
			_notifyHandle = IntPtr.Zero;
		}
	}
}
