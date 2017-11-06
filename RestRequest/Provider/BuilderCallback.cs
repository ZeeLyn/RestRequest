using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using RestRequest.interfaces;

namespace RestRequest.Provider
{
	public class BuilderCallback : IBuilderCallback
	{
		public Action<HttpStatusCode, Stream> Success { get; set; }
		public Action<WebException> Fail { get; set; }
	}
}
