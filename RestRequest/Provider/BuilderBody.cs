using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestRequest.interfaces;

namespace RestRequest.Provider
{
	public class BuilderBody : BuilderBase, IBuilder
	{
		public BuilderBody(string url, HttpMethod method) : base(url, method)
		{
		}
		public IBuilderNoneBody Body(object parameters)
		{
			var body = new JsonBody();
			body.AddParameter(parameters);
			RequestBody = body;
			return this;
		}

		public IBuilderNoneBody Form(object parameters)
		{
			var body = new FormBody();
			body.AddParameter(parameters);
			RequestBody = body;
			return this;
		}

		public IBuilderNoneBody Form(Dictionary<string, string> parameters)
		{
			var body = new FormBody();
			body.AddParameter(parameters);
			RequestBody = body;
			return this;
		}

		public IBuilderNoneBody Form(IEnumerable<NamedFileStream> files, object parameters)
		{
			var body = new MultipartBody();
			body.AddParameters(parameters);
			body.AddFiles(files);
			RequestBody = body;
			return this;
		}

		public IBuilderNoneBody Form(IEnumerable<NamedFileStream> files)
		{
			var body = new MultipartBody();
			body.AddFiles(files);
			RequestBody = body;
			return this;
		}

		public IBuilderNoneBody Form(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				throw new NullReferenceException("Post文本内容不能为空");
			RequestBody = new TextBody(text);
			return this;
		}

		public IBuilderNoneBody Form(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("Post stream is null");
			RequestBody = new StreamBody(stream);
			return this;
		}

		public IBuilderNoneBody Form(IEnumerable<NamedFileStream> files, Dictionary<string, string> parameters)
		{
			var body = new MultipartBody();
			body.AddParameters(parameters);
			body.AddFiles(files);
			RequestBody = body;
			return this;
		}

		public IActionCallback OnSuccess(Action<HttpStatusCode, string> action)
		{
			SuccessAction = action;
			return this;
		}

		public IActionCallback OnFail(Action<WebException> action)
		{
			FailAction = action;
			return this;
		}

		public IBuilderNoneBody Headers(Dictionary<string, string> headers)
		{
			if (!(headers?.Count > 0)) return this;
			foreach (var item in headers)
				RequestHeaders[item.Key] = item.Value;
			return this;
		}

		public IBuilderNoneBody Headers(object headers)
		{
			if (headers == null)
				return this;
			var properties = headers.GetType().GetProperties();
			foreach (var item in properties)
				RequestHeaders[item.Name] = item.GetValue(headers).ToString();
			return this;
		}

		public Stream DownloadStream()
		{
			throw new NotImplementedException("Post方式不支持下载");
		}

		public void Start()
		{
			var builder = new BuilderRequest(this);
			builder.CreateRequest();
			builder.BuildRequestAndCallback();
		}

		public IBuilderNoneBody ContentType(string contenttype)
		{
			RequestBody?.SetContentType(contenttype);
			return this;
		}


		public ResponseResult<string> ResponseString()
		{
			var builder = new BuilderRequest(this);
			builder.CreateRequest();
			builder.BuildRequest();
			var res = builder.GetResponse();
			return new ResponseResult<string>
			{
				Succeed = res.Success,
				StatusCode = res.StatusCode,
				Content = res.ResponseContent
			};
		}

		public async Task<ResponseResult<string>> ResponseStringAsync()
		{
			var builder = new BuilderRequestAsync(this);
			builder.CreateRequest();
			await builder.BuildRequest();
			var res = await builder.GetResponse();
			return new ResponseResult<string>
			{
				Succeed = res.Success,
				StatusCode = res.StatusCode,
				Content = res.ResponseContent
			};
		}

		public ResponseResult<T> ResponseValue<T>()
		{
			var res = ResponseString();
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = JsonConvert.DeserializeObject<T>(res.Content)
			};
		}

		public async Task<ResponseResult<T>> ResponseValueAsync<T>()
		{
			var res = await ResponseStringAsync();
			return new ResponseResult<T>
			{
				Succeed = res.Succeed,
				StatusCode = res.StatusCode,
				Content = JsonConvert.DeserializeObject<T>(res.Content)
			};
		}
	}
}
