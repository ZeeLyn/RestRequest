using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RestRequest
{
	public class ResponseResult<T>
	{
		public bool Succeed { get; set; }

		public HttpStatusCode StatusCode { get; set; }

		public T Content { get; set; }
	}
}
