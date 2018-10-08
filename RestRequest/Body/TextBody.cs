using System.Text;
using RestRequest.Interface;

namespace RestRequest.Body
{
	internal class TextBody : IBody
	{
		private string Content { get; }

		public TextBody(string content)
		{
			Content = content;
		}

		public byte[] GetBody()
		{
			return Encoding.UTF8.GetBytes(Content);
		}
	}
}
