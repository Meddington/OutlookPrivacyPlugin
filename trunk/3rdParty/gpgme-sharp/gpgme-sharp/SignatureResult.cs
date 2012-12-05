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
    public class SignatureResult
    {
        /* The list of invalid signers.  */
        private InvalidKey invalid_signers;
        public InvalidKey InvalidSigners
        {
            get { return invalid_signers; }
        }
        
        private NewSignature signatures;          
        public NewSignature Signatures
        {
            get { return signatures; }
        }

        internal SignatureResult(IntPtr sigrstPtr)
        {
            if (sigrstPtr.Equals(IntPtr.Zero))
                throw new InvalidPtrException("An invalid signature result pointer has been supplied.");
            UpdateFromMem(sigrstPtr);
        }
        private void UpdateFromMem(IntPtr sigrstPtr)
        {
            _gpgme_op_sign_result rst = new _gpgme_op_sign_result();
            Marshal.PtrToStructure(sigrstPtr, rst);

            if (!rst.invalid_signers.Equals(IntPtr.Zero))
                invalid_signers = new InvalidKey(rst.invalid_signers);

            if (!rst.signatures.Equals(IntPtr.Zero))
                signatures = new NewSignature(rst.signatures);
        }
    }
}
