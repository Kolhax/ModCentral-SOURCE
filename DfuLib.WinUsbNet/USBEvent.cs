using System;

namespace DfuLib.WinUsbNet;

public class USBEvent : EventArgs
{
	public Guid Guid;

	public string DevicePath;

	public USBEventType Type;

	internal USBEvent(USBEventType type, Guid guid, string devicePath)
	{
		Guid = guid;
		DevicePath = devicePath;
		Type = type;
	}
}
