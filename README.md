# RestRequest是基于.NET Standard 2.0 封装的轻量级restful http请求库

## Get请求 Content-Type默认是application/json

直接返回响应字符串
```csharp
using(HttpRequest.Get("url").ResponseString())
{
	...
}
```

把返回值转换成指定的类型
```csharp
using(HttpRequest.Get("url").ResponseValue<obj>())
{
	...
}
```
  
## Post请求 
### Form的Content-Type默认是application/x-www-form-urlencoded
### Body的Content-Type默认是application/json

直接返回响应字符串
```csharp
using(HttpRequest.Post("url").Form(new{name="jack"}).ResponseString())
{
	...
}
using(HttpRequest.Post("url").Body(new{name="jack"}).ResponseString())
{
	...
}
```
把返回值转换成指定类型
```csharp
using(HttpRequest.Post("url").Form(new{name="jack"}).ResponseValue<obj>())
{
	...
}
```
### 也可以上传文件 默认Content-Type是multipart/form-data
```csharp
using(HttpRequest.Post("url").Form(
      new List<NamedFileStream>{new NamedFileStream("name","filename",FileStream)}, new{name="jack"}).ResponseValue<string>())
      {
      	...
      }
```

### 可以通过Headers设置自定义头
```csharp
using(HttpRequest.Post("url").Body(new{name="jack"}).Headers(new{Authorization = "Bearar token"}).ResponseString())
{
	...
}
```

### 可以通过ContentType设置Content-Type值,但是不支持自定义multipart/form-data
```csharp
using(HttpRequest.Post("url").Body(new{name="jack"}).ContentType("html/text").ResponseString())
{
	...
}
```

### 异步回调
```csharp
HttpRequest.Post("url").OnSuccess((statuscode, content) => {

			}).OnFail(ex => {

			}).Start();
```

### 证书

默认忽略证书错误，如果不需要，可以通过以下代码关闭
```csharp
using(HttpRequest.Post("url",ignoreCertificateError:false).Body(new{name="jack"}).ContentType("html/text").ResponseString())
{
	...
}
```

设置证书
```csharp
using(HttpRequest.Post("url").Body(new{name="jack"}).AddCertificate("","").ContentType("html/text").ResponseString())
{
	...
}
```
