using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using CollectiveMinds.AppUpdate;
using CollectiveMinds.AppUpdate.Models;
using CollectiveMinds.Commmon.Network;
using CollectiveMinds.Common;
using CollectiveMinds.Common.Azure;
using Semver;
using StrikePackCfg.Backend;
using StrikePackCfg.Properties;
using StrikePackCfg.Windows;

namespace StrikePackCfg;

public partial class App : Application, IComponentConnector
{
	private const string UpdateUrl = "https://cmupdate.azureedge.net/appupdate/StrikePackCfg/update.xml";

	internal static IDeviceState CurrentDevice;

	internal static MainWindow MainWin;

	internal static string VersionString;

	private static string GetVersionString()
	{
		Assembly assembly = Assembly.GetAssembly(typeof(App));
		Type type = assembly.GetType(assembly.GetName().Name + ".GitVersionInformation");
		if (type != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(type.GetField("MajorMinorPatch").GetValue(null));
			stringBuilder.Append("-");
			string value = type.GetField("PreReleaseLabel").GetValue(null).ToString();
			if (!string.IsNullOrWhiteSpace(value))
			{
				stringBuilder.Append(value);
				stringBuilder.Append(type.GetField("PreReleaseNumber").GetValue(null));
				stringBuilder.Append("-");
			}
			stringBuilder.Append(type.GetField("BranchName").GetValue(null));
			stringBuilder.Append("-");
			stringBuilder.Append(type.GetField("Sha").GetValue(null).ToString()
				.Substring(0, 8));
			return stringBuilder.ToString();
		}
		return assembly.GetName().Version.ToString();
	}

	internal static SemVersion GetSemver()
	{
		Assembly assembly = Assembly.GetAssembly(typeof(App));
		Type type = assembly.GetType(assembly.GetName().Name + ".GitVersionInformation");
		if (type != null)
		{
			return type.GetField("SemVer").GetValue(null).ToString();
		}
		Version version = assembly.GetName().Version;
		return version.Major + "." + version.Minor + "." + version.Build;
	}

	private static string GetSha256Hash()
	{
		using HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();
		byte[] buffer = File.ReadAllBytes(new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath);
		return hashAlgorithm.ComputeHash(buffer).ToHexString();
	}

	private static async void GenerateManifest(bool requiredUpdate = false, bool generateNew = true)
	{
		using MemoryStream ms = new MemoryStream();
		XmlWriterSettings settings = new XmlWriterSettings
		{
			Encoding = Encoding.UTF8,
			Indent = true
		};
		using (XmlWriter xw = XmlWriter.Create(ms, settings))
		{
			DataContractSerializer serializer = new DataContractSerializer(typeof(ApplicationUpdateManifestV2));
			string url2 = "https://cmupdate.azureedge.net/appupdate/StrikePackCfg/";
			string text = $"{Path.GetFileNameWithoutExtension(new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath)}-{GetSemver()}.exe";
			File.WriteAllBytes(text, File.ReadAllBytes(new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath));
			url2 += text;
			ApplicationUpdateManifestV2 applicationUpdateManifestV;
			if (generateNew)
			{
				applicationUpdateManifestV = new ApplicationUpdateManifestV2
				{
					LatestAvailableApplicationSha256Hash = GetSha256Hash(),
					LatestAvailableApplicationUrl = url2,
					LatestAvailableApplicationVersion = GetSemver()
				};
				if (requiredUpdate)
				{
					applicationUpdateManifestV.MinimumRequiredApplicationVersion = GetSemver();
				}
			}
			else
			{
				applicationUpdateManifestV = await RetryHelper.RetryWithExponentialBackoff(() => NetworkHelper.FetchManifest<ApplicationUpdateManifestV2>("https://cmupdate.azureedge.net/appupdate/StrikePackCfg/update.xml"), 3, 500);
				applicationUpdateManifestV.LatestAvailableApplicationSha256Hash = GetSha256Hash();
				applicationUpdateManifestV.LatestAvailableApplicationVersion = GetSemver();
				applicationUpdateManifestV.LatestAvailableApplicationUrl = url2;
			}
			serializer.WriteObject(xw, applicationUpdateManifestV);
		}
		File.WriteAllBytes("update.xml", ms.ToArray());
	}

