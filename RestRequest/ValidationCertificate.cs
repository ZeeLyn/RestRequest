using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace RestRequest
{
	internal class ValidationCertificate
	{
		protected internal static bool VerifyServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
	}
}
