using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Semver;

namespace StrikePackCfg.HelperClasses;

public static class Extensions
{
	public static T CreateInstance<T>(params object[] args)
	{
		Type typeFromHandle = typeof(T);
		return (T)typeFromHandle.Assembly.CreateInstance(typeFromHandle.FullName, ignoreCase: false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, args, null, null);
	}

	public static object CreateInternalInstance<TInSameAssembly>(string fullClassName, params object[] args)
	{
		Type type = typeof(TInSameAssembly).Assembly.GetType(fullClassName);
		return type.Assembly.CreateInstance(type.FullName, ignoreCase: false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, args, null, null);
	}

	public static void DoWithReadLock(this ReaderWriterLockSlim _lock, Action action)
	{
		_lock.EnterReadLock();
		try
		{
			action();
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public static T DoWithReadLock<T>(this ReaderWriterLockSlim _lock, Func<T> action)
	{
		_lock.EnterReadLock();
		try
		{
			return action();
		}
		finally
		{
			_lock.ExitReadLock();
		}
	}

	public static void DoWithWriteLock(this ReaderWriterLockSlim _lock, Action action)
	{
		_lock.EnterWriteLock();
		try
		{
			action();
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}

	public static T DoWithWriteLock<T>(this ReaderWriterLockSlim _lock, Func<T> action)
	{
		_lock.EnterWriteLock();
		try
		{
			return action();
		}
		finally
		{
			_lock.ExitWriteLock();
		}
	}

	public static SemVersion ReadSemVersion(this BinaryReader br)
	{
		int count = br.ReadInt32();
		return Encoding.UTF8.GetString(br.ReadBytes(count));
	}

	public static void Write(this BinaryWriter bw, SemVersion ver)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(ver.ToString());
		bw.Write(bytes.Length);
		bw.Write(bytes);
	}

	public static IEnumerable<KeyValuePair<int, short>> ToDictionary(this short[] data)
	{
		Dictionary<int, short> dictionary = new Dictionary<int, short>();
		for (int i = 0; i < data.Length; i++)
		{
			dictionary[i] = data[i];
		}
		return dictionary;
	}
}
