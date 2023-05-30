using System;
using System.Linq;
using DfuLib;

namespace StrikePackCfg.Backend;

internal static class FlashHelper
{
	public static void WriteToFlash(byte[] data, int slot, bool isEncrypted, int blocksPerSlot = 16)
	{
		byte[][] array = data.Partition(1024).ToArray();
		CmPpDfuDeviceBackend dev = App.CurrentDevice.Device.GetDeviceBackend();
		try
		{
			dev.ClearErrorAndAbort();
			int num = slot * blocksPerSlot;
			for (int i = 0; i < array.Length; i++)
			{
				byte[] buf = new byte[1024];
				Array.Copy(array[i], 0, buf, 0, array[i].Length);
				int block = num + i;
				if (isEncrypted)
				{
					dev.Execute(delegate
					{
						dev.FlashWrite((ushort)block, buf);
					}, DfuState.DownloadIdle, $"Attempting to flash gamepack slot '{slot}' to device '{App.CurrentDevice.Device.Product.DisplayName}'");
				}
				else
				{
					dev.Execute(delegate
					{
						dev.FlashWriteUnc((ushort)block, buf);
					}, DfuState.DownloadIdle, $"Attempting to flash gamepack slot '{slot}' to device '{App.CurrentDevice.Device.Product.DisplayName}'");
				}
			}
			dev.Manifest();
		}
		finally
		{
			if (dev != null)
			{
				((IDisposable)dev).Dispose();
			}
		}
	}

	public static void WriteToFlashAes(byte[] data)
	{
		byte[][] array = data.Partition(1024).ToArray();
		CmPpDfuDeviceBackend dev = App.CurrentDevice.Device.GetDeviceBackend();
		try
		{
			dev.ClearErrorAndAbort();
			ushort i = 0;
			byte[][] array2 = array;
			foreach (byte[] blk in array2)
			{
				dev.Execute(delegate
				{
					dev.FlashWrite(i, blk);
				}, DfuState.DownloadIdle, $"Attempting to flash aes encrypted block to device '{App.CurrentDevice.Device.Product.DisplayName}'");
				ushort num = i;
				i = (ushort)(num + 1);
			}
			dev.Manifest();
		}
		finally
		{
			if (dev != null)
			{
				((IDisposable)dev).Dispose();
			}
		}
	}
}
