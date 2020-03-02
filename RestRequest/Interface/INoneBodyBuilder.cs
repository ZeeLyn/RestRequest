using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace RestRequest.Interface
{
    public interface INoneBodyBuilder : IActionCallback
    {

        [Obsolete("Use Download instead")]
        ResponseResult<Stream> ResponseStream(HttpStatusCode succeedStatus = HttpStatusCode.OK);

        ResponseResult<byte[]> Download(HttpStatusCode succeedStatus = HttpStatusCode.OK);

        void Download(Action<ResponseResult<byte[]>> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);


        bool Download(string saveFileName, HttpStatusCode succeedStatus = HttpStatusCode.OK);


        Task DownloadFromBreakPointAsync(string saveFileName, Action<long, long, decimal> onProgressChanged = default, Action onCompleted = default, CancellationToken cancellationToken = default);


        [Obsolete("Use Download instead")]
        Task<ResponseResult<Stream>> ResponseStreamAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK);


        Task<ResponseResult<byte[]>> DownloadAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task<ResponseResult<byte[]>> DownloadAsync(CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK);


        Task DownloadAsync(Action<ResponseResult<byte[]>> action, HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task DownloadAsync(Action<ResponseResult<byte[]>> action, CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task DownloadAsync(string saveFileName, HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task DownloadAsync(string saveFileName, CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK);


        [Obsolete("Use ResponseValue instead")]
        ResponseResult<string> ResponseString(HttpStatusCode succeedStatus = HttpStatusCode.OK);


        [Obsolete("Use ResponseValue instead")]
        Task<ResponseResult<string>> ResponseStringAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK);


        ResponseResult<T> ResponseValue<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task<ResponseResult<T>> ResponseValueAsync<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task<ResponseResult<T>> ResponseValueAsync<T>(CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK);


        void ResponseValue(Action<bool, HttpStatusCode, byte[], string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);

        void ResponseValue(Action<bool, HttpStatusCode, byte[], string, Dictionary<string, string>> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);


        Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string> response, CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string, Dictionary<string, string>> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string, Dictionary<string, string>> response, CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK);


        void ResponseValue<T>(Action<bool, HttpStatusCode, T, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);

        void ResponseValue<T>(Action<bool, HttpStatusCode, T, string, Dictionary<string, string>> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);


        Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string> response, CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string, Dictionary<string, string>> response, HttpStatusCode succeedStatus = HttpStatusCode.OK);

        Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string, Dictionary<string, string>> response, CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK);


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

        INoneBodyBuilder Proxy(string url, int port);

        INoneBodyBuilder Proxy(IWebProxy proxy);
    }
}
