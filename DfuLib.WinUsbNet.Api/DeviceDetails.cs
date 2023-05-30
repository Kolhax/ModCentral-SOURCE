namespace DfuLib.WinUsbNet.Api;

internal struct DeviceDetails
{
	public string DevicePath;

	public string Manufacturer;

	public string DeviceDescription;

	public string ContainerID;

	public ushort VID;

	public ushort PID;
}
