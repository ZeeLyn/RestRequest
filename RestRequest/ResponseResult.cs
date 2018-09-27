using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace RestRequest
{
	public class ResponseResult<T> : IDisposable
	{
		/// <summary>
		/// http响应对象
		/// </summary>
		[JsonIgnore]
		internal HttpWebResponse Response { get; set; }

		/// <summary>
		/// http request
		/// </summary>
		[JsonIgnore]
		internal HttpWebRequest Request { get; set; }

		/// <summary>
		/// 是否请求成功
		/// </summary>
		public bool Succeed { get; internal set; }
		/// <summary>
		/// 响应状态码
		/// </summary>
		public HttpStatusCode StatusCode { get; internal set; }
		/// <summary>
		/// 成功响应内容
		/// </summary>
		public T Content { get; internal set; }

		/// <summary>
		/// 失败响应内容
		/// </summary>
		public string FailedContent { get; internal set; }

		public void Dispose()
		{
			Request?.Abort();
			var stream = Content as Stream;
			stream?.Dispose();
			Response?.Dispose();
		}
	}
}
