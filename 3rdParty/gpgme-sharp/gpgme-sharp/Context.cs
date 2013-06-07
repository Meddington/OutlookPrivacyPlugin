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
using System.Security;
using System.IO;

using Libgpgme.Interop;

namespace Libgpgme
{
    public class Context : IDisposable
    {
        public const int CERTIFICATES_NO = 0;
        public const int CERTIFICATES_ALL = -1;
        public const int CERTIFICATES_ALL_EXCEPT_ROOT = -2;
        public const int CERTIFICATES_SENDER_ONLY = 1;
        public const int CERTIFICATES_DEFAULT = -256;

        private IntPtr ctxPtr = IntPtr.Zero;
        private object ctxLock = new object();
        
        private gpgme_passphrase_cb_t passphrase_cb = null;
        private PassphraseDelegate passphraseFunc = null;

        private KeyStore keystore = null;
        public Exception LastCallbackException;

        static Context()
        {
            // We need to call gpgme_check_version at least once!
            try
            {
                Gpgme.CheckVersion();
            }
            catch { };
        }

        public Context()
        {
            IntPtr ptr;
            int err;

            err = libgpgme.gpgme_new(out ptr);
            gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);

            switch (errcode)
            {
                case gpg_err_code_t.GPG_ERR_NO_ERROR:
                    this.ctxPtr = ptr;
                    this.signers = new ContextSigners(this);
                    this.signots = new ContextSignatureNotations(this);
                    this.keystore = new KeyStore(this);
                    break;
                case gpg_err_code_t.GPG_ERR_INV_VALUE:
                    throw new InvalidPtrException("CTX is not a valid pointer.\nBad programmer *spank* *spank*");
                case gpg_err_code_t.GPG_ERR_ENOMEM:
                    throw new OutOfMemoryException();
                default:
                    throw new GeneralErrorException("An unexpected error occurred during context creation. " + errcode.ToString());
            }

        }

