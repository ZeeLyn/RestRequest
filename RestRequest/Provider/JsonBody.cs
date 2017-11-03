using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using RestRequest.interfaces;

namespace RestRequest.Provider
{
	public class JsonBody : IBody
	{
		private object _parameters;

		private Stream BodyStream { get; set; }

		private string ContentType { get; set; } = "application/json";
		public JsonBody()
		{
			_parameters = new Dictionary<string, string>();
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

		public void SetContentType(string contenttype)
		{
			ContentType = contenttype;
		}


		public void AddParameter(object parameters)
		{
			_parameters = parameters;
		}
	}
}
