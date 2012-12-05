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

namespace Libgpgme
{
    public class PassphraseInfo
    {
        internal IntPtr hook;
        public IntPtr Hook
        {
            get { return hook; }
        }
        internal string hintText;
        public string HintText
        {
            get { return hintText; }
        }
        internal string info;
        public string Info
        {
            get { return info; }
        }

        private string request_keyid;
        public string RequestKeyId
        {
            get { return request_keyid; }
        }
        
        private string main_keyid;
        public string MainKeyId
        {
            get { return main_keyid; }
        }

        private KeyAlgorithm keytype;
        public KeyAlgorithm PubkeyAlgorithm
        {
            get { return keytype; }
        }

        private int keylength;
        public int KeyLength
        {
            get { return keylength; }
        }

        private string uidkeyid;
        public string UidKeyId
        {
            get { return uidkeyid; }
        }

        private string uid;
        public string Uid
        {
            get { return uid; }
        }

        private bool prevwasbad;
        public bool PrevWasBad
        {
            get { return prevwasbad; }
        }

        internal PassphraseInfo(IntPtr hook, string hintText, string info, bool prevwasbad)
        {
            this.hook = hook;
            this.hintText = hintText;
            this.info = info;
            this.prevwasbad = prevwasbad;

            parsehintText();
            parseinfo();
        }

        private void parsehintText() 
        {
            if (hintText != null)
            {
                int firstspace = hintText.IndexOf(' ');
                if (firstspace > 0)
                {
                    uidkeyid = hintText.Substring(0, firstspace);
                    uid = hintText.Substring(
                        firstspace, 
                        hintText.Length - firstspace).TrimStart(new char[] {' '});
                }
            }
        }
        private void parseinfo()
        {
            if (info != null)
            {
                string[] token = info.Split(' ');
                int len = token.Length;

                if (len > 0)
                    request_keyid = token[0];
                if (len > 1)
                    main_keyid = token[1];
                if (len > 2)
                    try 
                    {
                        int ktype = int.Parse(token[2]);
                        keytype = (KeyAlgorithm)ktype;
                    } 
                    catch {}
                if (len > 3)
                    try
                    {
                        int ksize = int.Parse(token[3]);
                        keylength = ksize;
                    }
                    catch { }
            }
        }

    }
}
