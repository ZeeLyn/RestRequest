using System.Collections.Generic;
using System.IO;
using RestRequest.Body;
using RestRequest.Interface;

namespace RestRequest.Builder
{
	public partial class ContextBuilder
	{

		/// <summary>
		/// 设置json数据
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public INoneBodyBuilder Body(object parameters)
		{
			var body = new JsonBody();
			body.AddParameter(parameters);
			_requestBody = body;
			ContentType("application/json");
			return this;
		}

		/// <summary>
		/// 设置表单数据
		/// content-type=application/x-www-form-urlencoded
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public INoneBodyBuilder Form(object parameters)
		{
			var body = new FormBody();
			body.AddParameter(parameters);
			_requestBody = body;
			ContentType("application/x-www-form-urlencoded");
			return this;
		}

		/// <summary>
		/// 设置表单数据
		/// content-type=application/x-www-form-urlencoded
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public INoneBodyBuilder Form(IDictionary<string, object> parameters)
		{
			var body = new FormBody();
			body.AddParameter(parameters);
			_requestBody = body;
			ContentType("application/x-www-form-urlencoded");
			return this;
		}

		/// <summary>
		/// 设置表单数据和上传的文件
		/// content-type=multipart/form-data
		/// </summary>
		/// <param name="files">上传的文件信息</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public INoneBodyBuilder Form(IEnumerable<NamedFileStream> files, object parameters)
		{
			var body = new MultipartBody();
			body.AddParameters(parameters);
			body.AddFiles(files);
			_requestBody = body;
			ContentType("multipart/form-data; boundary=" + body.Boundary);
			return this;
		}

		/// <summary>
		/// 设置上传的文件
		/// content-type=multipart/form-data
		/// </summary>
		/// <param name="files">上传的文件信息</param>
		/// <returns></returns>
		public INoneBodyBuilder Form(IEnumerable<NamedFileStream> files)
		{
			var body = new MultipartBody();
			body.AddFiles(files);
			_requestBody = body;
			ContentType("multipart/form-data; boundary=" + body.Boundary);
			return this;
		}

		/// <summary>
		/// 设置表单数据和上传的文件
		/// content-type=multipart/form-data
		/// </summary>
		/// <param name="files">上传的文件信息</param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public INoneBodyBuilder Form(IEnumerable<NamedFileStream> files, IDictionary<string, object> parameters)
		{
			var body = new MultipartBody();
			body.AddParameters(parameters);
			body.AddFiles(files);
			_requestBody = body;
			ContentType("multipart/form-data; boundary=" + body.Boundary);
			return this;
		}

		/// <summary>
		/// 设置提交的文本
		/// content-type=application/text
		/// </summary>
		/// <param name="text">提交文本</param>
		/// <returns></returns>
		public INoneBodyBuilder Form(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return this;
			_requestBody = new TextBody(text);
			ContentType("application/text");
			return this;
		}

		/// <summary>
		/// 设置提交的Stream
		/// content-type=application/octet-stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public INoneBodyBuilder Form(Stream stream)
		{
			if (stream == null)
				return this;
			_requestBody = new StreamBody(stream);
			ContentType("application/octet-stream");
			return this;
		}

	}
}
