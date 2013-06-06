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
    public class KeyStore : IKeyStore, IKeyGenerator
    {
        private Context ctx = null;

        public KeyStore(Context ctx)
        {
            this.ctx = ctx;
        }

        public ImportResult Import(GpgmeData keydata)
        {
            if (ctx == null 
                || !(ctx.IsValid))
                throw new InvalidContextException();

            if (keydata == null)
                throw new ArgumentNullException("An invalid data buffer has been specified.",
                    new InvalidDataBufferException());

            if (!(keydata.IsValid))
                throw new InvalidDataBufferException();

            lock (ctx.CtxLock)
            {
                int err = libgpgme.gpgme_op_import(ctx.CtxPtr, keydata.dataPtr);
                gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

                if (errcode != gpg_err_code_t.GPG_ERR_NO_ERROR)
                    throw new KeyImportException("Error " + errcode, err);

                IntPtr result = libgpgme.gpgme_op_import_result(ctx.CtxPtr);
                return new ImportResult(result);
            }
        }

        public void Export(string pattern, GpgmeData keydata)
        {
            Export(new string[] { pattern }, keydata);
        }

        public void Export(string[] pattern, GpgmeData keydata)
        {
            if (ctx == null ||
                !(ctx.IsValid))
                throw new InvalidContextException();

            if (keydata == null)
                throw new ArgumentNullException("Invalid data buffer",
                    new InvalidDataBufferException());
            
            if (!(keydata.IsValid))
                throw new InvalidDataBufferException();

            IntPtr[] parray = null;
            if (pattern != null)
            {
                parray = Gpgme.StringToCoTaskMemUTF8(pattern);
            }

            int err;
            uint reserved = 0;

            lock (ctx.CtxLock)
            {
                if (parray != null)
                {
                    err = libgpgme.gpgme_op_export_ext(
                        ctx.CtxPtr,
                        parray,
                        reserved,
                        keydata.dataPtr);
                }
                else
                {
                    err = libgpgme.gpgme_op_export(
                        ctx.CtxPtr,
                        IntPtr.Zero,
                        reserved,
                        keydata.dataPtr);
                }
            }

            GC.KeepAlive(keydata);

            // Free memory 
            Gpgme.FreeStringArray(parray);

            gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

            if (errcode != gpg_err_code_t.GPG_ERR_NO_ERROR)
                throw new KeyExportException("Error " + errcode, err);
        }

        public Key GetKey(string fpr, bool secretOnly)
        {
            if (ctx == null ||
                !(ctx.IsValid))
                throw new InvalidContextException();

            if (fpr == null || fpr.Equals(string.Empty))
                throw new InvalidKeyFprException();

            int secret = secretOnly ? 1 : 0;
            IntPtr rkeyPtr = (IntPtr)0;

            lock (ctx.CtxLock)
            {
                // no deadlock because the query is made by the same thread
                Protocol proto = ctx.Protocol;

                gpg_err_code_t errcode = GetKey(fpr, secret, out rkeyPtr);

                if (errcode == gpg_err_code_t.GPG_ERR_NO_ERROR &&
                    !(rkeyPtr.Equals((IntPtr)0)))
                {
                    Key key = null;

                    if (proto == Protocol.OpenPGP)
                        key = new PgpKey(rkeyPtr);
                    else if (proto == Protocol.CMS)
                        key = new X509Key(rkeyPtr);
                    else
                        key = new Key(rkeyPtr);
                    
                    //libgpgme.gpgme_key_release(rkeyPtr);
                    return key;
                }
                else
                    throw new KeyNotFoundException("The key " + fpr + " could not be found in the keyring.");
            }
        }

        private gpg_err_code_t GetKey(string fpr, int secret, out IntPtr rkeyPtr)
        {
            // the fingerprint could be a UTF8 encoded name
            IntPtr fprPtr = Gpgme.StringToCoTaskMemUTF8(fpr);
            
            int err = libgpgme.gpgme_get_key(ctx.CtxPtr, fprPtr, out rkeyPtr, secret);

            // free memory
            if (fprPtr != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(fprPtr);
                fprPtr = IntPtr.Zero;
            }

            gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);

            switch (errcode)
            {
                case gpg_err_code_t.GPG_ERR_INV_VALUE:
                    throw new ArgumentException("Invalid key fingerprint has been given. Error: " + err.ToString());
                case gpg_err_code_t.GPG_ERR_AMBIGUOUS_NAME:
                    throw new AmbiguousKeyException("The key id was not unique. Error: " + err.ToString());
                case gpg_err_code_t.GPG_ERR_ENOMEM:
                    throw new OutOfMemoryException("Not enough memory available for this operation.");
                case gpg_err_code_t.GPG_ERR_EOF:
                    throw new KeyNotFoundException("The key " + fpr + " (secret=" + secret + ") could not be found in the keyring.");
            }
            return errcode;
        }

        public Key[] GetKeyList(string pattern, bool secretOnly)
        {
            return GetKeyList(new string[] { pattern }, secretOnly);
        }

        public Key[] GetKeyList(string[] pattern, bool secretOnly)
        {

            if (ctx == null ||
                !(ctx.IsValid))
                throw new InvalidContextException();

            List<Key> list = new List<Key>();

            int reserved = 0;
            int secret_only = 0;
            if (secretOnly) secret_only = 1;

            IntPtr[] parray = null;
            if (pattern != null)
                parray = Gpgme.StringToCoTaskMemUTF8(pattern);

            lock (ctx.CtxLock)
            {
                // no deadlock because the query is made by the same thread
                Protocol proto = ctx.Protocol;

                int err = 0;

                if (parray != null)
                    err = libgpgme.gpgme_op_keylist_ext_start(
                        ctx.CtxPtr,
                        parray,
                        secret_only,
                        reserved);
                else
                    err = libgpgme.gpgme_op_keylist_start(
                        ctx.CtxPtr,
                        IntPtr.Zero,
                        secret_only);

                while (err == 0)
                {
                    IntPtr keyPtr = (IntPtr)0;
                    err = libgpgme.gpgme_op_keylist_next(ctx.CtxPtr, out keyPtr);
                    if (err != 0)
                        break;

                    Key key = null;

                    if (proto == Protocol.OpenPGP)
                        key = new PgpKey(keyPtr);
                    else if (proto == Protocol.CMS)
                        key = new X509Key(keyPtr);
                    else
                        key = new Key(keyPtr);

                    list.Add(key);

                    //libgpgme.gpgme_key_release(keyPtr);
                }

                // Free memory 
                if (parray != null)
                    Gpgme.FreeStringArray(parray);

                gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);
                if (errcode != gpg_err_code_t.GPG_ERR_EOF)
                {
                    libgpgme.gpgme_op_keylist_end(ctx.CtxPtr);
                    throw new GpgmeException(Gpgme.GetStrError(err), err);
                }
            }
            return list.ToArray();
        }

        public GenkeyResult GenerateKey(Protocol protocoltype, KeyParameters keyparms)
        {
            if (!ctx.IsValid)
                throw new InvalidContextException();

            if (keyparms == null)
                throw new ArgumentNullException("No KeyParameters object supplied. Bad programmer! *spank* *spank*");

            if (keyparms.Email == null ||
                keyparms.Email.Equals(String.Empty))
                throw new ArgumentException("No email address has been supplied.");

            // Convert the key parameter to an XML string for GPGME
            string parms = keyparms.GetXmlText(protocoltype);

            // Convert key parameter XML string to UTF8 and retrieve the memory pointer
            IntPtr parmsPtr = Gpgme.StringToCoTaskMemUTF8(parms);

            GenkeyResult keyresult = null;
            int err = 0;
            gpg_err_code_t errcode = gpg_err_code_t.GPG_ERR_NO_ERROR;

            lock (ctx.CtxLock)
            {
                // Protocol specific key generation
                switch (protocoltype)
                {
                    case Protocol.OpenPGP:
                        err = libgpgme.gpgme_op_genkey(ctx.CtxPtr, parmsPtr, (IntPtr)0, (IntPtr)0);
                        errcode = libgpgme.gpgme_err_code(err);
                        if (errcode == gpg_err_code_t.GPG_ERR_NO_ERROR)
                        {
                            IntPtr resultPtr = libgpgme.gpgme_op_genkey_result(ctx.CtxPtr);
                            if (!resultPtr.Equals((IntPtr)0))
                            {
                                keyresult = new GenkeyResult(resultPtr);
                            }
                            else
                                errcode = gpg_err_code_t.GPG_ERR_GENERAL;
                        }
                        break;
                    default:
                        // free memory
                        if (parmsPtr != IntPtr.Zero)
                        {
                            Marshal.FreeCoTaskMem(parmsPtr);
                            parmsPtr = IntPtr.Zero;
                        }
                        throw new NotSupportedException("The protocol " + protocoltype + " is currently not supported for key generation.");
                }
            }

            // free memory
            if (parmsPtr != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(parmsPtr);
                parmsPtr = IntPtr.Zero;
            }

            if (errcode == gpg_err_code_t.GPG_ERR_INV_VALUE)
                throw new ArgumentException("The key parameters are invalid.");
            if (errcode == gpg_err_code_t.GPG_ERR_NOT_SUPPORTED)
                throw new NotSupportedException("The PUBLIC or SECRET part is invalid. Error: " + err.ToString());
            if (errcode != gpg_err_code_t.GPG_ERR_NO_ERROR)
                throw new GeneralErrorException("No key has been created by the backend.");

            return keyresult;
        }
       
        public void DeleteKey(Key key, bool deleteSecret)
        {
            if (ctx == null ||
               !(ctx.IsValid))
                throw new InvalidContextException();

            if (key == null || key.KeyPtr.Equals(IntPtr.Zero))
                throw new InvalidKeyException("An invalid key has been supplied.");

            int secret = deleteSecret ? 1 : 0;

            lock (ctx.CtxLock)
            {
                int err = libgpgme.gpgme_op_delete(ctx.CtxPtr, key.KeyPtr, secret);
                
                gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);

                switch (errcode)
                {
                    case gpg_err_code_t.GPG_ERR_NO_ERROR:
                        break;
                    case gpg_err_code_t.GPG_ERR_NO_PUBKEY:
                        throw new KeyNotFoundException("The public key could not be found.");
                    case gpg_err_code_t.GPG_ERR_CONFLICT:
                        throw new KeyConflictException("Cannot delete the public key without deleting the secret key as well.");
                    case gpg_err_code_t.GPG_ERR_INV_VALUE:
                        throw new InvalidPtrException("Either the context (ctx) or the key parameter was invalid.");
                    case gpg_err_code_t.GPG_ERR_AMBIGUOUS_NAME:
                        throw new AmbiguousKeyException("The key id was not unique.");
                }
            }
        }

        public TrustItem[] GetTrustList(string pattern, int maxlevel)
        {
            if (ctx == null ||
                !(ctx.IsValid))
                throw new InvalidContextException();

            if (pattern == null || pattern.Equals(String.Empty))
                throw new ArgumentException("An invalid pattern has been specified.");

            IntPtr patternPtr = Gpgme.StringToCoTaskMemUTF8(pattern);

            List<TrustItem> lst = new List<TrustItem>();

            lock (ctx.CtxLock)
            {
                int err = libgpgme.gpgme_op_trustlist_start(ctx.CtxPtr, patternPtr, maxlevel);
                gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

                if (errcode != gpg_err_code_t.GPG_ERR_NO_ERROR)
                {
                    if (patternPtr != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(patternPtr);
                        patternPtr = IntPtr.Zero;
                    }
                    throw new GeneralErrorException("An unexpected error occurred. Error: " + err.ToString());
                }
                
                while (errcode == gpg_err_code_t.GPG_ERR_NO_ERROR)
                {
                    IntPtr itemPtr = IntPtr.Zero;
                    err = libgpgme.gpgme_op_trustlist_next(ctx.CtxPtr, out itemPtr);
                    errcode = libgpgerror.gpg_err_code(err);

                    if (errcode == gpg_err_code_t.GPG_ERR_NO_ERROR)
                    {
                        lst.Add(new TrustItem(itemPtr));
                    }
                }
                // Release context if there are any pending trustlist items
                err = libgpgme.gpgme_op_trustlist_end(ctx.CtxPtr);

                if (patternPtr != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(patternPtr);
                    patternPtr = IntPtr.Zero;
                }
				
                if (errcode != gpg_err_code_t.GPG_ERR_EOF)
                    throw new GeneralErrorException("An unexpected error occurred. " + errcode.ToString());

            }

            if (lst.Count == 0)
                return new TrustItem[0];
            else
                return lst.ToArray();
        }
        public Context Context 
        {
        	get { return ctx; }
        }
    }
}
