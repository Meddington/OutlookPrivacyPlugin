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
using System.IO;

namespace Libgpgme
{
    public class PgpRevokeSignatureOptions
    {
        internal bool cmdSend = false; // revsig command send to gnupg?
        internal bool uidSend = false; // uid selected?
        internal bool reasonSend = false;

        internal string[] reasonTxt = null;
        internal int nreasonTxt = 0;

        public int SelectedUid = 1;
        public int[] SelectedSignatures = null;
        internal int nrevokenum = 0;

        public PgpRevokeSignatureReasonCode ReasonCode = PgpRevokeSignatureReasonCode.NoReason;
        public string ReasonText
        {
            get
            {
                if (reasonTxt == null) return null;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < reasonTxt.Length; i++)
                {
                    if (i > 0)
                        sb.Append("\n");
                    sb.Append(reasonTxt[i]);
                }
                return sb.ToString();
            }
            set
            {
                StringReader reader = new StringReader(value);
                List<string> lst = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                    lst.Add(line);
                reasonTxt = lst.ToArray();
            }
        }
    }
}
