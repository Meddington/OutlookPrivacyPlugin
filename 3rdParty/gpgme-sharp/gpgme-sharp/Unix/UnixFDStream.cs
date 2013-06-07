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
using System.Runtime.InteropServices;

namespace Libgpgme.Unix
{
	internal class UnixFDStream : Stream
	{
		[DllImport("libc", CharSet = CharSet.Ansi)]
		private extern static IntPtr read(
		             [In] int fd, 
		             [In] IntPtr buf, // byte[]
		             [In] UIntPtr count 
		             );
		
		[DllImport("libc", CharSet = CharSet.Ansi)]
		private extern static IntPtr write(
		              [In] int fd, 
		              [In] IntPtr buf, // byte[]
		              [In] UIntPtr count 
		              );

		
		private int fd;
        internal UnixFDStream(int fd)
		{
			this.fd = fd;
			if (fd <= 0)
				throw new System.IO.IOException("An invalid file descriptor has been specified.");
		}
        public override void Write(byte[] buffer, int offset, int count) 
		{
			
			int bufsize = count - offset;			

			if (bufsize == 0)
				return;
			
			if (bufsize > buffer.Length)
				throw new System.IO.IOException("The supplied buffer is less than the requested count of bytes to write.");
			
			/* It would be faster and more efficient to use unsafe code (pointers).
			 * But I do not want to compile a security related application with /unsafe :-)
			 */
			IntPtr buf = Marshal.AllocCoTaskMem(bufsize);
			
			if (buf != IntPtr.Zero)
			{
				Marshal.Copy(buffer, offset, buf, bufsize);
				
				// Mono.Unix.Native.Syscall.write(fd, buf, (ulong) bufsize);
		
				IntPtr written = write(fd, buf, (UIntPtr)bufsize);
                if (buf != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(buf);
                    buf = IntPtr.Zero;
                }

				if (written.Equals((IntPtr)(-1)))
					throw new System.IO.IOException("An error occurred while writing to file descriptor " + fd + ".");
				
				if (((int)written) != bufsize)
					throw new System.IO.IOException("Error: only " + (int)written + " byte(s) of " + bufsize + " have been written.");
			} else
				throw new System.IO.IOException("Could not allocate " + bufsize + " bytes from memory.");
		}
        public override bool CanWrite
        {
			get { return true; } 
		}

        public override void Flush() 
		{
			// nothing to do
		}

        public override int Read(byte[] buffer, int offset, int count) 
		{
			throw new System.NotSupportedException();
		}
        public override bool CanRead
        {
			get { return false; }
		}

        public override long Seek(long offset, SeekOrigin origin) 
		{
			throw new System.NotSupportedException();
		}
        public override bool CanSeek 
		{
			get { return false; }
		}

        public override void SetLength(long value)
        {
			throw new System.NotSupportedException();
		}

        public override long Length
        { 
			get { throw new System.NotSupportedException(); }
		}

        public override long Position
        { 
			set { throw new System.NotSupportedException(); } 
			get { throw new System.NotSupportedException(); }
		}
	}
}
