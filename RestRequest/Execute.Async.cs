using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestRequest
{
	internal partial class Execute
	{
		internal async Task BuildRequestAsync()
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


		internal async Task<(bool Success, HttpStatusCode StatusCode, Stream ResponseContent, HttpWebResponse Response)> GetResponseAsync()
		{
			try
			{
				var response = (HttpWebResponse)await Request.GetResponseAsync();
				return (true, response.StatusCode, response.GetResponseStream(), response);
			}
			catch (WebException e)
			{
				if (e.Response == null)
					return (false, HttpStatusCode.InternalServerError, new MemoryStream(Encoding.UTF8.GetBytes(e.Message)), null);
				var badRes = (HttpWebResponse)e.Response;
				return (false, badRes.StatusCode, new MemoryStream(Encoding.UTF8.GetBytes(e.Message)), badRes);
			}
		}
	}
}
