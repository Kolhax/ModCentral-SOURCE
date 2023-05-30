using System.Text;

namespace SPGamepack;

public static class Extensions
{
	public static string TrimUTF8(this string str, int len)
	{
		while (Encoding.UTF8.GetByteCount(str) > len)
		{
			str = str.Substring(0, len);
		}
		return str;
	}
}
