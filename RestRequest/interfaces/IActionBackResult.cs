using System;
using System.Collections.Generic;
using System.Text;

namespace RestRequest.interfaces
{
	public interface IActionBackResult : IActionCallback
	{
		IBuilderNoneBody ContentType(string contenttype);
	}
}
