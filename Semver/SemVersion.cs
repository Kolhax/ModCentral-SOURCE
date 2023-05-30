using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Semver;

[Serializable]
public sealed class SemVersion : IComparable<SemVersion>, IComparable, ISerializable
{
	private static Regex parseEx = new Regex("^(?<major>\\d+)(\\.(?<minor>\\d+))?(\\.(?<patch>\\d+))?(\\-(?<pre>[0-9A-Za-z\\-\\.]+))?(\\+(?<build>[0-9A-Za-z\\-\\.]+))?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

	public int Major { get; private set; }

	public int Minor { get; private set; }

	public int Patch { get; private set; }

	public string Prerelease { get; private set; }

	public string Build { get; private set; }

	private SemVersion(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		SemVersion semVersion = Parse(info.GetString("SemVersion"));
		Major = semVersion.Major;
		Minor = semVersion.Minor;
		Patch = semVersion.Patch;
		Prerelease = semVersion.Prerelease;
		Build = semVersion.Build;
	}

	public SemVersion(int major, int minor = 0, int patch = 0, string prerelease = "", string build = "")
	{
		Major = major;
		Minor = minor;
		Patch = patch;
		Prerelease = prerelease ?? "";
		Build = build ?? "";
	}

	public SemVersion(Version version)
	{
		if (version == null)
		{
			throw new ArgumentNullException("version");
		}
		Major = version.Major;
		Minor = version.Minor;
		if (version.Revision >= 0)
		{
			Patch = version.Revision;
		}
		Prerelease = string.Empty;
		if (version.Build > 0)
		{
			Build = version.Build.ToString();
		}
		else
		{
			Build = string.Empty;
		}
	}

	public static SemVersion Parse(string version, bool strict = false)
	{
		Match match = parseEx.Match(version);
		if (!match.Success)
		{
			throw new ArgumentException("Invalid version.", "version");
		}
		int major = int.Parse(match.Groups["major"].Value, CultureInfo.InvariantCulture);
		Group group = match.Groups["minor"];
		int minor = 0;
		if (group.Success)
		{
			minor = int.Parse(group.Value, CultureInfo.InvariantCulture);
		}
		else if (strict)
		{
			throw new InvalidOperationException("Invalid version (no minor version given in strict mode)");
		}
		Group group2 = match.Groups["patch"];
		int patch = 0;
		if (group2.Success)
		{
			patch = int.Parse(group2.Value, CultureInfo.InvariantCulture);
		}
		else if (strict)
		{
			throw new InvalidOperationException("Invalid version (no patch version given in strict mode)");
		}
		string value = match.Groups["pre"].Value;
		string value2 = match.Groups["build"].Value;
		return new SemVersion(major, minor, patch, value, value2);
	}

	public static bool TryParse(string version, out SemVersion semver, bool strict = false)
	{
		try
		{
			semver = Parse(version, strict);
			return true;
		}
		catch (Exception)
		{
			semver = null;
			return false;
		}
	}

	public static bool Equals(SemVersion versionA, SemVersion versionB)
	{
		return versionA?.Equals(versionB) ?? ((object)versionB == null);
	}

	public static int Compare(SemVersion versionA, SemVersion versionB)
	{
		if ((object)versionA == null)
		{
			if ((object)versionB != null)
			{
				return -1;
			}
			return 0;
		}
		return versionA.CompareTo(versionB);
	}

	public SemVersion Change(int? major = null, int? minor = null, int? patch = null, string prerelease = null, string build = null)
	{
		return new SemVersion(major ?? Major, minor ?? Minor, patch ?? Patch, prerelease ?? Prerelease, build ?? Build);
	}

	public override string ToString()
	{
		string text = Major + "." + Minor + "." + Patch;
		if (!string.IsNullOrEmpty(Prerelease))
		{
			text = text + "-" + Prerelease;
		}
		if (!string.IsNullOrEmpty(Build))
		{
			text = text + "+" + Build;
		}
		return text;
	}

	public int CompareTo(object obj)
	{
		return CompareTo((SemVersion)obj);
	}

	public int CompareTo(SemVersion other)
	{
		if ((object)other == null)
		{
			return 1;
		}
		int num = CompareByPrecedence(other);
		if (num != 0)
		{
			return num;
		}
		return CompareComponent(Build, other.Build);
	}

	public bool PrecedenceMatches(SemVersion other)
	{
		return CompareByPrecedence(other) == 0;
	}

	public int CompareByPrecedence(SemVersion other)
	{
		if ((object)other == null)
		{
			return 1;
		}
		int num = Major.CompareTo(other.Major);
		if (num != 0)
		{
			return num;
		}
		num = Minor.CompareTo(other.Minor);
		if (num != 0)
		{
			return num;
		}
		num = Patch.CompareTo(other.Patch);
		if (num != 0)
		{
			return num;
		}
		return CompareComponent(Prerelease, other.Prerelease, lower: true);
	}

	private static int CompareComponent(string a, string b, bool lower = false)
	{
		bool flag = string.IsNullOrEmpty(a);
		bool flag2 = string.IsNullOrEmpty(b);
		if (flag && flag2)
		{
			return 0;
		}
		if (flag)
		{
			if (!lower)
			{
				return -1;
			}
			return 1;
		}
		if (flag2)
		{
			if (!lower)
			{
				return 1;
			}
			return -1;
		}
		string[] array = a.Split('.');
		string[] array2 = b.Split('.');
		int num = Math.Min(array.Length, array2.Length);
		for (int i = 0; i < num; i++)
		{
			string text = array[i];
			string text2 = array2[i];
			int result;
			bool flag3 = int.TryParse(text, out result);
			int result2;
			bool flag4 = int.TryParse(text2, out result2);
			if (flag3 && flag4)
			{
				if (result.CompareTo(result2) != 0)
				{
					return result.CompareTo(result2);
				}
				continue;
			}
			if (flag3)
			{
				return -1;
			}
			if (flag4)
			{
				return 1;
			}
			int num2 = string.CompareOrdinal(text, text2);
			if (num2 != 0)
			{
				return num2;
			}
		}
		return array.Length.CompareTo(array2.Length);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		SemVersion semVersion = (SemVersion)obj;
		if (Major == semVersion.Major && Minor == semVersion.Minor && Patch == semVersion.Patch && string.Equals(Prerelease, semVersion.Prerelease, StringComparison.Ordinal))
		{
			return string.Equals(Build, semVersion.Build, StringComparison.Ordinal);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((Major.GetHashCode() * 31 + Minor.GetHashCode()) * 31 + Patch.GetHashCode()) * 31 + Prerelease.GetHashCode()) * 31 + Build.GetHashCode();
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		info.AddValue("SemVersion", ToString());
	}

	public static implicit operator SemVersion(string version)
	{
		return Parse(version);
	}

	public static bool operator ==(SemVersion left, SemVersion right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(SemVersion left, SemVersion right)
	{
		return !Equals(left, right);
	}

	public static bool operator >(SemVersion left, SemVersion right)
	{
		return Compare(left, right) > 0;
	}

	public static bool operator >=(SemVersion left, SemVersion right)
	{
		if (!(left == right))
		{
			return left > right;
		}
		return true;
	}

	public static bool operator <(SemVersion left, SemVersion right)
	{
		return Compare(left, right) < 0;
	}

	public static bool operator <=(SemVersion left, SemVersion right)
	{
		if (!(left == right))
		{
			return left < right;
		}
		return true;
	}
}
