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
    public class PgpSignatureOptions
    {
        private static DateTime unixdate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        private int trustdepth = 1;

        internal bool cmdSend = false; // sign command send to gnupg?
        internal bool forceQuit = false;
        internal int nUid = 0;
        internal bool signAllUids = true;

        public PgpSignatureType Type = PgpSignatureType.Normal;
        public PgpSignatureClass Class = PgpSignatureClass.Generic;
        
        public DateTime ExpirationDate = unixdate;
        public PgpSignatureTrustLevel TrustLevel = PgpSignatureTrustLevel.Marginal;
        public int[] SelectedUids = null;
        public int TrustDepth
        {
            get { return trustdepth; }
            set
            {
                if (value > 0)
                    trustdepth = value;
                else
                    throw new GpgmeException("You cannot specify a trust level lower than 1.");
            }
        }

        public string TrustRegexp = "";
        public bool LocalPromoteOkay = true;


        internal string GetExpirationDate()
        {
            if (ExpirationDate.Equals(unixdate))
                return "0";
            else
                return ExpirationDate.ToString("yyyy-MM-dd");
        }

        public bool IsInfinitely
        {
            get { return ExpirationDate.Equals(unixdate); }
            set
            {
                if (value == true)
                    ExpirationDate = unixdate;
            }
        }
    }
}
