using System.IO;
using RestRequest.Interface;

namespace RestRequest.Body
{
	internal class StreamBody : IBody
	{
		private Stream BodyStream { get; set; }

		private string ContentType { get; set; } = "application/octet-stream";

		internal StreamBody(Stream stream)
		{
			BodyStream = stream;
		}

		public void SetContentType(string contentType)
		{
			ContentType = contentType;
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
