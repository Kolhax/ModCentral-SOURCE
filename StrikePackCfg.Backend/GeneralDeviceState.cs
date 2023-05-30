namespace StrikePackCfg.Backend;

public class GeneralDeviceState : IDeviceState
{
	public CollectiveMindsDevice Device { get; }

	public bool IsOkToExecute { get; set; }

	public bool FirmwareSupported => Device.ApplicationVersion >= Device.Product.MinimumSupportedFirmware;

	public bool GamepacksSupported => false;

	public GeneralDeviceState(CollectiveMindsDevice device)
	{
		Device = device;
	}
}
