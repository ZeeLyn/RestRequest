using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using RestRequest.Interface;

namespace RestRequest.Builder
{
	internal partial class ContextBuilder : IBodyBuilder
	{
		internal Uri Url { get; }

		internal string _ContentType { get; set; } = "application/json";

		internal HttpMethod Method { get; }

		internal WebHeaderCollection RequestHeaders { get; }

		internal IBody RequestBody { get; set; }

		internal Action<HttpStatusCode, Stream> SuccessAction { get; set; }

		internal HttpStatusCode SucceedStatus { get; set; }

		internal Action<HttpStatusCode?, string> FailAction { get; set; }

		internal bool IgnoreCertificateError { get; set; }

		internal X509CertificateCollection ClientCertificates { get; set; }

		internal string _UserAgent { get; set; }

		internal string _Referer { get; set; }

		internal int _Timeout { get; set; }

		internal List<Cookie> _Cookies { get; set; }

		internal bool _KeepAlive { get; set; }
		internal int _ConnectionLimit { get; set; } = 2;

		internal ContextBuilder(string url, HttpMethod method)
		{
			Url = new Uri(url);
			Method = method;
			RequestHeaders = new WebHeaderCollection();
		}

		public INoneBodyBuilder Headers(IDictionary<string, string> headers)
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
				RequestHeaders[item.Name] = item.GetValue(headers).ToString();
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
			_KeepAlive = true;
			return this;
		}

		public INoneBodyBuilder UserAgent(string userAgent)
		{
			if (!string.IsNullOrWhiteSpace(userAgent))
				_UserAgent = userAgent;
			return this;
		}

		public INoneBodyBuilder Referer(string referer)
		{
			_Referer = referer;
			return this;
		}

		public INoneBodyBuilder Timeout(int timeout)
		{
			if (timeout < 0)
				throw new ArgumentException("Timeout cannot be less than 0.");
			_Timeout = timeout;
			return this;
		}

		public INoneBodyBuilder Cookies(object cookies)
		{
			if (cookies == null) return this;
			_Cookies = new List<Cookie>();
			var properties = cookies.GetType().GetProperties();
			foreach (var enumerator in properties)
			{
				_Cookies.Add(new Cookie { Name = enumerator.Name, Value = enumerator.GetValue(cookies).ToString(), Domain = Url.Host });
			}
			return this;
		}

		public INoneBodyBuilder Cookies(IDictionary<string, string> cookies)
		{
			if (cookies != null && cookies.Count > 0)
			{
				_Cookies = new List<Cookie>();
				foreach (var cookie in cookies)
				{
					_Cookies.Add(new Cookie { Name = cookie.Key, Value = cookie.Value, Domain = Url.Host });
				}
			}
			return this;
		}

		public INoneBodyBuilder Cookies(IEnumerable<Cookie> cookies)
		{
			_Cookies = cookies?.ToList();
			return this;
		}

		public INoneBodyBuilder ContentType(string contentType)
		{
			if (!string.IsNullOrWhiteSpace(contentType))
				_ContentType = contentType;
			return this;
		}

		public INoneBodyBuilder ConnectionLimit(int maxLimit)
		{
			if (maxLimit < 1)
				throw new ArgumentException("The connection limit is equal to or less than 0.");
			_ConnectionLimit = maxLimit;
			return this;
		}

	}
}
