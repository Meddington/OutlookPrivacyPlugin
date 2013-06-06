/*
 * libgpgme-sharp - .NET wrapper classes for libgpgme (GnuPG Made Easy)
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
using System.Reflection;
using System.Runtime.InteropServices;

namespace Libgpgme.Interop
{
    /* Trust items and operations.  */
    [StructLayout(LayoutKind.Sequential)]
    internal class _gpgme_trust_item
    {
        public _gpgme_trust_item()
        {
            _keyid = new byte[17];
            _owner_trust = new byte[2];
            _validity = new byte[2];
        }

        /* Internal to GPGME, do not use.  */
        public uint _refs;

        /* The key ID to which the trust item belongs.  */
        public IntPtr keyid; // char *

        /* Internal to GPGME, do not use.  */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public byte[] _keyid;

        /* The type of the trust item, 1 refers to a key, 2 to a user ID.  */
        public int type;

        /* The trust level.  */
        public int level;

        /* The owner trust if TYPE is 1.  */
        public IntPtr owner_trust; //char *

        /* Internal to GPGME, do not use.  */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _owner_trust;

        /* The calculated validity.  */
        public IntPtr validity; //char *

        /* Internal to GPGME, do not use.  */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] _validity;

        /* The user name if TYPE is 2.  */
        public IntPtr name; //char *
    }
}
