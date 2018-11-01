using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using RestRequest;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTestProject
{
	public class UnitTestRequest
	{
		private ITestOutputHelper output { get; }

		public UnitTestRequest(ITestOutputHelper _output)
		{
			output = _output;
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(1)]
		public void TestGet(int id)
		{
			if (id > 0)
			{
				var result = HttpRequest.Get($"http://localhost:61389/api/values/{id}").ContentType("application/json").Timeout(2000).UserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36").Referer("http://www.baidu.com").Cookies(new { token = "bearer token" }).ResponseValue<int>();
				Assert.True(result.Succeed);
				Assert.Equal(result.Content, id);

				HttpRequest.Get($"http://localhost:61389/api/values/{id}").Response<int>((succeed, status, content,
					error) =>
				{
					Assert.True(succeed);
					Assert.Equal(content, id);

				});
			}
			else
			{
				var result = HttpRequest.Get($"http://localhost:61389/api/values/{id}").ResponseValue<int>();
				Assert.False(result.Succeed);
				Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
			}


		}

		[Theory]
		[InlineData("")]
		[InlineData("jack")]
		public void TestPost(string name)
		{
			var result = HttpRequest.Post("http://localhost:61389/api/values/json").Body(new { name })
				.ResponseValue<TestData>();
			if (!string.IsNullOrWhiteSpace(name))
			{
				Assert.True(result.Succeed);
				Assert.Equal(name, result.Content.Name);
			}
			else
			{
				Assert.False(result.Succeed);
				Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
			}

			var upload = HttpRequest.Post("http://localhost:61389/api/values/upload").Form(new List<NamedFileStream>
			{
				new NamedFileStream
				{
					Stream = File.OpenRead(Directory.GetCurrentDirectory() + "/RestRequest.dll"), Name = "file",
					FileName = "RestRequest.dll"
				}
			}, new { name }).ResponseValue<List<long>>();

			Assert.True(upload.Succeed);
			Assert.Single(upload.Content);


			var upload2 = HttpRequest.Post("http://localhost:61389/api/values/upload").Form(new List<NamedFileStream>
			{
				new NamedFileStream
				{
					Stream = File.OpenRead(Directory.GetCurrentDirectory() + "/RestRequest.dll"), Name = "file",
					FileName = "RestRequest.dll"
				}
			}, new { name }).ContentType("application/json").ResponseValue<List<long>>();
			Assert.False(upload2.Succeed);
			output.WriteLine(upload2.StatusCode.ToString());
			Assert.Equal(HttpStatusCode.UnsupportedMediaType, upload2.StatusCode);
		}
	}
}
