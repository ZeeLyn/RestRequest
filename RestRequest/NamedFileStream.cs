using System.IO;

namespace RestRequest
{
	public class NamedFileStream
	{
		public string Name { get; set; }

		public string FileName { get; set; }

		public string ContentType { get; set; } = "application/octet-stream";

		public Stream Stream { get; set; }

		public NamedFileStream()
		{
		}

		public NamedFileStream(string name, string filename, Stream stream, string contentType = "application/octet-stream")
		{
			Name = name;
			FileName = filename;
			Stream = stream;
			ContentType = contentType;
		}
	}
}
