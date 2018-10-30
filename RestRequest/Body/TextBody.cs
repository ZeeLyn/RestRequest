using System.Text;
using RestRequest.Interface;

namespace RestRequest.Body
{
	public class TextBody : IBody
	{
		private string Content { get; }

		public TextBody(string content)
		{
			Content = content;
		}

		public byte[] GetBody()
		{
			if (string.IsNullOrWhiteSpace(Content))
				return null;
			return Encoding.UTF8.GetBytes(Content);
		}
	}
}
