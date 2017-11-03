using System;
using System.Collections.Generic;
using System.Text;

namespace RestRequest.interfaces
{
	public interface IActionBackResult : IActionCallback
	{
		/// <summary>
		/// 设置Content-Type
		/// </summary>
		/// <param name="contenttype"></param>
		/// <returns></returns>
		IBuilderNoneBody ContentType(string contenttype);
	}
}