	[STAThread]
	public static int Main(string[] args)
	{
		try
		{
			File.Delete("update.xml");
		}
		catch
		{
		}
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		VersionString = GetVersionString();
		int i;
		if (args != null)
		{
			for (i = 0; i < args.Length; i++)
			{
				switch (args[i].ToLowerInvariant())
				{
				case "--generate-update":
					GenerateManifest();
					break;
				case "--generate-forced-update":
					GenerateManifest(requiredUpdate: true);
					break;
				case "--generate-current-update":
					GenerateManifest(requiredUpdate: false, generateNew: false);
					break;
				}
			}
		}
		TimeSpan waitTime = TimeSpan.Zero;
		if (ApplicationUpdateManager.ApplicationWasRelaunched)
		{
			waitTime = TimeSpan.FromMinutes(1.0);
		}
		using (SingleGlobalInstanceHelper singleGlobalInstanceHelper = SingleGlobalInstanceHelper.CollectiveMindsDevices(waitTime))
		{
			if (!singleGlobalInstanceHelper.IsAllowedToRun)
			{
				MessageBox.Show(UI.NotAllowedToRunMessage, UI.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Hand);
				i = 0;
			}
			else
			{
				App app = new App();
				app.InitializeComponent();
				app.Startup += AppStartupHandler;
				app.Exit += AppExitHandler;
				i = app.Run();
			}
		}
		return i;
	}

	private static void AppExitHandler(object sender, ExitEventArgs e)
	{
		TelemetryHelper.EndTrackingSessionDuration();
		TelemetryHelper.AppExit();
	}

	private static void AppStartupHandler(object sender, StartupEventArgs e)
	{
		TelemetryHelper.Initialize("9c58dee7-42f9-48ef-9afd-e0afbc16ccba", VersionString);
		UnhandledExceptionTelemetryCollector.RegisterHandlers();
		TelemetryHelper.BeginTrackingSessionDuration();
		try
		{
			ApplicationUpdateManager.FinalizeUpdateAfterRelaunch();
		}
		catch (Exception exception)
		{
			TelemetryHelper.TrackException(exception);
		}
	}

	internal static async Task<ApplicationUpdateManager> GetUpdateManager()
	{
		GitVersionCurrentApplicationVersionSource versrc = new GitVersionCurrentApplicationVersionSource();
		try
		{
			return new ApplicationUpdateManager(await RetryHelper.RetryWithExponentialBackoff(() => NetworkHelper.FetchManifest<ApplicationUpdateManifestV2>("https://cmupdate.azureedge.net/appupdate/StrikePackCfg/update.xml"), 3, 500), versrc);
		}
		catch (Exception exception)
		{
			TelemetryHelper.TrackException(exception);
		}
		return new ApplicationUpdateManager(new ApplicationUpdateManifest
		{
			LatestAvailableApplicationVersion = versrc.GetCurrentApplicationVersion()
		}, versrc);
	}

	internal static async Task<byte[]> FetchUpdate(ApplicationUpdateManager updateManager)
	{
		try
		{
			byte[] array = await RetryHelper.RetryWithExponentialBackoff(() => NetworkHelper.FetchBytes(updateManager.Manifest.LatestAvailableApplicationUrl), 3, 500);
			updateManager.VerifyUpdateData(array);
			return array;
		}
		catch (Exception exception)
		{
			TelemetryHelper.TrackException(exception);
			return null;
		}
	}

	internal static async Task PerformUpdate(byte[] updateData)
	{
		if (updateData != null)
		{
			await ApplicationUpdateManager.PerformUpdate(updateData);
			Application.Current.Dispatcher.InvokeShutdown();
		}
	}
}
