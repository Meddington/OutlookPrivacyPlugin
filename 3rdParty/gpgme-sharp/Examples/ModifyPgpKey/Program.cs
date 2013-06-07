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

namespace ModifyPgpKey
{
    class Program
    {
        static void Main(string[] args)
        {
            Context ctx = new Context();

            if (ctx.Protocol != Protocol.OpenPGP)
                ctx.SetEngineInfo(Protocol.OpenPGP, null, null);

            Console.WriteLine("Search Bob's PGP key in the default keyring..");
            
            String searchstr = "bob@home.internal";
            IKeyStore keyring = ctx.KeyStore;
            
            // retrieve all keys that have Bob's email address 
            Key[] keys = keyring.GetKeyList(searchstr, false);
            if (keys == null || keys.Length == 0)
            {
                Console.WriteLine("Cannot find Bob's PGP key {0} in your keyring.", searchstr);
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
            PgpKey bob = (PgpKey)keys[0]; 
            if (bob.Uid == null || bob.Fingerprint == null)
                throw new InvalidKeyException();

            Console.WriteLine("\nUsing key {0}", bob.Fingerprint);

            /////// CHANGE PASSPHRASE ///////

            Console.WriteLine("Change the secret key's password.");
            
            PgpPassphraseOptions passopts = new PgpPassphraseOptions();
            /* We need to specify our own passphrase callback methods
             * in case the user does not use gpg-agent.
             */
            passopts.OldPassphraseCallback = new PassphraseDelegate(MyPassphraseCallback);
            passopts.NewPassphraseCallback = new PassphraseDelegate(MyNewPassphraseCallback);
            passopts.EmptyOkay = false; // we do not allow an empty passphrase

            bob.ChangePassphrase(ctx, passopts);

            /////// ADD SUBKEY ///////

            Console.Write("Add a new subkey to Bob's key.. ");

            /* Set the password callback - needed if the user doesn't run
             * gpg-agent or any other password / pin-entry software.
             */
            ctx.SetPassphraseFunction(new PassphraseDelegate(MyPassphraseCallback));
            
            PgpSubkeyOptions subopts = new PgpSubkeyOptions();
            subopts.Algorithm = PgpSubkeyAlgorithm.RSAEncryptOnly;
            /* Same as:
               subopts.SetAlgorithm(KeyAlgorithm.RSA);
               subopts.Capability = AlgorithmCapability.CanEncrypt;
             */
            subopts.KeyLength = PgpSubkeyOptions.KEY_LENGTH_4096;
            subopts.ExpirationDate = DateTime.Now.AddDays(90);

            bob.AddSubkey(ctx, subopts);
            
            Console.WriteLine("Done.");

            /////// VIEW SUBKEYS ///////
            
            // Reload Bobs key
            bob = (PgpKey)keyring.GetKey(bob.Fingerprint, false);

            Console.WriteLine("Bob has now the following sub keys:");
            int subkeycount = 0;
            foreach (Subkey subkey in bob.Subkeys)
            {
                subkeycount++;
                Console.WriteLine("{0}\n\tAlgorithm: {1}\n\t"
                    + "Length: {2}\n\t"
                    + "Expires: {3}\n",
                    subkey.Fingerprint,
                    Gpgme.GetPubkeyAlgoName(subkey.PubkeyAlgorithm),
                    subkey.Length.ToString(),
                    subkey.Expires.ToString());
            }
            Console.WriteLine("Found {0} sub keys.", subkeycount.ToString());

            /////// SET OWNER TRUST ///////
            Console.WriteLine("Set owner trust of Bob's key.");
            Console.Write("\tto never.. ");
            bob.SetOwnerTrust(ctx, PgpOwnerTrust.Never);
            Console.WriteLine("done.");
            Console.Write("\tto ultimate.. ");
            bob.SetOwnerTrust(ctx, PgpOwnerTrust.Ultimate);
            Console.WriteLine("done.");

            /////// ENABLE / DISABLE ///////
            Console.Write("Disable Bob's key.. ");
            bob.Disable(ctx);
            Console.WriteLine("done.");
            Console.Write("Enable Bob's key.. ");
            bob.Enable(ctx);
            Console.WriteLine("done.");

            /////// SET EXPIRE DATE ///////
            DateTime newdate = DateTime.Now.AddYears(5);
            Console.WriteLine("Set new expire date: {0}", newdate);
            
            PgpExpirationOptions expopts = new PgpExpirationOptions();
            expopts.ExpirationDate = newdate;
            expopts.SelectedSubkeys = null; // only the primary key
            bob.SetExpirationDate(ctx, expopts);

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

        /// <summary>
        /// Passphrase callback method. Invoked if a action requires the user's password.
        /// </summary>
        /// <param name="ctx">Context that has invoked the callback.</param>
        /// <param name="info">Information about the key.</param>
        /// <param name="passwd">User supplied password.</param>
        /// <returns></returns>
        public static PassphraseResult MyNewPassphraseCallback(
               Context ctx,
               PassphraseInfo info,
               ref char[] passwd)
        {
            Console.Write("Please enter your new passphrase.\n"
             + "Uid: " + info.Uid
             + "\nKey id: " + info.UidKeyId
             + "\nNew password: ");

            passwd = Console.ReadLine().ToCharArray();

            return PassphraseResult.Success;
        }       

    }
 
}
