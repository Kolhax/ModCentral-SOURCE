using System;
using DfuLib.WinUsbNet.Api;

namespace DfuLib.WinUsbNet;

public class UsbPipe
{
	private WINUSB_PIPE_INFORMATION _pipeInfo;

	private UsbInterface _interface;

	private UsbDevice _device;

	private UsbPipePolicy _policy;

	public byte Address => _pipeInfo.PipeId;

	public UsbDevice Device => _device;

	public int MaximumPacketSize => _pipeInfo.MaximumPacketSize;

	public UsbInterface Interface => _interface;

	public UsbPipePolicy Policy => _policy;

	public bool IsOut => (_pipeInfo.PipeId & 0x80) == 0;

	public bool IsIn => (_pipeInfo.PipeId & 0x80) != 0;

	public int Read(byte[] buffer)
	{
		return Read(buffer, 0, buffer.Length);
	}

	public int Read(byte[] buffer, int offset, int length)
	{
		CheckReadParams(buffer, offset, length);
		try
		{
			_device.InternalDevice.ReadPipe(Interface.InterfaceIndex, _pipeInfo.PipeId, buffer, offset, length, out var bytesRead);
			return (int)bytesRead;
		}
		catch (ApiException innerException)
		{
			throw new UsbException("Failed to read from pipe.", innerException);
		}
	}

	private void CheckReadParams(byte[] buffer, int offset, int length)
	{
		if (!IsIn)
		{
			throw new NotSupportedException("Cannot read from a pipe with OUT direction.");
		}
		int num = buffer.Length;
		if (offset < 0 || offset >= num)
		{
			throw new ArgumentOutOfRangeException("Offset of data to read is outside the buffer boundaries.");
		}
		if (length < 0 || offset + length > num)
		{
			throw new ArgumentOutOfRangeException("Length of data to read is outside the buffer boundaries.");
		}
	}

	private void CheckWriteParams(byte[] buffer, int offset, int length)
	{
		if (!IsOut)
		{
			throw new NotSupportedException("Cannot write to a pipe with IN direction.");
		}
		int num = buffer.Length;
		if (offset < 0 || offset >= num)
		{
			throw new ArgumentOutOfRangeException("Offset of data to write is outside the buffer boundaries.");
		}
		if (length < 0 || offset + length > num)
		{
			throw new ArgumentOutOfRangeException("Length of data to write is outside the buffer boundaries.");
		}
	}

	public IAsyncResult BeginRead(byte[] buffer, int offset, int length, AsyncCallback userCallback, object stateObject)
	{
		CheckReadParams(buffer, offset, length);
		UsbAsyncResult usbAsyncResult = new UsbAsyncResult(userCallback, stateObject);
		try
		{
			_device.InternalDevice.ReadPipeOverlapped(Interface.InterfaceIndex, _pipeInfo.PipeId, buffer, offset, length, usbAsyncResult);
			return usbAsyncResult;
		}
		catch (ApiException innerException)
		{
			usbAsyncResult?.Dispose();
			throw new UsbException("Failed to read from pipe.", innerException);
		}
		catch (Exception)
		{
			usbAsyncResult?.Dispose();
			throw;
		}
	}

	public int EndRead(IAsyncResult asyncResult)
	{
		if (asyncResult == null)
		{
			throw new NullReferenceException("asyncResult cannot be null");
		}
		if (!(asyncResult is UsbAsyncResult))
		{
			throw new ArgumentException("AsyncResult object was not created by calling BeginRead on this class.");
		}
		UsbAsyncResult usbAsyncResult = (UsbAsyncResult)asyncResult;
		try
		{
			if (!usbAsyncResult.IsCompleted)
			{
				usbAsyncResult.AsyncWaitHandle.WaitOne();
			}
			if (usbAsyncResult.Error != null)
			{
				throw new UsbException("Asynchronous read from pipe has failed.", usbAsyncResult.Error);
			}
			return usbAsyncResult.BytesTransfered;
		}
		finally
		{
			usbAsyncResult.Dispose();
		}
	}

	public void Write(byte[] buffer)
	{
		Write(buffer, 0, buffer.Length);
	}

	public void Write(byte[] buffer, int offset, int length)
	{
		CheckWriteParams(buffer, offset, length);
		try
		{
			_device.InternalDevice.WritePipe(Interface.InterfaceIndex, _pipeInfo.PipeId, buffer, offset, length);
		}
		catch (ApiException innerException)
		{
			throw new UsbException("Failed to write to pipe.", innerException);
		}
	}

	public IAsyncResult BeginWrite(byte[] buffer, int offset, int length, AsyncCallback userCallback, object stateObject)
	{
		CheckWriteParams(buffer, offset, length);
		UsbAsyncResult usbAsyncResult = new UsbAsyncResult(userCallback, stateObject);
		try
		{
			_device.InternalDevice.WriteOverlapped(Interface.InterfaceIndex, _pipeInfo.PipeId, buffer, offset, length, usbAsyncResult);
			return usbAsyncResult;
		}
		catch (ApiException innerException)
		{
			usbAsyncResult?.Dispose();
			throw new UsbException("Failed to write to pipe.", innerException);
		}
		catch (Exception)
		{
			usbAsyncResult?.Dispose();
			throw;
		}
	}

	public void EndWrite(IAsyncResult asyncResult)
	{
		if (asyncResult == null)
		{
			throw new NullReferenceException("asyncResult cannot be null");
		}
		if (!(asyncResult is UsbAsyncResult))
		{
			throw new ArgumentException("AsyncResult object was not created by calling BeginWrite on this class.");
		}
		UsbAsyncResult usbAsyncResult = (UsbAsyncResult)asyncResult;
		try
		{
			if (!usbAsyncResult.IsCompleted)
			{
				usbAsyncResult.AsyncWaitHandle.WaitOne();
			}
			if (usbAsyncResult.Error != null)
			{
				throw new UsbException("Asynchronous write to pipe has failed.", usbAsyncResult.Error);
			}
		}
		finally
		{
			usbAsyncResult.Dispose();
		}
	}

	public void Abort()
	{
		try
		{
			_device.InternalDevice.AbortPipe(Interface.InterfaceIndex, _pipeInfo.PipeId);
		}
		catch (ApiException innerException)
		{
			throw new UsbException("Failed to abort pipe.", innerException);
		}
	}

	public void Flush()
	{
		if (!IsIn)
		{
			throw new NotSupportedException("Flush is only supported on IN direction pipes");
		}
		try
		{
			_device.InternalDevice.FlushPipe(Interface.InterfaceIndex, _pipeInfo.PipeId);
		}
		catch (ApiException innerException)
		{
			throw new UsbException("Failed to flush pipe.", innerException);
		}
	}

	internal UsbPipe(UsbDevice device, WINUSB_PIPE_INFORMATION pipeInfo)
	{
		_pipeInfo = pipeInfo;
		_device = device;
		_policy = null;
	}

	internal void AttachInterface(UsbInterface usbInterface)
	{
		_interface = usbInterface;
		_policy = new UsbPipePolicy(_device, _interface.InterfaceIndex, _pipeInfo.PipeId);
	}
}
