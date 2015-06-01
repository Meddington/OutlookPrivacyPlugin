using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Bcpg.Sig;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Bcpg;

using NLog;

namespace Deja.Crypto.BcPgp
{
	/// <summary>
	/// Implement OpenPGP Crypto using BouncyCastle API
	/// </summary>
	public class PgpCrypto
	{
		static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

		public PgpCrypto(CryptoContext context)
		{
			Context = context;
		}

		public CryptoContext Context { get; set; }

		#region Key Management

		public bool IsSigningAlg(PgpPublicKey key)
		{
			var alg = key.Algorithm;
			switch (alg)
			{
				case PublicKeyAlgorithmTag.Dsa:
				case PublicKeyAlgorithmTag.RsaSign:
				case PublicKeyAlgorithmTag.ECDsa:
					return true;
			}

			return true;
		}

		public bool IsEncryptionAlg(PgpPublicKey key)
		{
			var alg = key.Algorithm;
			switch (alg)
			{
				case PublicKeyAlgorithmTag.DiffieHellman:
				case PublicKeyAlgorithmTag.EC:
				case PublicKeyAlgorithmTag.ElGamalEncrypt:
				case PublicKeyAlgorithmTag.RsaEncrypt:
					return true;
			}

			return false;
		}

		internal SymmetricKeyAlgorithmTag GetSymAlgTagFromString(string alg)
		{
			switch(alg.ToLower())
			{
				case "cast5":
					return SymmetricKeyAlgorithmTag.Cast5;
				case "aes-128":
					return SymmetricKeyAlgorithmTag.Aes128;
				case "aes-192":
					return SymmetricKeyAlgorithmTag.Aes192;
				case "aes-256":
					return SymmetricKeyAlgorithmTag.Aes256;
			}

			throw new InvalidParameterException("Error, '" + alg + "' is an invalid encryption algorithm.");

		}

		internal HashAlgorithmTag GetHashAlgTagFromString(string alg)
		{
			switch(alg.ToLower())
			{
				case "sha-1":
					return HashAlgorithmTag.Sha1;
				case "sha-224":
					return HashAlgorithmTag.Sha224;
				case "sha-256":
					return HashAlgorithmTag.Sha256;
				case "sha-384":
					return HashAlgorithmTag.Sha384;
				case "sha-512":
					return HashAlgorithmTag.Sha512;
			}

			throw new InvalidParameterException("Error, '"+alg+"' is an invalid digest algorithm.");
		}

		/// <summary>
		/// Is key allowed for signing?
		/// </summary>
		/// <remarks>
		/// Checks both key algorithm and also key flags.
		/// </remarks>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsSigningKey(PgpPublicKey key)
		{
			foreach (PgpSignature sig in key.GetSignatures())
			{
				var hashedSubPackets = sig.GetHashedSubPackets();
				if (hashedSubPackets == null)
					continue;

				var keyFlags = hashedSubPackets.GetKeyFlags();

				if ((keyFlags & KeyFlags.SignData) > 0)
					return true;
			}

			if (IsSigningAlg(key))
				return true;

			return false;
		}

		/// <summary>
		/// Is key allowed for encryption?
		/// </summary>
		/// <remarks>
		/// Checks both key algorithm and also key flags.
		/// </remarks>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsEncryptionKey(PgpPublicKey key)
		{
			// First check key use flags
			foreach (PgpSignature sig in key.GetSignatures())
			{
				var hashedSubPackets = sig.GetHashedSubPackets();
				if (hashedSubPackets == null)
					continue;

				var keyFlags = hashedSubPackets.GetKeyFlags();

				if ((keyFlags & KeyFlags.EncryptComms) > 0)
					return true;

				if ((keyFlags & KeyFlags.EncryptStorage) > 0)
					return true;
			}

			// NOTE: Some keys do not have flags set. Instead use
			//       the alg type. But only those specific to encryption.

			if (IsEncryptionAlg(key))
				return true;

			return false;
		}

		/// <summary>
		/// Perform basic validation of key. Check if revoked or expired.
		/// </summary>
		/// <param name="pubKey"></param>
		/// <returns></returns>
		public bool IsKeyValid(PgpPublicKey pubKey)
		{
			if (pubKey.IsRevoked())
				return false;

			// Check if key has expired
			if (pubKey.GetValidSeconds() != 0)
			{
				var expireTime = pubKey.CreationTime.AddSeconds(pubKey.GetValidSeconds());
				if (DateTime.Now > expireTime)
					return false;
			}

			return true;
		}

		/// <summary>
		/// Perform basic validation of key. Check if revoked or expired.
		/// </summary>
		/// <param name="pubKey"></param>
		/// <returns></returns>
		public bool IsKeyValid(PgpSecretKey secKey)
		{
			return IsKeyValid(secKey.PublicKey);
		}

