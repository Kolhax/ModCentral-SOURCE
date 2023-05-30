using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CollectiveMinds.Commmon.Network;
using CollectiveMinds.Common.Azure;
using StrikePackCfg.HelperClasses;

namespace StrikePackCfg.Gamepacks;

public sealed class UserSlotViewModel : INotifyPropertyChanged
{
	public ObservableCollection<GamepackData> Gamepacks { get; } = new ObservableCollection<GamepackData>();


	public ObservableCollection<GamepackData> Modpacks { get; } = new ObservableCollection<GamepackData>();


	public static UserSlotViewModel Instance { get; } = new UserSlotViewModel();


	public event PropertyChangedEventHandler PropertyChanged;

	private UserSlotViewModel()
	{
	}

	public async void UpdateGamepackList()
	{
		string d = "#" + App.CurrentDevice.Device.SerialNumber + "#" + App.CurrentDevice.Device.ApplicationVersion + "#" + App.CurrentDevice.Device.Platform;
		string url = "http://gp.ctrlmaxplus.ca/mods.php?d=" + XXTEA.Urldata(d);
		try
		{
			GamepackData[] obj = await NetworkHelper.FetchManifest<GamepackData[]>(url);
			Gamepacks.Clear();
			Modpacks.Clear();
			Gamepacks.Add(GamepackData.Empty);
			GamepackData[] array = obj;
			foreach (GamepackData gamepackData in array)
			{
				if (gamepackData.ModPack)
				{
					Modpacks.Add(gamepackData);
				}
				else
				{
					Gamepacks.Add(gamepackData);
				}
			}
		}
		catch (Exception exception)
		{
			MessageBox.Show("Error downloading data.\n\n Application will close.");
			TelemetryHelper.TrackException(exception);
			Environment.Exit(-1);
		}
		finally
		{
			OnPropertyChanged("Gamepacks");
		}
	}

	private void OnPropertyChanged(string propertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
