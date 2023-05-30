using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace StrikePackCfg.Views;

public sealed partial class SliderAdjustmentView : UserControl, INotifyPropertyChanged, IComponentConnector
{
	private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(SliderAdjustmentView), new PropertyMetadata(0.0, ValueChangedCallback));

	public string ValueString => string.Format("{0}{1:F0}%", (Value >= 0.0) ? "+" : "", Math.Round(Value));

	public double Value
	{
		get
		{
			return (double)GetValue(ValueProperty);
		}
		set
		{
			if (!(Value == value))
			{
				SetValue(ValueProperty, value);
				OnPropertyChanged("Value");
				OnPropertyChanged("ValueString");
			}
		}
	}

	public double Minimum
	{
		get
		{
			return (double)GetValue(RangeBase.MinimumProperty);
		}
		set
		{
			if (!(Minimum == value))
			{
				SetValue(RangeBase.MinimumProperty, value);
				OnPropertyChanged("Minimum");
			}
		}
	}

	public double Maximum
	{
		get
		{
			return (double)GetValue(RangeBase.MaximumProperty);
		}
		set
		{
			if (!(Maximum == value))
			{
				SetValue(RangeBase.MaximumProperty, value);
				OnPropertyChanged("Maximum");
			}
		}
	}

	public string Header
	{
		get
		{
			return (string)GetValue(HeaderedContentControl.HeaderProperty);
		}
		set
		{
			if (!string.Equals(Header, value, StringComparison.Ordinal))
			{
				SetValue(HeaderedContentControl.HeaderProperty, value);
				OnPropertyChanged("Header");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public SliderAdjustmentView()
	{
		InitializeComponent();
	}

	private static void ValueChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
	{
		(obj as SliderAdjustmentView)?.OnPropertyChanged("ValueString");
	}

	private void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
