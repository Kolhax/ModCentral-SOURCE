using System.Collections.Generic;
using Semver;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.HelperClasses;

namespace StrikePackCfg.Backend.Products;

public class BattlePackXb1 : CollectiveMindsProduct
{
	public override StrikeMaxFlags Flags => StrikeMaxFlags.BattlePack | StrikeMaxFlags.Xb1;

	public override int UserSlots => 8;

	public override int TotalSlots => 10;

	public override int FirstSlot => 0;

	public override SemVersion MinimumSupportedFirmware => SemVersion.Parse("2.0.0-beta.1");

	public override int FPSSlot => 1;

	public BattlePackXb1()
		: base("{A2C348FA-11DE-7DA3-596F-F32B757A161C}", "Collective Minds Eliminator for Xbox One")
	{
	}

	public override bool Is16Bit(string version)
	{
		return true;
	}

	public override IEnumerable<BaseConfig> GetBuiltInConfig()
	{
		return ConfigHelper.GetXb1Config();
	}

	public override bool SupportsAdjustments(string version)
	{
		return SemVersion.Parse(version) >= "1.3.0-beta.1";
	}

	public override bool SupportsSlot0Remapper(string version)
	{
		return SemVersion.Parse(version) >= "1.3.0-beta.1";
	}
}
