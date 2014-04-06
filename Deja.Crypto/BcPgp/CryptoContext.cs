using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Bcpg;

namespace Deja.Crypto.BcPgp
{
    public class CryptoContext
    {
		public CryptoContext()
		{
			IsEncrypted = false;
			IsSigned = false;
			SignatureValidated = false;
			IsCompressed = false;

			Password = null;
			OnePassSignature = null;
			Signature = null;

			var gpgHome = System.Environment.GetEnvironmentVariable("GNUPGHOME");
			if (gpgHome == null)
			{
				// Now try via registry
				var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\GNU\GnuPG");
				if (key != null)
				{
					gpgHome = key.GetValue("HomeDir", null) as string;
				}

				if (gpgHome == null)
				{
					// Now try default location
					gpgHome = System.Environment.GetEnvironmentVariable("APPDATA");
					gpgHome = Path.Combine(gpgHome, "gnupg");
				}
			}

			PublicKeyRingFile = Path.Combine(gpgHome, "pubring.gpg");
			PrivateKeyRingFile = Path.Combine(gpgHome, "secring.gpg");
		}

		public CryptoContext(char[] password) : this()
		{
			Password = password;
		}

		public CryptoContext(char[] password, string publicKeyRing, string secretKeyRing) : this(password)
		{
			PublicKeyRingFile = publicKeyRing;
			PrivateKeyRingFile = secretKeyRing;
		}

		public CryptoContext(CryptoContext context)
		{
			if (context == null)
				throw new Exception("Error, crypto context is null.");

			IsEncrypted = false;
			IsSigned = false;
			SignatureValidated = false;
			IsCompressed = false;
			OnePassSignature = null;
			Signature = null;
			SignedBy = null;

			Password = context.Password;
			PublicKeyRingFile = context.PublicKeyRingFile;
			PrivateKeyRingFile = context.PrivateKeyRingFile;
		}

		public char[] Password { get; set; }
        public string PublicKeyRingFile { get; set; }
        public string PrivateKeyRingFile { get; set; }

        public bool IsCompressed { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsSigned { get; set; }
        public bool SignatureValidated { get; set; }
		public PgpPublicKey SignedBy{ get; set; }
		public string SignedByUserId
		{
			get
			{
				if (SignedBy == null)
					return "Missing Key";

				string lastId = null;

				foreach (string id in SignedBy.GetUserIds())
				{
					lastId = id;
					if (id.IndexOf("@") > -1)
						return id;
				}

				return lastId;
			}
		}
		public string SignedByKeyId
		{
			get
			{
				if (SignedBy == null)
				{
					if (OnePassSignature != null)
					{
						return OnePassSignature.KeyId.ToString("X");
					}
					else
						return "Unknown KeyId";
				}

				return SignedBy.KeyId.ToString("X");
			}
		}

        public PgpOnePassSignature OnePassSignature { get; set; }
        public PgpSignature Signature { get; set; }
        public PgpSecretKey SecretKey { get; set; }
    }
}

// end
