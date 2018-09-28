using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using RestRequest.Body;
using RestRequest.Interface;

namespace RestRequest.Builder
{
	public class NoneBodyBuilder : BaseBuilder, INoneBodyBuilder
	{
		public NoneBodyBuilder(string url, HttpMethod method) : base(url, method)
		{
			//RequestBody = new DefaultBody();
		}


		public IActionCallback OnSuccess(Action<HttpStatusCode, Stream> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			SuccessAction = action;
			SucceedStatus = succeedStatus;
			return this;
		}

		public IActionCallback OnSuccess(Action<HttpStatusCode, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			SuccessAction = (statusCode, stream) =>
			{
				using (var reader = new StreamReader(stream))
				{
					action(statusCode, reader.ReadToEnd());
				}
			};
			SucceedStatus = succeedStatus;
			return this;
		}

		public IActionCallback OnSuccess<T>(Action<HttpStatusCode, T> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			SuccessAction = (statusCode, stream) =>
			{
				using (var reader = new StreamReader(stream))
				{
					action(statusCode, JsonConvert.DeserializeObject<T>(reader.ReadToEnd()));
				}
			};
			SucceedStatus = succeedStatus;
			return this;
		}

		public IActionCallback OnFail(Action<HttpStatusCode?, string> action, HttpStatusCode succeedStatus = HttpStatusCode.OK)
		{
			FailAction = action;
			SucceedStatus = succeedStatus;
			return this;
		}


		public INoneBodyBuilder Headers(Dictionary<string, string> headers)
		{
			if (!(headers?.Count > 0)) return this;
			foreach (var item in headers)
				RequestHeaders[item.Key] = item.Value;
			return this;
		}

		public INoneBodyBuilder Headers(object headers)
		{
			if (headers == null)
				return this;
			var properties = headers.GetType().GetProperties();
			foreach (var item in properties)
			{
				RequestHeaders[item.Name] = item.GetValue(headers).ToString();
			}
			return this;
		}

		public INoneBodyBuilder AddCertificate(string certificateUrl, string certificatePassword)
		{
			if (!File.Exists(certificateUrl))
				throw new FileNotFoundException($"证书文件不存在{certificateUrl}");
			if (ClientCertificates == null)
				ClientCertificates = new X509CertificateCollection();
			ClientCertificates.Add(new X509Certificate(certificateUrl, certificatePassword));
			return this;
		}

		public INoneBodyBuilder AddCertificate(byte[] rawData, string certificatePassword)
		{
			if (ClientCertificates == null)
				ClientCertificates = new X509CertificateCollection();
			ClientCertificates.Add(new X509Certificate(rawData, certificatePassword));
			return this;
		}

		public INoneBodyBuilder AddCertificate(X509Certificate cert)
		{
			if (ClientCertificates == null)
				ClientCertificates = new X509CertificateCollection();
			ClientCertificates.Add(new X509Certificate(cert));
			return this;
		}

		public INoneBodyBuilder IgnoreCertError()
		{
			IgnoreCertificateError = true;
			return this;
		}

		public INoneBodyBuilder KeepAlive()
		{
			base.KeepAlive = true;
			return this;
		}

		public INoneBodyBuilder UserAgent(string userAgent)
		{
			if (!string.IsNullOrWhiteSpace(userAgent))
				base.UserAgent = userAgent;
			return this;
		}

		public INoneBodyBuilder Timeout(int timeout)
		{
			if (timeout <= 0)
				throw new ArgumentOutOfRangeException("超时时间必须大于0");
			base.Timeout = timeout;
			return this;
		}


		public INoneBodyBuilder Cookies(object cookies)
		{
			if (cookies == null) return this;
			base.Cookies = new List<Cookie>();
			var properties = cookies.GetType().GetProperties();
			foreach (var enumerator in properties)
			{
				base.Cookies.Add(new Cookie { Name = enumerator.Name, Value = enumerator.GetValue(cookies).ToString(), Domain = Url.Host });
			}
			return this;
		}

		public INoneBodyBuilder Cookies(Dictionary<string, string> cookies)
		{
			if (cookies != null && cookies.Count > 0)
			{
				base.Cookies = new List<Cookie>();
				foreach (var cookie in cookies)
				{
					base.Cookies.Add(new Cookie { Name = cookie.Key, Value = cookie.Value, Domain = Url.Host });
				}
			}
			return this;
		}

		public INoneBodyBuilder Cookies(IEnumerable<Cookie> cookies)
		{
			base.Cookies = cookies?.ToList();
			return this;
		}

		public INoneBodyBuilder ContentType(string contentType)
		{
			if (!string.IsNullOrWhiteSpace(contentType))
				base.ContentType = contentType;
			return this;
		}

	}
}