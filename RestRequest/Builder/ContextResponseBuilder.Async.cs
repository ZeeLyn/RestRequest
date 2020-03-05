using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RestRequest.Interface;

namespace RestRequest.Builder
{
    public partial class ContextBuilder
    {
        private async Task<(bool Succeed, HttpStatusCode StatusCode, byte[] ResponseBytes, string FailMessage, Dictionary<string, string> Headers)> ExecuteRequestAsync(CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var result = await ExecuteRequestReturnResponseAsync(cancellationToken, succeedStatus);
            using (result.Response)
            using (var contentStream = result.Response.GetResponseStream())
            {
                var headers = result.Response.Headers.AllKeys.ToDictionary(key => key, key => result.Response.Headers.Get(key));
                var bytes = await contentStream.AsBytesAsync(cancellationToken);
                return (result.Response.StatusCode == succeedStatus, result.Response.StatusCode,
                    succeedStatus == result.Response.StatusCode ? bytes : null,
                    succeedStatus == result.Response.StatusCode ? "" : bytes.AsString(), headers);
            }
        }

        private async Task<(bool Succeed, HttpWebResponse Response)> ExecuteRequestReturnResponseAsync(CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            using (var builder = new RequestBuilder(this))
            {
                builder.BuildRequest();
                await builder.WriteRequestBodyAsync(cancellationToken);
                var response = await builder.GetResponseAsync();
                var headers = response.Headers.AllKeys.ToDictionary(key => key, key => response.Headers.Get(key));
                return (response.StatusCode == succeedStatus, response);
            }
        }


        #region Async request

        public IActionCallback OnSuccess(Action<HttpStatusCode, byte[]> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            _successAction = (statusCode, stream) =>
            {
                action(statusCode, stream.AsBytes());
            };
            _succeedStatus = succeedStatus;
            return this;
        }


        public IActionCallback OnSuccess<T>(Action<HttpStatusCode, T> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            _successAction = (statusCode, stream) =>
            {
                action(statusCode, stream.AsBytes().AsString().JsonTo<T>());
            };
            _succeedStatus = succeedStatus;
            return this;
        }

        public IActionCallback OnFail(Action<HttpStatusCode, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            _failAction = action;
            _succeedStatus = succeedStatus;
            return this;
        }

        public void Start()
        {
            var builder = new RequestBuilder(this);
            builder.BuildRequest();
            builder.BuildCallback();
        }

        #endregion

        #region Download
        public async Task<ResponseResult<byte[]>> DownloadAsync(HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
            return new ResponseResult<byte[]>
            {
                Succeed = res.StatusCode == succeedStatus,
                StatusCode = res.StatusCode,
                Content = res.ResponseBytes,
                FailMessage = res.FailMessage,
                Headers = res.Headers
            };
        }

        public async Task<ResponseResult<byte[]>> DownloadAsync(CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
            return new ResponseResult<byte[]>
            {
                Succeed = res.StatusCode == succeedStatus,
                StatusCode = res.StatusCode,
                Content = res.ResponseBytes,
                FailMessage = res.FailMessage,
                Headers = res.Headers
            };
        }


        public async Task DownloadAsync(Action<ResponseResult<byte[]>> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            action?.Invoke(await DownloadAsync(succeedStatus));
        }

        public async Task DownloadAsync(Action<ResponseResult<byte[]>> action, CancellationToken cancellationToken,
            HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            action?.Invoke(await DownloadAsync(cancellationToken, succeedStatus));
        }


        public async Task DownloadAsync(string saveFileName, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
            if (res.Succeed)
                res.ResponseBytes.SaveAs(saveFileName);
        }

        public async Task DownloadAsync(string saveFileName, CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
            if (res.Succeed)
                res.ResponseBytes.SaveAs(saveFileName);
        }

        public async Task DownloadFromBreakPoint(string saveFileName, Action<long, long, decimal> onProgressChanged = default, Action onCompleted = default, Action<string> onError = default, Action<long, long> onAborted = default, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(saveFileName))
                throw new ArgumentNullException(nameof(saveFileName));

