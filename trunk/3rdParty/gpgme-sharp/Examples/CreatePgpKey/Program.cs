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

namespace CreatePgpKey
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This example will create PGP keys in your default keyring.\n");

            // First step is to create a context
            Context ctx = new Context();

            EngineInfo info = ctx.EngineInfo;
            
            if (info.Protocol != Protocol.OpenPGP)
            {
                ctx.SetEngineInfo(Protocol.OpenPGP, null, null);
                info = ctx.EngineInfo;
            }

            Console.WriteLine("GnuPG home directory: {0}\n"
                + "Version: {1}\n"
                + "Reqversion: {2} \n"
                + "Program: {3}\n",
                info.HomeDir,
                info.Version,
                info.ReqVersion,
                info.FileName);

            IKeyGenerator keygen = ctx.KeyStore;

            KeyParameters aliceparam, bobparam, malloryparam;

            aliceparam = new KeyParameters();
            aliceparam.RealName = "Alice";
            aliceparam.Comment = "my comment";
            aliceparam.Email = "alice@home.internal";
            aliceparam.ExpirationDate = DateTime.Now.AddYears(3);
 
            // primary key parameters
            aliceparam.KeyLength = KeyParameters.KEY_LENGTH_2048;
            aliceparam.PubkeyAlgorithm = KeyAlgorithm.RSA;
            // the primary key algorithm MUST have the "Sign" capability
            aliceparam.PubkeyUsage = 
                AlgorithmCapability.CanSign 
                | AlgorithmCapability.CanAuth 
                | AlgorithmCapability.CanCert;

            // subkey parameters (optional)
            aliceparam.SubkeyLength = KeyParameters.KEY_LENGTH_4096;
            aliceparam.SubkeyAlgorithm = KeyAlgorithm.RSA;
            aliceparam.SubkeyUsage = AlgorithmCapability.CanEncrypt;
            
            aliceparam.Passphrase = "topsecret";

            // Generate Alice key
            Console.WriteLine(
                "Create a new PGP key for Alice.\n"
                + "Name: {0}\n"
                + "Comment: {1} \n"
                + "Email: {2} \n"
                + "Secret passphrase: {3} \n"
                + "Expire date: {4} \n"
                + "Primary key algorithm = {5} ({6} bit)\n"
                + "Sub key algorithm = {7} ({8} bit)",
                aliceparam.RealName,
                aliceparam.Comment,
                aliceparam.Email,
                aliceparam.Passphrase,
                aliceparam.ExpirationDate.ToString(),
                Gpgme.GetPubkeyAlgoName(aliceparam.PubkeyAlgorithm),
                aliceparam.PubkeyLength,
                Gpgme.GetPubkeyAlgoName(aliceparam.SubkeyAlgorithm),
                aliceparam.SubkeyLength
                );

            Console.Write("Start key generation.. ");
            GenkeyResult result = keygen.GenerateKey(
                Protocol.OpenPGP,
                aliceparam);

            Console.WriteLine("done.\nFingerprint: {0}\n",
                result.Fingerprint);

            // okay, create two more keys
            
            Console.Write("Create PGP key for Bob.. ");
            bobparam = new KeyParameters();
            bobparam.RealName = "Bob";
            bobparam.Email = "bob@home.internal";
            bobparam.ExpirationDate = DateTime.Now.AddYears(2);
            bobparam.Passphrase = "topsecret";

            result = keygen.GenerateKey(
                Protocol.OpenPGP,
                bobparam);
            Console.WriteLine("done.\nFingerprint: {0}\n",
                result.Fingerprint);

            Console.Write("Create PGP key for Mallory.. ");
            malloryparam = new KeyParameters();
            malloryparam.RealName = "Mallory";
            malloryparam.Email = "mallory@home.internal";
            malloryparam.MakeInfinitely(); // PGP key does not expire
            malloryparam.Passphrase = "topsecret";

            result = keygen.GenerateKey(
                Protocol.OpenPGP,
                malloryparam);
            Console.WriteLine("done.\nFingerprint: {0}",
                result.Fingerprint);

            return;
        }
    }
}
