using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using StrikePackCfg.Backend.EEProm;
using StrikePackCfg.Windows;

namespace StrikePackCfg.Views;

public abstract class RemapperViewBase : UserControl, INotifyPropertyChanged
{
	private bool remapEnabled;

	private bool mirrorConfigurationEnabled;

	private bool mirrorConfiguratioChecked;

	[CompilerGenerated]
	private RemapperWindow _003CWindow_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CRightTriggerDeadzone_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CRXDeadzone_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CRYDeadzone_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CLeftTriggerDeadzone_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CLXDeadzone_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CLYDeadzone_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CRightTriggerSensitivity_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CRXSensitivity_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CRYSensitivity_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CLeftTriggerSensitivity_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CLXSensitivity_003Ek__BackingField;

	[CompilerGenerated]
	private double _003CLYSensitivity_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CInvertRX_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CInvertRY_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CInvertLX_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CInvertLY_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CUseHairTriggersLeft_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CUseHairTriggersRight_003Ek__BackingField;

	public RemapperWindow Window
	{
		[CompilerGenerated]
		protected get
		{
			return _003CWindow_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!object.Equals(_003CWindow_003Ek__BackingField, value))
			{
				_003CWindow_003Ek__BackingField = value;
				OnPropertyChanged("Window");
			}
		}
	}

	public double RightTriggerDeadzone
	{
		[CompilerGenerated]
		get
		{
			return _003CRightTriggerDeadzone_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CRightTriggerDeadzone_003Ek__BackingField == value))
			{
				_003CRightTriggerDeadzone_003Ek__BackingField = value;
				OnPropertyChanged("RightTriggerDeadzone");
			}
		}
	}

	public double RXDeadzone
	{
		[CompilerGenerated]
		get
		{
			return _003CRXDeadzone_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CRXDeadzone_003Ek__BackingField == value))
			{
				_003CRXDeadzone_003Ek__BackingField = value;
				OnPropertyChanged("RXDeadzone");
			}
		}
	}

	public double RYDeadzone
	{
		[CompilerGenerated]
		get
		{
			return _003CRYDeadzone_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CRYDeadzone_003Ek__BackingField == value))
			{
				_003CRYDeadzone_003Ek__BackingField = value;
				OnPropertyChanged("RYDeadzone");
			}
		}
	}

	public double LeftTriggerDeadzone
	{
		[CompilerGenerated]
		get
		{
			return _003CLeftTriggerDeadzone_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CLeftTriggerDeadzone_003Ek__BackingField == value))
			{
				_003CLeftTriggerDeadzone_003Ek__BackingField = value;
				OnPropertyChanged("LeftTriggerDeadzone");
			}
		}
	}

	public double LXDeadzone
	{
		[CompilerGenerated]
		get
		{
			return _003CLXDeadzone_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CLXDeadzone_003Ek__BackingField == value))
			{
				_003CLXDeadzone_003Ek__BackingField = value;
				OnPropertyChanged("LXDeadzone");
			}
		}
	}

	public double LYDeadzone
	{
		[CompilerGenerated]
		get
		{
			return _003CLYDeadzone_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CLYDeadzone_003Ek__BackingField == value))
			{
				_003CLYDeadzone_003Ek__BackingField = value;
				OnPropertyChanged("LYDeadzone");
			}
		}
	}

	public double RightTriggerSensitivity
	{
		[CompilerGenerated]
		get
		{
			return _003CRightTriggerSensitivity_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CRightTriggerSensitivity_003Ek__BackingField == value))
			{
				_003CRightTriggerSensitivity_003Ek__BackingField = value;
				OnPropertyChanged("RightTriggerSensitivity");
			}
		}
	}

	public double RXSensitivity
	{
		[CompilerGenerated]
		get
		{
			return _003CRXSensitivity_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CRXSensitivity_003Ek__BackingField == value))
			{
				_003CRXSensitivity_003Ek__BackingField = value;
				OnPropertyChanged("RXSensitivity");
			}
		}
	}

	public double RYSensitivity
	{
		[CompilerGenerated]
		get
		{
			return _003CRYSensitivity_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CRYSensitivity_003Ek__BackingField == value))
			{
				_003CRYSensitivity_003Ek__BackingField = value;
				OnPropertyChanged("RYSensitivity");
			}
		}
	}

	public double LeftTriggerSensitivity
	{
		[CompilerGenerated]
		get
		{
			return _003CLeftTriggerSensitivity_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CLeftTriggerSensitivity_003Ek__BackingField == value))
			{
				_003CLeftTriggerSensitivity_003Ek__BackingField = value;
				OnPropertyChanged("LeftTriggerSensitivity");
			}
		}
	}

	public double LXSensitivity
	{
		[CompilerGenerated]
		get
		{
			return _003CLXSensitivity_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CLXSensitivity_003Ek__BackingField == value))
			{
				_003CLXSensitivity_003Ek__BackingField = value;
				OnPropertyChanged("LXSensitivity");
			}
		}
	}

	public double LYSensitivity
	{
		[CompilerGenerated]
		get
		{
			return _003CLYSensitivity_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!(_003CLYSensitivity_003Ek__BackingField == value))
			{
				_003CLYSensitivity_003Ek__BackingField = value;
				OnPropertyChanged("LYSensitivity");
			}
		}
	}

	public bool InvertRX
	{
		[CompilerGenerated]
		get
		{
			return _003CInvertRX_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CInvertRX_003Ek__BackingField != value)
			{
				_003CInvertRX_003Ek__BackingField = value;
				OnPropertyChanged("InvertRX");
			}
		}
	}

	public bool InvertRY
	{
		[CompilerGenerated]
		get
		{
			return _003CInvertRY_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CInvertRY_003Ek__BackingField != value)
			{
				_003CInvertRY_003Ek__BackingField = value;
				OnPropertyChanged("InvertRY");
			}
		}
	}

	public bool InvertLX
	{
		[CompilerGenerated]
		get
		{
			return _003CInvertLX_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CInvertLX_003Ek__BackingField != value)
			{
				_003CInvertLX_003Ek__BackingField = value;
				OnPropertyChanged("InvertLX");
			}
		}
	}

	public bool InvertLY
	{
		[CompilerGenerated]
		get
		{
			return _003CInvertLY_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CInvertLY_003Ek__BackingField != value)
			{
				_003CInvertLY_003Ek__BackingField = value;
				OnPropertyChanged("InvertLY");
			}
		}
	}

	public bool UseHairTriggersLeft
	{
		[CompilerGenerated]
		get
		{
			return _003CUseHairTriggersLeft_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CUseHairTriggersLeft_003Ek__BackingField != value)
			{
				_003CUseHairTriggersLeft_003Ek__BackingField = value;
				OnPropertyChanged("UseHairTriggersLeft");
			}
		}
	}

	public bool UseHairTriggersRight
	{
		[CompilerGenerated]
		get
		{
			return _003CUseHairTriggersRight_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CUseHairTriggersRight_003Ek__BackingField != value)
			{
				_003CUseHairTriggersRight_003Ek__BackingField = value;
				OnPropertyChanged("UseHairTriggersRight");
			}
		}
	}

	public bool RemapEnabled
	{
		get
		{
			return remapEnabled;
		}
		set
		{
			if (remapEnabled != value)
			{
				remapEnabled = value;
				OnPropertyChanged("RemapEnabled");
			}
		}
	}

	public bool MirrorConfigurationEnabled
	{
		get
		{
			return mirrorConfigurationEnabled;
		}
		set
		{
			if (mirrorConfigurationEnabled != value)
			{
				mirrorConfigurationEnabled = value;
				OnPropertyChanged("MirrorConfigurationEnabled");
			}
		}
	}

	public bool MirrorConfigurationChecked
	{
		get
		{
			return mirrorConfiguratioChecked;
		}
		set
		{
			if (mirrorConfiguratioChecked != value)
			{
				mirrorConfiguratioChecked = value;
				OnPropertyChanged("MirrorConfigurationChecked");
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public abstract void SetButtons(DominatorData cur);

	internal abstract SlotView.RemapData GetRemapperData(int slot);

	protected void SetButtons(SlotMetaDataBase data)
	{
		LeftTriggerDeadzone = data.Deadzone.LT;
		RightTriggerDeadzone = data.Deadzone.RT;
		LXDeadzone = data.Deadzone.LX;
		RXDeadzone = data.Deadzone.RX;
		LYDeadzone = data.Deadzone.LY;
		RYDeadzone = data.Deadzone.RY;
		LeftTriggerSensitivity = data.Sensitivity.LT;
		RightTriggerSensitivity = data.Sensitivity.RT;
		LXSensitivity = data.Sensitivity.LX;
		RXSensitivity = data.Sensitivity.RX;
		LYSensitivity = data.Sensitivity.LY;
		RYSensitivity = data.Sensitivity.RY;
		InvertLX = data.Flags.HasFlag(SlotMetaDataBase.SlotFlags.InvertLX);
		InvertLY = data.Flags.HasFlag(SlotMetaDataBase.SlotFlags.InvertLY);
		InvertRX = data.Flags.HasFlag(SlotMetaDataBase.SlotFlags.InvertRX);
		InvertRY = data.Flags.HasFlag(SlotMetaDataBase.SlotFlags.InvertRY);
		UseHairTriggersLeft = data.ExtraFlags.HasFlag(SlotMetaDataBase.SlotExtraFlags.UseHairTriggersLeft);
		UseHairTriggersRight = data.ExtraFlags.HasFlag(SlotMetaDataBase.SlotExtraFlags.UseHairTriggersRight);
	}

	protected void ResetToDefaults()
	{
		MirrorConfigurationChecked = false;
		SetButtons(App.CurrentDevice.Device.GetMetaBase());
	}

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
