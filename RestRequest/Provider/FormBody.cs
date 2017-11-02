using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using RestRequest.interfaces;

namespace RestRequest.Provider
{
	public class FormBody : IBody
	{
		private Dictionary<string, string> _parameters;
		private Stream BodyStream { get; set; }

		private string ContentType { get; set; } = "application/x-www-form-urlencoded";

		public FormBody()
		{
			_parameters = new Dictionary<string, string>();
		}

		public Stream GetBody()
		{
			var query = new StringBuilder();
			foreach (var item in _parameters)
			{
				query.AppendFormat("&{0}={1}", item.Key, WebUtility.UrlEncode(item.Value));
			}
			BodyStream = new MemoryStream();
			var bytes = Encoding.UTF8.GetBytes(query.ToString().TrimStart('&'));
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

		public void AddParameter(Dictionary<string, string> parameters)
		{
			_parameters = parameters;
		}

		public void AddParameter(object parameters)
		{
			var properties = parameters.GetType().GetProperties();
			foreach (var enumerator in properties)
			{
				_parameters.Add(enumerator.Name, enumerator.GetValue(parameters).ToString());
			}
		}
	}
}
