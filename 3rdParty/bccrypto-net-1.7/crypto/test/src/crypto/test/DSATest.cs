using System;

using NUnit.Framework;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
    /// <summary>Test based on FIPS 186-2, Appendix 5, an example of DSA.</summary>
    [TestFixture]
    public class DsaTest
		: SimpleTest
    {
		public override string Name
        {
			get { return "DSA"; }
        }

		internal BigInteger pValue = new BigInteger("8df2a494492276aa3d25759bb06869cbeac0d83afb8d0cf7cbb8324f0d7882e5d0762fc5b7210eafc2e9adac32ab7aac49693dfbf83724c2ec0736ee31c80291", 16);
        internal BigInteger qValue = new BigInteger("c773218c737ec8ee993b4f2ded30f48edace915f", 16);

		public override void PerformTest()
        {
			byte[] k1 = Hex.Decode("d5014e4b60ef2ba8b6211b4062ba3224e0427dd3");
			byte[] k2 = Hex.Decode("345e8d05c075c3a508df729a1685690e68fcfb8c8117847e89063bca1f85d968fd281540b6e13bd1af989a1fbf17e06462bf511f9d0b140fb48ac1b1baa5bded");

			SecureRandom random = FixedSecureRandom.From(k1, k2);

			byte[] keyData = Hex.Decode("b5014e4b60ef2ba8b6211b4062ba3224e0427dd3");

			SecureRandom keyRandom = FixedSecureRandom.From(keyData, keyData);

			BigInteger r = new BigInteger("68076202252361894315274692543577577550894681403");
            BigInteger s = new BigInteger("1089214853334067536215539335472893651470583479365");
            DsaParametersGenerator pGen = new DsaParametersGenerator();

            pGen.Init(512, 80, random);

            DsaParameters parameters = pGen.GenerateParameters();
            DsaValidationParameters pValid = parameters.ValidationParameters;

            if (pValid.Counter != 105)
            {
                Fail("Counter wrong");
            }

            if (!pValue.Equals(parameters.P) || !qValue.Equals(parameters.Q))
            {
                Fail("p or q wrong");
            }

            DsaKeyPairGenerator dsaKeyGen = new DsaKeyPairGenerator();
            DsaKeyGenerationParameters genParam = new DsaKeyGenerationParameters(keyRandom, parameters);

            dsaKeyGen.Init(genParam);

            AsymmetricCipherKeyPair pair = dsaKeyGen.GenerateKeyPair();

            ParametersWithRandom param = new ParametersWithRandom(pair.Private, keyRandom);

            DsaSigner dsa = new DsaSigner();

            dsa.Init(true, param);

			byte[] message = new BigInteger("968236873715988614170569073515315707566766479517").ToByteArrayUnsigned();
			BigInteger[] sig = dsa.GenerateSignature(message);

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

			dsa.Init(false, pair.Public);

            if (!dsa.VerifySignature(message, sig[0], sig[1]))
            {
                Fail("verification fails");
            }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new DsaTest());
        }

		[Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
