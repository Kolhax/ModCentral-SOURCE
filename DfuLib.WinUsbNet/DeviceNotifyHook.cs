using System;
using System.Windows.Forms;
using DfuLib.WinUsbNet.Api;

namespace DfuLib.WinUsbNet;

internal class DeviceNotifyHook : NativeWindow, IDeviceNotifyHook, IDisposable
{
	private Control _parent;

	private UsbNotifier _notifier;

	private Guid _guid;

	private IntPtr _notifyHandle;

	public DeviceNotifyHook(UsbNotifier notifier, Control parent, Guid guid)
	{
		_parent = parent;
		_guid = guid;
		_parent.HandleCreated += OnHandleCreated;
		_parent.HandleDestroyed += OnHandleDestroyed;
		_notifier = notifier;
		if (_parent.IsHandleCreated)
		{
			OnHandleCreated(parent, new EventArgs());
		}
	}

	~DeviceNotifyHook()
	{
		Dispose(disposing: false);
	}

	internal void OnHandleCreated(object sender, EventArgs e)
	{
		try
		{
			IntPtr controlHandle = ((Control)sender).Handle;
			AssignHandle(controlHandle);
			if (_notifyHandle != IntPtr.Zero)
			{
				DeviceManagement.StopDeviceDeviceNotifications(_notifyHandle);
				_notifyHandle = IntPtr.Zero;
			}
			DeviceManagement.RegisterForDeviceNotifications(controlHandle, _guid, ref _notifyHandle);
		}
		catch (ApiException innerException)
		{
			throw new UsbException("Failed to register new window handle for device notification.", innerException);
		}
	}

	internal void OnHandleDestroyed(object sender, EventArgs e)
	{
		try
		{
			ReleaseHandle();
			if (_notifyHandle != IntPtr.Zero)
			{
				DeviceManagement.StopDeviceDeviceNotifications(_notifyHandle);
				_notifyHandle = IntPtr.Zero;
			}
		}
		catch (ApiException innerException)
		{
			throw new UsbException("Failed to unregister destroyed window handle for device notification.", innerException);
		}
	}

	protected override void WndProc(ref Message m)
	{
		int msg = m.Msg;
		if (msg == 537)
		{
			_notifier.HandleDeviceChange(m);
		}
		base.WndProc(ref m);
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
