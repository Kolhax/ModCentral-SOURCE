using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CollectiveMinds.AppUpdate;
using CollectiveMinds.Common.Azure;
using DfuLib;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using StrikePackCfg.Backend;
using StrikePackCfg.Backend.EEProm;
using StrikePackCfg.Backend.Products;
using StrikePackCfg.HelperClasses;
using StrikePackCfg.Properties;
using StrikePackCfg.Views;

namespace StrikePackCfg.Windows;

public class MainWindow : MetroWindow, IDisposable, IComponentConnector, INotifyPropertyChanged
{
	public class SerialStatus : INotifyPropertyChanged
	{
		[CompilerGenerated]
		private string _003CFirmware_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CSerial_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CDevice_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CStatus_003Ek__BackingField;

		[CompilerGenerated]
		private bool _003CIsConnected_003Ek__BackingField;

		[CompilerGenerated]
		private bool? _003CIsLatestFirmwareInstalled_003Ek__BackingField;

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

		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;

		protected void _003C_003EOnPropertyChanged(PropertyChangedEventArgs eventArgs)
		{
			this.PropertyChanged?.Invoke(this, eventArgs);
		}
	}

	[DataContract]
	private class ApiResponseEntitlement
	{
		[DataMember(Name = "start")]
		public string Start { get; set; }

		[DataMember(Name = "end")]
		public string End { get; set; }

		[DataMember(Name = "news")]
		public string News { get; set; }

		[DataMember(Name = "bl")]
		public string Bl { get; set; }

		[DataMember(Name = "auth")]
		public string Auth { get; set; }
	}

	public static readonly RoutedUICommand FactoryResetCommand = new RoutedUICommand("Factory Reset", "FactoryReset", typeof(MainWindow));

	public readonly ObservableCollection<IDeviceState> Devices = new ObservableCollection<IDeviceState>();

	private CollectiveMindsProductEnumerator _devEnum;

	private bool _isErrorShown;

	public EventHandler CloseWindow;

	[CompilerGenerated]
	private ApplicationUpdateManager _003CUpdateManager_003Ek__BackingField;

	[CompilerGenerated]
	private Visibility _003CShowDeviceSelectorButtonVisibility_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CEnableDeviceSelectorButton_003Ek__BackingField;

	[CompilerGenerated]
	private Visibility _003CUpgradeBanner_003Ek__BackingField;

	internal SerialPopup SerialPopup;

	internal StatusBarItem SerialStatusBarItem;

	internal GroupBox RenewGb;

	internal StackPanel MainGrid;

	internal SlotView MainView;

	private bool _contentLoaded;

