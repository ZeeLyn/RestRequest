using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using RestRequest;

namespace WZL.RestRequest.Example
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var r = HttpRequest.Get("http://localhost:11353/api/v2.0/push/list").Headers(new { Authorization = "bearar dsafadsfasf" }).ResponseString();

			Console.WriteLine(r.Content);
			Console.WriteLine("Hello World!");
			Console.ReadKey();
		}
	}
}
