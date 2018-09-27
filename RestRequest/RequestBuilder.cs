using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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

		internal void BuildRequestAndCallback()
		{
			if (Builder.RequestBody != null && (Request.Method.Equals(HttpMethod.Post.ToString(), StringComparison.CurrentCultureIgnoreCase) || Request.Method.Equals(HttpMethod.Put.ToString(), StringComparison.CurrentCultureIgnoreCase)))
			{
				Request.ContentType = Builder.ContentType;
				{
					Request.BeginGetRequestStream(r =>
					{
						var request = (HttpWebRequest)r.AsyncState;
						using (var bodyStream = Builder.RequestBody.GetBody())
						{
							Request.ContentLength = bodyStream.Length;
							using (var requestStream = request.EndGetRequestStream(r))
							{
								var bytes = new byte[bodyStream.Length];
								bodyStream.Read(bytes, 0, bytes.Length);
								requestStream.Write(bytes, 0, bytes.Length);
							}
						}
						request.BeginGetResponse(ProcessCallback(Builder.SuccessAction, Builder.FailAction), request);
					}, Request);
				}
			}
			else
			{
				Request.BeginGetResponse(ProcessCallback(Builder.SuccessAction, Builder.FailAction), Request);
			}
		}

		private AsyncCallback ProcessCallback(Action<HttpStatusCode, Stream> success, Action<WebException> fail)
		{
			return r =>
			{
				try
				{
					var webRequest = (HttpWebRequest)r.AsyncState;
					using (var response = (HttpWebResponse)webRequest.EndGetResponse(r))
					using (var stream = response.GetResponseStream())
					{
						success?.Invoke(response.StatusCode, stream);
					}
				}
				catch (WebException ex)
				{
					fail?.Invoke(ex);
				}
				finally
				{
					Dispose();
				}
			};
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
