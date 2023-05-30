using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace CollectiveMinds.Common;

public class Serialization
{
	public static T Deserialize<T>(Stream stream) where T : class
	{
		using XmlReader reader = XmlReader.Create(stream);
		return (new DataContractSerializer(typeof(T)).ReadObject(reader, verifyObjectName: true) as T) ?? throw new InvalidDataException(string.Format("{0} deserialization failed", "T"));
	}

	public static byte[] Serialize<T>(T data) where T : class
	{
		using MemoryStream memoryStream = new MemoryStream();
		XmlWriterSettings settings = new XmlWriterSettings
		{
			Encoding = Encoding.UTF8,
			Indent = true
		};
		using (XmlWriter writer = XmlWriter.Create(memoryStream, settings))
		{
			new DataContractSerializer(typeof(T)).WriteObject(writer, data);
		}
		return memoryStream.ToArray();
	}
}
