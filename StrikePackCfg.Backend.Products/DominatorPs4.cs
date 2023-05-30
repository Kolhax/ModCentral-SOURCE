using System.Collections.Generic;
using Semver;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.HelperClasses;

namespace StrikePackCfg.Backend.Products;

public class DominatorPs4 : CollectiveMindsProduct
{
	public override StrikeMaxFlags Flags => StrikeMaxFlags.Dominator | StrikeMaxFlags.Ps4;

	public override int UserSlots => 8;

	public override int TotalSlots => 10;

	public override int FirstSlot => 0;

	public override SemVersion MinimumSupportedFirmware => SemVersion.Parse("1.2.3-beta.1");

	public override int FPSSlot => 1;

	public DominatorPs4()
		: base("{432D2316-94AE-4298-C123-363754AFA29D}", "Collective Minds StrikePack FPS for PS4")
	{
	}

	public override IEnumerable<BaseConfig> GetBuiltInConfig()
	{
		return ConfigHelper.GetPs4Config();
	}

	public override bool Is16Bit(string version)
	{
		return false;
	}

	public override bool Is32Bit()
	{
		return true;
	}

	public override bool SupportsAdjustments(string version)
	{
		return SemVersion.Parse(version) > "1.3.0-alpha.1";
	}

	public override bool SupportsSlot0Remapper(string version)
	{
		return SemVersion.Parse(version) >= "1.3.0-beta.1";
	}

	public override bool SupportsFpsGamepacks(string version)
	{
		return SemVersion.Parse(version) >= "2.0.0-alpha.1";
	}
}
