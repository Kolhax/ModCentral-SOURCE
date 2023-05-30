using System.Windows.Media;

namespace StrikePackCfg.Backend.Config;

public abstract class BaseConfig
{
	public readonly bool AlwaysColor;

	public readonly Brush Color;

	public readonly string Flag;

	public readonly int Id;

	public readonly bool Indent;

	public readonly string Name;

	public readonly int Options;

	public readonly bool TrueColor;

	public readonly string Help;

	public readonly string Ep;

	protected BaseConfig(int id, string name, int options, string flag, string help, string ep)
	{
		Id = id;
		Name = name;
		Options = options;
		Help = help;
		Flag = flag;
		Ep = ep;
		if (flag == null)
		{
			return;
		}
		if (flag.Contains("IND"))
		{
			Indent = true;
		}
		if (flag.Contains("ALC"))
		{
			AlwaysColor = true;
		}
		if (flag.Contains("TC"))
		{
			TrueColor = true;
		}
		if (flag.Contains("#"))
		{
			switch (flag[flag.IndexOf('#') + 1])
			{
			case 'R':
			case 'r':
				Color = Brushes.Red;
				break;
			case 'G':
			case 'g':
				Color = Brushes.Green;
				break;
			case 'B':
			case 'b':
				Color = Brushes.Blue;
				break;
			case 'P':
			case 'p':
				Color = Brushes.Purple;
				break;
			case 'O':
			case 'o':
				Color = Brushes.Orange;
				break;
			}
		}
	}
}
