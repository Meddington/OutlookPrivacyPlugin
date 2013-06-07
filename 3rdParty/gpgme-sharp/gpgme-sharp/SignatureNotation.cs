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
    public class SignatureNotation: IEnumerable<SignatureNotation>
    {
        private string _name, _value;
        private bool critical, human_readable;
        private SignatureNotationFlags flags;
        private SignatureNotation next = null;

        internal SignatureNotation(IntPtr signotPtr)
        {

            if (signotPtr == (IntPtr)0)
                throw new InvalidPtrException("The signature notation pointer is invalid. Bad programmer! *spank* *spank*");

            UpdateFromMem(signotPtr);
        }

        private void UpdateFromMem(IntPtr signotPtr)
        {
            int len;
            _gpgme_sig_notation signot = (_gpgme_sig_notation)Marshal.PtrToStructure(signotPtr,
                typeof(_gpgme_sig_notation));

            if (signot.value != (IntPtr)0)
            {
                len = signot.value_len;
                _value = Gpgme.PtrToStringUTF8(signot.value,
                    len);
            }
            if (signot.name != (IntPtr)0)
            {
                len = signot.name_len;
                _name = Gpgme.PtrToStringUTF8(signot.name,
                    len);
            }

            flags = (SignatureNotationFlags)signot.flags;
            critical = signot.critical;
            human_readable = signot.human_readable;

            if (signot.next != (IntPtr)0)
                next = new SignatureNotation(signot.next);
        }

        public SignatureNotation Next
        {
            get { return next; }
        }
        public bool Critical
        {
            get { return critical; }
        }
        public bool HumanReadable
        {
            get { return human_readable; }
        }
        public string Value
        {
            get { return _value; }
        }
        public string Name
        {
            get { return _name; }
        }
        public SignatureNotationFlags Flags
        {
            get { return flags; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public IEnumerator<SignatureNotation> GetEnumerator()
        {
            SignatureNotation signot = this;
            while (signot != null)
            {
                yield return signot;
                signot = signot.Next;
            }
        }
    }
}
