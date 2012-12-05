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
    /* Request a passphrase from the user.  */
    internal delegate int gpgme_passphrase_cb_t(
           IntPtr hook,
           IntPtr uid_hint,
           IntPtr passphrase_info,
           int prev_was_bad,
           int fd);

    /* Inform the user about progress made.  */
    internal delegate void gpgme_progress_cb_t(
        IntPtr opaque,
        IntPtr what,
        int type,
        int current,
        int total);

    /* Read up to SIZE bytes into buffer BUFFER from the data object with
       the handle HANDLE.  Return the number of characters read, 0 on EOF
       and -1 on error.  If an error occurs, errno is set.  */
    internal delegate IntPtr gpgme_data_read_cb_t(
        IntPtr handle,
        IntPtr buffer,
        UIntPtr size);

    /* Write up to SIZE bytes from buffer BUFFER to the data object with
       the handle HANDLE.  Return the number of characters written, or -1
       on error.  If an error occurs, errno is set.  */
    internal delegate IntPtr gpgme_data_write_cb_t(
        IntPtr handle,
        IntPtr buffer,
        UIntPtr size);

    /* Set the current position from where the next read or write starts
       in the data object with the handle HANDLE to OFFSET, relativ to
       WHENCE.  */
    internal delegate IntPtr gpgme_data_seek_cb_t(
        IntPtr handle,
        IntPtr offset,
        int whence);
    /* Set the current position from where the next read or write starts
       in the data object with the handle HANDLE to OFFSET, relativ to
       WHENCE.  */
    internal delegate long gpgme_data_seek_cb_t_lfs(
        IntPtr handle,
        long offset,
        int whence);
	
    /* Close the data object with the handle DL.  */
    internal delegate void gpgme_data_release_cb_t(
        IntPtr handle);

    /* Interact with the user about an edit operation.  */
    internal delegate int gpgme_edit_cb_t(
        IntPtr opaque,  // void *
        int status,     //gpgme_status_code_t
        IntPtr args,    // const char *
        int fd);
}
