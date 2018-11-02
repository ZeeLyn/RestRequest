using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RestRequest.Interface
{
	public interface INoneBodyBuilder : IActionCallback
	{

		[Obsolete("Use Download instead")]
		ResponseResult<Stream> ResponseStream(HttpStatusCode succeedStatus = HttpStatusCode.OK);

		ResponseResult<byte[]> Download(HttpStatusCode succeedStatus = HttpStatusCode.OK);

		void Download(Action<ResponseResult<byte[]>> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		bool Download(string SaveFileName, HttpStatusCode succeedStatus = HttpStatusCode.OK);


		[Obsolete("Use Download instead")]
		Task<ResponseResult<Stream>> ResponseStreamAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK);


		Task<ResponseResult<byte[]>> DownloadAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK);

		Task DownloadAsync(Action<ResponseResult<byte[]>> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

		Task DownloadAsync(string SaveFileName, HttpStatusCode succeedStatus = HttpStatusCode.OK);


		[Obsolete("Use ResponseValue instead")]
		ResponseResult<string> ResponseString(HttpStatusCode succeedStatus = HttpStatusCode.OK);


		[Obsolete("Use ResponseValue instead")]
		Task<ResponseResult<string>> ResponseStringAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK);


		ResponseResult<T> ResponseValue<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK);

		Task<ResponseResult<T>> ResponseValueAsync<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK);


		void Response(Action<bool, HttpStatusCode, byte[], string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);


		Task ResponseAsync(Action<bool, HttpStatusCode, byte[], string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);



		void Response(Action<bool, HttpStatusCode, string, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);


		Task ResponseAsync(Action<bool, HttpStatusCode, string, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);



		void Response<T>(Action<bool, HttpStatusCode, T, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);


		Task ResponseAsync<T>(Action<bool, HttpStatusCode, T, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);


		INoneBodyBuilder ContentType(string contentType);


		INoneBodyBuilder Headers(IDictionary<string, string> headers);

		INoneBodyBuilder Headers(object headers);


		INoneBodyBuilder AddCertificate(string certificateUrl, string certificatePassword);



		INoneBodyBuilder AddCertificate(byte[] rawData, string certificatePassword);


		INoneBodyBuilder AddCertificate(X509Certificate cert);

		INoneBodyBuilder IgnoreCertError();

		INoneBodyBuilder KeepAlive(bool enable = true);


		INoneBodyBuilder UserAgent(string userAgent);

		INoneBodyBuilder Referer(string referer);


		INoneBodyBuilder Timeout(int timeout);


		INoneBodyBuilder Cookies(object cookies);


		INoneBodyBuilder Cookies(IDictionary<string, string> cookies);


		INoneBodyBuilder Cookies(IEnumerable<Cookie> cookies);


		INoneBodyBuilder ConnectionLimit(int limit);


		INoneBodyBuilder Expect100Continue(bool enable = false);


		INoneBodyBuilder Pipelined(bool enable = true);


		INoneBodyBuilder ReadWriteTimeout(int timeout);

		INoneBodyBuilder ProtocolVersion(string version);

		INoneBodyBuilder AllowAutoRedirect(bool allow = true);

		INoneBodyBuilder MaxRedirections(int redirections);

		INoneBodyBuilder CachePolicy(RequestCachePolicy policy);
	}
}
