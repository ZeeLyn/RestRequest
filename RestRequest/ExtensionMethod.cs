using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestRequest
{
	public static class ExtensionMethod
	{
		public static string AsString(this byte[] input)
		{
			if (input == null || input.Length == 0)
				return "";
			return Encoding.UTF8.GetString(input, 0, input.Length);
		}

		public static T JsonTo<T>(this string json)
		{
			if (string.IsNullOrWhiteSpace(json))
				return default;
			if (typeof(T) == typeof(string))
				return (T)Convert.ChangeType(json, typeof(string));
			return JsonConvert.DeserializeObject<T>(json);
		}

		public static byte[] AsBytes(this Stream stream)
		{
			if (stream == null || !stream.CanRead)
				return null;
			var buffer = new byte[16 * 1024];
			using (stream)
			using (var ms = new MemoryStream())
			{
				int read;
				while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
					ms.WriteAsync(buffer, 0, read);
				return ms.ToArray();
			}
		}

		public static async Task<byte[]> AsBytesAsync(this Stream stream, CancellationToken cancellationToken)
		{
			if (stream == null || !stream.CanRead)
				return null;
			var buffer = new byte[16 * 1024];
			using (stream)
			using (var ms = new MemoryStream())
			{
				int read;
				while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
					await ms.WriteAsync(buffer, 0, read, cancellationToken);
				return ms.ToArray();
			}
		}

		public static void SaveAs(this byte[] input, string filename)
		{
			File.WriteAllBytes(filename, input);
		}

		public static Dictionary<string, object> ReadProperties(this object obj)
		{
			var type = obj.GetType();
			var properties = ObjectPropertyCache.Get(type) ?? ObjectPropertyCache.Set(type, obj.GetType().GetProperties().ToList());
			return properties.ToDictionary(key => key.Name, value => value.GetValue(obj));
		}
	}
}
