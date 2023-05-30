using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CollectiveMinds.Commmon.Network;
using CollectiveMinds.Common.Azure;
using CollectiveMinds.Common.DeviceUpdate;
using DfuLib;
using Semver;
using StrikePackCfg.Backend.EEProm;
using StrikePackCfg.Backend.Products;

namespace StrikePackCfg.Backend;

public class CollectiveMindsDevice : INotifyPropertyChanged
{
	private static UpdateManifest updateManifest;

	[CompilerGenerated]
	private string _003CSerialNumber_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CApplicationVersion_003Ek__BackingField;

	[CompilerGenerated]
	private CollectiveMindsProduct _003CProduct_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CHasSubscription_003Ek__BackingField;

	private string Path { get; }

	public string SerialNumber
	{
		[CompilerGenerated]
		get
		{
			return _003CSerialNumber_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!string.Equals(_003CSerialNumber_003Ek__BackingField, value, StringComparison.Ordinal))
			{
				_003CSerialNumber_003Ek__BackingField = value;
				OnPropertyChanged("SerialNumber");
			}
		}
	}

	public string ApplicationVersion
	{
		[CompilerGenerated]
		get
		{
			return _003CApplicationVersion_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!string.Equals(_003CApplicationVersion_003Ek__BackingField, value, StringComparison.Ordinal))
			{
				_003CApplicationVersion_003Ek__BackingField = value;
				OnPropertyChanged("Is16Bit");
				OnPropertyChanged("SupportsAdjustments");
				OnPropertyChanged("SupportsSlot0Remapper");
				OnPropertyChanged("SupportsSlot1Remapper");
				OnPropertyChanged("SupportsFpsGamepacks");
				OnPropertyChanged("IsLatestFirmwareInstalled");
				OnPropertyChanged("ApplicationVersion");
			}
		}
	}

	public CollectiveMindsProduct Product
	{
		[CompilerGenerated]
		get
		{
			return _003CProduct_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!object.Equals(_003CProduct_003Ek__BackingField, value))
			{
				_003CProduct_003Ek__BackingField = value;
				OnPropertyChanged("Is16Bit");
				OnPropertyChanged("Is32Bit");
				OnPropertyChanged("SupportsAdjustments");
				OnPropertyChanged("SupportsSlot0Remapper");
				OnPropertyChanged("CanReflashFPS");
				OnPropertyChanged("SupportsSlot1Remapper");
				OnPropertyChanged("SupportsFpsGamepacks");
				OnPropertyChanged("Platform");
				OnPropertyChanged("IsLatestFirmwareInstalled");
				OnPropertyChanged("Product");
			}
		}
	}

	public bool Is16Bit => Product.Is16Bit(ApplicationVersion);

	public bool Is32Bit => Product.Is32Bit();

	public bool SupportsAdjustments => Product.SupportsAdjustments(ApplicationVersion);

	public bool SupportsSlot0Remapper => Product.SupportsSlot0Remapper(ApplicationVersion);

	public bool CanReflashFPS => Product.CanReflashFPS();

	public bool SupportsSlot1Remapper => Product.SupportsSlot1Remapper(ApplicationVersion);

	public bool SupportsFpsGamepacks => Product.SupportsFpsGamepacks(ApplicationVersion);

	public bool HasSubscription
	{
		[CompilerGenerated]
		get
		{
			return _003CHasSubscription_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CHasSubscription_003Ek__BackingField != value)
			{
				_003CHasSubscription_003Ek__BackingField = value;
				OnPropertyChanged("HasSubscription");
			}
		}
	}

	public int Platform
	{
		get
		{
			if (Product.DeviceInterfaceGuid == new DominatorPs4().DeviceInterfaceGuid || Product.DeviceInterfaceGuid == new DominatorPs4v2().DeviceInterfaceGuid || Product.DeviceInterfaceGuid == new EliminatorPs4().DeviceInterfaceGuid)
			{
				return 2;
			}
			if (Product.DeviceInterfaceGuid == new DominatorXb1().DeviceInterfaceGuid || Product.DeviceInterfaceGuid == new BattlePackXb1().DeviceInterfaceGuid || Product.DeviceInterfaceGuid == new StrikePackXb1().DeviceInterfaceGuid || Product.DeviceInterfaceGuid == new StrikePackXb1().DeviceInterfaceGuid)
			{
				return 1;
			}
			if (Product.DeviceInterfaceGuid == new SwitchCon().DeviceInterfaceGuid)
			{
				return 4;
			}
			return 0;
		}
	}

	public bool? IsLatestFirmwareInstalled
	{
		get
		{
			if (updateManifest == null || Product == null)
			{
				return null;
			}
			List<ResetRequiredProduct> supportedResetRequiredProducts = updateManifest.SupportedResetRequiredProducts;
			ResetRequiredProduct resetDev = supportedResetRequiredProducts.FirstOrDefault((ResetRequiredProduct d) => d.DeviceInterfaceGuid == Product.DeviceInterfaceGuid);
			if (resetDev == null)
			{
				return true;
			}
			UpdateProduct updateProduct = updateManifest.SupportedProducts.FirstOrDefault((UpdateProduct d) => resetDev.KnownDfuModeGuidList.Contains(d.DeviceInterfaceGuid));
			if (updateProduct == null)
			{
				return null;
			}
			SemVersion semVersion = SemVersion.Parse(updateProduct.LatestFirmwareVersion);
			return SemVersion.Parse(ApplicationVersion) >= semVersion;
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public CollectiveMindsDevice(string path)
	{
		Path = path;
		BackgroundWorker backgroundWorker = new BackgroundWorker();
		backgroundWorker.DoWork += delegate
		{
			if (updateManifest == null)
			{
				Task.Run(delegate
				{
					GetUpdateManifest();
				}).Wait();
			}
		};
		backgroundWorker.RunWorkerCompleted += delegate
		{
			OnPropertyChanged("IsLatestFirmwareInstalled");
		};
		backgroundWorker.RunWorkerAsync();
	}

	private static async void GetUpdateManifest()
	{
		try
		{
			updateManifest = await NetworkHelper.FetchManifest<UpdateManifest>("https://cmupdate.azureedge.net/firmware/manifest.xml");
		}
		catch (Exception exception)
		{
			TelemetryHelper.TrackException(exception);
			try
			{
				updateManifest = await NetworkHelper.FetchManifest<UpdateManifest>("https://cmupdate.blob.core.windows.net/firmware/manifest.xml");
			}
			catch (Exception exception2)
			{
				TelemetryHelper.TrackException(exception2);
			}
		}
	}

	public CmPpDfuDeviceBackend GetDeviceBackend()
	{
		return new CmPpDfuDeviceBackend(Path);
	}

	public SlotMetaDataBase GetMetaBase()
	{
		if (Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.Xb1))
		{
			return new SlotMetaDataXb1(Is16Bit, SupportsAdjustments);
		}
		if (Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.Ps4))
		{
			return new SlotMetaDataPs4(Is16Bit, SupportsAdjustments);
		}
		if (Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.Switch))
		{
			return new SlotMetaDataSwitch(Is16Bit, SupportsAdjustments);
		}
		throw new InvalidOperationException($"Invalid Flags for the '{App.CurrentDevice.Device.Product.DisplayName}' device, it must be either XB1 or PS4 compatible...");
	}

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
