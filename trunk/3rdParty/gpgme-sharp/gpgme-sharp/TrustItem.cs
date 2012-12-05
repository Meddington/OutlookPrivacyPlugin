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
using Libgpgme.Interop;

namespace Libgpgme
{
    public class TrustItem
    {
        internal IntPtr itemPtr = IntPtr.Zero;
        private string keyid;
        public string KeyId
        {
            get { return keyid; }
        }
        private TrustItemType type;
        public TrustItemType Type
        {
            get { return type; }
        }
        private int level;
        public int Level
        {
            get { return level; }
        }
        private string owner_trust;
        public string OwnerTrust
        {
            get { return owner_trust; }
        }
        private string validity;
        public string Validity
        {
            get { return validity; }
        }
        private string name;
        public string Name
        {
            get { return name; }
        }

        ~TrustItem()
        {
            if (itemPtr != IntPtr.Zero)
            {
                // remove trust item reference
                libgpgme.gpgme_trust_item_unref(itemPtr);
                itemPtr = IntPtr.Zero;
            }
        }
      
        internal TrustItem(IntPtr itemPtr)
        {
            if (itemPtr.Equals(IntPtr.Zero))
                throw new InvalidPtrException("An invalid trust item pointer has been supplied.");
            
            UpdateFromMem(itemPtr);
        }

        private void UpdateFromMem(IntPtr itemPtr)
        {
            _gpgme_trust_item titem = new _gpgme_trust_item();
            Marshal.PtrToStructure(itemPtr, titem);

            keyid = Gpgme.PtrToStringAnsi(titem.keyid);
            switch (titem.type)
            {
                case 1:
                    type = TrustItemType.Key;
                    break;
                case 2:
                    type = TrustItemType.UserId;
                    break;
                default:
                    throw new GeneralErrorException("Unknown trust item type value of " + titem.type);
            }
            level = titem.level;
            owner_trust = Gpgme.PtrToStringUTF8(titem.owner_trust);
            validity = Gpgme.PtrToStringAnsi(titem.validity);
            name = Gpgme.PtrToStringUTF8(titem.name);
        }
    }
}
