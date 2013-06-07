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
    internal class _gpgme_import_status
    {
        public IntPtr next; //_gpgme_import_status

        /* Fingerprint.  */
        public IntPtr fpr; // char *

        /* If a problem occured, the reason why the key could not be
           imported.  Otherwise GPGME_No_Error.  */
        public int result; //gpgme_error_t

        /* The result of the import, the GPGME_IMPORT_* values bit-wise
           ORed.  0 means the key was already known and no new components
           have been added.  */
        public uint status;
		
		internal _gpgme_import_status() 
		{
			next 	= IntPtr.Zero;
			fpr 	= IntPtr.Zero;
			result 	= 0;
			status 	= 0;
		}
    }
}