using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Semver;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.HelperClasses;
using StrikePackCfg.Properties;
using StrikePackCfg.Views;

namespace StrikePackCfg.Windows;

public class ConfigWindow : MetroWindow, IComponentConnector
{
	private readonly IEnumerable<ConfigViewBase> _views;

	private readonly IEnumerable<BaseConfig> _cfg;

	private readonly ushort gamepackId;

	private readonly SemVersion gamepackVersion;

	private readonly string gamepackName;

	internal StackPanel Panel;

	private bool _contentLoaded;

	public string HelpUrl { get; set; } = "";


	public ConfigWindow(IEnumerable<BaseConfig> cfg, ushort gamepackId = 0, SemVersion gamepackVersion = null, string gamepackName = "", string helpUrl = "")
	{
		InitializeComponent();
		MainWindow mainWin = App.MainWin;
		mainWin.CloseWindow = (EventHandler)Delegate.Combine(mainWin.CloseWindow, new EventHandler(CloseWindow));
		((Window)this).Closing += delegate
		{
			MainWindow mainWin2 = App.MainWin;
			mainWin2.CloseWindow = (EventHandler)Delegate.Remove(mainWin2.CloseWindow, new EventHandler(CloseWindow));
		};
		_cfg = (cfg as BaseConfig[]) ?? cfg.ToArray();
		_views = ConfigHelper.GetViews(_cfg);
		if (Uri.TryCreate(helpUrl, UriKind.Absolute, out var result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
		{
			HelpUrl = helpUrl;
			Button button = new Button
			{
				Content = $"Open {gamepackName} Online Manual",
				FontSize = 12.0,
				Margin = new Thickness(5.0, 5.0, 5.0, 5.0),
				ToolTip = "Open online help in your browser."
			};
			button.Click += helpBtn_Click;
			Panel.Children.Add(button);
		}
		foreach (ConfigViewBase view in _views)
		{
			Panel.Children.Add(view);
		}
		if (gamepackVersion == null)
		{
			gamepackVersion = App.CurrentDevice.Device.ApplicationVersion;
		}
		this.gamepackVersion = gamepackVersion;
		this.gamepackId = gamepackId;
		this.gamepackName = gamepackName;
		((Window)this).Title = gamepackName + " - " + ((Window)this).Title;
	}

	private void helpBtn_Click(object sender, RoutedEventArgs e)
	{
		Process.Start(new ProcessStartInfo(HelpUrl));
		e.Handled = true;
	}

	private void CloseWindow(object sender, EventArgs eventArgs)
	{
		((Window)this).Close();
	}

	public void SetConfig(IEnumerable<KeyValuePair<int, short>> cfg)
	{
		foreach (KeyValuePair<int, short> item in cfg)
		{
			foreach (ConfigViewBase view in _views)
			{
				if (view.GetId() == item.Key)
				{
					view.SetValue(item.Value);
				}
			}
		}
	}

	public IEnumerable<KeyValuePair<int, short>> GetConfig()
	{
		return _views.Select((ConfigViewBase view) => new KeyValuePair<int, short>(view.GetId(), view.GetValue())).ToList();
	}

	private void Reset_Click(object sender, RoutedEventArgs e)
	{
		if (MessageBox.Show(UI.ResetToDefaultWarningConfig, UI.WarningTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
		{
			SetConfig(ConfigHelper.GetDefaults(_cfg));
		}
	}

	private void Save_Click(object sender, RoutedEventArgs e)
	{
		((Window)this).DialogResult = true;
	}

	private void Export_Click(object sender, RoutedEventArgs e)
	{
		short[] pvars = (from kvp in GetConfig()
			orderby kvp.Key
			select kvp.Value).ToArray();
		ConfigExportData data = new ConfigExportData(gamepackId, gamepackVersion, pvars);
		SaveFileDialog saveFileDialog = new SaveFileDialog
		{
			FileName = gamepackName,
			AddExtension = true,
			DefaultExt = "spcc",
			Filter = UI.spcfgFilter,
			Title = string.Format(UI.SelectSaveLocationConfigFor, gamepackName)
		};
		if (saveFileDialog.ShowDialog() != true)
		{
			return;
		}
		using Stream stream = saveFileDialog.OpenFile();
		data.Save(stream);
		MessageBox.Show(UI.ConfigExportSuccessful, UI.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
	}

	private void Import_Click(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			FileName = gamepackName + ".spcc",
			Filter = UI.spcfgFilter,
			Title = string.Format(UI.SelectExportedConfigFor, gamepackName)
		};
		if (openFileDialog.ShowDialog() != true)
		{
			return;
		}
		using Stream stream = openFileDialog.OpenFile();
		try
		{
			ConfigExportData configExportData = ConfigExportHelper.Load(stream);
			if (configExportData.GamepackId != gamepackId)
			{
				MessageBox.Show(UI.WrongConfigGamepackID, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			if (configExportData.GamepackVersion != gamepackVersion)
			{
				MessageBox.Show(UI.ConfigDifferentVersion, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			SetConfig(configExportData.Pvars.ToDictionary());
			MessageBox.Show(UI.ConfigImportSuccessful, UI.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
		}
		catch (InvalidDataException)
		{
			MessageBox.Show(UI.ConfigCorrupt, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
		}
		catch (NotSupportedException)
		{
			MessageBox.Show(UI.ConfigIncompatibleFileFormat, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
		}
	}

	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/ModCentral;component/windows/configwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((Button)target).Click += Reset_Click;
			break;
		case 2:
			((Button)target).Click += Save_Click;
			break;
		case 3:
			((Button)target).Click += Export_Click;
			break;
		case 4:
			((Button)target).Click += Import_Click;
			break;
		case 5:
			Panel = (StackPanel)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
