using System.Net.Http;
using RestRequest.interfaces;
using RestRequest.Provider;

namespace RestRequest
{
	public class HttpRequest
	{
		/// <summary>
		/// Get请求
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static IBuilderNoneBody Get(string url)
		{
			return new BuilderNoneBody(url, HttpMethod.Get);
		}

		/// <summary>
		/// Post请求
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static IBuilder Post(string url)
		{
			return new BuilderBody(url, HttpMethod.Post);
		}
		/// <summary>
		/// Put请求
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static IBuilder Put(string url)
		{
			return new BuilderBody(url, HttpMethod.Put);
		}
		/// <summary>
		/// Delete请求
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static IBuilderNoneBody Delete(string url)
		{
			return new BuilderNoneBody(url, HttpMethod.Delete);
		}
	}
}
