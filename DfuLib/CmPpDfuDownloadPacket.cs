using System;
using System.IO;

namespace DfuLib;

internal class CmPpDfuDownloadPacket
{
	private enum PacketType
	{
		EfsWrite,
		EfsFormat,
		FlashWrite,
		FlashFormat,
		EnterBootloader,
		ReloadSlots,
		EfsRead,
		EnterController,
		FlashWriteUnc
	}

	internal ushort Block;

	private PacketType type;

	private ushort length;

	private string fileName;

	private byte[] data;

	private CmPpDfuDownloadPacket(PacketType packetType)
	{
		type = packetType;
	}

	public byte[] ToBytes()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
		{
			binaryWriter.Write((uint)type);
			switch (type)
			{
			case PacketType.EfsWrite:
				binaryWriter.Write(fileName, 4, '\0', nullTerm: false);
				binaryWriter.Write((ushort)data.Length);
				binaryWriter.Write(data);
				break;
			case PacketType.EfsRead:
				binaryWriter.Write(fileName, 4, '\0', nullTerm: false);
				binaryWriter.Write(length);
				break;
			case PacketType.FlashWrite:
			case PacketType.FlashWriteUnc:
				binaryWriter.Write(data);
				break;
			}
		}
		return memoryStream.ToArray();
	}

	public static CmPpDfuDownloadPacket EfsFormat()
	{
		return new CmPpDfuDownloadPacket(PacketType.EfsFormat);
	}

	public static CmPpDfuDownloadPacket FlashFormat()
	{
		return new CmPpDfuDownloadPacket(PacketType.FlashFormat);
	}

	public static CmPpDfuDownloadPacket EnterBootloader()
	{
		return new CmPpDfuDownloadPacket(PacketType.EnterBootloader);
	}

	public static CmPpDfuDownloadPacket ReloadSlots()
	{
		return new CmPpDfuDownloadPacket(PacketType.ReloadSlots);
	}

	public static CmPpDfuDownloadPacket EnterController()
	{
		return new CmPpDfuDownloadPacket(PacketType.EnterController);
	}

	public static CmPpDfuDownloadPacket EfsWrite(string fileName, byte[] data)
	{
		if (data.Length > 1018)
		{
			throw new ArgumentOutOfRangeException("data", "data cannot be larger than 1018 bytes");
		}
		return new CmPpDfuDownloadPacket(PacketType.EfsWrite)
		{
			fileName = fileName,
			data = data
		};
	}

	public static CmPpDfuDownloadPacket EfsRead(string fileName, ushort length)
	{
		if (length > 1024)
		{
			throw new ArgumentOutOfRangeException("length", "length cannot be larger than 1024 bytes");
		}
		return new CmPpDfuDownloadPacket(PacketType.EfsRead)
		{
			fileName = fileName,
			length = length
		};
	}

	public static CmPpDfuDownloadPacket FlashWrite(ushort block, byte[] data)
	{
		if (data.Length > 1024)
		{
			throw new ArgumentOutOfRangeException("data", "data cannot be larger than 1024 bytes");
		}
		return new CmPpDfuDownloadPacket(PacketType.FlashWrite)
		{
			Block = block,
			data = data
		};
	}

	public static CmPpDfuDownloadPacket FlashWriteUnc(ushort block, byte[] data)
	{
		if (data.Length > 1024)
		{
			throw new ArgumentOutOfRangeException("data", "data cannot be larger than 1024 bytes");
		}
		return new CmPpDfuDownloadPacket(PacketType.FlashWriteUnc)
		{
			Block = block,
			data = data
		};
	}
}
