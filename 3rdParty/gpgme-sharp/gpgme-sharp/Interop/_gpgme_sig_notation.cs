/*
 * libgpgme-sharp - .NET wrapper classes for libgpgme (GnuPG Made Easy)
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

namespace Libgpgme.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal class _gpgme_sig_notation // *gpgme_sig_notation_t
    {
        public IntPtr next;

        /* If NAME is a null pointer, then VALUE contains a policy URL
           rather than a notation.  */
        public IntPtr name; // char*

        /* The value of the notation data.  */
        public IntPtr value; // char*

        /* The length of the name of the notation data.  */
        public int name_len;

        /* The length of the value of the notation data.  */
        public int value_len;

        /* The accumulated flags.  */
        public gpgme_sig_notation_flags_t flags;

        /* Notation data is human-readable.  
                uint human_readable : 1;
           Notation data is critical.  
                uint critical : 1;
           Internal to GPGME, do not use.  
                int _unused : 30;
         */
        public uint additionalflags;

        public bool human_readable
        {
            get { return ((additionalflags & 1) > 0); }
            set
            {
                if (value)
                    additionalflags |= 1;
                else
                    additionalflags &= (~(uint)1);
            }
        }
        public bool critical
        {
            get { return ((additionalflags & 2) > 0); }
            set
            {
                if (value)
                    additionalflags |= 2;
                else
                    additionalflags &= (~(uint)2);
            }
        }
    }
}
