using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel.Implementation;

namespace CollectiveMinds.Common.Azure;

public class WpfApplicationLifeCycle : IApplicationLifecycle
{
	private readonly List<Task> blockingTasks = new List<Task>();

	public event Action<object, object> Started
	{
		add
		{
		}
		remove
		{
		}
	}

	public event EventHandler<ApplicationStoppingEventArgs> Stopping
	{
		add
		{
			if (value.Target.GetType().Name != "ApplicationLifecycleTransmissionPolicy")
			{
				StoppingInternal += value;
			}
		}
		remove
		{
			StoppingInternal -= value;
		}
	}

	private event EventHandler<ApplicationStoppingEventArgs> StoppingInternal;

	private void OnStopping(ApplicationStoppingEventArgs eventArgs)
	{
		this.StoppingInternal?.Invoke(this, eventArgs);
	}

	public void AppExit()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		OnStopping(new ApplicationStoppingEventArgs((Func<Func<Task>, Task>)RunOnCurrentThread));
		Task.WaitAll(blockingTasks.ToArray());
	}

	private Task RunOnCurrentThread(Func<Task> asyncMethod)
	{
		Task task = asyncMethod();
		blockingTasks.Add(task);
		return task;
	}
}
