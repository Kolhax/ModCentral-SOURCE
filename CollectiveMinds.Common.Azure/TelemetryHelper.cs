using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;

namespace CollectiveMinds.Common.Azure;

public static class TelemetryHelper
{
	private static readonly Stopwatch SessionStopwatch = new Stopwatch();

	private static readonly WpfApplicationLifeCycle ApplicationLifeCycle = new WpfApplicationLifeCycle();

	public static TelemetryConfiguration Configuration { get; private set; }

	public static ServerTelemetryChannel TelemetryChannel { get; private set; }

	private static TelemetryClient Client { get; set; }

	public static void Initialize(string instrumentationKey, string versionOverride = null)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		Configuration = TelemetryConfiguration.CreateDefault();
		Configuration.InstrumentationKey = instrumentationKey;
		object obj = Extensions.CreateInternalInstance<ServerTelemetryChannel>("Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel.Implementation.Network", new object[0]);
		TelemetryChannel = Extensions.CreateInstance<ServerTelemetryChannel>(new object[2] { obj, ApplicationLifeCycle });
		Configuration.TelemetryChannel = (ITelemetryChannel)(object)TelemetryChannel;
		TelemetryChannel.Initialize(Configuration);
		Client = new TelemetryClient(Configuration);
		BuildTelemetryClientContext(Client.Context, versionOverride);
	}

	public static void AppExit()
	{
		ApplicationLifeCycle.AppExit();
		Thread.Sleep(1000);
	}

	private static void EnsureInitialized([CallerMemberName] string memberName = "")
	{
		if (Client != null)
		{
			return;
		}
		throw new InvalidOperationException(string.Format("{0}.{1}() must be invoked before {2}", "TelemetryHelper", "Initialize", memberName));
	}

	public static void BeginTrackingSessionDuration()
	{
		EnsureInitialized("BeginTrackingSessionDuration");
		SessionStopwatch.Start();
		Client.TrackEvent("SessionBegin", (IDictionary<string, string>)null, (IDictionary<string, double>)null);
	}

	public static void EndTrackingSessionDuration()
	{
		EnsureInitialized("EndTrackingSessionDuration");
		SessionStopwatch.Stop();
		Client.TrackEvent("SessionEnd", (IDictionary<string, string>)null, (IDictionary<string, double>)new Dictionary<string, double> { 
		{
			"SessionDuration",
			SessionStopwatch.Elapsed.TotalMilliseconds
		} });
	}

	private static void BuildTelemetryClientContext(TelemetryContext ctx, string versionOverride)
	{
		AssemblyName name = Assembly.GetEntryAssembly().GetName();
		ctx.User.Id = $"{Environment.MachineName}\\{Environment.UserName}";
		ctx.Session.Id = Guid.NewGuid().ToString();
		ctx.User.UserAgent = $"{name.Name} {name.Version}";
		ctx.Component.Version = versionOverride ?? name.Version.ToString();
		ctx.Device.Language = CultureInfo.InstalledUICulture.EnglishName;
		ctx.Device.OperatingSystem = Environment.OSVersion.ToString();
		ctx.Device.ScreenResolution = $"{SystemParameters.PrimaryScreenWidth}x{SystemParameters.PrimaryScreenHeight}";
	}

	public static void Flush()
	{
		EnsureInitialized("Flush");
		Client.Flush();
	}

	public static void TrackException(ExceptionTelemetry telemetry, bool throwOnDebugger = true)
	{
		EnsureInitialized("TrackException");
		Client.TrackException(telemetry);
		Client.Flush();
		if (throwOnDebugger && Debugger.IsAttached)
		{
			throw telemetry.Exception;
		}
	}

	public static void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null, bool throwOnDebugger = true)
	{
		EnsureInitialized("TrackException");
		if (properties == null)
		{
			properties = new Dictionary<string, string>();
		}
		properties["_exception.FullTrace"] = exception.FullTrace();
		PropertyInfo[] properties2 = exception.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties2)
		{
			properties["_exception." + propertyInfo.Name] = string.Format("{0}", propertyInfo.GetValue(exception) ?? "(NULL)");
		}
		Client.TrackException(exception, properties, metrics);
		Client.Flush();
		if (throwOnDebugger && Debugger.IsAttached)
		{
			throw exception;
		}
	}

	public static void TrackDependency(string dependencyName, string commandName, DateTimeOffset startTime, TimeSpan duration, bool success)
	{
		EnsureInitialized("TrackDependency");
		Client.TrackDependency(dependencyName, commandName, startTime, duration, success);
	}
}
