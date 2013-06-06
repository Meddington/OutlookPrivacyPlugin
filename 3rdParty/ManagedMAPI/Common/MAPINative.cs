#region File Info
//
// File       : mapinative.cs
// Description: MAPI32 P/Invoke definitions
// Package    : ManagedMAPI
//
// Authors    : Fred Song
//
#endregion
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Win32;

namespace ManagedMAPI
{
    public class MAPINative
    {
        [DllImport("MAPI32.dll")]
        internal static extern HRESULT MAPIInitialize(IntPtr lpMapiInit);

        [DllImport("MAPI32.dll")]
        internal static extern void MAPIUninitialize();

        [DllImport("MAPI32.dll")]
        internal static extern int MAPILogonEx(uint ulUIParam, [MarshalAs(UnmanagedType.LPWStr)] string lpszProfileName,
                [MarshalAs(UnmanagedType.LPWStr)] string lpszPassword, uint flFlags, out IntPtr lppSession);

        [DllImport("MAPI32.dll")]
        internal static extern HRESULT MAPIFreeBuffer(IntPtr lpBuffer);
      
    }
}
