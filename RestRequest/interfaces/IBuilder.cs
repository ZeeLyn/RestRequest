using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RestRequest.interfaces
{
	public interface IBuilder : IBuilderNoneBody
	{
		IBuilderNoneBody Body(object parameters);

		IBuilderNoneBody Form(object parameters);

		IBuilderNoneBody Form(IEnumerable<NamedFileStream> files, Dictionary<string, string> parameters);
		IBuilderNoneBody Form(Dictionary<string, string> parameters);
		IBuilderNoneBody Form(IEnumerable<NamedFileStream> files, object parameters);
		IBuilderNoneBody Form(IEnumerable<NamedFileStream> files);

	}
}
