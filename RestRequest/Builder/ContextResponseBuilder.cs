using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RestRequest.Interface;

namespace RestRequest.Builder
{
	public partial class ContextBuilder
	{
		#region Execute request
		private (bool Succeed, HttpStatusCode StatusCode, byte[] ResponseBytes, string FailMessage, Dictionary<string, string> Headers) ExecuteRequest(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			using (var builder = new RequestBuilder(this))
			{
				builder.BuildRequest();
				builder.WriteRequestBody();
				using (var response = builder.GetResponse())
				using (var contentStream = response.GetResponseStream())
				{
					var headers = response.Headers.AllKeys.ToDictionary(key => key, key => response.Headers.Get(key));
					var bytes = contentStream.AsBytes();
					return (response.StatusCode == succeedStatus, response.StatusCode,
						succeedStatus == response.StatusCode ? bytes : null,
						succeedStatus == response.StatusCode ? "" : bytes.AsString(), headers);
				}
			}
		}
		#endregion


		#region Obsolete
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
				action(statusCode, stream.AsBytes().AsString());
			};
			_succeedStatus = succeedStatus;
			return this;
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

				var statusCode = res.StatusCode;
				return new ResponseResult<Stream>
				{
					Succeed = succeed,
					StatusCode = statusCode,
					Content = contentStream,
					FailMessage = failString,
					Headers = res.Headers.AllKeys.ToDictionary(key => key, key => res.Headers.Get(key))
				};
			}
		}

		public async Task<ResponseResult<Stream>> ResponseStreamAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			using (var builder = new RequestBuilder(this))
			{
				builder.BuildRequest();
				await builder.WriteRequestBodyAsync(CancellationToken.None);
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
					FailMessage = failString,
					Headers = res.Headers.AllKeys.ToDictionary(key => key, key => res.Headers.Get(key))
				};
			}
		}



		public ResponseResult<string> ResponseString(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ExecuteRequest(succeedStatus);
			return new ResponseResult<string>
			{
				Succeed = res.StatusCode == succeedStatus,
				FailMessage = res.FailMessage,
				StatusCode = res.StatusCode,
				Content = res.Succeed ? res.ResponseBytes.AsString() : "",
				Headers = res.Headers
			};
		}

		public async Task<ResponseResult<string>> ResponseStringAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
			return new ResponseResult<string>
			{
				Succeed = res.StatusCode == succeedStatus,
				FailMessage = res.FailMessage,
				StatusCode = res.StatusCode,
				Content = res.Succeed ? res.ResponseBytes.AsString() : "",
				Headers = res.Headers
			};
		}

		#endregion


		#region Download
		public ResponseResult<byte[]> Download(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ExecuteRequest(succeedStatus);
			return new ResponseResult<byte[]>
			{
				Succeed = res.StatusCode == succeedStatus,
				StatusCode = res.StatusCode,
				Content = res.ResponseBytes,
				FailMessage = res.FailMessage,
				Headers = res.Headers
			};
		}



		public void Download(Action<ResponseResult<byte[]>> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			action?.Invoke(Download(succeedStatus));
		}





		public bool Download(string saveFileName, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ExecuteRequest(succeedStatus);
			if (res.Succeed)
				res.ResponseBytes.SaveAs(saveFileName);
			return res.Succeed;
		}





		#endregion


		#region  Response type

		public ResponseResult<T> ResponseValue<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ExecuteRequest(succeedStatus);
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default,
				FailMessage = res.FailMessage,
				Headers = res.Headers
			};
		}


		public void ResponseValue(Action<bool, HttpStatusCode, byte[], string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ExecuteRequest(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage);
		}

		public void ResponseValue(Action<bool, HttpStatusCode, byte[], string, Dictionary<string, string>> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ExecuteRequest(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage, res.Headers);
		}

		public void ResponseValue<T>(Action<bool, HttpStatusCode, T, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ExecuteRequest(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default, res.FailMessage);
		}

		public void ResponseValue<T>(Action<bool, HttpStatusCode, T, string, Dictionary<string, string>> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ExecuteRequest(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default, res.FailMessage, res.Headers);
		}
		#endregion
	}
}
