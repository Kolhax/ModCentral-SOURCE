using System.Collections.Generic;
using System.IO;
using Semver;

namespace StrikePackCfg.HelperClasses;

public class ConfigExportData
{
	private const int PvarCount = 64;

	public ushort GamepackId { get; }

	public SemVersion GamepackVersion { get; }

	public short[] Pvars { get; } = new short[64];


	public ConfigExportData(ushort gamepackId, SemVersion gamepackVersion, IReadOnlyList<short> pvars)
	{
		GamepackId = gamepackId;
		GamepackVersion = gamepackVersion ?? ((SemVersion)"1.0");
		for (int i = 0; i < 64 && i < pvars.Count; i++)
		{
			Pvars[i] = pvars[i];
		}
	}

	public byte[] ToBytes()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(GamepackId);
		binaryWriter.Write(GamepackVersion);
		short[] pvars = Pvars;
		foreach (short value in pvars)
		{
			binaryWriter.Write(value);
		}
		return memoryStream.ToArray();
	}

	public static ConfigExportData FromBytes(byte[] data)
	{
		using MemoryStream input = new MemoryStream(data);
		using BinaryReader binaryReader = new BinaryReader(input);
		ushort gamepackId = binaryReader.ReadUInt16();
		SemVersion gamepackVersion = binaryReader.ReadSemVersion();
		short[] array = new short[64];
		for (int i = 0; i < 64; i++)
		{
			array[i] = binaryReader.ReadInt16();
		}
		return new ConfigExportData(gamepackId, gamepackVersion, array);
	}
}
