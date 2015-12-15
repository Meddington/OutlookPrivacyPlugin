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
			//password = args[0].ToCharArray();

			//var context = new CryptoContext(PasswordCallback, "AES-128", "SHA-1");
			//var crypto = new PgpCrypto(context);
			try
			{
				using (var inputStream = File.OpenRead(@"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\private\andrew-pubring.gpg"))
				using (var decodeStream = PgpUtilities.GetDecoderStream(inputStream))
				{
					var pgpPub = new PgpPublicKeyRingBundle(decodeStream);

					foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
					{
						// The master key is normally the first key returned.
						var masterKey = kRing.GetPublicKey();
						if (!masterKey.IsMasterKey)
						{
							foreach (PgpPublicKey k in kRing.GetPublicKeys())
								if (k.IsMasterKey)
									Console.WriteLine("{0:X}", k.KeyId);
						}

						foreach (PgpPublicKey k in kRing.GetPublicKeys())
						{
							Console.WriteLine("{0:X}", k.KeyId);
						}
					}
				}
			}
			catch (Exception)
			{
				throw;
			}

		}

		static char[] PasswordCallback(PgpSecretKey masterKey, PgpSecretKey key)
		{
			return password;
		}
	}
}
