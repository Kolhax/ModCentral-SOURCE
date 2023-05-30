namespace DfuLib;

public enum DfuState : byte
{
	AppIdle = 0,
	AppDetach = 1,
	Idle = 2,
	DownloadSync = 3,
	DownloadBusy = 4,
	DownloadIdle = 5,
	ManifestSync = 6,
	Manifest = 7,
	ManifestWaitReset = 8,
	UploadIdle = 9,
	Error = 10,
	Locked = 11,
	UploadSync = 145,
	UploadBusy = 146
}
