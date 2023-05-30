using System;
using System.IO;

namespace SPGamepack;

public class GamepackFile
{
	private const uint MAGIC = 1346850899u;

	public GamepackBlob GamepackData { get; set; }

	public static GamepackFile FromStream(Stream stream)
	{
		using BinaryReader binaryReader = new BinaryReader(stream);
		if (binaryReader.ReadUInt32() != 1346850899)
		{
			throw new InvalidDataException("BAD MAGIC!");
		}
		byte b = binaryReader.ReadByte();
		if (b == 0)
		{
			int key = binaryReader.ReadInt32();
			int count = binaryReader.ReadInt32();
			using MemoryStream memoryStream = new MemoryStream(XXTEA.Decrypt(binaryReader.ReadBytes(count), key));
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return new GamepackFile
			{
				GamepackData = GamepackBlob.FromStream(memoryStream)
			};
		}
		throw new NotSupportedException($"Unsupported fileformat version: {b}");
	}

	public byte[] Save()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
		{
			binaryWriter.Write(1346850899u);
			binaryWriter.Write((byte)0);
			Random random = new Random();
			int num = (int)((DateTime.UtcNow.ToBinary() + random.Next()) / random.Next()) + random.Next();
			binaryWriter.Write(num);
			byte[] data;
			using (MemoryStream memoryStream2 = new MemoryStream())
			{
				GamepackData.Save(memoryStream2);
				data = memoryStream2.ToArray();
			}
			data = XXTEA.Encrypt(data, num);
			binaryWriter.Write(data.Length);
			binaryWriter.Write(data);
		}
		return memoryStream.ToArray();
	}
}
