using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestRequest.Interface;

namespace RestRequest.Builder
{
	public partial class ContextBuilder
	{
		public IActionCallback OnSuccess(Action<HttpStatusCode, Stream> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			_successAction = action;
			_succeedStatus = succeedStatus;
			return this;
		}

		public IActionCallback OnSuccess(Action<HttpStatusCode, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			_successAction = (statusCode, stream) =>
			{
				using (var reader = new StreamReader(stream))
				{
					action(statusCode, reader.ReadToEnd());
				}
			};
			_succeedStatus = succeedStatus;
			return this;
		}

		public IActionCallback OnSuccess<T>(Action<HttpStatusCode, T> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			_successAction = (statusCode, stream) =>
			{
				using (var reader = new StreamReader(stream))
				{
					action(statusCode, JsonConvert.DeserializeObject<T>(reader.ReadToEnd()));
				}
			};
			_succeedStatus = succeedStatus;
			return this;
		}

		public IActionCallback OnFail(Action<HttpStatusCode?, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			_failAction = action;
			_succeedStatus = succeedStatus;
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
				var failString = "";
				if (!succeed && contentStream != null && contentStream.CanRead)
				{
					using (var reader = new StreamReader(contentStream))
					{
						failString = reader.ReadToEnd();
					}
				}
				return new ResponseResult<Stream>
				{
					Succeed = succeed,
					StatusCode = res.StatusCode,
					Content = contentStream,
					FailedContent = failString,
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
				var failString = "";
				if (!succeed && contentStream != null && contentStream.CanRead)
				{
					using (var reader = new StreamReader(contentStream))
					{
						failString = await reader.ReadToEndAsync();
					}
				}
				return new ResponseResult<Stream>
				{
					Succeed = succeed,
					StatusCode = res.StatusCode,
					Content = contentStream,
					FailedContent = failString,
					Response = res,
					Request = builder.Request
				};
			}
		}

		public ResponseResult<string> ResponseString(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ResponseStream(succeedStatus);
			var result = new ResponseResult<string>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Response = res.Response,
				FailedContent = res.FailedContent,
				Request = res.Request
			};
			if (!res.Content.CanRead)
				return result;
			using (var stream = res.Content)
			using (var reader = new StreamReader(stream))
			using (res.Response)
			{
				result.Content = reader.ReadToEnd();
			}
			return result;
		}

		public async Task<ResponseResult<string>> ResponseStringAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ResponseStreamAsync(succeedStatus);
			var result = new ResponseResult<string>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Response = res.Response,
				FailedContent = res.FailedContent,
				Request = res.Request
			};
			if (!res.Content.CanRead)
				return result;
			using (var stream = res.Content)
			using (var reader = new StreamReader(stream))
			using (res.Response)
			{
				result.Content = await reader.ReadToEndAsync();
			}

			return result;
		}

		public ResponseResult<T> ResponseValue<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ResponseString(succeedStatus);
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = string.IsNullOrWhiteSpace(res.Content) ? default : JsonConvert.DeserializeObject<T>(res.Content),
				FailedContent = res.FailedContent,
				Response = res.Response,
				Request = res.Request
			};
		}

		public async Task<ResponseResult<T>> ResponseValueAsync<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ResponseStringAsync(succeedStatus);
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = string.IsNullOrWhiteSpace(res.Content) ? default : JsonConvert.DeserializeObject<T>(res.Content),
				FailedContent = res.FailedContent,
				Response = res.Response,
				Request = res.Request
			};
		}

		public void Response(Action<bool, HttpStatusCode, Stream, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ResponseStream(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Content, res.FailedContent);
		}

		public async Task ResponseAsync(Action<bool, HttpStatusCode, Stream, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ResponseStreamAsync(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Content, res.FailedContent);
		}

		public void Response(Action<bool, HttpStatusCode, string, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ResponseString(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Content, res.FailedContent);
		}

		public async Task ResponseAsync(Action<bool, HttpStatusCode, string, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ResponseStringAsync(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Content, res.FailedContent);
		}

		public void Response<T>(Action<bool, HttpStatusCode, T, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ResponseValue<T>(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Content, res.FailedContent);
		}

		public async Task ResponseAsync<T>(Action<bool, HttpStatusCode, T, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ResponseValueAsync<T>(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Content, res.FailedContent);
		}
	}
}
