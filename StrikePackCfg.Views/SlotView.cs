using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CollectiveMinds.Common.Azure;
using DfuLib;
using Semver;
using StrikePackCfg.Backend;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.Backend.EEProm;
using StrikePackCfg.HelperClasses;
using StrikePackCfg.Properties;
using StrikePackCfg.Windows;

namespace StrikePackCfg.Views;

public partial class SlotView : UserControl, IComponentConnector, INotifyPropertyChanged
{
	internal class RemapData
	{
		public readonly SlotMetaDataBase.AdjustmentData Deadzone = new SlotMetaDataBase.AdjustmentData();

		public readonly SlotMetaDataBase.AdjustmentData Sensitivity = new SlotMetaDataBase.AdjustmentData();

		public readonly int Slot;

		public byte[] Buttons;

		public bool InvertLX;

		public bool InvertLY;

		public bool InvertRX;

		public bool InvertRY;

		public uint Left;

		public bool Mirror;

		public uint Left2;

		public uint Right;

		public uint Right2;

		public bool Rumble;

		public bool UseHairTriggerLeft;

		public bool UseHairTriggerRight;

		public RemapData(int slot, RemapperViewBase view)
		{
			Slot = slot;
			Deadzone.LT = (sbyte)Math.Round(view.LeftTriggerDeadzone);
			Deadzone.LX = (sbyte)Math.Round(view.LXDeadzone);
			Deadzone.LY = (sbyte)Math.Round(view.LYDeadzone);
			Deadzone.RT = (sbyte)Math.Round(view.RightTriggerDeadzone);
			Deadzone.RX = (sbyte)Math.Round(view.RXDeadzone);
			Deadzone.RY = (sbyte)Math.Round(view.RYDeadzone);
			Sensitivity.LT = (sbyte)Math.Round(view.LeftTriggerSensitivity);
			Sensitivity.LX = (sbyte)Math.Round(view.LXSensitivity);
			Sensitivity.LY = (sbyte)Math.Round(view.LYSensitivity);
			Sensitivity.RT = (sbyte)Math.Round(view.RightTriggerSensitivity);
			Sensitivity.RX = (sbyte)Math.Round(view.RXSensitivity);
			Sensitivity.RY = (sbyte)Math.Round(view.RYSensitivity);
			InvertLX = view.InvertLX;
			InvertLY = view.InvertLY;
			InvertRX = view.InvertRX;
			InvertRY = view.InvertRY;
			UseHairTriggerLeft = view.UseHairTriggersLeft;
			UseHairTriggerRight = view.UseHairTriggersRight;
		}
	}

	[CompilerGenerated]
	private int _003CSlot_003Ek__BackingField;

	public bool isOpened;

	[CompilerGenerated]
	private string _003CHeader_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CRemapTitle_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CHasConfig_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CNews_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CHasGamepacks_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CHasGears_003Ek__BackingField;

