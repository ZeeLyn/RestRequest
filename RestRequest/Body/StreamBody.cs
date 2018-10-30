using System.IO;
using RestRequest.Interface;

namespace RestRequest.Body
{
	public class StreamBody : IBody
	{
		private Stream BodyStream { get; }

		public StreamBody(Stream stream)
		{
			BodyStream = stream;
		}


		public byte[] GetBody()
		{
			if (BodyStream == null)
				return null;
			using (BodyStream)
			{
				var bytes = new byte[BodyStream.Length];
				BodyStream.Read(bytes, 0, bytes.Length);
				return bytes;
			}
		}
	}
}
