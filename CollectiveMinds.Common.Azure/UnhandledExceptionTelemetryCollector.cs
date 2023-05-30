using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CollectiveMinds.Common.Azure;

public static class UnhandledExceptionTelemetryCollector
{
	private static readonly HashSet<Exception> ProcessedExceptions = new HashSet<Exception>();

	public static event UnhandledExceptionEventHandler UnhandledException;

	public static void RegisterHandlers()
	{
		AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
		Application.Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
		TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
	}

	private static void OnUnhandledException(object sender, Exception exception, bool isTerminating)
	{
		UnhandledExceptionTelemetryCollector.UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgs(exception, isTerminating));
	}

	private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		Exception ex = (Exception)e.ExceptionObject;
		if (!ProcessedExceptions.Contains(ex))
		{
			ProcessedExceptions.Add(ex);
			TelemetryHelper.TrackException(ex, null, null, throwOnDebugger: false);
			OnUnhandledException(sender, ex, e.IsTerminating);
		}
	}

	private static void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		if (!ProcessedExceptions.Contains(e.Exception))
		{
			ProcessedExceptions.Add(e.Exception);
			TelemetryHelper.TrackException(e.Exception, null, null, throwOnDebugger: false);
			e.Handled = true;
			OnUnhandledException(sender, e.Exception, isTerminating: false);
		}
	}

	private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
	{
		if (!ProcessedExceptions.Contains(e.Exception))
		{
			ProcessedExceptions.Add(e.Exception);
			TelemetryHelper.TrackException(e.Exception, null, null, throwOnDebugger: false);
			e.SetObserved();
			OnUnhandledException(sender, e.Exception, isTerminating: false);
		}
	}
}
