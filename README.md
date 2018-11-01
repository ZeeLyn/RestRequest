# RestReuest-Simple .NET REST Client, based on .NET Standard 2.0.


```csharp
var res=HttpRequest.Get("url")
.ContentType("application/json")
.Timeout(2000)
.UserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.77 Safari/537.36")
.Referer("http://www.baidu.com")
.Cookies(new { token = "bearer token" })
.KeepAlive()
.IgnoreCertError()
.ConnectionLimit(20)
.ResponseValue<T>();


var res=HttpRequest.Post("url").Form(new{name="jack"}).ResponseValue<T>();

var res=HttpRequest.Post("url").Form(
      new List<NamedFileStream>{new NamedFileStream("name","filename",FileStream)}, new{name="jack"})
      .ResponseValue<T>();
      
HttpRequest.Post("url").OnSuccess((statuscode, content) => {

			}).OnFail(ex => {

			}).Start();
var res=HttpRequest.Post("url").Body(new{name="jack"})
.AddCertificate("","")
.ContentType("html/text")
.ResponseString();
```

