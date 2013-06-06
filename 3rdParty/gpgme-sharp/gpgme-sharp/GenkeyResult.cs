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
using System.Threading;
using Libgpgme.Interop;

namespace Libgpgme
{
    public class GenkeyResult
    {
        private string fingerprint;
        private bool hasPrimary;
        private bool hasSub;
        internal GenkeyResult(IntPtr keyresultPtr)
        {
            if (keyresultPtr == IntPtr.Zero)
                throw new InvalidPtrException("An invalid key result pointer has been given." +
                                              " Bad programmer! *spank* *spank*");
            UpdateFromMem(keyresultPtr);
        }
        private void UpdateFromMem(IntPtr keyresultPtr)
        {
            _gpgme_op_genkey_result result = new _gpgme_op_genkey_result();
            Marshal.PtrToStructure(keyresultPtr, result);
            fingerprint = Gpgme.PtrToStringAnsi(result.fpr);
            hasPrimary = result.primary;
            hasSub = result.sub;
        }
        public string Fingerprint
        {
            get { return fingerprint; }
        }
        public bool HasPrimary
        {
            get { return hasPrimary; }
        }
        public bool HasSub
        {
            get { return hasSub; }
        }
    }
}
