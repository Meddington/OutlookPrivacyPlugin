/*
 * gpgme-sharp - .NET wrapper classes for libgpgme (GnuPG Made Easy)
 *  Copyright (C) 2008 Daniel Mueller <daniel@danm.de>
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
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Libgpgme
{
    public sealed class KeyParameters
    {
        public const int KEY_LENGTH_1024 = 1024;
        public const int KEY_LENGTH_2048 = 2048;
        public const int KEY_LENGTH_4096 = 4096;

        static DateTime unixdate = new DateTime(1970, 1, 1);

        private string format = "internal";
        private KeyAlgorithm pubkeytype = KeyAlgorithm.DSA;
        private AlgorithmCapability pubkeycap = AlgorithmCapability.CanSign | AlgorithmCapability.CanCert | AlgorithmCapability.CanAuth;
        private int keylength = KEY_LENGTH_1024;
        
        private KeyAlgorithm subkeytype = KeyAlgorithm.ELG_E;
        private AlgorithmCapability subkeycap = AlgorithmCapability.CanEncrypt;
        private int subkeylength = KEY_LENGTH_1024;
        
        private string realname = "";
        private string comment = "";
        private string email = "";
        private DateTime expirationdate = unixdate;
        private string passphrase = "";
        private bool nosubkey = false;
        private bool autoalgocap = true;
        // X.509
        private string namedn;

        /* If set to TRUE: everytime the user changes the *keytype 
         * (algorithm) of the pub or sub key the class
         * automatically finds the correct key usage flags.
         */
        public bool AutoKeyUsage 
        {
            get { return autoalgocap; }
            set { autoalgocap = value; }
        }

        public KeyAlgorithm PubkeyAlgorithm
        {
            get { return pubkeytype; }
            set 
            { 
                pubkeytype = value;
                if (autoalgocap)
                    pubkeycap = Gpgme.GetAlgorithmCapability<KeyAlgorithm>(value);
            }
        }
        public AlgorithmCapability PubkeyUsage
        {
            get { return pubkeycap; }
            set { pubkeycap = value; }
        }
        public int KeyLength
        {
            get { return keylength; }
            set { keylength = value; }
        }
        public int PubkeyLength
        {
            get { return KeyLength; }
            set { KeyLength = value; }
        }
        public bool NoSubkey
        {
            get { return nosubkey; }
            set { nosubkey = value; }
        }
        public KeyAlgorithm SubkeyAlgorithm
        {
            get { return subkeytype; }
            set 
            { 
                subkeytype = value;
                if (autoalgocap)
                    subkeycap = Gpgme.GetAlgorithmCapability<KeyAlgorithm>(value);
            }
        }
        public AlgorithmCapability SubkeyUsage
        {
            get { return subkeycap; }
            set { subkeycap = value; }
        }

        public int SubkeyLength
        {
            get { return subkeylength; }
            set { subkeylength = value; }
        }
        public string RealName
        {
            get { return realname; }
            set
            {
                if (CheckForInvalidChars(value))
                    throw new InvalidPassphraseException("Real name contains invalid chars.");
                realname = value;
            }
        }
        public string Comment
        {
            get { return comment; }
            set
            {
                if (CheckForInvalidChars(value))
                    throw new InvalidPassphraseException("Comment contains invalid chars.");
                comment = value;
            }
        }
        public string Email
        {
            get { return email; }
            set
            {
                if (CheckForInvalidChars(value))
                    throw new InvalidPassphraseException("Email contains invalid chars.");
                email = value;
            }
        }
        public DateTime ExpirationDate
        {
            get { return expirationdate; }
            set { expirationdate = value; }
        }
        public string Passphrase
        {
            get { return passphrase; }
            set
            {
                if (CheckForInvalidChars(value))
                    throw new InvalidPassphraseException("Passphrase contains invalid chars.");
                passphrase = value;
            }
        }
        public string NameDN
        {
            get { return namedn; }
            set { namedn = value; }
        }

        internal string GetXmlText(Protocol protocoltype)
        {
            StringBuilder sb = new StringBuilder();

            switch (protocoltype)
            {
                case Protocol.OpenPGP:
                    // obsolete algorithm
                    if (pubkeytype == KeyAlgorithm.RSA_S)
                        throw new InvalidPubkeyAlgoException("RSA-S is obsolete and therefore not supported. [RFC4880#9.1]");

                    // invalid algorithm for (primary) asymmetric key
                    if (pubkeytype == KeyAlgorithm.RSA_E ||
                        pubkeytype == KeyAlgorithm.ELG_E ||
                        pubkeytype == KeyAlgorithm.ELG)
                        throw new InvalidPubkeyAlgoException("The primary key algorithm must be utilizable to sign. Choose DSA or RSA and specify the key usage attributes.");

                    // invalid capability attribute
                    if ((pubkeycap & AlgorithmCapability.CanSign) != AlgorithmCapability.CanSign)
                        throw new InvalidPubkeyAlgoException("The primary key must have sign capabilies.");

                    sb.Append("<GnupgKeyParms format=\"" + format + "\">\n");
                    sb.Append("Key-Type: " + GetAttrDesc<KeyAlgorithm>(pubkeytype) + "\n");
                    sb.Append("Key-Usage: ");
                    sb.Append(AlgorithmCapabilityAttribute.GetKeyUsageText(pubkeycap));
                    sb.Append("\n");

                    sb.Append("Key-Length: " + keylength + "\n");

                    if (!nosubkey)
                    {
                        if (subkeytype == KeyAlgorithm.RSA_E || // RSA-S/E are obsolete [RFC4880#9.1]
                            subkeytype == KeyAlgorithm.RSA_S)
                            throw new InvalidPubkeyAlgoException("RSA-S/E is obsolete and therefore not supported. [RFC4880#9.1]");

                        sb.Append("Subkey-Type: " + GetAttrDesc<KeyAlgorithm>(subkeytype) + "\n");

                        sb.Append("Subkey-Usage: ");
                        sb.Append(AlgorithmCapabilityAttribute.GetKeyUsageText(subkeycap));
                        sb.Append("\n");

                        sb.Append("Subkey-Length: " + subkeylength + "\n");
                    }

                    sb.Append("Name-Real: " + realname + "\n");
                    if (comment != null && comment.Length != 0)
                        sb.Append("Name-Comment: " + comment + "\n");
                    sb.Append("Name-Email: " + email + "\n");
                    if (expirationdate.Equals(unixdate))
                    {
                        // 0 means the key lifetime is infinitely
                        sb.Append("Expire-Date: 0\n");
                    }
                    else
                    {
                        sb.Append("Expire-Date: " + expirationdate.ToString("yyyy-MM-dd") + "\n");
                    }
                    if (passphrase != null && passphrase.Length > 0)
                        sb.Append("Passphrase: " + passphrase + "\n");
                    sb.Append("</GnupgKeyParms>");

                    break;
                case Protocol.CMS:
                    sb.Append("<GnupgKeyParms format=\"" + format + "\">\n");
                    sb.Append("Key-Type: " + GetAttrDesc<KeyAlgorithm>(pubkeytype) + "\n");
                    sb.Append("Key-Length: " + keylength + "\n");
                    sb.Append("Name-DN: " + namedn + "\n");
                    sb.Append("Name-Email: " + email + "\n");
                    sb.Append("</GnupgKeyParms>");
                    break;
                default:
                    throw new InvalidProtocolException("Invalid protocol");
            }
            return sb.ToString();
        }

        private static string GetAttrDesc<T>(T attr)
        {
            FieldInfo fieldinf = attr.GetType().GetField(attr.ToString());
            try
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldinf.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);
                return (attributes.Length > 0) ? attributes[0].Description : attr.ToString();
            }
            catch
            {
                return "Unknown attribute/description.";
            }
        }

        public bool IsInfinitely
        {
            get { return expirationdate.Equals(unixdate); }
        }
        public void MakeInfinitely()
        {
            expirationdate = unixdate;
        }
        private bool CheckForInvalidChars(string str)
        {
            if (str.Contains("\n") ||
                str.Contains("\0"))
                return true;
            return false;
        }
    }
}    

