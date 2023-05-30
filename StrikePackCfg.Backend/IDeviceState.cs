namespace StrikePackCfg.Backend;

public interface IDeviceState
{
	CollectiveMindsDevice Device { get; }

	bool IsOkToExecute { get; set; }

	bool FirmwareSupported { get; }

	bool GamepacksSupported { get; }
}
