using System.Net.Http;
using RestRequest.Builder;
using RestRequest.Interface;

namespace RestRequest
{
	public static class HttpRequest
	{
		/// <summary>
		/// Get请求
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static INoneBodyBuilder Get(string url)
		{
			return new ContentBuilder(url, HttpMethod.Get);
		}

		/// <summary>
		/// Post请求
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static IBodyBuilder Post(string url)
		{
			return new ContentBuilder(url, HttpMethod.Post);
		}
		/// <summary>
		/// Put请求
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static IBodyBuilder Put(string url)
		{
			return new ContentBuilder(url, HttpMethod.Put);
		}
		/// <summary>
		/// Delete请求
		/// </summary>
		/// <param name="url">url</param>
		/// <returns></returns>
		public static INoneBodyBuilder Delete(string url)
		{
			return new ContentBuilder(url, HttpMethod.Delete);
		}
	}
}
