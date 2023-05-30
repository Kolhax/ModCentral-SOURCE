using System.Collections.Generic;
using Semver;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.HelperClasses;

namespace StrikePackCfg.Backend.Products;

public class EliminatorProXb1 : CollectiveMindsProduct
{
	public override StrikeMaxFlags Flags => StrikeMaxFlags.BattlePack | StrikeMaxFlags.Xb1 | StrikeMaxFlags.IsPro;

	public override int UserSlots => 0;

	public override int TotalSlots => 1;

	public override int FirstSlot => 0;

	public override SemVersion MinimumSupportedFirmware => SemVersion.Parse("2.0.7-beta.2");

	public override int FPSSlot => 0;

	public EliminatorProXb1()
		: base("{0B5EE9C6-1785-4D2C-8CEC-D017754B7EC2}", "Collective Minds Battle Pack for Xbox One")
	{
	}

	public override bool Is16Bit(string version)
	{
		return true;
	}

	public override IEnumerable<BaseConfig> GetBuiltInConfig()
	{
		return ConfigHelper.GetXb1ConfigPro();
	}

	public override bool SupportsAdjustments(string version)
	{
		return true;
	}

	public override bool SupportsSlot0Remapper(string version)
	{
		return true;
	}
}
