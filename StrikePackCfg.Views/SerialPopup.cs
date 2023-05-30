using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using StrikePackCfg.Properties;

namespace StrikePackCfg.Views;

public partial class SerialPopup : UserControl, IComponentConnector, INotifyPropertyChanged
{
	private const double ANIMATION_TIME = 0.5;

	[CompilerGenerated]
	private bool? _003CIsLatestFirmwareInstalled_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CIsConnected_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CFirmware_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CSerial_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CEntitlement_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CDevice_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CStatus_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CIsOpen_003Ek__BackingField;

	public bool? IsLatestFirmwareInstalled
	{
		[CompilerGenerated]
		get
		{
			return _003CIsLatestFirmwareInstalled_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!Nullable.Equals(_003CIsLatestFirmwareInstalled_003Ek__BackingField, value))
			{
				_003CIsLatestFirmwareInstalled_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.IsLatestFirmwareInstalled);
			}
		}
	}

	public bool IsConnected
	{
		[CompilerGenerated]
		get
		{
			return _003CIsConnected_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CIsConnected_003Ek__BackingField != value)
			{
				_003CIsConnected_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.IsConnected);
			}
		}
	}

	public string Firmware
	{
		[CompilerGenerated]
		get
		{
			return _003CFirmware_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!string.Equals(_003CFirmware_003Ek__BackingField, value, StringComparison.Ordinal))
			{
				_003CFirmware_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.Firmware);
			}
		}
	} = UI.DeviceNotDetected;


	public string Serial
	{
		[CompilerGenerated]
		get
		{
			return _003CSerial_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!string.Equals(_003CSerial_003Ek__BackingField, value, StringComparison.Ordinal))
			{
				_003CSerial_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.Serial);
			}
		}
	} = UI.DeviceNotDetected;


	public string Entitlement
	{
		[CompilerGenerated]
		get
		{
			return _003CEntitlement_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!string.Equals(_003CEntitlement_003Ek__BackingField, value, StringComparison.Ordinal))
			{
				_003CEntitlement_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.Entitlement);
			}
		}
	} = UI.DeviceNotDetected;


	public string Device
	{
		[CompilerGenerated]
		get
		{
			return _003CDevice_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!string.Equals(_003CDevice_003Ek__BackingField, value, StringComparison.Ordinal))
			{
				_003CDevice_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.Device);
			}
		}
	} = UI.DeviceNotDetected;


	public string Status
	{
		[CompilerGenerated]
		get
		{
			return _003CStatus_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!string.Equals(_003CStatus_003Ek__BackingField, value, StringComparison.Ordinal))
			{
				_003CStatus_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.Status);
			}
		}
	} = UI.SerialStatusNotConnected;


	public bool IsOpen
	{
		[CompilerGenerated]
		get
		{
			return _003CIsOpen_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			if (_003CIsOpen_003Ek__BackingField != value)
			{
				_003CIsOpen_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.IsOpen);
			}
		}
	}

	[field: NonSerialized]
	public event PropertyChangedEventHandler PropertyChanged;

	public SerialPopup()
	{
		InitializeComponent();
		base.DataContext = this;
		base.RenderTransform = new TranslateTransform();
		base.Visibility = Visibility.Hidden;
		IsOpen = false;
	}

	public void Show()
	{
		if (!IsOpen)
		{
			base.Visibility = Visibility.Visible;
			IsOpen = true;
			DoubleAnimation animation = new DoubleAnimation
			{
				From = 0.0 - base.ActualWidth,
				To = 0.0,
				Duration = TimeSpan.FromSeconds(0.5)
			};
			base.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation);
		}
	}

	public void Hide()
	{
		if (IsOpen)
		{
			IsOpen = false;
			DoubleAnimation animation = new DoubleAnimation
			{
				From = 0.0,
				To = 0.0 - base.ActualWidth,
				Duration = TimeSpan.FromSeconds(0.5)
			};
			base.RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation);
		}
	}

	private void CopyInfo_Click(object sender, RoutedEventArgs e)
	{
		if (IsConnected)
		{
			StringBuilder stringBuilder = new StringBuilder("Collective Minds Device Information:\r\n");
			stringBuilder.AppendLine($"Device: {Device}");
			stringBuilder.AppendLine($"Firmware: {Firmware}");
			stringBuilder.AppendLine($"Serial: {Serial}");
			Clipboard.SetText(stringBuilder.ToString(), TextDataFormat.UnicodeText);
		}
	}

	protected void _003C_003EOnPropertyChanged(PropertyChangedEventArgs eventArgs)
	{
		this.PropertyChanged?.Invoke(this, eventArgs);
	}
}
