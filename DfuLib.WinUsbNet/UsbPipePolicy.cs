using System;
using DfuLib.WinUsbNet.Api;

namespace DfuLib.WinUsbNet;

public class UsbPipePolicy
{
	private byte _pipeID;

	private int _interfaceIndex;

	private UsbDevice _device;

	public bool AllowPartialReads
	{
		get
		{
			RequireDirectionIn();
			return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, POLICY_TYPE.ALLOW_PARTIAL_READS);
		}
		set
		{
			RequireDirectionIn();
			_device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, POLICY_TYPE.ALLOW_PARTIAL_READS, value);
		}
	}

	public bool AutoClearStall
	{
		get
		{
			return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, POLICY_TYPE.AUTO_CLEAR_STALL);
		}
		set
		{
			_device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, POLICY_TYPE.AUTO_CLEAR_STALL, value);
		}
	}

	public bool AutoFlush
	{
		get
		{
			RequireDirectionIn();
			return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, POLICY_TYPE.AUTO_FLUSH);
		}
		set
		{
			RequireDirectionIn();
			_device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, POLICY_TYPE.AUTO_FLUSH, value);
		}
	}

	public bool IgnoreShortPackets
	{
		get
		{
			RequireDirectionIn();
			return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, POLICY_TYPE.IGNORE_SHORT_PACKETS);
		}
		set
		{
			RequireDirectionIn();
			_device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, POLICY_TYPE.IGNORE_SHORT_PACKETS, value);
		}
	}

	public int PipeTransferTimeout
	{
		get
		{
			return (int)_device.InternalDevice.GetPipePolicyUInt(_interfaceIndex, _pipeID, POLICY_TYPE.PIPE_TRANSFER_TIMEOUT);
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("Pipe transfer timeout cannot be negative.");
			}
			_device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, POLICY_TYPE.PIPE_TRANSFER_TIMEOUT, (uint)value);
		}
	}

	public bool RawIO
	{
		get
		{
			return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, POLICY_TYPE.RAW_IO);
		}
		set
		{
			_device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, POLICY_TYPE.RAW_IO, value);
		}
	}

	public bool ShortPacketTerminate
	{
		get
		{
			RequireDirectionOut();
			return _device.InternalDevice.GetPipePolicyBool(_interfaceIndex, _pipeID, POLICY_TYPE.SHORT_PACKET_TERMINATE);
		}
		set
		{
			RequireDirectionOut();
			_device.InternalDevice.SetPipePolicy(_interfaceIndex, _pipeID, POLICY_TYPE.SHORT_PACKET_TERMINATE, value);
		}
	}

	internal UsbPipePolicy(UsbDevice device, int interfaceIndex, byte pipeID)
	{
		_pipeID = pipeID;
		_interfaceIndex = interfaceIndex;
		_device = device;
	}

	private void RequireDirectionOut()
	{
		if ((_pipeID & 0x80u) != 0)
		{
			throw new NotSupportedException("This policy type is only allowed on OUT direction pipes.");
		}
	}

	private void RequireDirectionIn()
	{
		if ((_pipeID & 0x80) == 0)
		{
			throw new NotSupportedException("This policy type is only allowed on IN direction pipes.");
		}
	}
}
