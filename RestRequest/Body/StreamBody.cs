using System.IO;
using RestRequest.Interface;

namespace RestRequest.Body
{
	internal class StreamBody : IBody
	{
		private Stream BodyStream { get; set; }

		internal StreamBody(Stream stream)
		{
			BodyStream = stream;
		}


		public Stream GetBody()
		{
			return BodyStream;
		}
	}
}
