using System.Text;

namespace CollectiveMinds.AppUpdate;

public static class Extensions
{
	public static string ToHexString(this byte[] ba)
	{
		StringBuilder stringBuilder = new StringBuilder(ba.Length * 2);
		foreach (byte b in ba)
		{
			stringBuilder.AppendFormat("{0:X2}", b);
		}
		return stringBuilder.ToString();
	}
}
