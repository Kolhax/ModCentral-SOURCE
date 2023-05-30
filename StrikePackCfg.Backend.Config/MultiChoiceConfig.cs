using System.Collections.Generic;

namespace StrikePackCfg.Backend.Config;

public class MultiChoiceConfig : BaseConfig
{
	public readonly KeyValuePair<int, string>[] OptionsList;

	public MultiChoiceConfig(int id, string name, int options, string flag, string help, string ep, KeyValuePair<int, string>[] optlist)
		: base(id, name, options, flag, help, ep)
	{
		OptionsList = optlist;
	}
}
