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

namespace PgpSignVerify
{
    class Program
    {
        static void Main(string[] args)
        {
            Context ctx = new Context();

            if (ctx.Protocol != Protocol.OpenPGP)
                ctx.SetEngineInfo(Protocol.OpenPGP, null, null);

            Console.WriteLine("Search Alice's PGP key in the default keyring..");

            String searchstr = "alice@home.internal";
            IKeyStore keyring = ctx.KeyStore;

            // retrieve all keys that have Alice's email address 
            Key[] keys = keyring.GetKeyList(searchstr, false);
            if (keys == null || keys.Length == 0)
            {
                Console.WriteLine("Cannot find Alice's PGP key {0} in your keyring.", searchstr);
                Console.WriteLine("You may want to create the PGP key by using the appropriate\n"
                    + "sample in the Samples/ directory.");
                return;
            }

            // print a list of all returned keys
            foreach (Key key in keys)
                if (key.Uid != null
                    && key.Fingerprint != null)
                    Console.WriteLine("Found key {0} with fingerprint {1}",
                        key.Uid.Name,
                        key.Fingerprint);

            // we are going to use the first key in the list
            PgpKey alice = (PgpKey)keys[0];
            if (alice.Uid == null || alice.Fingerprint == null)
                throw new InvalidKeyException();

            // Create a sample string
            StringBuilder randomtext = new StringBuilder();
            for (int i = 0; i < 80 * 6; i++)
                randomtext.Append((char)(34 + i % 221));
            string origintxt = new string('+', 508)
                + " Die Gedanken sind frei "
                + new string('+', 508)
                + randomtext.ToString();

            Console.WriteLine("Text to be signed:\n\n{0}", origintxt);

            // we want our string UTF8 encoded.
            UTF8Encoding utf8 = new UTF8Encoding();

            // Prepare a file for later usage
            File.WriteAllText("original.txt", origintxt, utf8 );

            /////// SIGN DATA (detached signature) ///////

            Console.Write("Write a detached signature to file: original.txt.sig.. ");

            GpgmeData origin = new GpgmeFileData("original.txt", 
                FileMode.Open, 
                FileAccess.Read);

            GpgmeData detachsig = new GpgmeFileData("original.txt.sig", 
                FileMode.Create, 
                FileAccess.Write);

            // Set Alice as signer
            ctx.Signers.Clear();
            ctx.Signers.Add(alice);

            // we want or PGP encrypted/signed data RADIX/BASE64 encoded.
            ctx.Armor = true;

            /* Set the password callback - needed if the user doesn't run
             * gpg-agent or any other password / pin-entry software.
             */
            ctx.SetPassphraseFunction(new PassphraseDelegate(MyPassphraseCallback));

            // create a detached signature
            SignatureResult sigrst = ctx.Sign(
                origin,     // plain text (source buffer)
                detachsig,  // signature (destination buffer)
                SignatureMode.Detach);

            Console.WriteLine("done.");

            // print out invalid signature keys
            if (sigrst.InvalidSigners != null)
            {
                foreach (InvalidKey key in sigrst.InvalidSigners)
                    Console.WriteLine("Invalid key: {0} ({1})",
                        key.Fingerprint,
                        key.Reason);
            }

            // print out signature information
            if (sigrst.Signatures != null)
            {
                foreach (NewSignature newsig in sigrst.Signatures)
                    Console.WriteLine("New signature: "
                        + "\n\tFingerprint: {0}"
                        + "\n\tHash algorithm: {1}"
                        + "\n\tKey algorithm: {2}"
                        + "\n\tTimestamp: {3}"
                        + "\n\tType: {4}",
                        newsig.Fingerprint,
                        Gpgme.GetHashAlgoName(newsig.HashAlgorithm),
                        Gpgme.GetPubkeyAlgoName(newsig.PubkeyAlgorithm),
                        newsig.Timestamp,
                        newsig.Type);
            }

            origin.Close();
            origin = null;

            detachsig.Close();
            detachsig = null;

            /////// VERIFY DATA (detached signature) ///////
            Console.Write("Verify a detached signature from file: original.txt.sig.. ");

            origin = new GpgmeFileData("original.txt",
                FileMode.Open,
                FileAccess.Read);

            detachsig = new GpgmeFileData("original.txt.sig",
                FileMode.Open,
                FileAccess.Read);

            VerificationResult verrst = ctx.Verify(
                detachsig,  // detached signature
                origin,     // original data
                null);      // should be NULL if a detached signature has been provided

            Console.WriteLine("done.");
            Console.WriteLine("Filename: {0}", verrst.FileName);

            // print out signature information
            if (verrst.Signature != null)
            {
            	foreach(Signature sig in verrst.Signature) {
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
