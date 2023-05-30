namespace StrikePackCfg.Backend.EEProm;

public class SlotMetaDataSwitch : SlotMetaDataBase
{
	public sealed override byte[] Maps { get; protected set; }

	public SlotMetaDataSwitch(bool is16Bit, bool supportsAdjustments)
		: base(is16Bit, supportsAdjustments)
	{
		Maps = new byte[36];
		for (byte b = 0; b < Maps.Length; b = (byte)(b + 1))
		{
			Maps[b] = b;
		}
	}
}
