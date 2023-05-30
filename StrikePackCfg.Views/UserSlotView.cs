using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DfuLib;
using Semver;
using StrikePackCfg.Backend;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.Backend.EEProm;
using StrikePackCfg.Gamepacks;
using StrikePackCfg.HelperClasses;
using StrikePackCfg.Windows;

namespace StrikePackCfg.Views;

public partial class UserSlotView : UserControl, INotifyPropertyChanged, IComponentConnector
{
	[CompilerGenerated]
	private int _003CSlot_003Ek__BackingField;

	private GamepackData _selectedItem;

	private int _gpSize;

	[CompilerGenerated]
	private string _003CDescription_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CHelpUrl_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CSlotEnabled_003Ek__BackingField;

	public int Slot
	{
		[CompilerGenerated]
		get
		{
			return _003CSlot_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CSlot_003Ek__BackingField != value)
			{
				_003CSlot_003Ek__BackingField = value;
				OnPropertyChanged("Mama");
				OnPropertyChanged("Message");
				OnPropertyChanged("Slot");
			}
		}
	}

	public string Mama
	{
		get
		{
			if (Slot != 1)
			{
				return (Slot - 1).ToString();
			}
			return "F";
		}
	}

	public string Message
	{
		get
		{
			if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.Switch))
			{
				if (Slot > 2)
				{
					return (Slot - 2).ToString();
				}
				return "B";
			}
			if (Slot == 1)
			{
				return "F";
			}
			return (Slot - 1).ToString();
		}
	}

	public GamepackData SelectedItem
	{
		get
		{
			return _selectedItem;
		}
		set
		{
			if (object.Equals(_selectedItem, value))
			{
				return;
			}
			Window window = Application.Current.Windows.OfType<Window>().SingleOrDefault((Window w) => w.IsActive);
			bool flag = false;
			if (value != null)
			{
				if (_selectedItem != null && (value.Id == 0 || _selectedItem.Id > 0))
				{
					(window as UserSlotConfigWindow).FlashUsage -= (int)Math.Round((double)(100 * _gpSize) / 131072.0);
					(window as UserSlotConfigWindow).FlashUsage += (int)Math.Round((double)(100 * value.GpackSize) / 131072.0);
					flag = true;
				}
				else
				{
					(window as UserSlotConfigWindow).FlashUsage += (int)Math.Round((double)(100 * value.GpackSize) / 131072.0);
					if (_selectedItem != null && value.GpackSize > 0)
					{
						flag = true;
					}
				}
				if ((window as UserSlotConfigWindow).FlashUsage < 100)
				{
					(window as UserSlotConfigWindow).SaveBtn.IsEnabled = true;
				}
				else
				{
					MessageBox.Show("Flash size limit reached. Please remove last gamepack.", "Warning!");
					(window as UserSlotConfigWindow).SaveBtn.IsEnabled = false;
					flag = false;
				}
				_gpSize = value.GpackSize;
				if (value.Id == 0)
				{
					Description = "Please select a gamepack.";
				}
				else
				{
					HelpUrl = value.GpOnlineHelp;
					if (value.Description != null)
					{
						Description = value.Description;
					}
					else
					{
						Description = "N.A.";
					}
				}
			}
			_selectedItem = value;
			OnPropertyChanged("ConfigEnabled");
			OnPropertyChanged("SelectedItem");
			if (flag)
			{
				Configure_Click(null, null);
			}
		}
	}

	public string Description
	{
		[CompilerGenerated]
		get
		{
			return _003CDescription_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!string.Equals(_003CDescription_003Ek__BackingField, value, StringComparison.Ordinal))
			{
				_003CDescription_003Ek__BackingField = value;
				OnPropertyChanged("Description");
			}
		}
	} = "Please select a gamepack.";


	public string HelpUrl
	{
		[CompilerGenerated]
		get
		{
			return _003CHelpUrl_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!string.Equals(_003CHelpUrl_003Ek__BackingField, value, StringComparison.Ordinal))
			{
				_003CHelpUrl_003Ek__BackingField = value;
				OnPropertyChanged("HelpUrl");
			}
		}
	} = "";


	public bool ConfigEnabled
	{
		get
		{
			if (SelectedItem != null)
			{
				return SelectedItem.Id > 0;
			}
			return false;
		}
	}

	public bool SlotEnabled
	{
		[CompilerGenerated]
		get
		{
			return _003CSlotEnabled_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CSlotEnabled_003Ek__BackingField != value)
			{
				_003CSlotEnabled_003Ek__BackingField = value;
				OnPropertyChanged("SlotEnabled");
			}
		}
	} = true;


	public int GpSize => _gpSize;

	public event PropertyChangedEventHandler PropertyChanged;

	public UserSlotView(int slt)
	{
		InitializeComponent();
		if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.Switch))
		{
			if (slt == 1 || slt == 2)
			{
				Packs.ItemsSource = UserSlotViewModel.Instance.Modpacks;
				Packs.IsEnabled = false;
			}
			else
			{
				Packs.ItemsSource = UserSlotViewModel.Instance.Gamepacks;
			}
		}
		else
		{
			Packs.ItemsSource = ((slt == 1) ? UserSlotViewModel.Instance.Modpacks : UserSlotViewModel.Instance.Gamepacks);
		}
		UserSlotViewModel.Instance.UpdateGamepackList();
		base.DataContext = this;
	}

	protected void OnPropertyChanged(string name)
	{
		PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
		if (propertyChanged != null)
		{
			propertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}

	private async void Configure_Click(object sender, RoutedEventArgs e)
	{
		using (new DisableUIContext())
		{
			if (string.IsNullOrEmpty(SelectedItem.Name))
			{
				return;
			}
			Btn.IsEnabled = false;
			int checkGp = 0;
			using (CmPpDfuDeviceBackend dev = App.CurrentDevice.Device.GetDeviceBackend())
			{
				SlotMetaDataBase meta = Helper.GetMeta(dev, Slot);
				checkGp = meta.GamePackId;
			}
			bool setDefaultsOnLoad = false;
			if (checkGp != SelectedItem.Id && checkGp != 0)
			{
				if (MessageBox.Show("This will erase previous gamepack settings.\n\nAre you sure you want to proceed?", "Gamepack Settings.", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
				{
					Btn.IsEnabled = true;
					return;
				}
				setDefaultsOnLoad = true;
				await Helper.SaveConfig(new KeyValuePair<int, short>[0], Slot, 0, (ushort)checkGp);
			}
			if (checkGp == 0)
			{
				setDefaultsOnLoad = true;
			}
			Tuple<IEnumerable<BaseConfig>, int, SemVersion> tuple = await SelectedItem.FetchConfig();
			await SettingsHandler.HandleSettings(Slot, tuple.Item1, tuple.Item3, (ushort)tuple.Item2, SelectedItem.Name, HelpUrl, setDefaultsOnLoad);
			Btn.IsEnabled = true;
		}
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		if (Uri.IsWellFormedUriString(HelpUrl, UriKind.Absolute))
		{
			Process.Start(new ProcessStartInfo(HelpUrl));
		}
		e.Handled = true;
	}
}
