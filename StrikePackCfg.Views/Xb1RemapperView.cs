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

public partial class Xb1RemapperView : RemapperViewBase, INotifyPropertyChanged, IComponentConnector
{
	private readonly IDictionary<byte, string> _buttons = RemapHelper.GetXboxOneButtons();

	private readonly IDictionary<ushort, string> _paddleButtons = RemapHelper.GetXboxOnePaddles();

	private readonly IDictionary<byte, string> _sticks = RemapHelper.GetXboxOneSticks();

	private byte[] _mappings = new byte[30];

	private KeyValuePair<ushort, string> _pl;

	private KeyValuePair<ushort, string> _pr;

	private KeyValuePair<ushort, string> _pl2;

	private KeyValuePair<ushort, string> _pr2;

	private bool _updating;

	public ReadOnlyDictionary<byte, string> Buttons => new ReadOnlyDictionary<byte, string>(_buttons);

	public ReadOnlyDictionary<byte, string> Sticks => new ReadOnlyDictionary<byte, string>(_sticks);

	public ReadOnlyDictionary<ushort, string> PaddleButtons => new ReadOnlyDictionary<ushort, string>(_paddleButtons);

	public KeyValuePair<byte, string> XB1_XBOX
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[0]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_XBOX, value))
			{
				SwapButton(0, value.Key);
				OnPropertyChanged("XB1_XBOX");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_VIEW
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[1]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_VIEW, value))
			{
				SwapButton(1, value.Key);
				OnPropertyChanged("XB1_VIEW");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_MENU
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[2]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_MENU, value))
			{
				SwapButton(2, value.Key);
				OnPropertyChanged("XB1_MENU");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_LX
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[11]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_LX, value))
			{
				SwapButton(11, value.Key);
				OnPropertyChanged("XB1_LX");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_LY
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[12]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_LY, value))
			{
				SwapButton(12, value.Key);
				OnPropertyChanged("XB1_LY");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_RX
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[9]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_RX, value))
			{
				SwapButton(9, value.Key);
				OnPropertyChanged("XB1_RX");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_RY
	{
		get
		{
			return Sticks.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[10]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_RY, value))
			{
				SwapButton(10, value.Key);
				OnPropertyChanged("XB1_RY");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_LB
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[6]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_LB, value))
			{
				SwapButton(6, value.Key);
				OnPropertyChanged("XB1_LB");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_LT
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[7]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_LT, value))
			{
				SwapButton(7, value.Key);
				OnPropertyChanged("XB1_LT");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_RB
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[3]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_RB, value))
			{
				SwapButton(3, value.Key);
				OnPropertyChanged("XB1_RB");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_RT
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[4]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_RT, value))
			{
				SwapButton(4, value.Key);
				OnPropertyChanged("XB1_RT");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_LS
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[8]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_LS, value))
			{
				SwapButton(8, value.Key);
				OnPropertyChanged("XB1_LS");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_RS
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[5]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_RS, value))
			{
				SwapButton(5, value.Key);
				OnPropertyChanged("XB1_RS");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_UP
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[13]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_UP, value))
			{
				SwapButton(13, value.Key);
				OnPropertyChanged("XB1_UP");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_DOWN
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[14]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_DOWN, value))
			{
				SwapButton(14, value.Key);
				OnPropertyChanged("XB1_DOWN");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_LEFT
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[15]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_LEFT, value))
			{
				SwapButton(15, value.Key);
				OnPropertyChanged("XB1_LEFT");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_RIGHT
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[16]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_RIGHT, value))
			{
				SwapButton(16, value.Key);
				OnPropertyChanged("XB1_RIGHT");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_Y
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[17]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_Y, value))
			{
				SwapButton(17, value.Key);
				OnPropertyChanged("XB1_Y");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_B
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[18]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_B, value))
			{
				SwapButton(18, value.Key);
				OnPropertyChanged("XB1_B");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_A
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[19]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_A, value))
			{
				SwapButton(19, value.Key);
				OnPropertyChanged("XB1_A");
			}
		}
	}

	public KeyValuePair<byte, string> XB1_X
	{
		get
		{
			return Buttons.FirstOrDefault((KeyValuePair<byte, string> kvp) => kvp.Key == _mappings[20]);
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<byte, string>>.Default.Equals(XB1_X, value))
			{
				SwapButton(20, value.Key);
				OnPropertyChanged("XB1_X");
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

	public KeyValuePair<ushort, string> PL2
	{
		get
		{
			return _pl2;
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<ushort, string>>.Default.Equals(_pl2, value))
			{
				_pl2 = value;
				OnPropertyChanged("PL2");
			}
		}
	}

	public KeyValuePair<ushort, string> PR2
	{
		get
		{
			return _pr2;
		}
		set
		{
			if (!EqualityComparer<KeyValuePair<ushort, string>>.Default.Equals(_pr2, value))
			{
				_pr2 = value;
				OnPropertyChanged("PR2");
			}
		}
	}

	public Xb1RemapperView()
	{
		for (byte b = 0; b < _mappings.Length; b = (byte)(b + 1))
		{
			_mappings[b] = b;
		}
		PL = (PR = _paddleButtons.First());
		base.DataContext = this;
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
		if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
		{
			PL2 = TranslateToPaddle((ushort)cur.PaddleConfig.LeftMask2);
			PR2 = TranslateToPaddle((ushort)cur.PaddleConfig.RightMask2);
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
			ResetToDefaults();
			DisableRumble.IsChecked = false;
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
