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
			if (Context.IgnoreCertificateError)
				Request.ServerCertificateValidationCallback = ValidationCertificate.VerifyServerCertificate;
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
			Request.KeepAlive = Context._KeepAlive;
			Request.Referer = Context._Referer;
			Request.ServicePoint.ConnectionLimit = Context._ConnectionLimit;
		}

		internal void WriteRequestBody()
		{
			var bodyStream = Context.RequestBody?.GetBody();
			if (bodyStream == null)
				return;
			using (bodyStream)
			using (var requestStream = Request.GetRequestStream())
			{
				Request.ContentLength = bodyStream.Length;
				bodyStream.Position = 0;
				var bytes = new byte[bodyStream.Length];
				bodyStream.Read(bytes, 0, bytes.Length);
				requestStream.Write(bytes, 0, bytes.Length);
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
