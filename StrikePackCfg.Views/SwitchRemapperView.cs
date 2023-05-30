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
using StrikePackCfg.Backend.EEProm;
using StrikePackCfg.HelperClasses;
using StrikePackCfg.Properties;

namespace StrikePackCfg.Views;

public partial class SwitchRemapperView : RemapperViewBase, INotifyPropertyChanged, IComponentConnector
{
	private readonly IDictionary<byte, string> _buttons = RemapHelper.GetSwitchButtons();

	private readonly IDictionary<ushort, string> _paddleButtons = RemapHelper.GetXboxOnePaddles();

	private readonly IDictionary<byte, string> _sticks = RemapHelper.GetSwitchSticks();

	private byte[] _mappings = new byte[35];

	private KeyValuePair<ushort, string> _pl;

	private KeyValuePair<ushort, string> _pr;

	private bool _updating;

	public ReadOnlyDictionary<byte, string> Buttons => new ReadOnlyDictionary<byte, string>(_buttons);

	public ReadOnlyDictionary<byte, string> Sticks => new ReadOnlyDictionary<byte, string>(_sticks);

	public ReadOnlyDictionary<ushort, string> PaddleButtons => new ReadOnlyDictionary<ushort, string>(_paddleButtons);

	public KeyValuePair<byte, string> Switch_HOME
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[0]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_HOME, value))
			{
				SwapButton(0, value.Key);
				OnPropertyChanged("Switch_HOME");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_MINUS
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[1]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_MINUS, value))
			{
				SwapButton(1, value.Key);
				OnPropertyChanged("Switch_MINUS");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_PLUS
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[2]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_PLUS, value))
			{
				SwapButton(2, value.Key);
				OnPropertyChanged("Switch_PLUS");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_LX
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[11]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_LX, value))
			{
				SwapButton(11, value.Key);
				OnPropertyChanged("Switch_LX");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_LY
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[12]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_LY, value))
			{
				SwapButton(12, value.Key);
				OnPropertyChanged("Switch_LY");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_RX
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[9]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_RX, value))
			{
				SwapButton(9, value.Key);
				OnPropertyChanged("Switch_RX");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_RY
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[10]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_RY, value))
			{
				SwapButton(10, value.Key);
				OnPropertyChanged("Switch_RY");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_L
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[6]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_L, value))
			{
				SwapButton(6, value.Key);
				OnPropertyChanged("Switch_L");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_ZL
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[7]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_ZL, value))
			{
				SwapButton(7, value.Key);
				OnPropertyChanged("Switch_ZL");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_R
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[3]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_R, value))
			{
				SwapButton(3, value.Key);
				OnPropertyChanged("Switch_R");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_ZR
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[4]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_ZR, value))
			{
				SwapButton(4, value.Key);
				OnPropertyChanged("Switch_ZR");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_LS
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[8]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_LS, value))
			{
				SwapButton(8, value.Key);
				OnPropertyChanged("Switch_LS");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_RS
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[5]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_RS, value))
			{
				SwapButton(5, value.Key);
				OnPropertyChanged("Switch_RS");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_UP
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[13]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_UP, value))
			{
				SwapButton(13, value.Key);
				OnPropertyChanged("Switch_UP");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_DOWN
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[14]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_DOWN, value))
			{
				SwapButton(14, value.Key);
				OnPropertyChanged("Switch_DOWN");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_LEFT
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[15]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_LEFT, value))
			{
				SwapButton(15, value.Key);
				OnPropertyChanged("Switch_LEFT");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_RIGHT
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[16]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_RIGHT, value))
			{
				SwapButton(16, value.Key);
				OnPropertyChanged("Switch_RIGHT");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_X
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[17]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_X, value))
			{
				SwapButton(17, value.Key);
				OnPropertyChanged("Switch_X");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_A
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[18]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_A, value))
			{
				SwapButton(18, value.Key);
				OnPropertyChanged("Switch_A");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_B
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[19]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_B, value))
			{
				SwapButton(19, value.Key);
				OnPropertyChanged("Switch_B");
			}
		}
	}

	public KeyValuePair<byte, string> Switch_Y
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[20]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(Switch_Y, value))
			{
				SwapButton(20, value.Key);
				OnPropertyChanged("Switch_Y");
			}
		}
	}

	public KeyValuePair<ushort, string> PL
	{
		get
		{
			return _pl;
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<ushort, string>>.Default.Equals(_pl, value))
			{
				_pl = value;
				OnPropertyChanged("PL");
			}
		}
	}

	public KeyValuePair<ushort, string> PR
	{
		get
		{
			return _pr;
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<ushort, string>>.Default.Equals(_pr, value))
			{
				_pr = value;
				OnPropertyChanged("PR");
			}
		}
	}

	public SwitchRemapperView()
	{
		for (byte b = 0; b < _mappings.Length; b = (byte)(b + 1))
		{
			_mappings[b] = b;
		}
		PL = (PR = _paddleButtons.First());
		base.DataContext = this;
		App.CurrentDevice.Device.Product.ProductHasPaddles();
		InitializeComponent();
	}

	private KeyValuePair<ushort, string> TranslateToPaddle(ushort value)
	{
		return PaddleButtons.FirstOrDefault((KeyValuePair<ushort, string> kvp) => kvp.Key == value);
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
		PL = TranslateToPaddle((ushort)cur.PaddleConfig.LeftMask);
		PR = TranslateToPaddle((ushort)cur.PaddleConfig.RightMask);
		DisableRumble.IsChecked = cur.SlotData.Flags.HasFlag(SlotMetaDataBase.SlotFlags.Rumble);
		RefreshProperties();
	}

	internal override SlotView.RemapData GetRemapperData(int slot)
	{
		return new SlotView.RemapData(slot, this)
		{
			Buttons = _mappings,
			Left = PL.Key,
			Right = PR.Key,
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

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
