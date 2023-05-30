namespace StrikePackCfg.Backend.EEProm;

public class DominatorData
{
	public readonly PaddlesData PaddleConfig;

	public readonly SlotMetaDataBase SlotData;

	public DominatorData(PaddlesData paddleConfig, SlotMetaDataBase slotData)
	{
		PaddleConfig = paddleConfig;
		SlotData = slotData;
	}
}
