using System.Windows.Controls;

namespace StrikePackCfg.Views;

public abstract class ConfigViewBase : UserControl
{
	public abstract short GetValue();

	public abstract int GetId();

	public abstract void SetValue(int value);
}
