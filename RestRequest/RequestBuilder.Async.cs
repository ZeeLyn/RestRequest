using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestRequest
{
	internal partial class RequestBuilder
	{
		#region async callback
		internal void BuildCallback()
		{

			if (Builder.RequestBody != null && (Builder.Method == HttpMethod.Post || Builder.Method == HttpMethod.Put))
				Request.BeginGetRequestStream(GetRequestStreamCallback, Request);
			else
				Request.BeginGetResponse(GetResponseCallback, Request);
		}

		private void GetRequestStreamCallback(IAsyncResult asyncResult)
		{
			var request = (HttpWebRequest)asyncResult.AsyncState;
			using (var bodyStream = Builder.RequestBody.GetBody())
			{
				Request.ContentLength = bodyStream.Length;
				using (var requestStream = request.EndGetRequestStream(asyncResult))
				{
					var bytes = new byte[bodyStream.Length];
					bodyStream.Read(bytes, 0, bytes.Length);
					requestStream.Write(bytes, 0, bytes.Length);
				}
			}
			request.BeginGetResponse(GetResponseCallback, Request);
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
					{
						Builder.FailAction?.Invoke(null, ex.Message);
					}
				}

				if (response != null)
				{
					using (response)
					{
						if (Builder.SucceedStatus == response.StatusCode)
						{
							using (var stream = response.GetResponseStream())
							{
								Builder.SuccessAction?.Invoke(response.StatusCode, stream);
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
										Builder.FailAction?.Invoke(response.StatusCode, reader.ReadToEnd());
									}
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

		internal async Task WriteRequestBodyAsync()
		{
			var bodyStream = Builder.RequestBody?.GetBody();
			if (bodyStream == null)
				return;
			using (bodyStream)
			{
				Request.ContentLength = bodyStream.Length;
				using (var requestStream = await Request.GetRequestStreamAsync())
				{
					bodyStream.Position = 0;
					var bytes = new byte[bodyStream.Length];
					await bodyStream.ReadAsync(bytes, 0, bytes.Length);
					await requestStream.WriteAsync(bytes, 0, bytes.Length);
				}
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
