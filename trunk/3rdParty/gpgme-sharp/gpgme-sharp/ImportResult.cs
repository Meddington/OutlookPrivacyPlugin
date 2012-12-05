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
using System.Reflection;
using System.Runtime.InteropServices;
using Libgpgme.Interop;

namespace Libgpgme
{
    public class ImportResult
    {
        /* Number of considered keys.  */
        private int considered;
        public int Considered
        {
            get { return considered; }
        }

        /* Keys without user ID.  */
        private int no_user_id;
        public int NoUserId
        {
            get { return no_user_id; }
        }

        /* Imported keys.  */
        private int imported;
        public int Imported 
        {
            get { return imported; }
        }

        /* Imported RSA keys.  */
        private int imported_rsa;
        public int ImportedRSA
        {
            get { return imported_rsa; }
        }

        /* Unchanged keys.  */
        private int unchanged;
        public int Unchanged
        {
            get { return unchanged; }
        }

        /* Number of new user ids.  */
        private int new_user_ids;
        public int NewUserIds
        {
            get { return new_user_ids; }
        }

        /* Number of new sub keys.  */
        private int new_sub_keys;
        public int NewSubkeys
        {
            get { return new_sub_keys; }
        }

        /* Number of new signatures.  */
        private int new_signatures;
        public int NewSignatures
        {
            get { return new_signatures; }
        }

        /* Number of new revocations.  */
        private int new_revocations;
        public int NewRevocations
        {
            get { return new_revocations; }
        }

        /* Number of secret keys read.  */
        private int secret_read;
        public int SecretRead
        {
            get { return secret_read; }
        }

        /* Number of secret keys imported.  */
        private int secret_imported;
        public int SecretImported
        {
            get { return secret_imported; }
        }

        /* Number of secret keys unchanged.  */
        private int secret_unchanged;
        public int SecretUnchanged
        {
            get { return secret_unchanged; }
        }

        /* Number of new keys skipped.  */
        private int skipped_new_keys;
        public int SkippedNewKeys
        {
            get { return skipped_new_keys; }
        }

        /* Number of keys not imported.  */
        private int not_imported;
        public int NotImported
        {
            get { return not_imported; }
        }

        private ImportStatus imports;
        public ImportStatus Imports
        {
            get { return imports; }
        }

        internal ImportResult(IntPtr resultPtr)
        {
            if (resultPtr == IntPtr.Zero)
                throw new InvalidPtrException("An invalid key import result pointer has been given.");

            UpdateFromMem(resultPtr);
        }

        private void UpdateFromMem(IntPtr resultPtr)
        {
            _gpgme_op_import_result result = new _gpgme_op_import_result();
            Marshal.PtrToStructure(resultPtr, result);

            this.considered = result.considered;
            this.no_user_id = result.no_user_id;
            this.imported = result.imported;
            this.imported_rsa = result.imported_rsa;
            this.unchanged = result.unchanged;
            this.new_user_ids = result.new_user_ids;
            this.new_sub_keys = result.new_sub_keys;
            this.new_signatures = result.new_signatures;
            this.new_revocations = result.new_revocations;
            this.secret_read = result.secret_read;
            this.secret_imported = result.secret_imported;
            this.secret_unchanged = result.secret_unchanged;
            this.skipped_new_keys = result.skipped_new_keys;
            this.not_imported = result.not_imported;

            if (result.imports != IntPtr.Zero)
                this.imports = new ImportStatus(result.imports);
        }
    }
}
