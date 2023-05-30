using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CollectiveMinds.Common.Azure;
using Microsoft.Win32;
using SPGamepack;
using StrikePackCfg.Backend;
using StrikePackCfg.Backend.EEProm;
using StrikePackCfg.Properties;
using StrikePackCfg.Windows;

namespace StrikePackCfg.HelperClasses;

internal static class GamepackPackageHelper
{
	private static GamepackPackage LoadPackage(string path)
	{
		using FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		return new GamepackPackage(GamepackFile.FromStream(stream).GamepackData);
	}

	public static GamepackPackage FlashGamepackPackage(int id, string serial)
	{
		string d = "#" + App.CurrentDevice.Device.SerialNumber + "#" + id + "#" + App.CurrentDevice.Device.Platform;
		HttpWebRequest obj = (HttpWebRequest)WebRequest.Create($"http://gp.ctrlmaxplus.ca/getmod_aes2.php?d={XXTEA.Urldata(d)}");
		obj.UserAgent = "StrikePackCFG";
		using Stream stream = obj.GetResponse().GetResponseStream();
		return new GamepackPackage(GamepackFile.FromStream(stream).GamepackData);
	}

	public static async Task HandleGamepackPackage(int slot)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Title = "Select Gamepack Package to load...",
			FileName = "Gamepack.spgp"
		};
		if (openFileDialog.ShowDialog() != true)
		{
			return;
		}
		try
		{
			GamepackPackage pkg = LoadPackage(openFileDialog.FileName);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Successfully Loaded Gamepack Package, this information was inside:");
			stringBuilder.AppendLine($"Name:         {pkg.Name}");
			stringBuilder.AppendLine($"Version:      {pkg.Version}");
			stringBuilder.AppendLine(string.Format("Encrypted:    {0}", pkg.IsEncrypted ? "Yes" : "No"));
			stringBuilder.AppendLine(string.Format("Deprecated: {0}", pkg.Deprecated ? "-" : "-"));
			stringBuilder.AppendLine(string.Format("Have CFG:     {0}", (pkg.Config != null) ? "Yes" : "No"));
			if (pkg.CfgVersion != null)
			{
				stringBuilder.AppendLine($"CFG Version:  {pkg.CfgVersion}");
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("Do you wish to continue?");
			if (MessageBox.Show(stringBuilder.ToString(), UI.SuccessTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk) != MessageBoxResult.Yes)
			{
				return;
			}
			ushort ver = 0;
			if (pkg.CfgVersion != null)
			{
				ver = (ushort)((pkg.CfgVersion.Major << 8) | pkg.CfgVersion.Minor);
			}
			if (pkg.Config == null)
			{
				FlashHelper.WriteToFlash(pkg.Bytecode, slot, pkg.IsEncrypted);
				await Helper.SaveConfig(new KeyValuePair<int, short>[0], slot, ver, pkg.ID, overrideFlags: true);
			}
			else
			{
				ConfigWindow configWindow = new ConfigWindow(pkg.Config, pkg.ID, pkg.CfgVersion, pkg.Name);
				if (((Window)(object)configWindow).ShowDialog() != true)
				{
					MessageBox.Show("You cancelled the gamepack package flashing, you must click on the save button to flash!", "Cancelled operation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
				FlashHelper.WriteToFlash(pkg.Bytecode, slot, pkg.IsEncrypted);
				await Helper.SaveConfig(configWindow.GetConfig(), slot, ver, pkg.ID, overrideFlags: true);
			}
			MessageBox.Show("Successfully flashed Gamepack Package!", UI.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
		catch (Exception exception)
		{
			TelemetryHelper.TrackException(exception);
			MessageBox.Show("Failed to flash the Gamepack Package. Please reconnect your Strikepack and try again.", UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}
}
