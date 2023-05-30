using System.ComponentModel;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
internal static class _003C_003EPropertyChangedEventArgs
{
	internal static readonly PropertyChangedEventArgs Device = new PropertyChangedEventArgs("Device");

	internal static readonly PropertyChangedEventArgs EnableDeviceSelectorButton = new PropertyChangedEventArgs("EnableDeviceSelectorButton");

	internal static readonly PropertyChangedEventArgs Entitlement = new PropertyChangedEventArgs("Entitlement");

	internal static readonly PropertyChangedEventArgs Firmware = new PropertyChangedEventArgs("Firmware");

	internal static readonly PropertyChangedEventArgs HasConfig = new PropertyChangedEventArgs("HasConfig");

	internal static readonly PropertyChangedEventArgs HasGamepacks = new PropertyChangedEventArgs("HasGamepacks");

	internal static readonly PropertyChangedEventArgs HasGears = new PropertyChangedEventArgs("HasGears");

	internal static readonly PropertyChangedEventArgs Header = new PropertyChangedEventArgs("Header");

	internal static readonly PropertyChangedEventArgs IsConnected = new PropertyChangedEventArgs("IsConnected");

	internal static readonly PropertyChangedEventArgs IsLatestFirmwareInstalled = new PropertyChangedEventArgs("IsLatestFirmwareInstalled");

	internal static readonly PropertyChangedEventArgs IsOpen = new PropertyChangedEventArgs("IsOpen");

	internal static readonly PropertyChangedEventArgs News = new PropertyChangedEventArgs("News");

	internal static readonly PropertyChangedEventArgs RemapTitle = new PropertyChangedEventArgs("RemapTitle");

	internal static readonly PropertyChangedEventArgs Serial = new PropertyChangedEventArgs("Serial");

	internal static readonly PropertyChangedEventArgs ShowDeviceSelectorButtonVisibility = new PropertyChangedEventArgs("ShowDeviceSelectorButtonVisibility");

	internal static readonly PropertyChangedEventArgs Slot = new PropertyChangedEventArgs("Slot");

	internal static readonly PropertyChangedEventArgs Status = new PropertyChangedEventArgs("Status");

	internal static readonly PropertyChangedEventArgs UpdateAvailableVisibility = new PropertyChangedEventArgs("UpdateAvailableVisibility");

	internal static readonly PropertyChangedEventArgs UpdateManager = new PropertyChangedEventArgs("UpdateManager");

	internal static readonly PropertyChangedEventArgs UpdateRequiredVisibility = new PropertyChangedEventArgs("UpdateRequiredVisibility");

	internal static readonly PropertyChangedEventArgs UpgradeBanner = new PropertyChangedEventArgs("UpgradeBanner");
}
