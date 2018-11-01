using System;
using System.Collections.Generic;
using System.IO;
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

		public static void SaveAs(this byte[] input, string filename)
		{
			File.WriteAllBytes(filename, input);
		}
	}
}
