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
		public static IBuilderNoneBody Get(string url, bool ignoreCertificateError = true)
		{
			return new BuilderNoneBody(url, HttpMethod.Get, ignoreCertificateError);
		}

		public static IBuilder Post(string url, bool ignoreCertificateError = true)
		{
			return new BuilderBody(url, HttpMethod.Post, ignoreCertificateError);
		}

		public static IBuilder Put(string url, bool ignoreCertificateError = true)
		{
			return new BuilderBody(url, HttpMethod.Put, ignoreCertificateError);
		}

		public static IBuilderNoneBody Delete(string url, bool ignoreCertificateError = true)
		{
			return new BuilderNoneBody(url, HttpMethod.Delete, ignoreCertificateError);
		}
	}
}
