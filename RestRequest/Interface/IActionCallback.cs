using System;
using System.IO;
using System.Net;

namespace RestRequest.Interface
{
	public interface IActionCallback
	{
		[Obsolete]
		IActionCallback OnSuccess(Action<HttpStatusCode, Stream> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		IActionCallback OnSuccess(Action<HttpStatusCode, byte[]> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		[Obsolete]
		IActionCallback OnSuccess(Action<HttpStatusCode, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		IActionCallback OnSuccess<T>(Action<HttpStatusCode, T> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		IActionCallback OnFail(Action<HttpStatusCode, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		void Start();
	}
}
