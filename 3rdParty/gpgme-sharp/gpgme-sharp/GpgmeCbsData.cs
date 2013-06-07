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
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using Libgpgme.Interop;

namespace Libgpgme
{
    public abstract class GpgmeCbsData: GpgmeData, IDisposable
    {
        private static IntPtr globalhandle = IntPtr.Zero;
        private static object globallock = new object();
        private object locallock = new object();

        private IntPtr handle;

        private _gpgme_data_cbs cbs;
		// See GPGME manual: 2.3 Largefile Support (LFS)
		private _gpgme_data_cbs_lfs cbs_lfs;
		
		private IntPtr cbsPtr;
		private GCHandle pinnedCbs;
		private GCHandle pinnedCbs_lfs;

        private bool releaseCBfuncInit = false;
        private ManualResetEvent releaseCBevent = new ManualResetEvent(false);

        public Exception LastCallbackException;

        private IntPtr IncGlobalHandle()
        {
            lock (globallock)
            {
                long value = globalhandle.ToInt64();
                ++value;
                globalhandle = (IntPtr)value;
                return globalhandle;
            }
        }

        ~GpgmeCbsData()
        {
            ReleaseCbsData();            
        }

        private void ReleaseCbsData()
        {
            if (!dataPtr.Equals(IntPtr.Zero) && !releaseCBfuncInit)
            {
                libgpgme.gpgme_data_release(dataPtr);
                
				if (libgpgme.use_lfs) {
					if (cbs_lfs.release != null)
                	{
                    	releaseCBfuncInit = true;
                    	// wait until libgpgme has called the _release callback method
                    	releaseCBevent.WaitOne();
                	}
				} else {
					if (cbs.release != null)
                	{
                    	releaseCBfuncInit = true;
                    	// wait until libgpgme has called the _release callback method
                    	releaseCBevent.WaitOne();
                	}
				}

                dataPtr = IntPtr.Zero;
            }
            lock (locallock)
            {
                if (!cbsPtr.Equals(IntPtr.Zero))
                {
                    Marshal.FreeCoTaskMem(cbsPtr);
                    cbsPtr = IntPtr.Zero;
                }
                if (pinnedCbs.IsAllocated)
                {
                    pinnedCbs.Free();
                }
				if (pinnedCbs_lfs.IsAllocated)
				{
					pinnedCbs_lfs.Free();
				}
            }
        }

        protected override void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            ReleaseCbsData();

            base.Dispose(disposing);
        }

        protected GpgmeCbsData()
        {
            // Inherited class overrides CanRead etc.
            Init(CanRead, CanWrite, CanSeek, CanRelease);
        }
        protected GpgmeCbsData(bool canRead, bool canWrite, bool canSeek, bool canRelease)
        {
            // The user specifies the implemented callback functions directly.
            Init(canRead, canWrite, canSeek, canRelease);
        }

        private void Init(bool canRead, bool canWrite, bool canSeek, bool canRelease)
        {
#if (VERBOSE_DEBUG)
			DebugOutput("GpgmeCbsData.Init(" + canRead.ToString() + "," 
			            + canWrite.ToString() + ","
			            + canSeek.ToString() + ","
			            + canRelease.ToString() + ")");
#endif	
            handle = IncGlobalHandle(); // increment the global handle 
            
            cbs = new _gpgme_data_cbs();
			cbs_lfs = new _gpgme_data_cbs_lfs();

            // Read function
            if (canRead)
			{
                cbs.read = new gpgme_data_read_cb_t(_read_cb);
				cbs_lfs.read = new gpgme_data_read_cb_t(_read_cb);
			}
            else
			{
                cbs.read = null;
				cbs_lfs.read = null;
			}
            
            // Write function
            if (canWrite)
			{
                cbs.write = new gpgme_data_write_cb_t(_write_cb);
				cbs_lfs.write = new gpgme_data_write_cb_t(_write_cb);
			}
            else
			{
                cbs.write = null;
				cbs_lfs.write = null;
			}
            
            // Seek function
            if (canSeek)
			{
                cbs.seek = new gpgme_data_seek_cb_t(_seek_cb);
				cbs_lfs.seek = new gpgme_data_seek_cb_t_lfs(_seek_cb_lfs);
			}
            else
			{
                cbs.seek = null;
				cbs_lfs.seek = null;
			}
            
            // Release
            if (canRelease)
			{
                cbs.release = new gpgme_data_release_cb_t(_release_cb);
				cbs_lfs.release = new gpgme_data_release_cb_t(_release_cb);
			}
            else
			{
                cbs.release = null;
				cbs_lfs.release = null;
			}
			
			pinnedCbs = GCHandle.Alloc(cbs);
			pinnedCbs_lfs = GCHandle.Alloc(cbs_lfs);
			if (libgpgme.use_lfs)
			{
				cbsPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cbs_lfs));
				Marshal.StructureToPtr(cbs_lfs, cbsPtr, false);
			} else {
				cbsPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cbs));
				Marshal.StructureToPtr(cbs, cbsPtr, false);
			}
			
            int err = libgpgme.gpgme_data_new_from_cbs(
                out dataPtr,
                cbsPtr,
                handle);

