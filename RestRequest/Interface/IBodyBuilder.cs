using System.Collections.Generic;
using System.IO;

namespace RestRequest.Interface
{
	public interface IBodyBuilder : INoneBodyBuilder
	{
		INoneBodyBuilder Body(object parameters);


		INoneBodyBuilder Form(object parameters);

		INoneBodyBuilder Form(IDictionary<string, object> parameters);


		INoneBodyBuilder Form(IEnumerable<NamedFileStream> files, IDictionary<string, object> parameters);


		INoneBodyBuilder Form(IEnumerable<NamedFileStream> files, object parameters);


		INoneBodyBuilder Form(IEnumerable<NamedFileStream> files);


		INoneBodyBuilder Form(string text);


		INoneBodyBuilder Form(Stream stream);
	}
}
