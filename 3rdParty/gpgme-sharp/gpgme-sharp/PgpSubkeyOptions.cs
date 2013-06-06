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
using System.Text;

namespace Libgpgme
{
    public class PgpSubkeyOptions
    {
        public const int KEY_LENGTH_1024 = 1024;
        public const int KEY_LENGTH_2048 = 2048;
        public const int KEY_LENGTH_4096 = 4096;

        static DateTime unixdate = new DateTime(1970, 1, 1);
        internal bool cmdSend = false;

        private int subkeylength = KEY_LENGTH_2048;
        private DateTime expirationdate = unixdate;

        public PgpSubkeyAlgorithm Algorithm;
        public AlgorithmCapability Capability;

        public int KeyLength
        {
            get { return subkeylength; }
            set { subkeylength = value; }
        }

        public bool IsInfinitely
        {
            get { return expirationdate.Equals(unixdate); }
        }
        public void MakeInfinitely()
        {
            expirationdate = unixdate;
        }
        public DateTime ExpirationDate
        {
            get { return expirationdate; }
            set { expirationdate = value; }
        }
        public void SetAlgorithm(KeyAlgorithm algo)
        {
            switch (algo)
            {
                case KeyAlgorithm.DSA:
                    Algorithm = PgpSubkeyAlgorithm.DSASignOnly;
                    break;
                case KeyAlgorithm.ELG:
                    Algorithm = PgpSubkeyAlgorithm.ELGEncryptOnly;
                    break;
                case KeyAlgorithm.RSA:
                    Algorithm = PgpSubkeyAlgorithm.RSAUseCapabilities;
                    break;
                default:
                    throw new System.NotSupportedException("Algorithm is not supported as sub key.",
                        new NotSupportedException("Algorithm is not supported as sub key."));
            }
        }
    }
}
