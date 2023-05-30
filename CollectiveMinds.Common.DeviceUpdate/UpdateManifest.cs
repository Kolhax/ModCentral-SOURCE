using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CollectiveMinds.Common.DeviceUpdate;

[DataContract(Namespace = "http://collectiveminds.com/update/manifest/v1")]
public class UpdateManifest
{
	[DataMember(IsRequired = true)]
	public List<UpdateProduct> SupportedProducts { get; set; }

	[DataMember]
	public List<ResetRequiredProduct> SupportedResetRequiredProducts { get; set; }

	[DataMember]
	public string ApplicationUpdateManifestUrl { get; set; }
}
