using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using RestRequest;
using Newtonsoft.Json;

namespace WZL.RestRequest.Example
{
	internal class Program
	{
		static void Main(string[] args)
		{
			using (var r1 = HttpRequest.Get("http://localhost:61389/api/values/2").ResponseValue<int>())
			{
				Console.WriteLine("succeed:{0},status:{1},value:{2},error:{3}", r1.Succeed, r1.StatusCode, r1.Content,
					r1.FailedContent);
			}

			using (var r2 = HttpRequest.Get("http://localhost:61389/api/values/0").ResponseValue<int>())
			{
				Console.WriteLine("succeed:{0},status:{1},value:{2},error:{3}", r2.Succeed, r2.StatusCode, r2.Content,
					r2.FailedContent);
			}

			using (var r3 = HttpRequest.Post("http://localhost:61389/api/values/json").Body(new { name = "test" })
				.ResponseValue<TestData>())
			{
				Console.WriteLine("succeed:{0},status:{1},value:{2},error:{3}", r3.Succeed, r3.StatusCode,
					JsonConvert.SerializeObject(r3.Content), r3.FailedContent);
			}

			using (var r4 = HttpRequest.Post("http://localhost:61389/api/values/json").Body(new { name = "error" })
				.ResponseValue<TestData>())
			{
				Console.WriteLine("succeed:{0},status:{1},value:{2},error:{3}", r4.Succeed, r4.StatusCode,
					JsonConvert.SerializeObject(r4.Content), r4.FailedContent);
			}

			using (var r5 = HttpRequest.Post("http://localhost:61389/api/values/upload").Form(new List<NamedFileStream>
			{
				new NamedFileStream
				{
					Stream = File.OpenRead("D://1.jpg"), Name = "file", FileName = "1.jpg"
				},
				new NamedFileStream
				{
					Stream = File.OpenRead("D://2.jpg"), Name = "file", FileName = "2.jpg"
				}
			}).ResponseValue<dynamic>())
			{
				Console.WriteLine("succeed:{0},status:{1},value:{2},error:{3}", r5.Succeed, r5.StatusCode,
					JsonConvert.SerializeObject(r5.Content), r5.FailedContent);
			}

			using (var r6 = HttpRequest.Post("http://localhost:61389/api/values/upload").Form(new List<NamedFileStream>
			{
				new NamedFileStream
				{
					Stream = File.OpenRead("D://1.jpg"), Name = "file", FileName = "1.jpg"
				}
			}, new { name = "jack" }).ResponseValue<dynamic>())
			{
				Console.WriteLine("succeed:{0},status:{1},value:{2},error:{3}", r6.Succeed, r6.StatusCode,
					JsonConvert.SerializeObject(r6.Content), r6.FailedContent);
			}

			HttpRequest.Post("http://localhost:61389/api/values/upload").Form(new List<NamedFileStream>
				{
					new NamedFileStream
					{
						Stream = File.OpenRead("D://1.jpg"), Name = "file", FileName = "1.jpg"
					}
				}, new { name = "jack" })
				.OnSuccess((HttpStatusCode code, string content) =>
				{
					Console.WriteLine("async callback-----status:{0},value:{1}", code, content);
				})
				.OnFail((code, err) => { Console.WriteLine("async callback----status:{0},error:{1}", code, err); })
				.Start();

			using (var r7 = HttpRequest.Post("http://localhost:61389/api/values/form").Form(new { name = "jack" }).ResponseValue<dynamic>())
			{
				Console.WriteLine("r7----succeed:{0},status:{1},value:{2},error:{3}", r7.Succeed, r7.StatusCode,
					JsonConvert.SerializeObject(r7.Content), r7.FailedContent);
			}

			using (var r8 = HttpRequest.Post("http://localhost:61389/api/values/form").Form(new { name = "jack" }).ContentType("application/json").ResponseValue<dynamic>())
			{
				Console.WriteLine("r8----succeed:{0},status:{1},value:{2},error:{3}", r8.Succeed, r8.StatusCode,
					JsonConvert.SerializeObject(r8.Content), r8.FailedContent);
			}


			Console.WriteLine("Hello World!");
			Console.ReadKey();
		}
	}
}
