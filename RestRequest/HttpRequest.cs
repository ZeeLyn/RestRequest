using System.Net.Http;
using RestRequest.Builder;
using RestRequest.Interface;

namespace RestRequest
{
	public static class HttpRequest
	{
		/// <summary>
		/// Get request
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static INoneBodyBuilder Get(string url)
		{
			return new ContextBuilder(url, HttpMethod.Get);
		}

		/// <summary>
		/// Head request
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static INoneBodyBuilder Head(string url)
		{
			return new ContextBuilder(url, HttpMethod.Head);
		}

		/// <summary>
		/// Options request
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static INoneBodyBuilder Options(string url)
		{
			return new ContextBuilder(url, HttpMethod.Options);
		}

		/// <summary>
		/// Trace request
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static INoneBodyBuilder Trace(string url)
		{
			return new ContextBuilder(url, HttpMethod.Trace);
		}

		/// <summary>
		/// Post request
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static IBodyBuilder Post(string url)
		{
			return new ContextBuilder(url, HttpMethod.Post);
		}

		/// <summary>
		/// Put request
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static IBodyBuilder Put(string url)
		{
			return new ContextBuilder(url, HttpMethod.Put);
		}

		/// <summary>
		/// Delete request
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static INoneBodyBuilder Delete(string url)
		{
			return new ContextBuilder(url, HttpMethod.Delete);
		}
	}
}
