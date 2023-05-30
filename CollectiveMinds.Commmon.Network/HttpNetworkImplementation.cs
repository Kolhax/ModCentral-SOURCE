using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CollectiveMinds.Common;

namespace CollectiveMinds.Commmon.Network;

internal class HttpNetworkImplementation : INetworkImplementation
{
	private static readonly HttpClient Client = new HttpClient();

	public Task<Uri> ResolveUri(Uri uri)
	{
		return Task.FromResult(uri);
	}

	public async Task<Stream> FetchContent(Uri uri)
	{
		return await RetryHelper.RetryWithExponentialBackoff(async delegate
		{
			HttpResponseMessage obj = await Client.GetAsync(uri).AsyncTrace("FetchContent", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\HttpNetworkImplementation.cs", 23).ConfigureAwait(continueOnCapturedContext: false);
			obj.EnsureSuccessStatusCode();
			return await obj.Content.ReadAsStreamAsync().AsyncTrace("FetchContent", "C:\\Users\\Antonio.Mazzanti\\Documents\\GitHub\\ModCentral\\Libraries\\CollectiveMinds.Common\\CollectiveMinds.Common\\Network\\HttpNetworkImplementation.cs", 29);
		}, 3, 500).ConfigureAwait(continueOnCapturedContext: false);
	}
}
