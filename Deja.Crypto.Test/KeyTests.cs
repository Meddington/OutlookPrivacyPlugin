using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deja.Crypto.BcPgp;
using NUnit.Framework;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Deja.Crypto.Test
{
	[TestFixture]
	class KeyTests
	{
		[Test]
		public void ExportPublicKey()
		{
			var expectedOutput = @"-----BEGIN PGP PUBLIC KEY BLOCK-----

mQENBFY1R3kBCADPO0yyqcHmBt7tXs28sjiXW+xnSoqgD7x63f5ePUsJ2emDff/Z
FwvmW+EAlgQ17AYBetz8uuFHgo41uNfOEVDphzBVOuZ9r/wtNOzhaT2fXyo4HTA0
6K5/c5/WcyXg/AnvM2SHQ1WfqV4MZ3l/umrKawFLZVuRKmQQmcuMvPJTq5lI6nDz
L98b62DUXb/3pZCVFuJNjdoxAy2KsTKl5g0rxDM/I6V7t4t4udWZ08Gk0iuOSRON
aG91C/H63KIOEwHtxpjQmS7QCb6H9BUKsO0JwvQYvfUYFQxPjL1GU7Wpg9xY6mT3
qJJLQ4XTlwcUENbeOzgnyqUnlGNibxmB1JFrABEBAAG0F1JzYSBKaWxsIDxyc2FA
amlsbC5jb20+iQE4BBMBAgAiBQJWNUd5AhsDBgsJCAcDAgYVCAIJCgsEFgIDAQIe
AQIXgAAKCRD7Oy0tkscDmvBACACFsrLKVJ1bOMMNGmXWwyy6ftnlpNIUI1D02tKN
ZD4sR1sRHQT79TBaYG39DeJjZpXPNBpKqFV6Ux6R9xPnr0oyschxSFj855Cu08Ls
7MHIzM+CGGsF//mxPzJoNQWLsYWeoDnwHSqAMj4ApdTENZQoOZK2jBM68V/fwQKB
c9FhE5I26Sc2/fkH+CqCfkqh1MIzbz2Me6QPEVDVmYwwHkKmKfB6HTL1o/+iNeFb
9/813HhvpUK+iYKsIW3A17bANr3ex4j4gFS6nnGSd4zm5RvHxcp0iZW08SiaEgrF
dUXA3GVNMgjJbxdp9t1NulolXPcDiO5yfjIgouj3oVdM+lF8sAIAA7kBDQRWNUd5
AQgAzDaJVBd1Yh3GchXpVmYPQOFK7HHX7D1/Yn+YvfkbO4EKEzVZ1iIoekCzXeZl
Q5hO0cpyhblXaJJAUGNJmKLL5z/OkcfG7FUsZgnhdkXNVejVXWJZv+HoHKDHXdjJ
i3c4no2cqzzeLBut3fsqThargLLqrWyadvGQRkxOtTsQVHLrgnID33Jdh9hsYMvB
3/mHUT4g+ZU98ekA5vTKZii/dcy2160GKOMP1PmvcKrOm0m399OZgyh0sTeQylZu
MPrVFU6KxA+g5708OpLrpasFkWDnzwfCAl6l+GdU29QfW1rfLOPj81Ip39ppanhQ
xkjmgqgpARK96cnfZ126HWx1GwARAQABiQEfBBgBAgAJBQJWNUd5AhsMAAoJEPs7
LS2SxwOav9MIAJyfqlXPTrxtnXvB85/qXyhB6KWkhOT9TPR3H9UHzSDyZVus0aza
W/Qe59QqpSjLPBvKJs+FhhLPHjS4qPjExHIo/jSWZ5nKW15UPdQdhj+JGRKXLZRd
7ENvjw9Md922G+M/z9V0mqSZlwDwnMLy5/dQmduZdysIXeqvPQk3ZDQbH2kyPxHF
oa3UhjC8/UZYVfD0EmO6b9wtWDCfgr0z47TMr7myn1NMwWSn13ZXYvwXdlpSx1Ps
l1ISDLDE94wcCDzbnPvXXCFJ26ATYlX8PPI6jmp9VHU/FdQtfVd9+3TprboR9JVl
qVpBT5OoBs9xe9FVirifzbAZ4JCrWcAJ1eKwAgAD
=uWzT
-----END PGP PUBLIC KEY BLOCK-----
";

			var context = new CryptoContext(
				GetPasswordCallback,
				@"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\pubring.gpg",
				@"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\secring.gpg",
				"rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			var key = crypto.PublicKey("rsa@jill.com", new Dictionary<string,string>());

			Assert.AreEqual(expectedOutput, key);
		}

		[Test]
		public void CorrectEncryptKeyTest()
		{
			var context = new CryptoContext(
				GetPasswordCallback,
				@"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\pubring.gpg",
				@"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\secring.gpg",
				"rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			var key = crypto.GetSecretKeyForEncryption("rsa@jill.com");

			Assert.AreEqual(long.Parse("BED5C89E8F3ABF8E", NumberStyles.HexNumber), key.KeyId);
		}

		[Test]
		public void CorrectSignKeyTest()
		{
			var context = new CryptoContext(
				GetPasswordCallback,
				@"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\pubring.gpg",
				@"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\secring.gpg",
				"rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			PgpSecretKey masterKey;
			var signKey = crypto.GetSecretKeyForSigning("rsa@jill.com", out masterKey);

			Assert.AreEqual(long.Parse("FB3B2D2D92C7039A", NumberStyles.HexNumber), signKey.KeyId);
		}


		public char[] GetPasswordCallback(PgpSecretKey masterKey, PgpSecretKey key)
		{
			return "test".ToCharArray();
		}


	}
}
