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
		public HttpWebResponse Response { get; protected internal set; }

		/// <summary>
		/// http request
		/// </summary>
		[JsonIgnore]
		public HttpWebRequest Request { get; protected internal set; }

		/// <summary>
		/// 是否请求成功
		/// </summary>
		public bool Succeed { get; internal set; }
		/// <summary>
		/// 响应状态码
		/// </summary>
		public HttpStatusCode StatusCode { get; internal set; }
		/// <summary>
		/// 响应内容
		/// </summary>
		public T Content { get; internal set; }

		public void Dispose()
		{
			Request?.Abort();
			var stream = Content as Stream;
			stream?.Dispose();
			Response?.Dispose();
		}
	}
}
