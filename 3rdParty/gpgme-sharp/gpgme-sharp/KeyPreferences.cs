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

namespace Libgpgme
{
    public sealed class KeyPreferences
    {
        public List<CipherAlgorithm> Ciphers = new List<CipherAlgorithm>();
        public List<HashAlgorithm> Hashes = new List<HashAlgorithm>();
        public List<CompressAlgorithm> Compress = new List<CompressAlgorithm>();
        public PgpFeatureFlags PGPFeatures = PgpFeatureFlags.MDC;

        public string GetPrefString()
        {
            StringBuilder sb = new StringBuilder();
            
            // prefered symmetric ciphers
            foreach (CipherAlgorithm algo in Ciphers)
                UpdateSb(sb, "S" + (int)algo);

            // prefered hashes
            foreach (HashAlgorithm hash in Hashes)
                UpdateSb(sb, "H" + (int)hash);

            // prefered compression algorithms
            foreach (CompressAlgorithm compress in Compress)
                UpdateSb(sb, "Z" + (int)compress);

            //if (

            return sb.ToString();

        }
        private void UpdateSb(StringBuilder sb, string addtxt)
        {
            if (sb.Length > 0)
                sb.Append(" ");
            sb.Append(addtxt);
        }
    }
}