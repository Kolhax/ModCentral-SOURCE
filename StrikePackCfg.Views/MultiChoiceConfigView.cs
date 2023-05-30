using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Windows.Media;
using StrikePackCfg.Backend.Config;

namespace StrikePackCfg.Views;

public sealed partial class MultiChoiceConfigView : ConfigViewBase, INotifyPropertyChanged, IComponentConnector
{
	private readonly MultiChoiceConfig _cfg;

	private string _header;

	private Brush _headerBrush;

	private KeyValuePair<int, string>? _selectedItem;

	public Brush HeaderBrush
	{
		get
		{
			return _headerBrush;
		}
		private set
		{
			if (!object.Equals(_headerBrush, value))
			{
				_headerBrush = value;
				OnPropertyChanged("HeaderBrush");
			}
		}
	}

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

	private int Id { get; }

	public KeyValuePair<int, string>[] Options { get; }

	public string Help { get; }

	public KeyValuePair<int, string>? SelectedItem
	{
		get
		{
			return _selectedItem;
		}
		set
		{
			if (!Nullable.Equals(_selectedItem, value))
			{
				_selectedItem = value;
				SetColors();
				OnPropertyChanged("SelectedItem");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public MultiChoiceConfigView(MultiChoiceConfig cfg)
	{
		_cfg = cfg;
		Id = cfg.Id;
		Options = cfg.OptionsList;
		Help = cfg.Help;
		SelectedItem = Options.FirstOrDefault();
		base.DataContext = this;
		InitializeComponent();
		Header = "";
		if (cfg.Name != null)
		{
			Header = cfg.Name.ToUpper();
		}
		SetColors();
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

	public override short GetValue()
	{
		return (short)(SelectedItem?.Key ?? 0);
	}

	public override int GetId()
	{
		return Id;
	}

	public override void SetValue(int value)
	{
		SelectedItem = Options.FirstOrDefault((KeyValuePair<int, string> kvp) => kvp.Key == value);
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
