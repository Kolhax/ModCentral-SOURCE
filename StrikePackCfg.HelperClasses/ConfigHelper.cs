using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using CollectiveMinds.Commmon.Network;
using CollectiveMinds.Common.Azure;
using IniParser.Model;
using IniParser.Parser;
using Semver;
using StrikePackCfg.Backend;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.Views;

namespace StrikePackCfg.HelperClasses;

public static class ConfigHelper
{
	private static string staticCfgUrl = "http://gp.ctrlmaxplus.ca/Packages/ModPacks/Configs/PS4Dominator.cfg";

	private static IEnumerable<BaseConfig> _xb1Config;

	private static IEnumerable<BaseConfig> _ps4Config;

	internal static SemVersion Xb1ConfigVer { get; private set; } = "0.0";


	internal static SemVersion Ps4ConfigVer { get; private set; } = "0.0";


	internal static Tuple<IEnumerable<BaseConfig>, SemVersion> ParseIniConfig(string data, string name)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		List<BaseConfig> list = new List<BaseConfig>();
		SemVersion item = "0.0";
		try
		{
			IniData val = new IniDataParser().Parse(data);
			if (val.Global.ContainsKey("version"))
			{
				item = val.Global["version"];
			}
			foreach (SectionData section in val.Sections)
			{
				try
				{
					byte id = Convert.ToByte(section.SectionName.Substring(section.SectionName.IndexOf(':') + 1));
					int num = Convert.ToInt32(section.Keys["options"]);
					if (num > 1)
					{
						string text = section.Keys["ep"];
						if (text != null && !App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
						{
							if (text == "true")
							{
								continue;
							}
							List<string> ep = section.Keys["ep"].Split(',').ToList();
							if (ep != null)
							{
								for (int i = 0; i < ep.Count; i++)
								{
									if (ep[i].Length > 1)
									{
										ep[i] = "opt000" + ep[i];
									}
									else
									{
										ep[i] = "opt0000" + ep[i];
									}
								}
								num -= ep.Count;
							}
							list.Add(new MultiChoiceConfig(id, section.Keys["name"], num, section.Keys["flag"], section.Keys["help"], section.Keys["ep"], (from key in (IEnumerable<KeyData>)section.Keys
								where key.KeyName.StartsWith("opt")
								where key.KeyName != "options"
								where key.KeyName != "opt0002"
								where !ep.Contains(key.KeyName)
								select new KeyValuePair<int, string>(Convert.ToInt32(key.KeyName.Substring(3)), key.Value)).ToArray()));
						}
						else
						{
							list.Add(new MultiChoiceConfig(id, section.Keys["name"], num, section.Keys["flag"], section.Keys["help"], section.Keys["ep"], (from key in (IEnumerable<KeyData>)section.Keys
								where key.KeyName.StartsWith("opt")
								where key.KeyName != "options"
								where key.KeyName != "opt0002"
								select new KeyValuePair<int, string>(Convert.ToInt32(key.KeyName.Substring(3)), key.Value)).ToArray()));
						}
						continue;
					}
					int defaultValue = Convert.ToInt32(section.Keys["default"]);
					int minimumValue = Convert.ToInt32(section.Keys["minimum"]);
					int maximumValue = Convert.ToInt32(section.Keys["maximum"]);
					int step = Convert.ToInt32(section.Keys["step"]);
					if (section.Keys["ep"] == "true")
					{
						if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
						{
							list.Add(new RangeConfig(id, section.Keys["name"], num, section.Keys["flag"], section.Keys["help"], section.Keys["ep"], defaultValue, minimumValue, maximumValue, step));
						}
					}
					else
					{
						list.Add(new RangeConfig(id, section.Keys["name"], num, section.Keys["flag"], section.Keys["help"], section.Keys["ep"], defaultValue, minimumValue, maximumValue, step));
					}
				}
				catch (Exception innerException)
				{
					TelemetryHelper.TrackException(new Exception($"Failed parsing section '{section.SectionName}' of the config for '{name}'", innerException));
				}
			}
		}
		catch (Exception innerException2)
		{
			TelemetryHelper.TrackException(new Exception($"Failed parsing the config for '{name}'", innerException2));
		}
		return new Tuple<IEnumerable<BaseConfig>, SemVersion>(list, item);
	}

