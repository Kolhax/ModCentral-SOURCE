using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Semver;
using StrikePackCfg.Backend.Config;
using StrikePackCfg.HelperClasses;

namespace StrikePackCfg.Gamepacks;

[DataContract(Namespace = "http://collectiveminds.com/gamepack/manifest/v1")]
public class GamepackData
{
	[IgnoreDataMember]
	public static readonly GamepackData Empty = new GamepackData
	{
		Platform = "",
		ReleaseDate = "",
		Description = "",
		Id = 0,
		IdSub = 0,
		Version = "1.0.0",
		Name = "",
		ModPack = false,
		GpackSize = 0
	};

	[IgnoreDataMember]
	private IEnumerable<BaseConfig> configData;

	[IgnoreDataMember]
	private SemVersion CfgVersion;

	[DataMember(Name = "gpName", Order = 3)]
	public string Name { get; set; }

	[DataMember(Name = "gpVersion", Order = 7)]
	public string VersionStorage { get; set; }

	[DataMember(Name = "gpSize", Order = 8)]
	public int GpackSize { get; set; }

	[DataMember(Name = "gpOnlineHelp", Order = 9)]
	public string GpOnlineHelp { get; set; }

	[IgnoreDataMember]
	public SemVersion Version
	{
		get
		{
			return VersionStorage;
		}
		set
		{
			VersionStorage = value.Major + "." + value.Minor;
		}
	}

	[DataMember(Name = "idGamepack", Order = 0)]
	public int Id { get; set; }

	[DataMember(Name = "gpDescription", Order = 2)]
	public string Description { get; set; }

	[DataMember(Name = "idGamepackVersion", Order = 1)]
	public int IdSub { get; set; }

	[DataMember(Name = "gpReleaseDate", Order = 6)]
	public string ReleaseDate { get; set; }

	[DataMember(Name = "gpPlatform", Order = 4)]
	public string Platform { get; set; }

	[DataMember(Name = "gpIsModpack", Order = 5)]
	public bool ModPack { get; set; }

	public override string ToString()
	{
		if (!string.IsNullOrWhiteSpace(Name))
		{
			return $"{Name.Trim()} (v{Version})";
		}
		return "";
	}

	public async Task<Tuple<IEnumerable<BaseConfig>, int, SemVersion>> FetchConfig()
	{
		Tuple<IEnumerable<BaseConfig>, SemVersion> tuple = await ConfigHelper.FetchConfig(Id, Name);
		configData = tuple.Item1;
		CfgVersion = tuple.Item2;
		return new Tuple<IEnumerable<BaseConfig>, int, SemVersion>(configData, Id, CfgVersion);
	}
}
