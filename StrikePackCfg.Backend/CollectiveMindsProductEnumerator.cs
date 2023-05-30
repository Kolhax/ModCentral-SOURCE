using System.Collections.Generic;
using System.Windows.Controls;
using DfuLib;

namespace StrikePackCfg.Backend;

public class CollectiveMindsProductEnumerator : BaseDeviceEnumerator<CollectiveMindsProduct, IDeviceState>
{
	public CollectiveMindsProductEnumerator(Control window, ICollection<IDeviceState> devices)
		: base(window, devices)
	{
	}

	protected override IDeviceState HandleDeviceEnumeration(CollectiveMindsProduct item, string devicePath)
	{
		GeneralDeviceState generalDeviceState = new GeneralDeviceState(new CollectiveMindsDevice(devicePath)
		{
			Product = item
		});
		using CmPpDfuDeviceBackend cmPpDfuDeviceBackend = new CmPpDfuDeviceBackend(devicePath);
		generalDeviceState.Device.SerialNumber = cmPpDfuDeviceBackend.GetSerialNumber();
		generalDeviceState.Device.ApplicationVersion = cmPpDfuDeviceBackend.GetApplicationVersion();
		return generalDeviceState;
	}
}
