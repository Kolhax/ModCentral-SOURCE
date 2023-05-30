using System;
using System.Threading;
using DfuLib.WinUsbNet;

namespace DfuLib;

public abstract class DfuDeviceBackendBase : IDfuDeviceBackend, IDisposable
{
	public DfuStatus Execute(Action action, DfuState expectedState, string message)
	{
		action();
		if (expectedState == DfuState.Error)
		{
			return null;
		}
		DfuStatus status;
		do
		{
			try
			{
				status = GetStatus();
			}
			catch (UsbException inner)
			{
				throw new DfuException(message, inner);
			}
			Thread.Sleep(status.PollTimeout);
		}
		while (status.State == DfuState.DownloadBusy || status.State == DfuState.UploadBusy || status.State == DfuState.Manifest);
		DfuException.ThrowOnStatus(status, expectedState, message);
		return status;
	}

	public DfuStatus ClearErrorAndAbort(DfuState expected = DfuState.Idle)
	{
		DfuStatus dfuStatus = GetStatus();
		if (dfuStatus.State == DfuState.Error)
		{
			dfuStatus = Execute(delegate
			{
				ClearStatus();
			}, expected, "ClearStatus failed");
		}
		if (dfuStatus.State != expected)
		{
			dfuStatus = Execute(delegate
			{
				Abort();
			}, expected, "Abort failed");
		}
		return dfuStatus;
	}

	public DfuStatus Manifest(DfuState expected = DfuState.Idle)
	{
		return Execute(delegate
		{
			Download(0, new byte[0]);
		}, expected, "Manifest failed");
	}

	public abstract void Detach();

	public abstract void Download(ushort block, byte[] data);

	public abstract byte[] Upload(ushort block, uint length);

	public abstract DfuStatus GetStatus();

	public abstract void ClearStatus();

	public abstract DfuState GetState();

	public abstract void Abort();

	public abstract void Close();

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			Close();
		}
	}
}
