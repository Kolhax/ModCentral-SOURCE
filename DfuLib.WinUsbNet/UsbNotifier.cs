using System;
using System.Windows;
using System.Windows.Forms;
using DfuLib.WinUsbNet.Api;

namespace DfuLib.WinUsbNet;

public class UsbNotifier : IDisposable
{
	private IDeviceNotifyHook _hook;

	private Guid _guid;

	public Guid Guid => _guid;

	public event USBEventHandler Arrival;

	public event USBEventHandler Removal;

	public UsbNotifier(object control, string guidString)
		: this(control, new Guid(guidString))
	{
	}

	public UsbNotifier(object control, Guid guid)
	{
		_guid = guid;
		if (control is Control)
		{
			_hook = new DeviceNotifyHook(this, control as Control, _guid);
			return;
		}
		if (control is Window)
		{
			_hook = new DeviceNotifyHookWpf(this, control as Window, guid);
			return;
		}
		throw new ArgumentException("invalid type given, must be control or window", "control");
	}

	protected void OnArrival(string devicePath)
	{
		if (this.Arrival != null)
		{
			this.Arrival(this, new USBEvent(USBEventType.DeviceArrival, _guid, devicePath));
		}
	}

	protected void OnRemoval(string devicePath)
	{
		if (this.Removal != null)
		{
			this.Removal(this, new USBEvent(USBEventType.DeviceRemoval, _guid, devicePath));
		}
	}

	internal void HandleDeviceChange(Message m)
	{
		if (m.Msg != 537)
		{
			throw new UsbException("Invalid device change message.");
		}
		if ((int)m.WParam == 32768)
		{
			string notifyMessageDeviceName = DeviceManagement.GetNotifyMessageDeviceName(m, _guid);
			if (notifyMessageDeviceName != null)
			{
				OnArrival(notifyMessageDeviceName);
			}
		}
		if ((int)m.WParam == 32772)
		{
			string notifyMessageDeviceName2 = DeviceManagement.GetNotifyMessageDeviceName(m, _guid);
			if (notifyMessageDeviceName2 != null)
			{
				OnRemoval(notifyMessageDeviceName2);
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			_hook.Dispose();
		}
	}
}
