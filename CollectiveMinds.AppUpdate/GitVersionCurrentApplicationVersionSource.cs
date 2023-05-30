using System.Reflection;
using Semver;

namespace CollectiveMinds.AppUpdate;

public sealed class GitVersionCurrentApplicationVersionSource : ICurrentApplicationVersionSource
{
	public SemVersion GetCurrentApplicationVersion()
	{
		Assembly entryAssembly = Assembly.GetEntryAssembly();
		string text = entryAssembly.EntryPoint.DeclaringType?.Namespace ?? "";
		string arg = (string.IsNullOrEmpty(text) ? "" : ".");
		return SemVersion.Parse(entryAssembly.GetType($"{text}{arg}GitVersionInformation")?.GetField("SemVer")?.GetValue(null)?.ToString() ?? "0.0.0", strict: true);
	}
}
