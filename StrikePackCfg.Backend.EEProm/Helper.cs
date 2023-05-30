using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DfuLib;
using Semver;
using StrikePackCfg.HelperClasses;
using StrikePackCfg.Views;

namespace StrikePackCfg.Backend.EEProm;

public static class Helper
{
	public static DominatorData GetDominatorData(int slot, bool is32Bit)
	{
		using CmPpDfuDeviceBackend cmPpDfuDeviceBackend = App.CurrentDevice.Device.GetDeviceBackend();
		byte[] data;
		try
		{
			data = cmPpDfuDeviceBackend.EfsRead("PMAP", (ushort)(is32Bit ? 20u : 12u));
		}
		catch (Exception innerException)
		{
			throw new Exception($"Attempting to read 'PMAP' from '{App.CurrentDevice.Device.Product.DisplayName}'", innerException);
		}
		PaddlesData paddleConfig;
		try
		{
			paddleConfig = PaddlesData.FromBytes(data, App.CurrentDevice.Device.Is16Bit);
		}
		catch (Exception innerException2)
		{
			throw new Exception($"Attempting to parse 'PMAP' from '{App.CurrentDevice.Device.Product.DisplayName}'", innerException2);
		}
		return new DominatorData(paddleConfig, GetMeta(cmPpDfuDeviceBackend, slot));
	}

	internal static SlotMetaDataBase GetMeta(CmPpDfuDeviceBackend dev, int slot)
	{
		SlotMetaDataBase metaBase = App.CurrentDevice.Device.GetMetaBase();
		string text = "SLT" + slot;
		byte[] data;
		try
		{
			data = dev.EfsRead(text, metaBase.Size);
		}
		catch (Exception innerException)
		{
			throw new Exception($"Attempting to read '{text}' from device '{App.CurrentDevice.Device.Product.DisplayName}'", innerException);
		}
		try
		{
			return SlotMetaDataBase.FromBytes(data, metaBase);
		}
		catch (Exception innerException2)
		{
			throw new Exception($"Attempting to parse '{text}' from device '{App.CurrentDevice.Device.Product.DisplayName}'", innerException2);
		}
	}

	private static SlotMetaDataBase.SlotFlags UpdateFlag(bool state, SlotMetaDataBase.SlotFlags current, SlotMetaDataBase.SlotFlags flag)
	{
		current = ((!state) ? (current & (SlotMetaDataBase.SlotFlags)(~(uint)flag)) : (current | flag));
		return current;
	}

	private static SlotMetaDataBase.SlotExtraFlags UpdateExtraFlag(bool state, SlotMetaDataBase.SlotExtraFlags current, SlotMetaDataBase.SlotExtraFlags flag)
	{
		current = ((!state) ? (current & (SlotMetaDataBase.SlotExtraFlags)(~(uint)flag)) : (current | flag));
		return current;
	}

	internal static void SaveRemapper(CmPpDfuDeviceBackend dev, SlotMetaDataBase cur, SlotView.RemapData data, int slot)
	{
		for (int i = 0; i < data.Buttons.Length && i < cur.Maps.Length; i++)
		{
			cur.Maps[i] = data.Buttons[i];
		}
		cur.Deadzone = data.Deadzone;
		cur.Sensitivity = data.Sensitivity;
		cur.LeftPaddleMask = data.Left;
		cur.RightPaddleMask = data.Right;
		cur.LeftPaddleMask2 = data.Left2;
		cur.RightPaddleMask2 = data.Right2;
		cur.Flags = UpdateFlag(data.Rumble, cur.Flags, SlotMetaDataBase.SlotFlags.Rumble);
		cur.Flags = UpdateFlag(data.InvertLX, cur.Flags, SlotMetaDataBase.SlotFlags.InvertLX);
		cur.Flags = UpdateFlag(data.InvertLY, cur.Flags, SlotMetaDataBase.SlotFlags.InvertLY);
		cur.Flags = UpdateFlag(data.InvertRX, cur.Flags, SlotMetaDataBase.SlotFlags.InvertRX);
		cur.Flags = UpdateFlag(data.InvertRY, cur.Flags, SlotMetaDataBase.SlotFlags.InvertRY);
		cur.Flags |= SlotMetaDataBase.SlotFlags.Remap;
		cur.ExtraFlags = UpdateExtraFlag(data.UseHairTriggerLeft, cur.ExtraFlags, SlotMetaDataBase.SlotExtraFlags.UseHairTriggersLeft);
		cur.ExtraFlags = UpdateExtraFlag(data.UseHairTriggerRight, cur.ExtraFlags, SlotMetaDataBase.SlotExtraFlags.UseHairTriggersRight);
		string slt = "SLT" + slot;
		dev.Execute(delegate
		{
			dev.EfsWrite(slt, cur.ToBytes());
		}, DfuState.DownloadIdle, $"Attempting to write '{slt}' to device '{App.CurrentDevice.Device.Product.DisplayName}'");
	}

