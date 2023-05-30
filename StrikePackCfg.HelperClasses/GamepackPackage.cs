using System;
using System.Collections.Generic;
using Semver;
using SPGamepack;
using StrikePackCfg.Backend.Config;

namespace StrikePackCfg.HelperClasses;

public class GamepackPackage
{
	public readonly byte[] Bytecode;

	public readonly SemVersion CfgVersion;

	public readonly IEnumerable<BaseConfig> Config;

	public readonly ushort ID;

	public readonly bool Deprecated;

	public readonly bool IsEncrypted;

	public readonly string Name;

	public readonly SemVersion Version;

	public GamepackPackage(GamepackBlob blob)
	{
		Bytecode = blob.Bytecode;
		ID = blob.GamepackID;
		Name = blob.Name;
		Version = blob.Version;
		IsEncrypted = blob.EncryptionMethod != GamepackBlob.EncryptionMethods.None;
		Deprecated = blob.Deprecated;
		if (blob.CFGData.Length > 0)
		{
			Tuple<IEnumerable<BaseConfig>, SemVersion> tuple = ConfigHelper.ParseIniConfig(blob.CFGData, Name);
			Config = tuple.Item1;
			CfgVersion = tuple.Item2;
		}
	}
}
