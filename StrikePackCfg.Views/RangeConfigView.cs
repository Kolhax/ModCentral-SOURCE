using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using MahApps.Metro.Controls;
using StrikePackCfg.Backend.Config;

namespace StrikePackCfg.Views;

public sealed partial class RangeConfigView : ConfigViewBase, INotifyPropertyChanged, IComponentConnector
{
	private readonly RangeConfig _cfg;

	private readonly double _defaultValue;

	private string _header;

	private Brush _headerBrush;

	private double _value;

	private NumericUpDown Nupd;

	private int Id { get; }

	public string Header
	{
		get
		{
			return _header;
		}
		set
		{
			if (!string.Equals(_header, value, StringComparison.Ordinal))
			{
				_header = value;
				OnPropertyChanged("Header");
			}
		}
	}

	public double Value
	{
		get
		{
			return _value;
		}
		set
		{
			if (!(_value == value))
			{
				_value = value;
				SetColors();
				OnPropertyChanged("Value");
			}
		}
	}

	public Brush HeaderBrush
	{
		get
		{
			return _headerBrush;
		}
		set
		{
			if (!object.Equals(_headerBrush, value))
			{
				_headerBrush = value;
				OnPropertyChanged("HeaderBrush");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public RangeConfigView(RangeConfig cfg)
	{
		_cfg = cfg;
		InitializeComponent();
		base.DataContext = this;
		Id = cfg.Id;
		Header = cfg.Name.ToUpper();
		((UIElement)(object)Nupd).Focusable = false;
		Nupd.Maximum = cfg.MaximumValue;
		Nupd.Minimum = cfg.MinimumValue;
		Nupd.Interval = cfg.Step;
		Value = (_defaultValue = cfg.DefaultValue);
		SetColors();
	}

	public override short GetValue()
	{
		return (short)Value;
	}

	public override int GetId()
	{
		return Id;
	}

	private void SetColors()
	{
		if (_cfg.AlwaysColor || (_cfg.TrueColor && GetValue() != 0))
		{
			HeaderBrush = _cfg.Color;
		}
		else
		{
			HeaderBrush = (Brush)FindResource("AccentColorBrush");
		}
	}

	public override void SetValue(int value)
	{
		if ((double)value < Nupd.Minimum || (double)value > Nupd.Maximum)
		{
			Value = _defaultValue;
		}
		else
		{
			Value = value;
		}
	}

	private void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[DebuggerNonUserCode]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}
}
