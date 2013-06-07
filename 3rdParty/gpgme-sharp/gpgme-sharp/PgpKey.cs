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
using System.IO;
using System.Reflection;

using Libgpgme.Interop;

namespace Libgpgme
{
    public class PgpKey: Key 
    {
        internal enum KeyEditOp
        {
            Signature,          // sign, lsign, tsign, nrsign, ..
            Passphrase,         // passwd
            RevokeSignature,    // revsig
            DeleteSignature,    // delsig
            EnableDisable,      // enable, disable
            Trust,              // trust
            AddSubkey,          // addkey
            Expire              // expire
        };

        // Variables for key editing
             
        // general key edit settings
        private Settings settings;


        internal PgpKey(IntPtr keyPtr)
            : base(keyPtr) 
        {
            this.settings = new Settings(this);
        }

        public Settings EditSettings
        {
            get { return settings; }
        }

        protected override int KeyEditCallback(IntPtr handle, KeyEditStatusCode status, string args, Stream fd)
        {
            KeyEditOp op = (KeyEditOp)handle;
            byte[] output = null;
            bool runhandler = true;
#if (VERBOSE_DEBUG)
            DebugOutput("Callback op=" + op.ToString() + " status=" + status.ToString() + " args=" + args);
#endif
            // Ignore ACK calls
            if (status == KeyEditStatusCode.GotIt && args == null && fd == null)
                runhandler = false;

            // actions that are equal in all key editing queries - except passphrase changes
            if (op != KeyEditOp.Passphrase)
            {
                switch (status)
                {
                    case KeyEditStatusCode.GoodPassphrase:
                        settings.passSettings.PassphrasePrevWasBad = false;
                        output = new byte[0];
                        runhandler = false;
                        break;
                }
                if (args != null)
                {
                    switch (status)
                    {
                        case KeyEditStatusCode.UserIdHint:
                            settings.passSettings.PassphraseUserIdHint = args;
                            output = new byte[0];
                            runhandler = false;
                            break;

                        case KeyEditStatusCode.NeedPassphrase:
                            settings.passSettings.PassphraseInfo = args;
                            output = new byte[0];
                            runhandler = false;
                            break;

                        case KeyEditStatusCode.MissingPassphrase:
                        case KeyEditStatusCode.BadPassphrase:
                            settings.passSettings.PassphrasePrevWasBad = true;
                            output = new byte[0];
                            runhandler = false;
                            break;

                        case KeyEditStatusCode.GetHidden:
                            if (args.Equals("passphrase.enter"))
                            {
                                char[] passphrase = null;

                                /* "passphrase.enter" appears if the context has no passphrase 
                                 * callback function specified.
                                 */
                                if (settings.passSettings.PassphraseFunction != null
                                    && fd != null
                                    && settings.passSettings.PassphraseLastResult != PassphraseResult.Canceled)
                                {
                                    settings.passSettings.PassphraseLastResult =
                                        settings.passSettings.PassphraseFunction(
                                        null,
                                        new PassphraseInfo(IntPtr.Zero,
                                            settings.passSettings.PassphraseUserIdHint,
                                            settings.passSettings.PassphraseInfo,settings.passSettings.PassphrasePrevWasBad),
                                        ref passphrase);
                                    if (passphrase != null)
                                    {

                                        byte[] p = Gpgme.ConvertCharArrayToUTF8(passphrase, 0);
                                        fd.Write(p, 0, p.Length);

                                        int i;
                                        // try to clear passphrase in memory
                                        for (i = 0; i < p.Length; i++)
                                            p[i] = 0;
                                        for (i = 0; i < passphrase.Length; i++)
                                            passphrase[i] = '\0';
                                    }
                                }
                                else if (settings.passSettings.Passphrase != null && fd != null)
                                {
                                    byte[] p = Gpgme.ConvertCharArrayToUTF8(
                                        settings.passSettings.Passphrase,
                                        0);

                                    fd.Write(p, 0, p.Length);

                                    int i;
                                    // try to clear passphrase in memory
                                    for (i = 0; i < p.Length; i++)
                                        p[i] = 0;
                                }
                                else
                                {
                                    // No password or password callback function specified!
                                    fd.Write(new byte[1] { 0 }, 0, 1);
                                }

                                output = new byte[0]; // confirm password (send \n)
                                
                                runhandler = false;
                            }

                            break;
                    }
                }
            }

            if (runhandler)
            {
#if (VERBOSE_DEBUG)
				DebugOutput("Run handler " + op.ToString());
#endif		
                switch (op)
                {
                    case KeyEditOp.Signature:
                        output = SignHandler(status, args, fd);
                        break;
                    case KeyEditOp.Passphrase:
                        output = PassphraseHandler(status, args, fd);
                        if (output == null && settings.passOptions.aborthandler)
                            return 1; // abort
                        break;
                    case KeyEditOp.RevokeSignature:
                        output = RevokeSignatureHandler(status, args, fd);
                        break;
                    case KeyEditOp.DeleteSignature:
                        output = DeleteSignatureHandler(status, args, fd);
                        break;
                    case KeyEditOp.EnableDisable:
                        output = EnableDisableHandler(status, args, fd);
                        break;
                    case KeyEditOp.Trust:
                        output = TrustHandler(status, args, fd);
                        break;
                    case KeyEditOp.AddSubkey:
                        output = AddSubkeyHandler(status, args, fd);
                        break;
                    case KeyEditOp.Expire:
                        output = ExpireHandler(status, args, fd);
                        break;
                }

#if (VERBOSE_DEBUG)
				DebugOutput("Handler " + op.ToString() + " finished.");
#endif
            }

            if (output != null && fd != null)
            {
#if (VERBOSE_DEBUG)
                DebugOutput(output);
#endif
                fd.Write(output, 0, output.Length);
                fd.Flush();
                fd.Write(new byte[] { (byte)'\n' }, 0, 1);
                fd.Flush();
            }

            return 0;
        }

#if (VERBOSE_DEBUG)
        private void DebugOutput(byte[] barray)
        {
            if (barray != null)
            {
                Console.Write("Debug: ");
                foreach (byte b in barray)
                    Console.Write((char)b);
                Console.WriteLine();
				Console.Out.Flush();
            }
        }
        private void DebugOutput(string text)
        {
            Console.WriteLine("Debug: " + text);
			Console.Out.Flush();
        }
#endif

