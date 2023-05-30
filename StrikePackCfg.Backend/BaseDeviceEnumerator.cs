using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using DfuLib.WinUsbNet;
using StrikePackCfg.HelperClasses;

namespace StrikePackCfg.Backend;

public abstract class BaseDeviceEnumerator<TDevice, TState> : ICollection<TDevice>, IEnumerable<TDevice>, IEnumerable, IDisposable where TDevice : IDeviceInterfaceGuid
{
	private readonly Dictionary<string, TState> _devicePathMap = new Dictionary<string, TState>();

	private readonly ICollection<TState> _devices;

	private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

	private readonly Dictionary<TDevice, UsbNotifier> _notifiers = new Dictionary<TDevice, UsbNotifier>();

	private readonly Control _window;

	public int Count => _lock.DoWithReadLock(() => _notifiers.Count);

	public bool IsReadOnly => false;

	protected BaseDeviceEnumerator(Control window, ICollection<TState> devices)
	{
		_window = window;
		_devices = devices;
	}

	public void Dispose()
	{
		Clear();
	}

	private async void AddInternal(TDevice item)
	{
		UsbNotifier usbNotifier = new UsbNotifier(_window, item.DeviceInterfaceGuid);
		_notifiers.Add(item, usbNotifier);
		usbNotifier.Arrival += NotifierArrival;
		usbNotifier.Removal += NotifierRemoval;
		UsbDeviceInfo[] devices = UsbDevice.GetDevices(item.DeviceInterfaceGuid);
		foreach (UsbDeviceInfo usbDeviceInfo in devices)
		{
			await EnumerateDevice(item, usbDeviceInfo.DevicePath).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private void ClearInternal()
	{
		foreach (UsbNotifier value in _notifiers.Values)
		{
			value.Arrival -= NotifierArrival;
			value.Removal -= NotifierRemoval;
			value.Dispose();
		}
		_notifiers.Clear();
	}

	private bool RemoveInternal(TDevice item)
	{
		if (_notifiers.TryGetValue(item, out var value))
		{
			value.Arrival -= NotifierArrival;
			value.Removal -= NotifierRemoval;
			value.Dispose();
		}
		return _notifiers.Remove(item);
	}

	private async void NotifierArrival(object sender, USBEvent e)
	{
		TDevice val = _notifiers.Keys.FirstOrDefault((TDevice p) => p.DeviceInterfaceGuid == e.Guid);
		if (val != null)
		{
			await EnumerateDevice(val, e.DevicePath).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private async void NotifierRemoval(object sender, USBEvent e)
	{
		string devicePath = e.DevicePath.ToLowerInvariant();
		await _window.Dispatcher.InvokeAsync(delegate
		{
			if (_devicePathMap.TryGetValue(devicePath, out var value))
			{
				_devices.Remove(value);
				_devicePathMap.Remove(devicePath);
			}
		});
	}

	private async Task EnumerateDevice(TDevice item, string devicePath)
	{
		devicePath = devicePath.ToLowerInvariant();
		if (!_devicePathMap.ContainsKey(devicePath))
		{
			TState updateState = default(TState);
			_ = updateState;
			updateState = await Task.Factory.StartNew(() => HandleDeviceEnumeration(item, devicePath)).ConfigureAwait(continueOnCapturedContext: false);
			await _window.Dispatcher.InvokeAsync(delegate
			{
				_devicePathMap.Add(devicePath, updateState);
				_devices.Add(updateState);
			});
		}
	}

	protected abstract TState HandleDeviceEnumeration(TDevice item, string devicePath);

	public bool Contains(TDevice item)
	{
		return _lock.DoWithReadLock(() => _notifiers.ContainsKey(item));
	}

	public void CopyTo(TDevice[] array, int index)
	{
		_lock.DoWithReadLock(delegate
		{
			_notifiers.Keys.CopyTo(array, index);
		});
	}

	public IEnumerator<TDevice> GetEnumerator()
	{
		return _lock.DoWithReadLock(() => _notifiers.Keys.GetEnumerator());
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _lock.DoWithReadLock(() => _notifiers.Keys.GetEnumerator());
	}

	public void Add(TDevice item)
	{
		_lock.DoWithWriteLock(delegate
		{
			AddInternal(item);
		});
	}

	public void Clear()
	{
		_lock.DoWithWriteLock(ClearInternal);
	}

	public bool Remove(TDevice item)
	{
		return _lock.DoWithWriteLock(() => RemoveInternal(item));
	}
}
