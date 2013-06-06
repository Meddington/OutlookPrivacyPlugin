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
    public class Recipient: IEnumerable<Recipient>
    {
        private string keyid;
        public string KeyId
        {
            get { return keyid; }
        }
        private KeyAlgorithm pubkey_algo;
        public KeyAlgorithm KeyAlgorithm
        {
            get { return pubkey_algo; }
        }
        private int status;
        public int Status
        {
            get { return status; }
        }
        private Recipient next;
        public Recipient Next
        {
            get { return next; }
        }

        internal Recipient(IntPtr recpPtr)
        {
            if (recpPtr == IntPtr.Zero)
                throw new InvalidPtrException("An invalid recipient pointer has been given.");
            
            UpdateFromMem(recpPtr);
        }
        private void UpdateFromMem(IntPtr recpPtr)
        {
            _gpgme_recipient recp = new _gpgme_recipient();
            Marshal.PtrToStructure(recpPtr, recp);

            keyid = Gpgme.PtrToStringUTF8(recp.keyid);
            pubkey_algo = (KeyAlgorithm)recp.pubkey_algo;
            status = recp.status;

            if (recp.next != IntPtr.Zero)
                next = new Recipient(recp.next);
        }

        public IEnumerator<Recipient> GetEnumerator()
        {
            Recipient recp = this;
            while (recp != null)
            {
                yield return recp;
                recp = recp.Next;
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
