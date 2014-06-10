using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deja.Crypto.BcPgp;

namespace TestApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var context = new CryptoContext(args[0].ToCharArray());
			var crypto = new PgpCrypto(context);

			Console.WriteLine(ASCIIEncoding.ASCII.GetString(crypto.DecryptAndVerify(File.ReadAllBytes(args[1]), true)));

			if (context.FailedIntegrityCheck)
				Console.WriteLine("Failed integrity check");

			Console.WriteLine("Done");


		}
	}
}
