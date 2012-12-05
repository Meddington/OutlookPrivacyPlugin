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
    public sealed class GpgmeMemoryData : GpgmeData
    {
        private static object globallock = new object();
        
        private IntPtr memPtr;
        private UIntPtr memSize;
        private bool freeMem = false;

        ~GpgmeMemoryData()
        {
            ReleaseMemoryData();
        }
        public GpgmeMemoryData()
        {
            int err = libgpgme.gpgme_data_new(out dataPtr);
            gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);

            
            if (errcode == gpg_err_code_t.GPG_ERR_NO_ERROR)
            {
                // everything went fine
                return;
            }

            if (errcode == gpg_err_code_t.GPG_ERR_ENOMEM)
                throw new OutOfMemoryException("Not enough memory available to create GPGME data object.");

            throw new GeneralErrorException("Unknown error " + errcode + " (" + err + ")");
        }

		public GpgmeMemoryData(int size)
		{
			IntPtr tmpPtr = Marshal.AllocCoTaskMem(size);

            if (tmpPtr.Equals(IntPtr.Zero))
                throw new OutOfMemoryException();
            
            freeMem = true;
            
            InitGpgmeMemoryData(tmpPtr, size);
		}
		
		public GpgmeMemoryData(IntPtr memAddr, int size) 
		{
			freeMem = false;
			
			InitGpgmeMemoryData(memAddr, size);
		}

        private void InitGpgmeMemoryData(IntPtr memAddr, int size)
        {
            memPtr = memAddr;
            if (memPtr.Equals(IntPtr.Zero))
            {
            	memSize = UIntPtr.Zero;
                throw new ArgumentException("The supplied memory address was 0.");
            }
                
            memSize = (UIntPtr)size;

            /* If COPY is not zero, a private copy of the data is made. If COPY
               is zero, the data is taken from the specified buffer as needed,
               and the user has to ensure that the buffer remains valid for the
               whole life span of the data object. */
            int copy = 0; 
            int err = libgpgme.gpgme_data_new_from_mem(
                out dataPtr,
                memPtr,
                memSize,
                copy);

            gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);

            if (errcode == gpg_err_code_t.GPG_ERR_NO_ERROR)
            {
                // everything went fine
                return;
            }

            if (errcode == gpg_err_code_t.GPG_ERR_ENOMEM)
                throw new OutOfMemoryException("Not enough memory available to create GPGME data object.");

            throw new GeneralErrorException("Unknown error " + errcode + " (" + err + ")");
        }

        public GpgmeMemoryData(string filename)
            :this(filename, (long)0, (long)-1) { }
		public GpgmeMemoryData(string filename, int offset, int length)
			:this(filename, (long)offset, (long)length)	{}
        public GpgmeMemoryData(string filename, long offset, long length)
        {
            FileInfo finfo = new FileInfo(filename);
            if (!finfo.Exists)
                throw new FileNotFoundException("The supplied file could not be found.", filename);

            if (offset == 0 && length == -1)
                length = (int)finfo.Length;

            if (finfo.Length < (offset + length))
                throw new ArgumentException("The file size is smaller than file offset + length.");

            using (FileStream f = finfo.OpenRead())
            {
                if (!f.CanRead)
                    throw new FileLoadException("Cannot read file " + filename + ".", filename);
            }

            IntPtr pfilepath = Marshal.StringToCoTaskMemAnsi(filename);
            IntPtr handle = IntPtr.Zero;
            UIntPtr plen = (UIntPtr)length;
            
			int err = 0;
			if (libgpgme.use_lfs) 
			{
            	err = libgpgme.gpgme_data_new_from_filepart(
                	out dataPtr,
                	pfilepath,
                	handle,
                	offset,
                	plen);
			} else {

				IntPtr poffset = (IntPtr)offset;
            	err = libgpgme.gpgme_data_new_from_filepart(
                	out dataPtr,
                	pfilepath,
                	handle,
                	poffset,
                	plen);
			}

            gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);

            if (pfilepath != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pfilepath);
                pfilepath = IntPtr.Zero;
            }
            

            if (errcode == gpg_err_code_t.GPG_ERR_NO_ERROR)
            {
                // everything went fine, set default filename
                FileName = finfo.Name;

                return; 
            }

            if (errcode == gpg_err_code_t.GPG_ERR_ENOMEM)
                throw new OutOfMemoryException("Not enough memory available to create GPGME data object.");

            throw new GeneralErrorException("Unknown error " + errcode + " (" + err + ")");
        }

        private void ReleaseMemoryData()
        {
            lock (globallock)
            {
                if (!dataPtr.Equals(IntPtr.Zero))
                {
                    libgpgme.gpgme_data_release(dataPtr);
                    dataPtr = IntPtr.Zero;
                }
                if (!memPtr.Equals(IntPtr.Zero))
                {
                	if (freeMem)
                    	Marshal.FreeCoTaskMem(memPtr);
                    memPtr = IntPtr.Zero;
                    memSize = UIntPtr.Zero;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseMemoryData(); // forced in deconstructor anyway
                GC.SuppressFinalize(this);
            }
        }

        public override long Length
        {
            get 
            {
                if (!memPtr.Equals(IntPtr.Zero))
                    return (long)memSize;
                
                // save the current position
                long pos = Position;
                
                // read the current stream length
                long len = Seek(0, SeekOrigin.End);
                
                // restore old position
                Position = pos;

                return len;
            }
        }

        public override bool IsValid
        {
            get { return (!dataPtr.Equals(IntPtr.Zero)); }
        }

        public override void Flush() { }

        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanWrite
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return true; }
        }
        public IntPtr MemoryAddress
        {
        	get { return memPtr; }
       	}
       	public long MemorySize
       	{
       		get { return (long)memSize; }
       	}
    }
}
