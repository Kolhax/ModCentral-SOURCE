using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CollectiveMinds.Common.DeviceUpdate;

[DataContract(Namespace = "http://collectiveminds.com/update/manifest/v1")]
public class UpdateProduct : IDeviceInterfaceGuid
{
	[DataMember(IsRequired = true)]
	public string DisplayName { get; set; }

	[DataMember(IsRequired = true)]
	public string LatestFirmwareVersion { get; set; }

	[DataMember(IsRequired = true)]
	public List<UpdateVersion> AvailableFirmwareVersions { get; set; }

	[DataMember(IsRequired = true)]
	public Guid DeviceInterfaceGuid { get; set; }
}
