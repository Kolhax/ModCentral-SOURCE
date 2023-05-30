using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CollectiveMinds.Common.Azure;
using DfuLib;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using StrikePackCfg.Backend;
using StrikePackCfg.Backend.EEProm;
using StrikePackCfg.Gamepacks;
using StrikePackCfg.HelperClasses;
using StrikePackCfg.Views;

namespace StrikePackCfg.Windows;

public class UserSlotConfigWindow : MetroWindow, INotifyPropertyChanged, IComponentConnector
{
	private int _flashUsage;

	public static string Url;

	public ushort[] CurrentsGamepack = new ushort[10];

	private bool _mainClosing;

	internal StackPanel SlotsPanel;

	internal ProgressBar flashPb;

	internal TextBlock flashUsage;

	internal Button SaveBtn;

	private bool _contentLoaded;

	public int FlashUsage
	{
		get
		{
			return _flashUsage;
		}
		set
		{
			if (_flashUsage != value)
			{
				_flashUsage = value;
				RaisePropertyChanged("FlashUsageMsg");
				RaisePropertyChanged("FlashUsage");
				RaisePropertyChanged("NumRecsLoaded");
				RaisePropertyChanged("RecsLoadedMessage");
			}
		}
	}

	public string FlashUsageMsg => $"Flash Usage: {FlashUsage}% of 128Kb";

	public event PropertyChangedEventHandler PropertyChanged = delegate
	{
	};

	public UserSlotConfigWindow()
	{
		InitializeComponent();
		((FrameworkElement)this).DataContext = this;
		FlashUsage = 0;
		((Window)this).Owner = (Window)(object)App.MainWin;
		MainWindow mainWin = App.MainWin;
		mainWin.CloseWindow = (EventHandler)Delegate.Combine(mainWin.CloseWindow, new EventHandler(CloseWindow2));
		((Window)this).Closing += delegate
		{
			MainWindow mainWin2 = App.MainWin;
			mainWin2.CloseWindow = (EventHandler)Delegate.Remove(mainWin2.CloseWindow, new EventHandler(CloseWindow2));
		};
		using (CmPpDfuDeviceBackend dev = App.CurrentDevice.Device.GetDeviceBackend())
		{
			for (int i = 1; i <= App.CurrentDevice.Device.Product.UserSlots + 1; i++)
			{
				SlotMetaDataBase meta = Helper.GetMeta(dev, i);
				CurrentsGamepack[i - 1] = meta.GamePackId;
			}
		}
		SlotsPanel.Children.Clear();
		((Window)this).Title = "Available FPSPacks and GamePacks";
		for (int j = 1; j <= App.CurrentDevice.Device.Product.UserSlots + 1; j++)
		{
			SlotsPanel.Children.Add(new UserSlotView(j)
			{
				Slot = j
			});
		}
		UserSlotViewModel.Instance.PropertyChanged += UserSlotViewModelOnPropertyChanged;
	}

	private void CloseWindow2(object sender, EventArgs e)
	{
		_mainClosing = true;
		UserSlotViewModel.Instance.PropertyChanged -= UserSlotViewModelOnPropertyChanged;
		((Window)this).Close();
	}

	private void RaisePropertyChanged(string propName)
	{
		this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
	}

	private void UserSlotViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
	{
		FlashUsage = 0;
		ObservableCollection<GamepackData> gamepacks = UserSlotViewModel.Instance.Gamepacks;
		if (gamepacks == null || !gamepacks.Any())
		{
			return;
		}
		int i;
		for (i = 1; i <= App.CurrentDevice.Device.Product.UserSlots + 1; i++)
		{
			UserSlotView userSlotView = SlotsPanel.Children.OfType<UserSlotView>().FirstOrDefault((UserSlotView s) => s.Slot == i);
			if (userSlotView == null)
			{
				continue;
			}
			if (i == 1)
			{
				GamepackData selectedItem = UserSlotViewModel.Instance.Modpacks.FirstOrDefault((GamepackData g) => g.Id == CurrentsGamepack[i - 1]);
				userSlotView.SelectedItem = selectedItem;
			}
			else
			{
				GamepackData selectedItem2 = UserSlotViewModel.Instance.Gamepacks.FirstOrDefault((GamepackData g) => g.Id == CurrentsGamepack[i - 1]);
				userSlotView.SelectedItem = selectedItem2;
			}
		}
	}

	private void CloseWindow(object sender, CancelEventArgs e)
	{
		if (_mainClosing)
		{
			_mainClosing = false;
			return;
		}
		MainWindow mainWin = App.MainWin;
		mainWin.CloseWindow = (EventHandler)Delegate.Combine(mainWin.CloseWindow, new EventHandler(CloseWindow2));
		e.Cancel = true;
		((Window)this).Hide();
	}

