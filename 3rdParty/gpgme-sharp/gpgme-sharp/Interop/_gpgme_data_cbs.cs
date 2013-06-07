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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Libgpgme.Interop
{
	[StructLayout(LayoutKind.Sequential)]
    internal class _gpgme_data_cbs 
    {
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public gpgme_data_read_cb_t read;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public gpgme_data_write_cb_t write;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public gpgme_data_seek_cb_t seek;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public gpgme_data_release_cb_t release;
		
		internal _gpgme_data_cbs() 
		{
			read    = null;
			write   = null;
			seek    = null;
			release = null;
		}
    }

	[StructLayout(LayoutKind.Sequential)]
    internal class _gpgme_data_cbs_lfs
    {
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public gpgme_data_read_cb_t read;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public gpgme_data_write_cb_t write;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public gpgme_data_seek_cb_t_lfs seek;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public gpgme_data_release_cb_t release;
		
		internal _gpgme_data_cbs_lfs() 
		{
			read    = null;
			write   = null;
			seek    = null;
			release = null;
		}
    }
}
