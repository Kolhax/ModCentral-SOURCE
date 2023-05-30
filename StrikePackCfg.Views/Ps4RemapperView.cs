using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using StrikePackCfg.Backend;
using StrikePackCfg.Backend.EEProm;
using StrikePackCfg.HelperClasses;
using StrikePackCfg.Properties;

namespace StrikePackCfg.Views;

public partial class Ps4RemapperView : RemapperViewBase, INotifyPropertyChanged, IComponentConnector
{
	private readonly IDictionary<byte, string> _buttons = RemapHelper.GetPs4Buttons();

	private readonly IDictionary<uint, string> _paddleButtons = RemapHelper.GetPs4Paddles();

	private readonly IDictionary<byte, string> _sticks = RemapHelper.GetPs4Sticks();

	private byte[] _mappings = new byte[30];

	private KeyValuePair<uint, string> _pl;

	private KeyValuePair<uint, string> _pr;

	private KeyValuePair<uint, string> _pl2;

	private KeyValuePair<uint, string> _pr2;

	private bool _updating;

	public ReadOnlyDictionary<byte, string> Buttons => new ReadOnlyDictionary<byte, string>(_buttons);

	public ReadOnlyDictionary<byte, string> Sticks => new ReadOnlyDictionary<byte, string>(_sticks);

	public ReadOnlyDictionary<uint, string> PaddleButtons => new ReadOnlyDictionary<uint, string>(_paddleButtons);

