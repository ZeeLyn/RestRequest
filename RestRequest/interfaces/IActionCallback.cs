using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RestRequest.interfaces
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
