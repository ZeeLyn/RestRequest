# RestRequest
Simple .NET REST Client, based on .NET Standard 2.0.

# Package & Status
Package | NuGet
--------|------
RestRequest|[![NuGet package](https://buildstats.info/nuget/RestRequest)](https://www.nuget.org/packages/RestRequest)

# Usage
```csharp
var res=HttpRequest.Get("url")
.ContentType("application/json")
.Timeout(2000)
.UserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36")
.Referer("http://www.baidu.com")
.Headers(new { auth = "bearer token" })
.Cookies(new { u = "123456" })
.KeepAlive()
.IgnoreCertError()
.ConnectionLimit(20)
.ResponseValue<T>();


var res=HttpRequest.Post("url").Form(new{name="jack"}).ResponseValue<T>();

var res=HttpRequest.Post("url").Form(
      new List<NamedFileStream>{new NamedFileStream("name","filename",FileStream)}, new{name="jack"})
      .ResponseValue<T>();
      
HttpRequest.Post("url").OnSuccess((statuscode, content) => {

			}).OnFail((statuscode,error) => {

			}).Start();
			
			
var res=HttpRequest.Post("url").Body(new{name="jack"})
.AddCertificate("","")
.ContentType("html/text")
.ResponseValue<T>();


HttpRequest.Get("https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png").Download("/download/logo.png");
```

