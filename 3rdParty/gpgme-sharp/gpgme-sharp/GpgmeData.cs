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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Libgpgme.Interop;

namespace Libgpgme
{
    public abstract class GpgmeData: Stream
    {
        public const long EOF = 0;
        public const long ERROR = -1;

        protected const int SEEK_SET = 0; /* Seek from beginning of file. */
        protected const int SEEK_CUR = 1; /* Seek from current position. */
        protected const int SEEK_END = 2; /* Seek from end of file. */

        internal IntPtr dataPtr = IntPtr.Zero;

        public abstract bool IsValid { get; }

		internal GpgmeData() { }

        public int Read(byte[] buffer)
        {
            return Read(buffer, buffer.Length);
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!IsValid)
                throw new InvalidDataBufferException("The data buffer is invalid.");
            if (buffer == null)
                throw new ArgumentNullException("An empty destination buffer has been given.");
            if (buffer.Length < (offset + count))
                throw new ArgumentException("The sum of offset and count is bigger than the destination buffer size.");
            if (offset < 0 || count < 0)
                throw new ArgumentOutOfRangeException("Invalid / negative offset or count value supplied.");

            GCHandle pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            long memaddr = pinnedBuffer.AddrOfPinnedObject().ToInt64() + (long)offset;
            IntPtr memaddrPtr = (IntPtr)memaddr;

            UIntPtr size = (UIntPtr)count;
            IntPtr bytesRead = libgpgme.gpgme_data_read(
                dataPtr,
                memaddrPtr,
                size);

            pinnedBuffer.Free();
            return bytesRead.ToInt32();
        }
        public int Read(byte[] buffer, int count)
        {
            if (!IsValid)
                throw new InvalidDataBufferException("The data buffer is invalid.");
            if (buffer == null)
                throw new ArgumentNullException("An empty destination buffer has been given.");
            if (buffer.Length < count)
                throw new ArgumentException("Requested number of bytes to read is bigger than the destination buffer.");
            if (count < 0)
                throw new ArgumentOutOfRangeException("Negative read count value supplied.");

            UIntPtr bufsize = (UIntPtr)count;
            IntPtr bytesRead = libgpgme.gpgme_data_read(
                dataPtr,
                buffer,
                bufsize);

            return bytesRead.ToInt32();
        }

        public override long Seek(long offset, SeekOrigin whence)
        {
            if (!IsValid)
                throw new InvalidDataBufferException("Invalid data buffer.");

            int iwhence = SEEK_CUR;
            switch (whence)
            {
                case SeekOrigin.Begin:
                    iwhence = SEEK_SET;
                    break;
                case SeekOrigin.Current:
                    iwhence = SEEK_CUR;
                    break;
                case SeekOrigin.End:
                    iwhence = SEEK_END;
                    break;
            }
            
			if (libgpgme.use_lfs) 
			{
				return libgpgme.gpgme_data_seek(
					dataPtr,
				    offset,
				    iwhence);
				
			} 
			else 
			{
            	IntPtr poffset = (IntPtr)offset;
            	IntPtr offs = libgpgme.gpgme_data_seek(
                	dataPtr,
                	poffset,
                	iwhence);
            
            	return offs.ToInt64();
			}
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!IsValid)
                throw new InvalidDataBufferException("Invalid data buffer");
            if (buffer == null)
                throw new ArgumentNullException("Empty source buffer given.");
            if (buffer.Length < (offset + count))
                throw new ArgumentException("Requested number of bytes to write is bigger than the source buffer starting at offset " + offset.ToString()+ ".");
            if (offset < 0 || count < 0)
                throw new ArgumentOutOfRangeException("The offset or count is negative.");

            GCHandle pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            long memaddr = pinnedBuffer.AddrOfPinnedObject().ToInt64() + (long)offset;
            IntPtr memaddrPtr = (IntPtr)memaddr;

            UIntPtr bufsize = (UIntPtr)count;
            IntPtr bytesWritten = libgpgme.gpgme_data_write(
                dataPtr,
                memaddrPtr,
                bufsize);

            pinnedBuffer.Free();
            return;
        }

        public int Write(byte[] buffer, int count)
        {
            if (!IsValid)
                throw new InvalidDataBufferException("Invalid data buffer");
            if (buffer == null)
                throw new ArgumentNullException("An empty source buffer has been given.");
            if (buffer.Length < count)
                throw new ArgumentException("Requested number of bytes to write is bigger than the source buffer.");
            if (count < 0)
                throw new ArgumentOutOfRangeException("The read count is negative.");

            UIntPtr bufsize = (UIntPtr)count;
            IntPtr bytesWritten = libgpgme.gpgme_data_write(
                dataPtr,
                buffer,
                bufsize);

            return bytesWritten.ToInt32();
        }

        public override void SetLength(long value)
        {
            if (!IsValid)
                throw new InvalidDataBufferException("Invalid data buffer");

            throw new NotSupportedException("SetLength(long) is not supported.");
        }

        public string FileName
        {
            get
            {
                if (!IsValid)
                    throw new InvalidDataBufferException();
                IntPtr ptr = libgpgme.gpgme_data_get_file_name(dataPtr);
                if (!ptr.Equals(IntPtr.Zero))
                    return Gpgme.PtrToStringAnsi(ptr);
                else
                    return null;
            }
            set
            {
                if (!IsValid)
                    throw new InvalidDataBufferException();
                if (value == null)
                    throw new ArgumentNullException("Invalid file path.");

                IntPtr ptr = Marshal.StringToCoTaskMemAnsi(value);
                if (!ptr.Equals(IntPtr.Zero))
                {
                    int err = libgpgme.gpgme_data_set_file_name(dataPtr, ptr);
                    gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);
                    if (ptr != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(ptr);
                        ptr = IntPtr.Zero;
                    }
                    if (errcode != gpg_err_code_t.GPG_ERR_NO_ERROR)
                        if (errcode == gpg_err_code_t.GPG_ERR_ENOMEM)
                            throw new OutOfMemoryException();
                        else
                            throw new GeneralErrorException("Unknown error " + errcode + " (" + err + ")");
                }
                else
                    throw new OutOfMemoryException();
            }
        }
        public DataEncoding Encoding
        {
            get
            {
                if (!IsValid)
                    throw new InvalidDataBufferException();

                gpgme_data_encoding_t enc = libgpgme.gpgme_data_get_encoding(dataPtr);
                
                return (DataEncoding)enc;
            }
            set
            {
                if (!IsValid)
                    throw new InvalidDataBufferException();

                gpgme_data_encoding_t enc = (gpgme_data_encoding_t)value;

                int err = libgpgme.gpgme_data_set_encoding(dataPtr, enc);
                gpg_err_code_t errcode = libgpgerror.gpg_err_code(err);
                if (errcode != gpg_err_code_t.GPG_ERR_NO_ERROR)
                    throw new GeneralErrorException("Could not set data encoding to " + value);
            }
        }
        public void Rewind()
        {
            Seek(0, SeekOrigin.Begin);
        }

        public override long Position
        {
            get
            {
                if (!CanSeek)
                    throw new System.NotSupportedException();
                else
                    return Seek(0, SeekOrigin.Current);
            }
            set
            {
                if (!CanSeek)
                    throw new System.NotSupportedException();
                else
                    Seek(value, SeekOrigin.Begin);
            }
        }
		
#if (VERBOSE_DEBUG)
		internal void DebugOutput(string text)
        {
            Console.WriteLine("Debug: " + text);
			Console.Out.Flush();
        }
#endif
    }
}
