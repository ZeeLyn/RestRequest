using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RestRequest.interfaces;

namespace RestRequest.Provider
{
	internal class StreamBody : IBody
	{
		private Stream BodyStream { get; set; }

		private string ContentType { get; set; } = "application/octet-stream";

		internal StreamBody(Stream stream)
		{
			BodyStream = stream;
		}

		public void SetContentType(string contenttype)
		{
			ContentType = contenttype;
		}

		public string GetContentType()
		{
			return ContentType;
		}

		public Stream GetBody()
		{
			return BodyStream;
		}
	}
}
