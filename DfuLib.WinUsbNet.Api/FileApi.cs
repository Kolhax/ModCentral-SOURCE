using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace DfuLib.WinUsbNet.Api;

internal sealed class FileApi
{
	public const int FILE_ATTRIBUTE_NORMAL = 128;

	public const int FILE_FLAG_OVERLAPPED = 1073741824;

	public const int FILE_SHARE_READ = 1;

	public const int FILE_SHARE_WRITE = 2;

	public const uint GENERIC_READ = 2147483648u;

	public const uint GENERIC_WRITE = 1073741824u;

	public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

	public const int OPEN_EXISTING = 3;

	public const int ERROR_IO_PENDING = 997;

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	public static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);
}