	private async void Save_Slots(object sender, RoutedEventArgs e)
	{
		try
		{
			SaveBtn.IsEnabled = false;
			int num = 0;
			int num2 = 0;
			for (int j = 1; j < App.CurrentDevice.Device.Product.UserSlots + 1; j++)
			{
				UserSlotView userSlotView = (UserSlotView)SlotsPanel.Children[j];
				if (userSlotView.SelectedItem == null || userSlotView.SelectedItem.Id == 0)
				{
					if (num == 0)
					{
						num = j;
					}
				}
				else
				{
					num2 = j;
				}
			}
			if (num > 0 && num2 > num)
			{
				MessageBox.Show("Cannot proceed with save due to BLANK game pack slot in sequence.");
				return;
			}
			using (CmPpDfuDeviceBackend dev = App.CurrentDevice.Device.GetDeviceBackend())
			{
				for (int k = 1; k <= App.CurrentDevice.Device.Product.UserSlots + 1; k++)
				{
					SlotMetaDataBase meta = Helper.GetMeta(dev, k);
					CurrentsGamepack[k - 1] = meta.GamePackId;
				}
			}
			CustomProgressDialog dialog = new CustomProgressDialog("Saving Please Wait...");
			await DialogManager.ShowMetroDialogAsync((MetroWindow)(object)this, (BaseMetroDialog)(object)dialog, (MetroDialogSettings)null);
			_ = new byte[16];
			int bytesWritten = 0;
			int gpackFlashed = 0;
			using (CmPpDfuDeviceBackend cmPpDfuDeviceBackend = App.CurrentDevice.Device.GetDeviceBackend())
			{
				cmPpDfuDeviceBackend.FlashFormat();
			}
			for (int i = 0; i < App.CurrentDevice.Device.Product.UserSlots + 1; i++)
			{
				UserSlotView userSlotView2 = (UserSlotView)SlotsPanel.Children[i];
				if (userSlotView2.SelectedItem == null || userSlotView2.SelectedItem.Id <= 0)
				{
					await Helper.SaveConfig(new KeyValuePair<int, short>[0], i + 1, 0, 0);
					continue;
				}
				int id = userSlotView2.SelectedItem.Id;
				if (CurrentsGamepack[i] != id)
				{
					await Helper.SaveConfig(new KeyValuePair<int, short>[0], i + 1, 0, (ushort)id);
				}
				GamepackPackage gamepackPackage = GamepackPackageHelper.FlashGamepackPackage(id, App.CurrentDevice.Device.SerialNumber);
				byte[] data;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						binaryWriter.Write(gamepackPackage.Bytecode, 0, 16);
						binaryWriter.Write((uint)gamepackPackage.ID);
						binaryWriter.Write((uint)(gamepackPackage.Bytecode.Length - 16));
						binaryWriter.Write(1u);
						binaryWriter.Flush();
						binaryWriter.Write(new byte[1024 - binaryWriter.BaseStream.Position]);
						binaryWriter.Write(gamepackPackage.Bytecode, 16, gamepackPackage.Bytecode.Length - 16);
					}
					data = memoryStream.ToArray();
				}
				FlashHelper.WriteToFlashAes(data);
				gpackFlashed++;
				bytesWritten += gamepackPackage.Bytecode.Length - 16;
			}
			await DialogManager.HideMetroDialogAsync((MetroWindow)(object)this, (BaseMetroDialog)(object)dialog, (MetroDialogSettings)null);
			await DialogManager.ShowMessageAsync((MetroWindow)(object)this, "Success", "GamePacks Flashed: " + gpackFlashed + "\n\nTotal Bytes Written: " + bytesWritten + "\n\nFlash Used: " + (int)Math.Round((double)(100 * bytesWritten) / 131072.0) + "%", (MessageDialogStyle)0, (MetroDialogSettings)null);
			SaveBtn.IsEnabled = true;
		}
		catch (Exception exception)
		{
			TelemetryHelper.TrackException(exception);
			MessageBox.Show("Error flashing device.\n\n Application will now close", "Error flashing", MessageBoxButton.OK, MessageBoxImage.Hand);
			Environment.Exit(-1);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/ModCentral;component/windows/userslotconfigwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			((Window)(object)(UserSlotConfigWindow)target).Closing += CloseWindow;
			break;
		case 2:
			SlotsPanel = (StackPanel)target;
			break;
		case 3:
			flashPb = (ProgressBar)target;
			break;
		case 4:
			flashUsage = (TextBlock)target;
			break;
		case 5:
			SaveBtn = (Button)target;
			SaveBtn.Click += Save_Slots;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
