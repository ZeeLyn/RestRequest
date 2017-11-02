using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RestRequest
{
	public class NamedFileStream
	{
		public string Name { get; set; }

		public string FileName { get; set; }

		public string ContentType { get; set; }

		public Stream Stream { get; set; }

		public NamedFileStream()
		{
		}

		public NamedFileStream(string name, string filename, Stream stream, string contenttype = "application/octet-stream")
		{
			Name = name;
			FileName = filename;
			Stream = stream;
			ContentType = contenttype;
		}
	}
}
