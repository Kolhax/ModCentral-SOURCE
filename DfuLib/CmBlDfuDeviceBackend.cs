using System;
using System.Text;
using DfuLib.WinUsbNet;

namespace DfuLib;

public class CmBlDfuDeviceBackend : DfuDeviceBackendBase
{
	public enum DfuRequest : byte
	{
		Detach,
		Download,
		Upload,
		GetStatus,
		ClearStatus,
		GetState,
		Abort,
		Unlock
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

	public CmBlDfuDeviceBackend(string devicePathName)
	{
		device = new UsbDevice(devicePathName);
	}

	public CmBlDfuDeviceBackend(UsbDeviceInfo deviceInfo)
	{
		device = new UsbDevice(deviceInfo);
	}

	public override void Detach()
	{
		byte requestType = 33;
		device.ControlOut(requestType, 0, 3000, 3);
	}

	public override void Download(ushort block, byte[] data)
	{
		byte requestType = 33;
		device.ControlOut(requestType, 1, block, 3, data);
	}

	public override byte[] Upload(ushort block, uint length)
	{
		byte requestType = 161;
		return device.ControlIn(requestType, 2, 0, 3, (int)length);
	}

	public override DfuStatus GetStatus()
	{
		byte requestType = 161;
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
		byte requestType = 33;
		device.ControlTransfer(requestType, 4, 0, 3);
	}

	public override DfuState GetState()
	{
		byte requestType = 161;
		byte[] array = device.ControlIn(requestType, 5, 0, 3, 1);
		if (array[0] > 10)
		{
			throw new ArgumentOutOfRangeException("bytes", array[0], "");
		}
		return (DfuState)array[0];
	}

	public override void Abort()
	{
		byte requestType = 33;
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

	private void DfuUnlock(byte[] data)
	{
		byte requestType = 33;
		device.ControlOut(requestType, 7, 0, 0, data);
	}

	public byte[] GetLicenseBlob()
	{
		byte requestType = 193;
		return device.ControlIn(requestType, 2, 0, 0, 256);
	}

	public void EnterUserApplication(int flag = 0)
	{
		device.ControlTransfer(65, 3, flag, 0);
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

	public string GetSerialNumber()
	{
		return GetStringDescriptor(5, trimNulls: true);
	}

	public string GetBootloaderVersion()
	{
		return GetStringDescriptor(6, trimNulls: true);
	}

	public string GetApplicationVersion()
	{
		return GetStringDescriptor(7, trimNulls: true);
	}

	public void UnlockDevice(DfuFile.DfuLockBlock lockBlock)
	{
		Execute(delegate
		{
			DfuUnlock(lockBlock.Data);
		}, DfuState.Idle, "DfuUnlock failed");
	}

	public void FlashFile(DfuFile file, IDfuFlashProgress progressHandler = null)
	{
		int num = 0;
		if (!file.IsValid)
		{
			throw new ArgumentException("cannot flash an invalid file", "file");
		}
		progressHandler?.SetMaximumProgress(file.DataBlocks.Count + 3);
		Execute(delegate
		{
			ClearErrorAndAbort(DfuState.Locked);
		}, DfuState.Locked, "FlashFile ClearErrorAndAbort failed");
		progressHandler?.SetCurrentProgress(++num);
		UnlockDevice(file.LockBlock);
		progressHandler?.SetCurrentProgress(++num);
		ushort blockNumber = 0;
		foreach (DfuFile.DfuDataBlock block in file.DataBlocks)
		{
			Execute(delegate
			{
				CmBlDfuDeviceBackend cmBlDfuDeviceBackend = this;
				ushort num2 = blockNumber;
				blockNumber = (ushort)(num2 + 1);
				cmBlDfuDeviceBackend.Download(num2, block.Data);
			}, DfuState.DownloadIdle, $"FlashFile Download block {blockNumber} failed");
			progressHandler?.SetCurrentProgress(++num);
		}
		Execute(delegate
		{
			Download(0, new byte[0]);
		}, DfuState.Locked, "FlashFile Manifest failed");
		progressHandler?.SetCurrentProgress(++num);
	}
}
