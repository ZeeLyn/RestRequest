using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using RestRequest.interfaces;

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
			if (Builder.RequestBody != null)
				Request.ContentType = Builder.RequestBody.GetContentType();
			if (Builder.RequestHeaders != null && Builder.RequestHeaders.Count > 0)
				Request.Headers = Builder.RequestHeaders;

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
			if (Builder.RequestBody != null)
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

		protected AsyncCallback ProcessCallback(Action<HttpStatusCode, string> success, Action<WebException> fail)
		{
			return r =>
			{
				var webRequest = (HttpWebRequest)r.AsyncState;
				try
				{
					using (var response = (HttpWebResponse)webRequest.EndGetResponse(r))
					using (var res = response.GetResponseStream())
					using (var reader = new StreamReader(res))
					{
						success?.Invoke(response.StatusCode, reader.ReadToEnd());
					}
				}
				catch (WebException ex)
				{
					fail?.Invoke(ex);
				}
			};
		}


		internal (bool Success, HttpStatusCode StatusCode, string ResponseContent) GetResponse()
		{
			try
			{
				using (var response = (HttpWebResponse)Request.GetResponse())
				using (var res = response.GetResponseStream())
				using (var reader = new StreamReader(res))
				{
					return (true, response.StatusCode, reader.ReadToEnd());
				}
			}
			catch (WebException e)
			{
				if (e.Response == null)
					return (false, HttpStatusCode.InternalServerError, e.Message);
				using (var badRes = (HttpWebResponse)e.Response)
				using (var res = badRes.GetResponseStream())
				using (var reader = new StreamReader(res))
				{
					return (false, badRes.StatusCode, reader.ReadToEnd());
				}
			}
		}
	}
}
