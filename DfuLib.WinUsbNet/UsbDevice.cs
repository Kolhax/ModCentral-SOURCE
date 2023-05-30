using System;
using System.Collections.Generic;
using System.Threading;
using DfuLib.WinUsbNet.Api;

namespace DfuLib.WinUsbNet;

public class UsbDevice : IDisposable
{
	private WinUsbDevice _wuDevice;

	private bool _disposed;

	public UsbPipeCollection Pipes { get; private set; }

	public UsbInterfaceCollection Interfaces { get; private set; }

	public UsbDeviceDescriptor Descriptor { get; private set; }

	internal WinUsbDevice InternalDevice => _wuDevice;

	public int ControlPipeTimeout
	{
		get
		{
			return (int)_wuDevice.GetPipePolicyUInt(0, 0, POLICY_TYPE.PIPE_TRANSFER_TIMEOUT);
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("Control pipe timeout cannot be negative.");
			}
			_wuDevice.SetPipePolicy(0, 0, POLICY_TYPE.PIPE_TRANSFER_TIMEOUT, (uint)value);
		}
	}

	public UsbDevice(UsbDeviceInfo deviceInfo)
		: this(deviceInfo.DevicePath)
	{
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~UsbDevice()
	{
		Dispose(disposing: false);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing && _wuDevice != null)
			{
				_wuDevice.Dispose();
			}
			_disposed = true;
		}
	}

	public UsbDevice(string devicePathName)
	{
		Descriptor = GetDeviceDescriptor(devicePathName);
		_wuDevice = new WinUsbDevice();
		try
		{
			_wuDevice.OpenDevice(devicePathName);
			InitializeInterfaces();
		}
		catch (ApiException innerException)
		{
			_wuDevice.Dispose();
			throw new UsbException("Failed to open device.", innerException);
		}
	}

	private void InitializeInterfaces()
	{
		int interfaceCount = _wuDevice.InterfaceCount;
		List<UsbPipe> list = new List<UsbPipe>();
		UsbInterface[] array = new UsbInterface[interfaceCount];
		for (int i = 0; i < interfaceCount; i++)
		{
			_wuDevice.GetInterfaceInfo(i, out var descriptor, out var pipes);
			UsbPipe[] array2 = new UsbPipe[pipes.Length];
			for (int j = 0; j < pipes.Length; j++)
			{
				list.Add(array2[j] = new UsbPipe(this, pipes[j]));
			}
			UsbPipeCollection pipes2 = new UsbPipeCollection(array2);
			array[i] = new UsbInterface(this, i, descriptor, pipes2);
		}
		Pipes = new UsbPipeCollection(list.ToArray());
		Interfaces = new UsbInterfaceCollection(array);
	}

	private void CheckControlParams(int value, int index, byte[] buffer, int length)
	{
		if (value < 0 || value > 65535)
		{
			throw new ArgumentOutOfRangeException("Value parameter out of range.");
		}
		if (index < 0 || index > 65535)
		{
			throw new ArgumentOutOfRangeException("Index parameter out of range.");
		}
		if (length > buffer.Length)
		{
			throw new ArgumentOutOfRangeException("Length parameter is larger than the size of the buffer.");
		}
		if (length > 65535)
		{
			throw new ArgumentOutOfRangeException("Length too large");
		}
	}

	public void ControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer, int length)
	{
		CheckNotDisposed();
		CheckControlParams(value, index, buffer, length);
		try
		{
			_wuDevice.ControlTransfer(requestType, request, (ushort)value, (ushort)index, (ushort)length, buffer);
		}
		catch (ApiException innerException)
		{
			throw new UsbException("Control transfer failed", innerException);
		}
	}

	public IAsyncResult BeginControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer, int length, AsyncCallback userCallback, object stateObject)
	{
		CheckNotDisposed();
		CheckControlParams(value, index, buffer, length);
		UsbAsyncResult usbAsyncResult = new UsbAsyncResult(userCallback, stateObject);
		try
		{
			_wuDevice.ControlTransferOverlapped(requestType, request, (ushort)value, (ushort)index, (ushort)length, buffer, usbAsyncResult);
			return usbAsyncResult;
		}
		catch (ApiException innerException)
		{
			usbAsyncResult?.Dispose();
			throw new UsbException("Asynchronous control transfer failed", innerException);
		}
		catch (Exception)
		{
			usbAsyncResult?.Dispose();
			throw;
		}
	}

	public IAsyncResult BeginControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer, AsyncCallback userCallback, object stateObject)
	{
		return BeginControlTransfer(requestType, request, value, index, buffer, buffer.Length, userCallback, stateObject);
	}

	public int EndControlTransfer(IAsyncResult asyncResult)
	{
		if (asyncResult == null)
		{
			throw new NullReferenceException("asyncResult cannot be null");
		}
		if (!(asyncResult is UsbAsyncResult))
		{
			throw new ArgumentException("AsyncResult object was not created by calling one of the BeginControl* methods on this class.");
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
				throw new UsbException("Asynchronous control transfer from pipe has failed.", usbAsyncResult.Error);
			}
			return usbAsyncResult.BytesTransfered;
		}
		finally
		{
			usbAsyncResult.Dispose();
		}
	}

	public void ControlTransfer(byte requestType, byte request, int value, int index, byte[] buffer)
	{
		ControlTransfer(requestType, request, value, index, buffer, buffer.Length);
	}

	public void ControlTransfer(byte requestType, byte request, int value, int index)
	{
		ControlTransfer(requestType, request, value, index, new byte[0], 0);
	}

	private void CheckIn(byte requestType)
	{
		if ((requestType & 0x80) == 0)
		{
			throw new ArgumentException("Request type is not IN.");
		}
	}

	private void CheckOut(byte requestType)
	{
		if ((requestType & 0x80) == 128)
		{
			throw new ArgumentException("Request type is not OUT.");
		}
	}

	public byte[] ControlIn(byte requestType, byte request, int value, int index, int length)
	{
		CheckIn(requestType);
		byte[] array = new byte[length];
		ControlTransfer(requestType, request, value, index, array, array.Length);
		return array;
	}

	public void ControlIn(byte requestType, byte request, int value, int index, byte[] buffer, int length)
	{
		CheckIn(requestType);
		ControlTransfer(requestType, request, value, index, buffer, length);
	}

	public void ControlIn(byte requestType, byte request, int value, int index, byte[] buffer)
	{
		CheckIn(requestType);
		ControlTransfer(requestType, request, value, index, buffer);
	}

	public void ControlIn(byte requestType, byte request, int value, int index)
	{
		CheckIn(requestType);
		ControlTransfer(requestType, request, value, index, new byte[0]);
	}

	public void ControlOut(byte requestType, byte request, int value, int index, byte[] buffer, int length)
	{
		CheckOut(requestType);
		ControlTransfer(requestType, request, value, index, buffer, length);
	}

	public void ControlOut(byte requestType, byte request, int value, int index, byte[] buffer)
	{
		CheckOut(requestType);
		ControlTransfer(requestType, request, value, index, buffer);
	}

	public void ControlOut(byte requestType, byte request, int value, int index)
	{
		CheckOut(requestType);
		ControlTransfer(requestType, request, value, index, new byte[0]);
	}

	public IAsyncResult BeginControlTransfer(byte requestType, byte request, int value, int index, AsyncCallback userCallback, object stateObject)
	{
		return BeginControlTransfer(requestType, request, value, index, new byte[0], 0, userCallback, stateObject);
	}

	public IAsyncResult BeginControlIn(byte requestType, byte request, int value, int index, byte[] buffer, int length, AsyncCallback userCallback, object stateObject)
	{
		CheckIn(requestType);
		return BeginControlTransfer(requestType, request, value, index, buffer, length, userCallback, stateObject);
	}

	public IAsyncResult BeginControlIn(byte requestType, byte request, int value, int index, byte[] buffer, AsyncCallback userCallback, object stateObject)
	{
		CheckIn(requestType);
		return BeginControlTransfer(requestType, request, value, index, buffer, userCallback, stateObject);
	}

	public IAsyncResult BeginControlIn(byte requestType, byte request, int value, int index, AsyncCallback userCallback, object stateObject)
	{
		CheckIn(requestType);
		return BeginControlTransfer(requestType, request, value, index, userCallback, stateObject);
	}

	public IAsyncResult BeginControlOut(byte requestType, byte request, int value, int index, byte[] buffer, int length, AsyncCallback userCallback, object stateObject)
	{
		CheckOut(requestType);
		return BeginControlTransfer(requestType, request, value, index, buffer, length, userCallback, stateObject);
	}

	public IAsyncResult BeginControlOut(byte requestType, byte request, int value, int index, byte[] buffer, AsyncCallback userCallback, object stateObject)
	{
		CheckOut(requestType);
		return BeginControlTransfer(requestType, request, value, index, buffer, userCallback, stateObject);
	}

	public IAsyncResult BeginControlOut(byte requestType, byte request, int value, int index, AsyncCallback userCallback, object stateObject)
	{
		CheckOut(requestType);
		return BeginControlTransfer(requestType, request, value, index, new byte[0], userCallback, stateObject);
	}

	private void CheckNotDisposed()
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("USB device object has been disposed.");
		}
	}

	public static UsbDeviceInfo[] GetDevices(string guidString)
	{
		return GetDevices(new Guid(guidString));
	}

	public static UsbDeviceInfo[] GetDevices(Guid guid)
	{
		DeviceDetails[] array = DeviceManagement.FindDevicesFromGuid(guid);
		UsbDeviceInfo[] array2 = new UsbDeviceInfo[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = new UsbDeviceInfo(array[i]);
		}
		return array2;
	}

	public static UsbDevice GetSingleDevice(Guid guid)
	{
		DeviceDetails[] array = DeviceManagement.FindDevicesFromGuid(guid);
		if (array.Length == 0)
		{
			return null;
		}
		return new UsbDevice(array[0].DevicePath);
	}

	public static UsbDevice GetSingleDevice(string guidString)
	{
		return GetSingleDevice(new Guid(guidString));
	}

	private static UsbDeviceDescriptor GetDeviceDescriptor(string devicePath)
	{
		try
		{
			using WinUsbDevice winUsbDevice = new WinUsbDevice();
			winUsbDevice.OpenDevice(devicePath);
			Thread.Sleep(500);
			USB_DEVICE_DESCRIPTOR deviceDescriptor = winUsbDevice.GetDeviceDescriptor();
			string manufacturer = null;
			string product = null;
			string serialNumber = null;
			byte b = 0;
			b = deviceDescriptor.iManufacturer;
			if (b > 0)
			{
				manufacturer = winUsbDevice.GetStringDescriptor(b);
			}
			b = deviceDescriptor.iProduct;
			if (b > 0)
			{
				product = winUsbDevice.GetStringDescriptor(b);
			}
			b = deviceDescriptor.iSerialNumber;
			if (b > 0)
			{
				serialNumber = winUsbDevice.GetStringDescriptor(b);
			}
			return new UsbDeviceDescriptor(devicePath, deviceDescriptor, manufacturer, product, serialNumber);
		}
		catch (ApiException innerException)
		{
			throw new UsbException("Failed to retrieve device descriptor.", innerException);
		}
	}
}
