using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using RestRequest;

namespace WZL.RestRequest.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var r = HttpRequest.Get("http://localhost:11353/api/v2.0/push/list").ContentType("html/text").ResponseString();

			Console.WriteLine(r.Content);
			//HttpRequest.Post("").Body(new { }).ResponseResult();

			Console.WriteLine("Hello World!");
			Console.ReadKey();
		}
	}
}
