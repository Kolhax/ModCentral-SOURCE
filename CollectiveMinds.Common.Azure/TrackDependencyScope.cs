using System;
using System.Diagnostics;

namespace CollectiveMinds.Common.Azure;

public class TrackDependencyScope : IDisposable
{
	private readonly string commandName;

	private readonly string dependencyName;

	private readonly DateTimeOffset startTime = DateTimeOffset.UtcNow;

	private readonly Stopwatch timer = Stopwatch.StartNew();

	private bool success;

	public TrackDependencyScope(string dependencyName, string commandName)
	{
		this.dependencyName = dependencyName;
		this.commandName = commandName;
	}

	public void Dispose()
	{
		TelemetryHelper.TrackDependency(dependencyName, commandName, startTime, timer.Elapsed, success);
	}

	public static TrackDependencyScope FromUri(Uri uri)
	{
		return new TrackDependencyScope(uri.GetLeftPart(UriPartial.Authority), uri.AbsolutePath);
	}

	public void Success()
	{
		success = true;
	}

	public T Success<T>(T ret)
	{
		success = true;
		return ret;
	}
}
