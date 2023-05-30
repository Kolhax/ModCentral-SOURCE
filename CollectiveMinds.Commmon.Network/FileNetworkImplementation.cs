using System;
using System.IO;
using System.Threading.Tasks;
using CollectiveMinds.Common;

namespace CollectiveMinds.Commmon.Network;

internal class FileNetworkImplementation : INetworkImplementation
{
	private readonly string _root;

	public FileNetworkImplementation(string rootPath)
	{
		_root = rootPath;
	}

	public Task<Uri> ResolveUri(Uri uri)
	{
		return Task.FromResult(new Uri(Path.Combine(_root, uri.AbsolutePath.TrimStart('/'))));
	}

	public async Task<Stream> FetchContent(Uri uri)
	{
		return await GetFileContentsAsMemoryStream(uri.LocalPath).AsyncTrace("FetchContent", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\FileNetworkImplementation.cs", 28);
	}

	private static async Task<MemoryStream> GetFileContentsAsMemoryStream(string path)
	{
		MemoryStream ms = new MemoryStream();
		using (FileStream fs = File.OpenRead(path))
		{
			await fs.CopyToAsync(ms).AsyncTrace("GetFileContentsAsMemoryStream", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\FileNetworkImplementation.cs", 39);
		}
		ms.Seek(0L, SeekOrigin.Begin);
		return ms;
	}
}
