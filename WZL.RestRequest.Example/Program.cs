using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestRequest;
using System.Drawing;

namespace WZL.RestRequest.Example
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var r = HttpRequest.Get("http://localhost:11353/api/v2.0/push/list").Cookies(new { auth = "asfasf" }).UserAgent("ie9").Timeout(3000).Headers(new { Authorization = "bearar dsafadsfasf" }).ContentType("html/text").ResponseStringAsync().Result;

			//Console.WriteLine(r.Content);

			//using (var r = HttpRequest.Get("https://www.baidu.com/").ResponseStream())
			//{
			//	using (var reader = new StreamReader(r.Content))
			//	{
			//		Console.WriteLine(reader.ReadToEnd());
			//	}
			//}

			////异步回调
			//HttpRequest.Get("http://localhost:11353/api/v2.0/push/list/false").OnSuccess((HttpStatusCode code, Stream res) =>
			//{
			//	//var reader = new StreamReader(res);
			//	//var r = reader.ReadToEnd();
			//	Console.WriteLine(res);
			//}).OnFail(ex =>
			//{
			//	Console.WriteLine(ex.Message);
			//}).Start();

			////下载
			//HttpRequest.Get("https://ss0.baidu.com/6ONWsjip0QIZ8tyhnq/it/u=1756865070,3273411217&fm=173&s=E4A127F350B7F5EF022990C0030050F3&w=640&h=640&img.JPEG").OnSuccess((HttpStatusCode code, Stream res) =>
			//{
			//	//var reader = new StreamReader(res);
			//	//var r = reader.ReadToEnd();
			//	//Console.WriteLine(res);
			//	Image.FromStream(res).Save("d://download1.jpeg");

			//}).OnFail(ex =>
			//{
			//	Console.WriteLine(ex.Message);
			//}).Start();

			//using (var res =
			//	HttpRequest.Get(
			//		"https://ss0.baidu.com/6ONWsjip0QIZ8tyhnq/it/u=1756865070,3273411217&fm=173&s=E4A127F350B7F5EF022990C0030050F3&w=640&h=640&img.JPEG").ResponseStreamAsync().Result
			//)
			//{
			//	Image.FromStream(res.Content).Save("d://download2.jpeg");
			//}

			//using (var res = HttpRequest.Get("http://localhost:11353/api/v2.0/push/list/false").ResponseStringAsync().Result)
			//{
			//	//var c = res.Content;
			//	//var reader = new StreamReader(res.Content);
			//	//var r = reader.ReadToEnd();
			//	Console.WriteLine(res.Content);
			//}

			//using (var res = HttpRequest.Post("http://localhost:11353/api/v2.0/push/testpost").Form(new { name = "abd" }).Headers(new { Authorization = "Bearer safasfasf" }).ResponseStringAsync().Result)
			//{
			//	//var c = res.Content;
			//	//var reader = new StreamReader(res.Content);
			//	//var r = reader.ReadToEnd();
			//	Console.WriteLine(res.Content);
			//}

			//HttpRequest.Post("http://localhost:11353/api/v2.0/push/testpost").Form(new List<NamedFileStream>
			//{
			//	new NamedFileStream("file", "d:\\download1.jpg", File.OpenRead("d:\\download1.jpg"))
			//}, new { name = "abd" }).OnSuccess((HttpStatusCode code, string content) =>
			//{
			//	var r = content;
			//}).OnFail(ex =>
			//{
			//	var r = ex;
			//}).Start();


			Console.WriteLine();
			Console.WriteLine("Hello World!");
			Console.ReadKey();
		}
	}
}
