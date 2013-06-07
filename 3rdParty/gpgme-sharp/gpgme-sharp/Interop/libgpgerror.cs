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

namespace Libgpgme.Interop
{

    internal partial class libgpgerror
    {
        internal static int gpg_err_make(gpg_err_source_t source, gpg_err_code_t code)
        {
            return code == gpg_err_code_t.GPG_ERR_NO_ERROR 
                ? (int)gpg_err_code_t.GPG_ERR_NO_ERROR
                : ((((int)source & (int)Masks.GPG_ERR_SOURCE_MASK) << (int)Masks.GPG_ERR_SOURCE_SHIFT)
                | ((int)code & (int)Masks.GPG_ERR_CODE_MASK));
        }

        internal static gpg_err_code_t gpg_err_code(int err)
        {
            return (gpg_err_code_t)(err & (int)Masks.GPG_ERR_CODE_MASK);
        }

        internal static gpg_err_source_t gpg_err_source(int err)
        {
            return (gpg_err_source_t)((err >> (int)Masks.GPG_ERR_SOURCE_SHIFT)
                           & (int)Masks.GPG_ERR_SOURCE_MASK);
        }
    }
}
