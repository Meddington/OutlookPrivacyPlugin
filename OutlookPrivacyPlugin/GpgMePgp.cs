//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace OutlookPrivacyPlugin
//{
//    class GpgMePgp
//    {
//        private string SignEmail(string data, string key)
//        {
//            return SignEmail(this._encoding.GetBytes(data), key);
//        }

//        private string SignEmail(byte[] data, string key)
//        {
//            Context ctx = new Context();
//            if (ctx.Protocol != Protocol.OpenPGP)
//                ctx.SetEngineInfo(Protocol.OpenPGP, null, null);

//            var keyring = ctx.KeyStore;
//            Key[] senderKeys = keyring.GetKeyList(key, false);
//            if (senderKeys == null || senderKeys.Length == 0)
//            {
//                MessageBox.Show(
//                    "Error, Unable to locate sender key \"" + key + "\".",
//                    "Outlook Privacy Error",
//                    MessageBoxButtons.OK,
//                    MessageBoxIcon.Error);

//                return null;
//            }

//            PgpKey senderKey = (PgpKey)senderKeys[0];
//            if (senderKey.Uid == null || senderKey.Fingerprint == null)
//            {
//                MessageBox.Show(
//                    "Error, Sender key appears to be corrupt \"" + key + "\".",
//                    "Outlook Privacy Error",
//                    MessageBoxButtons.OK,
//                    MessageBoxIcon.Error);

//                return null;
//            }

//            using (var sin = new MemoryStream(data))
//            using (var sout = new MemoryStream())
//            {
//                using (var origin = new GpgmeStreamData(sin))
//                using (var detachsig = new GpgmeStreamData(sout))
//                {

//                    ctx.Signers.Clear();
//                    ctx.Signers.Add(senderKey);
//                    ctx.Armor = true;

//                    try
//                    {
//                        var sigresult = ctx.Sign(origin, detachsig, SignatureMode.Detach);

//                        if (sigresult.InvalidSigners != null)
//                        {
//                            MessageBox.Show(
//                                "Error, invalid signers were found.",
//                                "Outlook Privacy Error",
//                                MessageBoxButtons.OK,
//                                MessageBoxIcon.Error);

//                            return null;
//                        }

//                        if (sigresult.Signatures == null)
//                        {
//                            MessageBox.Show(
//                                "Error, no signatures were found.",
//                                "Outlook Privacy Error",
//                                MessageBoxButtons.OK,
//                                MessageBoxIcon.Error);

