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
    public class GpgmeStreamData : GpgmeCbsData, IDisposable
    {
        protected Stream iostream = null;

        ~GpgmeStreamData()
        {
            CleanUp();
        }

        // give sub classes the possibility to set "iostream" by themself
        protected GpgmeStreamData() {}
        protected GpgmeStreamData(bool canRead, bool canWrite, bool canSeek, bool canRelease) 
            : base(canRead, canWrite, canSeek, canRelease)
        { }

        public GpgmeStreamData(Stream streamobj)
            : base(streamobj.CanRead, streamobj.CanWrite, streamobj.CanSeek, false)
        {
            iostream = streamobj;
            if (streamobj == null)
                throw new ArgumentNullException();
        }

        protected override IntPtr ReadCB(IntPtr bufPtr, long size)
        {
            if (iostream != null && iostream.CanRead)
            {
                byte[] buf = new byte[(int)size];
                int bytesRead = iostream.Read(buf, 0, (int)size);
                if (bytesRead > 0)
                    Marshal.Copy(buf, 0, bufPtr, bytesRead);
                
                return (IntPtr)bytesRead;
            }
            else
                return (IntPtr)ERROR;
        }
        protected override IntPtr WriteCB(IntPtr bufPtr, long size)
        {
            if (iostream != null && iostream.CanWrite)
            {
                long oldpos = iostream.Position;
                byte[] buf = new byte[(int)size];
                if (size > 0)
                    Marshal.Copy(bufPtr, buf, 0, (int)size);
                
                iostream.Write(buf, 0, (int)size);

                return (IntPtr)(iostream.Position - oldpos);
            }
            else
                return (IntPtr)ERROR;
        }
        protected override long SeekCB(long offset, SeekOrigin whence)
        {
            if (iostream != null && iostream.CanSeek)
            {
                return iostream.Seek(offset, whence);
            }
            else
                return ERROR;
        }
		protected override void ReleaseCB ()
		{
			//
		}

        private void CleanUp() 
        {
            if (iostream != null)
                iostream = null;
        }

        protected override void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            CleanUp();

            base.Dispose(disposing);
        }

        public override bool IsValid
        {
            get
            {
                return (iostream != null && 
                    (!dataPtr.Equals(IntPtr.Zero)));
            }
        }
        public override void Flush()
        {
            if (iostream != null)
                iostream.Flush();
            else
                throw new IOException();
        }

        public override void SetLength(long value)
        {
            if (iostream != null)
                iostream.SetLength(value);
            else
                throw new ObjectDisposedException("iostream");
            
            return;
        }
        public override long Length
        {
            get
            {
                if (iostream != null)
                    return iostream.Length;
                else
                    throw new ObjectDisposedException("iostream");
            }
        }

        public override bool CanRead
        {
            get
            {
                if (iostream != null)
                    return iostream.CanRead;
                else
                    return false;
            }
        }
        public override bool CanSeek
        {
            get
            {
                if (iostream != null)
                    return iostream.CanSeek;
                else
                    return false;
            }
        }
        public override bool CanRelease
        {
            get { return true; }
        }
        public override bool CanWrite
        {
            get
            {
                if (iostream != null)
                    return iostream.CanWrite;
                else
                    return false;
            }
        }
        public override bool CanTimeout
        {
            get
            {
                if (iostream != null)
                    return iostream.CanTimeout;
                else
                    return false;
            }
        }
        public Stream OriginStream 
        {
        	get
        	{
        		return iostream;
        	}
        	protected set
        	{
        		iostream = value;
        	}
        }
    }
}
