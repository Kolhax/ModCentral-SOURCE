using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CollectiveMinds.Common.Azure;
using Semver;
using StrikePackCfg.Backend;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.Backend.EEProm;
using StrikePackCfg.Properties;
using StrikePackCfg.Windows;

namespace StrikePackCfg.HelperClasses;

internal static class SettingsHandler
{
	public static async Task HandleFpsSettings(int slot)
	{
		IEnumerable<BaseConfig> cfg = null;
		SemVersion overrideVersion = null;
		string overrideName = null;
		if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.Xb1))
		{
			cfg = ConfigHelper.GetXb1Config();
			overrideVersion = ConfigHelper.Xb1ConfigVer;
		}
		else if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.Ps4))
		{
			cfg = ConfigHelper.GetPs4Config();
			overrideVersion = ConfigHelper.Ps4ConfigVer;
		}
		else if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.Switch))
		{
			cfg = ConfigHelper.GetXb1Config();
			overrideVersion = ConfigHelper.Xb1ConfigVer;
		}
		await HandleSettings(slot, cfg, overrideVersion, 0, overrideName);
	}

	public static async Task HandleSettings(int slot, IEnumerable<BaseConfig> cfg, SemVersion overrideVersion = null, ushort overrideId = 0, string overrideName = null, string helpUrl = "", bool setDefault = false)
	{
		ushort id = 0;
		string name = null;
		SemVersion ver = null;
		foreach (Window window4 in Application.Current.Windows)
		{
			if (window4.GetType() == typeof(UserSlotConfigWindow))
			{
				((UserSlotConfigWindow)(object)window4).SaveBtn.IsEnabled = false;
			}
		}
		IEnumerable<KeyValuePair<int, short>> cur;
		try
		{
			cur = await Task.Run(() => Helper.LoadConfig(slot, out id, out ver));
			if (overrideId != 0)
			{
				id = overrideId;
			}
			if (overrideName != "")
			{
				name = overrideName;
			}
			if (overrideVersion != null)
			{
				ver = overrideVersion;
			}
		}
		catch (Exception exception)
		{
			TelemetryHelper.TrackException(exception);
			MessageBox.Show(UI.ConfigReadError, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
			return;
		}
		ConfigWindow configWindow = new ConfigWindow(cfg, id, ver, name, helpUrl)
		{
			Owner = (Window)(object)App.MainWin
		};
		if (setDefault)
		{
			configWindow.SetConfig(ConfigHelper.GetDefaults(cfg));
		}
		else
		{
			configWindow.SetConfig(cur);
		}
		if (((Window)(object)configWindow).ShowDialog() != true)
		{
			foreach (Window window5 in Application.Current.Windows)
			{
				if (window5.GetType() == typeof(UserSlotConfigWindow))
				{
					(window5 as UserSlotConfigWindow).SaveBtn.IsEnabled = true;
				}
			}
			return;
		}
		try
		{
			await Helper.SaveConfig(configWindow.GetConfig(), slot, 0, overrideId);
			MessageBox.Show(UI.SuccessfullySavedYourSettings, UI.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
			foreach (Window window6 in Application.Current.Windows)
			{
				if (window6.GetType() == typeof(UserSlotConfigWindow))
				{
					(window6 as UserSlotConfigWindow).SaveBtn.IsEnabled = true;
				}
			}
		}
		catch (Exception exception2)
		{
			TelemetryHelper.TrackException(exception2);
			MessageBox.Show(UI.ConfigWriteError, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}
}
