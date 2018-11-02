using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RestRequest.Interface;

namespace RestRequest.Body
{
	public class MultipartBody : IBody
	{
		private List<NamedFileStream> _files { get; }
		private IDictionary<string, object> Parameters { get; }

		internal string Boundary { get; }

		public MultipartBody()
		{
			_files = new List<NamedFileStream>();
			Parameters = new Dictionary<string, object>();
			Boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
		}

		public byte[] GetBody()
		{
			if (!_files.Any() && !Parameters.Any())
				return null;
			var itemBoundary = "\r\n--" + $"{Boundary}\r\n";
			var endBoundary = "\r\n--" + $"{Boundary}--";
			var itemBytes = Encoding.ASCII.GetBytes(itemBoundary);
			var endBytes = Encoding.ASCII.GetBytes(endBoundary);
			using (Stream bodyStream = new MemoryStream())
			{
				if (Parameters != null && Parameters.Count > 0)
				{
					var formDataTemplate = "\r\n--" + Boundary +
										   "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
					foreach (var item in Parameters)
					{
						var bytes = Encoding.UTF8.GetBytes(string.Format(formDataTemplate, item.Key, item.Value));
						bodyStream.Write(bytes, 0, bytes.Length);
					}
				}
				if (_files.Any())
				{
					const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
												  "Content-Type: {2}\r\n\r\n";
					foreach (var file in _files)
					{
						bodyStream.Write(itemBytes, 0, itemBytes.Length);
						var fileHeaderBytes = Encoding.UTF8.GetBytes(string.Format(headerTemplate, file.Name, file.FileName, file.ContentType));
						bodyStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
						using (var stream = file.Stream)
						{
							stream.Position = 0;
							int bytesRead;
							var buffer = new byte[1024];
							while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
							{
								bodyStream.Write(buffer, 0, bytesRead);
							}
						}
					}
					bodyStream.Write(endBytes, 0, endBytes.Length);
				}
				bodyStream.Seek(0, SeekOrigin.Begin);
				var result = new byte[bodyStream.Length];
				bodyStream.Read(result, 0, result.Length);
				return result;
			}
		}

		public void AddParameters(IDictionary<string, object> parameters)
		{

			if (parameters == null || parameters.Count == 0)
				return;
			foreach (var item in parameters)
			{
				Parameters.Add(item);
			}
		}

		public void AddParameters(object parameters)
		{
			if (parameters == null)
				return;
			var properties = parameters.ReadProperties();
			foreach (var enumerator in properties)
			{
				Parameters.Add(enumerator.Key, enumerator.Value);
			}
		}


		public void AddFiles(IEnumerable<NamedFileStream> files)
		{
			if (files == null || !files.Any())
				return;
			_files.AddRange(files);
		}
	}
}
