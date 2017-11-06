using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RestRequest.interfaces
{
	public interface IBuilderCallback
	{
		Action<HttpStatusCode, Stream> Success { get; }

		Action<WebException> Fail { get; }
	}
}
