using System;
using System.Net;
using RestRequest.Builder;

namespace RestRequest
{
	internal partial class RequestBuilder : IDisposable
	{
		public HttpWebRequest Request { get; private set; }

		private BaseBuilder Builder { get; }

		internal RequestBuilder(BaseBuilder builder)
		{
			Builder = builder;
		}

		internal void BuildRequest()
		{
			Request = (HttpWebRequest)WebRequest.Create(Builder.Url);
			Request.Method = Builder.Method.ToString().ToUpper();
			if (Builder.RequestHeaders != null && Builder.RequestHeaders.Count > 0)
				Request.Headers = Builder.RequestHeaders;
			Request.ContentType = Builder.ContentType;
			if (Builder.IgnoreCertificateError)
				Request.ServerCertificateValidationCallback = ValidationCertificate.VerifyServerCertificate;
			if (Builder.ClientCertificates != null && Builder.ClientCertificates.Count > 0)
				Request.ClientCertificates.AddRange(Builder.ClientCertificates);
			if (!string.IsNullOrWhiteSpace(Builder.UserAgent))
				Request.UserAgent = Builder.UserAgent;
			if (Builder.Timeout > 0)
				Request.Timeout = Builder.Timeout;
			if (Builder.Cookies?.Count > 0)
			{
				Request.CookieContainer = new CookieContainer();
				foreach (var cookie in Builder.Cookies)
				{
					Request.CookieContainer.Add(cookie);
				}
			}
			Request.KeepAlive = Builder.KeepAlive;
		}

		internal void WriteRequestBody()
		{
			var bodyStream = Builder.RequestBody?.GetBody();
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
