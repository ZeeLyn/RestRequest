using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using RestRequest.interfaces;

namespace RestRequest.Provider
{
	public class BuilderBase
	{
		internal Uri Url { get; set; }

		internal HttpMethod Method { get; set; }

		internal IBody RequestBody { get; set; }

		protected Action<HttpStatusCode, string> SuccessAction { get; set; }
		protected Action<WebException> FailAction { get; set; }

		public BuilderBase(string url, HttpMethod method)
		{
			Url = new Uri(url);
			Method = method;
		}

		public IBuilderCallback GetCallBack()
		{
			if (SuccessAction != null || FailAction != null)
				return new BuilderCallback
				{
					Success = SuccessAction,
					Fail = FailAction
				};
			return null;
		}
	}
}
