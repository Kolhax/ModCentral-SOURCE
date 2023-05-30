using System;
using DfuLib.WinUsbNet.Api;

namespace DfuLib.WinUsbNet;

public class UsbDeviceDescriptor
{
	public string PathName { get; private set; }

	public int VID { get; private set; }

	public int PID { get; private set; }

	public string Manufacturer { get; private set; }

	public string Product { get; private set; }

	public string SerialNumber { get; private set; }

	public string FullName
	{
		get
		{
			if (Manufacturer != null && Product != null)
			{
				return Product + " - " + Manufacturer;
			}
			if (Product != null)
			{
				return Product;
			}
			if (SerialNumber != null)
			{
				return SerialNumber;
			}
			return PathName;
		}
	}

	public byte ClassValue { get; private set; }

	public byte SubClass { get; private set; }

	public byte Protocol { get; private set; }

	public UsbBaseClass BaseClass { get; private set; }

	internal UsbDeviceDescriptor(string path, USB_DEVICE_DESCRIPTOR deviceDesc, string manufacturer, string product, string serialNumber)
	{
		PathName = path;
		VID = deviceDesc.idVendor;
		PID = deviceDesc.idProduct;
		Manufacturer = manufacturer;
		Product = product;
		SerialNumber = serialNumber;
		ClassValue = deviceDesc.bDeviceClass;
		SubClass = deviceDesc.bDeviceSubClass;
		Protocol = deviceDesc.bDeviceProtocol;
		BaseClass = UsbBaseClass.Unknown;
		if (Enum.IsDefined(typeof(UsbBaseClass), (int)deviceDesc.bDeviceClass))
		{
			BaseClass = (UsbBaseClass)deviceDesc.bDeviceClass;
		}
	}
}
