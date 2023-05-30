using System.IO;
using System.Text;

namespace StrikePackCfg.Backend.EEProm;

public class PaddlesData
{
	private const uint Magic = 5259597u;

	private readonly bool _is16Bit;

	public readonly uint LeftMask;

	public readonly uint RightMask;

	public readonly uint LeftMask2;

	public readonly uint RightMask2;

	public PaddlesData(uint left, uint right, bool is16Bit)
	{
		LeftMask = left;
		RightMask = right;
		_is16Bit = is16Bit;
	}

	public PaddlesData(uint left, uint right, uint left2, uint right2, bool is16Bit)
	{
		LeftMask = left;
		RightMask = right;
		LeftMask2 = left2;
		RightMask2 = right2;
		_is16Bit = is16Bit;
	}

	public byte[] ToBytes()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
		{
			binaryWriter.Write(5259597u);
			if (_is16Bit && !App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
			{
				binaryWriter.Write((ushort)LeftMask);
				binaryWriter.Write((ushort)RightMask);
			}
			else if (App.CurrentDevice.Device.Product.Is32Bit() && App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
			{
				binaryWriter.Write(LeftMask);
				binaryWriter.Write(RightMask);
				binaryWriter.Write(LeftMask2);
				binaryWriter.Write(RightMask2);
			}
			else if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
			{
				binaryWriter.Write((ushort)LeftMask);
				binaryWriter.Write((ushort)RightMask);
				binaryWriter.Write((ushort)LeftMask2);
				binaryWriter.Write((ushort)RightMask2);
			}
			else
			{
				binaryWriter.Write(LeftMask);
				binaryWriter.Write(RightMask);
			}
		}
		return memoryStream.ToArray();
	}

	public static PaddlesData FromBytes(byte[] data, bool is16Bit)
	{
		using MemoryStream stream = new MemoryStream(data);
		return FromStream(stream, is16Bit);
	}

	private static PaddlesData FromStream(Stream stream, bool is16Bit)
	{
		using BinaryReader binaryReader = new BinaryReader(stream, Encoding.Default, leaveOpen: true);
		if (binaryReader.ReadUInt32() != 5259597)
		{
			throw new InvalidDataException("Bad Magic");
		}
		uint left;
		uint right;
		uint left2;
		uint right2;
		if (is16Bit && !App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
		{
			left = binaryReader.ReadUInt16();
			right = binaryReader.ReadUInt16();
			left2 = 0u;
			right2 = 0u;
		}
		else if (App.CurrentDevice.Device.Product.Is32Bit() && App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
		{
			left = binaryReader.ReadUInt32();
			right = binaryReader.ReadUInt32();
			left2 = binaryReader.ReadUInt32();
			right2 = binaryReader.ReadUInt32();
		}
		else if (App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
		{
			left = binaryReader.ReadUInt16();
			right = binaryReader.ReadUInt16();
			left2 = binaryReader.ReadUInt16();
			right2 = binaryReader.ReadUInt16();
		}
		else
		{
			left = binaryReader.ReadUInt32();
			right = binaryReader.ReadUInt32();
			left2 = 0u;
			right2 = 0u;
		}
		return new PaddlesData(left, right, left2, right2, is16Bit);
	}
}
