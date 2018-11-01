using System;
using System.Net;
using Newtonsoft.Json;

namespace RestRequest
{
	public class ResponseResult<T>
	{
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
		public string FailMessage { get; internal set; }
	}
}
