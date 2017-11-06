using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RestRequest
{
	public class ResponseResult<T> : IDisposable
	{
		/// <summary>
		/// http响应对象
		/// </summary>
		public HttpWebResponse Response { get; protected internal set; }
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
			Response?.Dispose();
			var stream = Content as Stream;
			stream?.Dispose();
		}
	}
}