	public static async Task SaveConfig(IEnumerable<KeyValuePair<int, short>> pvars, int slot, ushort? ver = null, ushort? id = null, bool overrideFlags = false, bool overrideExtraFlags = false, SlotMetaDataBase.SlotFlags overrideFlagValue = SlotMetaDataBase.SlotFlags.Default | SlotMetaDataBase.SlotFlags.Gpc, SlotMetaDataBase.SlotExtraFlags overrideExtraFlagValue = SlotMetaDataBase.SlotExtraFlags.Default)
	{
		using (new DisableUIContext())
		{
			await Task.Run(delegate
			{
				CmPpDfuDeviceBackend dev = App.CurrentDevice.Device.GetDeviceBackend();
				try
				{
					SlotMetaDataBase cur = GetMeta(dev, slot);
					if (overrideFlags)
					{
						cur.Flags = overrideFlagValue;
					}
					if (overrideExtraFlags)
					{
						cur.ExtraFlags = overrideExtraFlagValue;
					}
					if (ver.HasValue)
					{
						cur.GamePackVersion = ver.Value;
					}
					if (id.HasValue)
					{
						cur.GamePackId = id.Value;
					}
					for (int i = 0; i < cur.Pvars.Length; i++)
					{
						cur.Pvars[i] = 0;
					}
					foreach (KeyValuePair<int, short> pvar in pvars)
					{
						cur.Pvars[pvar.Key] = pvar.Value;
					}
					string slt = "SLT" + slot;
					dev.ClearErrorAndAbort();
					dev.Execute(delegate
					{
						dev.EfsWrite(slt, cur.ToBytes());
					}, DfuState.DownloadIdle, $"Attempting to write '{slt}' to device '{App.CurrentDevice.Device.Product.DisplayName}'");
					dev.Manifest();
				}
				finally
				{
					if (dev != null)
					{
						((IDisposable)dev).Dispose();
					}
				}
			});
		}
	}

	public static void SavePaddles(CmPpDfuDeviceBackend dev, uint left, uint right, uint left2, uint right2, bool is16Bit)
	{
		if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
		{
			dev.Execute(delegate
			{
				dev.EfsWrite("PMAP", new PaddlesData(left, right, left2, right2, is16Bit).ToBytes());
			}, DfuState.DownloadIdle, $"Attempting to write 'PMAP' to device '{App.CurrentDevice.Device.Product.DisplayName}'");
		}
		else
		{
			dev.Execute(delegate
			{
				dev.EfsWrite("PMAP", new PaddlesData(left, right, is16Bit).ToBytes());
			}, DfuState.DownloadIdle, $"Attempting to write 'PMAP' to device '{App.CurrentDevice.Device.Product.DisplayName}'");
		}
	}

	public static IEnumerable<KeyValuePair<int, short>> LoadConfig(int slot, out ushort gamepackId, out SemVersion gamepackVersion)
	{
		using CmPpDfuDeviceBackend dev = App.CurrentDevice.Device.GetDeviceBackend();
		SlotMetaDataBase meta = GetMeta(dev, slot);
		gamepackId = meta.GamePackId;
		gamepackVersion = new SemVersion((meta.GamePackVersion >> 8) & 0xFF, meta.GamePackVersion & 0xFF);
		return meta.Pvars.Select((short t, int i) => new KeyValuePair<int, short>(i, t)).ToList();
	}

	public static void WriteCleanData(CmPpDfuDeviceBackend dev, bool is16Bit)
	{
		dev.Execute(dev.EfsFormat, DfuState.DownloadIdle, $"Attempting to perform Factory Reset on device '{App.CurrentDevice.Device.Product.DisplayName}'");
	}

	public static void EraseEfs(CmPpDfuDeviceBackend dev)
	{
		dev.Execute(dev.EfsFormat, DfuState.DownloadIdle, $"Attempting to perform Factory Reset on device '{App.CurrentDevice.Device.Product.DisplayName}'");
	}
}
