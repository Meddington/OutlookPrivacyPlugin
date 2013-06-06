#region File Info
//
// File       : sbinary.cs
// Description: SBinary struct
// Package    : ManagedMAPI
//
// Authors    : Fred Song
//
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ManagedMAPI
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SBinary
    {
        public uint cb;
        public IntPtr lpb;

        public byte[] AsBytes
        {
            get
            {
                byte[] b = new byte[cb];
                for (int i = 0; i < cb; i++)
                    b[i] = Marshal.ReadByte(lpb, i);
                return b;
            }
        }

        public static SBinary SBinaryCreate(byte[] data)
        {
            SBinary b;
            b.cb = (uint)data.Length;
            b.lpb = Marshal.AllocHGlobal((int)b.cb);
            for (int i = 0; i < b.cb; i++)
                Marshal.WriteByte(b.lpb, i, data[i]);
            return b;
        }

        public static void SBinaryRelease(ref SBinary b)
        {
            if (b.lpb != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(b.lpb);
                b.lpb = IntPtr.Zero;
                b.cb = 0;
            }
        }

    }
}
