using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DfuLib;

[Serializable]
public class DfuException : Exception
{
	public DfuError ErrorCode;

	public DfuStatus Status;

	public DfuState ExpectedState;

	public DfuException()
	{
	}

	public DfuException(string message)
		: base(message)
	{
	}

	public DfuException(string message, Exception inner)
		: base(message, inner)
	{
	}

	public DfuException(DfuError error, string message)
		: base(message)
	{
		ErrorCode = error;
		Data.Add("ErrorCode", ErrorCode);
	}

	public DfuException(DfuStatus status, DfuState expected, string message)
		: base(message)
	{
		Status = status;
		ExpectedState = expected;
		Data.Add("Status", Status);
		Data.Add("ExpectedState", ExpectedState);
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	protected DfuException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		ErrorCode = (DfuError)info.GetInt32("ErrorCode");
		ExpectedState = (DfuState)info.GetInt32("ExpectedState");
		Status = (DfuStatus)info.GetValue("Status", typeof(DfuStatus));
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		info.AddValue("ErrorCode", (int)ErrorCode);
		info.AddValue("ExpectedState", (int)ExpectedState);
		info.AddValue("Status", Status, typeof(DfuStatus));
		base.GetObjectData(info, context);
	}

	public static void ThrowOnError(DfuError error, string message)
	{
		if (error != 0)
		{
			throw new DfuException(error, message);
		}
	}

	public static void ThrowOnStatus(DfuStatus status, DfuState expected, string message)
	{
		if (status.State != expected)
		{
			throw new DfuException(status, expected, $"{status.State} != {expected}, err={status.Status}: {message}");
		}
	}
}
