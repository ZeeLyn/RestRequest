using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RestRequest.interfaces
{
	public interface IBuilderNoneBody : IActionBackResult
	{
		ResponseResult<string> ResponseString();

		ResponseResult<T> RresponseValue<T>();
	}
}
