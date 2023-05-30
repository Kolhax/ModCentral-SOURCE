using System.Runtime.Serialization;

namespace CollectiveMinds.Common.DeviceUpdate;

[DataContract(Namespace = "http://collectiveminds.com/update/manifest/v1")]
public class UpdateVersion
{
	[DataMember(IsRequired = true)]
	public string FirmwareVersion { get; set; }

	[DataMember(IsRequired = true)]
	public string FirmwareUrl { get; set; }
}
