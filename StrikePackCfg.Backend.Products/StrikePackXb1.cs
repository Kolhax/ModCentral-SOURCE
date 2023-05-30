using System.Collections.Generic;
using Semver;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.HelperClasses;

namespace StrikePackCfg.Backend.Products;

public class StrikePackXb1 : CollectiveMindsProduct
{
	public override StrikeMaxFlags Flags => StrikeMaxFlags.Dominator | StrikeMaxFlags.Xb1;

	public override int UserSlots => 8;

	public override int TotalSlots => 10;

	public override int FirstSlot => 0;

	public override SemVersion MinimumSupportedFirmware => SemVersion.Parse("2.0.1");

	public override int FPSSlot => 1;

	public StrikePackXb1()
		: base("{193b2104-91f4-4d21-b877-1435584f4a2a}", "Collective Minds StrikePack for Xbox One")
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
		return SemVersion.Parse(version) >= "2.0.0";
	}

	public override bool SupportsSlot0Remapper(string version)
	{
		return SemVersion.Parse(version) >= "2.0.0";
	}
}
