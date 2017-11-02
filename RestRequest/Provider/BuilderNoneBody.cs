using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using RestRequest.interfaces;

namespace RestRequest.Provider
{
	public class BuilderNoneBody : BuilderBase, IBuilderNoneBody
	{
		public BuilderNoneBody(string url, HttpMethod method) : base(url, method)
		{
			RequestBody = new DefaultBody();
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
			var res = builder.GetResponse();

			return new ResponseResult<string>
			{
				Succeed = res.Success,
				StatusCode = res.StatusCode,
				Content = res.ResponseContent
			};

		}

		public ResponseResult<T> RresponseValue<T>()
		{
			var builder = new BuilderRequest(this);
			builder.CreateRequest();
			var res = builder.GetResponse();
			return new ResponseResult<T>
			{
				Succeed = res.Success,
				StatusCode = res.StatusCode,
				Content = JsonConvert.DeserializeObject<T>(res.ResponseContent)
			};
		}
	}
}