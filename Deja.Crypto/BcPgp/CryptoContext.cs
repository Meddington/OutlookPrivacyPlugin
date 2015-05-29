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
		readonly string _publicFilename = "pubring.gpg";
		readonly string _privateFilename = "secring.gpg";

		/// <summary>
		/// Return password for provided key
		/// </summary>
		/// <param name="key">Private key to provide password for.</param>
		/// <returns>Password to secret key</returns>
		public delegate char[] GetPasswordCallback(PgpSecretKey masterKey, PgpSecretKey key);

		public CryptoContext()
		{
			IsEncrypted = false;
			IsSigned = false;
			SignatureValidated = false;
			IsCompressed = false;
			FailedIntegrityCheck = true;
			Cipher = "AES-128";
			Digest = "SHA-1";

			GetPasswordCallback PasswordCallback = null;
			OnePassSignature = null;
			Signature = null;

			List<string> gpgLocations = new List<string>();

			// If GNUPGHOME is set, add to list
			var gpgHome = System.Environment.GetEnvironmentVariable("GNUPGHOME");
			if (gpgHome != null)
				gpgLocations.Add(gpgHome);

			// If registry key is set, add to list
			using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\GNU\GnuPG"))
			{
				if (key != null)
				{
					gpgHome = key.GetValue("HomeDir", null) as string;

					if (gpgHome != null)
						gpgLocations.Add(gpgHome);
				}
			}

			// Add default location to list
			gpgHome = System.Environment.GetEnvironmentVariable("APPDATA");
			gpgHome = Path.Combine(gpgHome, "gnupg");
			gpgLocations.Add(gpgHome);

			// many systems also use C:\Users\[name]\.gnupg as well.
			gpgHome = System.Environment.GetEnvironmentVariable("USERPROFILE");
			gpgHome = Path.Combine(gpgHome, ".gnupg");
			gpgLocations.Add(gpgHome);


			// Try all possible locations
			foreach(var home in gpgLocations)
			{
				if (File.Exists(Path.Combine(home, _privateFilename)))
				{
					PublicKeyRingFile = Path.Combine(home, _publicFilename);
					PrivateKeyRingFile = Path.Combine(home, _privateFilename);
					return;
				}

				// Portable gnupg will use a subfolder named 'home'
				if (File.Exists(Path.Combine(home, "home", _privateFilename)))
				{
					PublicKeyRingFile = Path.Combine(home, "home", _publicFilename);
					PrivateKeyRingFile = Path.Combine(home, "home", _privateFilename);
					return;
				}
			}

			// failed to find keyrings!
			throw new ApplicationException("Error, failed to locate keyrings! Please specify location using GNUPGHOME environmental variable.");
		}

		public CryptoContext(GetPasswordCallback passwordCallback, string cipher, string digest)
			: this()
		{
			PasswordCallback = passwordCallback;
			Digest = digest;
			Cipher = cipher;
		}

		public CryptoContext(GetPasswordCallback passwordCallback, string publicKeyRing, string secretKeyRing,
			string cipher, string digest)
			: this(passwordCallback, cipher, digest)
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
			Cipher = context.Cipher;
			Digest = context.Digest;

			PasswordCallback = context.PasswordCallback;
			PublicKeyRingFile = context.PublicKeyRingFile;
			PrivateKeyRingFile = context.PrivateKeyRingFile;
		}

		public GetPasswordCallback PasswordCallback { get; set; }

		public string Cipher { get; set; }
		public string Digest { get; set; }

        public string PublicKeyRingFile { get; set; }
        public string PrivateKeyRingFile { get; set; }

		public bool FailedIntegrityCheck { get; set; }
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
				var crypto = new PgpCrypto(new CryptoContext());
				PgpPublicKey key = null;

				if (SignedBy == null)
				{
					if (OnePassSignature != null)
						key = crypto.GetPublicKey(OnePassSignature.KeyId);
					else
						return "Unknown KeyId";
				}
				else
				{
					key = crypto.GetPublicKey(SignedBy.KeyId);
				}

				var fingerPrint = key.GetFingerprint();
				var fingerPrintLength = fingerPrint.Length;
				var keyId =
					fingerPrint[fingerPrintLength - 4].ToString("X2") +
					fingerPrint[fingerPrintLength - 3].ToString("X2") +
					fingerPrint[fingerPrintLength - 2].ToString("X2") +
					fingerPrint[fingerPrintLength - 1].ToString("X2");

				return keyId;
			}
		}

        public PgpOnePassSignature OnePassSignature { get; set; }
        public PgpSignature Signature { get; set; }
        public PgpSecretKey SecretKey { get; set; }
    }
}

// end
