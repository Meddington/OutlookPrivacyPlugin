using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Deja.Crypto.BcPgp;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Deja.Crypto.Test
{
	[TestFixture]
    public class SignTest
    {
		// Not working yet
		//[Test]
		public void ClearTextSignTest()
		{
			var context = new CryptoContext(
				GetPasswordCallback, 
				@"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\pubring.gpg",
				@"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\secring.gpg",
				"rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			var dataDir = @"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\data";
			var signKey = "rsa@jill.com";
			var sign = string.Empty;

			foreach (var file in Directory.EnumerateFiles(dataDir, "*.bin"))
			{
				Console.WriteLine("ClearTextSignTest: " + file);

				context.Digest = "sha-1";
				sign = crypto.SignClear(File.ReadAllText(file), signKey, Encoding.UTF8, new Dictionary<string, string>());
				File.WriteAllText(@"c:\temp\test.bin", sign);
				Assert.AreEqual(File.ReadAllText(file + ".rsa.sha1.clearsign"), sign);

				context.Digest = "SHA-256";
				sign = crypto.SignClear(File.ReadAllText(file), signKey, Encoding.UTF8, new Dictionary<string, string>());
				Assert.AreEqual(File.ReadAllText(file + ".rsa.sha256.clearsign"), sign);

				context.Digest = "SHA-1";
				sign = crypto.Sign(File.ReadAllBytes(file), signKey, new Dictionary<string, string>());
				File.WriteAllText(@"c:\temp\test.bin", sign);
				Assert.AreEqual(File.ReadAllText(file + ".rsa.sha1.sign"), sign);
			}
		}

		public char[] GetPasswordCallback(PgpSecretKey masterKey, PgpSecretKey key)
		{
			return "test".ToCharArray();
		}

    }
}
