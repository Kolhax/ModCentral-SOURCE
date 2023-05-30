using System.Collections.Generic;
using Semver;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.HelperClasses;

namespace StrikePackCfg.Backend.Products;

public class DominatorXb1 : CollectiveMindsProduct
{
	public override StrikeMaxFlags Flags => StrikeMaxFlags.Dominator | StrikeMaxFlags.Xb1;

	public override int UserSlots => 8;

	public override int TotalSlots => 10;

	public override int FirstSlot => 0;

	public override SemVersion MinimumSupportedFirmware => SemVersion.Parse("1.1.0-beta.1");

	public override int FPSSlot => 1;

	public DominatorXb1()
		: base("{2a3d1102-3137-4d29-c887-143a584c425a}", "Collective Minds StrikePack FPS for Xbox One")
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
