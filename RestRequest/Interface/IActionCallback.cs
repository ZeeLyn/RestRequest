using System;
using System.IO;
using System.Net;

namespace RestRequest.Interface
{
	public interface IActionCallback
	{
		/// <summary>
		/// 成功时回调
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		IActionCallback OnSuccess(Action<HttpStatusCode, Stream> action);

		/// <summary>
		/// 成功时回调
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		IActionCallback OnSuccess(Action<HttpStatusCode, string> action);

		/// <summary>
		/// 失败时回调
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		IActionCallback OnFail(Action<WebException> action);

		/// <summary>
		/// 开始请求
		/// </summary>
		void Start();
	}
}