        private byte[] DeleteSignatureHandler(KeyEditStatusCode status, string args, Stream fd)
        {
            PgpDeleteSignatureOptions delsigOptions = settings.delsigOptions;
            string output = "";

            if (args != null)
            {
                if (args.Equals("keyedit.prompt") && !delsigOptions.cmdSend)
                {
                    if (!delsigOptions.uidSend)
                    {
                        // send uid number
                        delsigOptions.uidSend = true;
                        output = "uid " + delsigOptions.SelectedUid.ToString();
                        return ToU8(output);
                    }

                    // send command
                    delsigOptions.cmdSend = true;
                    output = "delsig";
                    return ToU8(output);
                }

                if (args.Equals("keyedit.delsig.unknown")
                    || args.Equals("keyedit.delsig.valid"))
                {
                    if (delsigOptions.SelectedSignatures == null)
                    {
                        output = "Y";
                    }
                    else
                    {
                        delsigOptions.ndeletenum++;
                        if (Array.Exists<int>(delsigOptions.SelectedSignatures,
                                delegate(int v)
                                {
                                    return (v == delsigOptions.ndeletenum);
                                }))
                        {
                            output = "Y";
                        }
                        else
                        {
                            output = "N";
                        }
                    }
                    return ToU8(output);
                }

                if (args.Equals("keyedit.delsig.selfsig"))
                {
                    if (delsigOptions.DeleteSelfSignature)
                        output = "Y";
                    else
                        output = "N";
                    return ToU8(output);
                }
  
                if (args.Equals("keyedit.prompt") && delsigOptions.cmdSend)
                {
                    output = "save";
                    return ToU8(output);
                }
            }
            
            return new byte[0];
        }
    
        public void DeleteSignature(Context ctx, PgpDeleteSignatureOptions options)
        {
            if (ctx == null || !ctx.IsValid)
                throw new InvalidContextException();

            if (options == null)
                throw new ArgumentNullException("No PgpDeleteSignatureOptions object specified.");

            if (options.SelectedSignatures == null ||
                options.SelectedSignatures.Length == 0)
                throw new ArgumentException("No signatures selected.");

            lock (settings.passLock)
            {
                lock (settings.delsigLock)
                {
                    settings.delsigOptions = options;

                    // reset object
                    options.cmdSend = false;
                    options.uidSend = false;
                    options.ndeletenum = 0;

                    // specify key edit operation;
                    KeyEditOp op = KeyEditOp.DeleteSignature;

                    // output data
                    GpgmeData data = new GpgmeMemoryData();

                    int err;

                    try
                    {
                        err = StartEdit(ctx, (IntPtr)op, data);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                            throw new BadPassphraseException(settings.passSettings.GetPassphraseInfo());

                        default:
                            throw new GpgmeException("An unknown error occurred. Error: " 
                                + err.ToString(), err);
                    }
                }
            }

        }

