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
    /* A key from the keyring.  */
    [StructLayout(LayoutKind.Sequential)]
    internal class _gpgme_key //gpgme_key_t
    {
        /* Internal to GPGME, do not use.  */
        public uint _refs;

        /* True if key is revoked.  1
        uint revoked : 1;
           True if key is expired.  2
        uint expired : 1;
           True if key is disabled.  4
        uint disabled : 1;
           True if key is invalid.  8
        uint invalid : 1;
           True if key can be used for encryption.  16
        uint can_encrypt : 1;
           True if key can be used for signing.  32
        uint can_sign : 1;
           True if key can be used for certification.  64
        uint can_certify : 1;
           True if key is secret.  128
        uint secret : 1;
           True if key can be used for authentication.  256
        uint can_authenticate : 1;
           True if subkey is qualified for signatures according to German law.  512
        uint is_qualified : 1;
           Internal to GPGME, do not use.  
        uint _unused : 22; */

        public uint flags;

        /* This is the protocol supported by this key.  */
        public gpgme_protocol_t protocol;

        /* If protocol is GPGME_PROTOCOL_CMS, this string contains the
           issuer serial.  */
        public IntPtr issuer_serial; // char*

        /* If protocol is GPGME_PROTOCOL_CMS, this string contains the
           issuer name.  */
        public IntPtr issuer_name; // char *

        /* If protocol is GPGME_PROTOCOL_CMS, this string contains the chain
           ID.  */
        public IntPtr chain_id; // char *

        /* If protocol is GPGME_PROTOCOL_OpenPGP, this field contains the
           owner trust.  */
        public gpgme_validity_t owner_trust;

        /* The subkeys of the key.  */
        public IntPtr subkeys; // gpgme_subkey_t

        /* The user IDs of the key.  */
        public IntPtr uids;   // gpgme_user_id_t

        /* Internal to GPGME, do not use.  */
        public IntPtr _last_subkey;    //gpgme_subkey_t

        /* Internal to GPGME, do not use.  */
        public IntPtr _last_uid;  //gpgme_user_id_t

        /* The keylist mode that was active when listing the key.  */
        public gpgme_keylist_mode_t keylist_mode;

		internal _gpgme_key() 
		{
			_refs 			= 0;
			flags 			= 0;
			protocol 		= gpgme_protocol_t.GPGME_PROTOCOL_UNKNOWN;
			issuer_serial 	= IntPtr.Zero;
			issuer_name 	= IntPtr.Zero;
			chain_id 		= IntPtr.Zero;
			owner_trust		= gpgme_validity_t.GPGME_VALIDITY_UNKNOWN;
			subkeys 		= IntPtr.Zero;
			uids 			= IntPtr.Zero;
			_last_subkey	= IntPtr.Zero;
			_last_uid 		= IntPtr.Zero;
			keylist_mode 	= gpgme_keylist_mode_t.GPGME_KEYLIST_MODE_LOCAL;
		}
		
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
    }
}
