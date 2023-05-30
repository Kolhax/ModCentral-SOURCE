namespace DfuLib;

public interface IDfuFlashProgress
{
	void SetCurrentProgress(int value);

	void SetMaximumProgress(int value);
}
