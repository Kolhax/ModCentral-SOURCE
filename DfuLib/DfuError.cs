namespace DfuLib;

public enum DfuError : byte
{
	Ok,
	Target,
	File,
	Write,
	Erase,
	CheckErased,
	Prog,
	Verify,
	Address,
	NotDone,
	Firmware,
	Vendor,
	UsbReset,
	PowerOnReset,
	Unknown,
	Stalled
}
