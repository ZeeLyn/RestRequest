using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RestRequest.Interface;

namespace RestRequest.Body
{
	public class JsonBody : IBody
	{
		private object _parameters;
		public byte[] GetBody()
		{
			if (_parameters == null)
				return null;
			return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_parameters));
		}

		public void AddParameter(object parameters)
		{
			_parameters = parameters;
		}
	}
}
