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
    public class Signature: IEnumerable<Signature>
    {
    	private Signature next;
    	public Signature Next 
    	{
    		get { return next; }
    	}
    
        /* A summary of the signature status.  */
        private SignatureSummary summary;
        public SignatureSummary Summary
        {
            get { return summary; }
        }

        /* The fingerprint or key ID of the signature.  */
        private string fpr;
        public string Fingerprint
        {
            get { return fpr; }
        }

        /* The status of the signature.  */
        private uint status; //gpgme_error_t
        public long Status
        {
            get { return (long)status; }
        }

        /* Notation data and policy URLs.  */
        private SignatureNotation notations; // gpgme_sig_notation_t
        public SignatureNotation Notations
        {
            get { return notations; }
        }

        /* Signature creation time.  */
        private UIntPtr timestamp;

        /* Signature exipration time or 0.  */
        private UIntPtr exp_timestamp;

        private Validity validity;
        public Validity Validity
        {
            get { return validity; }
        }

        private uint validity_reason; //gpgme_error_t
        public long ValidityReason
        {
            get { return (long)validity_reason; }
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

        /* The mailbox from the PKA information or NULL. */
        private string pka_address; // char *
        public string PKAAddress
        {
            get { return pka_address; }
        }

        private bool wrong_key_usage;
        public bool WrongKeyUsage
        {
            get { return wrong_key_usage; }
        }
        
        private PkaStatus pka_trust;
        public PkaStatus PKATrust
        {
            get { return pka_trust; }
        }
        
        private bool chain_model;
        public bool ChainModel
        {
            get { return chain_model; }
        }

        internal Signature(IntPtr sigPtr)
        {
            if (sigPtr == IntPtr.Zero)
                throw new InvalidPtrException("An invalid signature pointer has been given.");

            UpdateFromMem(sigPtr);
        }
        
        private void UpdateFromMem(IntPtr sigPtr)
        {
            /* Work around memory layout problem (bug?) on Windows systems
             * with libgpgme <= 1.1.8
               //_gpgme_signature sig = new _gpgme_signature();
             * 
             */
            if (!libgpgme.IsWindows ||
                (Gpgme.Version.Major >= 1 && 
                 Gpgme.Version.Minor >= 2)
                )
            {
                _gpgme_signature unixsig = new _gpgme_signature();
                Marshal.PtrToStructure(sigPtr, unixsig);

                summary = (SignatureSummary)unixsig.summary;
                fpr = Gpgme.PtrToStringUTF8(unixsig.fpr);
                status = unixsig.status;
                timestamp = unixsig.timestamp;
                exp_timestamp = unixsig.exp_timestamp;
                validity = (Validity)unixsig.validity;
                validity_reason = unixsig.validity_reason;
                pubkey_algo = (KeyAlgorithm)unixsig.pubkey_algo;
                hash_algo = (HashAlgorithm)unixsig.hash_algo;
                pka_address = Gpgme.PtrToStringUTF8(unixsig.pka_address);
                wrong_key_usage = unixsig.wrong_key_usage;
                pka_trust = (PkaStatus)unixsig.pka_trust;
                chain_model = unixsig.chain_model;

                if (unixsig.notations != IntPtr.Zero)
                    notations = new SignatureNotation(unixsig.notations);
                    
                if (unixsig.next != IntPtr.Zero)
                	next = new Signature(unixsig.next);
                    
            }
            else
            {
                _gpgme_signature_windows winsig = new _gpgme_signature_windows();
                Marshal.PtrToStructure(sigPtr, winsig);

                summary = (SignatureSummary)winsig.summary;
                fpr = Gpgme.PtrToStringUTF8(winsig.fpr);
                status = winsig.status;
                timestamp = winsig.timestamp;
                exp_timestamp = winsig.exp_timestamp;
                validity = (Validity)winsig.validity;
                validity_reason = winsig.validity_reason;
                pubkey_algo = (KeyAlgorithm)winsig.pubkey_algo;
                hash_algo = (HashAlgorithm)winsig.hash_algo;
                pka_address = Gpgme.PtrToStringUTF8(winsig.pka_address);
                wrong_key_usage = winsig.wrong_key_usage;
                pka_trust = (PkaStatus)winsig.pka_trust;
                chain_model = winsig.chain_model;

                if (winsig.notations != IntPtr.Zero)
                    notations = new SignatureNotation(winsig.notations);
                    
                if (winsig.next != IntPtr.Zero)
                	next = new Signature(winsig.next);
            }
        }

        public DateTime Timestamp
        {
            get 
            {
	           	return Gpgme.ConvertFromUnix((long)timestamp); 
            }
        }
        public DateTime TimestampUTC
        {
            get 
            {
            	return Gpgme.ConvertFromUnixUTC((long)timestamp); 
            }
        }
        public DateTime ExpTimestamp
        {
            get { return Gpgme.ConvertFromUnix((long)exp_timestamp); }
        }
        public DateTime ExpTimestampUTC
        {
            get { return Gpgme.ConvertFromUnixUTC((long)exp_timestamp); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public IEnumerator<Signature> GetEnumerator()
        {
            Signature sig = this;
            while (sig != null)
            {
                yield return sig;
                sig = sig.Next;
            }
        }
    }
}
