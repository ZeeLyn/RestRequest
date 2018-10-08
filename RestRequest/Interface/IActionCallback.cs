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
		/// <param name="succeedStatus"></param>
		/// <returns></returns>
		IActionCallback OnSuccess(Action<HttpStatusCode, Stream> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		/// <summary>
		/// 成功时回调
		/// </summary>
		/// <param name="action"></param>
		/// <param name="succeedStatus"></param>
		/// <returns></returns>
		IActionCallback OnSuccess(Action<HttpStatusCode, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		IActionCallback OnSuccess<T>(Action<HttpStatusCode, T> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		/// <summary>
		/// 失败时回调
		/// </summary>
		/// <param name="action"></param>
		/// <param name="succeedStatus"></param>
		/// <returns></returns>
		IActionCallback OnFail(Action<HttpStatusCode?, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		/// <summary>
		/// 开始请求
		/// </summary>
		void Start();
	}
}
