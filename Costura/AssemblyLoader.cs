using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Costura;

[CompilerGenerated]
internal static class AssemblyLoader
{
	private static object nullCacheLock = new object();

	private static Dictionary<string, bool> nullCache = new Dictionary<string, bool>();

	private static Dictionary<string, string> assemblyNames = new Dictionary<string, string>();

	private static Dictionary<string, string> symbolNames = new Dictionary<string, string>();

	private static int isAttached;

	private static string CultureToString(CultureInfo culture)
	{
		if (culture == null)
		{
			return "";
		}
		return culture.Name;
	}

	private static Assembly ReadExistingAssembly(AssemblyName name)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			AssemblyName name2 = assembly.GetName();
			if (string.Equals(name2.Name, name.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(CultureToString(name2.CultureInfo), CultureToString(name.CultureInfo), StringComparison.InvariantCultureIgnoreCase))
			{
				return assembly;
			}
		}
		return null;
	}

	private static void CopyTo(Stream source, Stream destination)
	{
		byte[] array = new byte[81920];
		int count;
		while ((count = source.Read(array, 0, array.Length)) != 0)
		{
			destination.Write(array, 0, count);
		}
	}

	private static Stream LoadStream(string fullname)
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		if (fullname.EndsWith(".compressed"))
		{
			using (Stream stream = executingAssembly.GetManifestResourceStream(fullname))
			{
				using DeflateStream source = new DeflateStream(stream, CompressionMode.Decompress);
				MemoryStream memoryStream = new MemoryStream();
				CopyTo(source, memoryStream);
				memoryStream.Position = 0L;
				return memoryStream;
			}
		}
		return executingAssembly.GetManifestResourceStream(fullname);
	}

	private static Stream LoadStream(Dictionary<string, string> resourceNames, string name)
	{
		if (resourceNames.TryGetValue(name, out var value))
		{
			return LoadStream(value);
		}
		return null;
	}

	private static byte[] ReadStream(Stream stream)
	{
		byte[] array = new byte[stream.Length];
		stream.Read(array, 0, array.Length);
		return array;
	}

	private static Assembly ReadFromEmbeddedResources(Dictionary<string, string> assemblyNames, Dictionary<string, string> symbolNames, AssemblyName requestedAssemblyName)
	{
		string text = requestedAssemblyName.Name.ToLowerInvariant();
		if (requestedAssemblyName.CultureInfo != null && !string.IsNullOrEmpty(requestedAssemblyName.CultureInfo.Name))
		{
			text = $"{requestedAssemblyName.CultureInfo.Name}.{text}";
		}
		byte[] rawAssembly;
		using (Stream stream = LoadStream(assemblyNames, text))
		{
			if (stream == null)
			{
				return null;
			}
			rawAssembly = ReadStream(stream);
		}
		using (Stream stream2 = LoadStream(symbolNames, text))
		{
			if (stream2 != null)
			{
				byte[] rawSymbolStore = ReadStream(stream2);
				return Assembly.Load(rawAssembly, rawSymbolStore);
			}
		}
		return Assembly.Load(rawAssembly);
	}

	public static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
	{
		lock (nullCacheLock)
		{
			if (nullCache.ContainsKey(e.Name))
			{
				return null;
			}
		}
		AssemblyName assemblyName = new AssemblyName(e.Name);
		Assembly assembly = ReadExistingAssembly(assemblyName);
		if (assembly != null)
		{
			return assembly;
		}
		assembly = ReadFromEmbeddedResources(assemblyNames, symbolNames, assemblyName);
		if (assembly == null)
		{
			lock (nullCacheLock)
			{
				nullCache[e.Name] = true;
			}
			if ((assemblyName.Flags & AssemblyNameFlags.Retargetable) != 0)
			{
				assembly = Assembly.Load(assemblyName);
			}
		}
		return assembly;
	}

	static AssemblyLoader()
	{
		assemblyNames.Add("costura", "costura.costura.dll.compressed");
		assemblyNames.Add("de.microsoft.expression.interactions.resources", "costura.de.microsoft.expression.interactions.resources.dll.compressed");
		assemblyNames.Add("en.microsoft.expression.interactions.resources", "costura.en.microsoft.expression.interactions.resources.dll.compressed");
		assemblyNames.Add("es.microsoft.expression.interactions.resources", "costura.es.microsoft.expression.interactions.resources.dll.compressed");
		assemblyNames.Add("fr.microsoft.expression.interactions.resources", "costura.fr.microsoft.expression.interactions.resources.dll.compressed");
		assemblyNames.Add("inifileparser", "costura.inifileparser.dll.compressed");
		assemblyNames.Add("it.microsoft.expression.interactions.resources", "costura.it.microsoft.expression.interactions.resources.dll.compressed");
		assemblyNames.Add("ja.microsoft.expression.interactions.resources", "costura.ja.microsoft.expression.interactions.resources.dll.compressed");
		assemblyNames.Add("ko.microsoft.expression.interactions.resources", "costura.ko.microsoft.expression.interactions.resources.dll.compressed");
		assemblyNames.Add("mahapps.metro", "costura.mahapps.metro.dll.compressed");
		assemblyNames.Add("mahapps.metro.iconpacks.fontawesome", "costura.mahapps.metro.iconpacks.fontawesome.dll.compressed");
		symbolNames.Add("mahapps.metro.iconpacks.fontawesome", "costura.mahapps.metro.iconpacks.fontawesome.pdb.compressed");
		assemblyNames.Add("mahapps.metro.iconpacks.modern", "costura.mahapps.metro.iconpacks.modern.dll.compressed");
		symbolNames.Add("mahapps.metro.iconpacks.modern", "costura.mahapps.metro.iconpacks.modern.pdb.compressed");
		symbolNames.Add("mahapps.metro", "costura.mahapps.metro.pdb.compressed");
		assemblyNames.Add("microsoft.ai.servertelemetrychannel", "costura.microsoft.ai.servertelemetrychannel.dll.compressed");
		assemblyNames.Add("microsoft.applicationinsights", "costura.microsoft.applicationinsights.dll.compressed");
		assemblyNames.Add("microsoft.expression.interactions", "costura.microsoft.expression.interactions.dll.compressed");
		assemblyNames.Add("propertychanged", "costura.propertychanged.dll.compressed");
		assemblyNames.Add("ru.microsoft.expression.interactions.resources", "costura.ru.microsoft.expression.interactions.resources.dll.compressed");
		assemblyNames.Add("system.windows.interactivity", "costura.system.windows.interactivity.dll.compressed");
		assemblyNames.Add("zh-Hans.microsoft.expression.interactions.resources", "costura.zh-Hans.microsoft.expression.interactions.resources.dll.compressed");
		assemblyNames.Add("zh-Hant.microsoft.expression.interactions.resources", "costura.zh-Hant.microsoft.expression.interactions.resources.dll.compressed");
	}

	public static void Attach()
	{
		if (Interlocked.Exchange(ref isAttached, 1) == 1)
		{
			return;
		}
		AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs e)
		{
			lock (nullCacheLock)
			{
				if (nullCache.ContainsKey(e.Name))
				{
					return null;
				}
			}
			AssemblyName assemblyName = new AssemblyName(e.Name);
			Assembly assembly = ReadExistingAssembly(assemblyName);
			if (assembly != null)
			{
				return assembly;
			}
			assembly = ReadFromEmbeddedResources(assemblyNames, symbolNames, assemblyName);
			if (assembly == null)
			{
				lock (nullCacheLock)
				{
					nullCache[e.Name] = true;
				}
				if ((assemblyName.Flags & AssemblyNameFlags.Retargetable) != 0)
				{
					assembly = Assembly.Load(assemblyName);
				}
			}
			return assembly;
		};
	}
}
