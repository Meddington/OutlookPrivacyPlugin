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
using System.IO;

using Libgpgme.Interop;

namespace Libgpgme
{
	public class Key: IDisposable 
	{
        // Key attributes
		private KeylistMode keylistmode;
		private bool revoked, expired, disabled, invalid, can_encrypt, can_sign, 
		    can_certify, can_authenticate, is_qualified, secret;
		private Protocol protocol;
		private string issuer_serial, issuer_name, chain_id;
		private Validity owner_trust;
		private Subkey subkeys;
		private UserId uids;
        private IntPtr keyPtr = IntPtr.Zero;

        // callback function for keyediting
        private gpgme_edit_cb_t _instance_key_edit_callback;

        // lock they PGP key during key edit operations
        protected object editlock = new object();
        public Exception LastCallbackException;

        ~Key()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            if (keyPtr != IntPtr.Zero)
            {
                libgpgme.gpgme_key_release(keyPtr);
                keyPtr = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                CleanUp();
        }

		internal Key(IntPtr keyPtr) {
			if (keyPtr == IntPtr.Zero)
				throw new InvalidPtrException("An invalid Key pointer has been given." +
				                              " Bad programmer! *spank* *spank*");
			UpdateFromMem(keyPtr);

		}		
		internal void UpdateFromMem(IntPtr keyPtr) {
            _gpgme_key key = (_gpgme_key)
                Marshal.PtrToStructure(keyPtr,
                    typeof(_gpgme_key));

            this.keyPtr = keyPtr;

			revoked = key.revoked;
			expired = key.expired;
			disabled = key.disabled;
			invalid = key.invalid;
			can_encrypt = key.can_encrypt;
			can_sign = key.can_sign;
			can_certify = key.can_certify;
			can_authenticate = key.can_authenticate;
			is_qualified = key.is_qualified;
			secret = key.secret;
			
			protocol = (Protocol)key.protocol;
			owner_trust = (Validity)key.owner_trust;
			keylistmode = (KeylistMode)key.keylist_mode;
			
            issuer_name = Gpgme.PtrToStringUTF8(key.issuer_name);
			issuer_serial = Gpgme.PtrToStringAnsi(key.issuer_serial);
			chain_id = Gpgme.PtrToStringAnsi(key.chain_id);

            if (key.subkeys != (IntPtr)0)
                subkeys = new Subkey(key.subkeys);

            if (key.uids != (IntPtr)0)
                uids = new UserId(key.uids);
		}

        protected int StartEdit(Context ctx, IntPtr handle, GpgmeData data)
        {
            if (KeyPtr == IntPtr.Zero)
                throw new InvalidKeyException();

            if (ctx == null || !(ctx.IsValid))
                throw new InvalidContextException();

            if (data == null || !(data.IsValid))
                throw new InvalidDataBufferException();

            lock (editlock)
            {
                LastCallbackException = null;

                // set the instance's _edit_cb() method as callback function for libgpgme
                _instance_key_edit_callback = new gpgme_edit_cb_t(_edit_cb);

                // start key editing
                int err = libgpgme.gpgme_op_edit(
                    ctx.CtxPtr,
                    KeyPtr,
                    _instance_key_edit_callback,
                    handle,
                    data.dataPtr);

				GC.KeepAlive(_instance_key_edit_callback);
				
                if (LastCallbackException != null)
                    throw LastCallbackException;

                return err;
            }
        }

        // internal callback function 
        private int _edit_cb(
           IntPtr opaque,  
           int status,     
           IntPtr args,    
           int fd)
        {
            gpgme_status_code_t statuscode = (gpgme_status_code_t)status;
            string cmdargs = Gpgme.PtrToStringUTF8(args);
            Stream fdstream;

            if (fd > 0)
                fdstream = Gpgme.ConvertToStream(fd, FileAccess.Write);
            else
                fdstream = null;
            
            int result = 0;
            try
            {
                // call user callback function.
                result = KeyEditCallback(
                    opaque,
                    (KeyEditStatusCode)statuscode,
                    cmdargs,
                    fdstream);
            }
            catch (Exception ex)
            {
                LastCallbackException = ex;
            }

            return result;
        }

        protected virtual int KeyEditCallback(IntPtr handle, KeyEditStatusCode status, string args, Stream fd) {
            if (fd != null)
            {
                fd.Write(new byte[] { (byte)'q', (byte)'u', (byte)'i', (byte)'t', (byte)'\n' }, 0, 5);
                fd.Flush();
            }
            throw new NotImplementedException("The function KeyEditCallback is not implemented.");
        }

		public KeylistMode KeylistMode {
			get { return keylistmode; }
		}
		public bool Revoked {
			get { return revoked; }
		}
		public bool Expired {
			get { return expired; }
		}
		public bool Disabled {
			get { return disabled; }
		}
		public bool Invalid {
			get { return invalid; }
		}
		public bool CanEncrypt {
			get { return can_encrypt; }
		}
		public bool CanSign {
			get { return can_sign; }
		}
		public bool CanCertify {
			get { return can_certify; }
		}
		public bool CanAuthenticate {
			get { return can_authenticate; }
		}
		public bool IsQualified {
			get { return is_qualified; }
		}
		public bool Secret {
			get { return secret; }
		}
		public Protocol Protocol {
			get { return protocol; }
		}
		public string IssuerSerial {
			get { return issuer_serial; }
		}
		public string IssuerName {
			get { return issuer_name; }
		}
		public string ChainId {
			get { return chain_id; }
		}
		public Validity OwnerTrust {
			get { return owner_trust; }
		}

        public UserId Uids
        {
            get { return uids; }
        }
        // The first one in the list is the main/primary UserID
        public UserId Uid
        {
            get
            {
                if (uids != null)
                    return uids;
                else
                    return null;
            }
        }

        public Subkey Subkeys {
			get { return subkeys; }
		}
        // The first subkey in the linked list is the primary key
        public string KeyId
        {
            get
            {
                if (subkeys != null)
                    return subkeys.KeyId;
                else
                    return null;
            }
        }
        // The first subkey in the linked list is the primary key
        public string Fingerprint
        {
            get
            {
                if (subkeys != null)
                    return subkeys.Fingerprint;
                else
                    return null;
            }
        }
        internal virtual IntPtr KeyPtr
        {
            get { return keyPtr; }
        }
	}
}
