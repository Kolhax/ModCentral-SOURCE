using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CollectiveMinds.Common.DeviceUpdate;

[DataContract(Namespace = "http://collectiveminds.com/update/manifest/v1")]
public class ResetRequiredProduct : IDeviceInterfaceGuid
{
	[DataMember(IsRequired = true)]
	public string DisplayName { get; set; }

	[DataMember(IsRequired = true)]
	public List<Guid> KnownDfuModeGuidList { get; set; }

	[DataMember(IsRequired = true)]
	public int ManifestationConfirmationDelay { get; set; }

	[DataMember(IsRequired = true)]
	public Guid DeviceInterfaceGuid { get; set; }
}
