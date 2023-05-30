using System;
using System.Threading;

namespace DfuLib.WinUsbNet;

internal class UsbAsyncResult : IAsyncResult, IDisposable
{
	private object _stateObject;

	private AsyncCallback _userCallback;

	private bool _completed;

	private bool _completedSynchronously;

	private ManualResetEvent _waitEvent;

	private int _bytesTransfered;

	private Exception _error;

	public object AsyncState => _stateObject;

	public Exception Error
	{
		get
		{
			lock (this)
			{
				return _error;
			}
		}
	}

	public int BytesTransfered => _bytesTransfered;

	public WaitHandle AsyncWaitHandle
	{
		get
		{
			lock (this)
			{
				if (_waitEvent == null)
				{
					_waitEvent = new ManualResetEvent(_completed);
				}
			}
			return _waitEvent;
		}
	}

	public bool CompletedSynchronously
	{
		get
		{
			lock (this)
			{
				return _completedSynchronously;
			}
		}
	}

	public bool IsCompleted
	{
		get
		{
			lock (this)
			{
				return _completed;
			}
		}
	}

	public UsbAsyncResult(AsyncCallback userCallback, object stateObject)
	{
		_stateObject = stateObject;
		_userCallback = userCallback;
		_completedSynchronously = false;
		_completed = false;
		_waitEvent = null;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void OnCompletion(bool completedSynchronously, Exception error, int bytesTransfered, bool synchronousCallback)
	{
		lock (this)
		{
			_completedSynchronously = completedSynchronously;
			_completed = true;
			_error = error;
			_bytesTransfered = bytesTransfered;
			if (_waitEvent != null)
			{
				_waitEvent.Set();
			}
		}
		if (_userCallback != null)
		{
			if (synchronousCallback)
			{
				RunCallback(null);
			}
			else
			{
				ThreadPool.QueueUserWorkItem(RunCallback);
			}
		}
	}

	private void RunCallback(object state)
	{
		_userCallback(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}
		lock (this)
		{
			if (_waitEvent != null)
			{
				_waitEvent.Close();
			}
		}
	}
}
