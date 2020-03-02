using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RestRequest.Builder
{
    internal partial class RequestBuilder
    {
        #region Async callback
        internal void BuildCallback()
        {
            if (Context._requestBody != null && (Context._method == HttpMethod.Post || Context._method == HttpMethod.Put))
                Request.BeginGetRequestStream(asyncResult =>
                {
                    var request = (HttpWebRequest)asyncResult.AsyncState;
                    using (var requestStream = request.EndGetRequestStream(asyncResult))
                    {
                        var bytes = Context._requestBody.GetBody();
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }
                    request.BeginGetResponse(GetResponseCallback, request);
                }, Request);
            else
                Request.BeginGetResponse(GetResponseCallback, Request);
        }

        private void GetResponseCallback(IAsyncResult asyncResult)
        {
            try
            {
                var webRequest = (HttpWebRequest)asyncResult.AsyncState;
                HttpWebResponse response;
                try
                {
                    response = (HttpWebResponse)webRequest.EndGetResponse(asyncResult);
                }
                catch (WebException ex)
                {
                    response = (HttpWebResponse)ex.Response;
                    if (response == null)
                        throw;
                }

                using (response)
                {
                    if (Context._succeedStatus == response.StatusCode)
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            Context._successAction?.Invoke(response.StatusCode, stream);
                        }
                    }
                    else
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    Context._failAction?.Invoke(response.StatusCode, reader.ReadToEnd());
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                Dispose();
            }
        }
        #endregion

        internal async Task WriteRequestBodyAsync(CancellationToken cancellationToken)
        {
            var bodyBytes = Context._requestBody?.GetBody();
            if (bodyBytes == null)
                return;
            Request.ContentLength = bodyBytes.Length;
            using (var requestStream = await Request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(bodyBytes, 0, bodyBytes.Length, cancellationToken);
            }
        }


        internal async Task<HttpWebResponse> GetResponseAsync()
        {
            try
            {
                return (HttpWebResponse)await Request.GetResponseAsync();
            }
            catch (WebException e)
            {
                if (e.Response is HttpWebResponse response)
                    return response;
                throw;
            }
        }
    }
}
