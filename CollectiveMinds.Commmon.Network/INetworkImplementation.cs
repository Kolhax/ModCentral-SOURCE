using System;
using System.IO;
using System.Threading.Tasks;

namespace CollectiveMinds.Commmon.Network;

public interface INetworkImplementation
{
	Task<Uri> ResolveUri(Uri uri);

	Task<Stream> FetchContent(Uri uri);
}
