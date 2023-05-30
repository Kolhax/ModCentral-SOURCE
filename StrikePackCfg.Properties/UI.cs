using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace StrikePackCfg.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
[CompilerGenerated]
[DebuggerNonUserCode]
public class UI
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("StrikePackCfg.Properties.UI", typeof(UI).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	public static string About => ResourceManager.GetString("About", resourceCulture);

	public static string AdvancedSettingsHeader => ResourceManager.GetString("AdvancedSettingsHeader", resourceCulture);

	public static string AnUpdateIsAvailable => ResourceManager.GetString("AnUpdateIsAvailable", resourceCulture);

	public static string ClickToShowDeviceInfo => ResourceManager.GetString("ClickToShowDeviceInfo", resourceCulture);

	public static string Close => ResourceManager.GetString("Close", resourceCulture);

	public static string ConfigCorrupt => ResourceManager.GetString("ConfigCorrupt", resourceCulture);

	public static string ConfigDifferentVersion => ResourceManager.GetString("ConfigDifferentVersion", resourceCulture);

	public static string ConfigExportSuccessful => ResourceManager.GetString("ConfigExportSuccessful", resourceCulture);

	public static string ConfigImportSuccessful => ResourceManager.GetString("ConfigImportSuccessful", resourceCulture);

	public static string ConfigIncompatibleFileFormat => ResourceManager.GetString("ConfigIncompatibleFileFormat", resourceCulture);

	public static string ConfigReadError => ResourceManager.GetString("ConfigReadError", resourceCulture);

	public static string ConfigToolTip => ResourceManager.GetString("ConfigToolTip", resourceCulture);

	public static string ConfiguratorTitle => ResourceManager.GetString("ConfiguratorTitle", resourceCulture);

	public static string ConfiguratorTitleEmb => ResourceManager.GetString("ConfiguratorTitleEmb", resourceCulture);

	public static string ConfigWriteError => ResourceManager.GetString("ConfigWriteError", resourceCulture);

	public static string CurrentlyOnLatestVersion => ResourceManager.GetString("CurrentlyOnLatestVersion", resourceCulture);

	public static string DeviceNotDetected => ResourceManager.GetString("DeviceNotDetected", resourceCulture);

	public static string DisableRumble => ResourceManager.GetString("DisableRumble", resourceCulture);

	public static string DisableRumbleToolTip => ResourceManager.GetString("DisableRumbleToolTip", resourceCulture);

	public static string DominatorHeader => ResourceManager.GetString("DominatorHeader", resourceCulture);

	public static string DominatorModeRemapper => ResourceManager.GetString("DominatorModeRemapper", resourceCulture);

	public static string ErrorTitle => ResourceManager.GetString("ErrorTitle", resourceCulture);

	public static string ExportSettings => ResourceManager.GetString("ExportSettings", resourceCulture);

	public static string ExportSettingsToolTip => ResourceManager.GetString("ExportSettingsToolTip", resourceCulture);

	public static string FactoryReset => ResourceManager.GetString("FactoryReset", resourceCulture);

	public static string FactoryResetCompletedSuccessfully => ResourceManager.GetString("FactoryResetCompletedSuccessfully", resourceCulture);

	public static string FactoryResetFailed => ResourceManager.GetString("FactoryResetFailed", resourceCulture);

	public static string FetchingApplicationUpdate => ResourceManager.GetString("FetchingApplicationUpdate", resourceCulture);

	public static string File => ResourceManager.GetString("File", resourceCulture);

	public static string FirmwareDisplay => ResourceManager.GetString("FirmwareDisplay", resourceCulture);

	public static string FirmwareUpdateAvailable => ResourceManager.GetString("FirmwareUpdateAvailable", resourceCulture);

	public static string FirmwareUpdateRequiredMessage => ResourceManager.GetString("FirmwareUpdateRequiredMessage", resourceCulture);

	public static string FirmwareUpdateRequiredTitle => ResourceManager.GetString("FirmwareUpdateRequiredTitle", resourceCulture);

	public static string Help => ResourceManager.GetString("Help", resourceCulture);

	public static string ImportSettings => ResourceManager.GetString("ImportSettings", resourceCulture);

	public static string ImportSettingsToolTip => ResourceManager.GetString("ImportSettingsToolTip", resourceCulture);

	public static string LatestFirmwareInstalled => ResourceManager.GetString("LatestFirmwareInstalled", resourceCulture);

	public static string MandatoryUpdateAvailable => ResourceManager.GetString("MandatoryUpdateAvailable", resourceCulture);

	public static string MandatoryUpdateFound => ResourceManager.GetString("MandatoryUpdateFound", resourceCulture);

	public static string MirrorConfiguration => ResourceManager.GetString("MirrorConfiguration", resourceCulture);

	public static string MirrorConfigurationToolTip => ResourceManager.GetString("MirrorConfigurationToolTip", resourceCulture);

	public static string NoDevicesFound => ResourceManager.GetString("NoDevicesFound", resourceCulture);

	public static string NotAllowedToRunMessage => ResourceManager.GetString("NotAllowedToRunMessage", resourceCulture);

	public static string NoUpdateAvailable => ResourceManager.GetString("NoUpdateAvailable", resourceCulture);

	public static string OkButtonText => ResourceManager.GetString("OkButtonText", resourceCulture);

	public static string PaddleLeft => ResourceManager.GetString("PaddleLeft", resourceCulture);

	public static string PaddleLeftToolTip => ResourceManager.GetString("PaddleLeftToolTip", resourceCulture);

	public static string PaddleRight => ResourceManager.GetString("PaddleRight", resourceCulture);

	public static string PaddleRightToolTip => ResourceManager.GetString("PaddleRightToolTip", resourceCulture);

	public static string RemapperNotAvailable => ResourceManager.GetString("RemapperNotAvailable", resourceCulture);

	public static string RemapperReadError => ResourceManager.GetString("RemapperReadError", resourceCulture);

	public static string RemapperToolTip => ResourceManager.GetString("RemapperToolTip", resourceCulture);

	public static string RemapperWriteError => ResourceManager.GetString("RemapperWriteError", resourceCulture);

	public static string ResetToDefault => ResourceManager.GetString("ResetToDefault", resourceCulture);

	public static string ResetToDefaultToolTipConfig => ResourceManager.GetString("ResetToDefaultToolTipConfig", resourceCulture);

	public static string ResetToDefaultToolTipRemapper => ResourceManager.GetString("ResetToDefaultToolTipRemapper", resourceCulture);

	public static string ResetToDefaultWarningConfig => ResourceManager.GetString("ResetToDefaultWarningConfig", resourceCulture);

	public static string ResetToDefaultWarningRemapper => ResourceManager.GetString("ResetToDefaultWarningRemapper", resourceCulture);

	public static string SaveConfiguration => ResourceManager.GetString("SaveConfiguration", resourceCulture);

	public static string SaveConfigurationToolTip => ResourceManager.GetString("SaveConfigurationToolTip", resourceCulture);

	public static string SaveSettingsToStrikePack => ResourceManager.GetString("SaveSettingsToStrikePack", resourceCulture);

	public static string SelectDevice => ResourceManager.GetString("SelectDevice", resourceCulture);

	public static string SelectExportedConfigFor => ResourceManager.GetString("SelectExportedConfigFor", resourceCulture);

	public static string SelectSaveLocationConfigFor => ResourceManager.GetString("SelectSaveLocationConfigFor", resourceCulture);

	public static string SerialDisplay => ResourceManager.GetString("SerialDisplay", resourceCulture);

	public static string SerialStatusConnected => ResourceManager.GetString("SerialStatusConnected", resourceCulture);

	public static string SerialStatusNotConnected => ResourceManager.GetString("SerialStatusNotConnected", resourceCulture);

	public static string ShowAdvancedSettings => ResourceManager.GetString("ShowAdvancedSettings", resourceCulture);

	public static string ShowAdvancedSettingsTooltipLeft => ResourceManager.GetString("ShowAdvancedSettingsTooltipLeft", resourceCulture);

	public static string ShowAdvancedSettingsTooltipRight => ResourceManager.GetString("ShowAdvancedSettingsTooltipRight", resourceCulture);

	public static string spcfgFilter => ResourceManager.GetString("spcfgFilter", resourceCulture);

	public static string StatusTooltipFirmware => ResourceManager.GetString("StatusTooltipFirmware", resourceCulture);

	public static string StatusTooltipFirmwareStatus => ResourceManager.GetString("StatusTooltipFirmwareStatus", resourceCulture);

	public static string StatusTooltipSerial => ResourceManager.GetString("StatusTooltipSerial", resourceCulture);

	public static string StatusTooltipStatus => ResourceManager.GetString("StatusTooltipStatus", resourceCulture);

	public static string StatusTooltipTitle => ResourceManager.GetString("StatusTooltipTitle", resourceCulture);

	public static string StrikePackConfiguratorTitle => ResourceManager.GetString("StrikePackConfiguratorTitle", resourceCulture);

	public static string SuccessfullySavedYourSettings => ResourceManager.GetString("SuccessfullySavedYourSettings", resourceCulture);

	public static string SuccessTitle => ResourceManager.GetString("SuccessTitle", resourceCulture);

	public static string Tools => ResourceManager.GetString("Tools", resourceCulture);

	public static string TournamentHeader => ResourceManager.GetString("TournamentHeader", resourceCulture);

	public static string TournamentModeRemapper => ResourceManager.GetString("TournamentModeRemapper", resourceCulture);

	public static string TradeMarksText => ResourceManager.GetString("TradeMarksText", resourceCulture);

	public static string UnhandledError => ResourceManager.GetString("UnhandledError", resourceCulture);

	public static string UnknownFirmwareStatus => ResourceManager.GetString("UnknownFirmwareStatus", resourceCulture);

	public static string UpdateNow => ResourceManager.GetString("UpdateNow", resourceCulture);

	public static string UpgradeText => ResourceManager.GetString("UpgradeText", resourceCulture);

	public static string UserHairTriggers => ResourceManager.GetString("UserHairTriggers", resourceCulture);

	public static string UserHairTriggersToolTip => ResourceManager.GetString("UserHairTriggersToolTip", resourceCulture);

	public static string VisitStrikepackCom => ResourceManager.GetString("VisitStrikepackCom", resourceCulture);

	public static string WarningTitle => ResourceManager.GetString("WarningTitle", resourceCulture);

	public static string WrongConfigGamepackID => ResourceManager.GetString("WrongConfigGamepackID", resourceCulture);

	internal UI()
	{
	}
}
