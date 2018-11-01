using System;
using System.IO;
using System.Net;

namespace RestRequest.Interface
{
	public interface IActionCallback
	{
		IActionCallback OnSuccess(Action<HttpStatusCode, Stream> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);


		IActionCallback OnSuccess(Action<HttpStatusCode, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		IActionCallback OnSuccess<T>(Action<HttpStatusCode, T> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);


		IActionCallback OnFail(Action<HttpStatusCode?, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);


		void Start();
	}
}
