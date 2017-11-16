using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestRequest.Provider
{
	internal class BuilderRequestAsync
	{
		private HttpWebRequest Request { get; set; }

		private BuilderBase Builder { get; }

		internal BuilderRequestAsync(BuilderBase builder)
		{
			Builder = builder;
		}

		internal void CreateRequest()
		{
			Request = (HttpWebRequest)WebRequest.Create(Builder.Url);
			Request.Method = Builder.Method.ToString().ToUpper();
			if (Builder.RequestHeaders != null && Builder.RequestHeaders.Count > 0)
				Request.Headers = Builder.RequestHeaders;
			if (Builder.RequestBody != null)
				Request.ContentType = Builder.RequestBody.GetContentType();
			if (Builder.IgnoreCertificateError)
				Request.ServerCertificateValidationCallback = ValidationCertificate.VerifyServerCertificate;
			if (Builder.ClientCertificates != null && Builder.ClientCertificates.Count > 0)
				Request.ClientCertificates.AddRange(Builder.ClientCertificates);
		}

		internal async Task BuildRequestAsync()
		{
			if (Builder.RequestBody == null)
				return;
			using (var bodyStream = Builder.RequestBody.GetBody())
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

		public void Dispose()
		{
			Request?.Abort();
		}
	}
}