		public PgpPublicKey[] GetPublicKeyUserIdsForEncryption()
		{
			logger.Debug("GetPublicKeyUserIdsForEncryption");

			using (var inputStream = File.OpenRead(Context.PublicKeyRingFile))
			using (var decoderStream = PgpUtilities.GetDecoderStream(inputStream))
			{
				var pgpPub = new PgpPublicKeyRingBundle(decoderStream);
				var keyUserIds = new List<PgpPublicKey>();

				foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
				{
					foreach (PgpPublicKey k in kRing.GetPublicKeys())
					{
						// Break on key with specific user id. For testing only.
						//var idIter = GetMasterPublicKey(k.KeyId).GetUserIds().GetEnumerator();
						//idIter.MoveNext();
						//if(((string)idIter.Current).StartsWith("And"))
						//{
						//	System.Diagnostics.Debugger.Break();
						//}

						if (!IsKeyValid(k))
							continue;

						if (!IsEncryptionKey(k))
							continue;

						var masterKey = k;
						if (!k.IsMasterKey)
							masterKey = GetMasterPublicKey(k.KeyId);

						if (!keyUserIds.Any(kk => kk.KeyId == masterKey.KeyId))
							keyUserIds.Add(masterKey);
					}
				}

				return keyUserIds.ToArray();
			}
		}

		public string[] GetPublicKeyUserIdsForSign()
		{
			logger.Debug("GetPublicKeyUserIdsForSign");

			using (var inputStream = File.OpenRead(Context.PublicKeyRingFile))
			using (var decoderStream = PgpUtilities.GetDecoderStream(inputStream))
			{
				var pgpPub = new PgpPublicKeyRingBundle(decoderStream);
				var keyUserIds = new List<string>();

				foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
				{
					foreach (PgpPublicKey k in kRing.GetPublicKeys())
					{
						if (!IsKeyValid(k) || !IsSigningKey(k))
							continue;

						foreach (string id in k.GetUserIds())
						{
							if(!keyUserIds.Contains(id))
								keyUserIds.Add(id);
						}
					}
				}

				return keyUserIds.ToArray();
			}
		}

