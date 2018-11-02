using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RestRequest
{
	public static class ExtensionMethod
	{
		public static string AsString(this byte[] input)
		{
			if (input == null)
				return "";
			return Encoding.UTF8.GetString(input, 0, input.Length);
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
					ms.Write(buffer, 0, read);
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