            var check = await GetHttpLength(_url);
            if (!check.Succeed)
            {
                onError?.Invoke(check.ErrorMessage);
                return;
            }

            var totalLength = check.Length;
            FileStream fileStream = null;
            long len = 0;
            try
            {
                if (File.Exists(saveFileName))
                {
                    fileStream = File.OpenWrite(saveFileName);
                    fileStream.Seek(fileStream.Length, SeekOrigin.Current);
                    len = _range = fileStream.Length;
                    if (totalLength == len)
                    {
                        onCompleted?.Invoke();
                        return;
                    }
                }
                else
                {
                    var dir = Path.GetDirectoryName(saveFileName);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    fileStream = new FileStream(saveFileName, FileMode.Create);
                }

                var res = await ExecuteRequestReturnResponseAsync(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    onAborted?.Invoke(len, totalLength);
                    return;
                }
                var buffer = new byte[1024];
                using (res.Response)
                using (var stream = res.Response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        while (true)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                onAborted?.Invoke(len, totalLength);
                                return;
                            }
                            var size = await stream.ReadAsync(buffer, 0, 1024, cancellationToken);
                            if (cancellationToken.IsCancellationRequested)
                            {
                                onAborted?.Invoke(len, totalLength);
                                return;
                            }
                            len += size;
                            var p = Math.Round((decimal)len / totalLength * 100, 3);
                            if (size == 0)
                                break;
                            await fileStream.WriteAsync(buffer, 0, size, cancellationToken);
                            onProgressChanged?.Invoke(totalLength, len, p);
                            if (cancellationToken.IsCancellationRequested)
                            {
                                onAborted?.Invoke(len, totalLength);
                                return;
                            }
                        }
                    }
                    onCompleted?.Invoke();
                }
            }
            catch (Exception e)
            {
                onError?.Invoke(e.Message);
            }
            finally
            {
                fileStream?.Dispose();
            }
        }

        public async Task DownloadFromBreakPointAsync(string saveFileName, Action<long, long, decimal> onProgressChanged = default, Action onCompleted = default, Action<string> onError = default, Action<long, long> onAborted = default, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(saveFileName))
                throw new ArgumentNullException(nameof(saveFileName));

            var check = await GetHttpLength(_url);
            if (!check.Succeed)
            {
                onError?.Invoke(check.ErrorMessage);
                return;
            }

            try
            {
                FileStream fileStream;
                if (File.Exists(saveFileName))
                {
                    fileStream = File.OpenWrite(saveFileName);
                    fileStream.Seek(fileStream.Length, SeekOrigin.Current);
                    var len = _range = fileStream.Length;
                    if (check.Length == len)
                    {
                        onCompleted?.Invoke();
                        return;
                    }
                }
                else
                {
                    var dir = Path.GetDirectoryName(saveFileName);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    fileStream = new FileStream(saveFileName, FileMode.Create);
                }

                var builder = new RequestBuilder(this);
                builder.BuildRequest();
                await builder.WriteRequestBodyAsync(cancellationToken);

                builder.Request.BeginGetResponse(async asyncResult =>
                {
                    var asyncState = (DownloadAsyncState)asyncResult.AsyncState;
                    var len = asyncState.FileStream.Length;
                    try
                    {
                        var buffer = new byte[1024];
                        var response = (HttpWebResponse)asyncState.Request.EndGetResponse(asyncResult);
                        using (response)
                        using (asyncState.FileStream)
                        using (var stream = response.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                while (true)
                                {
                                    if (asyncState.CancellationToken.IsCancellationRequested)
                                    {
                                        asyncState.OnAborted?.Invoke(len, asyncState.TotalLength);
                                        return;
                                    }
                                    var size = await stream.ReadAsync(buffer, 0, 1024, asyncState.CancellationToken);
                                    if (asyncState.CancellationToken.IsCancellationRequested)
                                    {
                                        asyncState.OnAborted?.Invoke(len, asyncState.TotalLength);
                                        return;
                                    }
                                    len += size;
                                    var p = Math.Round((decimal)len / asyncState.TotalLength * 100, 3);
                                    if (size == 0)
                                        break;
                                    await asyncState.FileStream.WriteAsync(buffer, 0, size, asyncState.CancellationToken);
                                    asyncState.OnProgressChanged?.Invoke(asyncState.TotalLength, len, p);
                                    if (asyncState.CancellationToken.IsCancellationRequested)
                                    {
                                        asyncState.OnAborted?.Invoke(len, asyncState.TotalLength);
                                        return;
                                    }
                                }
                            }
                            asyncState.OnCompleted?.Invoke();
                        }
                    }
                    catch (Exception e)
                    {
                        asyncState.OnError?.Invoke(e.Message);
                    }
                    finally
                    {
                        asyncState.Request.Abort();
                    }

                }, new DownloadAsyncState
                {
                    Request = builder.Request,
                    SaveFileName = saveFileName,
                    OnProgressChanged = onProgressChanged,
                    OnCompleted = onCompleted,
                    OnError = onError,
                    OnAborted = onAborted,
                    TotalLength = check.Length,
                    FileStream = fileStream,
                    CancellationToken = cancellationToken
                });
            }
            catch (Exception e)
            {
                onError?.Invoke(e.Message);
            }
        }


        async Task<(bool Succeed, long Length, string ErrorMessage)> GetHttpLength(Uri uri)
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.CreateDefault(uri);
                req.Method = "HEAD";
                req.Timeout = 5000;
                using (var res = (HttpWebResponse)await req.GetResponseAsync())
                {
                    return res.StatusCode == HttpStatusCode.OK ? (true, res.ContentLength, "") : (false, 0, $"Failed to check file size, status code is {res.StatusCode}.");
                }
            }
            catch (WebException e)
            {
                return (false, 0, e.Message);
            }
        }

        #endregion


        public async Task<ResponseResult<T>> ResponseValueAsync<T>(HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
            return new ResponseResult<T>
            {
                Succeed = res.Succeed,
                StatusCode = res.StatusCode,
                Content = res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default,
                FailMessage = res.FailMessage,
                Headers = res.Headers
            };
        }

        public async Task<ResponseResult<T>> ResponseValueAsync<T>(CancellationToken cancellationToken, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
            return new ResponseResult<T>
            {
                Succeed = res.Succeed,
                StatusCode = res.StatusCode,
                Content = res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default,
                FailMessage = res.FailMessage,
                Headers = res.Headers
            };
        }


        public async Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
            response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage);
        }

        public async Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string> response, CancellationToken cancellationToken,
            HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
            response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage);
        }

        public async Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string, Dictionary<string, string>> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
            response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage, res.Headers);
        }

        public async Task ResponseValueAsync(Action<bool, HttpStatusCode, byte[], string, Dictionary<string, string>> response, CancellationToken cancellationToken,
            HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
            response?.Invoke(res.Succeed, res.StatusCode, res.ResponseBytes, res.FailMessage, res.Headers);
        }

        public async Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
            response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default, res.FailMessage);
        }


        public async Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string> response, CancellationToken cancellationToken,
            HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
            response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default, res.FailMessage);
        }

        public async Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string, Dictionary<string, string>> response, HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(CancellationToken.None, succeedStatus);
            response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default, res.FailMessage, res.Headers);
        }

        public async Task ResponseValueAsync<T>(Action<bool, HttpStatusCode, T, string, Dictionary<string, string>> response, CancellationToken cancellationToken,
            HttpStatusCode succeedStatus = HttpStatusCode.OK)
        {
            var res = await ExecuteRequestAsync(cancellationToken, succeedStatus);
            response?.Invoke(res.Succeed, res.StatusCode, res.Succeed ? res.ResponseBytes.AsString().JsonTo<T>() : default, res.FailMessage, res.Headers);
        }
    }
}
