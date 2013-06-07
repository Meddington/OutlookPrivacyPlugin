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
    /* The engine information structure.  */
    [StructLayout(LayoutKind.Sequential)]
    internal class _gpgme_engine_info //*gpgme_engine_info_t;
    {
        public IntPtr next;

        /* The protocol ID.  */
        public gpgme_protocol_t protocol;

        /* The file name of the engine binary.  */
        public IntPtr file_name;

        /* The version string of the installed engine.  */
        public IntPtr version;

        /* The minimum version required for GPGME.  */
        public IntPtr req_version;

        /* The home directory used, or NULL if default.  */
        public IntPtr home_dir;
		
		internal _gpgme_engine_info()
		{
			next 		= IntPtr.Zero;
			protocol 	= gpgme_protocol_t.GPGME_PROTOCOL_UNKNOWN;
			file_name 	= IntPtr.Zero;
			version 	= IntPtr.Zero;
			req_version = IntPtr.Zero;
			home_dir 	= IntPtr.Zero;
		}
    }
}

