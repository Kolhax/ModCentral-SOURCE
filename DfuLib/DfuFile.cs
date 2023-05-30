using System.Collections.Generic;
using System.IO;

namespace DfuLib;

public class DfuFile
{
	public class DfuLockBlock
	{
		public const int DataLength = 112;

		public readonly byte[] Data;

		private DfuLockBlock(byte[] data)
		{
			Data = data;
		}

		public static DfuLockBlock FromBinaryReader(BinaryReader b)
		{
			byte[] array = new byte[112];
			b.Read(array, 0, array.Length);
			return new DfuLockBlock(array);
		}
	}

	public class DfuDataBlock
	{
		public const int DataLength = 1056;

		public readonly byte[] Data;

		private DfuDataBlock(byte[] data)
		{
			Data = data;
		}

		public static DfuDataBlock FromBinaryReader(BinaryReader b)
		{
			byte[] array = new byte[1056];
			b.Read(array, 0, array.Length);
			return new DfuDataBlock(array);
		}
	}

	public bool IsValid
	{
		get
		{
			if (DfuSuffix.IsValid)
			{
				return ActualCrc == DfuSuffix.Crc;
			}
			return false;
		}
	}

	public DfuLockBlock LockBlock { get; private set; }

	public List<DfuDataBlock> DataBlocks { get; private set; }

	public SdfuFileSuffix DfuSuffix { get; private set; }

	public uint ActualCrc { get; private set; }

	public static DfuFile FromBinaryReader(BinaryReader b)
	{
		DfuFile dfuFile = new DfuFile();
		dfuFile.DfuSuffix = SdfuFileSuffix.FromBinaryReader(b);
		if (!dfuFile.DfuSuffix.IsValid)
		{
			return dfuFile;
		}
		dfuFile.ActualCrc = Crc32.Compute(b.ReadBytes((int)b.BaseStream.Length - 4));
		b.BaseStream.Seek(0L, SeekOrigin.Begin);
		dfuFile.LockBlock = DfuLockBlock.FromBinaryReader(b);
		int num = (int)(b.BaseStream.Length - b.BaseStream.Position) - 16;
		if (num % 1056 != 0)
		{
			throw new InvalidDataException("remaining data is not a multiple of BlockSize");
		}
		int num2 = num / 1056;
		for (int i = 0; i < num2; i++)
		{
			dfuFile.DataBlocks.Add(DfuDataBlock.FromBinaryReader(b));
		}
		return dfuFile;
	}

	public DfuFile()
	{
		DataBlocks = new List<DfuDataBlock>();
		DfuSuffix = new SdfuFileSuffix();
	}
}
