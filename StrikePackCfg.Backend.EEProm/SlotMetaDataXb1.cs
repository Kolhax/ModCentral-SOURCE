namespace StrikePackCfg.Backend.EEProm;

public class SlotMetaDataXb1 : SlotMetaDataBase
{
	public sealed override byte[] Maps { get; protected set; }

	public SlotMetaDataXb1(bool is16Bit, bool supportsAdjustments)
		: base(is16Bit, supportsAdjustments)
	{
		Maps = (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack) ? new byte[34] : new byte[30]);
		for (byte b = 0; b < Maps.Length; b = (byte)(b + 1))
		{
			Maps[b] = b;
		}
	}
}