		public string[] GetSecretKeyUserIds()
		{
			logger.Debug("GetSecretKeyUserIds");

			using (var inputStream = File.OpenRead(Context.PrivateKeyRingFile))
			using (var decoderStream = PgpUtilities.GetDecoderStream(inputStream))
			{
				var pgpPub = new PgpSecretKeyRingBundle(decoderStream);
				var keyUserIds = new List<string>();

				foreach (PgpSecretKeyRing kRing in pgpPub.GetKeyRings())
				{
					foreach (PgpSecretKey k in kRing.GetSecretKeys())
					{
						var masterKey = k;
						if (!k.IsMasterKey)
							masterKey = GetMasterSecretKey(k.KeyId);

						foreach (string id in masterKey.UserIds)
						{
							if (!keyUserIds.Contains(id))
								keyUserIds.Add(id);
						}
					}
				}

				return keyUserIds.ToArray();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// PgpPublicKeyRing is a key set. The first key is the master key, rest are sub-keys.
		/// </remarks>
		/// <param name="email"></param>
		/// <returns></returns>
		public PgpPublicKey GetPublicKeyForEncryption(string email)
		{
			logger.Debug("GetPublicKeyForEncryption: {0}", email);

			using (var inputStream = File.OpenRead(Context.PublicKeyRingFile))
			using (var decoderStream = PgpUtilities.GetDecoderStream(inputStream))
			{
				var pgpPub = new PgpPublicKeyRingBundle(decoderStream);
				var emailSearch = "<" + email.ToLower().Trim() + ">";

				// Each KeyRing is a single key set (master + subs)
				foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
				{
					var masterKey = kRing.GetPublicKey();
					if (!masterKey.IsMasterKey)
						masterKey = GetMasterPublicKey(masterKey.KeyId);

					// Skip key's that don't match
					if (!masterKey.GetUserIds().Cast<string>().Any(id => id.ToLower().Contains(emailSearch)))
					{
						logger.Debug("Skipping key ring: {0}", masterKey.KeyId.ToString("X"));
						continue;
					}

					logger.Debug("Found correct master key, searching for encryption key...");

					PgpPublicKey encryptionKey = null;
					foreach (PgpPublicKey k in kRing.GetPublicKeys())
					{
						if (!IsKeyValid(k) || !IsEncryptionKey(k))
							continue;

						// Prefer sub-keys to master keys
						if (!k.IsMasterKey)
							return k;

						// Use master if no other options available.
						encryptionKey = k;
					}

					if (encryptionKey != null)
							return encryptionKey;
				}

				return null;
			}
		}

		public PgpPublicKey GetPublicKey(long keyId)
		{
			logger.Debug("GetPublicKey: {0}", keyId);

			using (var inputStream = File.OpenRead(Context.PublicKeyRingFile))
			using (var decodeStream = PgpUtilities.GetDecoderStream(inputStream))
			{
				var pgpPub = new PgpPublicKeyRingBundle(decodeStream);

				foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
				{
					foreach (PgpPublicKey k in kRing.GetPublicKeys())
					{
						if (k.KeyId == keyId)
							return k;
					}
				}

				return null;
			}
		}

		public PgpPublicKey GetMasterPublicKey(long keyId)
		{
			logger.Debug("GetMasterPublicKey: {0}", keyId);

			using (var inputStream = File.OpenRead(Context.PublicKeyRingFile))
			using (var decodeStream = PgpUtilities.GetDecoderStream(inputStream))
			{
				var pgpPub = new PgpPublicKeyRingBundle(decodeStream);

				foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
				{
					// The master key is normally the first key returned.
					var masterKey = kRing.GetPublicKey();
					if(!masterKey.IsMasterKey)
					{
						foreach (PgpPublicKey k in kRing.GetPublicKeys())
							if (k.IsMasterKey)
								masterKey = k;
					}

					foreach (PgpPublicKey k in kRing.GetPublicKeys())
					{
						if (k.KeyId == keyId)
							return masterKey;
					}
				}

				return null;
			}
		}

		public PgpSecretKey GetSecretKeyForEncryption(string email)
		{
			logger.Debug("GetSecretKeyForEncryption: {0}", email);

			if (email == null)
				return null;

			if (Context.PrivateKeyRingFile == null || !File.Exists(Context.PrivateKeyRingFile))
				return null;

			using (var inputStream = File.OpenRead(Context.PrivateKeyRingFile))
			using (var decoderStream = PgpUtilities.GetDecoderStream(inputStream))
			{
				var pgpPub = new PgpSecretKeyRingBundle(decoderStream);
				var emailSearch = "<" + email.ToLower().Trim() + ">";

				if (pgpPub == null)
					return null;

				foreach (PgpSecretKeyRing kRing in pgpPub.GetKeyRings())
				{
					var masterKey = kRing.GetSecretKey();
					if (!masterKey.IsMasterKey)
						masterKey = GetMasterSecretKey(masterKey.KeyId);

					// Skip key's that don't match
					if (!masterKey.UserIds.Cast<string>().Any(id => id.ToLower().Contains(emailSearch)))
						continue;

					PgpSecretKey encryptionKey = null;
					foreach (PgpSecretKey k in kRing.GetSecretKeys())
					{
						if (!IsEncryptionKey(k.PublicKey) || !IsKeyValid(k))
							continue;

						// prefer sub-key over master key
						if(!k.IsMasterKey)
							return k;

						encryptionKey = k;
					}

					if(encryptionKey != null)
						return encryptionKey;
				}

				return null;
			}
		}

		public PgpSecretKey GetSecretKeyForSigning(string email, out PgpSecretKey masterKey)
		{
			logger.Debug("GetSecretKeyForSigning: {0}", email);
			masterKey = null;

			using (var inputStream = File.OpenRead(Context.PrivateKeyRingFile))
			using (var decoderStream = PgpUtilities.GetDecoderStream(inputStream))
			{
				var pgpPub = new PgpSecretKeyRingBundle(decoderStream);
				var emailSearch = "<" + email.ToLower().Trim() + ">";

				foreach (PgpSecretKeyRing kRing in pgpPub.GetKeyRings())
				{
					masterKey = kRing.GetSecretKey();
					if (!masterKey.IsMasterKey)
						masterKey = GetMasterSecretKey(masterKey.KeyId);

					// Skip key's that don't match
					if (!masterKey.UserIds.Cast<string>().Any(id => id.ToLower().Contains(emailSearch)))
						continue;

					PgpSecretKey signingKey = null;
					foreach (PgpSecretKey k in kRing.GetSecretKeys())
					{
						if (!IsSigningKey(k.PublicKey) || !IsKeyValid(k))
							continue;

						// Prever sub-key over master key
						if (!k.IsMasterKey)
							return k;

						signingKey = k;
					}

					if (signingKey != null)
						return signingKey;
				}

				return null;
			}
		}

		public PgpSecretKey GetMasterSecretKey(long subKeyId)
		{
			using (var inputStream = File.OpenRead(Context.PrivateKeyRingFile))
			using (var decoderStream = PgpUtilities.GetDecoderStream(inputStream))
			{
				var pgpPub = new PgpSecretKeyRingBundle(decoderStream);

				foreach (PgpSecretKeyRing kRing in pgpPub.GetKeyRings())
				{
					var masterKey = kRing.GetSecretKey();
					if(!masterKey.IsMasterKey)
					{
						foreach (PgpSecretKey k in kRing.GetSecretKeys())
						{
							if (k.IsMasterKey)
								masterKey = k;
						}
					}

					foreach (PgpSecretKey k in kRing.GetSecretKeys())
					{
						if (k.KeyId == subKeyId)
							return masterKey;
					}
				}

				return null;
			}
		}

		public PgpSecretKey GetSecretKey(long id)
		{
			logger.Debug("GetSecretKey: {0}", id);

			using (var inputStream = File.OpenRead(Context.PrivateKeyRingFile))
			using (var decoderStream = PgpUtilities.GetDecoderStream(inputStream))
			{
				var pgpPub = new PgpSecretKeyRingBundle(decoderStream);

				foreach (PgpSecretKeyRing kRing in pgpPub.GetKeyRings())
				{
					foreach (PgpSecretKey k in kRing.GetSecretKeys())
					{
						if (k.KeyId == id)
							return k;
					}
				}

				return null;
			}
		}

		#endregion

		/// <summary>
		/// Sign data using key
		/// </summary>
		/// <param name="data">Data to sign</param>
		/// <param name="key">Email address of key</param>
		/// <returns>Returns ascii armored signature</returns>
		public string Sign(byte[] data, string key, Dictionary<string, string> headers)
		{
			logger.Trace("Sign({0}, {1})", data.Length, key);

			Context = new CryptoContext(Context);

			PgpSecretKey senderMasterKey;
			var senderKey = GetSecretKeyForSigning(key, out senderMasterKey);
			if (senderKey == null)
				throw new SecretKeyNotFoundException("Error, unable to locate signing key \"" + key + "\".");

			var compressedData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
			var literalData = new PgpLiteralDataGenerator();

			// Setup signature stuff //
			var tag = senderKey.PublicKey.Algorithm;
			var signatureData = new PgpSignatureGenerator(tag, GetHashAlgTagFromString(Context.Digest));
			signatureData.InitSign(PgpSignature.BinaryDocument, senderKey.ExtractPrivateKey(Context.PasswordCallback(senderMasterKey, senderKey)));

			foreach (string userId in senderKey.PublicKey.GetUserIds())
			{
				var subPacketGenerator = new PgpSignatureSubpacketGenerator();

				subPacketGenerator.SetSignerUserId(false, userId);
				signatureData.SetHashedSubpackets(subPacketGenerator.Generate());

				// Just the first one!
				break;
			}
			// //

			using (var sout = new MemoryStream())
			{
				using (var armoredOut = new ArmoredOutputStream(sout))
				{
					foreach (var header in headers)
						armoredOut.SetHeader(header.Key, header.Value);

					using (var compressedOut = compressedData.Open(armoredOut))
					using (var outputStream = new BcpgOutputStream(compressedOut))
					{
						signatureData.GenerateOnePassVersion(false).Encode(outputStream);

						using (var literalOut = literalData.Open(outputStream, 'b', "", data.Length, DateTime.Now))
						{
							literalOut.Write(data, 0, data.Length);
							signatureData.Update(data);
						}

						signatureData.Generate().Encode(outputStream);
					}
				}

				return ASCIIEncoding.ASCII.GetString(sout.ToArray());
			}
		}

		/// <summary>
		/// Sign data using key
		/// </summary>
		/// <param name="data">Data to sign</param>
		/// <param name="key">Email address of key</param>
		/// <returns>Returns ascii armored signature</returns>
		public string SignClear(string data, string key, Encoding encoding, Dictionary<string, string> headers)
		{
			Context = new CryptoContext(Context);

			PgpSecretKey senderMasterKey;
			var senderKey = GetSecretKeyForSigning(key, out senderMasterKey);
			if (senderKey == null)
				throw new SecretKeyNotFoundException("Error, unable to locate signing key \"" + key + "\".");

			// Setup signature stuff //
			var signatureData = new PgpSignatureGenerator(senderKey.PublicKey.Algorithm, GetHashAlgTagFromString(Context.Digest));
			signatureData.InitSign(PgpSignature.CanonicalTextDocument, senderKey.ExtractPrivateKey(Context.PasswordCallback(senderMasterKey, senderKey)));

			foreach (string userId in senderKey.PublicKey.GetUserIds())
			{
				var subPacketGenerator = new PgpSignatureSubpacketGenerator();

				subPacketGenerator.SetSignerUserId(false, userId);
				signatureData.SetHashedSubpackets(subPacketGenerator.Generate());

				// Just the first one!
				break;
			}

			// //

			using (var sout = new MemoryStream())
			{
				using (var armoredOut = new ArmoredOutputStream(sout))
				{
					foreach (var header in headers)
						armoredOut.SetHeader(header.Key, header.Value);

					armoredOut.BeginClearText(GetHashAlgTagFromString(Context.Digest));

					// Remove any extra trailing whitespace.
					// this should not include \r or \n.
					data = data.TrimEnd(null);

					using (var stringReader = new StringReader(data))
					{
						do
						{
							var line = stringReader.ReadLine();
							if (line == null)
								break;

							// Lines must have all white space removed
							line = line.TrimEnd(null);
							line = line.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

							line += "\r\n";

							signatureData.Update(encoding.GetBytes(line));
							armoredOut.Write(encoding.GetBytes(line));
						}
						while (true);
					}

					// Write extra line before signature block.
					armoredOut.Write(encoding.GetBytes("\r\n"));
					armoredOut.EndClearText();

					using (var outputStream = new BcpgOutputStream(armoredOut))
					{

						signatureData.Generate().Encode(outputStream);
					}
				}

				return encoding.GetString(sout.ToArray());
			}
		}

		public string PublicKey(string email, Dictionary<string, string> headers)
		{
			Context = new CryptoContext(Context);

			var publicKey = GetPublicKeyForEncryption(email);

			using (var sout = new MemoryStream())
			{
				using (var armoredOut = new ArmoredOutputStream(sout))
				{
					publicKey.Encode(armoredOut);
				}

				return ASCIIEncoding.ASCII.GetString(sout.ToArray());
			}
		}

		/// <summary>
		/// Encrypt data to a set of recipients
		/// </summary>
		/// <param name="data">Data to encrypt</param>
		/// <param name="recipients">List of email addresses</param>
		/// <param name="recipients">Headers to add to ascii armor</param>
		/// <returns>Returns ascii armored encrypted data</returns>
		public string Encrypt(byte[] data, IList<string> recipients, Dictionary<string, string> headers)
		{
			Context = new CryptoContext(Context);

			var compressedData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
			var literalData = new PgpLiteralDataGenerator();
			var cryptData = new PgpEncryptedDataGenerator(GetSymAlgTagFromString(Context.Cipher), true, new SecureRandom());

			foreach (var recipient in recipients)
			{
				var recipientKey = GetPublicKeyForEncryption(recipient);
				if (recipientKey == null)
					throw new PublicKeyNotFoundException("Error, unable to find recipient key \"" + recipient + "\".");

				cryptData.AddMethod(recipientKey);
			}

			using (var sout = new MemoryStream())
			{
				using (var armoredOut = new ArmoredOutputStream(sout))
				{
					foreach (var header in headers)
						armoredOut.SetHeader(header.Key, header.Value);

					using (var clearOut = new MemoryStream())
					{
						using (var compressedOut = compressedData.Open(clearOut))
						using (var literalOut = literalData.Open(
							compressedOut,
							PgpLiteralData.Binary,
							"email",
							data.Length,
							DateTime.UtcNow))
						{
							literalOut.Write(data, 0, data.Length);
						}

						var clearData = clearOut.ToArray();

						using (var encryptOut = cryptData.Open(armoredOut, clearData.Length))
						{
							encryptOut.Write(clearData, 0, clearData.Length);
						}
					}
				}

				return ASCIIEncoding.ASCII.GetString(sout.ToArray());
			}
		}

		public string SignAndEncryptText(byte[] data, string key, IList<string> recipients, Dictionary<string, string> headers)
		{
			return SignAndEncrypt(
				data,
				key,
				recipients,
				headers,
				false);
		}

		public string SignAndEncryptBinary(byte[] data, string key, IList<string> recipients, Dictionary<string, string> headers)
		{
			return SignAndEncrypt(
				data,
				key,
				recipients,
				headers,
				true);
		}

		/// <summary>
		/// Signs then encrypts data using key and list of recipients.
		/// </summary>
		/// <param name="data">Data to encrypt</param>
		/// <param name="key">Signing key</param>
		/// <param name="recipients">List of keys to encrypt to</param>
		/// <returns>Returns ascii armored signed/encrypted data</returns>
		protected string SignAndEncrypt(byte[] data, string key, IList<string> recipients, Dictionary<string, string> headers, bool isBinary)
		{
			Context = new CryptoContext(Context);

			var senderKey = GetSecretKeyForEncryption(key);
			if (senderKey == null)
				throw new SecretKeyNotFoundException("Error, Unable to locate sender encryption key \"" + key + "\".");

			PgpSecretKey senderMasterSignkey;
			var senderSignKey = GetSecretKeyForSigning(key, out senderMasterSignkey);
			if (senderSignKey == null)
				throw new SecretKeyNotFoundException("Error, Unable to locate sender signing key \"" + key + "\".");

			var compressedData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.ZLib);
			var literalData = new PgpLiteralDataGenerator();
			var cryptData = new PgpEncryptedDataGenerator(GetSymAlgTagFromString(Context.Cipher), true, new SecureRandom());

			foreach (var recipient in recipients)
			{
				var recipientKey = GetPublicKeyForEncryption(recipient);
				if (recipientKey == null)
					throw new PublicKeyNotFoundException("Error, unable to find recipient key \"" + recipient + "\".");

				cryptData.AddMethod(recipientKey);
			}

			// Setup signature stuff //
			var signatureData = new PgpSignatureGenerator(
				senderSignKey.PublicKey.Algorithm,
				GetHashAlgTagFromString(Context.Digest));
			signatureData.InitSign(
				isBinary ? PgpSignature.BinaryDocument : PgpSignature.CanonicalTextDocument,
				senderSignKey.ExtractPrivateKey(Context.PasswordCallback(senderMasterSignkey, senderSignKey)));

			foreach (string userId in senderKey.PublicKey.GetUserIds())
			{
				var subPacketGenerator = new PgpSignatureSubpacketGenerator();

				subPacketGenerator.SetSignerUserId(false, userId);
				signatureData.SetHashedSubpackets(subPacketGenerator.Generate());

				// Just the first one!
				break;
			}
			// //

			using (var sout = new MemoryStream())
			{
				using (var armoredOut = new ArmoredOutputStream(sout))
				{
					foreach (var header in headers)
						armoredOut.SetHeader(header.Key, header.Value);

					using (var clearOut = new MemoryStream())
					{
						using (var compressedOut = compressedData.Open(clearOut))
						{
							signatureData.GenerateOnePassVersion(false).Encode(compressedOut);

							using (var literalOut = literalData.Open(
								compressedOut,
								isBinary ? PgpLiteralData.Binary : PgpLiteralData.Text,
								"",
								data.Length,
								DateTime.UtcNow))
							{
								literalOut.Write(data, 0, data.Length);
								signatureData.Update(data, 0, data.Length);
							}

							signatureData.Generate().Encode(compressedOut);
						}

						var clearData = clearOut.ToArray();

						using (var encryptOut = cryptData.Open(armoredOut, clearData.Length))
						{
							encryptOut.Write(clearData, 0, clearData.Length);
						}
					}
				}

				return ASCIIEncoding.ASCII.GetString(sout.ToArray());
			}
		}

		/// <summary>
		/// Verify signature
		/// </summary>
		/// <param name="data">Data to verify</param>
		/// <returns>Return true if signature validates, else false.</returns>
		public bool Verify(byte[] data)
		{
			Context = new CryptoContext(Context);

			using (var dataIn = new MemoryStream(data))
			using (var armoredIn = new ArmoredInputStream(dataIn))
			{
				if (!armoredIn.IsClearText())
				{
					var factory = new PgpObjectFactory(armoredIn);

					var pgpObject = factory.NextPgpObject();
					if (pgpObject == null)
						return false;

					DecryptHandlePgpObject(pgpObject);
					if (Context.FailedIntegrityCheck)
						throw new VerifyException("Error, failed validation check.");

					if (!Context.IsSigned)
						throw new CryptoException("Error, message is not signed.");

					return Context.SignatureValidated;
				}
			}

			return VerifyClear(data);
		}

		/// <summary>
		/// Verify signature for cleartext (e.g. emails)
		/// </summary>
		/// <param name="data">Data to verify</param>
		/// <returns>Return true if signature validates, else false.</returns>
		public bool VerifyClear(byte[] data)
		{
			Context = new CryptoContext(Context);

			var crlf = new byte[] { (byte)'\r', (byte)'\n' };
			var encoding = ASCIIEncoding.UTF8;

			using (var dataIn = new MemoryStream(data))
			using (var armoredIn = new ArmoredInputStream(dataIn))
			{
				if (!armoredIn.IsClearText())
					throw new CryptoException("Error, message is not armored clear-text.");

				var headers = armoredIn.GetArmorHeaders();
				if (headers != null)
				{
					foreach (var header in headers)
					{
						if (Regex.IsMatch(header, @"Charset: ([^\s]*)"))
						{
							var encodingType = Regex.Match(header, @"Charset: ([^\s]*)").Groups[1].Value;
							encoding = Encoding.GetEncoding(encodingType);
						}
					}
				}

				using (var clearOut = new MemoryStream())
				{
					using (var clearIn = new MemoryStream())
					{
						int ch = 0;
						while ((ch = armoredIn.ReadByte()) >= 0 && armoredIn.IsClearText())
							clearIn.WriteByte((byte)ch);

						clearIn.Position = 0;

						using (var stringIn = new StringReader(encoding.GetString(clearIn.ToArray())))
						{
							do
							{
								var line = stringIn.ReadLine();
								if (line == null)
									break;

								line = line
									.TrimEnd(null)
									.TrimEnd(new char[] { ' ', '\t', '\n', '\r' })
									.TrimEnd(null)
									+ "\r\n";

								var buff = encoding.GetBytes(line);
								clearOut.Write(buff, 0, buff.Length);
							}
							while (true);
						}
					}

					clearOut.Position = 0;

					var factory = new PgpObjectFactory(armoredIn);
					var signatureList = (PgpSignatureList)factory.NextPgpObject();
					if (signatureList == null)
						return false;

					var signature = signatureList[0];

					Context.IsEncrypted = false;
					Context.IsSigned = true;
					Context.SignedBy = GetPublicKey(signature.KeyId);

					if (Context.SignedBy == null)
						throw new PublicKeyNotFoundException("Public key not found for key id \"" + signature.KeyId + "\".");

					signature.InitVerify(GetPublicKey(signature.KeyId));
					signature.Update(clearOut.ToArray(), 0, (int)(clearOut.Length - 2));
					Context.SignatureValidated = signature.Verify();

					return Context.SignatureValidated;
				}
			}
		}

		/// <summary>
		/// Decrypt and verify signature of data.
		/// </summary>
		/// <param name="data">Data to decrypt and verify</param>
		/// <returns>Returns decrypted data if signature verifies.</returns>
		public byte[] DecryptAndVerify(byte[] data, bool ignoreIntegrityCheck = false)
		{
			logger.Trace(string.Format("DecryptAndVerify({0}, {1})", data.Length, ignoreIntegrityCheck));

			Context = new CryptoContext(Context);

			var isArmored = ASCIIEncoding.ASCII.GetString(data).IndexOf("-----BEGIN PGP MESSAGE-----") > -1;

			using (var dataIn = new MemoryStream(data))
			{
				if (isArmored)
				{
					logger.Trace("isArmored");

					using (var armoredIn = new ArmoredInputStream(dataIn))
					{
						var factory = new PgpObjectFactory(armoredIn);

						while (true)
						{
							var obj = factory.NextPgpObject();
							if (obj == null)
								return null;

							if (obj is PgpMarker)
								continue;

							var ret = DecryptHandlePgpObject(obj);
							if (Context.FailedIntegrityCheck && !ignoreIntegrityCheck)
							{
								logger.Error("DecryptAndVerify: Data not integrity protected.");
								throw new VerifyException("Data not integrity protected.");
							}

							logger.Trace("DecryptAndVerify: Returning " +
								(ret == null ? "null" : ret.Length.ToString()) + " bytes");

							return ret;
						}
					}
				}
				else
				{
					var factory = new PgpObjectFactory(dataIn);

					while (true)
					{
						var obj = factory.NextPgpObject();
						if (obj == null)
							return null;

						if (obj is PgpMarker)
							continue;

						var ret = DecryptHandlePgpObject(obj);
						if (Context.FailedIntegrityCheck && !ignoreIntegrityCheck)
						{
							logger.Error("DecryptAndVerify: Data not integrity protected.");
							throw new VerifyException("Data not integrity protected.");
						}

						logger.Trace("DecryptAndVerify: Returning " +
							(ret == null ? "null" : ret.Length.ToString()) + " bytes");

						return ret;
					}
				}
			}
		}

		/// <summary>
		/// Recursive PGP Object handler.
		/// </summary>
		/// <param name="obj">Object to handle</param>
		/// <returns>Returns decrypted data if any</returns>
		byte[] DecryptHandlePgpObject(PgpObject obj)
		{
			logger.Trace("DecryptHandlePgpObject(" + obj.GetType().Name + ")");

			byte[] ret = null;

			if (obj is PgpEncryptedDataList)
			{
				logger.Trace("DecryptHandlePgpObject: IsEncrypted");
				Context.IsEncrypted = true;
				var dataList = obj as PgpEncryptedDataList;

				// Set once we have matched a keyid.
				bool secretKeyMatched = false;

				foreach (PgpPublicKeyEncryptedData encryptedData in dataList.GetEncryptedDataObjects())
				{
					try
					{
						// If we have already found a key to use, skip others. It is possible
						// to have all the keys in our ring.
						if (Context.SecretKey != null)
							continue;

						// NOTE: When content is encrypted to multiple recipients, only one of these blocks
						//       will match a known KeyId.  If a match is never made, then there is a problem :)

						var masterSecretKey = GetMasterSecretKey(encryptedData.KeyId);
						var secretKey = GetSecretKey(encryptedData.KeyId);

						if (masterSecretKey == null || secretKey == null)
							continue;

						var passphrase = Context.PasswordCallback(masterSecretKey, secretKey);

						// Incorrect passphrase or cancel
						if (passphrase == null)
							continue;

						Context.SecretKey = masterSecretKey;

						logger.Trace("DecryptHandlePgpObject: Found key: " + encryptedData.KeyId);
						secretKeyMatched = true;

						using (var cleartextIn = encryptedData.GetDataStream(
							secretKey.ExtractPrivateKey(passphrase)))
						{
							var clearFactory = new PgpObjectFactory(cleartextIn);
							var nextObj = clearFactory.NextPgpObject();
							if (nextObj == null)
								return null;

							var r = DecryptHandlePgpObject(nextObj);
							if (r != null)
								ret = r;
						}

						// This can fail due to integrity protection missing.
						// Legacy systems to not have this protection
						// Should make an option to ignore.
						try
						{
							if (!encryptedData.Verify())
							{
								logger.Debug("DecryptHandlePgpObject: encryptedData.Verify failed");
								throw new VerifyException("Verify of encrypted data failed!");
							}
						}
						catch (PgpException exx)
						{
							logger.Debug("DecryptHandlePgpObject: " + exx.Message);

							// Legacy systems do not have this protection
							// Exposed as a flag to allow library consumer to 
							// decide on correct coarse of action
							if (exx.Message == "data not integrity protected.")
								Context.FailedIntegrityCheck = true;
							else
								throw;
						}
					}
					catch (PgpException ex)
					{
						if (!(ex.InnerException is EndOfStreamException))
							throw ex;
					}
				}

				if (!secretKeyMatched)
				{
					logger.Debug("DecryptHandlePgpObject: Decryption key not found");
					throw new SecretKeyNotFoundException("Error, unable to locate decryption key.");
				}
			}
			else if (obj is PgpCompressedData)
			{
				Context.IsCompressed = true;
				var compressedData = obj as PgpCompressedData;
				using (var compressedIn = compressedData.GetDataStream())
				{
					var factory = new PgpObjectFactory(compressedIn);

					do
					{
						var nextObj = factory.NextPgpObject();
						if (nextObj == null)
							break;

						var r = DecryptHandlePgpObject(nextObj);
						if (r != null)
							ret = r;
					}
					while (true);
				}
			}
			else if (obj is PgpOnePassSignatureList)
			{
				logger.Trace("DecryptHandlePgpObject: IsSigned");

				Context.IsSigned = true;
				var signatureList = obj as PgpOnePassSignatureList;

				if (signatureList.Count > 1)
				{
					logger.Error("DecryptHandlePgpObject: Error, more than one signature present!");
					throw new CryptoException("Error, more than one signature present!");
				}

				Context.OnePassSignature = signatureList[0];
				var publicKey = GetPublicKey(Context.OnePassSignature.KeyId);
				if (publicKey == null)
				{
					logger.Debug("DecryptHandlePgpObject: Failed to find public key: " + Context.OnePassSignature.KeyId);
					Context.OnePassSignature = null;
				}
				else
					Context.OnePassSignature.InitVerify(publicKey);
			}
			else if (obj is PgpSignatureList)
			{
				var signatureList = obj as PgpSignatureList;

				if (signatureList.Count > 1)
				{
					logger.Error("DecryptHandlePgpObject: Error, more than one signature present!");
					throw new CryptoException("Error, more than one signature present!");
				}

				Context.Signature = signatureList[0];
				Context.IsSigned = true;

				if (Context.IsSigned && Context.OnePassSignature == null)
				{
					logger.Warn("DecryptHandlePgpObject: We don't have signature key for validation");

					// We don't have signature key for validation
					Context.SignatureValidated = false;
					Context.SignedBy = null;
				}
				else if (Context.OnePassSignature == null)
				{
				}
				else
				{
					if (Context.OnePassSignature.Verify(Context.Signature))
					{
						logger.Trace("DecryptHandlePgpObject: Context.OnePassSignature.Verify passed");
						Context.SignatureValidated = true;
						Context.SignedBy = GetMasterPublicKey(Context.Signature.KeyId);
					}
					else
					{
						logger.Trace("DecryptHandlePgpObject: Context.OnePassSignature.Verify failed");
					}
				}
			}
			else if (obj is PgpLiteralData)
			{
				var literalData = obj as PgpLiteralData;

				using (var dataOut = new MemoryStream())
				{
					using (var dataIn = literalData.GetInputStream())
						dataIn.CopyTo(dataOut);

					dataOut.Position = 0;

					ret = dataOut.ToArray();
				}

				if (Context.OnePassSignature != null)
					Context.OnePassSignature.Update(ret, 0, ret.Length);
				else if (Context.Signature != null)
				{
					var publicKey = GetPublicKey(Context.Signature.KeyId);
					if (publicKey == null)
					{
						logger.Debug("DecryptHandlePgpObject: Failed to find public key: " + Context.OnePassSignature.KeyId);
						Context.Signature = null;
					}
					else
					{
						Context.Signature.InitVerify(publicKey);
						Context.Signature.Update(ret, 0, ret.Length);

						if (Context.Signature.Verify())
						{
							logger.Trace("DecryptHandlePgpObject: Context.Signature.Verify passed");
							Context.SignatureValidated = true;
							Context.SignedBy = GetMasterPublicKey(Context.Signature.KeyId);
						}
						else
						{
							logger.Trace("DecryptHandlePgpObject: Context.Signature.Verify failed");
						}
					}
				}
			}
			else if (obj is PgpMarker)
			{
				// Skip, These packets are used by PGP 5.x to signal to earlier 
				// versions of PGP (eg. 2.6.x) that the message requires newer 
				// software to be read and understood.
			}
			else
			{
				logger.Debug("DecryptHandlePgpObject: Unknown pgp object: " + obj.GetType().ToString());
				throw new CryptoException("Unknown Pgp Object: " + obj.GetType().ToString());
			}

			logger.Trace("DecryptHandlePgpObject: Returning " +
				(ret == null ? "null" : ret.Length.ToString()) + " bytes");

			return ret;
		}
	}
}

// end