        ~Context()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (ctxPtr != IntPtr.Zero)
            {
                libgpgme.gpgme_release(ctxPtr);
                ctxPtr = IntPtr.Zero;
            }
        }
        public bool IsValid
        {
            get { return (ctxPtr != IntPtr.Zero); }
        }
        internal IntPtr CtxPtr
        {
            get { return ctxPtr; }
        }
        internal object CtxLock
        {
            get { return ctxLock; }
        }

        public Protocol Protocol
        {
            get
            {
                if (IsValid)
                {
                    lock (CtxLock)
                    {
                        gpgme_protocol_t proto = libgpgme.gpgme_get_protocol(CtxPtr);
                        return (Protocol)proto;
                    }
                }
                else
                    throw new InvalidContextException();
            }
            set
            {
                if (IsValid)
                {
                    lock (CtxLock)
                    {
                        gpgme_protocol_t proto = (gpgme_protocol_t)value;
                        int err = libgpgme.gpgme_set_protocol(CtxPtr, proto);

                        gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);
                        switch (errcode)
                        {
                            case gpg_err_code_t.GPG_ERR_NO_ERROR:
                                //UpdateFromMem();
                                break;
                            case gpg_err_code_t.GPG_ERR_INV_VALUE:
                                throw new InvalidProtocolException("The protocol "
                                    + value.ToString()
                                    + " is not supported by any installed GnuPG engine.");
                        }
                    }
                }
                else
                    throw new InvalidContextException();
            }
        }
        public EngineInfo EngineInfo
        {
            get
            {
                if (IsValid)
                {
                    lock (CtxLock)
                    {
                        IntPtr enginePtr = libgpgme.gpgme_ctx_get_engine_info(CtxPtr);
                        if (enginePtr != (IntPtr)0)
                            return new EngineInfo(this, enginePtr);
                        else
                            return null;
                    }
                }
                else
                    throw new InvalidContextException();
            }
        }

        public void SetEngineInfo(Protocol proto, string filename, string homedir)
        {
            if (IsValid)
            {
                lock (CtxLock)
                {
                    IntPtr filenamePtr = IntPtr.Zero, homedirPtr = IntPtr.Zero;

                    if (filename != null)
                        filenamePtr = Marshal.StringToCoTaskMemAnsi(filename);
                    if (homedir != null)
                        homedirPtr = Marshal.StringToCoTaskMemAnsi(homedir);

                    int err = libgpgme.gpgme_ctx_set_engine_info(
                        CtxPtr,
                        (gpgme_protocol_t)proto,
                        filenamePtr,
                        homedirPtr);

                    if (filenamePtr != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(filenamePtr);
                        filenamePtr = IntPtr.Zero;
                    }
                    if (homedirPtr != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(homedirPtr);
                        homedirPtr = IntPtr.Zero;
                    }

                    gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);
                    if (errcode != gpg_err_code_t.GPG_ERR_NO_ERROR)
                    {
                        string errmsg = null;
                        try
                        {
                            Gpgme.GetStrError(err, out errmsg);
                        }
                        catch
                        {
                            errmsg = "No error message available.";
                        }
                        throw new ArgumentException(errmsg + " Error: " + err.ToString());
                    }
                }
            }
            else
                throw new InvalidContextException();
        }

        public bool Armor
        {
            get
            {
                if (IsValid)
                {
                    lock (CtxLock)
                    {
                        int yes = libgpgme.gpgme_get_armor(CtxPtr);
                        if (yes > 0)
                            return true;
                        else
                            return false;
                    }
                }
                else
                    throw new InvalidContextException();
            }
            set
            {
                if (IsValid)
                {
                    lock (CtxLock)
                    {
                        int yes = 0;
                        if (value)
                            yes = 1;
                        libgpgme.gpgme_set_armor(CtxPtr, yes);
                    }
                }
                else
                    throw new InvalidContextException();
            }
        }

        public bool TextMode
        {
            get
            {
                if (IsValid)
                {
                    lock (CtxLock)
                    {
                        int yes = libgpgme.gpgme_get_textmode(CtxPtr);
                        if (yes > 0)
                            return true;
                        else
                            return false;
                    }
                }
                else
                    throw new InvalidContextException();
            }
            set
            {
                if (IsValid)
                {
                    lock (CtxLock)
                    {
                        int yes = 0;
                        if (value)
                            yes = 1;
                        libgpgme.gpgme_set_textmode(CtxPtr, yes);
                    }
                }
                else
                    throw new InvalidContextException();

            }
        }

        public int IncludedCerts
        {
            get
            {
                if (IsValid)
                {
                    lock (CtxLock)
                    {
                        return libgpgme.gpgme_get_include_certs(CtxPtr);
                    }

                }
                else
                    throw new InvalidContextException();
            }
            set
            {
                if (IsValid)
                {
                    lock (CtxLock)
                    {
                        libgpgme.gpgme_set_include_certs(CtxPtr, value);
                    }
                }
                else
                    throw new InvalidContextException();
            }
        }

        public void AddKeylistMode(KeylistMode mode)
        {
            KeylistMode |= mode;
        }

        public void RemoveKeylistMode(KeylistMode mode)
        {
            KeylistMode tmp = mode;

            // Notations can only be retrieved with attached signatures
            if ((tmp & KeylistMode.Signatures) == KeylistMode.Signatures)
                tmp |= KeylistMode.SignatureNotations;
            
            KeylistMode &= ~(tmp);
        }

        public KeylistMode KeylistMode
        {
            get
            {
                if (IsValid)
                {
                    lock (CtxLock)
                    {
                        return (KeylistMode)libgpgme.gpgme_get_keylist_mode(CtxPtr);
                    }
                }
                else
                    throw new InvalidContextException();
            }
            set
            {
                if (IsValid)
                {
                    lock (CtxLock)
                    {
                        if ((value & KeylistMode.SignatureNotations)
                            == KeylistMode.SignatureNotations)
                            value |= KeylistMode.Signatures;

                        gpgme_keylist_mode_t mode = (gpgme_keylist_mode_t)value;

                        int err = libgpgme.gpgme_set_keylist_mode(CtxPtr, mode);
                        gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);

                        if (errcode != gpg_err_code_t.GPG_ERR_NO_ERROR)
                        {
                            string errmsg;
                            try
                            {
                                Gpgme.GetStrError(err, out errmsg);
                            }
                            catch
                            {
                                errmsg = "Unknown error.";
                            }
                            throw new ArgumentException(errmsg + " Error: " + err.ToString());
                        }
                    }
                }
                else
                    throw new InvalidContextException();
            }
        }

        private int _passphrase_cb(IntPtr Hook, IntPtr uid_hint, IntPtr passphrase_info,
            int prev_was_bad, int fd)
        {

            bool prevbad;
            string hint, info;
            char[] passwd = null;

            hint = Gpgme.PtrToStringUTF8(uid_hint);
            info = Gpgme.PtrToStringUTF8(passphrase_info);
            if (prev_was_bad > 0)
                prevbad = true;
            else
                prevbad = false;

            PassphraseResult result;

            try
            {
                PassphraseInfo pinfo = new PassphraseInfo(Hook, hint, info, prevbad);
                result = passphraseFunc(this, pinfo, ref passwd);
            }
            catch (Exception ex)
            {
                LastCallbackException = ex;
                passwd = "".ToCharArray();
                result = PassphraseResult.Canceled;
            }

            if (fd > 0)
            {
                byte[] utf8passwd = Gpgme.ConvertCharArrayToUTF8(passwd, 0);

                Stream fdstream = Gpgme.ConvertToStream(fd, FileAccess.Write);
                fdstream.Write(utf8passwd, 0, utf8passwd.Length);
                fdstream.Flush();
                fdstream.WriteByte((byte)'\n');
                fdstream.Flush();

                // try to wipe the passwords
                int i;
                for (i = 0; i < utf8passwd.Length; i++)
                    utf8passwd[i] = 0;
            }

            return (int)result;
        }

        public void SetPassphraseFunction(PassphraseDelegate func)
        {
            SetPassphraseFunction(func, IntPtr.Zero);
        }
        public void SetPassphraseFunction(PassphraseDelegate func, IntPtr Hook)
        {
            if (IsValid)
            {
                lock (CtxLock)
                {
                    if (passphraseFunc == null)
                    {
                        passphrase_cb = new gpgme_passphrase_cb_t(_passphrase_cb);
                        libgpgme.gpgme_set_passphrase_cb(CtxPtr, passphrase_cb, Hook);

                        passphraseFunc = func;
                    }
                    else
                        throw new GpgmeException("Passphrase function is already set.");
                }
            }
            else
                throw new InvalidContextException();
        }

        public bool HasPassphraseFunction
        {
            get { return (passphraseFunc != null); }
        }

        public void ClearPassphraseFunction()
        {
            if (IsValid)
            {
                lock (CtxLock)
                {
                    if (passphraseFunc != null)
                    {
                        libgpgme.gpgme_set_passphrase_cb(CtxPtr, null, (IntPtr)0);
                        passphraseFunc = null;
                        passphrase_cb = null;
                    }
                }
            }
            else
                throw new InvalidContextException();
        }

        public KeyStore KeyStore
        {
            get { return keystore; }
        }

        public EncryptionResult Encrypt(Key[] recipients, EncryptFlags flags, GpgmeData plain, GpgmeData cipher)
        {
            if (IsValid)
            {
                if (plain == null)
                    throw new ArgumentNullException("Source data buffer must be supplied.");
                if (!(plain.IsValid))
                    throw new InvalidDataBufferException("The specified source data buffer is invalid.");

                if (cipher == null)
                    throw new ArgumentNullException("Destination data buffer must be supplied.");
                if (!(cipher.IsValid))
                    throw new InvalidDataBufferException("The specified destination data buffer is invalid.");

                IntPtr[] recp = Gpgme.KeyArrayToIntPtrArray(recipients);

                lock (CtxLock)
                {
#if (VERBOSE_DEBUG)
					DebugOutput("gpgme_op_encrypt(..) START");
#endif			
                    int err = libgpgme.gpgme_op_encrypt(
                        CtxPtr,
                        recp,
                        (gpgme_encrypt_flags_t)flags,
                        plain.dataPtr,
                        cipher.dataPtr);

#if (VERBOSE_DEBUG)
					DebugOutput("gpgme_op_encrypt(..) DONE");
#endif

                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);
                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_UNUSABLE_PUBKEY:
                            break;
                        case gpg_err_code_t.GPG_ERR_GENERAL:    // Bug? should be GPG_ERR_UNUSABLE_PUBKEY
                            break;
                        case gpg_err_code_t.GPG_ERR_INV_VALUE:
                            throw new InvalidPtrException("Either the context, recipient key array, plain text or cipher text pointer is invalid.");
                        case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                            throw new BadPassphraseException();
                        case gpg_err_code_t.GPG_ERR_EBADF:
                            throw new InvalidDataBufferException("The source (plain) or destination (cipher) data buffer is invalid for encryption.");
                        default:
                            throw new GeneralErrorException("An unexpected error " 
                                + errcode.ToString() 
                                + " (" + err.ToString()
                                + ") occurred.");
                    }
                    IntPtr rstPtr = libgpgme.gpgme_op_encrypt_result(CtxPtr);
#if (VERBOSE_DEBUG)
					DebugOutput("gpgme_op_encrypt_result(..) DONE");
#endif			
					GC.KeepAlive(recp);
					GC.KeepAlive(recipients);
                    GC.KeepAlive(plain);
                    GC.KeepAlive(cipher);

					if (rstPtr != IntPtr.Zero)
                    {
                        EncryptionResult encRst = new EncryptionResult(rstPtr);
                        return encRst;
                    }
                    else
                        throw new GeneralErrorException("An unexpected error occurred. " + errcode.ToString());
                }
            }
            else
                throw new InvalidContextException();
        }

        public EncryptionResult EncryptAndSign(Key[] recipients, EncryptFlags flags, GpgmeData plain, GpgmeData cipher)
        {
            if (IsValid)
            {
                if (plain == null)
                    throw new ArgumentNullException("Source data buffer must be supplied.");
                if (!(plain.IsValid))
                    throw new InvalidDataBufferException("The specified source data buffer is invalid.");

                if (cipher == null)
                    throw new ArgumentNullException("Destination data buffer must be supplied.");
                if (!(cipher.IsValid))
                    throw new InvalidDataBufferException("The specified destination data buffer is invalid.");

                IntPtr[] recp = Gpgme.KeyArrayToIntPtrArray(recipients);

                lock (CtxLock)
                {
                    int err = libgpgme.gpgme_op_encrypt_sign(
                        CtxPtr,
                        recp,
                        (gpgme_encrypt_flags_t)flags,
                        plain.dataPtr,
                        cipher.dataPtr);

                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);
                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_UNUSABLE_PUBKEY:
                            break;
                        case gpg_err_code_t.GPG_ERR_GENERAL:    // Bug? should be GPG_ERR_UNUSABLE_PUBKEY
                            break;
                        case gpg_err_code_t.GPG_ERR_UNUSABLE_SECKEY:
                            throw new InvalidKeyException("There is one or more invalid signing key(s) in the current context.");
                        case gpg_err_code_t.GPG_ERR_INV_VALUE:
                            throw new InvalidPtrException("Either the context, recipient key array, plain text or cipher text pointer is invalid.");
                        case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                            throw new BadPassphraseException();
                        default:
                            throw new GeneralErrorException("An unexpected error "
                                + errcode.ToString()
                                + " (" + err.ToString()
                                + ") occurred.");
                    }
                    IntPtr rstPtr = libgpgme.gpgme_op_encrypt_result(CtxPtr);

					GC.KeepAlive(recp);
                    GC.KeepAlive(recipients);
                    GC.KeepAlive(plain);
                    GC.KeepAlive(cipher);
					
                    if (rstPtr != IntPtr.Zero)
                    {
                        EncryptionResult encRst = new EncryptionResult(rstPtr);
                        return encRst;
                    }
                    else
                        throw new GeneralErrorException("An unexpected error occurred. " + errcode.ToString());
                }
            }
            else
                throw new InvalidContextException();
        }

        public SignatureResult Sign(GpgmeData plain, GpgmeData sig, SignatureMode mode)
        {
            if (IsValid)
            {
                if (plain == null) 
                    throw new ArgumentNullException("Source data buffer must be supplied.");
                if (!(plain.IsValid))
                    throw new InvalidDataBufferException("The specified source data buffer is invalid.");

                if (sig == null)
                    throw new ArgumentNullException("Destination data buffer must be supplied.");
                if (!(sig.IsValid))
                    throw new InvalidDataBufferException("The specified destination data buffer is invalid.");

                lock (CtxLock)
                {
                    int err = libgpgme.gpgme_op_sign(
                        CtxPtr,
                        plain.dataPtr,
                        sig.dataPtr,
                        (gpgme_sig_mode_t)mode);

                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);
                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_UNUSABLE_SECKEY:
                            throw new InvalidKeyException("There is one or more invalid signing key(s) in the current context.");
                        case gpg_err_code_t.GPG_ERR_INV_VALUE:
                            throw new InvalidPtrException("Either the context, plain text or cipher text pointer is invalid.");
                        case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                            throw new BadPassphraseException();
                        default:
                            throw new GeneralErrorException("An unexpected error "
                                + errcode.ToString()
                                + " (" + err.ToString()
                                + ") occurred.");
                    }
                    IntPtr rstPtr = libgpgme.gpgme_op_sign_result(CtxPtr);
                    if (rstPtr != IntPtr.Zero)
                    {
                        SignatureResult sigRst = new SignatureResult(rstPtr);
                        return sigRst;
                    }
                    else
                        throw new GeneralErrorException("An unexpected error occurred. " + errcode.ToString());
                }
            }
            else
                throw new InvalidContextException();
        }
        public DecryptionResult Decrypt(GpgmeData cipher, GpgmeData plain)
        {
            if (IsValid)
            {
                if (cipher == null) 
                    throw new ArgumentNullException("Source data buffer must be supplied.");
                if (!(cipher.IsValid))
                    throw new InvalidDataBufferException("The specified source data buffer is invalid.");

                if (plain == null) 
                    throw new ArgumentNullException("Destination data buffer must be supplied.");
                if (!(plain.IsValid))
                    throw new InvalidDataBufferException("The specified destination data buffer is invalid.");

                lock (CtxLock)
                {
#if (VERBOSE_DEBUG)
                    DebugOutput("gpgme_op_decrypt(..) START");
#endif
                    int err = libgpgme.gpgme_op_decrypt(
                        CtxPtr,
                        cipher.dataPtr,
                        plain.dataPtr);
#if (VERBOSE_DEBUG)
                    DebugOutput("gpgme_op_decrypt(..) DONE");
#endif
                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_NO_DATA:
                            throw new NoDataException("The cipher does not contain any data to decrypt or is corrupt.");
                        case gpg_err_code_t.GPG_ERR_INV_VALUE:
                            throw new InvalidPtrException("Either the context, cipher text or plain text pointer is invalid.");
                    }

                    DecryptionResult decRst = null;

                    IntPtr rstPtr = libgpgme.gpgme_op_decrypt_result(CtxPtr);
                    if (rstPtr != IntPtr.Zero)
                        decRst = new DecryptionResult(rstPtr);

                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_DECRYPT_FAILED:
                            if (decRst == null)
                                throw new DecryptionFailedException("An invalid cipher text has been supplied.");
                            else
                                throw new DecryptionFailedException(decRst);
                        case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                            throw new BadPassphraseException(decRst);
                        default:
                            throw new GeneralErrorException("An unexpected error occurred. " + errcode.ToString());
                    }

                    GC.KeepAlive(cipher);
                    GC.KeepAlive(plain);

                    return decRst;;
                }
            }
            else
                throw new InvalidContextException();
        }

        public CombinedResult DecryptAndVerify(GpgmeData cipher, GpgmeData plain)
        {
            if (IsValid)
            {
                if (cipher == null) 
                    throw new ArgumentNullException("Source data buffer must be supplied.");
                if (!(cipher.IsValid))
                    throw new InvalidDataBufferException("The specified source data buffer is invalid.");
                
                if (plain == null) 
                    throw new ArgumentNullException("Destination data buffer must be supplied.");
                if (!(plain.IsValid))
                    throw new InvalidDataBufferException("The specified destination data buffer is invalid.");

                lock (CtxLock)
                {
#if (VERBOSE_DEBUG)
                    DebugOutput("gpgme_op_decrypt_verify(..) START");
#endif
                    int err = libgpgme.gpgme_op_decrypt_verify(
                        CtxPtr,
                        cipher.dataPtr,
                        plain.dataPtr);
#if (VERBOSE_DEBUG)
                    DebugOutput("gpgme_op_decrypt_verify(..) DONE");
#endif
                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_NO_DATA:
                            // no encrypted data found - maybe it is only signed.
                            break;
                        case gpg_err_code_t.GPG_ERR_INV_VALUE:
                            throw new InvalidPtrException("Either the context, cipher text or plain text pointer is invalid.");
                    }

                    DecryptionResult decRst = null;

                    IntPtr rstPtr = libgpgme.gpgme_op_decrypt_result(CtxPtr);
                    if (rstPtr != IntPtr.Zero)
                        decRst = new DecryptionResult(rstPtr);

                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
             
                        case gpg_err_code_t.GPG_ERR_DECRYPT_FAILED:
                            if (decRst == null)
                                throw new DecryptionFailedException("An invalid cipher text has been supplied.");
                            else
                                throw new DecryptionFailedException(decRst);

                        case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                            throw new BadPassphraseException(decRst);

                        default:
                            throw new GeneralErrorException("An unexpected error occurred. " 
                                + errcode.ToString());
                    }

                    /* If decryption failed, verification cannot be proceeded */
                    VerificationResult verRst = null;

                    rstPtr = IntPtr.Zero;
                    rstPtr = libgpgme.gpgme_op_verify_result(CtxPtr);
                    if (rstPtr != IntPtr.Zero)
                        verRst = new VerificationResult(rstPtr);

                    GC.KeepAlive(cipher);
                    GC.KeepAlive(plain);

                    return new CombinedResult(decRst, verRst);
                }
            }
            else
                throw new InvalidContextException();
        }

        /// <summary>
        /// Sets the GNUPG directory where the libgpgme-11.dll can be found.
        /// </summary>
        /// <param name="path">Path to libgpgme-11.dll.</param>
        public void SetDllDirectory(string path)
        {
            if (Environment.OSVersion.Platform.ToString().Contains("Win32") ||
                Environment.OSVersion.Platform.ToString().Contains("Win64"))
            {
                if (path != null && path != String.Empty)
                {
                    string tmp;
                    if (path[path.Length - 1] != '\\')
                        tmp = path + "\\";
                    else
                        tmp = path;

                    if (!File.Exists(tmp + libgpgme.GNUPG_LIBNAME))
                        throw new FileNotFoundException("Could not find GPGME DLL file.", tmp + libgpgme.GNUPG_LIBNAME);

                    if (!libgpgme.SetDllDirectory(path))
                        throw new GpgmeException("Could not set DLL path " + path);
                    else
                    {
                        libgpgme.InitLibgpgme();
                    }
                }
            }
        }

        public VerificationResult Verify(GpgmeData signature, GpgmeData signedtext, GpgmeData plain)
        {
            if (IsValid)
            {
                if (signature == null)
                    throw new ArgumentNullException("A signature data buffer must be supplied.");
                if (!(signature.IsValid))
                    throw new InvalidDataBufferException("The specified signature data buffer is invalid.");

                if (
                    (signedtext == null || !(signedtext.IsValid)) 
                    && (plain == null || !(plain.IsValid))
                    )
                {
                    throw new InvalidDataBufferException("Either the signed text must be provided in order to prove the detached signature, or an empty data buffer to store the plain text result.");
                }
                lock (CtxLock)
                {
                    IntPtr sigPtr = IntPtr.Zero, sigtxtPtr = IntPtr.Zero, plainPtr = IntPtr.Zero;
                    
                    sigPtr = signature.dataPtr;
                    if (signedtext != null && signedtext.IsValid)
                        sigtxtPtr = signedtext.dataPtr;
                    if (plain != null && plain.IsValid)
                        plainPtr = plain.dataPtr;

                    int err = libgpgme.gpgme_op_verify(
                        CtxPtr,
                        sigPtr,
                        sigtxtPtr,
                        plainPtr);

                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);
                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_NO_DATA:
                            break;
                        case gpg_err_code_t.GPG_ERR_INV_VALUE:
                            throw new InvalidDataBufferException("Either the signature, signed text or plain text data buffer is invalid.");
                        default:
                            throw new GeneralErrorException("Unexpected error occurred. Error: " + errcode.ToString());
                    }

                    VerificationResult verRst = null;

                    IntPtr rstPtr = IntPtr.Zero;
                    rstPtr = libgpgme.gpgme_op_verify_result(CtxPtr);
                    if (rstPtr != IntPtr.Zero)
                    {
                        verRst = new VerificationResult(rstPtr);
                        if (errcode == gpg_err_code_t.GPG_ERR_NO_DATA)
                            throw new NoDataException("The signature does not contain any data to verify.");
                        else
                            return verRst;
                    }
                    else
                        throw new GeneralErrorException("Could not retrieve verification result.");
                }
            }
            else
                throw new InvalidContextException();
        }


        private ContextSigners signers;
        public ContextSigners Signers
        {
            get { return signers; }
        }

