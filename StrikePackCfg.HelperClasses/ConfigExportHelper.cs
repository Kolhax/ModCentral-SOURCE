using System;
using System.IO;

namespace StrikePackCfg.HelperClasses;

internal static class ConfigExportHelper
{
	private const uint MAGIC = 1128484947u;

	public static ConfigExportData Load(Stream stream)
	{
		using BinaryReader binaryReader = new BinaryReader(stream);
		if (binaryReader.ReadUInt32() != 1128484947)
		{
			throw new InvalidDataException("Invalid MAGIC");
		}
		byte b = binaryReader.ReadByte();
		if (b == 0)
		{
			int key = binaryReader.ReadInt32();
			int count = binaryReader.ReadInt32();
			byte[] array = binaryReader.ReadBytes(count);
			File.WriteAllBytes("enc_dec.bin", array);
			byte[] array2 = XXTEA.Decrypt(array, key);
			File.WriteAllBytes("raw_dec.bin", array2);
			return ConfigExportData.FromBytes(array2);
		}
		throw new NotSupportedException($"SPCC Version: {b}");
	}

	public static void Save(this ConfigExportData data, Stream stream)
	{
		using BinaryWriter binaryWriter = new BinaryWriter(stream);
		binaryWriter.Write(1128484947u);
		binaryWriter.Write((byte)0);
		Random random = new Random();
		int num = (int)DateTime.Now.Ticks % random.Next() + random.Next();
		binaryWriter.Write(num);
		byte[] array = data.ToBytes();
		File.WriteAllBytes("raw_enc.bin", array);
		byte[] array2 = XXTEA.Encrypt(array, num);
		File.WriteAllBytes("enc_enc.bin", array2);
		binaryWriter.Write(array2.Length);
		binaryWriter.Write(array2);
	}
}
