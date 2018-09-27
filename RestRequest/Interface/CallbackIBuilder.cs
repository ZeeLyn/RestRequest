using System;
using System.IO;
using System.Net;

namespace RestRequest.Interface
{
	public interface CallbackIBuilder
	{
		Action<HttpStatusCode, Stream> Success { get; }

		Action<WebException> Fail { get; }
	}
}
