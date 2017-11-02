using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RestRequest.interfaces
{
	public interface IActionCallback
	{

		IActionCallback OnSuccess(Action<HttpStatusCode, string> action);

		IActionCallback OnFail(Action<WebException> action);

		void Start();
	}
}
