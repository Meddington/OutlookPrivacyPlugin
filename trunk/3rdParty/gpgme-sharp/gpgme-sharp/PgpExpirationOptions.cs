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
    public class PgpExpirationOptions
    {
        static DateTime unixdate = new DateTime(1970, 1, 1);
        private DateTime expirationdate = unixdate;

        internal bool forceQuit = false;
        internal bool cmdSend = false;
        internal int nsubkey = 0;

        public int[] SelectedSubkeys; // if not set - expire the whole key (pub SC)
        
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
    }
}
