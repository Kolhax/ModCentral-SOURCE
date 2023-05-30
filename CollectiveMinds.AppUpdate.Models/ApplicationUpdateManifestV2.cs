using System.Runtime.Serialization;

namespace CollectiveMinds.AppUpdate.Models;

[DataContract(Namespace = "http://collectiveminds.com/application/update/manifest/v1", Name = "ApplicationUpdateManifest")]
public class ApplicationUpdateManifestV2 : ApplicationUpdateManifest
{
	[DataMember(IsRequired = true)]
	public string LatestAvailableApplicationSha256Hash { get; set; }
}
