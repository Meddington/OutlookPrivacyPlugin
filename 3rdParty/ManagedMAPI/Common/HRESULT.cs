#region File Info
//
// File       : hresult.cs
// Description: HRESULT enum
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
    /// <summary>
    /// The HRESULT data type is a 32-bit value is used to describe an error or warning.
    /// </summary>
    public enum HRESULT : uint
    {
        /// <summary>
        /// False
        /// </summary>
        S_FALSE = 0x00000001,
        /// <summary>
        /// OK
        /// </summary>
        S_OK = 0x00000000,
        /// <summary>
        /// Not implemented
        /// </summary>
        E_NOTIMPL = 0x80004001,
        /// <summary>
        /// Call failed
        /// </summary>
        MAPI_E_CALL_FAILED = 0x80004005,
        /// <summary>
        /// Not enough memory
        /// </summary>
        MAPI_E_NOT_ENOUGH_MEMORY = 0x8007000E,
        /// <summary>
        /// Invalid parameter
        /// </summary>
        MAPI_E_INVALID_PARAMETER = 0x80000003,
        /// <summary>
        /// Interface not supported
        /// </summary>
        MAPI_E_INTERFACE_NOT_SUPPORTED = 0x80000004,
        /// <summary>
        /// No access
        /// </summary>
        MAPI_E_NO_ACCESS = 0x80000009,
        /// <summary>
        /// Invalid argument
        /// </summary>
        E_INVALIDARG = 0x80070057,
        /// <summary>
        /// Out of memory
        /// </summary>
        E_OUTOFMEMORY = 0x80000002,
        /// <summary>
        /// Unexpected
        /// </summary>
        E_UNEXPECTED = 0x8000FFFF,
        /// <summary>
        /// Fail
        /// </summary>
        E_FAIL = 0x80000008,
        /// <summary>
        /// No support
        /// </summary>
        MAPI_E_NO_SUPPORT = 0x80040000 | 0x102,
        /// <summary>
        /// Bar char width
        /// </summary>
        MAPI_E_BAD_CHARWIDTH = 0x80040000 | 0x103,
        /// <summary>
        /// String too long
        /// </summary>
        MAPI_E_STRING_TOO_LONG = 0x80040000 | 0x105,
        /// <summary>
        /// Unknown flags
        /// </summary>
        MAPI_E_UNKNOWN_FLAGS = 0x80040000 | 0x106,
        /// <summary>
        /// Invalid entry ID
        /// </summary>
        MAPI_E_INVALID_ENTRYID = 0x80040000 | 0x107,
        /// <summary>
        /// Invalid object
        /// </summary>
        MAPI_E_INVALID_OBJECT = 0x80040000 | 0x108,
        /// <summary>
        /// Object changed
        /// </summary>
        MAPI_E_OBJECT_CHANGED = 0x80040000 | 0x109,
        /// <summary>
        /// Object deleted
        /// </summary>
        MAPI_E_OBJECT_DELETED = 0x80040000 | 0x10A,
        /// <summary>
        /// Busy
        /// </summary>
        MAPI_E_BUSY = 0x80040000 | 0x10B,
        /// <summary>
        /// Not enough disk
        /// </summary>
        MAPI_E_NOT_ENOUGH_DISK = 0x80040000 | 0x10D,
        /// <summary>
        /// Not enough resources
        /// </summary>
        MAPI_E_NOT_ENOUGH_RESOURCES = 0x80040000 | 0x10E,
        /// <summary>
        /// Not found
        /// </summary>
        MAPI_E_NOT_FOUND = 0x80040000 | 0x10F,
        /// <summary>
        /// Version error
        /// </summary>
        MAPI_E_VERSION = 0x80040000 | 0x110,
        /// <summary>
        /// Logon failed
        /// </summary>
        MAPI_E_LOGON_FAILED = 0x80040000 | 0x111,
        /// <summary>
        /// Session limited
        /// </summary>
        MAPI_E_SESSION_LIMIT = 0x80040000 | 0x112,
        /// <summary>
        /// User cancel
        /// </summary>
        MAPI_E_USER_CANCEL = 0x80040000 | 0x113,
        /// <summary>
        /// Unable to abort
        /// </summary>
        MAPI_E_UNABLE_TO_ABORT = 0x80040000 | 0x114,
        /// <summary>
        /// Network error
        /// </summary>
        MAPI_E_NETWORK_ERROR = 0x80040000 | 0x115,
        /// <summary>
        /// Disk error
        /// </summary>
        MAPI_E_DISK_ERROR = 0x80040000 | 0x116,
        /// <summary>
        /// Too complex
        /// </summary>
        MAPI_E_TOO_COMPLEX = 0x80040000 | 0x117,
        /// <summary>
        /// Bad column
        /// </summary>
        MAPI_E_BAD_COLUMN = 0x80040000 | 0x118,
        /// <summary>
        /// Extended error
        /// </summary>
        MAPI_E_EXTENDED_ERROR = 0x80040000 | 0x119,
        /// <summary>
        /// Computed
        /// </summary>
        MAPI_E_COMPUTED = 0x80040000 | 0x11A,
        /// <summary>
        /// Corrupt data
        /// </summary>
        MAPI_E_CORRUPT_DATA = 0x80040000 | 0x11B,
        /// <summary>
        /// Unconfigured
        /// </summary>
        MAPI_E_UNCONFIGURED = 0x80040000 | 0x11C,
        /// <summary>
        /// One provider fail
        /// </summary>
        MAPI_E_FAILONEPROVIDER = 0x80040000 | 0x11D,
        /// <summary>
        /// Unknown CPID
        /// </summary>
        MAPI_E_UNKNOWN_CPID = 0x80040000 | 0x11E,
        /// <summary>
        /// Unknown LCID 
        /// </summary>
        MAPI_E_UNKNOWN_LCID = 0x80040000 | 0x11F,
        /// <summary>
        /// Corrupt store
        /// </summary>
        MAPI_E_CORRUPT_STORE = 0x80040000 | 0x600,
        /// <summary>
        /// Not in queue
        /// </summary>
        MAPI_E_NOT_IN_QUEUE = 0x80040000 | 0x601,
        /// <summary>
        /// No suppress
        /// </summary>
        MAPI_E_NO_SUPPRESS = 0x80040000 | 0x602,
        /// <summary>
        /// Collision
        /// </summary>
        MAPI_E_COLLISION = 0x80040000 | 0x604,
        /// <summary>
        /// Not Initialized
        /// </summary>
        MAPI_E_NOT_INITIALIZED = 0x80040000 | 0x605,
        /// <summary>
        /// Not standard
        /// </summary>
        MAPI_E_NON_STANDARD = 0x80040000 | 0x606,
        /// <summary>
        /// No recipients
        /// </summary>
        MAPI_E_NO_RECIPIENTS = 0x80040000 | 0x607,
        /// <summary>
        /// Submitted error
        /// </summary>
        MAPI_E_SUBMITTED = 0x80040000 | 0x608,
        /// <summary>
        /// Has folders
        /// </summary>
        MAPI_E_HAS_FOLDERS = 0x80040000 | 0x609,
        /// <summary>
        /// Has message
        /// </summary>
        MAPI_E_HAS_MESSAGES = 0x80040000 | 0x60A,
        /// <summary>
        /// Folder cycle
        /// </summary>
        MAPI_E_FOLDER_CYCLE = 0x80040000 | 0x60B,
    }
}

