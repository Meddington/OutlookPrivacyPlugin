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
    public class VerificationResult
    {
        private string file_name;
        public string FileName
        {
            get { return file_name; }
        }

        private Signature signature;
        public Signature Signature
        {
            get { return signature; }
        }

        internal VerificationResult(IntPtr rstPtr)
        {
            if (rstPtr == IntPtr.Zero)
                throw new InvalidPtrException("An invalid verify result pointer has been given.");

            UpdateFromMem(rstPtr);
        }

        private void UpdateFromMem(IntPtr sigPtr)
        {
            _gpgme_op_verify_result ver = new _gpgme_op_verify_result();
            Marshal.PtrToStructure(sigPtr, ver);
            file_name = Gpgme.PtrToStringUTF8(ver.file_name);

            if (ver.signature != IntPtr.Zero)
                signature = new Signature(ver.signature);
        }
    }
}