//                            return null;
//                        }
//                    }
//                    catch (BadPassphraseException)
//                    {
//                        return null;
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show(
//                            "Error signing message: " + ex.Message,
//                            "Outlook Privacy Error",
//                            MessageBoxButtons.OK,
//                            MessageBoxIcon.Error);

//                        return null;
//                    }
//                }

//                sout.Position = 0;
//                using (var reader = new StreamReader(sout))
//                {
//                    return reader.ReadLine();
//                }
//            }
//        }

//        private string EncryptEmail(string mail, string passphrase, IList<string> recipients)
//        {
//            return EncryptEmail(this._encoding.GetBytes(mail), passphrase, recipients);
//        }

//        private string EncryptEmail(byte[] data, string passphrase, IList<string> recipients)
//        {
//            List<Key> encryptKeys = new List<Key>();

//            Context ctx = new Context();
//            if (ctx.Protocol != Protocol.OpenPGP)
//                ctx.SetEngineInfo(Protocol.OpenPGP, null, null);

//            var keyring = ctx.KeyStore;

//            foreach (var recip in recipients)
//            {
//                Key[] keys = keyring.GetKeyList(recip, false);
//                if (keys == null || keys.Length == 0)
//                {
//                    MessageBox.Show(
//                        "Error, Unable to locate recipient key \"" + recip + "\".",
//                        "Outlook Privacy Error",
//                        MessageBoxButtons.OK,
//                        MessageBoxIcon.Error);

//                    return null;
//                }

//                PgpKey key = (PgpKey)keys[0];
//                if (key.Uid == null || key.Fingerprint == null)
//                {
//                    MessageBox.Show(
//                        "Error, Recipient key appears to be corrupt \"" + recip + "\".",
//                        "Outlook Privacy Error",
//                        MessageBoxButtons.OK,
//                        MessageBoxIcon.Error);

//                    return null;
//                }

//                encryptKeys.Add(key);
//            }

//            using (var sin = new MemoryStream(data))
//            using (var sout = new MemoryStream())
//            {
//                using (var plain = new GpgmeStreamData(sin))
//                using (var cipher = new GpgmeStreamData(sout))
//                {
//                    ctx.Armor = true;

//                    try
//                    {
//                        var result = ctx.Encrypt(
//                            encryptKeys.ToArray(),
//                            _settings.GnuPgTrustModel ? EncryptFlags.None : EncryptFlags.AlwaysTrust,
//                            plain,
//                            cipher);

//                        if (result.InvalidRecipients != null)
//                        {
//                            MessageBox.Show(
//                                "Error, encryption failed, invalid recipients found.",
//                                "Outlook Privacy Error",
//                                MessageBoxButtons.OK,
//                                MessageBoxIcon.Error);

//                            return null;
//                        }
//                    }
//                    catch (BadPassphraseException)
//                    {
//                        return null;
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show(
//                            "Error, encryption failed:" + ex.Message,
//                            "Outlook Privacy Error",
//                            MessageBoxButtons.OK,
//                            MessageBoxIcon.Error);

//                        return null;
//                    }
//                }

//                sout.Position = 0;
//                string encryptedMessage;
//                using (var reader = new StreamReader(sout))
//                {
//                    encryptedMessage = reader.ReadToEnd();
//                }

//                return encryptedMessage.Replace("-----BEGIN PGP MESSAGE-----", "-----BEGIN PGP MESSAGE-----\nCharset: " + _encoding.WebName);
//            }
//        }

//        private string SignAndEncryptEmail(string data, string key, string passphrase, IList<string> recipients)
//        {
//            return SignAndEncryptEmail(this._encoding.GetBytes(data), key, passphrase, recipients);
//        }

//        private string SignAndEncryptEmail(byte[] data, string key, string passphrase, IList<string> recipients)
//        {
//            List<Key> encryptKeys = new List<Key>();

//            Context ctx = new Context();
//            if (ctx.Protocol != Protocol.OpenPGP)
//                ctx.SetEngineInfo(Protocol.OpenPGP, null, null);

//            var keyring = ctx.KeyStore;

//            foreach (var recip in recipients)
//            {
//                Key[] rkeys = keyring.GetKeyList(recip, false);
//                if (rkeys == null || rkeys.Length == 0)
//                {
//                    MessageBox.Show(
//                        "Error, Unable to locate recipient key \"" + recip + "\".",
//                        "Outlook Privacy Error",
//                        MessageBoxButtons.OK,
//                        MessageBoxIcon.Error);

//                    return null;
//                }

//                PgpKey rkey = (PgpKey)rkeys[0];
//                if (rkey.Uid == null || rkey.Fingerprint == null)
//                {
//                    MessageBox.Show(
//                        "Error, Recipient key appears to be corrupt \"" + recip + "\".",
//                        "Outlook Privacy Error",
//                        MessageBoxButtons.OK,
//                        MessageBoxIcon.Error);

//                    return null;
//                }

//                encryptKeys.Add(rkey);
//            }

//            Key[] senderKeys = keyring.GetKeyList(key, false);
//            if (senderKeys == null || senderKeys.Length == 0)
//            {
//                MessageBox.Show(
//                    "Error, Unable to locate sender key \"" + key + "\".",
//                    "Outlook Privacy Error",
//                    MessageBoxButtons.OK,
//                    MessageBoxIcon.Error);

//                return null;
//            }

//            PgpKey senderKey = (PgpKey)senderKeys[0];
//            if (senderKey.Uid == null || senderKey.Fingerprint == null)
//            {
//                MessageBox.Show(
//                    "Error, Sender key appears to be corrupt \"" + key + "\".",
//                    "Outlook Privacy Error",
//                    MessageBoxButtons.OK,
//                    MessageBoxIcon.Error);

//                return null;
//            }

//            using (var sin = new MemoryStream(data))
//            using (var sout = new MemoryStream())
//            {
//                using (var plain = new GpgmeStreamData(sin))
//                using (var cipher = new GpgmeStreamData(sout))
//                {
//                    ctx.Signers.Clear();
//                    ctx.Signers.Add(senderKey);
//                    ctx.Armor = true;

//                    try
//                    {
//                        var result = ctx.EncryptAndSign(
//                            encryptKeys.ToArray(),
//                            _settings.GnuPgTrustModel ? EncryptFlags.None : EncryptFlags.AlwaysTrust,
//                            plain,
//                            cipher);

//                        if (result.InvalidRecipients != null)
//                        {
//                            MessageBox.Show(
//                                "Error, encryption failed, invalid recipients found.",
//                                "Outlook Privacy Error",
//                                MessageBoxButtons.OK,
//                                MessageBoxIcon.Error);

//                            return null;
//                        }
//                    }
//                    catch (BadPassphraseException)
//                    {
//                        return null;
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show(
//                            "Error, encryption failed: " + ex.Message,
//                            "Outlook Privacy Error",
//                            MessageBoxButtons.OK,
//                            MessageBoxIcon.Error);

//                        return null;
//                    }
//                }

//                sout.Position = 0;
//                string encryptedMessage;
//                using (var reader = new StreamReader(sout))
//                {
//                    encryptedMessage = reader.ReadToEnd();
//                }

//                return encryptedMessage.Replace("-----BEGIN PGP MESSAGE-----", "-----BEGIN PGP MESSAGE-----\nCharset: " + _encoding.WebName);
//            }
//        }
//        internal void VerifyEmail(Outlook.MailItem mailItem)
//        {
//            string mail = mailItem.Body;
//            Outlook.OlBodyFormat mailType = mailItem.BodyFormat;

//            if (Regex.IsMatch(mailItem.Body, _pgpSignedHeader) == false)
//            {
//                MessageBox.Show(
//                    "Outlook Privacy cannot help here.",
//                    "Mail is not signed",
//                    MessageBoxButtons.OK,
//                    MessageBoxIcon.Exclamation);

//                return;
//            }

//            Context ctx = new Context();
//            if (ctx.Protocol != Protocol.OpenPGP)
//                ctx.SetEngineInfo(Protocol.OpenPGP, null, null);

//            var keyring = ctx.KeyStore;
//            Key[] senderKeys = null;

//            foreach (string toEmail in mailItem.To.Split(';'))
//            {
//                senderKeys = keyring.GetKeyList(toEmail, false);
//                if (senderKeys != null && senderKeys.Length > 0)
//                    break;
//            }

//            if (senderKeys == null || senderKeys.Length == 0)
//            {
//                MessageBox.Show(
//                    "Error, Unable to locate sender key in \"" + mailItem.To + "\".",
//                    "Outlook Privacy Error",
//                    MessageBoxButtons.OK,
//                    MessageBoxIcon.Error);

//                return;
//            }

//            PgpKey senderKey = (PgpKey)senderKeys[0];
//            if (senderKey.Uid == null || senderKey.Fingerprint == null)
//            {
//                MessageBox.Show(
//                    "Error, Sender key appears to be corrupt \"" + mailItem.To + "\".",
//                    "Outlook Privacy Error",
//                    MessageBoxButtons.OK,
//                    MessageBoxIcon.Error);

//                return;
//            }

//            using (var sin = new MemoryStream(this._encoding.GetBytes(mail)))
//            {
//                using (var origin = new GpgmeStreamData(sin))
//                {

//                    ctx.Signers.Clear();
//                    ctx.Signers.Add(senderKey);
//                    ctx.Armor = true;

//                    try
//                    {
//                        var result = ctx.Verify(null, origin, null);

//                        if (result.Signature == null || (result.Signature.Validity != Validity.Full &&
//                            result.Signature.Validity != Validity.Ultimate))
//                        {
//                            MessageBox.Show(
//                                "Error, signature could not be validated.",
//                                "Outlook Privacy Error",
//                                MessageBoxButtons.OK,
//                                MessageBoxIcon.Error);

//                            return;
//                        }
//                    }
//                    catch (BadPassphraseException)
//                    {
//                        MessageBox.Show(
//                            "Error, signature could not be validated due to bad passphrase.",
//                            "Outlook Privacy Error",
//                            MessageBoxButtons.OK,
//                            MessageBoxIcon.Error);

//                        return;
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show(
//                            "Error, signature could not be validated: " + ex.Message,
//                            "Outlook Privacy Error",
//                            MessageBoxButtons.OK,
//                            MessageBoxIcon.Error);

//                        return;
//                    }
//                }
//            }
//        }

//        internal void DecryptEmail(Outlook.MailItem mailItem)
//        {
//            if (Regex.IsMatch(mailItem.Body, _pgpEncryptedHeader) == false)
//            {
//                MessageBox.Show(
//                    "Outlook Privacy Plugin cannot help here.",
//                    "Mail is not encrypted",
//                    MessageBoxButtons.OK,
//                    MessageBoxIcon.Exclamation);
//                return;
//            }

//            // Sometimes messages could contain multiple message blocks.  In that case just use the 
//            // very first one.

//            string firstPgpBlock = mailItem.Body;
//            int endMessagePosition = firstPgpBlock.IndexOf("-----END PGP MESSAGE-----") + "-----END PGP MESSAGE-----".Length;
//            if (endMessagePosition != -1)
//                firstPgpBlock = firstPgpBlock.Substring(0, endMessagePosition);

//            string charset = null;
//            try
//            {
//                charset = Regex.Match(firstPgpBlock, @"Charset:\s+([^\s\r\n]+)").Groups[1].Value;
//            }
//            catch
//            {
//            }

//            // Set default encoding if charset was missing from 
//            // message.
//            if (string.IsNullOrWhiteSpace(charset))
//                charset = "ISO-8859-1";

//            var encoding = Encoding.GetEncoding(charset);

//            byte[] cleardata = DecryptAndVerify(mailItem.To, ASCIIEncoding.ASCII.GetBytes(firstPgpBlock));
//            if (cleardata != null)
//            {
//                mailItem.Body = encoding.GetString(cleardata);

//                // Don't HMTL encode or we will encode emails already in HTML format.
//                // Office has a safe html module they use to prevent security issues.
//                // Not encoding here should be no worse then reading a standard HTML
//                // email.
//                var html = encoding.GetString(cleardata);
//                html = html.Replace("\n", "<br/>");
//                mailItem.HTMLBody = "<html><body>" + html + "</body></html>";

//                // Decrypt all attachments
//                List<Microsoft.Office.Interop.Outlook.Attachment> mailAttachments = new List<Outlook.Attachment>();
//                foreach (Microsoft.Office.Interop.Outlook.Attachment attachment in mailItem.Attachments)
//                    mailAttachments.Add(attachment);

//                List<Attachment> attachments = new List<Attachment>();

//                foreach (var attachment in mailAttachments)
//                {
//                    //Outlook.PropertyAccessor pa = attachment.PropertyAccessor;
//                    //var value = pa.GetProperty("urn:schemas:mailheader");

//                    //var v = pa.GetProperties(new string[] { "urn:schemas:mailheader" });
//                    //var value = pa.GetProperty("content-id");
//                    //var value2 = pa.GetProperty("urn:schemas:mailheader:content-id");

//                    //MAPISession session = new MAPISession();
//                    //var stores = session.GetMessageStores();
//                    //session.OpenMessageStore(stores[0].Name);

//                    Attachment a = new Attachment();

//                    a.TempFile = Path.GetTempPath();
//                    a.FileName = Regex.Replace(attachment.FileName, @"\.(pgp\.asc|gpg\.asc|pgp|gpg|asc)$", "");
//                    a.DisplayName = attachment.DisplayName;
//                    a.AttachmentType = attachment.Type;

//                    a.TempFile = Path.Combine(a.TempFile, a.FileName);

//                    attachment.SaveAsFile(a.TempFile);
//                    //attachment.Delete();

//                    // Decrypt file
//                    var cyphertext = File.ReadAllBytes(a.TempFile);
//                    var plaintext = DecryptAndVerify(mailItem.To, cyphertext);

//                    File.WriteAllBytes(a.TempFile, plaintext);

//                    attachments.Add(a);
//                }

//                foreach (var attachment in attachments)
//                    mailItem.Attachments.Add(attachment.TempFile, attachment.AttachmentType, 1, attachment.FileName);

//                // Warning: Saving could save the message back to the server, not just locally
//                //mailItem.Save();
//            }
//        }
//        byte[] DecryptAndVerify(string to, byte[] data)
//        {
//            Context ctx = new Context();
//            if (ctx.Protocol != Protocol.OpenPGP)
//                ctx.SetEngineInfo(Protocol.OpenPGP, null, null);

//            var keyring = ctx.KeyStore;
//            Key[] senderKeys = null;
//            foreach (var toEmail in to.Split(';'))
//            {
//                senderKeys = keyring.GetKeyList(toEmail, false);
//                if (senderKeys != null && senderKeys.Length > 0)
//                    break;
//            }

//            if (senderKeys == null || senderKeys.Length == 0)
//            {
//                MessageBox.Show(
//                    "Error, Unable to locate sender key in \"" + to + "\".",
//                    "Outlook Privacy Error",
//                    MessageBoxButtons.OK,
//                    MessageBoxIcon.Error);

//                return null;
//            }

//            PgpKey senderKey = (PgpKey)senderKeys[0];
//            if (senderKey.Uid == null || senderKey.Fingerprint == null)
//            {
//                MessageBox.Show(
//                    "Error, Sender key appears to be corrupt \"" + to + "\".",
//                    "Outlook Privacy Error",
//                    MessageBoxButtons.OK,
//                    MessageBoxIcon.Error);

//                return null;
//            }

//            using (var sin = new MemoryStream(data))
//            using (var sout = new MemoryStream())
//            {
//                using (var cipher = new GpgmeStreamData(sin))
//                using (var plain = new GpgmeStreamData(sout))
//                {
//                    ctx.Signers.Clear();
//                    ctx.Signers.Add(senderKey);
//                    ctx.Armor = true;

//                    try
//                    {
//                        var result = ctx.DecryptAndVerify(cipher, plain);

//                        if (result.DecryptionResult.WrongKeyUsage)
//                        {
//                            MessageBox.Show(
//                                "Error, decryption failed due to wrong key usage.",
//                                "Outlook Privacy Error",
//                                MessageBoxButtons.OK,
//                                MessageBoxIcon.Error);

//                            return null;
//                        }

//                        if (result.DecryptionResult.UnsupportedAlgorithm != null)
//                        {
//                            MessageBox.Show(
//                                "Error, decryption failed due to unsupported algorithm.",
//                                "Outlook Privacy Error",
//                                MessageBoxButtons.OK,
//                                MessageBoxIcon.Error);

//                            return null;
//                        }

//                        if (result.VerificationResult.Signature != null &&
//                            (result.VerificationResult.Signature.Validity != Validity.Full &&
//                            result.VerificationResult.Signature.Validity != Validity.Ultimate))
//                        {
//                            MessageBox.Show(
//                                "Error, signature validation failed.",
//                                "Outlook Privacy Error",
//                                MessageBoxButtons.OK,
//                                MessageBoxIcon.Error);

//                            //return null;
//                        }
//                    }
//                    catch (DecryptionFailedException ex)
//                    {
//                        MessageBox.Show(
//                            "Error, decryption failed: " + ex.Message,
//                            "Outlook Privacy Error",
//                            MessageBoxButtons.OK,
//                            MessageBoxIcon.Error);

//                        return null;
//                    }
//                    catch (BadPassphraseException)
//                    {
//                        MessageBox.Show(
//                            "Error, decryption failed due to incorrect password.",
//                            "Outlook Privacy Error",
//                            MessageBoxButtons.OK,
//                            MessageBoxIcon.Error);

//                        return null;
//                    }
//                    catch (GeneralErrorException ex)
//                    {
//                        MessageBox.Show(
//                            "Error, decryption failed: " + ex.Message,
//                            "Outlook Privacy Error",
//                            MessageBoxButtons.OK,
//                            MessageBoxIcon.Error);

//                        return null;
//                    }
//                }

//                sout.Position = 0;
//                return sout.ToArray();
//            }
//        }
//    }
//}
