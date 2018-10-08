using System;
using System.Net;

namespace RestRequest.Builder
{
	internal partial class RequestBuilder : IDisposable
	{
		internal HttpWebRequest Request { get; private set; }

		private ContextBuilder Context { get; }

		internal RequestBuilder(ContextBuilder context)
		{
			Context = context;
		}

		internal void BuildRequest()
		{
			Request = (HttpWebRequest)WebRequest.Create(Context.Url);
			Request.Method = Context.Method.ToString().ToUpper();
			if (Context.RequestHeaders != null && Context.RequestHeaders.Count > 0)
				Request.Headers = Context.RequestHeaders;
			Request.ContentType = Context._ContentType;
			Request.KeepAlive = Context._KeepAlive;
			Request.Referer = Context._Referer;
			Request.ServicePoint.ConnectionLimit = Context._ConnectionLimit;
			if (Context.IgnoreCertificateError)
				Request.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
			if (Context.ClientCertificates != null && Context.ClientCertificates.Count > 0)
				Request.ClientCertificates.AddRange(Context.ClientCertificates);
			if (!string.IsNullOrWhiteSpace(Context._UserAgent))
				Request.UserAgent = Context._UserAgent;
			if (Context._Timeout > 0)
				Request.Timeout = Context._Timeout;
			if (Context._Cookies?.Count > 0)
			{
				Request.CookieContainer = new CookieContainer();
				foreach (var cookie in Context._Cookies)
				{
					Request.CookieContainer.Add(cookie);
				}
			}
		}

		internal void WriteRequestBody()
		{
			var bodyBytes = Context.RequestBody?.GetBody();
			if (bodyBytes == null)
				return;
			using (var requestStream = Request.GetRequestStream())
			{
				Request.ContentLength = bodyBytes.Length;
				requestStream.Write(bodyBytes, 0, bodyBytes.Length);
			}
		}



		internal HttpWebResponse GetResponse()
		{
			try
			{
				return (HttpWebResponse)Request.GetResponse();
			}
			catch (WebException e)
			{
				if (e.Response is HttpWebResponse response)
					return response;
				throw;
			}
		}

		public void Dispose()
		{
			Request?.Abort();
		}
	}
}
