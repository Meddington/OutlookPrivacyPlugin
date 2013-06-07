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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using Libgpgme.Interop;

namespace Libgpgme
{
    public class ImportStatus: IEnumerable<ImportStatus>
    {
        private ImportStatus next;
        public ImportStatus Next
        {
            get { return next; }
        }

        /* Fingerprint.  */
        private string fpr; // char *
        public String Fpr
        {
            get { return fpr; }
        }

        /* If a problem occured, the reason why the key could not be
           imported.  Otherwise GPGME_No_Error.  */
        private int result; //gpgme_error_t
        public int Result
        {
            get { return result; }
        }

        /* The result of the import, the GPGME_IMPORT_* values bit-wise
           ORed.  0 means the key was already known and no new components
           have been added.  */
        private uint status;

        /* The key was new.  */
        public bool NewKey {
            get { return ((status & (uint)ImportStatusFlags.New) > 0); }
        }
        
        /* The key contained new user IDs.  */
        public bool HasNewUids {
            get { return ((status & (uint)ImportStatusFlags.Uid) > 0); }
        }
         
        /* The key contained new signatures.  */
        public bool HasNewSignatures {
            get { return ((status & (uint)ImportStatusFlags.Signature) > 0); }
        }
        
        /* The key contained new sub keys.  */
        public bool HasNewSubkeys {
            get { return ((status & (uint)ImportStatusFlags.Subkey) > 0); }
        }
        
        /* The key contained a secret key.  */
        public bool NewSecretKey {
            get { return ((status & (uint)ImportStatusFlags.Secret) > 0); }
        }
        
        internal ImportStatus(IntPtr statusPtr)
        {
            if (statusPtr == IntPtr.Zero)
                throw new InvalidPtrException("An invalid import status pointer has been given.");

            UpdateFromMem(statusPtr);
        }

        private void UpdateFromMem(IntPtr statusPtr)
        {
            _gpgme_import_status result = new _gpgme_import_status();
            Marshal.PtrToStructure(statusPtr, result);

            if (result.fpr != IntPtr.Zero)
                fpr = Gpgme.PtrToStringAnsi(result.fpr);
            this.status = result.status;
            this.result = result.result;
            if (result.next != IntPtr.Zero)
                next = new ImportStatus(result.next);

        }

        public IEnumerator<ImportStatus> GetEnumerator()
        {
            ImportStatus st = this;
            while (st != null)
            {
                yield return st;
                st = st.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
