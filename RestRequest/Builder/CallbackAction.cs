using System;
using System.IO;
using System.Net;
using RestRequest.Interface;

namespace RestRequest.Builder
{
	public class CallbackAction : CallbackIBuilder
	{
		public Action<HttpStatusCode, Stream> Success { get; set; }
		public Action<WebException> Fail { get; set; }
	}
}
