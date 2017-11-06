using System;
using System.Collections.Generic;
using System.Text;

namespace RestRequest.interfaces
{
	public interface IActionBackResult : IActionCallback
	{
		/// <summary>
		/// 设置Content-Type,不支持multipart/form-data的自定义
		/// </summary>
		/// <param name="contenttype"></param>
		/// <returns></returns>
		IBuilderNoneBody ContentType(string contenttype);
	}
}
