using RestRequest.Body;
using RestRequest.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using System.Text;
using RestRequest;

namespace XUnitTestProject
{
	public class UnitTestBody
	{
		[Fact]
		public void TestFormBody()
		{
			FormBody body = new FormBody();
			Assert.Null(body.GetBody());
			body.AddParameter(new { id = 1, name = "name" });
			object p = null;
			body.AddParameter(p);
			body.AddParameter(new Dictionary<string, object> { { "title", "abc" } });
			body.AddParameter(new Dictionary<string, object> { });
			Assert.True(body.GetBody().Length > 0);
		}
		[Fact]
		public void TestJsonBody()
		{
			JsonBody body = new JsonBody();
			Assert.Null(body.GetBody());
			body.AddParameter(new { id = 1 });
			Assert.True(body.GetBody().Length > 0);
		}

		[Theory]
		[InlineData("")]
		[InlineData("abc")]
		public void TestTextBody(string content)
		{
			TextBody body = new TextBody(content);
			if (string.IsNullOrWhiteSpace(content))
				Assert.Null(body.GetBody());
			else
				Assert.True(body.GetBody().Length > 0);
		}


		public static List<object[]> GetStream()
		{
			var list = new List<object[]> { new object[] { new MemoryStream(Encoding.UTF8.GetBytes("abd")) } };
			return list;
		}

		[Theory]
		[InlineData(null)]
		[MemberData("GetStream")]
		public void TestStreamBody(Stream stream)
		{
			StreamBody body = new StreamBody(stream);
			if (stream == null)
				Assert.Null(body.GetBody());
			else
				Assert.True(body.GetBody().Length > 0);
		}

		[Fact]
		public void TestMultipartBody()
		{
			MultipartBody body = new MultipartBody();
			Assert.Null(body.GetBody());
			body.AddFiles(new List<NamedFileStream>
			{
				new NamedFileStream
				{
					FileName = "RestRequest.dll",
					Name = "files",
					Stream = File.OpenRead(Directory.GetCurrentDirectory() + "/RestRequest.dll")
				}
			});
			body.AddParameters(new { id = 1 });
			body.AddParameters(new Dictionary<string, object> { { "name", "jack" } });
			Assert.NotNull(body.GetBody());
		}
	}
}