#if (VERBOSE_DEBUG)
		private void DebugOutput(string text)
        {
            Console.WriteLine("Debug: " + text);
			Console.Out.Flush();
        }
#endif

        public class ContextSigners: IEnumerable<Key>
        {
            private Context ctx;
            internal ContextSigners(Context ctx)
            {
                this.ctx = ctx;
            }
            public void Add(Key signer)
            {
                if (ctx.IsValid)
                {
                    if (signer == null)
                        throw new ArgumentNullException("A signer key must be supplied.");
                    if (signer.KeyPtr.Equals(IntPtr.Zero))
                        throw new InvalidKeyException("An invalid signer key has been supplied.");

                    lock (ctx.CtxLock)
                    {
                        int err = libgpgme.gpgme_signers_add(ctx.CtxPtr, signer.KeyPtr);
                        gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);
                        if (errcode != gpg_err_code_t.GPG_ERR_NO_ERROR)
                            throw new GeneralErrorException("An unexpected error occurred. Error: " + errcode.ToString());
                    }
                }
                else
                    throw new InvalidContextException();
            }
            public void Clear()
            {
                if (ctx.IsValid)
                {
                    lock (ctx.CtxLock)
                    {
                        libgpgme.gpgme_signers_clear(ctx.CtxPtr);
                    }
                }
                else
                    throw new InvalidContextException();

            }

            public Key Enum(int seq)
            {
                if (ctx.IsValid)
                {
                    lock (ctx.CtxLock) // could be locked twice by Get()
                    {
                        IntPtr rstPtr = libgpgme.gpgme_signers_enum(ctx.CtxPtr, seq);

                        if (rstPtr.Equals(IntPtr.Zero))
                            return null;

                        return new Key(rstPtr);
                    }
                }
                else
                    throw new InvalidContextException();
            }
            public Key[] Get()
            {
                if (ctx.IsValid)
                {
                    lock (ctx.CtxLock)
                    {
                        List<Key> lst = new List<Key>();
                        int i = 0;
                        Key key;
                        while ((key = Enum(i++)) != null)
                            lst.Add(key);

                        if (lst.Count == 0)
                            return new Key[0];
                        else
                            return lst.ToArray();
                    }
                }
                else
                    throw new InvalidContextException();
            }
            public IEnumerator<Key> GetEnumerator()
            {
                int i = 0;
                Key key;

                while ((key = Enum(i++)) != null)
                    yield return key;
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        private ContextSignatureNotations signots;
        public ContextSignatureNotations SignatureNotations
        {
            get { return signots; }
        }
        public class ContextSignatureNotations: IEnumerable<SignatureNotation>
        {
            Context ctx;
            internal ContextSignatureNotations(Context ctx)
            {
                this.ctx = ctx;
            }
            public void Add(string name, string value, SignatureNotationFlags flags)
            {
                if (ctx.IsValid)
                {
                    IntPtr namePtr = IntPtr.Zero;
                    IntPtr valuePtr = IntPtr.Zero;
                    if (name != null)
                        namePtr = Gpgme.StringToCoTaskMemUTF8(name);
                    if (value != null)
                        valuePtr = Gpgme.StringToCoTaskMemUTF8(value);

                    int err = libgpgme.gpgme_sig_notation_add(
                        ctx.CtxPtr,
                        namePtr,
                        valuePtr,
                        (gpgme_sig_notation_flags_t)flags);

                    if (namePtr != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(namePtr);
                        namePtr = IntPtr.Zero;
                    }
                    if (valuePtr != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(valuePtr);
                        valuePtr = IntPtr.Zero;
                    }

                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);
                    if (errcode == gpg_err_code_t.GPG_ERR_NO_ERROR)
                        return;
                    if (errcode == gpg_err_code_t.GPG_ERR_INV_VALUE)
                        throw new ArgumentException("NAME and VALUE are in an invalid combination.");

                    throw new GeneralErrorException("An unexpected error occurred. Error: " 
                        + errcode.ToString());
                }
                else
                    throw new InvalidContextException();
            }
            public void Clear()
            {
                if (ctx.IsValid)
                {
                    libgpgme.gpgme_sig_notation_clear(ctx.CtxPtr);
                }
                else
                    throw new InvalidContextException();
            }
            public SignatureNotation Get()
            {
                if (ctx.IsValid)
                {
                    IntPtr rstPtr = libgpgme.gpgme_sig_notation_get(ctx.CtxPtr);
                    if (rstPtr.Equals(IntPtr.Zero))
                        return null;
                    else
                        return new SignatureNotation(rstPtr);
                }
                else
                    throw new InvalidContextException();
            }
            public IEnumerator<SignatureNotation> GetEnumerator()
            {
                return Get().GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}