using System;
using System.Collections;
using System.Collections.Generic;

namespace DfuLib.WinUsbNet;

public class UsbPipeCollection : IEnumerable<UsbPipe>, IEnumerable
{
	private class UsbPipeEnumerator : IEnumerator<UsbPipe>, IDisposable, IEnumerator
	{
		private int _index;

		private UsbPipe[] _pipes;

		public UsbPipe Current => GetCurrent();

		object IEnumerator.Current => GetCurrent();

		public UsbPipeEnumerator(UsbPipe[] pipes)
		{
			_pipes = pipes;
			_index = -1;
		}

		public void Dispose()
		{
		}

		private UsbPipe GetCurrent()
		{
			try
			{
				return _pipes[_index];
			}
			catch (IndexOutOfRangeException)
			{
				throw new InvalidOperationException();
			}
		}

		public bool MoveNext()
		{
			_index++;
			return _index < _pipes.Length;
		}

		public void Reset()
		{
			_index = -1;
		}
	}

	private Dictionary<byte, UsbPipe> _pipes;

	public UsbPipe this[byte pipeAddress]
	{
		get
		{
			if (!_pipes.TryGetValue(pipeAddress, out var value))
			{
				throw new IndexOutOfRangeException();
			}
			return value;
		}
	}

	internal UsbPipeCollection(UsbPipe[] pipes)
	{
		_pipes = new Dictionary<byte, UsbPipe>(pipes.Length);
		foreach (UsbPipe usbPipe in pipes)
		{
			if (_pipes.ContainsKey(usbPipe.Address))
			{
				throw new UsbException("Duplicate pipe address in endpoint.");
			}
			_pipes[usbPipe.Address] = usbPipe;
		}
	}

	private UsbPipe[] GetPipeList()
	{
		Dictionary<byte, UsbPipe>.ValueCollection values = _pipes.Values;
		UsbPipe[] array = new UsbPipe[values.Count];
		values.CopyTo(array, 0);
		return array;
	}

	public IEnumerator<UsbPipe> GetEnumerator()
	{
		return new UsbPipeEnumerator(GetPipeList());
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new UsbPipeEnumerator(GetPipeList());
	}
}
