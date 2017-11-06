using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestRequest.interfaces;

namespace RestRequest.Provider
{
	public class BuilderNoneBody : BuilderBase, IBuilderNoneBody
	{
		public BuilderNoneBody(string url, HttpMethod method) : base(url, method)
		{
			RequestBody = new DefaultBody();
		}


		public IActionCallback OnSuccess(Action<HttpStatusCode, Stream> action)
		{
			SuccessAction = action;
			return this;
		}

		public IActionCallback OnSuccess(Action<HttpStatusCode, string> action)
		{
			SuccessAction = (statuscode, stream) =>
			{
				using (var reader = new StreamReader(stream))
				{
					action(statuscode, reader.ReadToEnd());
				}
			};
			return this;
		}

		public IActionCallback OnFail(Action<WebException> action)
		{
			FailAction = action;
			return this;
		}


		public IBuilderNoneBody Headers(Dictionary<string, string> headers)
		{
			if (!(headers?.Count > 0)) return this;
			foreach (var item in headers)
				RequestHeaders[item.Key] = item.Value;
			return this;
		}

		public IBuilderNoneBody Headers(object headers)
		{
			if (headers == null)
				return this;
			var properties = headers.GetType().GetProperties();
			foreach (var item in properties)
			{
				RequestHeaders[item.Name] = item.GetValue(headers).ToString();
			}
			return this;
		}

		public void Start()
		{
			var builder = new BuilderRequest(this);
			builder.CreateRequest();
			builder.BuildRequestAndCallback();
		}

		public IBuilderNoneBody ContentType(string contenttype)
		{
			RequestBody?.SetContentType(contenttype);
			return this;
		}

		public ResponseResult<Stream> ResponseStream()
		{
			var builder = new BuilderRequest(this);
			builder.CreateRequest();
			var res = builder.GetResponse();
			builder.Dispose();
			return new ResponseResult<Stream>
			{
				Succeed = res.Success,
				StatusCode = res.StatusCode,
				Content = res.ResponseContent,
				Response = res.Response
			};
		}

		public async Task<ResponseResult<Stream>> ResponseStreamAsync()
		{
			var builder = new BuilderRequestAsync(this);
			builder.CreateRequest();
			var res = await builder.GetResponseAsync();
			builder.Dispose();
			return new ResponseResult<Stream>
			{
				Succeed = res.Success,
				StatusCode = res.StatusCode,
				Content = res.ResponseContent,
				Response = res.Response
			};
		}

		public ResponseResult<string> ResponseString()
		{
			var res = ResponseStream();
			using (var stream = res.Content)
			using (var reader = new StreamReader(stream))
			{
				return new ResponseResult<string>
				{
					Succeed = res.Succeed,
					StatusCode = res.StatusCode,
					Content = reader.ReadToEnd(),
					Response = res.Response
				};
			}
		}

		public async Task<ResponseResult<string>> ResponseStringAsync()
		{
			var res = await ResponseStreamAsync();
			using (var stream = res.Content)
			using (var reader = new StreamReader(stream))
			{
				return new ResponseResult<string>
				{
					Succeed = res.Succeed,
					StatusCode = res.StatusCode,
					Content = await reader.ReadToEndAsync(),
					Response = res.Response
				};
			}
		}

		public ResponseResult<T> ResponseValue<T>()
		{
			var res = ResponseString();
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = JsonConvert.DeserializeObject<T>(res.Content),
				Response = res.Response
			};
		}

		public async Task<ResponseResult<T>> ResponseValueAsync<T>()
		{
			var res = await ResponseStringAsync();
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = JsonConvert.DeserializeObject<T>(res.Content),
				Response = res.Response
			};
		}
	}
}