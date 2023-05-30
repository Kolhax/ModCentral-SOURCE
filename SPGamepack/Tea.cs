using System;
using System.Collections.Generic;
using System.Linq;

namespace SPGamepack;

public static class Tea
{
	private const uint Delta = 2654435769u;

	private static readonly byte[] XboxOneKey = new byte[16]
	{
		4, 3, 7, 1, 9, 5, 6, 2, 3, 4,
		9, 0, 8, 6, 7, 5
	};

	private static readonly byte[] Ps4Key = new byte[16]
	{
		65, 173, 174, 31, 244, 53, 91, 234, 245, 243,
		37, 136, 97, 1, 216, 23
	};

	public static byte[] EncryptXboxOne(byte[] data)
	{
		return Encrypt(data, XboxOneKey);
	}

	public static byte[] EncryptPs4(byte[] data)
	{
		return Encrypt(data, Ps4Key);
	}

	private static byte[] Encrypt(byte[] data, byte[] key)
	{
		List<uint> list = new List<uint>();
		for (int i = 0; i < key.Length; i += 4)
		{
			list.Add(BitConverter.ToUInt32(key, i));
		}
		if (data.Length % 8 > 0)
		{
			Array.Resize(ref data, data.Length + (8 - data.Length % 8));
		}
		List<uint> list2 = new List<uint>();
		for (int j = 0; j < data.Length; j += 4)
		{
			list2.Add(BitConverter.ToUInt32(data, j));
		}
		uint[] key2 = list.ToArray();
		uint[] data2 = list2.ToArray();
		for (int k = 0; k < data2.Length; k += 2)
		{
			Encrypt(ref data2, key2, k);
		}
		return data2.SelectMany(BitConverter.GetBytes).ToArray();
	}

	private static void Encrypt(ref uint[] data, IReadOnlyList<uint> key, int index)
	{
		uint num = data[index];
		uint num2 = data[1 + index];
		uint num3 = 0u;
		uint num4 = key[0];
		uint num5 = key[1];
		uint num6 = key[2];
		uint num7 = key[3];
		for (uint num8 = 0u; num8 < 32; num8++)
		{
			num3 += 2654435769u;
			num += ((num2 << 4) + num4) ^ (num2 + num3) ^ ((num2 >> 5) + num5);
			num2 += ((num << 4) + num6) ^ (num + num3) ^ ((num >> 5) + num7);
		}
		data[index] = num;
		data[1 + index] = num2;
	}
}
