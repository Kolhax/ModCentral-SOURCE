using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DfuLib;

public static class Extensions
{
	public static IEnumerable<T[]> Partition<T>(this IList<T> source, int size)
	{
		for (int i = 0; (double)i < Math.Ceiling((double)source.Count / (double)size); i++)
		{
			yield return source.Skip(size * i).Take(size).ToArray();
		}
	}

	public static void Write(this BinaryWriter writer, string value, int length, char pad = '\0', bool nullTerm = true)
	{
		if (value.Length > length)
		{
			throw new ArgumentOutOfRangeException("value", "value cannot be longer than length");
		}
		if (value.Length < length)
		{
			value = value.PadRight(length, pad);
		}
		writer.Write(Encoding.ASCII.GetBytes(value));
		if (nullTerm)
		{
			writer.Write('\0');
		}
	}
}