	public UserSlotConfigWindow FlashDialog;

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
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.Slot);
			}
		}
	}

	public string Header
	{
		[CompilerGenerated]
		get
		{
			return _003CHeader_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!string.Equals(_003CHeader_003Ek__BackingField, value, StringComparison.Ordinal))
			{
				_003CHeader_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.Header);
			}
		}
	} = "Default/Tournament";


	public string RemapTitle
	{
		[CompilerGenerated]
		get
		{
			return _003CRemapTitle_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!string.Equals(_003CRemapTitle_003Ek__BackingField, value, StringComparison.Ordinal))
			{
				_003CRemapTitle_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.RemapTitle);
			}
		}
	} = "Remapper";


	public bool HasConfig
	{
		[CompilerGenerated]
		get
		{
			return _003CHasConfig_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CHasConfig_003Ek__BackingField != value)
			{
				_003CHasConfig_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.HasConfig);
			}
		}
	} = true;


	public int News
	{
		[CompilerGenerated]
		get
		{
			return _003CNews_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CNews_003Ek__BackingField != value)
			{
				_003CNews_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.News);
			}
		}
	}

	public bool HasGamepacks
	{
		[CompilerGenerated]
		get
		{
			return _003CHasGamepacks_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CHasGamepacks_003Ek__BackingField != value)
			{
				_003CHasGamepacks_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.HasGamepacks);
			}
		}
	}

	public bool HasGears
	{
		[CompilerGenerated]
		get
		{
			return _003CHasGears_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CHasGears_003Ek__BackingField != value)
			{
				_003CHasGears_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.HasGears);
			}
		}
	}

	[field: NonSerialized]
	public event PropertyChangedEventHandler PropertyChanged;

	public SlotView()
	{
		InitializeComponent();
		base.DataContext = this;
	}

	private async void Remapper_Click(object sender, RoutedEventArgs e)
	{
		using (new DisableUIContext())
		{
			DominatorData cur = default(DominatorData);
			try
			{
				_ = cur;
				cur = await Task.Run(() => Helper.GetDominatorData(Slot, App.CurrentDevice.Device.Is32Bit));
			}
			catch (Exception exception)
			{
				TelemetryHelper.TrackException(exception);
				MessageBox.Show(UI.RemapperReadError, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			RemapperViewBase remapperViewBase = OpenRemapper(cur);
			if (remapperViewBase == null)
			{
				return;
			}
			RemapData data = remapperViewBase.GetRemapperData((Slot != 0) ? App.CurrentDevice.Device.Product.FPSSlot : 0);
			try
			{
				await Task.Run(delegate
				{
					using CmPpDfuDeviceBackend cmPpDfuDeviceBackend = App.CurrentDevice.Device.GetDeviceBackend();
					SlotMetaDataBase meta = Helper.GetMeta(cmPpDfuDeviceBackend, (data.Slot == 0) ? 1 : 0);
					cmPpDfuDeviceBackend.ClearErrorAndAbort();
					Helper.SavePaddles(cmPpDfuDeviceBackend, data.Left, data.Right, data.Left2, data.Right2, App.CurrentDevice.Device.Is16Bit);
					Helper.SaveRemapper(cmPpDfuDeviceBackend, cur.SlotData, data, data.Slot);
					if (data.Mirror)
					{
						for (int i = 1; i < 10; i++)
						{
							meta = Helper.GetMeta(cmPpDfuDeviceBackend, i);
							meta.Deadzone = data.Deadzone;
							meta.Sensitivity = data.Sensitivity;
							if (data.InvertRX)
							{
								meta.Flags |= SlotMetaDataBase.SlotFlags.InvertRX;
							}
							if (data.InvertLX)
							{
								meta.Flags |= SlotMetaDataBase.SlotFlags.InvertLX;
							}
							if (data.InvertLY)
							{
								meta.Flags |= SlotMetaDataBase.SlotFlags.InvertLY;
							}
							if (data.InvertRY)
							{
								meta.Flags |= SlotMetaDataBase.SlotFlags.InvertRY;
							}
							if (data.UseHairTriggerLeft)
							{
								meta.ExtraFlags |= SlotMetaDataBase.SlotExtraFlags.UseHairTriggersLeft;
							}
							if (data.UseHairTriggerRight)
							{
								meta.ExtraFlags |= SlotMetaDataBase.SlotExtraFlags.UseHairTriggersRight;
							}
							data.Buttons = meta.Maps;
							Helper.SaveRemapper(cmPpDfuDeviceBackend, meta, data, i);
						}
					}
					else
					{
						for (int j = 1; j < App.CurrentDevice.Device.Product.UserSlots; j++)
						{
							meta = Helper.GetMeta(cmPpDfuDeviceBackend, j);
							data.Deadzone.L2 = 0;
							data.Deadzone.R2 = 0;
							data.Deadzone.LX = 0;
							data.Deadzone.LY = 0;
							data.Deadzone.RT = 0;
							data.Deadzone.LT = 0;
							data.Deadzone.RY = 0;
							data.Deadzone.RX = 0;
							data.Sensitivity.L2 = 0;
							data.Sensitivity.LT = 0;
							data.Sensitivity.LX = 0;
							data.Sensitivity.LY = 0;
							data.Sensitivity.R2 = 0;
							data.Sensitivity.RT = 0;
							data.Sensitivity.LT = 0;
							data.Sensitivity.RY = 0;
							data.InvertLX = false;
							data.InvertLY = false;
							data.InvertRX = false;
							data.InvertRY = false;
							data.Rumble = false;
							meta.Flags = SlotMetaDataBase.SlotFlags.Default | SlotMetaDataBase.SlotFlags.Gpc;
							meta.ExtraFlags = SlotMetaDataBase.SlotExtraFlags.Default;
							for (byte b = 0; b < data.Buttons.Length; b = (byte)(b + 1))
							{
								data.Buttons[b] = b;
							}
							Helper.SaveRemapper(cmPpDfuDeviceBackend, meta, data, j);
						}
					}
					cmPpDfuDeviceBackend.Manifest();
				});
				MessageBox.Show("Successfully saved your remapper settings", UI.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			catch (Exception exception2)
			{
				TelemetryHelper.TrackException(exception2);
				MessageBox.Show(UI.RemapperWriteError, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
	}

	private async void Gear_Click(object sender, RoutedEventArgs e)
	{
		if (App.CurrentDevice.Device.HasSubscription)
		{
			return;
		}
		using (new DisableUIContext())
		{
			ushort id = 0;
			int slot = 1;
			SemVersion ver = null;
			string txt = "FPS CODE STANDARD";
			IEnumerable<BaseConfig> cfg;
			if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.IsPro))
			{
				cfg = ConfigHelper.GetXb1ConfigPro();
				txt = "GADGETS ADJUSTMENTS";
				slot = 0;
			}
			else
			{
				cfg = ConfigHelper.GetXb1Config();
			}
			IEnumerable<KeyValuePair<int, short>> cur;
			try
			{
				cur = await Task.Run(() => Helper.LoadConfig(slot, out id, out ver));
			}
			catch (Exception exception)
			{
				TelemetryHelper.TrackException(exception);
				MessageBox.Show(UI.ConfigReadError, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			ConfigWindow configWindow = new ConfigWindow(cfg, id, ver, txt)
			{
				Owner = (Window)(object)App.MainWin
			};
			configWindow.SetConfig(cur);
			if (((Window)(object)configWindow).ShowDialog() != true)
			{
				return;
			}
			try
			{
				await Helper.SaveConfig(configWindow.GetConfig(), slot);
				MessageBox.Show("Successfully saved your settings", UI.SuccessTitle, MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			catch (Exception exception2)
			{
				TelemetryHelper.TrackException(exception2);
				MessageBox.Show(UI.ConfigWriteError, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
	}

	private void Mods_Click(object sender, RoutedEventArgs e)
	{
		using (new DisableUIContext())
		{
			if (FlashDialog != null)
			{
				try
				{
					((Window)(object)FlashDialog).Show();
					return;
				}
				catch (Exception)
				{
					FlashDialog = new UserSlotConfigWindow();
					((Window)(object)FlashDialog).ShowDialog();
					return;
				}
			}
			FlashDialog = new UserSlotConfigWindow();
			((Window)(object)FlashDialog).ShowDialog();
		}
	}

	private RemapperViewBase OpenRemapper(DominatorData cur)
	{
		RemapperViewBase remapperViewBase = null;
		if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.Ps4))
		{
			remapperViewBase = new Ps4RemapperView();
		}
		else if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.Xb1))
		{
			remapperViewBase = new Xb1RemapperView();
		}
		else if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.Switch))
		{
			remapperViewBase = new SwitchRemapperView();
		}
		if (remapperViewBase != null)
		{
			remapperViewBase.SetButtons(cur);
			remapperViewBase.RemapEnabled = Slot != 1 || App.CurrentDevice.Device.SupportsSlot1Remapper;
			remapperViewBase.MirrorConfigurationEnabled = true;
			using (CmPpDfuDeviceBackend dev = App.CurrentDevice.Device.GetDeviceBackend())
			{
				SlotMetaDataBase meta = Helper.GetMeta(dev, 0);
				SlotMetaDataBase meta2 = Helper.GetMeta(dev, 1);
				byte[] first = new byte[6];
				if ((!first.SequenceEqual(meta2.Sensitivity.ToBytes()) || !first.SequenceEqual(meta2.Deadzone.ToBytes())) && meta.Sensitivity.ToBytes().SequenceEqual(meta2.Sensitivity.ToBytes()) && meta.Deadzone.ToBytes().SequenceEqual(meta2.Deadzone.ToBytes()))
				{
					remapperViewBase.MirrorConfigurationChecked = true;
				}
				if ((meta2.Flags & SlotMetaDataBase.SlotFlags.Rumble) == SlotMetaDataBase.SlotFlags.Rumble)
				{
					remapperViewBase.MirrorConfigurationChecked = true;
				}
				if ((meta2.Flags & SlotMetaDataBase.SlotFlags.InvertLX) == SlotMetaDataBase.SlotFlags.InvertLX)
				{
					remapperViewBase.MirrorConfigurationChecked = true;
				}
				if ((meta2.Flags & SlotMetaDataBase.SlotFlags.InvertRX) == SlotMetaDataBase.SlotFlags.InvertRX)
				{
					remapperViewBase.MirrorConfigurationChecked = true;
				}
				if ((meta2.Flags & SlotMetaDataBase.SlotFlags.InvertLY) == SlotMetaDataBase.SlotFlags.InvertLY)
				{
					remapperViewBase.MirrorConfigurationChecked = true;
				}
				if ((meta2.Flags & SlotMetaDataBase.SlotFlags.InvertRY) == SlotMetaDataBase.SlotFlags.InvertRY)
				{
					remapperViewBase.MirrorConfigurationChecked = true;
				}
			}
			if (remapperViewBase.MirrorConfigurationChecked)
			{
				MessageBox.Show("Some advanced configurations are mirrored to all slots.\n\nPlease review them before saving.", "Advanced configuration", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			}
			if (((Window)(object)new RemapperWindow(remapperViewBase, RemapTitle)).ShowDialog() != true)
			{
				return null;
			}
			return remapperViewBase;
		}
		return null;
	}

	private async void Config_Click(object sender, RoutedEventArgs e)
	{
		using (new DisableUIContext())
		{
			await SettingsHandler.HandleFpsSettings(App.CurrentDevice.Device.Product.FPSSlot);
		}
	}

	private void News_Click(object sender, RoutedEventArgs e)
	{
		((Window)(object)new News()).ShowDialog();
	}

	protected void _003C_003EOnPropertyChanged(PropertyChangedEventArgs eventArgs)
	{
		this.PropertyChanged?.Invoke(this, eventArgs);
	}
}
