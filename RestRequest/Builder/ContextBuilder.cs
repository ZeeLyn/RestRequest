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
	public partial class ContextBuilder : IBodyBuilder
	{
		public Uri _Url { get; }

		public string _ContentType { get; private set; } = "application/json";

		public HttpMethod _Method { get; }

		public WebHeaderCollection _RequestHeaders { get; }

		public IBody _RequestBody { get; private set; }

		public Action<HttpStatusCode, Stream> _SuccessAction { get; private set; }

		public HttpStatusCode _SucceedStatus { get; private set; }

		public Action<HttpStatusCode?, string> _FailAction { get; private set; }

		public bool _IgnoreCertificateError { get; private set; }

		public X509CertificateCollection _ClientCertificates { get; private set; }

		public string _UserAgent { get; private set; }

		public string _Referer { get; private set; }

		public int _Timeout { get; private set; }

		public List<Cookie> _Cookies { get; private set; } = new List<Cookie>();

		public bool _KeepAlive { get; private set; }
		public int _ConnectionLimit { get; private set; } = 2;

		public ContextBuilder(string url, HttpMethod method)
		{
			_Url = new Uri(url);
			_Method = method;
			_RequestHeaders = new WebHeaderCollection();
		}

		public INoneBodyBuilder Headers(IDictionary<string, string> headers)
		{
			if (headers == null || headers.Count == 0) return this;
			foreach (var item in headers)
				_RequestHeaders[item.Key] = item.Value;
			return this;
		}

		public INoneBodyBuilder Headers(object headers)
		{
			if (headers == null)
				return this;
			var properties = headers.GetType().GetProperties();
			foreach (var item in properties)
				_RequestHeaders[item.Name] = item.GetValue(headers).ToString();
			return this;
		}

		public INoneBodyBuilder AddCertificate(string certificateUrl, string certificatePassword)
		{
			if (!File.Exists(certificateUrl))
				throw new FileNotFoundException($"证书文件不存在{certificateUrl}");
			if (_ClientCertificates == null)
				_ClientCertificates = new X509CertificateCollection();
			_ClientCertificates.Add(new X509Certificate(certificateUrl, certificatePassword));
			return this;
		}

		public INoneBodyBuilder AddCertificate(byte[] rawData, string certificatePassword)
		{
			if (_ClientCertificates == null)
				_ClientCertificates = new X509CertificateCollection();
			_ClientCertificates.Add(new X509Certificate(rawData, certificatePassword));
			return this;
		}

		public INoneBodyBuilder AddCertificate(X509Certificate cert)
		{
			if (_ClientCertificates == null)
				_ClientCertificates = new X509CertificateCollection();
			_ClientCertificates.Add(new X509Certificate(cert));
			return this;
		}

		public INoneBodyBuilder IgnoreCertError()
		{
			_IgnoreCertificateError = true;
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
			if (!string.IsNullOrWhiteSpace(referer))
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
			var properties = cookies.GetType().GetProperties();
			foreach (var enumerator in properties)
			{
				_Cookies.Add(new Cookie { Name = enumerator.Name, Value = enumerator.GetValue(cookies).ToString(), Domain = _Url.Host });
			}
			return this;
		}

		public INoneBodyBuilder Cookies(IDictionary<string, string> cookies)
		{
			if (cookies != null && cookies.Count > 0)
			{
				foreach (var cookie in cookies)
				{
					_Cookies.Add(new Cookie { Name = cookie.Key, Value = cookie.Value, Domain = _Url.Host });
				}
			}
			return this;
		}

		public INoneBodyBuilder Cookies(IEnumerable<Cookie> cookies)
		{
			if (cookies != null)
				_Cookies.AddRange(cookies);
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