        private byte[] RevokeSignatureHandler(KeyEditStatusCode status, string args, Stream fd)
        {
            PgpRevokeSignatureOptions revsigOptions = settings.revsigOptions;
            string output = "";

            if (args != null)
            {
                // specify the uids from that the signature shall be revoked
                if (args.Equals("keyedit.prompt") && !revsigOptions.uidSend)
                {
                    revsigOptions.uidSend = true;
                    output = "uid " + revsigOptions.SelectedUid.ToString();
                    return ToU8(output);
                }

                if (args.Equals("keyedit.prompt") && !revsigOptions.cmdSend)
                {
                    revsigOptions.cmdSend = true;
                    output = "revsig";
                    return ToU8(output);
                }

                if (args.Equals("ask_revoke_sig.one") 
                    || args.Equals("ask_revoke_sig.expired")) 
                {
                    revsigOptions.nrevokenum++;
                    // the user can specify his signatures that shall be revoked
                    if (Array.Exists<int>(revsigOptions.SelectedSignatures,
                                delegate(int v)
                                {
                                    return (v == revsigOptions.nrevokenum);
                                }))
                    {
                        output = "Y";
                    }
                    else
                    {
                        output = "N";
                    }
                    return ToU8(output);
                }
                if (args.Equals("ask_revoke_sig.okay")
                    || args.Equals("ask_revocation_reason.okay"))
                {
                    output = "Y"; // we can revoke all signatures that were signed by private key from our store
                    return ToU8(output);
                }

                if (args.Equals("ask_revocation_reason.code"))
                {
                    output = ((int)revsigOptions.ReasonCode).ToString();
                    return ToU8(output);
                }

                if (args.Equals("ask_revocation_reason.text"))
                {
                    if (revsigOptions.reasonTxt == null)
                        output = "";
                    else
                    {
                        if (revsigOptions.nreasonTxt >= revsigOptions.reasonTxt.Length)
						{
							if (libgpgme.IsWindows)
								output = "";
							else
								output = " ";
						}
                        else
                            output = revsigOptions.reasonTxt[revsigOptions.nreasonTxt++];
                    }
                    return ToU8(output);
                }

                if (args.Equals("keyedit.prompt") && revsigOptions.cmdSend)
                {
                    output = "save";
                    return ToU8(output);
                }
            }

            return new byte[0];
        }

        public void RevokeSignature(Context ctx, PgpRevokeSignatureOptions options)
        {
            if (ctx == null)
                throw new ArgumentNullException("No context object supplied.");
            if (!ctx.IsValid)
                throw new InvalidContextException("An invalid context has been supplied.");

            if (options == null)
                throw new ArgumentNullException("No revocation options specified.");

            if (options.SelectedSignatures == null ||
                options.SelectedSignatures.Length == 0)
                throw new ArgumentException("No signatures selected.");

            lock (settings.passLock)
            {
                lock (settings.revsigLock)
                {
                    settings.revsigOptions = options;

                    // reset object
                    options.cmdSend     = false;
                    options.uidSend     = false;
                    options.reasonSend  = false;
                    // reset reason text counter (gnupg prompts for each line)
                    options.nreasonTxt  = 0;
                    /* reset own signature counter (user could have signed the key with more
                     * than one of his keys. */
                    options.nrevokenum  = 0;
                    
                    // specify key edit operation;
                    KeyEditOp op = KeyEditOp.RevokeSignature;

                    // output data
                    GpgmeData data = new GpgmeMemoryData();

                    int err;

                    try
                    {
                        err = StartEdit(ctx, (IntPtr)op, data);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                            throw new BadPassphraseException(settings.passSettings.GetPassphraseInfo());

                        default:
                            throw new GpgmeException("An unknown error occurred. Error: "
                                + err.ToString(), err);
                    }
                }
            }
        }

        private byte[] TrustHandler(KeyEditStatusCode status, string args, Stream fd)
        {
            PgpTrustOptions trustOptions = settings.trustOptions;
            string output = "";

            if (args != null)
            {
                if (args.Equals("keyedit.prompt"))
                {
                    if (!trustOptions.cmdSend)
                    {
                        output = "trust";
                        trustOptions.cmdSend = true;
                    }
                    else
                    {
                        output = "quit";
                    }

                    return ToU8(output);
                }
                if (args.Equals("edit_ownertrust.set_ultimate.okay"))
                {
                    output = "Y";
                    return ToU8(output);
                }
                if (args.Equals("edit_ownertrust.value"))
                {
                    output = ((int)trustOptions.trust).ToString();
                    return ToU8(output);
                }
            }

            return new byte[0];
        }


