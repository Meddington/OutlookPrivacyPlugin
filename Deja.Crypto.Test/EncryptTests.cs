using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deja.Crypto.BcPgp;
using NUnit.Framework;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace Deja.Crypto.Test
{
	[TestFixture]
	public class EncryptTests
	{
		private const string Pubring = @"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\pubring.gpg";
		private const string Secring = @"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\secring.gpg";
		private const string Data = @"C:\projects\OutlookPrivacyPlugin\Deja.Crypto.Test\data";

		public char[] GetPasswordCallback(PgpSecretKey masterKey, PgpSecretKey key)
		{
			return "test".ToCharArray();
		}

		[Test]
		public void DecryptAndVerifyCryptix()
		{
			var asc = @"-----BEGIN PGP MESSAGE-----
Version: Cryptix OpenPGP 0.20050418

hIwDblSySC/BGaQBBACXq8hOXnGCsCrBt3YU3XgxR+KBrwNh5U/hseMODMY+Gg+w
jHARa7h9r7ekw+UEw3e2CJzoI0TL1LYM7o3N79mBqUJMIRjyV+dFaE8/it+Xphv3
+B+DyyrlO7Y013MM9v8o7UF5wJyxyIMedeodfboVYOuEYZGMOCZNeP9WgbZIa6Tv
qC+dKk++UQH1nAq7PWl2hrXHl3wWK/kZ+O/bGMFDEpbDL2PenLEy47kvQT4fGRZT
iB+O4gP38rpoPF/s1agFtwfNkWHBGlwHOqBpjuT1ya56aPfkDsNGTW0/Jp+dnTIH
YYGwwCEOQLJemt5ELK8+BZEMRi/5zCmxF/4z9yRUP6IqrotQMWuxjm2d8Z4QhecK
j/nOOPQyDT/WQLFFOI4whGy9D8QHkXNrqsGHY1YORN3s8Di0+ARkSFtRMhjEgzA0
+U+RVtsnaH4pw8pzbHEpZS9Znt75ep+Vune/cyiPfJuvw5uQlns9bLAtHKtt9NY=
=uF5e
-----END PGP MESSAGE-----
";
			var context = new CryptoContext(GetPasswordCallback, Pubring, Secring, "rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			var clear = crypto.DecryptAndVerify(Encoding.UTF8.GetBytes(asc), true);

			Assert.NotNull(clear);
			Assert.AreEqual("This is a test message.\r\nThis is another line.\r\n",
				Encoding.UTF8.GetString(clear));
		}

		[Test]
		public void DecryptHiddenRecipient()
		{
			var asc = @"-----BEGIN PGP MESSAGE-----
Version: GnuPG v1.4.13 (MingW32)

hQEMAwAAAAAAAAAAAQgAjccvC3wJSL/FDp0yBkq3ktYHzFDk31whvAG6JX2ZoJzP
4bqkPxEOYKFur+ioxHN1TKrwRMQsjAHrfsBVkWxFNx/UqDeZSGGlreRPcn/Vv83x
Cs8J/5OaE/8iVzXSplENPI8o7Ahb8+7ewllJdmzr6E0gOl5VCUoFTTLq8rkzXg6c
VzdFC71QMr0x+Ef37zJwxkBsRWOBq6Kdwa7o5DXeyVJ54QsBwJJXesF3zKKmjE8y
8kdzLJqbK78yzmif0fa/IVizf+rQTYhiFuqGdnKjIpNr5YVbLofTgDveNSTB7XOg
Yw5/Va1VG6Om1puPeVBAXwr3ZVwZehG4H5xL2dhFONJ+AejHz6CeT8Fz1badKW92
VFL2bm1FZYHwjZjwIGXMR1CAbE/rQs+iS9b2bNLWPOAg/FB5zcUQ6Cc7IkH4hd3H
4PWtXsmwqLB1kKl+DYvy16rCvFPhvHdJO0Z3z/yn5520yx+TQ0brUWGUEkF0WbMR
3ioXxFhpcXhmPmTYqctK
=s0gQ
-----END PGP MESSAGE-----
";

			var context = new CryptoContext(GetPasswordCallback, Pubring, Secring, "rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			var clear = crypto.DecryptAndVerify(Encoding.UTF8.GetBytes(asc));

			Assert.NotNull(clear);
			Assert.AreEqual(File.ReadAllText(Path.Combine(Data, "msg1.bin")),
				Encoding.UTF8.GetString(clear));
		}

		[Test]
		public void DecryptRsa()
		{
			var asc = @"-----BEGIN PGP MESSAGE-----
Version: GnuPG v1.4.13 (MingW32)

hQEMA77VyJ6POr+OAQgAgdI5IJmGU7yy4HmT9KJ1ILyXLP7jYmCyXRIqfKYzJSVC
f1ftizgK+mcyhKSDkGWfG5iQrrujEWC49eYbRFdleHm7Nc1Vh1hrL3I4u+6pBMr+
DpgkUPdl0X1JT6DkD6szhuNHKSoubGCS4W4ix4ajLhjUd83AppdC0SZPDqEKB928
W5MN0GqCbKXexS5cJlvMpCzC+VgNRY+OHvP/QMYDWtiva6+hqcz6nXqOWGL5uSA6
hydPHzcdVr/U4oyIiy1d+U8mDBVf63fjDL0fyz5+1BwvCDajo53UaBBhm6x9Ypi0
gUUeoPAop6rwdLCOidJnkZ9urL0aVP8BGkuYpHu6MdJ/AcDyLO+SKpIZB/jagAYx
X9umSv/cUrgsWLW33XG1mrN1AnoPmGxnXonmjaSjGg+gx23iwfAoJ7FgbiQwsjEG
uZO20xfs+U01YMTTxUM7mjYVW0vbH48bTNxTmEKOF4ok5GBE/1ZUBWKjIM1gAo9Z
pc+96NTCnChekF7WxjbcVQ==
=ARyr
-----END PGP MESSAGE-----
";

			var context = new CryptoContext(GetPasswordCallback, Pubring, Secring, "rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			var clear = crypto.DecryptAndVerify(Encoding.UTF8.GetBytes(asc));

			Assert.NotNull(clear);
			Assert.AreEqual(File.ReadAllText(Path.Combine(Data, "msg1.bin")),
				Encoding.UTF8.GetString(clear));
		}

		[Test]
		public void DecryptAndVerifyRsa()
		{
			var asc = @"-----BEGIN PGP MESSAGE-----
Version: GnuPG v1.4.13 (MingW32)

hQEMA++lDjl5SynZAQf9EJhlb+wtv2/uagrx9TQwlj8PVewiY42F2+xVI6CWGqWH
2nraSkrEf4WXebZhVFmJUjK8f1MkqoRSBnUuGw3QzE5SkI4l13qvebqrZT2P1eTF
VesWuV0Fgy4qI0LHQs99q/SE0fAbYdsveiPucOwrNw1Wui77VFXQpklvJDG3PpZJ
zlL1842S83enQZ07gJHwFq6E/nm72be8wEIZAXtKFx1KIq0Is4JBVRIMymbljTWr
+V3bSRVhZvNVdNYgrQirH5zW6LZaloMHQhuvs2BWburgGBut5TxjmA93OwD0Le8r
Uc/ghOKEPa4GsIm5cNe+Iw5JNIUl/cjamrqULzc+YdLA8QGN2VCkm8upBIPR1Zpq
9T4EX+mhysUL31xedxki8mIh36DIdg5tu/2/6OggU5bm630Dv85YtstN40grN5oy
eUyhp/tnwWBs39ECGrIwgaNx5l9r3gD1UWZNg5rRd/wnIzCOHA7vVkfr9kEE0x1q
6voJxk/c4XBm3WMpXqSeuCpfLOzdaQFepWl0zymTogW2BGozbsVTMalHDOppXeWx
yikNljRB9NarZrnXLTvZXwRPlelO08/H7pq971RWYMl6vrzPK26/jwu6SYuLQS+B
4iazgNiJuoI2zc05cw8uq2G65cSJJj8bmHPJafr6BtGKjfkc+h4/OFt7zW9l1Be6
KeH+Q4pcdvjjejfuykzvDna+bIC4+Fq7PWairE0eaF9QAv+M2hKnoj34EdPbp1w1
pq+QloQyFU3v4mVrzaq6mTOqfDHsIIz41P401wTbRFnS19bGS3TObYd51MfDTftV
Jznp6f0w6cOYxlviTG0V0Q2QumCJ3WUZzDi4FyBLNn9bOX50Cb/1xFG+rlT/PBLY
DfLUSL37Hf3oOiLW1P1wTlJikWeIZfv4eSlaeAdIFCtML5g=
=oSWy
-----END PGP MESSAGE-----
";

			var context = new CryptoContext(GetPasswordCallback, Pubring, Secring, "rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			var clear = crypto.DecryptAndVerify(Encoding.UTF8.GetBytes(asc));

			Assert.NotNull(clear);
			Assert.AreEqual(File.ReadAllText(Path.Combine(Data, "msg1.bin")),
				Encoding.UTF8.GetString(clear));
		}

		[Test]
		public void DecryptAndVerifyHiddenRecipientRsa()
		{
			var asc = @"-----BEGIN PGP MESSAGE-----
Version: GnuPG v1.4.13 (MingW32)

hQEMAwAAAAAAAAAAAQf+O/YreMkRPsjSlTKUjsELPkvLix7BeliDXvbpH/oCjf85
IP9uC5WQrCenvHiMkOYCYDqZ87BMTXvw5rjGZHhT73LVygJ8QXgpUCnFr6WJrv9D
58hoo5zXJVGJz4288nyi0iSMsTGsPPqCxT5nAEVBa/yuoJwAfHFC8vOGmneQgzuf
VaGFAVgNwSMx2jNrfipmQ9HPjZhyf8eVIQ4sO2K6uwNffsq1rkZp/ioQ1CTreIU2
OCmlJdZElM2RjkFlURGT5heXC8tUwclVAd3sDVfWQAGdXW2lpamIUgWdj7Sy48jn
wXnrW582QkHZNnzVcVq3x64TKUlbPl40SRaEchuhd9LA8QGeQ2ca8k5XX1acQj00
8wnZTm1m9DIe6ztqmemqTdOCopsV5HLRjBGMWTr8qi5bDH7mFJilRWrHQdro1izY
FJOD2ZLbAMYaYyUu1kEBhGYiaO1HLEyMjjFhnp+hULSE89a5tFs4Wk3ZvUqUoE2e
3GBxOVp5xaqCpDiYlf65PiSdKF9P1xmdmivFGqsSN1zuhUiXBIaUy3X65qLhpIT/
krfW/VTHMx/3pzdgrkFtOgNlauZ/9SOA6BvmYR433tR3KKJcdT76P9GJiOWaoAXd
RyF9w8r0V39qk0fVO1P3zK4CHnInLJlrMx41Ghsp5kKAhrUd4yIsbiUEHfNOpnqm
SpY248YkMC+gKMufTyH3AL7J4gCf8Xp1wIB5ZvsOECexlXBiID3vTKt2x3M6TAIJ
tOO+dpjwLi503izL2IStiPcGXqtdy7gG/Ca4jbcZXqh0bMz+ArOkfthazyjlLs3r
PgNIbeABQgWE+kOKZSMa6eqUMMa/JEak5AWTO3Rno0MPxZ5FXlsOljuuWx2J159M
3tFsO7a+UUM5fGH761PUA9PEx2lFsy6c4NGbI4ckYo+ACEI=
=W5+P
-----END PGP MESSAGE-----
";

			var context = new CryptoContext(GetPasswordCallback, Pubring, Secring, "rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			var clear = crypto.DecryptAndVerify(Encoding.UTF8.GetBytes(asc));

			Assert.NotNull(clear);
			Assert.AreEqual(File.ReadAllText(Path.Combine(Data, "msg1.bin")),
				Encoding.UTF8.GetString(clear));
		}

		[Test]
		public void DecryptRsaNoKeyFound()
		{
			var asc = @"-----BEGIN PGP MESSAGE-----
Version: GnuPG v1.4.13 (MingW32)

hQEMA2si+TwJAdWiAQgAmbzqGlG1+Kykb8BaoqEThxxxJgyyW1TdskmiQXWGOhbx
8L6jAHloQjcYHPkQvsUY+fXZtvZVQEsuH+/XIWgB1Z7WEM2I+koJa20NyoF5SLFo
J+rhWRqth/BkmbzH+lx2UssENBDTrskWzm+9SXrw1B3NH+AzB5mVAvvVnIEbG/mY
w4mxV9lea2LeVMskHxoXnvXJtzpH9niTBDlN8hrNB0HaX8IK1fZ2z1Np7ckQ8Osv
mHDyWc8nzIjPecgeVVriZ4RcExAlVxgAdoEG1wgBUj6LlsVIuQ3moD52pI8P61xf
ysPatezVC6RE5A1JOVrjRMOF1jCxqS2b8BfcVmXsCNJ+AYSYCD1NPJAv4/y3UmIm
9X/V8ZiaAurFpH5Vc0tEBR9vaiD8IEawoSeu/ALXnVzq7tCAozxKfL1TM7d1kRG0
9kKUcstxJ14D44fDjfRDhkw2nZqp60ZYm7WkM1E8CmFMGx7HyXg9q/mhew5F/hfV
fby+XTLHp2GRtgsSJUdb
=y/8S
-----END PGP MESSAGE-----
";

			var context = new CryptoContext(GetPasswordCallback, Pubring, Secring, "rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			try
			{
				crypto.DecryptAndVerify(Encoding.UTF8.GetBytes(asc));
			}
			catch (SecretKeyNotFoundException)
			{
				return;
			}

			Assert.True(false);
		}

		[Test]
		public void DecryptHiddenRecipientRsaNoKeyFound()
		{
			var asc = @"-----BEGIN PGP MESSAGE-----
Version: GnuPG v1.4.13 (MingW32)

hQEMAwAAAAAAAAAAAQgArP3V+DuoFP20hrZAAsX4+Uiiv1F4MG36dXzUcUFPFLjG
dyXC0D9mbSzt2FFGzgxMsJUf3vv1vhA6bPgmAlOKX+GVLOAdO3Tm7gt1p3i0fWvL
D0q3YVnq2U2Uiq3D5oy79bH1fjKT0lygruSipD9rntN7ZcGbzh7oaug3uyjAGm6K
Z9LbPJihdb84l4EfDnowzO3Nr5Tuxmz/e982fXEVHumozMJqEQz7dTovOBR3WovI
+Sj1uDfshPTr1xxmCxvNW70/3w0TC9Uo6RbG/LatO0vjUWrelic3SStxz+FNrEJz
KDBrN+Btdi7/RoiZaGrn7SFT6mRz68XJaz7iFTWXaNJ+ATmUzo0EJ4DmGqCFlzu5
8KqAf1fiTgW5eKOa1gAlbPvqMlUc85+8/P8xuzvLpQgHPSCWK0+lDXwaxcVwfs0l
XzDCLj3rHgbG9CcLlYG1fL0nkTg9G0sKCF+mldRO/H+2qWyy9/ce/O7OfVuVb0iv
9vyYxzh8E7P/4aqjnbWE
=m8iN
-----END PGP MESSAGE-----
";

			var context = new CryptoContext(GetPasswordCallback, Pubring, Secring, "rsa", "sha-1");
			var crypto = new PgpCrypto(context);

			try
			{
				crypto.DecryptAndVerify(Encoding.UTF8.GetBytes(asc));
			}
			catch (SecretKeyNotFoundException)
			{
				return;
			}

			Assert.True(false);
		}
	}
}
