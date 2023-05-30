using System;

namespace StrikePackCfg.HelperClasses;

internal class DisableUIContext : IDisposable
{
	public DisableUIContext()
	{
		App.MainWin.SetState(state: false);
	}

	public void Dispose()
	{
		App.MainWin.SetState(state: true);
	}
}