        public void SetOwnerTrust(Context ctx, PgpOwnerTrust trust)
        {
            if (ctx == null)
                throw new ArgumentNullException("No context object supplied.");
            if (!ctx.IsValid)
                throw new InvalidContextException("An invalid context has been supplied.");

            lock (settings.trustLock)
            {
                settings.trustOptions = new PgpTrustOptions();
                settings.trustOptions.trust = trust;

                // specify key edit operation;
                KeyEditOp op = KeyEditOp.Trust;

                // output data
                GpgmeData data = new GpgmeMemoryData();

                int err;

                try
                {
                    err = StartEdit(ctx, (IntPtr)op, data);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

                switch (errcode)
                {
                    case gpg_err_code_t.GPG_ERR_NO_ERROR:
                        break;
                    case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                        throw new BadPassphraseException(settings.passSettings.GetPassphraseInfo());

                    default:
                        throw new GpgmeException("An unknown error occurred.", err);
                }
            }
        }

        private byte[] EnableDisableHandler(KeyEditStatusCode status, string args, Stream fd)
        {
            PgpEnableDisableOptions endisOptions = settings.endisOptions;
            string output = "";

            if (args != null)
            {
                if (args.Equals("keyedit.prompt"))
                {
                    if (!endisOptions.cmdSend)
                    {
                        switch (endisOptions.OperationMode) {
                            case PgpEnableDisableOptions.Mode.Enable:
                                output = "enable";
                                break;
                            case PgpEnableDisableOptions.Mode.Disable:
                                output = "disable";
                                break;
                        }
                        endisOptions.cmdSend = true;
                    }
                    else
                    {
                        output = "quit";
                    }

                    return ToU8(output);
                }
            }

            return new byte[0];
        }

        public void Enable(Context ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException("No context object supplied.");
            if (!ctx.IsValid)
                throw new InvalidContextException("An invalid context has been supplied.");

            lock (settings.endisLock)
            {
                settings.endisOptions = new PgpEnableDisableOptions();
                settings.endisOptions.OperationMode = PgpEnableDisableOptions.Mode.Enable;

                // specify key edit operation;
                KeyEditOp op = KeyEditOp.EnableDisable;

                // output data
                GpgmeData data = new GpgmeMemoryData();

                int err;

                try
                {
                    err = StartEdit(ctx, (IntPtr)op, data);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

                switch (errcode)
                {
                    case gpg_err_code_t.GPG_ERR_NO_ERROR:
                        break;
                    case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                        throw new BadPassphraseException(settings.passSettings.GetPassphraseInfo());

                    default:
                        throw new GpgmeException("An unknown error occurred.", err);
                }

            }
        }

        public void Disable(Context ctx)
        {
            if (ctx == null)
                throw new ArgumentNullException("No context object supplied.");
            if (!ctx.IsValid)
                throw new InvalidContextException("An invalid context has been supplied.");

            lock (settings.endisLock)
            {
                settings.endisOptions = new PgpEnableDisableOptions();
                settings.endisOptions.OperationMode = PgpEnableDisableOptions.Mode.Disable;

                // specify key edit operation;
                KeyEditOp op = KeyEditOp.EnableDisable;

                // output data
                GpgmeData data = new GpgmeMemoryData();

                int err;

                try
                {
                    err = StartEdit(ctx, (IntPtr)op, data);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

                switch (errcode)
                {
                    case gpg_err_code_t.GPG_ERR_NO_ERROR:
                        break;
                    case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                        throw new BadPassphraseException(settings.passSettings.GetPassphraseInfo());

                    default:
                        throw new GpgmeException("An unknown error occurred.", err);
                }

            }
        }

        private byte[] SignHandler(KeyEditStatusCode status, string args, Stream fd) 
        {
            PgpSignatureOptions sigOptions = settings.sigOptions;
            string output = "";

            if (args != null)
            {
                if (status == KeyEditStatusCode.AlreadySigned)
                    throw new AlreadySignedException(args);

                // specify the uids that shall be signed
                if (args.Equals("keyedit.prompt") && !sigOptions.cmdSend 
                    && sigOptions.nUid == 0)
                {
                    // do we want to specify the uids that we want to sign?
                    if (sigOptions.SelectedUids != null && sigOptions.SelectedUids.Length > 0)
                        sigOptions.signAllUids = false;
                    else
                        sigOptions.signAllUids = true; 
                }

                if (args.Equals("keyedit.prompt") && (!sigOptions.signAllUids)
                    && sigOptions.nUid < sigOptions.SelectedUids.Length)
                {
                    output = "uid " + sigOptions.SelectedUids[sigOptions.nUid++];
                    return ToU8(output);
                }

                if (args.Equals("keyedit.prompt") && !sigOptions.cmdSend)
                {
                    StringBuilder sb = new StringBuilder();

                    if ((sigOptions.Type & PgpSignatureType.NonExportable) 
                        == PgpSignatureType.NonExportable)
                        sb.Append("l");

                    if ((sigOptions.Type & PgpSignatureType.Trust)
                        == PgpSignatureType.Trust)
                        sb.Append("t");

                    if ((sigOptions.Type & PgpSignatureType.NonRevocable)
                        == PgpSignatureType.NonRevocable)
                        sb.Append("nr");

                    output = sb.ToString() + "sign";

                    // mark that the operation command has been sent
                    sigOptions.cmdSend = true;

                    return ToU8(output);
                }
                
                if (args.Equals("sign_uid.class"))
                {
                    output = ((int)sigOptions.Class).ToString();
                    return ToU8(output);
                }

                if (args.Equals("sign_uid.expire"))
                {
                    output = sigOptions.IsInfinitely ? "N" : "Y";
                    return ToU8(output);
                }

                if (args.Equals("siggen.valid"))
                {
                    output = sigOptions.GetExpirationDate();
                    return ToU8(output);
                }

                if (args.Equals("trustsig_prompt.trust_value"))
                {
                    output = ((int)sigOptions.TrustLevel).ToString();
                    return ToU8(output);
                }

                if (args.Equals("trustsig_prompt.trust_depth"))
                {
                    output = sigOptions.TrustDepth.ToString();
                    return ToU8(output);
                }

                if (args.Equals("trustsig_prompt.trust_regexp"))
                {
                    output = sigOptions.TrustRegexp;
                    if (output == null)
                        output = "";
                    return Gpgme.ConvertCharArrayAnsi(output.ToCharArray());
                }

                if (args.Equals("sign_uid.local_promote_okay"))
                {
                    output = sigOptions.LocalPromoteOkay ? "Y" : "N";
                    return ToU8(output);
                }

                if (args.Equals("sign_uid.okay"))
                {
                    output = "Y"; // Really sign? (y/N)
                    return ToU8(output);
                }

                if (args.Equals("keyedit.sign_all.okay"))
                {
                    if (sigOptions.signAllUids) // Really sign all user IDs? (y/N)
                    {
                        output = "Y"; 
                    }
                    else
                    {
                        output = "N";
                    }
                    return ToU8(output);
                }

                if (args.Equals("keyedit.prompt"))
                {
                    if (!sigOptions.forceQuit)
                    {
                        output = "save";
                        sigOptions.forceQuit = true;
                    }
                    else
                        output = "quit";
                    return ToU8(output);
                }

                if (args.Equals("keyedit.save.okay"))
                {
                    output = "Y";          // Save changes? (y/N) 
                    return ToU8(output);
                }

                // .. unknown question
            }

            return new byte[0];
        }

        public void Sign(Context ctx, PgpSignatureOptions options)
        {
            if (ctx == null)
                throw new ArgumentNullException("No context object supplied.");
            if (!ctx.IsValid)
                throw new InvalidContextException("An invalid context has been supplied.");

            if (options == null)
                throw new ArgumentNullException("No PgpSignatureOptions object specified.");

            lock (settings.passLock)
            {
                lock (settings.sigLock)
                {
                    settings.sigOptions = options;

                    // reset object
                    options.cmdSend = false;
                    options.nUid = 0;
                    options.forceQuit = false;
                    options.signAllUids = true;

                    // specify key edit operation;
                    KeyEditOp op = KeyEditOp.Signature;

                    // output data
                    GpgmeData data = new GpgmeMemoryData();

                    int err;

                    try
                    {
                        err = StartEdit(ctx, (IntPtr)op, data);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                            throw new BadPassphraseException(settings.passSettings.GetPassphraseInfo());
                                
                        default:
                            throw new GpgmeException("An unknown error occurred. Error: " 
                                + err.ToString(), err);
                    }
                }
            }
        }


        private byte[] ExpireHandler(KeyEditStatusCode status, string args, Stream fd)
        {
            PgpExpirationOptions expireOptions = settings.expireOptions;
            string output = "";

            if (args != null)
            {
                if (args.Equals("keyedit.prompt") && !expireOptions.cmdSend)
                {
                    if (expireOptions.SelectedSubkeys != null
                        && (expireOptions.nsubkey < expireOptions.SelectedSubkeys.Length))
                    {
                        output = "key " + expireOptions.SelectedSubkeys[expireOptions.nsubkey++].ToString();
                    }
                    else
                    {
                        expireOptions.cmdSend = true;
                        output = "expire";
                    }
                    return ToU8(output);
                }

                // Expire date in days
                if (args.Equals("keygen.valid"))
                {
                    if (expireOptions.IsInfinitely || (expireOptions.ExpirationDate.CompareTo(DateTime.Now) < 0))
                        output = "0";
                    else
                        output = (expireOptions.ExpirationDate - DateTime.Now).Days.ToString();

                    return ToU8(output);
                }

                if (args.Equals("keyedit.save.okay"))
                {
                    if (!expireOptions.forceQuit)
                        output = "Y";
                    else
                    {
                        // maybe an other gnupg process is editing this key at the same time
                        output = "N"; 
                    }

                    return ToU8(output);
                }

                if (args.Equals("keyedit.prompt") && expireOptions.forceQuit)
                {
                    output = "quit";
                    return ToU8(output);
                }

                if (args.Equals("keyedit.prompt"))
                {
                    expireOptions.forceQuit = true;
                    output = "save";
                    return ToU8(output);
                }

            }
            
            return new byte[0];
        }

        public void SetExpirationDate(Context ctx, PgpExpirationOptions options)
        {
            if (ctx == null)
                throw new ArgumentNullException("No context object supplied.");
            if (!ctx.IsValid)
                throw new InvalidContextException("An invalid context has been supplied.");

            if (options == null)
                throw new ArgumentNullException("No PgpExpireOptions object specified.");

            lock (settings.passLock)
            {
                lock (settings.expireLock)
                {
                    settings.expireOptions = options;
                    
                    // reset object
                    options.cmdSend = false;
                    // reset subkey index
                    options.nsubkey = 0;
                    // reset enforced quit
                    options.forceQuit = false;

                    KeyEditOp op = KeyEditOp.Expire;
                    GpgmeData data = new GpgmeMemoryData();

                    int err;
                    try
                    {
                        err = StartEdit(ctx, (IntPtr)op, data);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);
                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                            throw new BadPassphraseException(settings.passSettings.GetPassphraseInfo());
                        default:
                            throw new GpgmeException("An unknown error occurred. Error: "
                                + err.ToString(), err);
                    }
                }
            }
        }

        private byte[] AddSubkeyHandler(KeyEditStatusCode status, string args, Stream fd)
        {
#if (VERBOSE_DEBUG)
			DebugOutput("Inside AddSubkeyHandler(..)");
#endif
			
            PgpSubkeyOptions subkeyOptions = settings.subkeyOptions;
            string output = "";

            if (args != null)
            {
                if (args.Equals("keyedit.prompt") && !subkeyOptions.cmdSend)
                {
                    // send command
                    subkeyOptions.cmdSend = true;
                    output = "addkey";
					
#if (VERBOSE_DEBUG)
					DebugOutput("End keygen.algo.");
#endif
                    return ToU8(output);
                }
                if (args.Equals("keygen.algo"))
                {
                    /* Gnupg's configuration mode needs be set to "expert" 
                     * in order to specify customized DSA or RSA subkeys.
                     */

                    settings.subkeyalgoquestion++;

                    output = ((int)subkeyOptions.Algorithm).ToString();
                    
                    // GPG IS NOT IN EXPERT MODE!
                    if (settings.subkeyalgoquestion > 1)
					{
#if (VERBOSE_DEBUG)
						DebugOutput("End keygen.algo.");
#endif
                        return ToU8("2");
					}

#if (VERBOSE_DEBUG)
					DebugOutput("End keygen.algo.");
#endif         
					return ToU8(output);
                }
                if (args.Equals("keygen.flags"))
                {

                    // Auth is NOT enabled by default
                    if ((subkeyOptions.Capability & AlgorithmCapability.CanAuth) == AlgorithmCapability.CanAuth &&
                        ((settings.subkeycapability & AlgorithmCapability.CanAuth) != AlgorithmCapability.CanAuth))
                    {
                        settings.subkeycapability |= AlgorithmCapability.CanAuth;
                        output = "A";
                    }
                    // Sign is enabled by default!
                    else if ((subkeyOptions.Capability & AlgorithmCapability.CanSign) != AlgorithmCapability.CanSign &&
                        ((settings.subkeycapability & AlgorithmCapability.CanSign) != AlgorithmCapability.CanSign))
                    {
                        settings.subkeycapability |= AlgorithmCapability.CanSign; // save, that we have checked "Sign" flag
                        output = "S";
                    }
                    // Encrypt is enabled by default!
                    else if ((subkeyOptions.Capability & AlgorithmCapability.CanEncrypt) != AlgorithmCapability.CanEncrypt &&
                        ((settings.subkeycapability & AlgorithmCapability.CanEncrypt) != AlgorithmCapability.CanEncrypt))
                    {
                        settings.subkeycapability |= AlgorithmCapability.CanEncrypt; // save, that we have checked "Encrypt" flag
                        output = "E";
                    }
                    else
                        // all flags specified
                        output = "Q";
					
#if (VERBOSE_DEBUG)
					DebugOutput("End keygen.flags.");
#endif         
					return ToU8(output);
                }
                if (args.Equals("keygen.size"))
                {
                    output = subkeyOptions.KeyLength.ToString();

#if (VERBOSE_DEBUG)
					DebugOutput("End keygen.size.");
#endif         
					return ToU8(output);
                }
                if (args.Equals("keygen.valid"))
                {
                    if (subkeyOptions.IsInfinitely || (subkeyOptions.ExpirationDate.CompareTo(DateTime.Now) < 0))
                        output = "0";
                    else
                        output = (subkeyOptions.ExpirationDate - DateTime.Now).Days.ToString();
                    
#if (VERBOSE_DEBUG)
					DebugOutput("End keygen.valid.");
#endif         
					return ToU8(output);
                }
                if (args.Equals("keyedit.prompt"))
                {
                    if (settings.subkeyalgoquestion > 1)
                        /* Do not save the new created subkey because the user 
                         * requested a customized subkey but GnuPG was not in 
                         * "Expert" mode.
                         */
                        output = "quit"; 
                    else
                        output = "save";
					
#if (VERBOSE_DEBUG)
					DebugOutput("End --edit-key session.");
#endif         
					return ToU8(output);
                }
            }
			
#if (VERBOSE_DEBUG)
			DebugOutput("End - WITH BYTE[0]");
#endif
            return new byte[0];
        }

    
        public void AddSubkey(Context ctx, PgpSubkeyOptions options)
        {
            if (ctx == null)
                throw new ArgumentNullException("No context object supplied.");
            if (!ctx.IsValid)
                throw new InvalidContextException("An invalid context has been supplied.");

            if (options == null)
                throw new ArgumentNullException("No PgpSubkeyOptions object specified.");
            
            lock (settings.passLock)
            {
                lock (settings.subkeyLock)
                {
                    settings.subkeyOptions = options;
                    settings.subkeycapability = AlgorithmCapability.CanNothing;
                    settings.subkeyalgoquestion = 0;
                    options.cmdSend = false;

                    KeyEditOp op = KeyEditOp.AddSubkey;
                    GpgmeData data = new GpgmeMemoryData();

                    int err;
                    try
                    {
                        err = StartEdit(ctx, (IntPtr)op, data);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);
                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            if (settings.subkeyalgoquestion > 1)
                                throw new NotSupportedException("GnuPG was not in expert mode. Customized subkeys are not supported.");
                            break;
                        case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                            throw new BadPassphraseException(settings.passSettings.GetPassphraseInfo());
                        default:
                            throw new GpgmeException("An unknown error occurred. Error: "
                                + err.ToString(), err);
                    }
                }
            }
        }

        private byte[] PassphraseHandler(KeyEditStatusCode status, string args, Stream fd)
        {
            PgpPassphraseOptions passOptions = settings.passOptions;
            string output = "";
            int i;

            switch (status)
            {
                case KeyEditStatusCode.MissingPassphrase:
                    passOptions.missingpasswd = true; // empty passphrase
                    passOptions.emptypasswdcount++;
				
                    if (passOptions.missingpasswd &&
                        passOptions.emptypasswdcount >= PgpPassphraseOptions.MAX_PASSWD_COUNT)
                    {
                        passOptions.aborthandler = true;
                        return null;
                    }
                    break;

                case KeyEditStatusCode.GoodPassphrase:
                    if (passOptions.needoldpw)  // old password has been entered correctly
                        passOptions.needoldpw = false;

                    return null;


                case KeyEditStatusCode.NeedPassphraseSym:
                    if (passOptions.needoldpw)  // old password has been entered already
                        passOptions.needoldpw = false;

                    return null;
            }
            

            if (args != null)
            {
                if (args.Equals("keyedit.prompt") && !passOptions.passphraseSendCmd)
                {
                    passOptions.passphraseSendCmd = true;

                    output = "passwd";
                    return ToU8(output);
                }

                if (args != null)
                {
                    switch (status)
                    {
                        case KeyEditStatusCode.UserIdHint:
                            settings.passSettings.PassphraseUserIdHint = args;
                            return new byte[0];
                            
                        case KeyEditStatusCode.NeedPassphrase:
                            settings.passSettings.PassphraseInfo = args;
                            return new byte[0];

                        case KeyEditStatusCode.MissingPassphrase:
                        case KeyEditStatusCode.BadPassphrase:
                            settings.passSettings.PassphrasePrevWasBad = true;
                            return new byte[0];

                        case KeyEditStatusCode.GetHidden:
                            if (args.Equals("passphrase.enter"))
                            {
                                char[] passphrase = null;
                                PassphraseDelegate passphraseFunc = null;

                                if (passOptions.needoldpw)
                                {
                                    // ask for old password
                                    passphraseFunc = passOptions.OldPassphraseCallback;
                                    if (passOptions.OldPassphrase != null)
                                    {
                                        passphrase = new char[passOptions.OldPassphrase.Length];
                                        // TODO: can we trust Array.Copy?
                                        Array.Copy(passOptions.OldPassphrase, 
                                            passphrase, 
                                            passOptions.OldPassphrase.Length);
                                    }
                                }
                                else
                                {
                                    // ask for new password
                                    passphraseFunc = passOptions.NewPassphraseCallback;
                                    if (passOptions.NewPassphrase != null)
                                    {
                                        passphrase = new char[passOptions.NewPassphrase.Length];
                                        // TODO: can we trust Array.Copy?
                                        Array.Copy(passOptions.OldPassphrase, 
                                            passphrase, 
                                            passOptions.NewPassphrase.Length);
                                    }
                                }

                                if (passphraseFunc != null
                                    && fd != null
                                    && settings.passSettings.PassphraseLastResult != PassphraseResult.Canceled)
                                {
#if (VERBOSE_DEBUG)
								    DebugOutput("Calling passphrase callback function.. ");
#endif
                                    // run callback function
                                    settings.passSettings.PassphraseLastResult =
                                        passphraseFunc(
                                        null,
                                        new PassphraseInfo(IntPtr.Zero,
                                            settings.passSettings.PassphraseUserIdHint,
                                            settings.passSettings.PassphraseInfo, settings.passSettings.PassphrasePrevWasBad),
                                        ref passphrase);
                                }
                                
                                if (passphrase != null)
                                {
                                    if (fd != null)
                                    {
                                        byte[] p = Gpgme.ConvertCharArrayToUTF8(passphrase, 0);
                                        fd.Write(p, 0, p.Length);

                                        // try to clear passphrase in memory
                                        for (i = 0; i < p.Length; i++)
                                            p[i] = 0;
                                    }

                                    // try to clear
                                    for (i = 0; i < passphrase.Length; i++)
                                        passphrase[i] = '\0';
                                }
                            }
                            break;
                    
                    }
                    if (args.Equals("change_passwd.empty.okay"))
                    {
                        output = (passOptions.EmptyOkay) ? "Y" : "N";
                        return ToU8(output);
                    }

                    if (args.Equals("keyedit.prompt"))
                    {
                        output = "save";
                        return ToU8(output);
                    }

                }
            }

            return new byte[0];
        }

        public void ChangePassphrase(Context ctx, PgpPassphraseOptions options)
        {
            if (ctx == null)
                throw new ArgumentNullException("No context object supplied.");
            if (!ctx.IsValid)
                throw new InvalidContextException("An invalid context has been supplied.");

            if (ctx.HasPassphraseFunction)
                throw new InvalidContextException("The context must not have a passphrase callback function.");

            if (options == null)
                throw new ArgumentNullException("PgpPassphraseOptions object required.");

            lock (settings.passLock)
            {
                lock (settings.newpassLock)
                {
                    // specify key edit operation;
                    KeyEditOp op = KeyEditOp.Passphrase;
                    settings.passOptions = options;
                    
                    // reset object
                    options.passphraseSendCmd = false;
                    options.needoldpw = true;
                    options.missingpasswd = false;
                    options.aborthandler = false;
                    options.emptypasswdcount = 0;

                    // output data
                    GpgmeData data = new GpgmeMemoryData();

                    int err;
                    try
                    {
                        err = StartEdit(ctx, (IntPtr)op, data);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);

                    switch (errcode)
                    {
                        case gpg_err_code_t.GPG_ERR_NO_ERROR:
                            break;
                        case gpg_err_code_t.GPG_ERR_BAD_PASSPHRASE:
                            // This could be thrown if the user has chosen an empty password.
                            if (options.missingpasswd == true
                                && options.EmptyOkay == false 
                                && options.aborthandler == false
                                && (options.emptypasswdcount < PgpPassphraseOptions.MAX_PASSWD_COUNT))
                            {
                                break;
                            }
                            else
                                throw new BadPassphraseException(settings.passSettings.GetPassphraseInfo());
                        default:
                            if (options.missingpasswd && options.aborthandler)
                                throw new EmptyPassphraseException(settings.passSettings.GetPassphraseInfo());
                            else
                                throw new GpgmeException("An unknown error occurred. Error:"
                                    + err.ToString(), err);
                    }
                }
            }
        }

        private byte[] ToU8(string text)
        {
            return Gpgme.ConvertCharArrayToUTF8(text.ToCharArray(), 0);
        }

        public class PassphraseSettings
        {
            // variables used to save the last passphrase information (keyid etc.)
            internal string PassphraseUserIdHint = "";
            internal string PassphraseInfo = "";
            internal bool PassphrasePrevWasBad = false;
            
            // give the user the opportunity to cancel the passphrase dialogs completely
            internal PassphraseResult PassphraseLastResult = PassphraseResult.Success;

            internal PassphraseInfo GetPassphraseInfo()
            {
                return new PassphraseInfo(IntPtr.Zero,
                                    PassphraseUserIdHint,
                                    PassphraseInfo,
                                    PassphrasePrevWasBad);
            }

            public PassphraseDelegate PassphraseFunction = null;
            public char[] Passphrase = null;
        }

        public class Settings
        {
			private PgpKey key;
			
            // passphrase queries during key editing
            internal object passLock = new object();
            internal PassphraseSettings passSettings = new PassphraseSettings();

            // key signatures
            internal object sigLock = new object();
            internal PgpSignatureOptions sigOptions;

            // change passphrase
            internal object newpassLock = new object();
            internal PgpPassphraseOptions passOptions;

            // revoke signature
            internal object revsigLock = new object();
            internal PgpRevokeSignatureOptions revsigOptions;

            // delete signature
            internal object delsigLock = new object();
            internal PgpDeleteSignatureOptions delsigOptions;

            // enable/disable key
            internal object endisLock = new object();
            internal PgpEnableDisableOptions endisOptions;

            // trust
            internal object trustLock = new object();
            internal PgpTrustOptions trustOptions;

            // addkey
            internal object subkeyLock = new object();
            internal PgpSubkeyOptions subkeyOptions;
            internal AlgorithmCapability subkeycapability;
            internal int subkeyalgoquestion = 0;

            // expire
            internal object expireLock = new object();
            internal PgpExpirationOptions expireOptions;

            internal Settings(PgpKey key)
            {
                this.key = key;
            }

            public PassphraseDelegate PassphraseFunction
            {
                get
                {
                    lock (passLock)
                    {
                        return passSettings.PassphraseFunction;
                    }
                }
                set
                {
                    lock (passLock)
                    {
                        passSettings.PassphraseFunction = value;
                    }
                }
            }
            public char[] Passphrase
            {
                get
                {
                    lock (passLock)
                    {
                        return passSettings.Passphrase;
                    }
                }
                set
                {
                    lock (passLock)
                    {
                        passSettings.Passphrase = value;
                    }
                }
            }
        }
    }
}
