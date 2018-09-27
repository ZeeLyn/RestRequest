using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RestRequest.Interface
{
	public interface INoneBodyBuilder : IActionCallback
	{
		/// <summary>
		/// 获取响应流
		/// </summary>
		/// <returns></returns>
		ResponseResult<Stream> ResponseStream(HttpStatusCode succeedStatus = HttpStatusCode.OK);
		/// <summary>
		/// 获取响应流
		/// </summary>
		/// <returns></returns>
		Task<ResponseResult<Stream>> ResponseStreamAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK);
		/// <summary>
		/// 获取响应字符串
		/// </summary>
		/// <returns></returns>
		ResponseResult<string> ResponseString(HttpStatusCode succeedStatus = HttpStatusCode.OK);

		/// <summary>
		/// 获取响应字符串
		/// </summary>
		/// <returns></returns>
		Task<ResponseResult<string>> ResponseStringAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK);

		/// <summary>
		/// 获取响应内容，并转换成指定类型
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		ResponseResult<T> ResponseValue<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK) where T : class;

		/// <summary>
		/// 获取响应内容，并转换成指定类型
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		Task<ResponseResult<T>> ResponseValueAsync<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK) where T : class;

		/// <summary>
		/// 设置Content-Type,不支持multipart/form-data的自定义
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		INoneBodyBuilder ContentType(string contentType);

		/// <summary>
		/// 设置header
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		INoneBodyBuilder Headers(Dictionary<string, string> headers);

		/// <summary>
		/// 设置header
		/// </summary>
		/// <param name="headers"></param>
		/// <returns></returns>
		INoneBodyBuilder Headers(object headers);

		/// <summary>
		/// 添加证书
		/// </summary>
		/// <param name="certificateUrl"></param>
		/// <param name="certificatePassword"></param>
		/// <returns></returns>
		INoneBodyBuilder AddCertificate(string certificateUrl, string certificatePassword);


		/// <summary>
		/// 添加证书
		/// </summary>
		/// <param name="rawData"></param>
		/// <param name="certificatePassword"></param>
		/// <returns></returns>
		INoneBodyBuilder AddCertificate(byte[] rawData, string certificatePassword);

		/// <summary>
		/// 添加证书
		/// </summary>
		/// <param name="cert"></param>
		/// <returns></returns>
		INoneBodyBuilder AddCertificate(X509Certificate cert);

		/// <summary>
		/// 忽略证书错误
		/// </summary>
		/// <returns></returns>
		INoneBodyBuilder IgnoreCertError();

		INoneBodyBuilder KeepAlive();

		/// <summary>
		/// 设置UserAgent
		/// </summary>
		/// <param name="userAgent"></param>
		/// <returns></returns>
		INoneBodyBuilder UserAgent(string userAgent);

		/// <summary>
		/// 设置超时时间（单位毫秒）
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns></returns>
		INoneBodyBuilder Timeout(int timeout);

		/// <summary>
		/// 设置cookie
		/// </summary>
		/// <param name="cookies"></param>
		/// <returns></returns>
		INoneBodyBuilder Cookies(object cookies);

		/// <summary>
		/// 设置cookie
		/// </summary>
		/// <param name="cookies"></param>
		/// <returns></returns>
		INoneBodyBuilder Cookies(Dictionary<string, string> cookies);

		/// <summary>
		/// 设置cookie
		/// </summary>
		/// <param name="cookies"></param>
		/// <returns></returns>
		INoneBodyBuilder Cookies(IEnumerable<Cookie> cookies);
	}
}
