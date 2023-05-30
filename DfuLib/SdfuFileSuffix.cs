using System.IO;

namespace DfuLib;

public class SdfuFileSuffix
{
	public const uint CorrectSignature = 272909909u;

	public const ushort CorrectDfuBcd = 256;

	public const int SuffixLength = 16;

	public uint Crc { get; set; }

	public uint Signature { get; set; }

	public ushort DfuBcd { get; set; }

	public ushort VendorId { get; set; }

	public ushort ProductId { get; set; }

	public ushort DeviceBcd { get; set; }

	public bool SignatureIsCorrect => Signature == 272909909;

	public bool DfuBcdIsCorrect => DfuBcd == 256;

	public bool IsValid
	{
		get
		{
			if (SignatureIsCorrect)
			{
				return DfuBcdIsCorrect;
			}
			return false;
		}
	}

	public SdfuFileSuffix()
	{
		Signature = 272909909u;
		DfuBcd = 256;
	}

	public void ToBinaryWriter(BinaryWriter b)
	{
		b.Write(DeviceBcd);
		b.Write(ProductId);
		b.Write(VendorId);
		b.Write(DfuBcd);
		b.Write(Signature);
		byte[] array = new byte[b.BaseStream.Length];
		b.BaseStream.Seek(0L, SeekOrigin.Begin);
		b.BaseStream.Read(array, 0, array.Length);
		Crc = Crc32.Compute(array);
		b.Write(Crc);
	}

	public static SdfuFileSuffix FromBinaryReader(BinaryReader b, bool seek = true, bool restore = true)
	{
		long position = b.BaseStream.Position;
		SdfuFileSuffix sdfuFileSuffix = new SdfuFileSuffix();
		if (seek)
		{
			b.BaseStream.Seek(-16L, SeekOrigin.End);
		}
		sdfuFileSuffix.DeviceBcd = b.ReadUInt16();
		sdfuFileSuffix.ProductId = b.ReadUInt16();
		sdfuFileSuffix.VendorId = b.ReadUInt16();
		sdfuFileSuffix.DfuBcd = b.ReadUInt16();
		sdfuFileSuffix.Signature = b.ReadUInt32();
		sdfuFileSuffix.Crc = b.ReadUInt32();
		if (seek && restore)
		{
			b.BaseStream.Seek(position, SeekOrigin.Begin);
		}
		return sdfuFileSuffix;
	}
}
