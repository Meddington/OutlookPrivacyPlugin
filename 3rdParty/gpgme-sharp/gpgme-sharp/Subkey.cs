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
using System.Runtime.InteropServices;
using System.Threading;
using Libgpgme.Interop;

namespace Libgpgme
{
    public class Subkey: IEnumerable<Subkey>
    {
        private Subkey next;
        private bool revoked, expired, disabled, invalid, can_encrypt,
            can_sign, can_certify, can_authenticate, is_qualified,
            secret;
        private KeyAlgorithm pubkey_algo;
        private long length;
        string keyid, fpr;
        private long timestamp, expires;

        internal Subkey(IntPtr subkeyPtr)
        {
            if (subkeyPtr == (IntPtr)0)
                throw new InvalidPtrException("Invalid subkey pointer. Bad programmer! *spank* *spank*");

            UpdateFromMem(subkeyPtr);
        }

        private void UpdateFromMem(IntPtr subkeyPtr)
        {
            _gpgme_subkey subkey = (_gpgme_subkey)
                Marshal.PtrToStructure(subkeyPtr, 
                    typeof(_gpgme_subkey));

            revoked = subkey.revoked;
            expired = subkey.expired;
            disabled = subkey.disabled;
            invalid = subkey.invalid;
            can_encrypt = subkey.can_encrypt;
            can_sign = subkey.can_sign;
            can_certify = subkey.can_certify;
            can_authenticate = subkey.can_authenticate;
            is_qualified = subkey.is_qualified;
            secret = subkey.secret;

            pubkey_algo = (KeyAlgorithm)subkey.pubkey_algo;
            length = (long)subkey.length;

            keyid = Gpgme.PtrToStringAnsi(subkey.keyid);
            fpr = Gpgme.PtrToStringAnsi(subkey.fpr);
            timestamp = (long)subkey.timestamp;
            expires = (long)subkey.expires;

            if (subkey.next != (IntPtr)0)
                next = new Subkey(subkey.next);
        }

        public KeyAlgorithm PubkeyAlgorithm
        {
            get { return pubkey_algo; }
        }

        public long Length
        {
            get { return length; }
        }

        public DateTime Timestamp
        {
            get 
            {
            	if (timestamp < 0)
            		throw new InvalidTimestampException();
            	if (timestamp == 0)
            		throw new TimestampNotAvailableException();
            	return Gpgme.ConvertFromUnix(timestamp); 
            }
        }
        public DateTime TimestampUTC
        {
            get 
            {
               	if (timestamp < 0)
            		throw new InvalidTimestampException();
            	if (timestamp == 0)
            		throw new TimestampNotAvailableException();
            	return Gpgme.ConvertFromUnixUTC(timestamp); 
            }
        }

        public DateTime Expires
        {
            get { return Gpgme.ConvertFromUnix(expires); }
        }
        public DateTime ExpiresUTC
        {
            get { return Gpgme.ConvertFromUnixUTC(expires); }
        }

        public string KeyId
        {
            get { return keyid; }
        }

        public string Fingerprint
        {
            get { return fpr; }
        }

        public Subkey Next
        {
            get { return next; }
        }
        public bool Revoked
        {
            get { return revoked; }
        }
        public bool Expired
        {
            get { return expired; }
        }
        public bool Disabled
        {
            get { return disabled; }
        }
        public bool Invalid
        {
            get { return invalid; }
        }
        public bool CanEncrypt
        {
            get { return can_encrypt; }
        }
        public bool CanSign
        {
            get { return can_sign; }
        }
        public bool CanCertify
        {
            get { return can_certify; }
        }
        public bool Secret
        {
            get { return secret; }
        }
        public bool CanAuthenticate
        {
            get { return can_authenticate; }
        }
        public bool IsQualified
        {
            get { return is_qualified; }
        }
        public bool IsInfinitely
        {
            get { return expires == 0; }
        }

        public IEnumerator<Subkey> GetEnumerator()
        {
            //return new SubkeyEnumerator(this);
            Subkey key = this;
            while (key != null)
            {
                yield return key;
                key = key.Next;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


    }
}
