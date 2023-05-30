using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DfuLib;

[Serializable]
public class DfuStatus : ISerializable
{
	public DfuError Status;

	public int PollTimeout;

	public DfuState State;

	public byte String;

	public DfuStatus(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		if (bytes.Length != 6)
		{
			throw new ArgumentException("Size of bytes must match the status structure size", "bytes");
		}
		if (bytes[0] > 15)
		{
			throw new ArgumentOutOfRangeException("bytes", bytes[0], "");
		}
		Status = (DfuError)bytes[0];
		PollTimeout = bytes[1] | (bytes[2] << 8) | (bytes[3] << 16);
		if (bytes[4] > 11)
		{
			throw new ArgumentOutOfRangeException("bytes");
		}
		State = (DfuState)bytes[4];
		String = bytes[5];
	}

	public DfuStatus()
	{
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	protected DfuStatus(SerializationInfo info, StreamingContext context)
	{
		Status = (DfuError)info.GetInt32("Status");
		PollTimeout = info.GetInt32("PollTimeout");
		State = (DfuState)info.GetInt32("State");
		String = info.GetByte("String");
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new ArgumentNullException("info");
		}
		info.AddValue("Status", (int)Status);
		info.AddValue("PollTimeout", PollTimeout);
		info.AddValue("State", (int)State);
		info.AddValue("String", String);
	}
}
