using System.Runtime.Serialization;
using Semver;

namespace CollectiveMinds.AppUpdate.Models;

[DataContract(Namespace = "http://collectiveminds.com/application/update/manifest/v1")]
public class ApplicationUpdateManifest
{
	[DataMember]
	public SemVersion MinimumRequiredApplicationVersion { get; set; }

	[DataMember(IsRequired = true)]
	public SemVersion LatestAvailableApplicationVersion { get; set; }

	[DataMember(IsRequired = true)]
	public string LatestAvailableApplicationUrl { get; set; }
}
