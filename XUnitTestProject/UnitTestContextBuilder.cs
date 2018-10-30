using RestRequest.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using RestRequest;
using RestRequest.Interface;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTestProject
{
	public class UnitTestContextBuilder : IClassFixture<ContextBuilderFixture>
	{
		private ContextBuilder Builder { get; }
		private ITestOutputHelper output { get; }

		public UnitTestContextBuilder(ContextBuilderFixture fixture, ITestOutputHelper _output)
		{
			Builder = fixture.builder;
			output = _output;
		}

		public static IEnumerable<object[]> GetHeaderByDic(string flag)
		{
			if (flag == "null")
			{
				return new List<object[]>
				{
					new object[]
					{
						new Dictionary<string, string> { }
					}
				};
			}
			else
			{
				return new List<object[]>
				{
					new object[]
					{
						new Dictionary<string, string> {{"auth", "bearer token"}}
					}
				};
			}
		}




		public static IEnumerable<object[]> GetHeaderByObj(string flag)
		{
			if (flag == "null")
			{
				return new List<object[]>
				{
					new object[]
					{
						new{}
					}
				};
			}
			else
			{
				return new List<object[]>
				{
					new object[]
					{
						new {tit="title"}
					}
				};
			}
		}

		[Theory]
		[MemberData("GetHeaderByDic", "null")]
		[MemberData("GetHeaderByDic", "1")]
		[InlineData(null)]
		public void TestHeaderByDic(Dictionary<string, string> dic)
		{
			output.WriteLine("-------------------->" + dic?.Count.ToString());
			var count = Builder._RequestHeaders.Count;
			output.WriteLine("-------------------->" + count);
			Builder.Headers(dic);
			output.WriteLine("-------------------->" + Builder._RequestHeaders.Count);
			Assert.Equal(Builder._RequestHeaders.Count, count + (dic?.Count ?? 0));
		}

		[Theory]
		[MemberData("GetHeaderByObj", "null")]
		[MemberData("GetHeaderByObj", "1")]
		[InlineData(null)]
		public void TestHeaderByObj(object obj)
		{
			var properties = obj?.GetType()?.GetProperties();
			var count = Builder._RequestHeaders.Count;
			Builder.Headers(obj);
			Assert.Equal(Builder._RequestHeaders.Count, count + (properties?.Length ?? 0));
		}

		[Fact]
		public void TestIgnoreCertError()
		{
			Builder.IgnoreCertError();
			Assert.True(Builder._IgnoreCertificateError);
		}


		[Fact]
		public void TestKeepAlive()
		{
			Builder.KeepAlive();
			Assert.True(Builder._KeepAlive);
		}

		[Theory]
		[InlineData("")]
		[InlineData("2")]
		public void TestUserAgent(string agent)
		{
			var _agent = Builder._UserAgent;
			Builder.UserAgent(agent);
			Assert.Equal(string.IsNullOrWhiteSpace(agent) ? _agent : agent, Builder._UserAgent);
		}

		[Theory]
		[InlineData("2")]
		[InlineData("")]
		public void TestReferer(string referer)
		{
			var _referer = Builder._Referer;
			Builder.Referer(referer);
			Assert.Equal(string.IsNullOrWhiteSpace(referer) ? _referer : referer, Builder._Referer);
		}

		[Theory]
		[InlineData("application/xml")]
		[InlineData("")]
		public void TestContentType(string contentType)
		{
			var _contentType = Builder._ContentType;
			Builder.ContentType(contentType);
			Assert.Equal(string.IsNullOrWhiteSpace(contentType) ? _contentType : contentType, Builder._ContentType);
		}


		[Theory]
		[InlineData(-1)]
		[InlineData(2)]
		public void TestTimeOut(int timeout)
		{
			if (timeout < 0)
				Assert.Throws<ArgumentException>(() =>
				{
					Builder.Timeout(timeout);
				});
			else
			{
				Builder.Timeout(timeout);
				Assert.Equal(timeout, Builder._Timeout);
			}
		}


		[Theory]
		[MemberData("GetHeaderByDic", "null")]
		[MemberData("GetHeaderByDic", "1")]
		[InlineData(null)]
		public void TestCookiresByDic(Dictionary<string, string> dic)
		{
			output.WriteLine("-------------------->" + dic?.Count.ToString());
			var count = Builder._Cookies.Count;
			output.WriteLine("-------------------->" + count);
			Builder.Cookies(dic);
			output.WriteLine("-------------------->" + Builder._Cookies.Count);
			Assert.Equal(Builder._Cookies.Count, count + (dic?.Count ?? 0));
		}

		[Theory]
		[MemberData("GetHeaderByObj", "1")]
		[MemberData("GetHeaderByObj", "null")]
		[InlineData(null)]
		public void TestCookiesByObj(object obj)
		{
			var properties = obj?.GetType()?.GetProperties();
			var count = Builder._Cookies.Count;
			Builder.Cookies(obj);
			Assert.Equal(Builder._Cookies.Count, count + (properties?.Length ?? 0));
		}


		public static IEnumerable<object[]> GetCookies(int count)
		{
			var cookies = new List<object[]>();
			var c = new List<Cookie>();
			for (var i = 1; i <= count; i++)
			{
				c.Add(new Cookie
				{
					Name = Guid.NewGuid().ToString(),
					Value = Guid.NewGuid().ToString()
				});

			}
			cookies.Add(new object[] { c });
			return cookies;
		}

		[Theory]
		[MemberData("GetCookies", 0)]
		[MemberData("GetCookies", 4)]
		[InlineData(null)]
		public void TestCookiesByCookie(IEnumerable<Cookie> cookies)
		{
			var count = Builder._Cookies.Count;
			Builder.Cookies(cookies);
			Assert.Equal(Builder._Cookies.Count, count + (cookies?.Count() ?? 0));
		}

		[Theory]
		[InlineData(0)]
		[InlineData(20)]
		public void TestConnectionLimit(int limit)
		{
			if (limit < 1)
				Assert.Throws<ArgumentException>(() => { Builder.ConnectionLimit(limit); });
			else
			{
				Builder.ConnectionLimit(limit);
				Assert.Equal(limit, Builder._ConnectionLimit);
			}
		}

		[Theory]
		[MemberData("GetHeaderByObj", "null")]
		[MemberData("GetHeaderByObj", "1")]
		[InlineData(null)]
		public void TestBody(object pars)
		{
			Assert.IsAssignableFrom<INoneBodyBuilder>(Builder.Body(pars));
		}

		[Theory]
		[MemberData("GetHeaderByObj", "null")]
		[MemberData("GetHeaderByObj", "1")]
		[InlineData(null)]
		public void TestFormByObj(object pars)
		{
			Assert.IsAssignableFrom<INoneBodyBuilder>(Builder.Form(pars));
		}



		public static IEnumerable<object[]> GetDataByDic(string flag)
		{
			if (flag == "null")
			{
				return new List<object[]>
				{
					new object[]
					{
						new Dictionary<string, object> { }
					}
				};
			}
			else
			{
				return new List<object[]>
				{
					new object[]
					{
						new Dictionary<string, object> {{"auth", "bearer token"}}
					}
				};
			}
		}

		[Theory]
		[MemberData("GetDataByDic", "null")]
		[MemberData("GetDataByDic", "1")]
		[InlineData(null)]
		public void TestFormByDic(Dictionary<string, object> pars)
		{
			Assert.IsAssignableFrom<INoneBodyBuilder>(Builder.Form(pars));
		}


		public static IEnumerable<object[]> GetFilesByDic(string flag)
		{
			if (flag == "null")
			{
				return new List<object[]>
				{

					new object[]
					{
						new List<NamedFileStream>{},
					}
				};
			}
			else
			{
				return new List<object[]>
				{
					new object[]
					{
						new List<NamedFileStream>{
							new NamedFileStream{
								Stream=File.OpenRead(Directory.GetCurrentDirectory() + "/RestRequest.dll")
							}
						}
					}
				};
			}
		}

		[Theory]
		[MemberData("GetFilesByDic", "null")]
		[MemberData("GetFilesByDic", "1")]
		[InlineData(null)]
		public void TestUploadByDic(IEnumerable<NamedFileStream> files)
		{
			Assert.IsAssignableFrom<INoneBodyBuilder>(Builder.Form(files));
		}

		public static IEnumerable<object[]> GetFilesAndParamByDic(string flag)
		{
			if (flag == "null")
			{
				return new List<object[]>
				{

					new object[]
					{
						new List<NamedFileStream>{},
						new Dictionary<string,object>()
					}
				};
			}
			else
			{
				return new List<object[]>
				{
					new object[]
					{
						new List<NamedFileStream>{
							new NamedFileStream{
								FileName = "RestRequest.dll",
								Name = "files",
								Stream=File.OpenRead(Directory.GetCurrentDirectory() + "/RestRequest.dll")
							}
						},
						new Dictionary<string,object>{{"id",1}}
					}
				};
			}
		}

		[Theory]
		[MemberData("GetFilesAndParamByDic", "null")]
		[MemberData("GetFilesAndParamByDic", "1")]
		[InlineData(null, null)]
		public void TestUploadAndParamByDic(IEnumerable<NamedFileStream> files, Dictionary<string, object> param)
		{
			Assert.IsAssignableFrom<INoneBodyBuilder>(Builder.Form(files, param));
		}


		public static IEnumerable<object[]> GetFilesAndParamByObj(string flag)
		{
			if (flag == "null")
			{
				return new List<object[]>
				{

					new object[]
					{
						new List<NamedFileStream>{},
						new{}
					}
				};
			}
			else
			{
				return new List<object[]>
				{
					new object[]
					{
						new List<NamedFileStream>{
							new NamedFileStream{
								FileName = "RestRequest.dll",
								Name = "files",
								Stream=File.OpenRead(Directory.GetCurrentDirectory() + "/RestRequest.dll")
							}
						},
						new {id=1}
					}
				};
			}
		}

		[Theory]
		[MemberData("GetFilesAndParamByObj", "null")]
		[MemberData("GetFilesAndParamByObj", "1")]
		[InlineData(null, null)]
		public void TestUploadAndParamByObj(IEnumerable<NamedFileStream> files, object param)
		{
			Assert.IsAssignableFrom<INoneBodyBuilder>(Builder.Form(files, param));
		}


		[Theory]
		[InlineData("")]
		[InlineData("textdata")]
		public void TestTextForm(string text)
		{
			Assert.IsAssignableFrom<INoneBodyBuilder>(Builder.Form(text));
		}



		public static IEnumerable<object[]> GetStreamData()
		{

			return new List<object[]>
				{
					new object[]
					{
						File.OpenRead(Directory.GetCurrentDirectory() + "/RestRequest.dll")
					}
				};
		}

		[Theory]
		[InlineData(null)]
		[MemberData("GetStreamData")]
		public void TestStreamForm(Stream stream)
		{
			Assert.IsAssignableFrom<INoneBodyBuilder>(Builder.Form(stream));
		}
	}
}
