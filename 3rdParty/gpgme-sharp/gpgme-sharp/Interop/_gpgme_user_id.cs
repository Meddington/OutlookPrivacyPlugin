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
    /* An user ID from a key.  */
    [StructLayout(LayoutKind.Sequential)]
    internal class _gpgme_user_id // gpgme_user_id_t
    {
        public IntPtr next;
        /* True if the user ID is revoked.  
            unsigned int revoked : 1;

           True if the user ID is invalid.  
            unsigned int invalid : 1;

           Internal to GPGME, do not use.  
            unsigned int _unused : 30; */
        public uint flags;

        /* The validity of the user ID.  */
        public gpgme_validity_t validity;

        /* The user ID string.  */
        public IntPtr uid;  // char*

        /* The name part of the user ID.  */
        public IntPtr name; // char*

        /* The email part of the user ID.  */
        public IntPtr email;  // char*

        /* The comment part of the user ID.  */
        public IntPtr comment; // char*

        /* The signatures of the user ID.  */
        public IntPtr signatures; // gpgme_key_sig_t

        /* Internal to GPGME, do not use.  */
        public IntPtr _last_keysig; //gpgme_key_sig_t

        public bool revoked
        {
            get { return ((flags & 1) > 0); }
            set
            {
                if (value)
                    flags |= 1;
                else
                    flags &= (~(uint)1);
            }
        }
        public bool invalid
        {
            get { return ((flags & 2) > 0); }
            set
            {
                if (value)
                    flags |= 2;
                else
                    flags &= (~(uint)2);
            }
        }
    }
}
