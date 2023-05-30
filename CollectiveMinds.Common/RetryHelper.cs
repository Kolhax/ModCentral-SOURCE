using System;
using System.Globalization;
using System.Threading.Tasks;
using CollectiveMinds.Common.Azure;
using Microsoft.ApplicationInsights.DataContracts;

namespace CollectiveMinds.Common;

public static class RetryHelper
{
	public static Task RetryWithExponentialBackoff(Action action, int retryCount, int baseRetryDelayInMs)
	{
		return RetryWithExponentialBackoff(action, retryCount, TimeSpan.FromMilliseconds(baseRetryDelayInMs));
	}

	public static Task<T> RetryWithExponentialBackoff<T>(Func<Task<T>> action, int retryCount, int baseRetryDelayInMs)
	{
		return RetryWithExponentialBackoff(action, retryCount, TimeSpan.FromMilliseconds(baseRetryDelayInMs));
	}

	public static async Task RetryWithExponentialBackoff(Action action, int retryCount, TimeSpan baseRetryDelay)
	{
		await RetryWithExponentialBackoff(async delegate
		{
			await Task.Factory.StartNew(action);
			return await Task.FromResult(0);
		}, retryCount, baseRetryDelay);
	}

	public static async Task<T> RetryWithExponentialBackoff<T>(Func<Task<T>> action, int retryCount, TimeSpan baseRetryDelay)
	{
		int currentTry = 0;
		using AlwaysSendTelemetryScope exceptionTelemetry = new AlwaysSendTelemetryScope();
		while (true)
		{
			currentTry++;
			try
			{
				return await action().ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				ExceptionTelemetry val = new ExceptionTelemetry(ex);
				val.Timestamp = DateTimeOffset.UtcNow;
				val.Properties.Add("CurrentTry", currentTry.ToString(CultureInfo.InvariantCulture));
				val.Properties.Add("MaximumTry", retryCount.ToString(CultureInfo.InvariantCulture));
				val.Properties.Add("RetryDelay", baseRetryDelay.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
				exceptionTelemetry.Add(val);
				if (currentTry >= retryCount)
				{
					exceptionTelemetry.Success = false;
					throw;
				}
				await Task.Delay(baseRetryDelay).ConfigureAwait(continueOnCapturedContext: false);
				baseRetryDelay = TimeSpan.FromMilliseconds(baseRetryDelay.TotalMilliseconds * 2.0);
			}
		}
	}
}
