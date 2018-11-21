using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using RestRequest.Interface;

namespace RestRequest.Builder
{
	public partial class ContextBuilder : IBodyBuilder
	{
		public Uri _url { get; }

		public string _contentType { get; private set; } = "application/json";

		public HttpMethod _method { get; }

		public WebHeaderCollection _requestHeaders { get; }

		public IBody _requestBody { get; private set; }

		public Action<HttpStatusCode, Stream> _successAction { get; private set; }

		public HttpStatusCode _succeedStatus { get; private set; }

		public Action<HttpStatusCode?, string> _failAction { get; private set; }

		public bool _ignoreCertificateError { get; private set; }

		public X509CertificateCollection _clientCertificates { get; private set; }

		public string _userAgent { get; private set; }

		public string _referer { get; private set; }

		public int _timeout { get; private set; } = 100000;

		public int _readWriteTimeout { get; private set; } = 300000;

		public List<Cookie> _cookies { get; private set; } = new List<Cookie>();

		public bool _keepAlive { get; private set; } = true;
		public int _connectionLimit { get; private set; } = 100;

		public bool _expect100Continue { get; private set; }

		public bool _pipelined { get; private set; } = true;

		public bool _allowAutoRedirect { get; private set; } = true;

		public Version _protocolVersion { get; private set; } = HttpVersion.Version11;

		public IWebProxy _proxy { get; private set; }

		public int _maxRedirections { get; private set; } = 50;

		public RequestCachePolicy _cachePolicy { get; private set; }

		public ContextBuilder(string url, HttpMethod method)
		{
			_url = new Uri(url);
			_method = method;
			_requestHeaders = new WebHeaderCollection();
		}

		public INoneBodyBuilder Headers(IDictionary<string, string> headers)
		{
			if (headers == null || headers.Count == 0) return this;
			foreach (var item in headers)
				_requestHeaders[item.Key] = item.Value;
			return this;
		}

		public INoneBodyBuilder Headers(object headers)
		{
			if (headers == null)
				return this;
			var properties = headers.ReadProperties();
			foreach (var item in properties)
				_requestHeaders[item.Key] = item.Value.ToString();
			return this;
		}

		public INoneBodyBuilder AddCertificate(string certificateUrl, string certificatePassword)
		{
			if (!File.Exists(certificateUrl))
				throw new FileNotFoundException($"Certificate file not found({certificateUrl})");
			if (_clientCertificates == null)
				_clientCertificates = new X509CertificateCollection();
			_clientCertificates.Add(new X509Certificate(certificateUrl, certificatePassword));
			return this;
		}

		public INoneBodyBuilder AddCertificate(byte[] rawData, string certificatePassword)
		{
			if (_clientCertificates == null)
				_clientCertificates = new X509CertificateCollection();
			_clientCertificates.Add(new X509Certificate(rawData, certificatePassword));
			return this;
		}

		public INoneBodyBuilder AddCertificate(X509Certificate cert)
		{
			if (_clientCertificates == null)
				_clientCertificates = new X509CertificateCollection();
			_clientCertificates.Add(new X509Certificate(cert));
			return this;
		}

		public INoneBodyBuilder IgnoreCertError()
		{
			_ignoreCertificateError = true;
			return this;
		}

		public INoneBodyBuilder KeepAlive(bool enable = true)
		{
			_keepAlive = enable;
			return this;
		}

		public INoneBodyBuilder UserAgent(string userAgent)
		{
			if (!string.IsNullOrWhiteSpace(userAgent))
				_userAgent = userAgent;
			return this;
		}

		public INoneBodyBuilder Referer(string referer)
		{
			if (!string.IsNullOrWhiteSpace(referer))
				_referer = referer;
			return this;
		}

		public INoneBodyBuilder Timeout(int timeout)
		{
			if (timeout > 0)
				_timeout = timeout;
			return this;
		}

		public INoneBodyBuilder Cookies(object cookies)
		{
			if (cookies == null) return this;
			var properties = cookies.ReadProperties();
			foreach (var enumerator in properties)
			{
				_cookies.Add(new Cookie { Name = enumerator.Key, Value = enumerator.Value.ToString(), Domain = _url.Host });
			}
			return this;
		}

		public INoneBodyBuilder Cookies(IDictionary<string, string> cookies)
		{
			if (cookies != null && cookies.Count > 0)
			{
				foreach (var cookie in cookies)
				{
					_cookies.Add(new Cookie { Name = cookie.Key, Value = cookie.Value, Domain = _url.Host });
				}
			}
			return this;
		}

		public INoneBodyBuilder Cookies(IEnumerable<Cookie> cookies)
		{
			if (cookies != null)
				_cookies.AddRange(cookies);
			return this;
		}


		public INoneBodyBuilder ContentType(string contentType)
		{
			if (!string.IsNullOrWhiteSpace(contentType))
				_contentType = contentType;
			return this;
		}

		public INoneBodyBuilder ConnectionLimit(int maxLimit)
		{
			if (maxLimit > 0)
				_connectionLimit = maxLimit;
			return this;
		}

		public INoneBodyBuilder Expect100Continue(bool enable = false)
		{
			_expect100Continue = enable;
			return this;
		}

		public INoneBodyBuilder Pipelined(bool enable = true)
		{
			_pipelined = enable;
			return this;
		}


		public INoneBodyBuilder ReadWriteTimeout(int timeout)
		{
			if (timeout > 0)
				_readWriteTimeout = timeout;
			return this;
		}


		public INoneBodyBuilder ProtocolVersion(string version)
		{
			if (!string.IsNullOrWhiteSpace(version))
				_protocolVersion = new Version(version);
			return this;
		}


		public INoneBodyBuilder AllowAutoRedirect(bool allow = true)
		{
			_allowAutoRedirect = allow;
			return this;
		}


		public INoneBodyBuilder MaxRedirections(int redirections)
		{
			_maxRedirections = redirections;
			return this;
		}


		public INoneBodyBuilder CachePolicy(RequestCachePolicy policy)
		{
			if (policy != null)
				_cachePolicy = policy;
			return this;
		}

		public INoneBodyBuilder Proxy(string url, int port)
		{
			_proxy = new WebProxy(url, port);
			return this;
		}

		public INoneBodyBuilder Proxy(IWebProxy proxy)
		{
			_proxy = proxy;
			return this;
		}
	}
}
