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
    internal class _gpgme_new_signature // // gpgme_new_signature_t
    {
        /* Signing.  */
        public IntPtr next;

        /* The type of the signature.  */
        public gpgme_sig_mode_t type;
        /* The public key algorithm used to create the signature.  */
        public gpgme_pubkey_algo_t pubkey_algo;
        /* The hash algorithm used to create the signature.  */
        public gpgme_hash_algo_t hash_algo;
        /* Internal to GPGME, do not use.  Must be set to the same value as
            CLASS below.  */
        public IntPtr _obsolete_class;
        /* Signature creation time.  */

        //TODO - stimmt das wirklich?
        public IntPtr timestamp; // long int
        /* The fingerprint of the signature.  */
        public IntPtr fpr; //char *
        public uint _obsolete_class_2;
        /* Crypto backend specific signature class.  */
        public uint sig_class;
    }
}
