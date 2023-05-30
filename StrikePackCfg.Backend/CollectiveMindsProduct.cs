using System;
using System.Collections.Generic;
using Semver;
using StrikePackCfg.Backend.Config;

namespace StrikePackCfg.Backend;

public abstract class CollectiveMindsProduct : IDeviceInterfaceGuid
{
	[Flags]
	public enum StrikeMaxFlags
	{
		Remapper = 1,
		BuiltInSlot = 2,
		UserSlots = 4,
		Ps4 = 8,
		Xb1 = 0x10,
		Switch = 0x20,
		ExtraPaddles = 0x40,
		IsPro = 0x80,
		Dominator = 3,
		Original = 3,
		BattlePack = 0x43,
		EliminatorPro = 0x83
	}

	public CollectiveMindsProduct Product { get; set; }

	public abstract StrikeMaxFlags Flags { get; }

	public abstract int UserSlots { get; }

	public abstract int TotalSlots { get; }

	public abstract int FirstSlot { get; }

	public virtual int FPSSlot => -1;

	public string DisplayName { get; }

	public abstract SemVersion MinimumSupportedFirmware { get; }

	public Guid DeviceInterfaceGuid { get; }

	public virtual int GamepackPocSlot => 2;

	protected CollectiveMindsProduct(string deviceInterfaceGuid, string displayName)
	{
		DisplayName = displayName;
		DeviceInterfaceGuid = Guid.Parse(deviceInterfaceGuid);
	}

	public virtual bool Is16Bit(string version)
	{
		return true;
	}

	public virtual bool Is32Bit()
	{
		return false;
	}

	public abstract IEnumerable<BaseConfig> GetBuiltInConfig();

	public virtual bool SupportsAdjustments(string version)
	{
		return false;
	}

	public virtual bool SupportsSlot0Remapper(string version)
	{
		return false;
	}

	public virtual bool ProductHasPaddles(string version)
	{
		return true;
	}

	public virtual bool SupportsSlot1Remapper(string version)
	{
		return false;
	}

	public virtual bool CanReflashFPS()
	{
		return true;
	}

	public virtual bool ProductHasPaddles()
	{
		return true;
	}

	public virtual bool SupportsFpsGamepacks(string version)
	{
		if (App.CurrentDevice.Device.Product.Flags.HasFlag(StrikeMaxFlags.Switch))
		{
			return true;
		}
		return SemVersion.Parse(version) >= "2.0.0-alpha.1";
	}
}
