using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RestRequest.interfaces
{
	public interface IBuilder : IBuilderNoneBody
	{
		/// <summary>
		/// 提交json数据
		/// Content-Type=application/json
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IBuilderNoneBody Body(object parameters);

		/// <summary>
		/// Post模拟提交表单数据
		/// Content-Type=application/x-www-form-urlencoded
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IBuilderNoneBody Form(object parameters);

		/// <summary>
		/// Post模拟提交表单数据
		/// Content-Type=application/x-www-form-urlencoded
		/// </summary>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IBuilderNoneBody Form(Dictionary<string, string> parameters);

		/// <summary>
		/// 同时上传文件和表单数据
		/// Content-Type=multipart/form-data
		/// </summary>
		/// <param name="files"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IBuilderNoneBody Form(IEnumerable<NamedFileStream> files, Dictionary<string, string> parameters);

		/// <summary>
		/// 同时上传文件和表单数据
		/// Content-Type=multipart/form-data
		/// </summary>
		/// <param name="files"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IBuilderNoneBody Form(IEnumerable<NamedFileStream> files, object parameters);

		/// <summary>
		/// 上传文件
		/// Content-Type=multipart/form-data
		/// </summary>
		/// <param name="files"></param>
		/// <returns></returns>
		IBuilderNoneBody Form(IEnumerable<NamedFileStream> files);

		/// <summary>
		/// 提交文本
		/// Content-Type=application/text
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		IBuilderNoneBody Form(string text);

		/// <summary>
		/// 提交Stream
		/// Content-Type=application/octet-stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		IBuilderNoneBody Form(Stream stream);
	}
}
