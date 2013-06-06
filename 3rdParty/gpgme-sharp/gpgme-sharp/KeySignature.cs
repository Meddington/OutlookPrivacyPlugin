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
    public class KeySignature: IEnumerable<KeySignature>
    {
        private KeySignature next;
        private bool revoked, expired, invalid, exportable;
        private KeyAlgorithm pubkey_algo;
        private string keyid, uid, name, comment, email;
        private long timestamp, expires;
        private int status;
        private long sig_class;
        private SignatureNotation notations;

        internal KeySignature(IntPtr keysigPtr)
        {
            if (keysigPtr == (IntPtr)0)
                throw new InvalidPtrException("Invalid key signature pointer. Bad programmer! *spank* *spank*");

            UpdateFromMem(keysigPtr);
        }

        private void UpdateFromMem(IntPtr keysigPtr)
        {
            _gpgme_key_sig keysig = (_gpgme_key_sig)Marshal.PtrToStructure(keysigPtr,
                typeof(_gpgme_key_sig));

            revoked = keysig.revoked;
            expired = keysig.expired;
            invalid = keysig.invalid;
            exportable = keysig.exportable;

            pubkey_algo = (KeyAlgorithm)keysig.pubkey_algo;

            keyid = Gpgme.PtrToStringAnsi(keysig.keyid);
            uid = Gpgme.PtrToStringAnsi(keysig.uid);
            name = Gpgme.PtrToStringAnsi(keysig.name);
            comment = Gpgme.PtrToStringAnsi(keysig.comment);
            email = Gpgme.PtrToStringAnsi(keysig.email);

            timestamp = (long)keysig.timestamp;
            expires = (long)keysig.expires;

            status = keysig.status;
            sig_class = (long)keysig.sig_class;

            if (keysig.notations != (IntPtr)0)
                notations = new SignatureNotation(keysig.notations);
            
            if (keysig.next != (IntPtr)0)
                next = new KeySignature(keysig.next);
        }

        public bool Revoked
        {
            get { return revoked; }
        }
        public bool Expired
        {
            get { return expired; }
        }
        public bool Invalid
        {
            get { return invalid; }
        }
        public bool Exportable
        {
            get { return exportable; }
        }

        public KeyAlgorithm PubkeyAlgorithm
        {
            get { return pubkey_algo; }
        }

        public string KeyId
        {
            get { return keyid; }
        }

        public DateTime Timestamp
        {
            get { return Gpgme.ConvertFromUnix(timestamp); }
        }
        public DateTime TimestampUTC
        {
            get { return Gpgme.ConvertFromUnixUTC(timestamp); }
        }

        public DateTime Expires
        {
            get { return Gpgme.ConvertFromUnix(expires); }
        }
        public DateTime ExpiresUTC
        {
            get { return Gpgme.ConvertFromUnixUTC(expires); }
        }

        public int Status
        {
            get { return status; }
        }

        public long SigClass
        {
            get { return sig_class; }
        }

        public string Uid
        {
            get { return uid; }
        }
        public string Name
        {
            get { return name; }
        }
        public string Comment
        {
            get { return comment; }
        }
        public string Email
        {
            get { return email; }
        }

        public SignatureNotation Notations
        {
            get { return notations; }
        }

        public KeySignature Next
        {
            get { return next; }
        }

        public bool IsInfinitely
        {
            get { return expires == 0; }
        }

        public IEnumerator<KeySignature> GetEnumerator()
        {
            KeySignature keysig = this;
            while (keysig != null)
            {
                yield return keysig;
                keysig = keysig.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
