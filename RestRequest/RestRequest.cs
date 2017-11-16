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
		/// <summary>
		/// Get请求
		/// </summary>
		/// <param name="url">url</param>
		/// <param name="keepAlive">keepAlive</param>
		/// <param name="ignoreCertificateError">忽略证书错误</param>
		/// <returns></returns>
		public static IBuilderNoneBody Get(string url, bool keepAlive = false, bool ignoreCertificateError = true)
		{
			return new BuilderNoneBody(url, HttpMethod.Get, keepAlive, ignoreCertificateError);
		}
		/// <summary>
		/// Post请求
		/// </summary>
		/// <param name="url">url</param>
		/// <param name="keepAlive">keepAlive</param>
		/// <param name="ignoreCertificateError">忽略证书错误</param>
		/// <returns></returns>
		public static IBuilder Post(string url, bool keepAlive = false, bool ignoreCertificateError = true)
		{
			return new BuilderBody(url, HttpMethod.Post, keepAlive, ignoreCertificateError);
		}
		/// <summary>
		/// Put请求
		/// </summary>
		/// <param name="url">url</param>
		/// <param name="keepAlive">keepAlive</param>
		/// <param name="ignoreCertificateError">忽略证书错误</param>
		/// <returns></returns>
		public static IBuilder Put(string url, bool keepAlive = false, bool ignoreCertificateError = true)
		{
			return new BuilderBody(url, HttpMethod.Put, keepAlive, ignoreCertificateError);
		}
		/// <summary>
		/// Delete请求
		/// </summary>
		/// <param name="url">url</param>
		/// <param name="keepAlive">keepAlive</param>
		/// <param name="ignoreCertificateError">忽略证书错误</param>
		/// <returns></returns>
		public static IBuilderNoneBody Delete(string url, bool keepAlive = false, bool ignoreCertificateError = true)
		{
			return new BuilderNoneBody(url, HttpMethod.Delete, keepAlive, ignoreCertificateError);
		}
	}
}
