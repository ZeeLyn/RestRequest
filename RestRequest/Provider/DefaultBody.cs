using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RestRequest.interfaces;

namespace RestRequest.Provider
{
	public class DefaultBody : IBody
	{
		private string ContentType { get; set; } = "application/json";
		public Stream GetBody()
		{
			return null;
		}

		public string GetContentType()
		{
			return ContentType;
		}

		public void SetContentType(string contenttype)
		{
			ContentType = contenttype;
		}
	}
}
