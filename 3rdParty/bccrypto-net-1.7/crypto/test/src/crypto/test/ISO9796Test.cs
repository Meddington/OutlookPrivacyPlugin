using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;

using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/// <summary> test vectors from ISO 9796-1 and ISO 9796-2 edition 1.</summary>
	[TestFixture]
	public class ISO9796Test
		: SimpleTest
	{
		static BigInteger mod1 = new BigInteger("0100000000000000000000000000000000bba2d15dbb303c8a21c5ebbcbae52b7125087920dd7cdf358ea119fd66fb064012ec8ce692f0a0b8e8321b041acd40b7", 16);

		static BigInteger pub1 = new BigInteger("03", 16);

		static BigInteger pri1 = new BigInteger("2aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaac9f0783a49dd5f6c5af651f4c9d0dc9281c96a3f16a85f9572d7cc3f2d0f25a9dbf1149e4cdc32273faadd3fda5dcda7", 16);

		static BigInteger mod2 = new BigInteger("ffffff7fa27087c35ebead78412d2bdffe0301edd494df13458974ea89b364708f7d0f5a00a50779ddf9f7d4cb80b8891324da251a860c4ec9ef288104b3858d", 16);

		static BigInteger pub2 = new BigInteger("03", 16);

		static BigInteger pri2 = new BigInteger("2aaaaa9545bd6bf5e51fc7940adcdca5550080524e18cfd88b96e8d1c19de6121b13fac0eb0495d47928e047724d91d1740f6968457ce53ec8e24c9362ce84b5", 16);

		static byte[] msg1 = Hex.Decode("0cbbaa99887766554433221100");

		//
		// you'll need to see the ISO 9796 to make sense of this
		//
		static byte[] sig1 = mod1.Subtract(new BigInteger("309f873d8ded8379490f6097eaafdabc137d3ebfd8f25ab5f138d56a719cdc526bdd022ea65dabab920a81013a85d092e04d3e421caab717c90d89ea45a8d23a", 16)).ToByteArray();

		static byte[] msg2 = Hex.Decode("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210");

		static byte[] sig2 = new BigInteger("319bb9becb49f3ed1bca26d0fcf09b0b0a508e4d0bd43b350f959b72cd25b3af47d608fdcd248eada74fbe19990dbeb9bf0da4b4e1200243a14e5cab3f7e610c", 16).ToByteArray();

		static byte[] msg3 = Hex.Decode("0112233445566778899aabbccd");

		static byte[] sig3 = mod2.Subtract(new BigInteger("58e59ffb4b1fb1bcdbf8d1fe9afa3730c78a318a1134f5791b7313d480ff07ac319b068edf8f212945cb09cf33df30ace54f4a063fcca0b732f4b662dc4e2454", 16)).ToByteArray();

		//
		// ISO 9796-2
		//
		static BigInteger mod3 = new BigInteger("ffffffff78f6c55506c59785e871211ee120b0b5dd644aa796d82413a47b24573f1be5745b5cd9950f6b389b52350d4e01e90009669a8720bf265a2865994190a661dea3c7828e2e7ca1b19651adc2d5", 16);

		static BigInteger pub3 = new BigInteger("03", 16);

		static BigInteger pri3 = new BigInteger("2aaaaaaa942920e38120ee965168302fd0301d73a4e60c7143ceb0adf0bf30b9352f50e8b9e4ceedd65343b2179005b2f099915e4b0c37e41314bb0821ad8330d23cba7f589e0f129b04c46b67dfce9d", 16);

		static BigInteger mod4 = new BigInteger("FFFFFFFF45f1903ebb83d4d363f70dc647b839f2a84e119b8830b2dec424a1ce0c9fd667966b81407e89278283f27ca8857d40979407fc6da4cc8a20ecb4b8913b5813332409bc1f391a94c9c328dfe46695daf922259174544e2bfbe45cc5cd", 16);
		static BigInteger pub4 = new BigInteger("02", 16);
		static BigInteger pri4 = new BigInteger("1fffffffe8be3207d7707a9a6c7ee1b8c8f7073e5509c2337106165bd8849439c193faccf2cd70280fd124f0507e4f94cb66447680c6b87b6599d1b61c8f3600854a618262e9c1cb1438e485e47437be036d94b906087a61ee74ab0d9a1accd8", 16);

		static byte[] msg4 = Hex.Decode("6162636462636465636465666465666765666768666768696768696a68696a6b696a6b6c6a6b6c6d6b6c6d6e6c6d6e6f6d6e6f706e6f7071");
		static byte[] sig4 = Hex.Decode("374695b7ee8b273925b4656cc2e008d41463996534aa5aa5afe72a52ffd84e118085f8558f36631471d043ad342de268b94b080bee18a068c10965f581e7f32899ad378835477064abed8ef3bd530fce");

		static byte[] msg5 = Hex.Decode("fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210");
		static byte[] sig5 = Hex.Decode("5cf9a01854dbacaec83aae8efc563d74538192e95466babacd361d7c86000fe42dcb4581e48e4feb862d04698da9203b1803b262105104d510b365ee9c660857ba1c001aa57abfd1c8de92e47c275cae");

		//
		// scheme 2 data
		//
		static BigInteger mod6 = new BigInteger("b259d2d6e627a768c94be36164c2d9fc79d97aab9253140e5bf17751197731d6f7540d2509e7b9ffee0a70a6e26d56e92d2edd7f85aba85600b69089f35f6bdbf3c298e05842535d9f064e6b0391cb7d306e0a2d20c4dfb4e7b49a9640bdea26c10ad69c3f05007ce2513cee44cfe01998e62b6c3637d3fc0391079b26ee36d5", 16);
		static BigInteger pub6 = new BigInteger("11", 16);
		static BigInteger pri6 = new BigInteger("92e08f83cc9920746989ca5034dcb384a094fb9c5a6288fcc4304424ab8f56388f72652d8fafc65a4b9020896f2cde297080f2a540e7b7ce5af0b3446e1258d1dd7f245cf54124b4c6e17da21b90a0ebd22605e6f45c9f136d7a13eaac1c0f7487de8bd6d924972408ebb58af71e76fd7b012a8d0e165f3ae2e5077a8648e619", 16);

		//		static byte[] sig6 = new BigInteger("0073FEAF13EB12914A43FE635022BB4AB8188A8F3ABD8D8A9E4AD6C355EE920359C7F237AE36B1212FE947F676C68FE362247D27D1F298CA9302EB21F4A64C26CE44471EF8C0DFE1A54606F0BA8E63E87CDACA993BFA62973B567473B4D38FAE73AB228600934A9CC1D3263E632E21FD52D2B95C5F7023DA63DE9509C01F6C7BBC", 16).ModPow(pri6, mod6).ToByteArray();
		static byte[] sig6 = new BigInteger("0073FEAF13EB12914A43FE635022BB4AB8188A8F3ABD8D8A9E4AD6C355EE920359C7F237AE36B1212FE947F676C68FE362247D27D1F298CA9302EB21F4A64C26CE44471EF8C0DFE1A54606F0BA8E63E87CDACA993BFA62973B567473B4D38FAE73AB228600934A9CC1D3263E632E21FD52D2B95C5F7023DA63DE9509C01F6C7BBC", 16).ModPow(pri6, mod6).ToByteArray();

		static byte[] msg7 = Hex.Decode("6162636462636465636465666465666765666768666768696768696A68696A6B696A6B6C6A6B6C6D6B6C6D6E6C6D6E6F6D6E6F706E6F70716F70717270717273");
		static byte[] sig7 = new BigInteger("296B06224010E1EC230D4560A5F88F03550AAFCE31C805CE81E811E5E53E5F71AE64FC2A2A486B193E87972D90C54B807A862F21A21919A43ECF067240A8C8C641DE8DCDF1942CF790D136728FFC0D98FB906E7939C1EC0E64C0E067F0A7443D6170E411DF91F797D1FFD74009C4638462E69D5923E7433AEC028B9A90E633CC", 16).ModPow(pri6, mod6).ToByteArray();

		static byte[] msg8 = Hex.Decode("FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA9876543210FEDCBA98");
		static byte[] sig8 = new BigInteger("01402B29ABA104079677CE7FC3D5A84DB24494D6F9508B4596484F5B3CC7E8AFCC4DDE7081F21CAE9D4F94D6D2CCCB43FCEDA0988FFD4EF2EAE72CFDEB4A2638F0A34A0C49664CD9DB723315759D758836C8BA26AC4348B66958AC94AE0B5A75195B57ABFB9971E21337A4B517F2E820B81F26BCE7C66F48A2DB12A8F3D731CC", 16).ModPow(pri6, mod6).ToByteArray();

		static byte[] msg9 = Hex.Decode("6162636462636465636465666465666765666768666768696768696A68696A6B696A6B6C6A6B6C6D6B6C6D6E6C6D6E6F6D6E6F706E6F70716F707172707172737172737472737475737475767475767775767778767778797778797A78797A61797A61627A6162636162636462636465");
		static byte[] sig9 = new BigInteger("6F2BB97571FE2EF205B66000E9DD06656655C1977F374E8666D636556A5FEEEEAF645555B25F45567C4EE5341F96FED86508C90A9E3F11B26E8D496139ED3E55ECE42860A6FB3A0817DAFBF13019D93E1D382DA07264FE99D9797D2F0B7779357CA7E74EE440D8855B7DDF15F000AC58EE3FFF144845E771907C0C83324A6FBC", 16).ModPow(pri6, mod6).ToByteArray();

		public override string Name
		{
			get { return "ISO9796"; }
		}

		private bool IsSameAs(
			byte[]	a,
			int		off,
			byte[]	b)
		{
			if ((a.Length - off) != b.Length)
			{
				return false;
			}

			for (int i = 0; i != b.Length; i++)
			{
				if (a[i + off] != b[i])
				{
					return false;
				}
			}

			return true;
		}

		private bool StartsWith(
			byte[]	a,
			byte[]	b)
		{
			if (a.Length < b.Length)
				return false;

			for (int i = 0; i != b.Length; i++)
			{
				if (a[i] != b[i])
					return false;
			}

			return true;
		}

		[Test]
		public virtual void DoTest1()
		{
			RsaKeyParameters pubParameters = new RsaKeyParameters(false, mod1, pub1);
			RsaKeyParameters privParameters = new RsaKeyParameters(true, mod1, pri1);
			RsaEngine rsa = new RsaEngine();
			byte[] data;

			//
			// ISO 9796-1 - public encrypt, private decrypt
			//
			ISO9796d1Encoding eng = new ISO9796d1Encoding(rsa);

			eng.Init(true, privParameters);

			eng.SetPadBits(4);

			data = eng.ProcessBlock(msg1, 0, msg1.Length);

			eng.Init(false, pubParameters);

			if (!AreEqual(sig1, data))
			{
				Fail("failed ISO9796-1 generation Test 1");
			}

			data = eng.ProcessBlock(data, 0, data.Length);

			if (!AreEqual(msg1, data))
			{
				Fail("failed ISO9796-1 retrieve Test 1");
			}
		}

		[Test]
		public virtual void DoTest2()
		{
			RsaKeyParameters pubParameters = new RsaKeyParameters(false, mod1, pub1);
			RsaKeyParameters privParameters = new RsaKeyParameters(true, mod1, pri1);
			RsaEngine rsa = new RsaEngine();
			byte[] data;

			//
			// ISO 9796-1 - public encrypt, private decrypt
			//
			ISO9796d1Encoding eng = new ISO9796d1Encoding(rsa);

			eng.Init(true, privParameters);

			data = eng.ProcessBlock(msg2, 0, msg2.Length);

			eng.Init(false, pubParameters);

			if (!IsSameAs(data, 1, sig2))
			{
				Fail("failed ISO9796-1 generation Test 2");
			}

			data = eng.ProcessBlock(data, 0, data.Length);

			if (!AreEqual(msg2, data))
			{
				Fail("failed ISO9796-1 retrieve Test 2");
			}
		}

		[Test]
		public virtual void DoTest3()
		{
			RsaKeyParameters pubParameters = new RsaKeyParameters(false, mod2, pub2);
			RsaKeyParameters privParameters = new RsaKeyParameters(true, mod2, pri2);
			RsaEngine rsa = new RsaEngine();
			byte[] data;

			//
			// ISO 9796-1 - public encrypt, private decrypt
			//
			ISO9796d1Encoding eng = new ISO9796d1Encoding(rsa);

			eng.Init(true, privParameters);

			eng.SetPadBits(4);

			data = eng.ProcessBlock(msg3, 0, msg3.Length);

			eng.Init(false, pubParameters);

			if (!IsSameAs(sig3, 1, data))
			{
				Fail("failed ISO9796-1 generation Test 3");
			}

			data = eng.ProcessBlock(data, 0, data.Length);

			if (!IsSameAs(msg3, 0, data))
			{
				Fail("failed ISO9796-1 retrieve Test 3");
			}
		}

		[Test]
		public virtual void DoTest4()
		{
			RsaKeyParameters pubParameters = new RsaKeyParameters(false, mod3, pub3);
			RsaKeyParameters privParameters = new RsaKeyParameters(true, mod3, pri3);
			RsaEngine rsa = new RsaEngine();
			byte[] data;

			//
			// ISO 9796-2 - Signing
			//
			Iso9796d2Signer eng = new Iso9796d2Signer(rsa, new RipeMD128Digest());

			eng.Init(true, privParameters);

			eng.Update(msg4[0]);
			eng.BlockUpdate(msg4, 1, msg4.Length - 1);

			data = eng.GenerateSignature();

			eng.Init(false, pubParameters);

			if (!IsSameAs(sig4, 0, data))
			{
				Fail("failed ISO9796-2 generation Test 4");
			}

			eng.Update(msg4[0]);
			eng.BlockUpdate(msg4, 1, msg4.Length - 1);

			if (!eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 verify Test 4");
			}

			if (eng.HasFullMessage())
			{
				eng = new Iso9796d2Signer(rsa, new RipeMD128Digest());

				eng.Init(false, pubParameters);

				if (!eng.VerifySignature(sig4))
				{
					Fail("failed ISO9796-2 verify and recover Test 4");
				}

				if(!IsSameAs(eng.GetRecoveredMessage(), 0, msg4))
				{
					Fail("failed ISO9796-2 recovered message Test 4");
				}

				// try update with recovered
				eng.UpdateWithRecoveredMessage(sig4);

				if(!IsSameAs(eng.GetRecoveredMessage(), 0, msg4))
				{
					Fail("failed ISO9796-2 updateWithRecovered recovered message Test 4");
				}
				
				if (!eng.VerifySignature(sig4))
				{
					Fail("failed ISO9796-2 updateWithRecovered verify and recover Test 4");
				}
				
				if(!IsSameAs(eng.GetRecoveredMessage(), 0, msg4))
				{
					Fail("failed ISO9796-2 updateWithRecovered recovered verify message Test 4");
				}
				
				// should fail
				eng.UpdateWithRecoveredMessage(sig4);
				
				eng.BlockUpdate(msg4, 0, msg4.Length);
				
				if (eng.VerifySignature(sig4))
				{
					Fail("failed ISO9796-2 updateWithRecovered verify and recover Test 4");
				}
			}
			else
			{
				Fail("full message flag false - Test 4");
			}
		}

		[Test]
		public virtual void DoTest5()
		{
			RsaKeyParameters pubParameters = new RsaKeyParameters(false, mod3, pub3);
			RsaKeyParameters privParameters = new RsaKeyParameters(true, mod3, pri3);
			RsaEngine rsa = new RsaEngine();
			byte[] data;

			//
			// ISO 9796-2 - Signing
			//
			Iso9796d2Signer eng = new Iso9796d2Signer(rsa, new RipeMD160Digest(), true);

			eng.Init(true, privParameters);

			eng.Update(msg5[0]);
			eng.BlockUpdate(msg5, 1, msg5.Length - 1);

			data = eng.GenerateSignature();

			eng.Init(false, pubParameters);

			if (!IsSameAs(sig5, 0, data))
			{
				Fail("failed ISO9796-2 generation Test 5");
			}

			eng.Update(msg5[0]);
			eng.BlockUpdate(msg5, 1, msg5.Length - 1);

			if (!eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 verify Test 5");
			}

			if (eng.HasFullMessage())
			{
				Fail("fullMessage true - Test 5");
			}

			if (!StartsWith(msg5, eng.GetRecoveredMessage()))
			{
				Fail("failed ISO9796-2 partial recovered message Test 5");
			}

			int length = eng.GetRecoveredMessage().Length;

			if (length >= msg5.Length)
			{
				Fail("Test 5 recovered message too long");
			}

			eng = new Iso9796d2Signer(rsa, new RipeMD160Digest(), true);

			eng.Init(false, pubParameters);

			eng.UpdateWithRecoveredMessage(sig5);

			if (!StartsWith(msg5, eng.GetRecoveredMessage()))
			{
				Fail("failed ISO9796-2 updateWithRecovered partial recovered message Test 5");
			}

			if (eng.HasFullMessage())
			{
				Fail("fullMessage updateWithRecovered true - Test 5");
			}

			for (int i = length ; i != msg5.Length; i++)
			{
				eng.Update(msg5[i]);
			}

			if (!eng.VerifySignature(sig5))
			{
				Fail("failed ISO9796-2 verify Test 5");
			}

			if (eng.HasFullMessage())
			{
				Fail("fullMessage updateWithRecovered true - Test 5");
			}

			// should fail
			eng.UpdateWithRecoveredMessage(sig5);

			eng.BlockUpdate(msg5, 0, msg5.Length);

			if (eng.VerifySignature(sig5))
			{
				Fail("failed ISO9796-2 updateWithRecovered verify fail Test 5");
			}
		}

		//
		// against a zero length string
		//
		[Test]
		public virtual void DoTest6()
		{
			byte[] salt = Hex.Decode("61DF870C4890FE85D6E3DD87C3DCE3723F91DB49");
			RsaKeyParameters pubParameters = new RsaKeyParameters(false, mod6, pub6);
			RsaKeyParameters privParameters = new RsaKeyParameters(true, mod6, pri6);
			ParametersWithSalt sigParameters = new ParametersWithSalt(privParameters, salt);
			RsaEngine rsa = new RsaEngine();
			byte[] data;

			//
			// ISO 9796-2 - PSS Signing
			//
			Iso9796d2PssSigner eng = new Iso9796d2PssSigner(rsa, new RipeMD160Digest(), 20, true);

			eng.Init(true, sigParameters);

			data = eng.GenerateSignature();

			eng.Init(false, pubParameters);

			if (!IsSameAs(sig6, 1, data))
			{
				Fail("failed ISO9796-2 generation Test 6");
			}

			if (!eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 verify Test 6");
			}
		}

		[Test]
		public virtual void DoTest7()
		{
			byte[] salt = new byte[0];
			RsaKeyParameters pubParameters = new RsaKeyParameters(false, mod6, pub6);
			RsaKeyParameters privParameters = new RsaKeyParameters(true, mod6, pri6);
			ParametersWithSalt sigParameters = new ParametersWithSalt(privParameters, salt);
			RsaEngine rsa = new RsaEngine();
			byte[] data;

			//
			// ISO 9796-2 - PSS Signing
			//
			Iso9796d2PssSigner eng = new Iso9796d2PssSigner(rsa, new Sha1Digest(), 0, false);

			eng.Init(true, sigParameters);

			eng.Update(msg7[0]);
			eng.BlockUpdate(msg7, 1, msg7.Length - 1);

			data = eng.GenerateSignature();

			eng.Init(false, pubParameters);

			if (!IsSameAs(sig7, 0, data))
			{
				Fail("failed ISO9796-2 generation Test 7");
			}

			eng.Update(msg7[0]);
			eng.BlockUpdate(msg7, 1, msg7.Length - 1);

			if (!eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 verify Test 7");
			}

			if (!IsSameAs(msg7, 0, eng.GetRecoveredMessage()))
			{
				Fail("failed ISO9796-2 recovery Test 7");
			}
		}

		[Test]
		public virtual void DoTest8()
		{
			byte[] salt = Hex.Decode("78E293203CBA1B7F92F05F4D171FF8CA3E738FF8");
			RsaKeyParameters pubParameters = new RsaKeyParameters(false, mod6, pub6);
			RsaKeyParameters privParameters = new RsaKeyParameters(true, mod6, pri6);
			ParametersWithSalt sigParameters = new ParametersWithSalt(privParameters, salt);
			RsaEngine rsa = new RsaEngine();
			byte[] data;

			//
			// ISO 9796-2 - PSS Signing
			//
			Iso9796d2PssSigner eng = new Iso9796d2PssSigner(rsa, new RipeMD160Digest(), 20, false);

			eng.Init(true, sigParameters);

			eng.Update(msg8[0]);
			eng.BlockUpdate(msg8, 1, msg8.Length - 1);

			data = eng.GenerateSignature();

			eng.Init(false, pubParameters);

			if (!IsSameAs(sig8, 0, data))
			{
				Fail("failed ISO9796-2 generation Test 8");
			}

			eng.Update(msg8[0]);
			eng.BlockUpdate(msg8, 1, msg8.Length - 1);

			if (!eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 verify Test 8");
			}
		}

		[Test]
		public virtual void DoTest9()
		{
			RsaKeyParameters pubParameters = new RsaKeyParameters(false, mod6, pub6);
			RsaKeyParameters privParameters = new RsaKeyParameters(true, mod6, pri6);
			RsaEngine rsa = new RsaEngine();
			byte[] data;

			//
			// ISO 9796-2 - PSS Signing
			//
			Iso9796d2PssSigner eng = new Iso9796d2PssSigner(rsa, new RipeMD160Digest(), 0, true);

			eng.Init(true, privParameters);

			eng.Update(msg9[0]);
			eng.BlockUpdate(msg9, 1, msg9.Length - 1);

			data = eng.GenerateSignature();

			eng.Init(false, pubParameters);

			if (!IsSameAs(sig9, 0, data))
			{
				Fail("failed ISO9796-2 generation Test 9");
			}

			eng.Update(msg9[0]);
			eng.BlockUpdate(msg9, 1, msg9.Length - 1);

			if (!eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 verify Test 9");
			}
		}

		[Test]
		public virtual void DoTest10()
		{
			BigInteger          mod = new BigInteger("B3ABE6D91A4020920F8B3847764ECB34C4EB64151A96FDE7B614DC986C810FF2FD73575BDF8532C06004C8B4C8B64F700A50AEC68C0701ED10E8D211A4EA554D", 16);
			BigInteger          pubExp = new BigInteger("65537", 10);
			BigInteger          priExp = new BigInteger("AEE76AE4716F77C5782838F328327012C097BD67E5E892E75C1356E372CCF8EE1AA2D2CBDFB4DA19F703743F7C0BA42B2D69202BA7338C294D1F8B6A5771FF41", 16);
			RsaKeyParameters    pubParameters = new RsaKeyParameters(false, mod, pubExp);
			RsaKeyParameters    privParameters = new RsaKeyParameters(true, mod, priExp);
			RsaEngine           rsa = new RsaEngine();
			byte[]              data;

			//
			// ISO 9796-2 - PSS Signing
			//
			IDigest              dig = new Sha1Digest();
			Iso9796d2PssSigner  eng = new Iso9796d2PssSigner(rsa, dig, dig.GetDigestSize());

			//
			// as the padding is random this test needs to repeat a few times to
			// make sure
			//
			for (int i = 0; i != 500; i++)
			{
				eng.Init(true, privParameters);

				eng.Update(msg9[0]);
				eng.BlockUpdate(msg9, 1, msg9.Length - 1);

				data = eng.GenerateSignature();

				eng.Init(false, pubParameters);

				eng.Update(msg9[0]);
				eng.BlockUpdate(msg9, 1, msg9.Length - 1);

				if (!eng.VerifySignature(data))
				{
					Fail("failed ISO9796-2 verify Test 10");
				}
			}
		}

		[Test]
		public virtual void DoTest11()
		{
			BigInteger          mod = new BigInteger("B3ABE6D91A4020920F8B3847764ECB34C4EB64151A96FDE7B614DC986C810FF2FD73575BDF8532C06004C8B4C8B64F700A50AEC68C0701ED10E8D211A4EA554D", 16);
			BigInteger          pubExp = new BigInteger("65537", 10);
			BigInteger          priExp = new BigInteger("AEE76AE4716F77C5782838F328327012C097BD67E5E892E75C1356E372CCF8EE1AA2D2CBDFB4DA19F703743F7C0BA42B2D69202BA7338C294D1F8B6A5771FF41", 16);
			RsaKeyParameters    pubParameters = new RsaKeyParameters(false, mod, pubExp);
			RsaKeyParameters    privParameters = new RsaKeyParameters(true, mod, priExp);
			RsaEngine           rsa = new RsaEngine();
			byte[]              data;
			byte[]              m1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			byte[]              m2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
			byte[]              m3 = { 1, 2, 3, 4, 5, 6, 7, 8 };

			//
			// ISO 9796-2 - PSS Signing
			//
			IDigest              dig = new Sha1Digest();
			Iso9796d2PssSigner  eng = new Iso9796d2PssSigner(rsa, dig, dig.GetDigestSize());

			//
			// check message bounds
			//
			eng.Init(true, privParameters);

			eng.BlockUpdate(m1, 0, m1.Length);

			data = eng.GenerateSignature();

			eng.Init(false, pubParameters);

			eng.BlockUpdate(m2, 0, m2.Length);

			if (eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 m2 verify Test 11");
			}

			eng.Init(false, pubParameters);

			eng.BlockUpdate(m3, 0, m3.Length);

			if (eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 m3 verify Test 11");
			}

			eng.Init(false, pubParameters);

			eng.BlockUpdate(m1, 0, m1.Length);

			if (!eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 verify Test 11");
			}
		}

		[Test]
		public virtual void DoTest12()
		{
			BigInteger          mod = new BigInteger("B3ABE6D91A4020920F8B3847764ECB34C4EB64151A96FDE7B614DC986C810FF2FD73575BDF8532C06004C8B4C8B64F700A50AEC68C0701ED10E8D211A4EA554D", 16);
			BigInteger          pubExp = new BigInteger("65537", 10);
			BigInteger          priExp = new BigInteger("AEE76AE4716F77C5782838F328327012C097BD67E5E892E75C1356E372CCF8EE1AA2D2CBDFB4DA19F703743F7C0BA42B2D69202BA7338C294D1F8B6A5771FF41", 16);
			RsaKeyParameters    pubParameters = new RsaKeyParameters(false, mod, pubExp);
			RsaKeyParameters    privParameters = new RsaKeyParameters(true, mod, priExp);
			RsaEngine           rsa = new RsaEngine();
			byte[]              data;
			byte[]              m1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			byte[]              m2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
			byte[]              m3 = { 1, 2, 3, 4, 5, 6, 7, 8 };

			//
			// ISO 9796-2 - Regular Signing
			//
			IDigest           dig = new Sha1Digest();
			Iso9796d2Signer  eng = new Iso9796d2Signer(rsa, dig);

			//
			// check message bounds
			//
			eng.Init(true, privParameters);

			eng.BlockUpdate(m1, 0, m1.Length);

			data = eng.GenerateSignature();

			eng.Init(false, pubParameters);

			eng.BlockUpdate(m2, 0, m2.Length);

			if (eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 m2 verify Test 12");
			}

			eng.Init(false, pubParameters);

			eng.BlockUpdate(m3, 0, m3.Length);

			if (eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 m3 verify Test 12");
			}

			eng.Init(false, pubParameters);

			eng.BlockUpdate(m1, 0, m1.Length);

			if (!eng.VerifySignature(data))
			{
				Fail("failed ISO9796-2 verify Test 12");
			}
		}

		public override void PerformTest()
		{
			DoTest1();
			DoTest2();
			DoTest3();
			DoTest4();
			DoTest5();
			DoTest6();
			DoTest7();
			DoTest8();
			DoTest9();
			DoTest10();
			DoTest11();
			DoTest12();
		}

		public static void Main(
			string[] args)
		{
			RunTest(new ISO9796Test());
		}
	}
}
