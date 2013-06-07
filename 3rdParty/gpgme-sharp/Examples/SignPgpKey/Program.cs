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

using Libgpgme;

namespace SignPgpKey
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

            /* Enable the listing of signatures. By default
             * key signatures are NOT passed.
             */
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
                Console.WriteLine("You may want to create the PGP key by using the appropriate\n"
                    + "sample in the Samples/ directory.");
                return;
            }

            // Print out all Uids from Bob's key
            PrintUidData(bob);

            // Print out all Uids from Alice's key
            PrintUidData(alice);


            Console.WriteLine("Set Alice's PGP key as signer key.");
            // Clear signer list (remove default key)
            ctx.Signers.Clear();
            // Add Alice's key as signer
            ctx.Signers.Add(alice);

            /* Set the password callback - needed if the user doesn't run
             * gpg-agent or any other password / pin-entry software.
             */
            ctx.SetPassphraseFunction(new PassphraseDelegate(MyPassphraseCallback));

            Console.WriteLine("Sign Bob's PGP key with Alice's key.. ");

            /////// SIGN KEY ///////

            PgpSignatureOptions signopts = new PgpSignatureOptions();

            signopts.SelectedUids = new int[] { 1 }; // sign the latest Uid only!
            signopts.TrustLevel = PgpSignatureTrustLevel.Full;
            signopts.Type = PgpSignatureType.Trust | PgpSignatureType.NonExportable;

            try
            {
                bob.Sign(ctx, signopts);
            }
            catch (AlreadySignedException)
            {
                Console.WriteLine("Bob's key is already signed!");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Refresh Bob's key 
            bob = (PgpKey)keyring.GetKey(bob.Fingerprint, false);

            PrintUidData(bob);

            /////// REVOKE SIGNATURE ///////

            Console.WriteLine("Revoke the signature..");

            // We need to find Alice's signature first
            int nsignature = 0;
            foreach (KeySignature keysig in bob.Uid.Signatures)
            {
                if (!keysig.Revoked)
                    nsignature++; // do not count revocation certificates

                if (keysig.KeyId.Equals(alice.KeyId) && 
                    !keysig.Revoked) // must not be a revocation certificate
                    break; // found!
            }
            
            PgpRevokeSignatureOptions revopts = new PgpRevokeSignatureOptions();
            revopts.SelectedUid = 1; // latest uid
            revopts.SelectedSignatures = new int[] { nsignature };
            revopts.ReasonText = "Test revocation";

            bob.RevokeSignature(ctx, revopts);

            // Refresh Bob's key
            bob = (PgpKey)keyring.GetKey(bob.Fingerprint, false);

            PrintUidData(bob);

            /////// DELETE SIGNATURE ///////

            Console.WriteLine("Remove Alice's signature and revocation certificate(s)..");

            List<int> siglst = new List<int>();
            nsignature = 0;
            foreach (KeySignature keysig in bob.Uid.Signatures)
            {
                nsignature++;
                if (keysig.KeyId.Equals(alice.KeyId))
                    siglst.Add(nsignature);
            }

            PgpDeleteSignatureOptions delsigopts = new PgpDeleteSignatureOptions();
            delsigopts.DeleteSelfSignature = false;
            delsigopts.SelectedUid = 1 ;
            delsigopts.SelectedSignatures = siglst.ToArray();

            bob.DeleteSignature(ctx, delsigopts);

            // Refresh Bob's key
            bob = (PgpKey)keyring.GetKey(bob.Fingerprint, false);

            PrintUidData(bob);

            return;
        }

        private static void PrintUidData(PgpKey key)
        {
            if (key.Uid == null)
                throw new InvalidKeyException();
            
            Console.WriteLine("{0}'s key {1}\nhas the following Uids and signatures",
                key.Uid.Name,
                key.Fingerprint);
            foreach (UserId id in key.Uids)
            {
                Console.WriteLine("\tReal name: {0}\n\t"
                    + "Email: {1}\n\t"
                    + "Comment: {2}\n\t"
                    + "Invalid: {3}\n\t"
                    + "Revoked: {4}\n\t"
                    + "Validity: {5}\n\t",
                    id.Name,
                    id.Email,
                    id.Comment,
                    id.Invalid.ToString(),
                    id.Revoked.ToString(),
                    id.Validity.ToString());

                Console.WriteLine("\tSignatures:");
                if (id.Signatures != null)
                {
                    foreach (KeySignature keysig in id.Signatures)
                        Console.WriteLine("\t\tFrom: {0}\n\t\t"
                            + "Key id: {1}\n\t\t"
                            + "Date: {2}\n\t\t"
                            + "Revoked: {3}\n\t\t"
                            + "Expires: {4}\n\t\t"
                            + "Invalid: {5}\n",
                            keysig.Name,
                            keysig.KeyId,
                            keysig.Timestamp.ToString(),
                            keysig.Revoked.ToString(),
                            keysig.Expires.ToString(),
                            keysig.Invalid.ToString());
                }
                else
                    Console.WriteLine("\t\tNone");
            }
            Console.WriteLine();
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
