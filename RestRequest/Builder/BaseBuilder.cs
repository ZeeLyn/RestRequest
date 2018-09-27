using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestRequest.Interface;

namespace RestRequest.Builder
{
	public class BaseBuilder
	{
		internal Uri Url { get; set; }

		internal string ContentType { get; set; }

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

		protected BaseBuilder(string url, HttpMethod method)
		{
			Url = new Uri(url);
			Method = method;
			RequestHeaders = new WebHeaderCollection();
		}

		public CallbackIBuilder GetCallBack()
		{
			if (SuccessAction != null || FailAction != null)
				return new CallbackAction
				{
					Success = SuccessAction,
					Fail = FailAction
				};
			return null;
		}

		public void Start()
		{
			var builder = new RestRequest.Execute(this);
			builder.CreateRequest();
			builder.BuildRequestAndCallback();
		}


		public ResponseResult<Stream> ResponseStream()
		{
			var builder = new RestRequest.Execute(this);
			builder.CreateRequest();
			builder.BuildRequest();
			var res = builder.GetResponse();
			return new ResponseResult<Stream>
			{
				Succeed = res.Success,
				StatusCode = res.StatusCode,
				Content = res.ResponseContent,
				Response = res.Response,
				Request = builder.Request
			};
		}

		public async Task<ResponseResult<Stream>> ResponseStreamAsync()
		{
			var builder = new RestRequest.Execute(this);
			builder.CreateRequest();
			await builder.BuildRequestAsync();
			var res = await builder.GetResponseAsync();
			return new ResponseResult<Stream>
			{
				Succeed = res.Success,
				StatusCode = res.StatusCode,
				Content = res.ResponseContent,
				Response = res.Response,
				Request = builder.Request
			};
		}

		public ResponseResult<string> ResponseString()
		{
			var res = ResponseStream();
			using (var stream = res.Content)
			using (var reader = new StreamReader(stream))
			using (res.Response)
			{
				return new ResponseResult<string>
				{
					Succeed = res.Succeed,
					StatusCode = res.StatusCode,
					Content = reader.ReadToEnd(),
					Response = res.Response,
					Request = res.Request
				};
			}
		}

		public async Task<ResponseResult<string>> ResponseStringAsync()
		{
			var res = await ResponseStreamAsync();
			using (var stream = res.Content)
			using (var reader = new StreamReader(stream))
			using (res.Response)
			{
				return new ResponseResult<string>
				{
					Succeed = res.Succeed,
					StatusCode = res.StatusCode,
					Content = await reader.ReadToEndAsync(),
					Response = res.Response,
					Request = res.Request
				};
			}
		}

		public ResponseResult<T> ResponseValue<T>() where T : class
		{
			var res = ResponseString();
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = string.IsNullOrWhiteSpace(res.Content) ? default(T) : JsonConvert.DeserializeObject<T>(res.Content),
				Response = res.Response,
				Request = res.Request
			};
		}

		public async Task<ResponseResult<T>> ResponseValueAsync<T>() where T : class
		{
			var res = await ResponseStringAsync();
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = string.IsNullOrWhiteSpace(res.Content) ? default(T) : JsonConvert.DeserializeObject<T>(res.Content),
				Response = res.Response,
				Request = res.Request
			};
		}
	}
}
