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
	public class BaseBuilder //: IActionCallback
	{
		internal Uri Url { get; set; }

		internal string ContentType { get; set; }

		internal HttpMethod Method { get; set; }

		internal WebHeaderCollection RequestHeaders { get; set; }

		internal IBody RequestBody { get; set; }

		internal Action<HttpStatusCode, Stream> SuccessAction { get; set; }

		internal Action<WebException> FailAction { get; set; }

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

		//public IActionCallback OnSuccess(Action<HttpStatusCode, Stream> action)
		//{
		//	SuccessAction = action;
		//	return this;
		//}

		//public IActionCallback OnSuccess(Action<HttpStatusCode, string> action)
		//{
		//	SuccessAction = (statusCode, stream) =>
		//	{
		//		using (var reader = new StreamReader(stream))
		//		{
		//			action(statusCode, reader.ReadToEnd());
		//		}
		//	};
		//	return this;
		//}

		//public IActionCallback OnFail(Action<WebException> action)
		//{
		//	FailAction = action;
		//	return this;
		//}

		public void Start()
		{
			var builder = new RequestBuilder(this);
			builder.BuildRequest();
			builder.BuildRequestAndCallback();
		}


		public ResponseResult<Stream> ResponseStream(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			using (var builder = new RequestBuilder(this))
			{
				builder.BuildRequest();
				builder.WriteRequestBody();
				var res = builder.GetResponse();
				return new ResponseResult<Stream>
				{
					Succeed = res.StatusCode == succeedStatus,
					StatusCode = res.StatusCode,
					Content = res.GetResponseStream(),
					Response = res,
					Request = builder.Request
				};
			}
		}

		public async Task<ResponseResult<Stream>> ResponseStreamAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			using (var builder = new RequestBuilder(this))
			{
				builder.BuildRequest();
				await builder.WriteRequestBodyAsync();
				var res = await builder.GetResponseAsync();
				return new ResponseResult<Stream>
				{
					Succeed = res.StatusCode == succeedStatus,
					StatusCode = res.StatusCode,
					Content = res.GetResponseStream(),
					Response = res,
					Request = builder.Request
				};
			}
		}

		public ResponseResult<string> ResponseString(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ResponseStream();
			using (var stream = res.Content)
			using (var reader = new StreamReader(stream))
			using (res.Response)
			{
				return new ResponseResult<string>
				{
					Succeed = res.StatusCode == succeedStatus,
					StatusCode = res.StatusCode,
					Content = reader.ReadToEnd(),
					Response = res.Response,
					Request = res.Request
				};
			}
		}

		public async Task<ResponseResult<string>> ResponseStringAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ResponseStreamAsync();
			using (var stream = res.Content)
			using (var reader = new StreamReader(stream))
			using (res.Response)
			{
				return new ResponseResult<string>
				{
					Succeed = res.StatusCode == succeedStatus,
					StatusCode = res.StatusCode,
					Content = await reader.ReadToEndAsync(),
					Response = res.Response,
					Request = res.Request
				};
			}
		}

		public ResponseResult<T> ResponseValue<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK) where T : class
		{
			var res = ResponseString();
			return new ResponseResult<T>
			{
				Succeed = res.StatusCode == succeedStatus,
				StatusCode = res.StatusCode,
				Content = string.IsNullOrWhiteSpace(res.Content) ? default(T) : JsonConvert.DeserializeObject<T>(res.Content),
				Response = res.Response,
				Request = res.Request
			};
		}

		public async Task<ResponseResult<T>> ResponseValueAsync<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK) where T : class
		{
			var res = await ResponseStringAsync();
			return new ResponseResult<T>
			{
				Succeed = res.StatusCode == succeedStatus,
				StatusCode = res.StatusCode,
				Content = string.IsNullOrWhiteSpace(res.Content) ? default(T) : JsonConvert.DeserializeObject<T>(res.Content),
				Response = res.Response,
				Request = res.Request
			};
		}
	}
}
