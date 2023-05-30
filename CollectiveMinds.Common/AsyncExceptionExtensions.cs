using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CollectiveMinds.Common;

public static class AsyncExceptionExtensions
{
	public static async Task AsyncTrace(this Task task, [CallerMemberName] string callerMemberName = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
	{
		try
		{
			await task;
		}
		catch (Exception ex)
		{
			AddAsyncStackTrace(ex, callerMemberName, callerFilePath, callerLineNumber);
			throw;
		}
	}

	public static async Task<T> AsyncTrace<T>(this Task<T> task, [CallerMemberName] string callerMemberName = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
	{
		try
		{
			return await task;
		}
		catch (Exception ex)
		{
			AddAsyncStackTrace(ex, callerMemberName, callerFilePath, callerLineNumber);
			throw;
		}
	}

	private static void AddAsyncStackTrace(Exception ex, string callerMemberName, string callerFilePath, int callerLineNumber = 0)
	{
		IList<string> list = (ex.Data["_AsyncStackTrace"] as IList<string>) ?? new List<string>();
		list.Add($"@{callerMemberName}, in '{callerFilePath}', line {callerLineNumber}");
		ex.Data["_AsyncStackTrace"] = list;
	}

	public static string FullTrace(this Exception ex)
	{
		if (!(ex.Data["_AsyncStackTrace"] is IList<string> values))
		{
			return string.Empty;
		}
		return $"{ex}\n--- Async stack trace:\n\t" + string.Join("\n\t", values);
	}
}
