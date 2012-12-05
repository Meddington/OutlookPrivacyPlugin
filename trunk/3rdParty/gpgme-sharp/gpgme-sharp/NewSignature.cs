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
    public class NewSignature: IEnumerable<NewSignature>
    {
        private NewSignature next;
        public NewSignature Next
        {
            get { return next; }
        }
        /* The type of the signature.  */
        private SignatureMode type;
        public SignatureMode Type
        {
            get { return type; }
        }
        /* The public key algorithm used to create the signature.  */
        private KeyAlgorithm pubkey_algo;
        public KeyAlgorithm PubkeyAlgorithm
        {
            get { return pubkey_algo; }
        }
        /* The hash algorithm used to create the signature.  */
        private HashAlgorithm hash_algo;
        public HashAlgorithm HashAlgorithm
        {
            get { return hash_algo; }
        }
        private long timestamp; // long int
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

        /* The fingerprint of the signature.  */
        private string fpr; //char *
        public string Fingerprint
        {
            get { return fpr; }
        }

        /* Crypto backend specific signature class.  */
        private long sig_class;
        public long SignatureClass
        {
            get { return sig_class; }
        }

        internal NewSignature(IntPtr sigPtr)
        {
            if (sigPtr.Equals(IntPtr.Zero))
                throw new InvalidPtrException("The pointer to the new signature structure is invalid.");

            UpdateFromMem(sigPtr);
        }
        private void UpdateFromMem(IntPtr sigPtr)
        {
            _gpgme_new_signature newsig = new _gpgme_new_signature();
            Marshal.PtrToStructure(sigPtr, newsig);

            type = (SignatureMode)newsig.type;
            pubkey_algo = (KeyAlgorithm)newsig.pubkey_algo;
            hash_algo = (HashAlgorithm)newsig.hash_algo;
            fpr = Gpgme.PtrToStringUTF8(newsig.fpr);
            sig_class = (long)newsig.sig_class;
            timestamp = (long)newsig.timestamp;

            if (!newsig.next.Equals(IntPtr.Zero))
                next = new NewSignature(newsig.next);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<NewSignature> GetEnumerator()
        {
            NewSignature sig = this;
            while (sig != null)
            {
                yield return sig;
                sig = sig.Next;
            }
        }
    }
}
