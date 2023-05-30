using System.Collections.Generic;
using Semver;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.HelperClasses;

namespace StrikePackCfg.Backend.Products;

public class SwitchCon : CollectiveMindsProduct
{
	public override StrikeMaxFlags Flags => StrikeMaxFlags.Dominator | StrikeMaxFlags.Switch;

	public override int UserSlots => 4;

	public override int TotalSlots => 7;

	public override int FirstSlot => 3;

	public override SemVersion MinimumSupportedFirmware => SemVersion.Parse("1.0.0");

	public override int FPSSlot => 1;

	public SwitchCon()
		: base("{962D7904-5B8D-4F98-BECE-0848B3A881AE} ", "Collective Minds SwitchUP for Nintendo Switch")
	{
	}

	public override IEnumerable<BaseConfig> GetBuiltInConfig()
	{
		return ConfigHelper.GetXb1Config();
	}

	public override bool SupportsAdjustments(string version)
	{
		return SemVersion.Parse(version) >= "1.0.0";
	}

	public override bool CanReflashFPS()
	{
		return false;
	}

	public override bool ProductHasPaddles()
	{
		return false;
	}

	public override bool SupportsSlot0Remapper(string version)
	{
		return SemVersion.Parse(version) >= "1.0.0";
	}

	public override bool Is16Bit(string version)
	{
		return true;
	}
}
