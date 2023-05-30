namespace DfuLib.WinUsbNet.Api;

internal struct WINUSB_PIPE_INFORMATION
{
	public USBD_PIPE_TYPE PipeType;

	public byte PipeId;

	public ushort MaximumPacketSize;

	public byte Interval;
}