	private static IEnumerable<BaseConfig> GetLocalized(string data, ResourceManager res, string name, out SemVersion ver)
	{
		List<BaseConfig> list = new List<BaseConfig>();
		Tuple<IEnumerable<BaseConfig>, SemVersion> tuple = ParseIniConfig(data, name);
		ver = tuple.Item2;
		foreach (BaseConfig item in tuple.Item1)
		{
			if (item is RangeConfig)
			{
				RangeConfig rangeConfig = item as RangeConfig;
				list.Add(new RangeConfig(rangeConfig, res.GetString(rangeConfig.Name)));
			}
			else if (item is MultiChoiceConfig)
			{
				MultiChoiceConfig multiChoiceConfig = item as MultiChoiceConfig;
				list.Add(new MultiChoiceConfig(multiChoiceConfig.Id, res.GetString(multiChoiceConfig.Name), multiChoiceConfig.Options, multiChoiceConfig.Flag, multiChoiceConfig.Help, multiChoiceConfig.Ep, multiChoiceConfig.OptionsList.Select((KeyValuePair<int, string> opt) => new KeyValuePair<int, string>(opt.Key, res.GetString(opt.Value))).ToArray()));
			}
			else
			{
				TelemetryHelper.TrackException(new InvalidOperationException($"Config type '{item.GetType().Name}' not handled"), null, null, throwOnDebugger: true);
			}
		}
		return list;
	}

	public static IEnumerable<BaseConfig> GetXb1Config()
	{
		if (_xb1Config == null)
		{
			try
			{
				using Stream stream = Assembly.GetAssembly(typeof(ConfigHelper)).GetManifestResourceStream(typeof(XB1Dominator), "XB1Dominator.cfg");
				using StreamReader streamReader = new StreamReader(stream);
				_xb1Config = GetLocalized(streamReader.ReadToEnd(), XB1Dominator.ResourceManager, "FPS Dominator", out var ver);
				Xb1ConfigVer = ver;
			}
			catch (Exception exception)
			{
				TelemetryHelper.TrackException(exception);
				return null;
			}
		}
		return _xb1Config;
	}

	public static IEnumerable<BaseConfig> GetXb1ConfigPro()
	{
		if (_xb1Config == null)
		{
			try
			{
				using Stream stream = Assembly.GetAssembly(typeof(ConfigHelper)).GetManifestResourceStream(typeof(EliminatorProPack), "EliminatorProPack.cfg");
				using StreamReader streamReader = new StreamReader(stream);
				_xb1Config = GetLocalized(streamReader.ReadToEnd(), EliminatorProPack.ResourceManager, "Eliminator Pro", out var ver);
				Xb1ConfigVer = ver;
			}
			catch (Exception exception)
			{
				TelemetryHelper.TrackException(exception);
				return null;
			}
		}
		return _xb1Config;
	}

	public static IEnumerable<BaseConfig> GetPs4Config()
	{
		if (_ps4Config == null)
		{
			try
			{
				using MemoryStream stream = new MemoryStream(new WebClient().DownloadData(staticCfgUrl));
				using StreamReader streamReader = new StreamReader(stream);
				_ps4Config = GetLocalized(streamReader.ReadToEnd(), PS4Dominator.ResourceManager, "PS4 FPS Dominator", out var ver);
				Ps4ConfigVer = ver;
			}
			catch (Exception exception)
			{
				TelemetryHelper.TrackException(exception);
				return null;
			}
		}
		return _ps4Config;
	}

	public static IEnumerable<ConfigViewBase> GetViews(IEnumerable<BaseConfig> cfg)
	{
		List<ConfigViewBase> list = new List<ConfigViewBase>();
		foreach (BaseConfig item in cfg)
		{
			ConfigViewBase configViewBase;
			if (item is RangeConfig)
			{
				configViewBase = new RangeConfigView(item as RangeConfig);
			}
			else
			{
				if (!(item is MultiChoiceConfig))
				{
					throw new InvalidOperationException("Unsupported BaseConfig type...");
				}
				configViewBase = new MultiChoiceConfigView(item as MultiChoiceConfig);
			}
			if (item.Indent)
			{
				configViewBase.Margin = new Thickness(30.0, 5.0, 0.0, 5.0);
			}
			list.Add(configViewBase);
		}
		return list;
	}

	public static IEnumerable<KeyValuePair<int, short>> GetDefaults(IEnumerable<BaseConfig> cfg)
	{
		List<KeyValuePair<int, short>> list = new List<KeyValuePair<int, short>>();
		foreach (BaseConfig item in cfg)
		{
			if (item is RangeConfig)
			{
				list.Add(new KeyValuePair<int, short>(item.Id, (short)((RangeConfig)item).DefaultValue));
				continue;
			}
			if (item is MultiChoiceConfig)
			{
				list.Add(((item as MultiChoiceConfig).OptionsList.FirstOrDefault().Key > 0) ? new KeyValuePair<int, short>(item.Id, (short)(item as MultiChoiceConfig).OptionsList.FirstOrDefault().Key) : new KeyValuePair<int, short>(item.Id, 0));
				continue;
			}
			throw new InvalidOperationException("Unsupported BaseConfig type...");
		}
		return list;
	}

	public static async Task<Tuple<IEnumerable<BaseConfig>, SemVersion>> FetchConfig(int id, string name)
	{
		string d = "#" + App.CurrentDevice.Device.SerialNumber + "#" + id + "#" + App.CurrentDevice.Device.Platform;
		return ParseIniConfig(await NetworkHelper.FetchString("http://gp.ctrlmaxplus.ca/getcfg.php?d=" + XXTEA.Urldata(d)), name);
	}
}
