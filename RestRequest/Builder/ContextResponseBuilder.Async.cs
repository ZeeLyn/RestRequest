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
		private async Task<(bool Succeed, HttpStatusCode StatusCode, byte[] ResponseBytes, string FailMessage, Dictionary<string, string> Headers)> ExecuteRequestAsync(CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			using (var builder = new RequestBuilder(this))
			{
				builder.BuildRequest();
				await builder.WriteRequestBodyAsync(cancellationToken);
				using (var response = await builder.GetResponseAsync())
				using (var contentStream = response.GetResponseStream())
				{
					var headers = response.Headers.AllKeys.ToDictionary(key => key, key => response.Headers.Get(key));
					var bytes = await contentStream.AsBytesAsync(cancellationToken);
					return (response.StatusCode == succeedStatus, response.StatusCode,
						succeedStatus == response.StatusCode ? bytes : null,
						succeedStatus == response.StatusCode ? "" : bytes.AsString(), headers);
				}
			}
		}
		#region Async request

		public IActionCallback OnSuccess(Action<HttpStatusCode, byte[]> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			_successAction = (statusCode, stream) =>
			{
				action(statusCode, stream.AsBytes());
			};
			_succeedStatus = succeedStatus;
			return this;
		}


		public IActionCallback OnSuccess<T>(Action<HttpStatusCode, T> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			_successAction = (statusCode, stream) =>
			{
				action(statusCode, stream.AsBytes().AsString().JsonTo<T>());
			};
			_succeedStatus = succeedStatus;
			return this;
		}

		public IActionCallback OnFail(Action<HttpStatusCode, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
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

		#endregion

		#region Download
		public async Task<ResponseResult<byte[]>> DownloadAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
			return new ResponseResult<byte[]>
			{
				Succeed = res.StatusCode == succeedStatus,
				StatusCode = res.StatusCode,
				Content = res.ResponseBytes,
				FailMessage = res.FailMessage,
				Headers = res.Headers
			};
		}

		public async Task<ResponseResult<byte[]>> DownloadAsync(CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
			return new ResponseResult<byte[]>
			{
				Succeed = res.StatusCode == succeedStatus,
				StatusCode = res.StatusCode,
				Content = res.ResponseBytes,
				FailMessage = res.FailMessage,
				Headers = res.Headers
			};
		}


		public async Task DownloadAsync(Action<ResponseResult<byte[]>> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			action?.Invoke(await DownloadAsync(succeedStatus));
		}

		public async Task DownloadAsync(Action<ResponseResult<byte[]>> action, CancellationToken cancellationToken,
			HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			action?.Invoke(await DownloadAsync(cancellationToken, succeedStatus));
		}


		public async Task DownloadAsync(string saveFileName, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
			if (res.Succeed)
				res.ResponseBytes.SaveAs(saveFileName);
		}

		public async Task DownloadAsync(string saveFileName, CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
			if (res.Succeed)
				res.ResponseBytes.SaveAs(saveFileName);
		}

		#endregion


		public async Task<ResponseResult<T>> ResponseValueAsync<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default,
				FailMessage = res.FailMessage,
				Headers = res.Headers
			};
		}

		public async Task<ResponseResult<T>> ResponseValueAsync<T>(CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default,
				FailMessage = res.FailMessage,
				Headers = res.Headers
			};
		}


		public async Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage);
		}

		public async Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string> response, CancellationToken cancellationToken,
			HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage);
		}

		public async Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string, Dictionary<string, string>> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage, res.Headers);
		}

		public async Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string, Dictionary<string, string>> response, CancellationToken cancellationToken,
			HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage, res.Headers);
		}

		public async Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default, res.FailMessage);
		}


		public async Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string> response, CancellationToken cancellationToken,
			HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default, res.FailMessage);
		}

		public async Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string, Dictionary<string, string>> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default, res.FailMessage, res.Headers);
		}

		public async Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string, Dictionary<string, string>> response, CancellationToken cancellationToken,
			HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default, res.FailMessage, res.Headers);
		}
	}
}
