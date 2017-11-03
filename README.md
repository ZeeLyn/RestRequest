# RestHttp 使用方法

## Get请求 Content-Type默认是application/json

> 直接返回响应字符串
> HttpRequest.Get("url").ResponseString();

> 把返回值转换成类型T
> HttpRequest.Get("url").ResponseValue<T>();
  
  
## Post请求 
### Form的Content-Type默认是application/x-www-form-urlencoded
### Body的Content-Type默认是application/json

> 直接返回响应字符串
> HttpRequest.Post("url").Form(new{name="jack"}).ResponseString();

> 把返回值转换成类型T
> HttpRequest.Post("url").Form(new{name="jack"}).ResponseValue<T>();
  
> HttpRequest.Post("url").Body(new{name="jack"}).ResponseString();

### 也可以上传文件
> HttpRequest.Post("url").Form(
      new List<NamedFileStream>{
				new NamedFileStream("name","filename",FileStream)
			}, new{name="jack"}).Headers(new { Authorization = "bearar dsafadsfasf" }).ResponseValue<string>();

### 可以通过Headers设置自定义头
> HttpRequest.Post("url").Body(new{name="jack"}).Headers(new{Authorization = "Bearar token"}).ResponseString();

### 可以通过ContentType设置Content-Type值
> HttpRequest.Post("url").Body(new{name="jack"}).ContentType("html/text").ResponseString();
