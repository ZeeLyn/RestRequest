using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using RestRequest.interfaces;

namespace RestRequest.Provider
{
	public class BuilderBase
	{
		internal Uri Url { get; set; }

		internal HttpMethod Method { get; set; }

		internal WebHeaderCollection RequestHeaders { get; set; }

		internal IBody RequestBody { get; set; }

		protected Action<HttpStatusCode, Stream> SuccessAction { get; set; }

		protected Action<WebException> FailAction { get; set; }

		internal bool IgnoreCertificateError { get; set; }

		internal X509CertificateCollection ClientCertificates { get; set; }

		internal string UserAgent { get; set; }

		internal int Timeout { get; set; }

		internal List<Cookie> Cookies { get; set; }

		internal bool KeepAlive { get; set; }

		public BuilderBase(string url, HttpMethod method, bool keepAlive, bool ignoreCertificateError)
		{
			Url = new Uri(url);
			Method = method;
			RequestHeaders = new WebHeaderCollection();
			IgnoreCertificateError = ignoreCertificateError;
			KeepAlive = keepAlive;
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
