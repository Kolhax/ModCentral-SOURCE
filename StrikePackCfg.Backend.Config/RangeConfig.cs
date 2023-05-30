namespace StrikePackCfg.Backend.Config;

public class RangeConfig : BaseConfig
{
	public readonly int DefaultValue;

	public readonly int MaximumValue;

	public readonly int MinimumValue;

	public readonly int Step;

	public RangeConfig(int id, string name, int options, string flag, string help, string ep, int defaultValue, int minimumValue, int maximumValue, int step)
		: base(id, name, options, flag, help, ep)
	{
		DefaultValue = defaultValue;
		MinimumValue = minimumValue;
		MaximumValue = maximumValue;
		Step = step;
	}

	public RangeConfig(RangeConfig cfg, string name)
		: this(cfg.Id, name, cfg.Options, cfg.Flag, cfg.Help, cfg.Ep, cfg.DefaultValue, cfg.MinimumValue, cfg.MaximumValue, cfg.Step)
	{
	}
}
