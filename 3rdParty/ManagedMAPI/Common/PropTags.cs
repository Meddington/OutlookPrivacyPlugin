#region File Info
//
// File       : proptag.cs
// Description: PropTags enum
// Package    : ManagedMAPI
//
// Authors    : Fred Song
//
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagedMAPI
{
    public enum PropTags : uint
    {
        /// <summary>
        /// Contains the display name of the message store.
        /// <br></br>Data Type: PT_TSTRING
        /// </summary>
        PR_DISPLAY_NAME = PT.PT_TSTRING | 0x3001 << 16,

        /// <summary>
        /// Contains TRUE if a message store is the default message store in the message store table.
        /// <br></br>Data Type: PT_BOOLEAN
        /// </summary>
        PR_DEFAULT_STORE = PT.PT_BOOLEAN | 0x3400 << 16,

        /// <summary>
        ///  Contains a MAPI entry identifier used to open and edit properties of a particular MAPI object.
        ///  <br></br>Data Type: PT_BINARY
        /// </summary>
        PR_ENTRYID = PT.PT_BINARY | 0x0FFF << 16,

        /// <summary>
        /// Contains a value indicating the service provider type.
        /// <br></br>Data Type: PT_LONG
        /// </summary>
        PR_RESOURCE_TYPE = PT.PT_LONG | 0x3E03 << 16,
    }
  
    /// <summary>
    /// Property Type
    /// </summary>
    public enum PT : uint
    {
        /// <summary>
        /// (Reserved for interface use) type doesn't matter to caller
        /// </summary>
        PT_UNSPECIFIED = 0,
        /// <summary>
        /// NULL property value
        /// </summary>
        PT_NULL = 1,
        /// <summary>
        /// Signed 16-bit value
        /// </summary>
        PT_I2 = 2,
        /// <summary>
        /// Signed 32-bit value
        /// </summary>
        PT_LONG = 3,
        /// <summary>
        /// 4-byte floating point
        /// </summary>
        PT_R4 = 4,
        /// <summary>
        /// Floating point double
        /// </summary>
        PT_DOUBLE = 5,
        /// <summary>
        /// Signed 64-bit int (decimal w/ 4 digits right of decimal pt)
        /// </summary>
        PT_CURRENCY = 6,
        /// <summary>
        /// Application time
        /// </summary>
        PT_APPTIME = 7,
        /// <summary>
        /// 32-bit error value
        /// </summary>
        PT_ERROR = 10,
        /// <summary>
        /// 16-bit boolean (non-zero true,zero false)
        /// </summary>
        PT_BOOLEAN = 11,
        /// <summary>
        /// 16-bit boolean (non-zero true)
        /// </summary>
        PT_BOOLEAN_DESKTOP = 11,
        /// <summary>
        /// Embedded object in a property
        /// </summary>
        PT_OBJECT = 13,
        /// <summary>
        /// 8-byte signed integer
        /// </summary>
        PT_I8 = 20,
        /// <summary>
        /// Null terminated 8-bit character string
        /// </summary>
        PT_STRING8 = 30,
        /// <summary>
        /// Null terminated Unicode string
        /// </summary>
        PT_UNICODE = 31,
        /// <summary>
        /// FILETIME 64-bit int w/ number of 100ns periods since Jan 1,1601
        /// </summary>
        PT_SYSTIME = 64,
        /// <summary>
        /// OLE GUID
        /// </summary>
        PT_CLSID = 72,
        /// <summary>
        /// Uninterpreted (counted byte array)
        /// </summary>
        PT_BINARY = 258,
        /// <summary>
        /// IF MAPI Unicode, PT_TString is Unicode string; otheriwse 8-bit character string
        /// </summary>
        PT_TSTRING = PT_UNICODE
    }
}
