using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using RestRequest.Interface;

namespace RestRequest.Body
{
	public class JsonBody : IBody
	{
		private object _parameters;

		private Stream BodyStream { get; set; }

		private string ContentType { get; set; } = "application/json";
		public JsonBody()
		{
			_parameters = new Dictionary<string, object>();
		}

		public Stream GetBody()
		{
			if (_parameters == null)
				return BodyStream;
			BodyStream = new MemoryStream();
			var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_parameters));
			BodyStream.Write(bytes, 0, bytes.Length);
			return BodyStream;
		}

		public string GetContentType()
		{
			return ContentType;
		}

		public void SetContentType(string contentType)
		{
			ContentType = contentType;
		}


		public void AddParameter(object parameters)
		{
			_parameters = parameters;
		}
	}
}
