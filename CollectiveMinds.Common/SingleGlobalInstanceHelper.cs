using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace CollectiveMinds.Common;

public class SingleGlobalInstanceHelper : IDisposable
{
	private static readonly SecurityIdentifier WorldSecurityIdentifier = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

	private static readonly MutexAccessRule WorldFullControlMutexAccessRule = new MutexAccessRule(WorldSecurityIdentifier, MutexRights.FullControl, AccessControlType.Allow);

	private Mutex mutex;

	public bool IsAllowedToRun { get; private set; }

	public SingleGlobalInstanceHelper(string instanceName)
		: this(instanceName, 0)
	{
	}

	public SingleGlobalInstanceHelper(string instanceName, TimeSpan timeout)
		: this(instanceName, (int)timeout.TotalMilliseconds)
	{
	}

	public SingleGlobalInstanceHelper(string instanceName, int millisecondsTimeout)
	{
		if (instanceName == null)
		{
			throw new ArgumentNullException("instanceName");
		}
		if (string.IsNullOrWhiteSpace(instanceName))
		{
			throw new ArgumentException(string.Format("{0} must have a non-empty value", "instanceName"), "instanceName");
		}
		MutexSecurity mutexSecurity = new MutexSecurity();
		mutexSecurity.AddAccessRule(WorldFullControlMutexAccessRule);
		mutex = new Mutex(initiallyOwned: false, $"Global\\SingleGlobalInstance_{instanceName}", out var _, mutexSecurity);
		try
		{
			IsAllowedToRun = mutex.WaitOne(millisecondsTimeout, exitContext: false);
		}
		catch (AbandonedMutexException)
		{
			IsAllowedToRun = true;
		}
	}

	public void Dispose()
	{
		if (mutex == null)
		{
			return;
		}
		try
		{
			if (IsAllowedToRun)
			{
				mutex.ReleaseMutex();
			}
		}
		finally
		{
			mutex.Dispose();
			mutex = null;
			IsAllowedToRun = false;
		}
	}

	public static SingleGlobalInstanceHelper CollectiveMindsDevices(TimeSpan waitTime)
	{
		return new SingleGlobalInstanceHelper("CollectiveMindsDevices", waitTime);
	}
}
