using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestRequest.interfaces
{
	public interface IBuilderNoneBody : IActionBackResult
	{
		/// <summary>
		/// 获取响应流
		/// </summary>
		/// <returns></returns>
		ResponseResult<Stream> ResponseStream();
		/// <summary>
		/// 获取响应流
		/// </summary>
		/// <returns></returns>
		Task<ResponseResult<Stream>> ResponseStreamAsync();
		/// <summary>
		/// 获取响应字符串
		/// </summary>
		/// <returns></returns>
		ResponseResult<string> ResponseString();

		/// <summary>
		/// 获取响应字符串
		/// </summary>
		/// <returns></returns>
		Task<ResponseResult<string>> ResponseStringAsync();

		/// <summary>
		/// 获取响应内容，并转换成指定类型
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		ResponseResult<T> ResponseValue<T>();

		/// <summary>
		/// 获取响应内容，并转换成指定类型
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		Task<ResponseResult<T>> ResponseValueAsync<T>();

		/// <summary>
		/// 设置header
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		IBuilderNoneBody Headers(Dictionary<string, string> headers);

		/// <summary>
		/// 设置header
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		IBuilderNoneBody Headers(object headers);

	}
}
