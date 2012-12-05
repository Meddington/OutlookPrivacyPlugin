/*
 * gpgme-sharp - .NET wrapper classes for libgpgme (GnuPG Made Easy)
 *  Copyright (C) 2009 Daniel Mueller <daniel@danm.de>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Libgpgme;

namespace PgpCombinedCrypto
{
    class Program
    {
        static void Main(string[] args)
        {
            Context ctx = new Context();

            if (ctx.Protocol != Protocol.OpenPGP)
                ctx.SetEngineInfo(Protocol.OpenPGP, null, null);

            Console.WriteLine("Search Bob's and Alice's PGP keys in the default keyring..");

            String[] searchpattern = new string[] {
                "bob@home.internal",
                "alice@home.internal" };

            IKeyStore keyring = ctx.KeyStore;

            // We want the key signatures!
            ctx.KeylistMode = KeylistMode.Signatures;

            // retrieve all keys that have Bob's or Alice's email address
            Key[] keys = keyring.GetKeyList(searchpattern, false);

            PgpKey bob = null, alice = null;
            if (keys != null && keys.Length != 0)
            {
                foreach (Key k in keys)
                {
                    if (k.Uid != null)
                    {
                        if (bob == null && k.Uid.Email.ToLower().Equals("bob@home.internal"))
                            bob = (PgpKey)k;
                        if (alice == null && k.Uid.Email.ToLower().Equals("alice@home.internal"))
                            alice = (PgpKey)k;
                    }
                    else
                        throw new InvalidKeyException();
                }
            }

            if (bob == null || alice == null)
            {
                Console.WriteLine("Cannot find Bob's or Alice's PGP key in your keyring.");
                Console.WriteLine("You may want to create the PGP keys by using the appropriate\n"
                    + "sample in the Samples/ directory.");
                return;
            }

            // Create a sample string
            StringBuilder randomtext = new StringBuilder();
            for (int i = 0; i < 80 * 6; i++)
                randomtext.Append((char)(34 + i % 221));
            string origintxt = new string('+', 508)
                + " Die Gedanken sind frei "
                + new string('+', 508)
                + randomtext.ToString();

            // we want our string UTF8 encoded.
            UTF8Encoding utf8 = new UTF8Encoding();

            // Write sample string to plain.txt
            File.WriteAllText("plain.txt", origintxt, utf8);

            /////// ENCRYPT AND SIGN DATA ///////

            Console.Write("Encrypt data for Bob and sign it with Alice's PGP key.. ");

            GpgmeData plain = new GpgmeFileData("plain.txt");
            GpgmeData cipher = new GpgmeFileData("cipher.asc");

            // we want or PGP encrypted/signed data RADIX/BASE64 encoded.
            ctx.Armor = true;

            /* Set the password callback - needed if the user doesn't run
             * gpg-agent or any other password / pin-entry software.
             */
            ctx.SetPassphraseFunction(new PassphraseDelegate(MyPassphraseCallback));

            // Set Alice's PGP key as signer
            ctx.Signers.Clear();
            ctx.Signers.Add(alice);

            EncryptionResult encrst = ctx.EncryptAndSign(
                new Key[] { bob },
                EncryptFlags.AlwaysTrust,
                plain,
                cipher);

            Console.WriteLine("done.");

            // print out invalid signature keys
            if (encrst.InvalidRecipients != null)
            {
                foreach (InvalidKey key in encrst.InvalidRecipients)
                    Console.WriteLine("Invalid key: {0} ({1})",
                        key.Fingerprint,
                        key.Reason);
            }

            plain.Close();
            plain = null;
            cipher.Close();
            cipher = null;

            /////// DECRYPT AND VERIFY DATA ///////

            Console.Write("Decrypt and verify data.. ");

            cipher = new GpgmeFileData("cipher.asc");
            plain = new GpgmeMemoryData();

            CombinedResult comrst = ctx.DecryptAndVerify(
                cipher, // source buffer
                plain); // destination buffer
            
            Console.WriteLine("Done. Filename: \"{0}\" Recipients:",
                comrst.DecryptionResult.FileName);

            /* print out all recipients key ids (a PGP package can be 
             * encrypted to various recipients).
             */
            DecryptionResult decrst = comrst.DecryptionResult;
            if (decrst.Recipients != null)
                foreach (Recipient recp in decrst.Recipients)
                    Console.WriteLine("\tKey id {0} with {1} algorithm",
                        recp.KeyId,
                        Gpgme.GetPubkeyAlgoName(recp.KeyAlgorithm));
            else
                Console.WriteLine("\tNone");

            // print out signature information
            VerificationResult verrst = comrst.VerificationResult;
            if (verrst.Signature != null)
            {
            	foreach (Signature sig in verrst.Signature) {
                	Console.WriteLine("Verification result (signature): "
                	    + "\n\tFingerprint: {0}"
                	    + "\n\tHash algorithm: {1}"
                	    + "\n\tKey algorithm: {2}"
                	    + "\n\tTimestamp: {3}"
                	    + "\n\tSummary: {4}"
                	    + "\n\tValidity: {5}",
                	    sig.Fingerprint,
                	    Gpgme.GetHashAlgoName(sig.HashAlgorithm),
                	    Gpgme.GetPubkeyAlgoName(sig.PubkeyAlgorithm),
                	    sig.Timestamp,
                	    sig.Summary,
                	    sig.Validity);
                }
            }

            return;
        }

        /// <summary>
        /// Passphrase callback method. Invoked if a action requires the user's password.
        /// </summary>
        /// <param name="ctx">Context that has invoked the callback.</param>
        /// <param name="info">Information about the key.</param>
        /// <param name="passwd">User supplied password.</param>
        /// <returns></returns>
        public static PassphraseResult MyPassphraseCallback(
               Context ctx,
               PassphraseInfo info,
               ref char[] passwd)
        {
            Console.Write("You need to enter your passphrase.\n"
             + "Uid: " + info.Uid
             + "\nKey id: " + info.UidKeyId
             + "\nPrevious passphrase was bad: " + info.PrevWasBad
             + "\nPassword: ");

            passwd = Console.ReadLine().ToCharArray();

            return PassphraseResult.Success;
        }
    }
}
