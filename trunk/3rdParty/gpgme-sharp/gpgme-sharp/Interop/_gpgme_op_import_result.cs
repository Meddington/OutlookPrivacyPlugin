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
    internal class _gpgme_op_import_result   //gpgme_import_result_t
    {
        /* Number of considered keys.  */
        public int considered;

        /* Keys without user ID.  */
        public int no_user_id;

        /* Imported keys.  */
        public int imported;

        /* Imported RSA keys.  */
        public int imported_rsa;

        /* Unchanged keys.  */
        public int unchanged;

        /* Number of new user ids.  */
        public int new_user_ids;

        /* Number of new sub keys.  */
        public int new_sub_keys;

        /* Number of new signatures.  */
        public int new_signatures;

        /* Number of new revocations.  */
        public int new_revocations;

        /* Number of secret keys read.  */
        public int secret_read;

        /* Number of secret keys imported.  */
        public int secret_imported;

        /* Number of secret keys unchanged.  */
        public int secret_unchanged;

        /* Number of new keys skipped.  */
        public int skipped_new_keys;

        /* Number of keys not imported.  */
        public int not_imported;

        /* List of keys for which an import was attempted.  */
        public IntPtr imports;     //gpgme_import_status_t
    }
}