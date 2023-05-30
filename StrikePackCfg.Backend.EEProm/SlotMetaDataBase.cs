using System;
using System.IO;
using System.Text;

namespace StrikePackCfg.Backend.EEProm;

public abstract class SlotMetaDataBase
{
	[Flags]
	public enum SlotFlags : byte
	{
		Default = 3,
		Enabled = 1,
		Remap = 2,
		Rumble = 4,
		Gpc = 8,
		InvertLX = 0x10,
		InvertLY = 0x20,
		InvertRX = 0x40,
		InvertRY = 0x80
	}

	[Flags]
	public enum SlotExtraFlags : byte
	{
		Default = 0,
		UseHairTriggersLeft = 1,
		UseHairTriggersRight = 2
	}

	public class AdjustmentData
	{
		public sbyte LX;

		public sbyte LY;

		public sbyte RX;

		public sbyte RY;

		public sbyte LT { get; set; }

		public sbyte L2
		{
			get
			{
				return LT;
			}
			set
			{
				LT = value;
			}
		}

		public sbyte RT { get; set; }

		public sbyte R2
		{
			get
			{
				return RT;
			}
			set
			{
				RT = value;
			}
		}

		public byte[] ToBytes()
		{
			return new byte[6]
			{
				(byte)LT,
				(byte)RT,
				(byte)LX,
				(byte)LY,
				(byte)RX,
				(byte)RY
			};
		}

		public void ReadFromStream(BinaryReader br)
		{
			LT = br.ReadSByte();
			RT = br.ReadSByte();
			LX = br.ReadSByte();
			LY = br.ReadSByte();
			RX = br.ReadSByte();
			RY = br.ReadSByte();
		}
	}

	private const uint Magic = 5259597u;

	public AdjustmentData Deadzone = new AdjustmentData();

	public readonly short[] Pvars = new short[64];

	public AdjustmentData Sensitivity = new AdjustmentData();

	public SlotFlags Flags = SlotFlags.Default;

	public SlotExtraFlags ExtraFlags;

	public ushort GamePackId;

	public ushort GamePackVersion;

	public uint LeftPaddleMask;

	public uint RightPaddleMask;

	public uint LeftPaddleMask2;

	public uint RightPaddleMask2;

	public abstract byte[] Maps { get; protected set; }

