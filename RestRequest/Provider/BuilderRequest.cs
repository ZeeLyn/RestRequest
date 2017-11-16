﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace RestRequest.Provider
{
	internal class BuilderRequest
	{
		private HttpWebRequest Request { get; set; }

		private BuilderBase Builder { get; }

		internal BuilderRequest(BuilderBase builder)
		{
			Builder = builder;
		}

		internal void CreateRequest()
		{
			Request = (HttpWebRequest)WebRequest.Create(Builder.Url);
			Request.Method = Builder.Method.ToString().ToUpper();
			if (Builder.RequestHeaders != null && Builder.RequestHeaders.Count > 0)
				Request.Headers = Builder.RequestHeaders;
			if (Builder.RequestBody != null)
				Request.ContentType = Builder.RequestBody.GetContentType();
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
		}

		internal void BuildRequest()
		{
			if (Builder.RequestBody == null)
				return;
			using (var bodyStream = Builder.RequestBody.GetBody())
			{
				Request.ContentLength = bodyStream.Length;
				using (var requestStream = Request.GetRequestStream())
				{
					bodyStream.Position = 0;
					var bytes = new byte[bodyStream.Length];
					bodyStream.Read(bytes, 0, bytes.Length);
					requestStream.Write(bytes, 0, bytes.Length);
				}
			}
		}

		internal void BuildRequestAndCallback()
		{
			if (Builder.RequestBody != null && (Request.Method.Equals(HttpMethod.Post.ToString(), StringComparison.CurrentCultureIgnoreCase) || Request.Method.Equals(HttpMethod.Put.ToString(), StringComparison.CurrentCultureIgnoreCase)))
			{
				Request.ContentType = Builder.RequestBody.GetContentType();
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
						var callback = Builder.GetCallBack();
						request.BeginGetResponse(ProcessCallback(callback?.Success, callback?.Fail), request);
					}, Request);
				}
			}
			else
			{
				var callback = Builder.GetCallBack();
				Request.BeginGetResponse(ProcessCallback(callback?.Success, callback?.Fail), Request);
			}
		}

		protected AsyncCallback ProcessCallback(Action<HttpStatusCode, Stream> success, Action<WebException> fail)
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
					Request.Abort();
				}
			};
		}


		internal (bool Success, HttpStatusCode StatusCode, Stream ResponseContent, HttpWebResponse Response) GetResponse()
		{
			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)Request.GetResponse();
				return (true, response.StatusCode, response.GetResponseStream(), response);
			}
			catch (WebException e)
			{
				response?.Close();
				if (e.Response == null)
					return (false, HttpStatusCode.InternalServerError, new MemoryStream(Encoding.UTF8.GetBytes(e.Message)), null);
				var badRes = (HttpWebResponse)e.Response;
				return (false, badRes.StatusCode, new MemoryStream(Encoding.UTF8.GetBytes(e.Message)), badRes);
			}
		}

		public void Dispose()
		{
			Request?.Abort();
		}
	}
}
