using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DfuLib.WinUsbNet.Api;

internal class ApiException : Exception
{
	public ApiException(string message)
		: base(message)
	{
	}

	public ApiException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public static ApiException Win32(string message)
	{
		return Win32(message, Marshal.GetLastWin32Error());
	}

	public static ApiException Win32(string message, int errorCode)
	{
		return new ApiException(message, new Win32Exception(errorCode));
	}
}
