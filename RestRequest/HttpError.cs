using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RestRequest
{
    public class HttpError
    {
        public HttpError(HttpStatusCode statusCode, string message = "")
        {
            StatusCode = statusCode;
            Message = message;
        }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
