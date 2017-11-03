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

		/// <summary>
		/// 下载返回Stream，只支持Get方式
		/// </summary>
		Stream DownloadStream();
	}
}