	private ApplicationUpdateManager UpdateManager
	{
		[CompilerGenerated]
		get
		{
			return _003CUpdateManager_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (!object.Equals(_003CUpdateManager_003Ek__BackingField, value))
			{
				_003CUpdateManager_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.UpdateRequiredVisibility);
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.UpdateAvailableVisibility);
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.UpdateManager);
			}
		}
	}

	public Visibility ShowDeviceSelectorButtonVisibility
	{
		[CompilerGenerated]
		get
		{
			return _003CShowDeviceSelectorButtonVisibility_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			if (_003CShowDeviceSelectorButtonVisibility_003Ek__BackingField != value)
			{
				_003CShowDeviceSelectorButtonVisibility_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.ShowDeviceSelectorButtonVisibility);
			}
		}
	} = Visibility.Collapsed;


	public bool EnableDeviceSelectorButton
	{
		[CompilerGenerated]
		get
		{
			return _003CEnableDeviceSelectorButton_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			if (_003CEnableDeviceSelectorButton_003Ek__BackingField != value)
			{
				_003CEnableDeviceSelectorButton_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.EnableDeviceSelectorButton);
			}
		}
	} = true;


	public Visibility UpgradeBanner
	{
		[CompilerGenerated]
		get
		{
			return _003CUpgradeBanner_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			if (_003CUpgradeBanner_003Ek__BackingField != value)
			{
				_003CUpgradeBanner_003Ek__BackingField = value;
				_003C_003EOnPropertyChanged(_003C_003EPropertyChangedEventArgs.UpgradeBanner);
			}
		}
	} = Visibility.Collapsed;


	public Visibility UpdateRequiredVisibility
	{
		get
		{
			if (UpdateManager == null)
			{
				return Visibility.Visible;
			}
			if (!UpdateManager.UpdateRequired)
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}
	}

	public Visibility UpdateAvailableVisibility
	{
		get
		{
			if (UpdateManager == null)
			{
				return Visibility.Collapsed;
			}
			if (!UpdateManager.UpdateAvailable)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}
	}

	[field: NonSerialized]
	public event PropertyChangedEventHandler PropertyChanged;

	public MainWindow()
	{
		App.MainWin = this;
		InitializeComponent();
		Version version = Assembly.GetAssembly(typeof(MainWindow)).GetName().Version;
		((Window)this).Title = string.Format(UI.StrikePackConfiguratorTitle, version.Major, version.Minor, version.Build);
		((FrameworkElement)this).DataContext = this;
		((FrameworkElement)this).Loaded += OnLoaded;
		UnhandledExceptionTelemetryCollector.UnhandledException += OnUnhandledException;
		Task.Run(async delegate
		{
			UpdateManager = await App.GetUpdateManager();
		});
		((UIElement)this).LostFocus += delegate
		{
			SerialPopup.Hide();
		};
		((UIElement)this).PreviewMouseUp += delegate
		{
			if (!SerialPopup.IsMouseOver && !SerialStatusBarItem.IsMouseOver)
			{
				SerialPopup.Hide();
			}
		};
	}

	public void Dispose()
	{
		if (_devEnum != null)
		{
			_devEnum.Dispose();
			_devEnum = null;
		}
	}

	private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		if (!_isErrorShown)
		{
			((DispatcherObject)this).Dispatcher.Invoke(delegate
			{
				_isErrorShown = true;
				MessageBox.Show(UI.UnhandledError, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
				_isErrorShown = false;
			});
		}
	}

	~MainWindow()
	{
		try
		{
			Dispose();
		}
		finally
		{
			((object)this).Finalize();
		}
	}

	private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
	{
		Devices.CollectionChanged += DevicesOnCollectionChanged;
		_devEnum = new CollectiveMindsProductEnumerator((Control)(object)this, Devices);
		_devEnum.Add(new DominatorPs4());
		_devEnum.Add(new DominatorXb1());
		_devEnum.Add(new StrikePackXb1());
		_devEnum.Add(new BattlePackXb1());
		_devEnum.Add(new DominatorPs4v2());
		_devEnum.Add(new EliminatorPs4());
		_devEnum.Add(new EliminatorProXb1());
	}

	private void DevicesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		ShowDeviceSelectorButtonVisibility = ((Devices.Count <= 1) ? Visibility.Collapsed : Visibility.Visible);
		if (App.CurrentDevice != null)
		{
			string serial = App.CurrentDevice.Device.SerialNumber;
			if (Devices.FirstOrDefault((IDeviceState dev) => dev.Device.SerialNumber == serial) == null)
			{
				App.CurrentDevice = null;
			}
		}
		if (App.CurrentDevice == null)
		{
			CloseWindow?.Invoke(this, EventArgs.Empty);
			App.CurrentDevice = Devices.FirstOrDefault();
			UpdateCurrentDevice();
		}
	}

	public async void UpdateCurrentDevice()
	{
		if (App.CurrentDevice != null && !App.CurrentDevice.FirmwareSupported)
		{
			App.CurrentDevice.IsOkToExecute = false;
			SerialPopup.Entitlement = "Device needs firmware update.";
			SerialPopup.Entitle.Foreground = Brushes.Red;
			MainView.HasGamepacks = false;
			MainView.HasConfig = false;
			MainView.HasGears = false;
			MainView.NewsTxt.Visibility = Visibility.Hidden;
			UpgradeBanner = Visibility.Collapsed;
			SerialPopup.IsConnected = App.CurrentDevice != null;
			SerialPopup.IsLatestFirmwareInstalled = false;
			SerialPopup.Status = ((App.CurrentDevice == null) ? UI.SerialStatusNotConnected : UI.SerialStatusConnected);
			SerialPopup.Firmware = App.CurrentDevice?.Device?.ApplicationVersion ?? UI.DeviceNotDetected;
			SerialPopup.Serial = App.CurrentDevice?.Device?.SerialNumber ?? UI.DeviceNotDetected;
			SerialPopup.Device = App.CurrentDevice?.Device?.Product?.DisplayName ?? UI.DeviceNotDetected;
			MessageBox.Show(UI.FirmwareUpdateRequiredMessage, UI.FirmwareUpdateRequiredTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
			return;
		}
		if (App.CurrentDevice != null)
		{
			App.CurrentDevice.IsOkToExecute = true;
		}
		CustomProgressDialog dialog = new CustomProgressDialog("Checking Device...");
		await DialogManager.ShowMetroDialogAsync((MetroWindow)(object)this, (BaseMetroDialog)(object)dialog, (MetroDialogSettings)null);
		SerialPopup.IsConnected = App.CurrentDevice != null;
		SerialPopup.IsLatestFirmwareInstalled = App.CurrentDevice?.Device?.IsLatestFirmwareInstalled;
		SerialPopup.Status = ((App.CurrentDevice == null) ? UI.SerialStatusNotConnected : UI.SerialStatusConnected);
		SerialPopup.Firmware = App.CurrentDevice?.Device?.ApplicationVersion ?? UI.DeviceNotDetected;
		SerialPopup.Serial = App.CurrentDevice?.Device?.SerialNumber ?? UI.DeviceNotDetected;
		SerialPopup.Device = App.CurrentDevice?.Device?.Product?.DisplayName ?? UI.DeviceNotDetected;
		MainWindow mainWindow = this;
		IDeviceState currentDevice = App.CurrentDevice;
		mainWindow.UpgradeBanner = ((currentDevice == null || currentDevice.Device?.SupportsFpsGamepacks != false) ? Visibility.Collapsed : Visibility.Visible);
		if (App.CurrentDevice != null && App.CurrentDevice.Device.Platform == 1)
		{
			MainView.TopL.Source = new BitmapImage(new Uri("/Resources/XB1_Controller_200px.png", UriKind.RelativeOrAbsolute));
		}
		else if (App.CurrentDevice != null && App.CurrentDevice.Device.Platform == 2)
		{
			MainView.TopL.Source = new BitmapImage(new Uri("/Resources/PS4_Controller_200px.png", UriKind.RelativeOrAbsolute));
		}
		if (App.CurrentDevice != null && App.CurrentDevice.Device?.Product != null && App.CurrentDevice.Device.SupportsFpsGamepacks)
		{
			string[] array = CheckSerialStatus(SerialPopup.Serial);
			if (array[0] != "Error")
			{
				bool flag = Convert.ToBoolean(Convert.ToInt16(array[3]));
				App.CurrentDevice.Device.HasSubscription = Convert.ToBoolean(Convert.ToInt16(array[4]));
				DateTime.Parse(array[0]);
				DateTime entitlementEnd = DateTime.Parse(array[1]);
				MainView.News = int.Parse(array[2]);
				DateTime todayDate = DateTime.Now;
				if (App.CurrentDevice.Device.Product != null && App.CurrentDevice.Device.HasSubscription && !App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.IsPro))
				{
					UpgradeBanner = Visibility.Collapsed;
					if (flag)
					{
						SerialPopup.Serial += "-B";
						SerialPopup.Entitle.Foreground = Brushes.DarkRed;
						SerialPopup.Entitlement = "Device Blacklisted";
						App.CurrentDevice.Device.HasSubscription = false;
						MainView.HasGamepacks = false;
						MainView.HasGears = false;
						MainGrid.IsEnabled = false;
						MainView.NewsTxt.Visibility = Visibility.Hidden;
						using (CmPpDfuDeviceBackend cmPpDfuDeviceBackend = App.CurrentDevice.Device.GetDeviceBackend())
						{
							cmPpDfuDeviceBackend.ClearErrorAndAbort();
							Helper.EraseEfs(cmPpDfuDeviceBackend);
							cmPpDfuDeviceBackend.Manifest();
						}
						await DialogManager.ShowMessageAsync((MetroWindow)(object)this, "Device Blacklisted", "We are sorry, but this device is blacklisted.\n\nPlease contact support for more information.", (MessageDialogStyle)0, (MetroDialogSettings)null);
					}
					else
					{
						if (MainView.News > 0)
						{
							MainView.NewsTxt.Visibility = Visibility.Visible;
						}
						if (entitlementEnd >= todayDate)
						{
							if ((entitlementEnd - todayDate).TotalDays < 30.0)
							{
								UpgradeBanner = Visibility.Visible;
								RenewGb.Header = "Click on the banner below to renew your Mod Pass.";
							}
							SerialPopup.Entitlement = entitlementEnd.ToString("D", CultureInfo.InvariantCulture);
							SerialPopup.Entitle.Foreground = Brushes.Green;
							if (!(App.CurrentDevice?.Device?.IsLatestFirmwareInstalled).Value)
							{
								await DialogManager.ShowMessageAsync((MetroWindow)(object)this, "New Firmware", "A new firmware has been released, please run our updater before using this software.", (MessageDialogStyle)0, (MetroDialogSettings)null);
								MainView.HasGamepacks = true;
								MainView.HasConfig = true;
								MainView.HasGears = false;
								MainGrid.IsEnabled = false;
							}
							else
							{
								MainView.HasGamepacks = true;
								MainView.HasConfig = true;
								MainView.HasGears = false;
								if (App.CurrentDevice != null && App.CurrentDevice.Device != null)
								{
									MainGrid.IsEnabled = App.CurrentDevice.Device.SupportsFpsGamepacks;
								}
							}
						}
						else
						{
							await DialogManager.ShowMessageAsync((MetroWindow)(object)this, "Subscription Expired", "Your subscription expired. Please renew it to continue using this software.\n\nExpire Date: " + entitlementEnd.ToString("D"), (MessageDialogStyle)0, (MetroDialogSettings)null);
							UpgradeBanner = Visibility.Visible;
							RenewGb.Header = "RENEW NOW TO CONTINUE USING YOUR STRIKEPACK POWERS!";
							MainView.HasGamepacks = false;
							MainView.HasConfig = true;
							MainView.HasGears = true;
							MainGrid.IsEnabled = true;
							App.CurrentDevice.Device.HasSubscription = false;
							if ((todayDate - entitlementEnd).TotalDays > 15.0)
							{
								using (CmPpDfuDeviceBackend dev = App.CurrentDevice.Device.GetDeviceBackend())
								{
									if (Helper.GetMeta(dev, 1).GamePackId == 59)
									{
										MainView.HasGears = true;
									}
								}
								using CmPpDfuDeviceBackend cmPpDfuDeviceBackend2 = App.CurrentDevice.Device.GetDeviceBackend();
								cmPpDfuDeviceBackend2.ClearErrorAndAbort();
								Helper.EraseEfs(cmPpDfuDeviceBackend2);
								cmPpDfuDeviceBackend2.Manifest();
							}
						}
					}
				}
				else
				{
					if (App.CurrentDevice.Device.Product != null && !App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.IsPro))
					{
						SerialPopup.Entitlement = "No Subscription";
						SerialPopup.Entitle.Foreground = Brushes.Red;
						MainView.HasGamepacks = false;
						MainView.HasGears = false;
						App.CurrentDevice.Device.HasSubscription = false;
						using (CmPpDfuDeviceBackend dev2 = App.CurrentDevice.Device.GetDeviceBackend())
						{
							if (Helper.GetMeta(dev2, 1).GamePackId == 59)
							{
								MainView.HasGears = true;
							}
						}
						UpgradeBanner = Visibility.Visible;
					}
					if (App.CurrentDevice.Device.Product != null && App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.IsPro))
					{
						SerialPopup.Entitlement = "Pro Version";
						SerialPopup.Entitle.Foreground = Brushes.DarkOrange;
						UpgradeBanner = Visibility.Collapsed;
					}
					MainView.HasGears = true;
					MainView.HasConfig = true;
					MainView.NewsTxt.Visibility = Visibility.Hidden;
					MainGrid.IsEnabled = true;
				}
			}
			else
			{
				await DialogManager.ShowMessageAsync((MetroWindow)(object)this, "Server Error", "Sorry, our server is currently unreachable.\n\nPlease check your connection and restart this app.", (MessageDialogStyle)0, (MetroDialogSettings)null);
				SerialPopup.Entitlement = "No data. Check connection and restart.";
				MainView.HasGamepacks = false;
				MainView.HasConfig = true;
				MainView.HasGears = false;
				using (CmPpDfuDeviceBackend dev3 = App.CurrentDevice.Device.GetDeviceBackend())
				{
					SlotMetaDataBase meta = Helper.GetMeta(dev3, 1);
					if (meta.GamePackId == 59 || meta.GamePackId == 24)
					{
						MainView.HasGears = true;
					}
				}
				UpgradeBanner = Visibility.Collapsed;
				App.CurrentDevice.Device.HasSubscription = false;
				MainView.News = 0;
				MainView.NewsTxt.Visibility = Visibility.Hidden;
				MainGrid.IsEnabled = true;
			}
		}
		else if (App.CurrentDevice == null)
		{
			SerialPopup.Entitlement = "Device Not Found.";
			SerialPopup.Entitle.Foreground = Brushes.Red;
			MainView.HasGamepacks = false;
			MainView.HasConfig = false;
			MainView.HasGears = false;
			MainView.NewsTxt.Visibility = Visibility.Hidden;
			UpgradeBanner = Visibility.Collapsed;
			MainGrid.IsEnabled = false;
		}
		else
		{
			SerialPopup.Entitlement = "Legacy Firmware. Please update.";
			SerialPopup.Entitle.Foreground = Brushes.Chocolate;
			MainView.HasGamepacks = false;
			MainView.HasConfig = true;
			MainView.HasGears = true;
			MainView.NewsTxt.Visibility = Visibility.Hidden;
			UpgradeBanner = Visibility.Visible;
			MainGrid.IsEnabled = true;
		}
		CommandManager.InvalidateRequerySuggested();
		await DialogManager.HideMetroDialogAsync((MetroWindow)(object)this, (BaseMetroDialog)(object)dialog, (MetroDialogSettings)null);
	}

	private void ShowDeviceSelector(object sender, RoutedEventArgs e)
	{
		((Window)(object)new DeviceSelector(this)).ShowDialog();
	}

	private void Close_Click(object sender, RoutedEventArgs e)
	{
		((Window)this).Close();
	}

	private void DeviceConnected_CanExecute(object sender, CanExecuteRoutedEventArgs e)
	{
		e.CanExecute = App.CurrentDevice != null && App.CurrentDevice.IsOkToExecute;
	}

	public void SetState(bool state)
	{
		EnableDeviceSelectorButton = state;
		if (App.CurrentDevice != null)
		{
			App.CurrentDevice.IsOkToExecute = state;
		}
		MainGrid.IsEnabled = state && App.CurrentDevice != null;
		CommandManager.InvalidateRequerySuggested();
	}

	private async void FactoryReset_Executed(object sender, ExecutedRoutedEventArgs e)
	{
		using (new DisableUIContext())
		{
			_ = 1;
			try
			{
				await Task.Run(delegate
				{
					using CmPpDfuDeviceBackend cmPpDfuDeviceBackend = App.CurrentDevice.Device.GetDeviceBackend();
					cmPpDfuDeviceBackend.ClearErrorAndAbort();
					Helper.EraseEfs(cmPpDfuDeviceBackend);
					cmPpDfuDeviceBackend.Manifest();
				});
				await DialogManager.ShowMessageAsync((MetroWindow)(object)this, UI.SuccessTitle, UI.FactoryResetCompletedSuccessfully, (MessageDialogStyle)0, (MetroDialogSettings)null);
				CloseWindow?.Invoke(this, EventArgs.Empty);
			}
			catch (Exception exception)
			{
				TelemetryHelper.TrackException(exception);
				MessageBox.Show(UI.FactoryResetFailed, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
			}
		}
	}

	private void AlwaysExecutable_CanExecute(object sender, CanExecuteRoutedEventArgs e)
	{
		e.CanExecute = true;
	}

	private void About_Executed(object sender, ExecutedRoutedEventArgs e)
	{
		About about = new About();
		((Window)(object)about).Owner = (Window)(object)this;
		((Window)(object)about).ShowDialog();
	}

	private void Manual_Click(object sender, RoutedEventArgs e)
	{
		if (App.CurrentDevice != null)
		{
			Manual manual = new Manual();
			((Window)(object)manual).Owner = (Window)(object)this;
			((Window)(object)manual).ShowDialog();
		}
		else
		{
			MessageBox.Show("Please connect a Strikepack.", "No device found");
		}
		e.Handled = true;
	}

	private void GamePack_Click(object sender, RoutedEventArgs e)
	{
		Process.Start(new ProcessStartInfo("https://www.modpass.ca/game-packs"));
		e.Handled = true;
	}

	private void Firmware_Click(object sender, RoutedEventArgs e)
	{
		if (App.CurrentDevice != null)
		{
			ExtractIcon("cmupdatetool.exe");
			string path = Environment.GetCommandLineArgs()[0];
			Process process = new Process();
			process.StartInfo = new ProcessStartInfo
			{
				FileName = Path.Combine(Environment.CurrentDirectory, "cmupdatetool.exe"),
				WorkingDirectory = Path.GetDirectoryName(path)
			};
			process.StartInfo.EnvironmentVariables["CM_SGI_INFINITE_WAIT"] = "1";
			process.StartInfo.UseShellExecute = false;
			process.Start();
			Application.Current.Shutdown();
		}
		else
		{
			MessageBox.Show("Please connect a Strikepack.", "No device found");
			e.Handled = true;
		}
	}

	internal static void ExtractIcon(string name, string resourcepath = "")
	{
		if (string.IsNullOrWhiteSpace(resourcepath))
		{
			resourcepath = $"StrikePackCfg.Resources.{name}";
		}
		using Stream stream = Assembly.GetAssembly(typeof(App)).GetManifestResourceStream(resourcepath);
		if (stream == null)
		{
			PrintError(new FileNotFoundException("Unable to find resource '" + resourcepath + "'"));
			return;
		}
		using FileStream fileStream = File.OpenWrite(name);
		do
		{
			byte[] array = new byte[4096];
			int num = stream.Read(array, 0, array.Length);
			if (num > 0)
			{
				fileStream.Write(array, 0, num);
			}
		}
		while (stream.Position < stream.Length);
		stream.Close();
		fileStream.Close();
	}

	public static void PrintError(Exception ex)
	{
		TelemetryHelper.TrackException(ex);
	}

	private async void UpdateNow_Click(object sender, RoutedEventArgs e)
	{
		CustomProgressDialog dialog = new CustomProgressDialog(UI.FetchingApplicationUpdate);
		await DialogManager.ShowMetroDialogAsync((MetroWindow)(object)this, (BaseMetroDialog)(object)dialog, (MetroDialogSettings)null);
		try
		{
			await App.PerformUpdate(await App.FetchUpdate(UpdateManager));
		}
		catch (Exception exception)
		{
			TelemetryHelper.TrackException(exception);
		}
		await DialogManager.HideMetroDialogAsync((MetroWindow)(object)this, (BaseMetroDialog)(object)dialog, (MetroDialogSettings)null);
	}

	private void Update_Click(object sender, RoutedEventArgs e)
	{
		if (MessageBox.Show("This will close the application and open the subscription page.\n\nConfirm?", "Start subscription Process.", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
		{
			string d = "#" + App.CurrentDevice.Device.SerialNumber + "#" + App.CurrentDevice.Device.ApplicationVersion + "#" + App.CurrentDevice.Device.Platform + "#" + WindowsIdentity.GetCurrent().Name.ToString();
			Process.Start($"http://shop.ctrlmaxplus.ca/?d={XXTEA.Urldata(d)}");
			Application.Current.Shutdown();
			e.Handled = true;
		}
	}

	private void SerialStatus_MouseUp(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left)
		{
			if (SerialPopup.IsOpen)
			{
				SerialPopup.Hide();
			}
			else
			{
				SerialPopup.Show();
			}
		}
	}

	private void SerialStatus_MouseEnter(object sender, MouseEventArgs e)
	{
		SerialPopup.Show();
	}

	public static string[] CheckSerialStatus(string deviceSerial)
	{
		string[] result = new string[4] { "Error", "Invalid", "0", "0" };
		try
		{
			string d = "#" + App.CurrentDevice.Device.SerialNumber + "#" + App.CurrentDevice.Device.ApplicationVersion + "#" + App.CurrentDevice.Device.Platform;
			HttpWebRequest obj = (HttpWebRequest)WebRequest.Create($"http://gp.ctrlmaxplus.ca/?d={XXTEA.Urldata(d)}");
			obj.UserAgent = "StrikePackCFG";
			using Stream stream = obj.GetResponse().GetResponseStream();
			if (stream == null)
			{
				return result;
			}
			ApiResponseEntitlement apiResponseEntitlement = (ApiResponseEntitlement)new DataContractJsonSerializer(typeof(ApiResponseEntitlement)).ReadObject(stream);
			return new string[5] { apiResponseEntitlement.Start, apiResponseEntitlement.End, apiResponseEntitlement.News, apiResponseEntitlement.Bl, apiResponseEntitlement.Auth };
		}
		catch (Exception)
		{
			return result;
		}
	}

	private void tw_click(object sender, RoutedEventArgs e)
	{
		Process.Start("https://twitter.com/cmgamingco");
		e.Handled = true;
	}

	private void yt_click(object sender, RoutedEventArgs e)
	{
		Process.Start("https://www.youtube.com/channel/UC1Sci7HUKgCJaV1ynyzi_jw");
		e.Handled = true;
	}

	private void fb_click(object sender, RoutedEventArgs e)
	{
		Process.Start("https://www.facebook.com/CMGamingCo");
		e.Handled = true;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/ModCentral;component/windows/mainwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((CommandBinding)target).CanExecute += DeviceConnected_CanExecute;
			((CommandBinding)target).Executed += FactoryReset_Executed;
			break;
		case 2:
			((CommandBinding)target).CanExecute += AlwaysExecutable_CanExecute;
			((CommandBinding)target).Executed += About_Executed;
			break;
		case 3:
			SerialPopup = (SerialPopup)target;
			break;
		case 4:
			((MenuItem)target).Click += Close_Click;
			break;
		case 5:
			((MenuItem)target).Click += Manual_Click;
			break;
		case 6:
			((MenuItem)target).Click += GamePack_Click;
			break;
		case 7:
			((MenuItem)target).Click += Firmware_Click;
			break;
		case 8:
			((Button)target).Click += UpdateNow_Click;
			break;
		case 9:
			SerialStatusBarItem = (StatusBarItem)target;
			SerialStatusBarItem.MouseUp += SerialStatus_MouseUp;
			break;
		case 10:
			((Button)target).Click += ShowDeviceSelector;
			break;
		case 11:
			((Button)target).Click += tw_click;
			break;
		case 12:
			((Button)target).Click += fb_click;
			break;
		case 13:
			((Button)target).Click += yt_click;
			break;
		case 14:
			RenewGb = (GroupBox)target;
			break;
		case 15:
			((Button)target).Click += Update_Click;
			break;
		case 16:
			MainGrid = (StackPanel)target;
			break;
		case 17:
			MainView = (SlotView)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}

	protected void _003C_003EOnPropertyChanged(PropertyChangedEventArgs eventArgs)
	{
		this.PropertyChanged?.Invoke(this, eventArgs);
	}
}
