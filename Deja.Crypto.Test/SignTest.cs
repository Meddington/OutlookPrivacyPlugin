using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Deja.Crypto.BcPgp;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Deja.Crypto.Test
{
	[TestFixture]
    public class SignTest
    {
		private const string Pubring = @"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\pubring.gpg";
		private const string Secring = @"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\secring.gpg";
		private const string Data = @"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\data";

		[Test]
		public void VerifyEmail()
		{
			var asc = @"-----BEGIN PGP SIGNED MESSAGE-----
Hash: SHA256

i think i have found the issue.
I write this mail as - only text - message.
If another receive this email, my signature email and webaddress are i

nterpreted as links and will be changed by outlook to html elements.


Mit freundlichen Grüßen,
Sebastian Lutz

Baebeca Solutions - Lutz
E-Mail: lutz@baebeca.de <mailto:lutz@baebeca.de> Tel. Büro: 02261 - 9202935 Tel. Mobil: 0171 - 6431821
Web: https://www.baebeca.de <https://www.baebeca.de> PGP Key: 0x5AD0240C

-----BEGIN PGP SIGNATURE-----

iQEcBAEBCAAGBQJWPGGdAAoJEEKN+AfqKr312lEIAJ6i2C/8ZWoU3K2T0JWUXLRJ
Rycl2f9IqZkTOA4/x39QX+MuJ8N20ek5YDDeljZZdZnuEkBKvWZUZ/E6f49JJv6p
MBpNZgPua13fjERPIlNNV5CLxXDqhaH+jFaP8hCzthuNMKuW4iPy2wppX4f+EXbH
O5NMNUOtwD149S8y3DDx90Y6RdvQL9HYijDzHHpko1RqRL2lrkxrzOyTk0R0JoS2
C4h6ab6bixbmV6QBCtzOFpp6nkxWT27CFRIN0yz9t6psGZQgEVYP7RQlmFqS0jr4
9pTfjB6djoxrLxNiQMHsaH0UKeC+3AQdfvAloaIljULuBfa9BV8U5CXJBy1JUiM=
=K83z
-----END PGP SIGNATURE-----
";
			var context = new CryptoContext(GetPasswordCallback, Pubring, Secring, "rsa", "sha-1");
			var crypto = new PgpCrypto(context);
			var encoding = Encoding.GetEncoding(28591);

			var ret = crypto.VerifyClear(encoding.GetBytes(asc));

			Assert.IsTrue(ret);

		}
		[Test]
		public void UnicodeWithNonUnicodePageTest()
		{
			var clear = @"Hi Meddington,

i think i have found the issue.
I write this mail as - only text - message.
If another receive this email, my signature email and webaddress are i
nterpreted as links and will be changed by outlook to html elements.


Mit freundlichen Grüßen,
Sebastian Lutz

Baebeca Solutions - Lutz
E-Mail: lutz@baebeca.de
Tel. Büro: 02261 - 9202935
Tel. Mobil: 0171 - 6431821
Web: https://www.baebeca.de
PGP Key: 0x5AD0240C
";

			var context = new CryptoContext(GetPasswordCallback, Pubring, Secring, "rsa", "sha-1");
			var crypto = new PgpCrypto(context);
			var encoding = Encoding.GetEncoding(28591);

			var clearSig = crypto.SignClear(clear, "rsa@jill.com", encoding, new Dictionary<string, string>());
			var ret = crypto.VerifyClear(encoding.GetBytes(clearSig));

			Assert.IsTrue(ret);
		}

		[Test]
		public void UnicodeUtf8PageTest()
		{
			var clear = @"Hi Meddington,

i think i have found the issue.
I write this mail as - only text - message.
If another receive this email, my signature email and webaddress are i
nterpreted as links and will be changed by outlook to html elements.


Mit freundlichen Grüßen,
Sebastian Lutz

Baebeca Solutions - Lutz
E-Mail: lutz@baebeca.de
Tel. Büro: 02261 - 9202935
Tel. Mobil: 0171 - 6431821
Web: https://www.baebeca.de
PGP Key: 0x5AD0240C
";

			var context = new CryptoContext(GetPasswordCallback, Pubring, Secring, "rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			var clearSig = crypto.SignClear(clear, "rsa@jill.com", Encoding.UTF8, new Dictionary<string, string>());
			var ret = crypto.VerifyClear(Encoding.UTF8.GetBytes(clearSig));

			Assert.IsTrue(ret);
		}

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
