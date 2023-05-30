using System.IO;
using System.Text;

namespace SPGamepack;

public class GamepackBlob
{
	public enum EncryptionMethods : byte
	{
		None = 0,
		Tea = 1,
		TeaXb1 = 3,
		TeaPs4 = 5
	}

	public string Name { get; set; }

	public string Version { get; set; }

	public ushort GamepackID { get; set; }

	public byte[] Bytecode { get; set; }

	public EncryptionMethods EncryptionMethod { get; set; }

	public string CFGData { get; set; }

	public bool IsPs4
	{
		get
		{
			return GetIdFlag();
		}
		set
		{
			SetIdFlag(value);
		}
	}

	public bool Deprecated
	{
		get
		{
			return !GetIdFlag();
		}
		set
		{
			SetIdFlag(!value);
		}
	}

	public static GamepackBlob FromStream(Stream stream)
	{
		using BinaryReader binaryReader = new BinaryReader(stream);
		GamepackBlob gamepackBlob = new GamepackBlob();
		int count = binaryReader.ReadUInt16();
		gamepackBlob.Name = Encoding.UTF8.GetString(binaryReader.ReadBytes(count));
		count = binaryReader.ReadByte();
		gamepackBlob.Version = Encoding.UTF8.GetString(binaryReader.ReadBytes(count));
		gamepackBlob.GamepackID = binaryReader.ReadUInt16();
		count = binaryReader.ReadUInt16();
		gamepackBlob.Bytecode = binaryReader.ReadBytes(count);
		gamepackBlob.EncryptionMethod = (EncryptionMethods)binaryReader.ReadByte();
		count = binaryReader.ReadInt32();
		gamepackBlob.CFGData = Encoding.UTF8.GetString(binaryReader.ReadBytes(count));
		return gamepackBlob;
	}

	public void Save(Stream stream)
	{
		using BinaryWriter binaryWriter = new BinaryWriter(stream);
		byte[] bytes = Encoding.UTF8.GetBytes(Name.TrimUTF8(65535));
		binaryWriter.Write((ushort)bytes.Length);
		binaryWriter.Write(bytes);
		bytes = Encoding.UTF8.GetBytes(Version.TrimUTF8(255));
		binaryWriter.Write((byte)bytes.Length);
		binaryWriter.Write(bytes);
		binaryWriter.Write(GamepackID);
		if (Bytecode.Length > 65535)
		{
			throw new InvalidDataException($"Bytecode can't be bigger then {ushort.MaxValue}, the data was {Bytecode.Length} bytes!");
		}
		binaryWriter.Write((ushort)Bytecode.Length);
		binaryWriter.Write(Bytecode);
		binaryWriter.Write((byte)EncryptionMethod);
		if (!string.IsNullOrWhiteSpace(CFGData))
		{
			bytes = Encoding.UTF8.GetBytes(CFGData.TrimUTF8(int.MaxValue));
			binaryWriter.Write(bytes.Length);
			binaryWriter.Write(bytes);
		}
		else
		{
			binaryWriter.Write(0);
		}
	}

	private void SetIdFlag(bool state)
	{
		if (state)
		{
			GamepackID |= 32768;
		}
		else
		{
			GamepackID = (ushort)(GamepackID & 0xFFFF7FFFu);
		}
	}

	private bool GetIdFlag()
	{
		return (GamepackID & 0x8000) == 32768;
	}
}
