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
    internal class _gpgme_subkey // gpgme_subkey_t
    {
        /* A subkey from a key.  */
        public IntPtr next;

        /* True if subkey is revoked. 1 
              unsigned int revoked : 1;

           True if subkey is expired. 2 
              unsigned int expired : 1;

           True if subkey is disabled. 4 
              unsigned int disabled : 1;

           True if subkey is invalid. 8 
              unsigned int invalid : 1;

           True if subkey can be used for encryption. 16 
              unsigned int can_encrypt : 1;

           True if subkey can be used for signing. 32 
              unsigned int can_sign : 1;

           True if subkey can be used for certification. 64 
              unsigned int can_certify : 1;

           True if subkey is secret. 128 
              unsigned int secret : 1;

           True if subkey can be used for authentication. 256 
              unsigned int can_authenticate : 1;

           True if subkey is qualified for signatures according to German law. 512 
              unsigned int is_qualified : 1;

           Internal to GPGME, do not use.  
              unsigned int _unused : 22;
         */
        public uint flags;

        /* Public key algorithm supported by this subkey.  */
        public gpgme_pubkey_algo_t pubkey_algo;

        /* Length of the subkey.  */
        public uint length;

        /* The key ID of the subkey.  */
        public IntPtr keyid;    // char*

        /* Internal to GPGME, do not use.  */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public byte[] _keyid;

        /* The fingerprint of the subkey in hex digit form.  */
        public IntPtr fpr;  // char*

        /* The creation timestamp, -1 if invalid, 0 if not available.  */
        public IntPtr timestamp;

        /* The expiration timestamp, 0 if the subkey does not expire.  */
        public IntPtr expires;

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
        public bool expired
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
        public bool disabled
        {
            get { return ((flags & 4) > 0); }
            set
            {
                if (value)
                    flags |= 4;
                else
                    flags &= (~(uint)4);
            }
        }
        public bool invalid
        {
            get { return ((flags & 8) > 0); }
            set
            {
                if (value)
                    flags |= 8;
                else
                    flags &= (~(uint)8);
            }
        }
        public bool can_encrypt
        {
            get { return ((flags & 16) > 0); }
            set
            {
                if (value)
                    flags |= 16;
                else
                    flags &= (~(uint)16);
            }
        }
        public bool can_sign
        {
            get { return ((flags & 32) > 0); }
            set
            {
                if (value)
                    flags |= 32;
                else
                    flags &= (~(uint)32);
            }
        }
        public bool can_certify
        {
            get { return ((flags & 64) > 0); }
            set
            {
                if (value)
                    flags |= 64;
                else
                    flags &= (~(uint)64);
            }
        }
        public bool secret
        {
            get { return ((flags & 128) > 0); }
            set
            {
                if (value)
                    flags |= 128;
                else
                    flags &= (~(uint)128);
            }
        }
        public bool can_authenticate
        {
            get { return ((flags & 256) > 0); }
            set
            {
                if (value)
                    flags |= 256;
                else
                    flags &= (~(uint)256);
            }
        }
        public bool is_qualified
        {
            get { return ((flags & 512) > 0); }
            set
            {
                if (value)
                    flags |= 512;
                else
                    flags &= (~(uint)512);
            }
        }

        public _gpgme_subkey()
        {
            _keyid = new byte[17];
        }
    }
}
