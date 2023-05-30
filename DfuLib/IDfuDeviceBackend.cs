using System;

namespace DfuLib;

public interface IDfuDeviceBackend : IDisposable
{
	void Detach();

	void Download(ushort block, byte[] data);

	byte[] Upload(ushort block, uint length);

	DfuStatus GetStatus();

	void ClearStatus();

	DfuState GetState();

	void Abort();

	void Close();
}
