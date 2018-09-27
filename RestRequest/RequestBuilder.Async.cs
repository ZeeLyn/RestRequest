using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestRequest
{
	internal partial class RequestBuilder
	{
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
