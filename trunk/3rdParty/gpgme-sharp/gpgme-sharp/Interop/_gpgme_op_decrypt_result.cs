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
    [StructLayout(LayoutKind.Sequential)]
    internal class _gpgme_op_decrypt_result // gpgme_decrypt_result_t
    {

        public IntPtr unsupported_algorithm; // char *

        /* Key should not have been used for encryption.  
        uint wrong_key_usage : 1;

        /* Internal to GPGME, do not use.  
        int _unused : 31;
        */
        public uint flags;

        public IntPtr recipients; //gpgme_recipient_t

        /* The original file name of the plaintext message, if
           available.  */
        public IntPtr file_name; //char *

        public bool wrong_key_usage
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
    }
}
