using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RestRequest.interfaces
{
	public interface IBody
	{
		/// <summary>
		/// 设置Content-Type，在调用Form，Body方法之后调用
		/// </summary>
		/// <param name="contenttype"></param>
		void SetContentType(string contenttype);
		string GetContentType();
		Stream GetBody();
	}
}
