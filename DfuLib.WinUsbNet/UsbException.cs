using System;

namespace DfuLib.WinUsbNet;

public class UsbException : Exception
{
	public UsbException(string message)
		: base(message)
	{
	}

	public UsbException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
