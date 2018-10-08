using System.IO;
using RestRequest.Interface;

namespace RestRequest.Body
{
	internal class StreamBody : IBody
	{
		private Stream BodyStream { get; }

		internal StreamBody(Stream stream)
		{
			BodyStream = stream;
		}


		public byte[] GetBody()
		{
			using (BodyStream)
			{
				var bytes = new byte[BodyStream.Length];
				BodyStream.Read(bytes, 0, bytes.Length);
				return bytes;
			}
		}
	}
}
