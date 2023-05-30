using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.DataContracts;

namespace CollectiveMinds.Common.Azure;

public class AlwaysSendTelemetryScope : List<ExceptionTelemetry>, IDisposable
{
	public bool Success { get; set; } = true;


	public void Dispose()
	{
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			ExceptionTelemetry current = enumerator.Current;
			current.Properties.Add("WasTransient", Success.ToString());
			TelemetryHelper.TrackException(current, throwOnDebugger: false);
		}
	}
}
