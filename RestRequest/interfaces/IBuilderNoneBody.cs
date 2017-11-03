using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestRequest.interfaces
{
	public interface IBuilderNoneBody : IActionBackResult
	{
		ResponseResult<string> ResponseString();

		Task<ResponseResult<string>> ResponseStringAsync();

		ResponseResult<T> ResponseValue<T>();

		Task<ResponseResult<T>> ResponseValueAsync<T>();

		IBuilderNoneBody Headers(Dictionary<string, string> headers);

		IBuilderNoneBody Headers(object headers);
	}
}
