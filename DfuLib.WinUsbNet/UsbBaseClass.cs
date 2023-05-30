namespace DfuLib.WinUsbNet;

public enum UsbBaseClass
{
	Unknown = -1,
	None = 0,
	Audio = 1,
	CommCDC = 2,
	HID = 3,
	Physical = 5,
	Image = 6,
	Printer = 7,
	MassStorage = 8,
	Hub = 9,
	CDCData = 10,
	SmartCard = 11,
	ContentSecurity = 13,
	Video = 14,
	PersonalHealthcare = 15,
	DiagnosticDevice = 220,
	WirelessController = 224,
	Miscellaneous = 239,
	ApplicationSpecific = 254,
	VendorSpecific = 255
}
