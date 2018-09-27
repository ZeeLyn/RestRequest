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
		private readonly List<NamedFileStream> _files;
		private Dictionary<string, object> Parameters { get; set; }

		public string Boundary { get; }

		public MultipartBody()
		{
			_files = new List<NamedFileStream>();
			Parameters = new Dictionary<string, object>();
			Boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
		}

		public Stream GetBody()
		{
			var itemboundary = "\r\n--" +
							   $"{Boundary}\r\n";
			var endboundary = "\r\n--" + $"{Boundary}--";
			var itemBytes = Encoding.ASCII.GetBytes(itemboundary);
			var endBytes = Encoding.ASCII.GetBytes(endboundary);
			Stream bodyStream = new MemoryStream();
			if (Parameters != null && Parameters.Count > 0)
			{
				var formdataTemplate = "\r\n--" + Boundary +
									   "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
				foreach (var item in Parameters)
				{
					var bytes = Encoding.UTF8.GetBytes(string.Format(formdataTemplate, item.Key, item.Value));
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
					var fileheaderBytes = Encoding.UTF8.GetBytes(string.Format(headerTemplate, file.Name, file.FileName, file.ContentType));
					bodyStream.Write(fileheaderBytes, 0, fileheaderBytes.Length);
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
			return bodyStream;
		}

		public void AddParameters(object parameters)
		{
			if (parameters == null)
				return;
			var properties = parameters.GetType().GetProperties();
			foreach (var enumerator in properties)
			{
				Parameters.Add(enumerator.Name, enumerator.GetValue(parameters).ToString());
			}
		}

		public void AddParameters(Dictionary<string, object> parameters)
		{
			if (parameters != null && parameters.Count > 0)
				Parameters = parameters;
		}

		public void AddFiles(IEnumerable<NamedFileStream> files)
		{
			if (files == null || !files.Any())
				return;
			_files.AddRange(files);
		}
	}
}
