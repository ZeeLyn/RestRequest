using System.Collections.Generic;
using System.Net;
using System.Text;
using RestRequest.Interface;

namespace RestRequest.Body
{
	public class FormBody : IBody
	{
		private IDictionary<string, object> _parameters;

		public FormBody()
		{
			_parameters = new Dictionary<string, object>();
		}

		public byte[] GetBody()
		{
			if (_parameters == null || _parameters.Count == 0)
				return null;
			var query = new StringBuilder();
			foreach (var item in _parameters)
			{
				query.AppendFormat("&{0}={1}", item.Key, WebUtility.UrlEncode(item.Value.ToString()));
			}

			return Encoding.UTF8.GetBytes(query.ToString().TrimStart('&'));
		}


		public void AddParameter(IDictionary<string, object> parameters)
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
