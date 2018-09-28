using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestRequest.Interface;

namespace RestRequest.Builder
{
	internal partial class ContextBuilder
	{
		public IActionCallback OnSuccess(Action<HttpStatusCode, Stream> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			SuccessAction = action;
			SucceedStatus = succeedStatus;
			return this;
		}

		public IActionCallback OnSuccess(Action<HttpStatusCode, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			SuccessAction = (statusCode, stream) =>
			{
				using (var reader = new StreamReader(stream))
				{
					action(statusCode, reader.ReadToEnd());
				}
			};
			SucceedStatus = succeedStatus;
			return this;
		}

		public IActionCallback OnSuccess<T>(Action<HttpStatusCode, T> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			SuccessAction = (statusCode, stream) =>
			{
				using (var reader = new StreamReader(stream))
				{
					action(statusCode, JsonConvert.DeserializeObject<T>(reader.ReadToEnd()));
				}
			};
			SucceedStatus = succeedStatus;
			return this;
		}

		public IActionCallback OnFail(Action<HttpStatusCode?, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			FailAction = action;
			SucceedStatus = succeedStatus;
			return this;
		}

		public void Start()
		{
			var builder = new RequestBuilder(this);
			builder.BuildRequest();
			builder.BuildCallback();
		}

		public ResponseResult<Stream> ResponseStream(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			using (var builder = new RequestBuilder(this))
			{
				builder.BuildRequest();
				builder.WriteRequestBody();
				var res = builder.GetResponse();
				var succeed = res.StatusCode == succeedStatus;
				var contentStream = res.GetResponseStream();
				var contentString = "";
				if (!succeed && contentStream != null)
				{
					using (var reader = new StreamReader(contentStream))
					{
						contentString = reader.ReadToEnd();
					}
				}
				return new ResponseResult<Stream>
				{
					Succeed = succeed,
					StatusCode = res.StatusCode,
					Content = contentStream,
					FailedContent = contentString,
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
				var succeed = res.StatusCode == succeedStatus;
				var contentStream = res.GetResponseStream();
				var contentString = "";
				if (!succeed && contentStream != null)
				{
					using (var reader = new StreamReader(contentStream))
					{
						contentString = await reader.ReadToEndAsync();
					}
				}
				return new ResponseResult<Stream>
				{
					Succeed = succeed,
					StatusCode = res.StatusCode,
					Content = contentStream,
					FailedContent = contentString,
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

		public ResponseResult<T> ResponseValue<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ResponseString();
			return new ResponseResult<T>
			{
				Succeed = res.StatusCode == succeedStatus,
				StatusCode = res.StatusCode,
				Content = string.IsNullOrWhiteSpace(res.Content) ? default : JsonConvert.DeserializeObject<T>(res.Content),
				Response = res.Response,
				Request = res.Request
			};
		}

		public async Task<ResponseResult<T>> ResponseValueAsync<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ResponseStringAsync();
			return new ResponseResult<T>
			{
				Succeed = res.StatusCode == succeedStatus,
				StatusCode = res.StatusCode,
				Content = string.IsNullOrWhiteSpace(res.Content) ? default : JsonConvert.DeserializeObject<T>(res.Content),
				Response = res.Response,
				Request = res.Request
			};
		}

	}
}
