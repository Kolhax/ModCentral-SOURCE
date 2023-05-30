namespace StrikePackCfg.Backend.EEProm;

public class SlotMetaDataPs4 : SlotMetaDataBase
{
	public sealed override byte[] Maps { get; protected set; }

	public SlotMetaDataPs4(bool is16Bit, bool supportsAdjustments)
		: base(is16Bit, supportsAdjustments)
	{
		Maps = new byte[34];
		for (byte b = 0; b < Maps.Length; b = (byte)(b + 1))
		{
			Maps[b] = b;
		}
	}
}
