using RestRequest.Builder;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace XUnitTestProject
{
	public class ContextBuilderFixture : IDisposable
	{
		public ContextBuilder builder { get; }

		public ContextBuilderFixture()
		{
			builder = new ContextBuilder("http://localhost", HttpMethod.Get);
		}


		public void Dispose()
		{

		}
	}
}
