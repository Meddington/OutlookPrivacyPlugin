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

namespace Libgpgme
{
    public class GpgmeFileData: GpgmeStreamData
    {
        public GpgmeFileData(string filename)
            :this(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None){}

        public GpgmeFileData(string filename, FileMode mode)
            :this (filename, mode, FileAccess.ReadWrite, FileShare.None) {}

        public GpgmeFileData(string filename, FileMode mode, FileAccess access)
            : this(filename, mode, access, FileShare.None) { }

        public GpgmeFileData(string filename, FileMode mode, FileAccess access, FileShare share)
            :base(
            (access == FileAccess.Read || access == FileAccess.ReadWrite),  // CanRead
            (access == FileAccess.ReadWrite || access == FileAccess.Write), // CanWrite
            (access == FileAccess.Read || access == FileAccess.ReadWrite),  // CanSeek
            false)   // CanRelease
        {
            FileInfo finfo = new FileInfo(filename);
            iostream = (Stream)finfo.Open(mode, access, share);

            // set default filename 
            FileName = finfo.Name;
        }
        public override void Close()
        {
            if (iostream != null)
                ((FileStream)(iostream)).Close();
                
            base.Close();
        }
    }
}
