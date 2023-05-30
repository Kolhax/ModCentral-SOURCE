using Semver;

namespace CollectiveMinds.AppUpdate;

public interface ICurrentApplicationVersionSource
{
	SemVersion GetCurrentApplicationVersion();
}
