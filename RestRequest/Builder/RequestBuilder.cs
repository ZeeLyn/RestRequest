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
			Request = (HttpWebRequest)WebRequest.Create(Context._url);
			Request.Method = Context._method.ToString().ToUpper();
			if (Context._requestHeaders != null && Context._requestHeaders.Count > 0)
				Request.Headers = Context._requestHeaders;
			Request.ContentType = Context._contentType;
			Request.KeepAlive = Context._keepAlive;
			Request.Referer = Context._referer;
			Request.ServicePoint.ConnectionLimit = Context._connectionLimit;
			if (Context._ignoreCertificateError)
				Request.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
			if (Context._clientCertificates != null && Context._clientCertificates.Count > 0)
				Request.ClientCertificates.AddRange(Context._clientCertificates);
			if (!string.IsNullOrWhiteSpace(Context._userAgent))
				Request.UserAgent = Context._userAgent;
			if (Context._timeout > 0)
				Request.Timeout = Context._timeout;
			if (Context._cookies?.Count > 0)
			{
				Request.CookieContainer = new CookieContainer();
				foreach (var cookie in Context._cookies)
				{
					Request.CookieContainer.Add(cookie);
				}
			}
		}

		internal void WriteRequestBody()
		{
			var bodyBytes = Context._requestBody?.GetBody();
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
