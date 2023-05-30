using System;
using System.IO;
using System.Threading.Tasks;
using CollectiveMinds.Common;
using CollectiveMinds.Common.Azure;

namespace CollectiveMinds.Commmon.Network;

public static class NetworkHelper
{
	private static readonly INetworkImplementation _defaultImpl = new HttpNetworkImplementation();

	private static INetworkImplementation _impl = _defaultImpl;

	internal static void SetNetworkImplementation(INetworkImplementation impl)
	{
		_impl = impl;
	}

	public static Task<T> FetchManifest<T>(string url, bool useDefault = false) where T : class
	{
		return FetchManifest<T>(new Uri(url), useDefault).AsyncTrace("FetchManifest", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 21);
	}

	public static async Task<T> FetchManifest<T>(Uri uri, bool useDefault = false) where T : class
	{
		INetworkImplementation impl = _impl;
		if (useDefault)
		{
			impl = _defaultImpl;
		}
		uri = await impl.ResolveUri(uri);
		using TrackDependencyScope tds = TrackDependencyScope.FromUri(uri);
		TrackDependencyScope trackDependencyScope = tds;
		T ret = Serialization.Deserialize<T>(await impl.FetchContent(uri).AsyncTrace("FetchManifest", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 37));
		return trackDependencyScope.Success(ret);
	}

	public static Task<Stream> FetchStream(string url, bool useDefault = false)
	{
		return FetchStream(new Uri(url), useDefault).AsyncTrace("FetchStream", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 43);
	}

	public static async Task<Stream> FetchStream(Uri uri, bool useDefault = false)
	{
		INetworkImplementation impl = _impl;
		if (useDefault)
		{
			impl = _defaultImpl;
		}
		uri = await impl.ResolveUri(uri).AsyncTrace("FetchStream", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 54);
		using TrackDependencyScope tds = TrackDependencyScope.FromUri(uri);
		TrackDependencyScope trackDependencyScope = tds;
		return trackDependencyScope.Success(await impl.FetchContent(uri).AsyncTrace("FetchStream", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 59));
	}

	public static Task<byte[]> FetchBytes(string url, bool useDefault = false)
	{
		return FetchBytes(new Uri(url), useDefault).AsyncTrace("FetchBytes", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 65);
	}

	public static async Task<byte[]> FetchBytes(Uri uri, bool useDefault = false)
	{
		INetworkImplementation impl = _impl;
		if (useDefault)
		{
			impl = _defaultImpl;
		}
		uri = await impl.ResolveUri(uri).AsyncTrace("FetchBytes", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 76);
		using TrackDependencyScope tds = TrackDependencyScope.FromUri(uri);
		using Stream stream = await impl.FetchContent(uri).AsyncTrace("FetchBytes", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 81);
		byte[] array = new byte[stream.Length];
		int num = stream.Read(array, 0, array.Length);
		if (num != array.Length)
		{
			throw new IOException($"Failed to read 0x{array:X} bytes from the stream, actually read 0x{num:X} bytes");
		}
		return tds.Success(array);
	}

	public static Task<string> FetchString(string url, bool useDefault = false)
	{
		return FetchString(new Uri(url), useDefault).AsyncTrace("FetchString", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 100);
	}

	public static async Task<string> FetchString(Uri uri, bool useDefault = false)
	{
		INetworkImplementation impl = _impl;
		if (useDefault)
		{
			impl = _defaultImpl;
		}
		uri = await impl.ResolveUri(uri).AsyncTrace("FetchString", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 111);
		using TrackDependencyScope tds = TrackDependencyScope.FromUri(uri);
		using Stream stream = await impl.FetchContent(uri).AsyncTrace("FetchString", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 116);
		using StreamReader reader = new StreamReader(stream);
		TrackDependencyScope trackDependencyScope = tds;
		return trackDependencyScope.Success(await reader.ReadToEndAsync().AsyncTrace("FetchString", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\NetworkHelper.cs", 118));
	}
}
