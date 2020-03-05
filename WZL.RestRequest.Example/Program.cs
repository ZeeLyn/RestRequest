using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using RestRequest;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace WZL.RestRequest.Example
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var c = new CancellationTokenSource();
            await HttpRequest
                .Get(
                    "https://lingque-oss-jssdk.oss-cn-beijing.aliyuncs.com/beiqi_drive/video/473e391c4bd64410b0012d8f7aa57538.mp4")
                .DownloadFromBreakPointAsync("d:\\download\\download.mp4",
                    (total, current, progress) =>
                    {
                        Console.WriteLine("total:{0},download:{1},progress:{2},r:{3}", total, current, progress,
                            (decimal)current / total);
                    }, () => { Console.WriteLine("完成"); }, (err) => { Console.WriteLine("error:{0}", err); }, (downloaded, total) =>
                     {
                         Console.WriteLine("Cancelled");
                     }, c.Token);

            Console.ReadKey();
            c.Cancel();
            Console.ReadKey();


            var r0 = HttpRequest.Get("http://localhost:61389/api/values/2").ResponseValue<string>();
            Console.WriteLine("0:succeed:{0},status:{1},value:{2},error:{3}", r0.Succeed, r0.StatusCode, r0.Content,
                r0.FailMessage);


            var r1 = HttpRequest.Get("http://localhost:61389/api/values/2").ResponseValue<int>();
            Console.WriteLine("1:succeed:{0},status:{1},value:{2},error:{3}", r1.Succeed, r1.StatusCode, r1.Content,
                r1.FailMessage);



            var r2 = HttpRequest.Get("http://localhost:61389/api/values/0").ResponseValue<int>();
            Console.WriteLine("2:succeed:{0},status:{1},value:{2},error:{3}", r2.Succeed, r2.StatusCode, r2.Content,
                r2.FailMessage);

            var r3 = HttpRequest.Post("http://localhost:61389/api/values/json").Body(new { name = "test" })
                .ResponseValue<TestData>();
            Console.WriteLine("3:succeed:{0},status:{1},value:{2},error:{3}", r3.Succeed, r3.StatusCode,
                JsonConvert.SerializeObject(r3.Content), r3.FailMessage);


            HttpRequest.Post("http://localhost:61389/api/values/json").Body(new { name = "test1" })
                .OnSuccess((HttpStatusCode code,
                    TestData d) =>
                {
                    Console.WriteLine("4:json async:status:{0},value:{1}", code,
                        JsonConvert.SerializeObject(d));
                })
                .OnFail((code, err) => { Console.WriteLine("4:Code:{0},Error:{1}", code, err); }).Start();

            var r4 = HttpRequest.Post("http://localhost:61389/api/values/json").Body(new { name = "error" })
                .ResponseValue<TestData>();
            Console.WriteLine("5:succeed:{0},status:{1},value:{2},error:{3}", r4.Succeed, r4.StatusCode,
                JsonConvert.SerializeObject(r4.Content), r4.FailMessage);

            var r5 = HttpRequest.Post("http://localhost:61389/api/values/upload").Form(new List<NamedFileStream>
            {
                new NamedFileStream
                {
                    Stream = File.OpenRead("D://1.jpg"), Name = "file", FileName = "1.jpg"
                },
                new NamedFileStream
                {
                    Stream = File.OpenRead("D://2.jpg"), Name = "file", FileName = "2.jpg"
                }
            }).ResponseValue<dynamic>();
            Console.WriteLine("6:succeed:{0},status:{1},value:{2},error:{3}", r5.Succeed, r5.StatusCode,
                JsonConvert.SerializeObject(r5.Content), r5.FailMessage);

            var r6 = HttpRequest.Post("http://localhost:61389/api/values/upload").Form(new List<NamedFileStream>
            {
                new NamedFileStream
                {
                    Stream = File.OpenRead("D://1.jpg"), Name = "file", FileName = "1.jpg"
                }
            }, new { name = "jack" }).ResponseValue<dynamic>();
            Console.WriteLine("7:succeed:{0},status:{1},value:{2},error:{3}", r6.Succeed, r6.StatusCode,
                JsonConvert.SerializeObject(r6.Content), r6.FailMessage);

            HttpRequest.Post("http://localhost:61389/api/values/upload").Form(new List<NamedFileStream>
                {
                    new NamedFileStream
                    {
                        Stream = File.OpenRead("D://1.jpg"), Name = "file", FileName = "1.jpg"
                    }
                }, new { name = "jack" })
                .OnSuccess((HttpStatusCode code, string content) =>
                {
                    Console.WriteLine("8:async callback-----status:{0},value:{1}", code, content);
                })
                .OnFail((code, err) => { Console.WriteLine("8:async callback----status:{0},error:{1}", code, err); })
                .Start();

            var r7 = HttpRequest.Post("http://localhost:61389/api/values/form").Form(new { name = "jack" })
                .ResponseValue<dynamic>();
            Console.WriteLine("9:succeed:{0},status:{1},value:{2},error:{3}", r7.Succeed, r7.StatusCode,
                JsonConvert.SerializeObject(r7.Content), r7.FailMessage);

            var r8 = HttpRequest.Post("http://localhost:61389/api/values/form").Form(new { name = "jack" })
                .ContentType("application/json").ResponseValue<dynamic>();
            Console.WriteLine("10:succeed:{0},status:{1},value:{2},error:{3}", r8.Succeed, r8.StatusCode,
                JsonConvert.SerializeObject(r8.Content), r8.FailMessage);

            HttpRequest.Post("http://localhost:61389/api/values/form").Form(new { name = "jack" }).ResponseValue<dynamic>(
                    (succeed, code, data, err) =>
                    {
                        Console.WriteLine("11:succeed:{0},status:{1},value:{2},error:{3}", succeed, code,
                            JsonConvert.SerializeObject(data), err);
                    });


            HttpRequest.Get("https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png").Download("d://logo.png");

            Console.ReadKey();
        }
    }
}