	public KeyValuePair<byte, string> PS4_PS
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[0]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_PS, value))
			{
				SwapButton(0, value.Key);
				OnPropertyChanged("PS4_PS");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_SHARE
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[1]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_SHARE, value))
			{
				SwapButton(1, value.Key);
				OnPropertyChanged("PS4_SHARE");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_OPTIONS
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[2]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_OPTIONS, value))
			{
				SwapButton(2, value.Key);
				OnPropertyChanged("PS4_OPTIONS");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_LX
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[11]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_LX, value))
			{
				SwapButton(11, value.Key);
				OnPropertyChanged("PS4_LX");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_LY
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[12]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_LY, value))
			{
				SwapButton(12, value.Key);
				OnPropertyChanged("PS4_LY");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_RX
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[9]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_RX, value))
			{
				SwapButton(9, value.Key);
				OnPropertyChanged("PS4_RX");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_RY
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[10]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_RY, value))
			{
				SwapButton(10, value.Key);
				OnPropertyChanged("PS4_RY");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_L1
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[6]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_L1, value))
			{
				SwapButton(6, value.Key);
				OnPropertyChanged("PS4_L1");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_L2
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[7]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_L2, value))
			{
				SwapButton(7, value.Key);
				OnPropertyChanged("PS4_L2");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_R1
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[3]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_R1, value))
			{
				SwapButton(3, value.Key);
				OnPropertyChanged("PS4_R1");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_R2
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[4]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_R2, value))
			{
				SwapButton(4, value.Key);
				OnPropertyChanged("PS4_R2");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_L3
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[8]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_L3, value))
			{
				SwapButton(8, value.Key);
				OnPropertyChanged("PS4_L3");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_R3
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[5]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_R3, value))
			{
				SwapButton(5, value.Key);
				OnPropertyChanged("PS4_R3");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_UP
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[13]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_UP, value))
			{
				SwapButton(13, value.Key);
				OnPropertyChanged("PS4_UP");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_DOWN
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[14]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_DOWN, value))
			{
				SwapButton(14, value.Key);
				OnPropertyChanged("PS4_DOWN");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_LEFT
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[15]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_LEFT, value))
			{
				SwapButton(15, value.Key);
				OnPropertyChanged("PS4_LEFT");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_RIGHT
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[16]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_RIGHT, value))
			{
				SwapButton(16, value.Key);
				OnPropertyChanged("PS4_RIGHT");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_TRIANGLE
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[17]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_TRIANGLE, value))
			{
				SwapButton(17, value.Key);
				OnPropertyChanged("PS4_TRIANGLE");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_CIRCLE
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[18]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_CIRCLE, value))
			{
				SwapButton(18, value.Key);
				OnPropertyChanged("PS4_CIRCLE");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_CROSS
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[19]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_CROSS, value))
			{
				SwapButton(19, value.Key);
				OnPropertyChanged("PS4_CROSS");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_SQUARE
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[20]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_SQUARE, value))
			{
				SwapButton(20, value.Key);
				OnPropertyChanged("PS4_SQUARE");
			}
		}
	}

	public KeyValuePair<byte, string> PS4_TOUCH
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[27]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(PS4_TOUCH, value))
			{
				SwapButton(27, value.Key);
				OnPropertyChanged("PS4_TOUCH");
			}
		}
	}

	public KeyValuePair<uint, string> PL
	{
		get
		{
			return _pl;
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<uint, string>>.Default.Equals(_pl, value))
			{
				_pl = value;
				OnPropertyChanged("PL");
			}
		}
	}

	public KeyValuePair<uint, string> PR
	{
		get
		{
			return _pr;
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<uint, string>>.Default.Equals(_pr, value))
			{
				_pr = value;
				OnPropertyChanged("PR");
			}
		}
	}

	public KeyValuePair<uint, string> PL2
	{
		get
		{
			return _pl2;
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<uint, string>>.Default.Equals(_pl2, value))
			{
				_pl2 = value;
				OnPropertyChanged("PL2");
			}
		}
	}

	public KeyValuePair<uint, string> PR2
	{
		get
		{
			return _pr2;
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<uint, string>>.Default.Equals(_pr2, value))
			{
				_pr2 = value;
				OnPropertyChanged("PR2");
			}
		}
	}

	public Ps4RemapperView()
	{
		for (byte b = 0; b < _mappings.Length; b = (byte)(b + 1))
		{
			_mappings[b] = b;
		}
		PL = (PR = _paddleButtons.First());
		base.DataContext = this;
		InitializeComponent();
	}

	private KeyValuePair<uint, string> TranslateToPaddle(uint value)
	{
		return PaddleButtons.FirstOrDefault((KeyValuePair<uint, string> kvp) => kvp.Key == value);
	}

	private void SwapButton(byte src, byte dst)
	{
		if (_mappings[src] != dst && !_updating)
		{
			RemapHelper.SwapButton(ref _mappings, src, dst);
			RefreshProperties();
		}
	}

	private void RefreshProperties()
	{
		_updating = true;
		PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
		foreach (PropertyInfo propertyInfo in properties)
		{
			OnPropertyChanged(propertyInfo.Name);
		}
		_updating = false;
	}

	public override void SetButtons(DominatorData cur)
	{
		for (int i = 0; i < cur.SlotData.Maps.Length && i < _mappings.Length; i++)
		{
			_mappings[i] = cur.SlotData.Maps[i];
		}
		PL = TranslateToPaddle(cur.PaddleConfig.LeftMask);
		PR = TranslateToPaddle(cur.PaddleConfig.RightMask);
		if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
		{
			PL2 = TranslateToPaddle(cur.PaddleConfig.LeftMask2);
			PR2 = TranslateToPaddle(cur.PaddleConfig.RightMask2);
		}
		DisableRumble.IsChecked = cur.SlotData.Flags.HasFlag(SlotMetaDataBase.SlotFlags.Rumble);
		SetButtons(cur.SlotData);
		RefreshProperties();
	}

	internal override SlotView.RemapData GetRemapperData(int slot)
	{
		return new SlotView.RemapData(slot, this)
		{
			Buttons = _mappings,
			Left = PL.Key,
			Right = PR.Key,
			Left2 = PL2.Key,
			Right2 = PR2.Key,
			Mirror = (MirrorConfiguration.IsChecked == true),
			Rumble = (DisableRumble.IsChecked == true)
		};
	}

	private void Save_Click(object sender, RoutedEventArgs e)
	{
		((Window)(object)base.Window).DialogResult = true;
	}

	private void Reset_Click(object sender, RoutedEventArgs e)
	{
		if (MessageBox.Show(UI.ResetToDefaultWarningRemapper, UI.WarningTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
		{
			for (byte b = 0; b < _mappings.Length; b = (byte)(b + 1))
			{
				_mappings[b] = b;
			}
			DisableRumble.IsChecked = false;
			ResetToDefaults();
			RefreshProperties();
		}
	}

	private void ShowRightFlyout(object sender, RoutedEventArgs e)
	{
		base.Window.ShowRightFlyout();
	}

	private void ShowLeftFlyout(object sender, RoutedEventArgs e)
	{
		base.Window.ShowLeftFlyout();
	}

	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