	public ushort Size
	{
		get
		{
			int num = 4;
			num += 2;
			num += 2;
			if (Is16Bit && !App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
			{
				num += 2;
				num += 2;
				num += 2;
			}
			if (Is16Bit && App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
			{
				num += 2;
				num += 2;
				num += 2;
				num += 2;
				num += 2;
			}
			if (App.CurrentDevice.Device.Product.Is32Bit() && App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
			{
				num += 4;
				num += 4;
				num += 4;
				num += 4;
				num += 4;
			}
			if (App.CurrentDevice.Device.Product.Is32Bit() && !App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
			{
				num += 4;
				num += 4;
				num += 4;
			}
			if (SupportsAdjustments)
			{
				num += 6;
				num += 6;
			}
			num += Maps.Length;
			num += Pvars.Length * 2;
			return (ushort)num;
		}
	}

	private bool Is16Bit { get; set; }

	private bool SupportsAdjustments { get; set; }

	private bool CanReflashFPS { get; set; }

	protected SlotMetaDataBase(bool is16Bit, bool supportsAdjustments)
	{
		Is16Bit = is16Bit;
		SupportsAdjustments = supportsAdjustments;
	}

	public byte[] ToBytes()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
		{
			binaryWriter.Write(5259597u);
			binaryWriter.Write(GamePackId);
			binaryWriter.Write(GamePackVersion);
			binaryWriter.Write((byte)Flags);
			binaryWriter.Write((byte)ExtraFlags);
			if (Is16Bit && !App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
			{
				binaryWriter.Write((ushort)LeftPaddleMask);
				binaryWriter.Write((ushort)RightPaddleMask);
			}
			if (Is16Bit && App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
			{
				binaryWriter.Write((ushort)LeftPaddleMask);
				binaryWriter.Write((ushort)RightPaddleMask);
				binaryWriter.Write((ushort)LeftPaddleMask2);
				binaryWriter.Write((ushort)RightPaddleMask2);
			}
			if (App.CurrentDevice.Device.Product.Is32Bit() && !App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
			{
				binaryWriter.Write(new byte[2]);
				binaryWriter.Write(LeftPaddleMask);
				binaryWriter.Write(RightPaddleMask);
			}
			if (App.CurrentDevice.Device.Product.Is32Bit() && App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
			{
				binaryWriter.Write(new byte[2]);
				binaryWriter.Write(LeftPaddleMask);
				binaryWriter.Write(RightPaddleMask);
				binaryWriter.Write(LeftPaddleMask2);
				binaryWriter.Write(RightPaddleMask2);
			}
			binaryWriter.Write(Maps);
			if (SupportsAdjustments)
			{
				binaryWriter.Write(Deadzone.ToBytes());
				binaryWriter.Write(Sensitivity.ToBytes());
			}
			short[] pvars = Pvars;
			foreach (short value in pvars)
			{
				binaryWriter.Write(value);
			}
		}
		return memoryStream.ToArray();
	}

	public static SlotMetaDataBase FromBytes(byte[] data, SlotMetaDataBase meta)
	{
		using MemoryStream stream = new MemoryStream(data);
		return FromStream(stream, meta);
	}

	private static SlotMetaDataBase FromStream(Stream stream, SlotMetaDataBase meta)
	{
		if (stream.Length != meta.Size)
		{
			throw new InvalidDataException($"Incorrect size, got {stream.Length} expected {meta.Size}");
		}
		using BinaryReader binaryReader = new BinaryReader(stream, Encoding.Default, leaveOpen: true);
		if (binaryReader.ReadUInt32() != 5259597)
		{
			throw new InvalidDataException("Bad Magic");
		}
		meta.GamePackId = binaryReader.ReadUInt16();
		meta.GamePackVersion = binaryReader.ReadUInt16();
		if (meta.Is16Bit && !App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
		{
			meta.Flags = (SlotFlags)binaryReader.ReadByte();
			meta.ExtraFlags = (SlotExtraFlags)binaryReader.ReadByte();
			meta.LeftPaddleMask = binaryReader.ReadUInt16();
			meta.RightPaddleMask = binaryReader.ReadUInt16();
		}
		if (meta.Is16Bit && App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
		{
			meta.Flags = (SlotFlags)binaryReader.ReadByte();
			meta.ExtraFlags = (SlotExtraFlags)binaryReader.ReadByte();
			meta.LeftPaddleMask = binaryReader.ReadUInt16();
			meta.RightPaddleMask = binaryReader.ReadUInt16();
			meta.LeftPaddleMask2 = binaryReader.ReadUInt16();
			meta.RightPaddleMask2 = binaryReader.ReadUInt16();
		}
		if (App.CurrentDevice.Device.Product.Is32Bit() && !App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
		{
			meta.Flags = (SlotFlags)binaryReader.ReadByte();
			meta.ExtraFlags = (SlotExtraFlags)binaryReader.ReadByte();
			binaryReader.ReadUInt16();
			meta.LeftPaddleMask = binaryReader.ReadUInt32();
			meta.RightPaddleMask = binaryReader.ReadUInt32();
		}
		if (App.CurrentDevice.Device.Product.Is32Bit() && App.CurrentDevice.Device.Product.Flags.HasFlag(CollectiveMindsProduct.StrikeMaxFlags.BattlePack))
		{
			meta.Flags = (SlotFlags)binaryReader.ReadByte();
			meta.ExtraFlags = (SlotExtraFlags)binaryReader.ReadByte();
			binaryReader.ReadUInt16();
			meta.LeftPaddleMask = binaryReader.ReadUInt32();
			meta.RightPaddleMask = binaryReader.ReadUInt32();
			meta.LeftPaddleMask2 = binaryReader.ReadUInt32();
			meta.RightPaddleMask2 = binaryReader.ReadUInt32();
		}
		meta.Maps = binaryReader.ReadBytes(meta.Maps.Length);
		if (meta.SupportsAdjustments)
		{
			meta.Deadzone.ReadFromStream(binaryReader);
			meta.Sensitivity.ReadFromStream(binaryReader);
		}
		for (int i = 0; i < meta.Pvars.Length; i++)
		{
			meta.Pvars[i] = binaryReader.ReadInt16();
		}
		return meta;
	}
}
