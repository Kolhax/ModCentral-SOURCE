using System;
using System.Text;
using DfuLib.WinUsbNet;

namespace DfuLib;

public class CmPpDfuDeviceBackend : DfuDeviceBackendBase
{
	public enum DfuRequest : byte
	{
		Detach,
		Download,
		Upload,
		GetStatus,
		ClearStatus,
		GetState,
		Abort
	}

	[Flags]
	public enum UsbRequestFlag : byte
	{
		RecipientDevice = 0,
		RecipientInterface = 1,
		RecipientEndpoint = 2,
		RecipientOther = 3,
		TypeStandard = 0,
		TypeClass = 0x20,
		TypeVendor = 0x40,
		TypeReserved = 0x60,
		DirectionOut = 0,
		DirectionIn = 0x80
	}

	public const string DeviceGuid = "";

	private UsbDevice device;

	public CmPpDfuDeviceBackend(string devicePathName)
	{
		device = new UsbDevice(devicePathName);
	}

	public CmPpDfuDeviceBackend(UsbDeviceInfo deviceInfo)
	{
		device = new UsbDevice(deviceInfo);
	}

	public override void Detach()
	{
		byte requestType = 65;
		device.ControlOut(requestType, 0, 3000, 3);
	}

	public override void Download(ushort block, byte[] data)
	{
		byte requestType = 65;
		device.ControlOut(requestType, 1, block, 3, data);
	}

	public override byte[] Upload(ushort block, uint length)
	{
		byte requestType = 193;
		return device.ControlIn(requestType, 2, 0, 3, (int)length);
	}

	public override DfuStatus GetStatus()
	{
		byte requestType = 193;
		byte[] array = device.ControlIn(requestType, 3, 0, 3, 6);
		return new DfuStatus
		{
			Status = (DfuError)array[0],
			PollTimeout = (array[1] | (array[2] << 8) | (array[3] << 16)),
			State = (DfuState)array[4],
			String = array[5]
		};
	}

	public override void ClearStatus()
	{
		byte requestType = 65;
		device.ControlTransfer(requestType, 4, 0, 3);
	}

	public override DfuState GetState()
	{
		byte requestType = 193;
		byte[] array = device.ControlIn(requestType, 5, 0, 3, 1);
		if (array[0] > 10)
		{
			throw new ArgumentOutOfRangeException("bytes", array[0], "");
		}
		return (DfuState)array[0];
	}

	public override void Abort()
	{
		byte requestType = 65;
		device.ControlTransfer(requestType, 6, 0, 3);
	}

	public override void Close()
	{
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		if (disposing && device != null)
		{
			device.Dispose();
			device = null;
		}
	}

	private void Download(CmPpDfuDownloadPacket packet)
	{
		Download(packet.Block, packet.ToBytes());
	}

	public void EfsWrite(string fileName, byte[] data)
	{
		Download(CmPpDfuDownloadPacket.EfsWrite(fileName, data));
	}

	public void FlashWrite(ushort block, byte[] data)
	{
		Download(CmPpDfuDownloadPacket.FlashWrite(block, data));
	}

	public void FlashWriteUnc(ushort block, byte[] data)
	{
		Download(CmPpDfuDownloadPacket.FlashWriteUnc(block, data));
	}

	public string GetStringDescriptor(int index, bool trimNulls = false)
	{
		byte requestType = 128;
		byte[] array = device.ControlIn(requestType, 6, 0x300 | (index & 0xFF), 0, 2);
		byte[] array2 = device.ControlIn(requestType, 6, 0x300 | (index & 0xFF), 0, array[0]);
		string @string = Encoding.Unicode.GetString(array2, 2, array2.Length - 2);
		if (!trimNulls)
		{
			return @string;
		}
		return @string.TrimEnd(default(char));
	}

	public void EfsFormat()
	{
		Execute(delegate
		{
			Download(CmPpDfuDownloadPacket.EfsFormat());
		}, DfuState.DownloadIdle, "EfsFormat failed");
	}

	public void FlashFormat()
	{
		Download(CmPpDfuDownloadPacket.FlashFormat());
	}

	public void EnterBootloader()
	{
		Download(CmPpDfuDownloadPacket.EnterBootloader());
	}

	public void ReloadSlots()
	{
		Download(CmPpDfuDownloadPacket.ReloadSlots());
	}

	public string GetSerialNumber()
	{
		return GetStringDescriptor(4, trimNulls: true);
	}

	public string GetApplicationVersion()
	{
		return GetStringDescriptor(5, trimNulls: true);
	}

	public byte[] EfsRead(string fileName, ushort length)
	{
		byte[] ret = null;
		Execute(delegate
		{
			ClearErrorAndAbort();
		}, DfuState.Idle, "EfsRead ClearErrorAndAbort failed");
		Execute(delegate
		{
			Download(CmPpDfuDownloadPacket.EfsRead(fileName, length));
		}, DfuState.DownloadIdle, "EfsRead Download failed");
		Execute(delegate
		{
			Download(0, new byte[0]);
		}, DfuState.Idle, "EfsRead Manifest failed");
		Execute(delegate
		{
			ret = Upload(0, length);
		}, DfuState.Idle, "EfsRead Upload failed");
		return ret;
	}

	public void EnterController()
	{
		Execute(delegate
		{
			ClearErrorAndAbort();
		}, DfuState.Idle, "EnterController ClearErrorAndAbort failed");
		Execute(delegate
		{
			Download(CmPpDfuDownloadPacket.EnterController());
		}, DfuState.Error, "EnterController Download failed");
	}
}
