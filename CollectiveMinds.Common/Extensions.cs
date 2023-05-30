using System;
using System.Reflection;

namespace CollectiveMinds.Common;

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
}
