using System;
using System.Collections;
using System.Collections.Generic;

namespace DfuLib.WinUsbNet;

public class UsbInterfaceCollection : IEnumerable<UsbInterface>, IEnumerable
{
	private class USBInterfaceEnumerator : IEnumerator<UsbInterface>, IDisposable, IEnumerator
	{
		private int _index;

		private UsbInterface[] _interfaces;

		public UsbInterface Current => GetCurrent();

		object IEnumerator.Current => GetCurrent();

		public USBInterfaceEnumerator(UsbInterface[] interfaces)
		{
			_interfaces = interfaces;
			_index = -1;
		}

		public void Dispose()
		{
		}

		private UsbInterface GetCurrent()
		{
			try
			{
				return _interfaces[_index];
			}
			catch (IndexOutOfRangeException)
			{
				throw new InvalidOperationException();
			}
		}

		public bool MoveNext()
		{
			_index++;
			return _index < _interfaces.Length;
		}

		public void Reset()
		{
			_index = -1;
		}
	}

	private UsbInterface[] _interfaces;

	public UsbInterface this[int interfaceNumber]
	{
		get
		{
			for (int i = 0; i < _interfaces.Length; i++)
			{
				UsbInterface usbInterface = _interfaces[i];
				if (usbInterface.Number == interfaceNumber)
				{
					return usbInterface;
				}
			}
			throw new IndexOutOfRangeException($"No interface with number {interfaceNumber} exists.");
		}
	}

	internal UsbInterfaceCollection(UsbInterface[] interfaces)
	{
		_interfaces = interfaces;
	}

	public UsbInterface Find(UsbBaseClass interfaceClass)
	{
		for (int i = 0; i < _interfaces.Length; i++)
		{
			UsbInterface usbInterface = _interfaces[i];
			if (usbInterface.BaseClass == interfaceClass)
			{
				return usbInterface;
			}
		}
		return null;
	}

	public UsbInterface[] FindAll(UsbBaseClass interfaceClass)
	{
		List<UsbInterface> list = new List<UsbInterface>();
		for (int i = 0; i < _interfaces.Length; i++)
		{
			UsbInterface usbInterface = _interfaces[i];
			if (usbInterface.BaseClass == interfaceClass)
			{
				list.Add(usbInterface);
			}
		}
		return list.ToArray();
	}

	public IEnumerator<UsbInterface> GetEnumerator()
	{
		return new USBInterfaceEnumerator(_interfaces);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new USBInterfaceEnumerator(_interfaces);
	}
}