#if (VERBOSE_DEBUG)
			DebugOutput("gpgme_data_new_from_cbs(..) DONE.");
#endif	
            gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);

            if (errcode == gpg_err_code_t.GPG_ERR_NO_ERROR)
                return;

            if (errcode == gpg_err_code_t.GPG_ERR_ENOMEM)
                throw new OutOfMemoryException("Not enough memory available to create user defined GPGME data object.");

            throw new GeneralErrorException("Unknown error " + errcode + " (" + err + ")");
        }

        private IntPtr _read_cb(IntPtr handle, IntPtr buffer, UIntPtr size)
        {
#if (VERBOSE_DEBUG)
			DebugOutput("_read_cb(..)");
#endif	
            if (this.handle.Equals(handle))
                try
                {
                    return ReadCB(buffer, (long)size);
                }
                catch (Exception ex)
                {
                    LastCallbackException = ex;
                }
                  

            return (IntPtr)ERROR;
        }
        protected virtual IntPtr ReadCB(IntPtr bufPtr, long size)
        {
            throw new System.NotSupportedException("The read callback function 'ReadCB' is not implemented.");
        }

        private IntPtr _write_cb(IntPtr handle, IntPtr buffer, UIntPtr size)
        {
#if (VERBOSE_DEBUG)
			DebugOutput("_write_cb(..)");
#endif	
            if (this.handle.Equals(handle))
                try
                {
                    return WriteCB(buffer, (long)size);
                }
                catch (Exception ex) 
                {
                    LastCallbackException = ex;
                }

            return (IntPtr)ERROR;
        }
        protected virtual IntPtr WriteCB(IntPtr bufPtr, long size)
        {
            throw new System.NotSupportedException("The write callback function 'WriteCB' is not implemented.");
        }

        private IntPtr _seek_cb(IntPtr handle, IntPtr offset, int whence)
        {
#if (VERBOSE_DEBUG)
			DebugOutput("_seek_cb(..)");
#endif	
            if (this.handle.Equals(handle))
            {
                SeekOrigin sorigin = SeekOrigin.Current;
                switch (whence)
                {
                    case SEEK_SET:
                        sorigin = SeekOrigin.Begin;
                        break;
                    case SEEK_CUR:
                        sorigin = SeekOrigin.Current;
                        break;
                    case SEEK_END:
                        sorigin = SeekOrigin.End;
                        break;
                }
                try
                {
                    return (IntPtr)SeekCB((long)offset, sorigin);
                }
                catch (Exception ex)
                { 
                    LastCallbackException = ex; 
                }
            }
            return (IntPtr)ERROR;
        }

		// LFS Hack
        private long _seek_cb_lfs(IntPtr handle, long offset, int whence)
        {
#if (VERBOSE_DEBUG)
			DebugOutput("_seek_cb_lfs(..)");
#endif	
            if (this.handle.Equals(handle))
            {
                SeekOrigin sorigin = SeekOrigin.Current;
                switch (whence)
                {
                    case SEEK_SET:
                        sorigin = SeekOrigin.Begin;
                        break;
                    case SEEK_CUR:
                        sorigin = SeekOrigin.Current;
                        break;
                    case SEEK_END:
                        sorigin = SeekOrigin.End;
                        break;
                }
                try
                {
                    return SeekCB(offset, sorigin);
                }
                catch (Exception ex)
                { 
                    LastCallbackException = ex; 
                }
            }
            return ERROR;
        }

        protected virtual long SeekCB(long offset, SeekOrigin whence)
        {
            throw new System.NotSupportedException("The seek callback function 'SeekCB' is not implemented.");
        }

        private void _release_cb(IntPtr handle)
        {
            if (this.handle.Equals(handle))
            {
                try
                {
                    ReleaseCB();
                }
                catch (Exception ex)
                {
                    LastCallbackException = ex;
                }

                // cbs structure can be freed in memory now
                releaseCBfuncInit = false;
                releaseCBevent.Set();
            }
        }
        protected virtual void ReleaseCB()
        {
            throw new System.NotSupportedException("The release callback function 'ReleaseCB' is not implemented.");
        }

        public override abstract bool CanRead { get; }
        public override abstract bool CanWrite { get; }
        public override abstract bool CanSeek { get; }
        public abstract bool CanRelease { get; }

    }
}
