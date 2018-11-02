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
		#region Execute request
		private (bool Succeed, HttpStatusCode StatusCode, byte[] ResponseBytes, string FailMessage) ExecuteRequest(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			using (var builder = new RequestBuilder(this))
			{
				builder.BuildRequest();
				builder.WriteRequestBody();
				var response = builder.GetResponse();
				var contentStream = response.GetResponseStream();
				var buffer = new byte[16 * 1024];
				using (response)
				using (contentStream)
				using (var ms = new MemoryStream())
				{
					int read;
					while ((read = contentStream.Read(buffer, 0, buffer.Length)) > 0)
						ms.Write(buffer, 0, read);
					var bytes = ms.ToArray();
					return (response.StatusCode == succeedStatus, response.StatusCode, succeedStatus == response.StatusCode ? bytes : null, succeedStatus == response.StatusCode ? "" : bytes.AsString());
				}

			}
		}

		private async Task<(bool Succeed, HttpStatusCode StatusCode, byte[] ResponseBytes, string FailMessage)> ExecuteRequestAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			using (var builder = new RequestBuilder(this))
			{
				builder.BuildRequest();
				await builder.WriteRequestBodyAsync();
				var response = await builder.GetResponseAsync();
				var contentStream = response.GetResponseStream();
				var buffer = new byte[16 * 1024];
				using (response)
				using (contentStream)
				using (var ms = new MemoryStream())
				{
					int read;
					while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
						ms.Write(buffer, 0, read);
					var bytes = ms.ToArray();
					return (response.StatusCode == succeedStatus, response.StatusCode, succeedStatus == response.StatusCode ? bytes : null, succeedStatus == response.StatusCode ? "" : bytes.AsString());
				}
			}
		}
		#endregion


		#region Async request
		public IActionCallback OnSuccess(Action<HttpStatusCode, Stream> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			_successAction = action;
			_succeedStatus = succeedStatus;
			return this;
		}

		public IActionCallback OnSuccess(Action<HttpStatusCode, byte[]> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			_successAction = (statusCode, stream) =>
			{
				action(statusCode, stream.AsBytes());
			};
			_succeedStatus = succeedStatus;
			throw new NotImplementedException();
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

		public IActionCallback OnSuccess<T>(Action<HttpStatusCode, T> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			_successAction = (statusCode, stream) =>
			{
				action(statusCode, JsonConvert.DeserializeObject<T>(stream.AsBytes().AsString()));
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

		#endregion


		#region Obsolete
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
					FailMessage = failString
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
					FailMessage = failString
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
				Content = res.Succeed ? res.ResponseBytes.AsString() : ""
			};
		}

		public async Task<ResponseResult<string>> ResponseStringAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(succeedStatus);
			return new ResponseResult<string>
			{
				Succeed = res.StatusCode == succeedStatus,
				FailMessage = res.FailMessage,
				StatusCode = res.StatusCode,
				Content = res.Succeed ? res.ResponseBytes.AsString() : ""
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
				FailMessage = res.FailMessage
			};
		}



		public void Download(Action<ResponseResult<byte[]>> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			action?.Invoke(Download(succeedStatus));
		}



		public async Task<ResponseResult<byte[]>> DownloadAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(succeedStatus);
			return new ResponseResult<byte[]>
			{
				Succeed = res.StatusCode == succeedStatus,
				StatusCode = res.StatusCode,
				Content = res.ResponseBytes,
				FailMessage = res.FailMessage
			};
		}


		public async Task DownloadAsync(Action<ResponseResult<byte[]>> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			action?.Invoke(await DownloadAsync(succeedStatus));
		}




		public bool Download(string SaveFileName, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ExecuteRequest(succeedStatus);
			if (res.Succeed)
				res.ResponseBytes.SaveAs(SaveFileName);
			return res.Succeed;
		}




		public async Task DownloadAsync(string SaveFileName, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(succeedStatus);
			if (res.Succeed)
				res.ResponseBytes.SaveAs(SaveFileName);
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
				Content = res.Succeed ? JsonConvert.DeserializeObject<T>(res.ResponseBytes.AsString()) : default,
				FailMessage = res.FailMessage
			};
		}

		public async Task<ResponseResult<T>> ResponseValueAsync<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(succeedStatus);
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = res.Succeed ? JsonConvert.DeserializeObject<T>(res.ResponseBytes.AsString()) : default,
				FailMessage = res.FailMessage
			};
		}

		public void ResponseValue(Action<bool, HttpStatusCode, byte[], string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ExecuteRequest(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage);
		}

		public async Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage);
		}


		public void ResponseValue<T>(Action<bool, HttpStatusCode, T, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = ExecuteRequest(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? JsonConvert.DeserializeObject<T>(res.ResponseBytes.AsString()) : default, res.FailMessage);
		}

		public async Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			var res = await ExecuteRequestAsync(succeedStatus);
			response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? JsonConvert.DeserializeObject<T>(res.ResponseBytes.AsString()) : default, res.FailMessage);
		}

		#endregion
	}
}
