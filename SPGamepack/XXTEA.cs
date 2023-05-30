using System;
using System.Collections.Generic;

namespace SPGamepack;

public static class XXTEA
{
	private const uint Delta = 2654435769u;

	private static uint Mx(uint sum, uint y, uint z, int p, uint e, uint[] k)
	{
		return (((z >> 5) ^ (y << 2)) + ((y >> 3) ^ (z << 4))) ^ ((sum ^ y) + (k[(p & 3) ^ e] ^ z));
	}

	public static byte[] Encrypt(byte[] data, int key)
	{
		return Encrypt(data, key.MakeKey());
	}

	public static byte[] Encrypt(byte[] data, byte[] key)
	{
		if (data.Length == 0)
		{
			return data;
		}
		return Encrypt(ToUInt32Array(data, includeLength: true), ToUInt32Array(FixKey(key), includeLength: false)).ToByteArray();
	}

	public static byte[] Decrypt(byte[] data, int key)
	{
		return Decrypt(data, key.MakeKey());
	}

	public static byte[] Decrypt(byte[] data, byte[] key)
	{
		if (data.Length == 0)
		{
			return data;
		}
		return Decrypt(ToUInt32Array(data, includeLength: false), ToUInt32Array(FixKey(key), includeLength: false)).ToByteArray(includeLength: true);
	}

	private static uint[] Encrypt(uint[] v, uint[] k)
	{
		int num = v.Length - 1;
		if (num < 1)
		{
			return v;
		}
		uint z = v[num];
		uint num2 = 0u;
		int num3 = 6 + 52 / (num + 1);
		while (0 < num3--)
		{
			num2 += 2654435769u;
			uint e = (num2 >> 2) & 3u;
			int i;
			uint y;
			for (i = 0; i < num; i++)
			{
				y = v[i + 1];
				z = (v[i] += Mx(num2, y, z, i, e, k));
			}
			y = v[0];
			z = (v[num] += Mx(num2, y, z, i, e, k));
		}
		return v;
	}

	private static uint[] Decrypt(uint[] v, uint[] k)
	{
		int num = v.Length - 1;
		if (num < 1)
		{
			return v;
		}
		uint y = v[0];
		for (uint num2 = (uint)((6 + 52 / (num + 1)) * 2654435769u); num2 != 0; num2 -= 2654435769u)
		{
			uint e = (num2 >> 2) & 3u;
			int num3;
			uint z;
			for (num3 = num; num3 > 0; num3--)
			{
				z = v[num3 - 1];
				y = (v[num3] -= Mx(num2, y, z, num3, e, k));
			}
			z = v[num];
			y = (v[0] -= Mx(num2, y, z, num3, e, k));
		}
		return v;
	}

	private static byte[] FixKey(byte[] key)
	{
		if (key.Length == 16)
		{
			return key;
		}
		byte[] array = new byte[16];
		if (key.Length < 16)
		{
			key.CopyTo(array, 0);
		}
		else
		{
			Array.Copy(key, 0, array, 0, 16);
		}
		return array;
	}

	private static uint[] ToUInt32Array(IReadOnlyList<byte> data, bool includeLength)
	{
		int count = data.Count;
		int num = (((count & 3) == 0) ? (count >> 2) : ((count >> 2) + 1));
		uint[] array;
		if (includeLength)
		{
			array = new uint[num + 1];
			array[num] = (uint)count;
		}
		else
		{
			array = new uint[num];
		}
		for (int i = 0; i < count; i++)
		{
			array[i >> 2] |= (uint)(data[i] << ((i & 3) << 3));
		}
		return array;
	}

	private static byte[] ToByteArray(this IReadOnlyList<uint> data, bool includeLength = false)
	{
		int num = data.Count << 2;
		if (includeLength)
		{
			int num2 = (int)data[data.Count - 1];
			num -= 4;
			if (num2 < num - 3 || num2 > num)
			{
				return null;
			}
			num = num2;
		}
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = (byte)(data[i >> 2] >> ((i & 3) << 3));
		}
		return array;
	}

	private static byte[] MakeKey(this int key)
	{
		return new uint[4]
		{
			(uint)key,
			(uint)(key * 2),
			(uint)key / 2u,
			(uint)key % 2u
		}.ToByteArray();
	}
}
