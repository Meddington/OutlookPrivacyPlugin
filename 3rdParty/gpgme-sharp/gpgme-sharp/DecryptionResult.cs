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
    public class DecryptionResult
    {
        private string file_name;
        public string FileName
        {
            get { return file_name; }
        }
        
        private bool wrong_key_usage;
        public bool WrongKeyUsage
        {
            get { return wrong_key_usage; }
        }

        private string unsupported_algorithm;
        public string UnsupportedAlgorithm
        {
            get { return unsupported_algorithm; }
        }

        private Recipient recipients;
        public Recipient Recipients
        {
            get { return recipients; }
        }

        internal DecryptionResult(IntPtr rstPtr)
        {
            if (rstPtr == IntPtr.Zero)
                throw new InvalidPtrException("An invalid decryption result structure pointer has been given.");

            UpdateFromMem(rstPtr);

        }
        private void UpdateFromMem(IntPtr rstPtr)
        {
            _gpgme_op_decrypt_result rst = new _gpgme_op_decrypt_result();
            Marshal.PtrToStructure(rstPtr, rst);

            file_name = Gpgme.PtrToStringUTF8(rst.file_name);
            wrong_key_usage = rst.wrong_key_usage;
            unsupported_algorithm = Gpgme.PtrToStringUTF8(rst.unsupported_algorithm);

            if (rst.recipients != IntPtr.Zero)
                recipients = new Recipient(rst.recipients);
        }
    }
}
