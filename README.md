# RestHttp是基于.NET Standard 2.0 封装的http请求库

## Get请求 Content-Type默认是application/json

直接返回响应字符串
```csharp
HttpRequest.Get("url").ResponseString();
```

把返回值转换成指定的类型
```csharp
HttpRequest.Get("url").ResponseValue<obj>();
```
  
## Post请求 
### Form的Content-Type默认是application/x-www-form-urlencoded
### Body的Content-Type默认是application/json

直接返回响应字符串
```csharp
HttpRequest.Post("url").Form(new{name="jack"}).ResponseString();
HttpRequest.Post("url").Body(new{name="jack"}).ResponseString();
```
把返回值转换成指定类型
```csharp
HttpRequest.Post("url").Form(new{name="jack"}).ResponseValue<obj>();
```
### 也可以上传文件 默认Content-Type是multipart/form-data
```csharp
HttpRequest.Post("url").Form(
      new List<NamedFileStream>{new NamedFileStream("name","filename",FileStream)}, new{name="jack"}).ResponseValue<string>();
```

### 可以通过Headers设置自定义头
```csharp
HttpRequest.Post("url").Body(new{name="jack"}).Headers(new{Authorization = "Bearar token"}).ResponseString();
```

### 可以通过ContentType设置Content-Type值
```csharp
HttpRequest.Post("url").Body(new{name="jack"}).ContentType("html/text").ResponseString();
```

### 异步回调
```csharp
HttpRequest.Post("url").OnSuccess((statuscode, content) => {

			}).OnFail(ex => {

			}).Start();
```

