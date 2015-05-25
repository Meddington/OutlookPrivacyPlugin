using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deja.Crypto.BcPgp;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace TestApp
{
	class Program
	{
		static char[] password;

		static void Main(string[] args)
		{
			password = args[0].ToCharArray();

			var context = new CryptoContext(PasswordCallback, "AES-128", "SHA-1");
			var crypto = new PgpCrypto(context);

			Console.WriteLine(ASCIIEncoding.ASCII.GetString(crypto.DecryptAndVerify(File.ReadAllBytes(args[1]), true)));

			if (context.FailedIntegrityCheck)
				Console.WriteLine("Failed integrity check");

			Console.WriteLine("Done");
		}

		static char[] PasswordCallback(PgpSecretKey masterKey, PgpSecretKey key)
		{
			return password;
		}
	}
}
