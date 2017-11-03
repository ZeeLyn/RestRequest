using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using RestRequest.interfaces;
using RestRequest.Provider;

namespace RestRequest
{
	public class HttpRequest
	{
		public static IBuilderNoneBody Get(string url)
		{
			return new BuilderNoneBody(url, HttpMethod.Get);
		}

		public static IBuilder Post(string url)
		{
			return new BuilderBody(url, HttpMethod.Post);
		}

		public static IBuilder Put(string url)
		{
			return new BuilderBody(url, HttpMethod.Put);
		}

		public static IBuilderNoneBody Delete(string url)
		{
			return new BuilderNoneBody(url, HttpMethod.Delete);
		}
	}
}
