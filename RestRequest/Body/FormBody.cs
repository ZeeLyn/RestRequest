using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using RestRequest.Interface;

namespace RestRequest.Body
{
	public class FormBody : IBody
	{
		private Dictionary<string, object> _parameters;
		private Stream BodyStream { get; set; }

		private string ContentType { get; set; } = "application/x-www-form-urlencoded";

		public FormBody()
		{
			_parameters = new Dictionary<string, object>();
		}

		public Stream GetBody()
		{
			if (_parameters == null || _parameters.Count == 0)
				return BodyStream;
			var query = new StringBuilder();
			foreach (var item in _parameters)
			{
				query.AppendFormat("&{0}={1}", item.Key, WebUtility.UrlEncode(item.Value.ToString()));
			}
			BodyStream = new MemoryStream();
			var bytes = Encoding.UTF8.GetBytes(query.ToString().TrimStart('&'));
			BodyStream.Write(bytes, 0, bytes.Length);
			return BodyStream;
		}


		public void AddParameter(Dictionary<string, object> parameters)
		{
			if (parameters == null || parameters.Count == 0)
				return;
			_parameters = parameters;
		}

		public void AddParameter(object parameters)
		{
			if (parameters == null)
				return;
			var properties = parameters.GetType().GetProperties();
			foreach (var enumerator in properties)
			{
				_parameters.Add(enumerator.Name, enumerator.GetValue(parameters).ToString());
			}
		}
	}
}
