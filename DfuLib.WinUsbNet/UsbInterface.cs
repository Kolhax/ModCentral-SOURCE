using System;
using DfuLib.WinUsbNet.Api;

namespace DfuLib.WinUsbNet;

public class UsbInterface
{
	public UsbPipeCollection Pipes { get; private set; }

	public int Number { get; private set; }

	public UsbDevice Device { get; private set; }

	public UsbPipe InPipe { get; private set; }

	public UsbPipe OutPipe { get; private set; }

	public UsbBaseClass BaseClass { get; private set; }

	public byte ClassValue { get; private set; }

	public byte SubClass { get; private set; }

	public byte Protocol { get; private set; }

	internal int InterfaceIndex { get; private set; }

	internal UsbInterface(UsbDevice device, int interfaceIndex, USB_INTERFACE_DESCRIPTOR rawDesc, UsbPipeCollection pipes)
	{
		ClassValue = rawDesc.bInterfaceClass;
		SubClass = rawDesc.bInterfaceSubClass;
		Protocol = rawDesc.bInterfaceProtocol;
		Number = rawDesc.bInterfaceNumber;
		InterfaceIndex = interfaceIndex;
		BaseClass = UsbBaseClass.Unknown;
		if (Enum.IsDefined(typeof(UsbBaseClass), (int)rawDesc.bInterfaceClass))
		{
			BaseClass = (UsbBaseClass)rawDesc.bInterfaceClass;
		}
		Device = device;
		Pipes = pipes;
		foreach (UsbPipe pipe in pipes)
		{
			pipe.AttachInterface(this);
			if (pipe.IsIn && InPipe == null)
			{
				InPipe = pipe;
			}
			if (pipe.IsOut && OutPipe == null)
			{
				OutPipe = pipe;
			}
		}
	}
}
