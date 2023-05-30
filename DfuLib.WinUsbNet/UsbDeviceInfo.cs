using System;
using DfuLib.WinUsbNet.Api;

namespace DfuLib.WinUsbNet;

public class UsbDeviceInfo
{
	private DeviceDetails _details;

	private Guid _containerGuid;

	public int VID => _details.VID;

	public int PID => _details.PID;

	public string Manufacturer => _details.Manufacturer;

	public string DeviceDescription => _details.DeviceDescription;

	public string DevicePath => _details.DevicePath;

	public Guid DeviceContainerId
	{
		get
		{
			if (_containerGuid == Guid.Empty)
			{
				_containerGuid = new Guid(_details.ContainerID);
			}
			return _containerGuid;
		}
	}

	internal UsbDeviceInfo(DeviceDetails details)
	{
		_details = details;
	}
}
