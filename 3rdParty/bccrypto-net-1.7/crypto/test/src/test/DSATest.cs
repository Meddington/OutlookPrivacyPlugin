using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	[TestFixture]
	public class DsaTest
		: SimpleTest
	{
		private static readonly byte[] k1 = Hex.Decode("d5014e4b60ef2ba8b6211b4062ba3224e0427dd3");
		private static readonly byte[] k2 = Hex.Decode("345e8d05c075c3a508df729a1685690e68fcfb8c8117847e89063bca1f85d968fd281540b6e13bd1af989a1fbf17e06462bf511f9d0b140fb48ac1b1baa5bded");

		private static readonly SecureRandom random = FixedSecureRandom.From(k1, k2);

		// TODO How shall we satisfy this compatibility test?
//		[Test]
//		public void TestCompat()
//		{
//			if (Security.getProvider("SUN") == null)
//				return;
//			ISigner s = SignerUtilities.GetSigner("DSA", "SUN");
//			KeyPairGenerator g = KeyPairGenerator.GetInstance("DSA", "SUN");
//			byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
//
//			g.initialize(512, new SecureRandom());
//
//			KeyPair p = g.generateKeyPair();
//
//			PrivateKey sKey = p.Private;
//			PublicKey vKey = p.Public;
//
//			//
//			// sign SUN - verify with BC
//			//
//			s.Init(true, sKey);
//
//			s.update(data);
//
//			byte[] sigBytes = s.GenerateSignature();
//
//			s = SignerUtilities.GetSigner("DSA");
//
//			s.Init(false, vKey);
//
//			s.update(data);
//
//			if (!s.VerifySignature(sigBytes))
//			{
//				Fail("SUN -> BC verification failed");
//			}
//
//			//
//			// sign BC - verify with SUN
//			//
//
//			s.Init(true, sKey);
//
//			s.update(data);
//
//			sigBytes = s.GenerateSignature();
//
//			s = SignerUtilities.GetSigner("DSA", "SUN");
//
//			s.Init(false, vKey);
//
//			s.update(data);
//
//			if (!s.VerifySignature(sigBytes))
//			{
//				Fail("BC -> SUN verification failed");
//			}
//
//			//
//			// key encoding test - BC decoding Sun keys
//			//
//			KeyFactory f = KeyFactory.GetInstance("DSA");
//			X509EncodedKeySpec x509s = new X509EncodedKeySpec(vKey.GetEncoded());
//			DSAPublicKey k1 = (DSAPublicKey)f.generatePublic(x509s);
//
//			if (!k1.Y.Equals(((DSAPublicKey)vKey).Y))
//			{
//				Fail("public number not decoded properly");
//			}
//
//			if (!k1.Parameters.G.Equals(((DSAPublicKey)vKey).Parameters.G))
//			{
//				Fail("public generator not decoded properly");
//			}
//
//			if (!k1.Parameters.P.Equals(((DSAPublicKey)vKey).Parameters.P))
//			{
//				Fail("public p value not decoded properly");
//			}
//
//			if (!k1.Parameters.Q.Equals(((DSAPublicKey)vKey).Parameters.Q))
//			{
//				Fail("public q value not decoded properly");
//			}
//
//			PKCS8EncodedKeySpec pkcs8 = new PKCS8EncodedKeySpec(sKey.GetEncoded());
//			DSAPrivateKey k2 = (DSAPrivateKey)f.generatePrivate(pkcs8);
//
//			if (!k2.X.Equals(((DSAPrivateKey)sKey).X))
//			{
//				Fail("private number not decoded properly");
//			}
//
//			if (!k2.Parameters.G.Equals(((DSAPrivateKey)sKey).Parameters.G))
//			{
//				Fail("private generator not decoded properly");
//			}
//
//			if (!k2.Parameters.P.Equals(((DSAPrivateKey)sKey).Parameters.P))
//			{
//				Fail("private p value not decoded properly");
//			}
//
//			if (!k2.Parameters.Q.Equals(((DSAPrivateKey)sKey).Parameters.Q))
//			{
//				Fail("private q value not decoded properly");
//			}
//
//			//
//			// key decoding test - SUN decoding BC keys
//			//
//			f = KeyFactory.GetInstance("DSA", "SUN");
//			x509s = new X509EncodedKeySpec(k1.GetEncoded());
//
//			vKey = (DSAPublicKey)f.generatePublic(x509s);
//
//			if (!k1.Y.Equals(((DSAPublicKey)vKey).Y))
//			{
//				Fail("public number not decoded properly");
//			}
//
//			if (!k1.Parameters.G.Equals(((DSAPublicKey)vKey).Parameters.G))
//			{
//				Fail("public generator not decoded properly");
//			}
//
//			if (!k1.Parameters.P.Equals(((DSAPublicKey)vKey).Parameters.P))
//			{
//				Fail("public p value not decoded properly");
//			}
//
//			if (!k1.Parameters.Q.Equals(((DSAPublicKey)vKey).Parameters.Q))
//			{
//				Fail("public q value not decoded properly");
//			}
//
//			pkcs8 = new PKCS8EncodedKeySpec(k2.GetEncoded());
//			sKey = (DSAPrivateKey)f.generatePrivate(pkcs8);
//
//			if (!k2.X.Equals(((DSAPrivateKey)sKey).X))
//			{
//				Fail("private number not decoded properly");
//			}
//
//			if (!k2.Parameters.G.Equals(((DSAPrivateKey)sKey).Parameters.G))
//			{
//				Fail("private generator not decoded properly");
//			}
//
//			if (!k2.Parameters.P.Equals(((DSAPrivateKey)sKey).Parameters.P))
//			{
//				Fail("private p value not decoded properly");
//			}
//
//			if (!k2.Parameters.Q.Equals(((DSAPrivateKey)sKey).Parameters.Q))
//			{
//				Fail("private q value not decoded properly");
//			}
//		}

		[Test]
		public void TestNONEwithDSA()
		{
			byte[] dummySha1 = Hex.Decode("01020304050607080910111213141516");

			SecureRandom rand = new SecureRandom();

			DsaParametersGenerator pGen = new DsaParametersGenerator();
			pGen.Init(512, 80, rand);

			IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator("DSA");
			g.Init(new DsaKeyGenerationParameters(rand, pGen.GenerateParameters()));

			AsymmetricCipherKeyPair kp = g.GenerateKeyPair();

			ISigner sig = SignerUtilities.GetSigner("NONEwithDSA");
			sig.Init(true, kp.Private);
			sig.BlockUpdate(dummySha1, 0, dummySha1.Length);
			byte[] sigBytes = sig.GenerateSignature();

			sig.Init(false, kp.Public);
			sig.BlockUpdate(dummySha1, 0, dummySha1.Length);
			sig.VerifySignature(sigBytes);

			// reset test

			sig.BlockUpdate(dummySha1, 0, dummySha1.Length);

			if (!sig.VerifySignature(sigBytes))
			{
				Fail("NONEwithDSA failed to reset");
			}

			// lightweight test
			DsaSigner signer = new DsaSigner();
			Asn1Sequence derSig = Asn1Sequence.GetInstance(Asn1Object.FromByteArray(sigBytes));

			signer.Init(false, kp.Public);

			if (!signer.VerifySignature(dummySha1,
				DerInteger.GetInstance(derSig[0]).Value, DerInteger.GetInstance(derSig[1]).Value))
			{
				Fail("NONEwithDSA not really NONE!");
			}
		}

		/**
		* X9.62 - 1998,<br/>
		* J.3.2, Page 155, ECDSA over the field Fp<br/>
		* an example with 239 bit prime
		*/
		[Test]
		public void TestECDsa239BitPrime()
		{
			BigInteger r = new BigInteger("308636143175167811492622547300668018854959378758531778147462058306432176");
			BigInteger s = new BigInteger("323813553209797357708078776831250505931891051755007842781978505179448783");

			byte[] kData = BigIntegers.AsUnsignedByteArray(
				new BigInteger("700000017569056646655505781757157107570501575775705779575555657156756655"));

			SecureRandom k = FixedSecureRandom.From(kData);

			ECCurve curve = new FpCurve(
				new BigInteger("883423532389192164791648750360308885314476597252960362792450860609699839"), // q
				new BigInteger("7fffffffffffffffffffffff7fffffffffff8000000000007ffffffffffc", 16), // a
				new BigInteger("6b016c3bdcf18941d0d654921475ca71a9db2fb27d1d37796185c2942c0a", 16)); // b

			ECDomainParameters spec = new ECDomainParameters(
				curve,
				curve.DecodePoint(Hex.Decode("020ffa963cdca8816ccc33b8642bedf905c3d358573d3f27fbbd3b3cb9aaaf")), // G
				new BigInteger("883423532389192164791648750360308884807550341691627752275345424702807307")); // n

			ECPrivateKeyParameters priKey = new ECPrivateKeyParameters(
				"ECDSA",
				new BigInteger("876300101507107567501066130761671078357010671067781776716671676178726717"), // d
				spec);

			ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
				"ECDSA",
				curve.DecodePoint(Hex.Decode("025b6dc53bc61a2548ffb0f671472de6c9521a9d2d2534e65abfcbd5fe0c70")), // Q
				spec);

			ISigner sgr = SignerUtilities.GetSigner("ECDSA");

//			KeyFactory f = KeyFactory.GetInstance("ECDSA");
//			PrivateKey sKey = f.generatePrivate(priKey);
//			PublicKey vKey = f.generatePublic(pubKey);
			AsymmetricKeyParameter sKey = priKey;
			AsymmetricKeyParameter vKey = pubKey;

			sgr.Init(true, new ParametersWithRandom(sKey, k));

			byte[] message = Encoding.ASCII.GetBytes("abc");

			sgr.BlockUpdate(message, 0, message.Length);

			byte[] sigBytes = sgr.GenerateSignature();

			sgr.Init(false, vKey);

			sgr.BlockUpdate(message, 0, message.Length);

			if (!sgr.VerifySignature(sigBytes))
			{
				Fail("239 Bit EC verification failed");
			}

			BigInteger[] sig = DerDecode(sigBytes);

			if (!r.Equals(sig[0]))
			{
				Fail("r component wrong." + SimpleTest.NewLine
						+ " expecting: " + r + SimpleTest.NewLine
						+ " got      : " + sig[0]);
			}

			if (!s.Equals(sig[1]))
			{
				Fail("s component wrong." + SimpleTest.NewLine
						+ " expecting: " + s + SimpleTest.NewLine
						+ " got      : " + sig[1]);
			}
		}

		/**
		* X9.62 - 1998,<br/>
		* J.2.1, Page 100, ECDSA over the field F2m<br/>
		* an example with 191 bit binary field
		*/
		[Test]
		public void TestECDsa239BitBinary()
		{
			BigInteger r = new BigInteger("21596333210419611985018340039034612628818151486841789642455876922391552");
			BigInteger s = new BigInteger("197030374000731686738334997654997227052849804072198819102649413465737174");

			byte[] kData = BigIntegers.AsUnsignedByteArray(
				new BigInteger("171278725565216523967285789236956265265265235675811949404040041670216363"));

			SecureRandom k = FixedSecureRandom.From(kData);

			ECCurve curve = new F2mCurve(
				239, // m
				36, // k
				new BigInteger("32010857077C5431123A46B808906756F543423E8D27877578125778AC76", 16), // a
				new BigInteger("790408F2EEDAF392B012EDEFB3392F30F4327C0CA3F31FC383C422AA8C16", 16)); // b

			ECDomainParameters parameters = new ECDomainParameters(
				curve,
				curve.DecodePoint(Hex.Decode("0457927098FA932E7C0A96D3FD5B706EF7E5F5C156E16B7E7C86038552E91D61D8EE5077C33FECF6F1A16B268DE469C3C7744EA9A971649FC7A9616305")), // G
				new BigInteger("220855883097298041197912187592864814557886993776713230936715041207411783"), // n
				BigInteger.ValueOf(4)); // h

			ECPrivateKeyParameters sKey = new ECPrivateKeyParameters(
				"ECDSA",
				new BigInteger("145642755521911534651321230007534120304391871461646461466464667494947990"), // d
				parameters);

			ECPublicKeyParameters vKey = new ECPublicKeyParameters(
				"ECDSA",
				curve.DecodePoint(Hex.Decode("045894609CCECF9A92533F630DE713A958E96C97CCB8F5ABB5A688A238DEED6DC2D9D0C94EBFB7D526BA6A61764175B99CB6011E2047F9F067293F57F5")), // Q
				parameters);

			ISigner sgr = SignerUtilities.GetSigner("ECDSA");
			byte[] message = Encoding.ASCII.GetBytes("abc");

			sgr.Init(true, new ParametersWithRandom(sKey, k));

			sgr.BlockUpdate(message, 0, message.Length);

			byte[] sigBytes = sgr.GenerateSignature();

			sgr.Init(false, vKey);

			sgr.BlockUpdate(message, 0, message.Length);

			if (!sgr.VerifySignature(sigBytes))
			{
				Fail("239 Bit EC verification failed");
			}

			BigInteger[] sig = DerDecode(sigBytes);

			if (!r.Equals(sig[0]))
			{
				Fail("r component wrong." + SimpleTest.NewLine
					+ " expecting: " + r + SimpleTest.NewLine
					+ " got      : " + sig[0]);
			}

			if (!s.Equals(sig[1]))
			{
				Fail("s component wrong." + SimpleTest.NewLine
					+ " expecting: " + s + SimpleTest.NewLine
					+ " got      : " + sig[1]);
			}
		}

		[Test]
		public void TestECDsa239BitBinaryRipeMD160()
		{
			DoTestECDsa239BitBinary("RIPEMD160withECDSA", TeleTrusTObjectIdentifiers.ECSignWithRipeMD160);
		}

		[Test]
		public void TestECDsa239BitBinarySha1()
		{
			DoTestECDsa239BitBinary("SHA1withECDSA", TeleTrusTObjectIdentifiers.ECSignWithSha1);
		}

		[Test]
		public void TestECDsa239BitBinarySha224()
		{
			DoTestECDsa239BitBinary("SHA224withECDSA", X9ObjectIdentifiers.ECDsaWithSha224);
		}

		[Test]
		public void TestECDsa239BitBinarySha256()
		{
			DoTestECDsa239BitBinary("SHA256withECDSA", X9ObjectIdentifiers.ECDsaWithSha256);
		}

		[Test]
		public void TestECDsa239BitBinarySha384()
		{
			DoTestECDsa239BitBinary("SHA384withECDSA", X9ObjectIdentifiers.ECDsaWithSha384);
		}

		[Test]
		public void TestECDsa239BitBinarySha512()
		{
			DoTestECDsa239BitBinary("SHA512withECDSA", X9ObjectIdentifiers.ECDsaWithSha512);
		}

		private void DoTestECDsa239BitBinary(
			string				algorithm,
			DerObjectIdentifier	oid)
		{
			BigInteger r = new BigInteger("21596333210419611985018340039034612628818151486841789642455876922391552");
			BigInteger s = new BigInteger("197030374000731686738334997654997227052849804072198819102649413465737174");

			byte[] kData = BigIntegers.AsUnsignedByteArray(
				new BigInteger("171278725565216523967285789236956265265265235675811949404040041670216363"));

			SecureRandom k = FixedSecureRandom.From(kData);

			ECCurve curve = new F2mCurve(
				239, // m
				36, // k
				new BigInteger("32010857077C5431123A46B808906756F543423E8D27877578125778AC76", 16), // a
				new BigInteger("790408F2EEDAF392B012EDEFB3392F30F4327C0CA3F31FC383C422AA8C16", 16)); // b

			ECDomainParameters parameters = new ECDomainParameters(
				curve,
				curve.DecodePoint(Hex.Decode("0457927098FA932E7C0A96D3FD5B706EF7E5F5C156E16B7E7C86038552E91D61D8EE5077C33FECF6F1A16B268DE469C3C7744EA9A971649FC7A9616305")), // G
				new BigInteger("220855883097298041197912187592864814557886993776713230936715041207411783"), // n
				BigInteger.ValueOf(4)); // h

			ECPrivateKeyParameters sKey = new ECPrivateKeyParameters(
				new BigInteger("145642755521911534651321230007534120304391871461646461466464667494947990"), // d
				parameters);

			ECPublicKeyParameters vKey = new ECPublicKeyParameters(
				curve.DecodePoint(Hex.Decode("045894609CCECF9A92533F630DE713A958E96C97CCB8F5ABB5A688A238DEED6DC2D9D0C94EBFB7D526BA6A61764175B99CB6011E2047F9F067293F57F5")), // Q
				parameters);

			ISigner sgr = SignerUtilities.GetSigner(algorithm);
			byte[] message = Encoding.ASCII.GetBytes("abc");

			sgr.Init(true, new ParametersWithRandom(sKey, k));

			sgr.BlockUpdate(message, 0, message.Length);

			byte[] sigBytes = sgr.GenerateSignature();

			sgr = SignerUtilities.GetSigner(oid.Id);

			sgr.Init(false, vKey);

			sgr.BlockUpdate(message, 0, message.Length);

			if (!sgr.VerifySignature(sigBytes))
			{
				Fail("239 Bit EC RIPEMD160 verification failed");
			}
		}

		private void doTestBadStrength(
			int strength)
		{
//			IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator("DSA");

			// test exception
			//
			try
			{
				SecureRandom rand = new SecureRandom();

				DsaParametersGenerator pGen = new DsaParametersGenerator();
				pGen.Init(strength, 80, rand);

//				g.Init(new DsaKeyGenerationParameters(rand, pGen.GenerateParameters()));

				Fail("illegal parameter " + strength + " check failed.");
			}
			catch (ArgumentException)
			{
				// expected
			}
		}

		[Test]
		public void TestGeneration()
		{
			ISigner s = SignerUtilities.GetSigner("DSA");
			byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
			SecureRandom rand = new SecureRandom();

			// KeyPairGenerator g = KeyPairGenerator.GetInstance("DSA");
			IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator("DSA");

			// test exception
			//

			doTestBadStrength(513);
			doTestBadStrength(510);
			doTestBadStrength(1025);

			//g.initialize(512, rand);
			{
				DsaParametersGenerator pGen = new DsaParametersGenerator();
				pGen.Init(512, 80, rand);

				g.Init(new DsaKeyGenerationParameters(rand, pGen.GenerateParameters()));
			}

			AsymmetricCipherKeyPair p = g.GenerateKeyPair();

			AsymmetricKeyParameter sKey = p.Private;
			AsymmetricKeyParameter vKey = p.Public;

			s.Init(true, sKey);

			s.BlockUpdate(data, 0, data.Length);

			byte[] sigBytes = s.GenerateSignature();

			s = SignerUtilities.GetSigner("DSA");

			s.Init(false, vKey);

			s.BlockUpdate(data, 0, data.Length);

			if (!s.VerifySignature(sigBytes))
			{
				Fail("DSA verification failed");
			}



			//
			// ECDSA Fp generation test
			//
			s = SignerUtilities.GetSigner("ECDSA");

			ECCurve curve = new FpCurve(
				new BigInteger("883423532389192164791648750360308885314476597252960362792450860609699839"), // q
				new BigInteger("7fffffffffffffffffffffff7fffffffffff8000000000007ffffffffffc", 16), // a
				new BigInteger("6b016c3bdcf18941d0d654921475ca71a9db2fb27d1d37796185c2942c0a", 16)); // b

			ECDomainParameters ecSpec = new ECDomainParameters(
				curve,
				curve.DecodePoint(Hex.Decode("020ffa963cdca8816ccc33b8642bedf905c3d358573d3f27fbbd3b3cb9aaaf")), // G
				new BigInteger("883423532389192164791648750360308884807550341691627752275345424702807307")); // n

			g = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
			g.Init(new ECKeyGenerationParameters(ecSpec, rand));

			p = g.GenerateKeyPair();

			sKey = p.Private;
			vKey = p.Public;

			s.Init(true, sKey);

			s.BlockUpdate(data, 0, data.Length);

			sigBytes = s.GenerateSignature();

			s = SignerUtilities.GetSigner("ECDSA");

			s.Init(false, vKey);

			s.BlockUpdate(data, 0, data.Length);

			if (!s.VerifySignature(sigBytes))
			{
				Fail("ECDSA verification failed");
			}

			//
			// ECDSA F2m generation test
			//
			s = SignerUtilities.GetSigner("ECDSA");

			curve = new F2mCurve(
					239, // m
					36, // k
					new BigInteger("32010857077C5431123A46B808906756F543423E8D27877578125778AC76", 16), // a
					new BigInteger("790408F2EEDAF392B012EDEFB3392F30F4327C0CA3F31FC383C422AA8C16", 16)); // b

			ecSpec = new ECDomainParameters(
				curve,
				curve.DecodePoint(Hex.Decode("0457927098FA932E7C0A96D3FD5B706EF7E5F5C156E16B7E7C86038552E91D61D8EE5077C33FECF6F1A16B268DE469C3C7744EA9A971649FC7A9616305")), // G
				new BigInteger("220855883097298041197912187592864814557886993776713230936715041207411783"), // n
				BigInteger.ValueOf(4)); // h

			g = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
			g.Init(new ECKeyGenerationParameters(ecSpec, rand));

			p = g.GenerateKeyPair();

			sKey = p.Private;
			vKey = p.Public;

			s.Init(true, sKey);

			s.BlockUpdate(data, 0, data.Length);

			sigBytes = s.GenerateSignature();

			s = SignerUtilities.GetSigner("ECDSA");

			s.Init(false, vKey);

			s.BlockUpdate(data, 0, data.Length);

			if (!s.VerifySignature(sigBytes))
			{
				Fail("ECDSA verification failed");
			}
		}

		[Test]
		public void TestParameters()
		{
//			AlgorithmParameterGenerator a = AlgorithmParameterGenerator.GetInstance("DSA");
//			a.init(512, random);
			DsaParametersGenerator a = new DsaParametersGenerator();
			a.Init(512, 20, random);

//			AlgorithmParameters parameters = a.generateParameters();
			DsaParameters p = a.GenerateParameters();

//			byte[] encodeParams = parameters.GetEncoded();
			byte[] encodeParams = new DsaParameter(p.P, p.Q, p.G).GetDerEncoded();

//			AlgorithmParameters a2 = AlgorithmParameters.GetInstance("DSA");
//			a2.init(encodeParams);
			DsaParameter dsaP = DsaParameter.GetInstance(Asn1Object.FromByteArray(encodeParams));
			DsaParameters p2 = new DsaParameters(dsaP.P, dsaP.Q, dsaP.G);

			// a and a2 should be equivalent!
//			byte[] encodeParams_2 = a2.GetEncoded();
			byte[] encodeParams_2 = new DsaParameter(p2.P, p2.Q, p2.G).GetDerEncoded();

			if (!AreEqual(encodeParams, encodeParams_2))
			{
				Fail("encode/Decode parameters failed");
			}

//			DSAParameterSpec dsaP = (DSAParameterSpec)parameters.getParameterSpec(typeof(DSAParameterSpec));

//			KeyPairGenerator g = KeyPairGenerator.GetInstance("DSA");
			IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator("DSA");
//			g.initialize(dsaP, new SecureRandom());
			g.Init(new DsaKeyGenerationParameters(new SecureRandom(), p));
//			KeyPair p = g.generateKeyPair();
			AsymmetricCipherKeyPair pair = g.GenerateKeyPair();

//			PrivateKey sKey = p.Private;
//			PublicKey vKey = p.Public;
			AsymmetricKeyParameter sKey = pair.Private;
			AsymmetricKeyParameter vKey = pair.Public;

			ISigner s = SignerUtilities.GetSigner("DSA");
			byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

			s.Init(true, sKey);

			s.BlockUpdate(data, 0, data.Length);

			byte[] sigBytes = s.GenerateSignature();

			s = SignerUtilities.GetSigner("DSA");

			s.Init(false, vKey);

			s.BlockUpdate(data, 0, data.Length);

			if (!s.VerifySignature(sigBytes))
			{
				Fail("DSA verification failed");
			}
		}

		public override void PerformTest()
		{
			// TODO
//			TestCompat();
			TestNONEwithDSA();
			TestECDsa239BitPrime();
			TestECDsa239BitBinary();
			TestECDsa239BitBinaryRipeMD160();
			TestECDsa239BitBinarySha1();
			TestECDsa239BitBinarySha224();
			TestECDsa239BitBinarySha256();
			TestECDsa239BitBinarySha384();
			TestECDsa239BitBinarySha512();
			TestGeneration();
			TestParameters();
		}

		protected BigInteger[] DerDecode(
			byte[] encoding)
		{
			Asn1Sequence s = (Asn1Sequence) Asn1Object.FromByteArray(encoding);

			return new BigInteger[]
			{
				((DerInteger)s[0]).Value,
				((DerInteger)s[1]).Value
			};
		}

		public override string Name
		{
			get { return "DSA/ECDSA"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new DsaTest());
		}
	}
}
