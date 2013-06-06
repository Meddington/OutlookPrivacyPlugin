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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using Libgpgme.Interop;

namespace Libgpgme
{
    public class EngineInfo: IEnumerable<EngineInfo>
    {
        private Context ctx;

        private Protocol protocol;
        private string homedir;
        private string filename;
        private string version;
        private string reqversion;
        private EngineInfo next;

        internal EngineInfo(Context ctx, IntPtr enginePtr)
        {
            this.ctx = ctx;

            if (enginePtr == (IntPtr)0)
                throw new InvalidPtrException("An invalid EngineInfo pointer has been given." +
                    " Bad programmer! *spank* *spank*");

            UpdateFromMem(enginePtr);
        }
        internal EngineInfo(IntPtr enginePtr)
            : this(null, enginePtr) { }

        internal void UpdateFromMem(IntPtr enginePtr)
        {
            _gpgme_engine_info engine = (_gpgme_engine_info)
                Marshal.PtrToStructure(enginePtr,
                    typeof(_gpgme_engine_info));

            this.protocol = (Protocol)engine.protocol;

            if (engine.home_dir != (IntPtr)0)
                this.homedir = PtrToStringAnsi(engine.home_dir);
            else
                this.homedir = null;

            if (engine.file_name != (IntPtr)0)
                this.filename = PtrToStringAnsi(engine.file_name);
            else
                this.filename = null;

            if (engine.version != (IntPtr)0)
                this.version = PtrToStringAnsi(engine.version);
            else
                this.version = null;

            if (engine.req_version != (IntPtr)0)
                this.reqversion = PtrToStringAnsi(engine.req_version);
            else
                this.reqversion = null;

            if (engine.next != (IntPtr)0)
                next = new EngineInfo(ctx, engine.next);
            else
                next = null;
        }

        public Protocol Protocol
        {
            get
            {
                if (CtxValid || (!HasCtx))
                {
                    return this.protocol;
                }
                else
                    throw new InvalidContextException();
            }
            set
            {
                if (CtxValid)
                {
                    ctx.SetEngineInfo(value, this.filename, this.homedir);
                    this.protocol = value;
                }
                else
                    throw new InvalidContextException();
            }
        }

        public string HomeDir
        {
            get
            {
                if (CtxValid || (!HasCtx))
                {
                    return this.homedir;
                }
                else
                    throw new InvalidContextException();
            }
            set
            {
                if (CtxValid)
                {
                    ctx.SetEngineInfo(this.protocol, this.filename, value);
                    this.homedir = value;
                }
                else
                    throw new InvalidContextException();
            }
        }

        public string FileName
        {
            get
            {
                if (CtxValid || (!HasCtx))
                {
                    return this.filename;
                }
                else
                    throw new InvalidContextException();
            }
            set
            {
                if (CtxValid)
                {
                    ctx.SetEngineInfo(this.protocol, value, this.homedir);
                    this.filename = value;
                }
                else
                    throw new InvalidContextException();
            }
        }

        public string Version
        {
            get
            {
                if (CtxValid || (!HasCtx))
                {
                    return this.version;
                }
                else
                    throw new InvalidContextException();
            }
        }

        public string ReqVersion
        {
            get
            {
                if (CtxValid || (!HasCtx))
                {
                    return this.reqversion;
                }
                else
                    throw new InvalidContextException();
            }
        }

        private string PtrToStringAnsi(IntPtr ptr)
        {
            string tmp;
            tmp = Marshal.PtrToStringAnsi(ptr);
            return tmp.Replace("\r", "");
        }

        public bool CtxValid
        {
            // If the context invalides the engine object(s) invailde(s) as well.
            get
            {
                if (HasCtx)
                    return ctx.IsValid;
                else
                    return false;
            }
        }
        
        public bool HasCtx
        {
            get { return (ctx != null); }
        }

        public EngineInfo Next
        {
            get
            {
                if (CtxValid || (!HasCtx))
                {
                    return next;
                }
                else
                    throw new InvalidContextException();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public IEnumerator<EngineInfo> GetEnumerator()
        {
            EngineInfo info = this;
            while (info != null)
            {
                yield return info;
                info = info.Next;
            }
        }
    }
}
