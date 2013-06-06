#region File Info
//
// File       : define.cs
// Description: MAPI Definitions
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
    #region public MAPI definitions

    /// <summary>
    /// A mask of values that indicate the types of notification events that the caller is interested in and should be included in the registration. There is a corresponding NOTIFICATION structure associated with each type of event that holds information about the event.
    /// </summary>
    public enum EEventMask : uint
    {
        /// <summary>
        /// Registers for notifications about severe errors, such as insufficient memory.
        /// </summary>
        fnevCriticalError = 0x00000001,
        /// <summary>
        /// Registers for notifications about the arrival of new messages.
        /// </summary>
        fnevNewMail = 0x00000002,
        /// <summary>
        /// Registers for notifications about the creation of a new folder or message.
        /// </summary>
        fnevObjectCreated = 0x00000004,
        /// <summary>
        /// Registers for notifications about a folder or message being deleted.
        /// </summary>
        fnevObjectDeleted = 0x00000008,
        /// <summary>
        /// Registers for notifications about a folder or message being modified.
        /// </summary>
        fnevObjectModified = 0x00000010,
        /// <summary>
        /// Registers for notifications about a folder or message being moved.
        /// </summary>
        fnevObjectMoved = 0x00000020,
        /// <summary>
        /// Registers for notifications about a folder or message being copied.
        /// </summary>
        fnevObjectCopied = 0x00000040,
        /// <summary>
        /// Registers for notifications about the completion of a search operation.
        /// </summary>
        fnevSearchComplete = 0x00000080,
        /// <summary>
        /// Registers for notifications about a table being modified.
        /// </summary>
        fnevTableModified = 0x00000100,
        /// <summary>
        /// Registers for notifications about a status object being modified.
        /// </summary>
        fnevStatusObjectModified = 0x00000200,
        /// <summary>
        /// Reserved
        /// </summary>
        fnevReservedForMapi = 0x40000000,
        /// <summary>
        /// Registers for notifications about events specific to the particular message store provider.
        /// </summary>
        fnevExtended = 0x80000000,
    }
    /// <summary>
    /// Describes a row from a table containing selected properties for a specific object.
    /// </summary>
    public struct SRow
    {
        /// <summary>
        /// An array of SPropValue structures that describe the property values for the columns in the row. 
        /// </summary>
        public IPropValue[] propVals;
    }
   
   /// <summary>
    /// Bitmask of flags that controls the mapi character set.
    /// </summary>
    public enum CharacterSet : uint
    {
        /// <summary>
        /// Ansi
        /// </summary>
        ANSI = 0,
        /// <summary>
        /// Unicode
        /// </summary>
        UNICODE = 0x80000000
    }

    /// <summary>
    /// Generic MAPI Bitmask
    /// </summary>
    public enum MAPIFlag : uint
    {
        LOGON_UI = 0x00000001,
        /// <summary>
        /// An attempt should be made to create a new MAPI session instead of acquiring the shared session. 
        /// </summary>
        NEW_SESSION = 0x00000002,
        /// <summary>
        /// An attempt should be made to download all of the user's messages before returning. If it is not set, messages can be downloaded in the background after the call to MAPILogonEx returns.
        /// </summary>
        FORCE_DOWNLOAD = 0x00001000,
        /// <summary>
        /// A dialog box should be displayed to prompt the user for logon information if required.
        /// </summary>
        MAPI_LOGON_UI = 0x00000001,
        /// <summary>
        /// The shared session should be returned, which allows later clients to obtain the session without providing any user credentials.
        /// </summary>
        ALLOW_OTHERS = 0x00000008,
        /// <summary>
        /// The default profile should not be used and the user should be required to supply a profile.
        /// </summary>
        EXPLICIT_PROFILE = 0x00000010,
        /// <summary>
        /// Log on with extended capabilities. This flag should always be set.
        /// </summary>
        EXTENDED = 0x00000020,
        /// <summary>
        /// MAPILogonEx should display a configuration dialog box for each message service in the profile. 
        /// </summary>
        SERVICE_UI_ALWAYS = 0x00002000,
        /// <summary>
        /// MAPI should not inform the MAPI spooler of the session's existence.
        /// </summary>
        NO_MAIL = 0x00008000,
        /// <summary>
        /// The messaging subsystem should substitute the profile name of the default profile for the Profile Name parameter. T
        /// </summary>
        USE_DEFAULT = 0x00000040,
        /// <summary>
        /// Requests that the object be opened by using the maximum network permissions allowed for the user and the maximum client application access. 
        /// </summary>
        BEST_ACCESS = 0x00000010,
        /// <summary>
        /// MAPI spooler is a process that is responsible for sending messages to and receiving messages from a messaging system.
        /// </summary>
        SPOOLER = 37,
        /// <summary>
        /// Suppresses the display of dialog boxes. If the AB_NO_DIALOG flag is not set, the address book providers that contribute to the integrated address book can prompt the user for any necessary information.
        /// </summary>
        AB_NO_DIALOG = 0x00000001,
        /// <summary>
        /// The ulUIParam parameter is ignored unless the MAPI_DIALOG flag is set.
        /// </summary>
        MAPI_DIALOG = 0x00000008,
    }

    /// <summary>
    /// Bitmask of flags that controls the flush operation.
    /// </summary>
    public enum FlushFlag
    {
        /// <summary>
        /// The outbound message queues should be flushed.
        /// </summary>
        UPLOAD = 2,
        /// <summary>
        /// The inbound message queues should be flushed.
        /// </summary>
        DOWNLOAD = 4,
        /// <summary>
        /// The flush operation should occur regardless, in spite of the possibility of performance degradation. This flag must be set when targeting an asynchronous transport provider.
        /// </summary>
        FORCE = 8,
        /// <summary>
        /// The status object should not display a progress indicator. This flag is used only by the MAPI spooler; providers ignore this flag.
        /// </summary>
        NO_UI = 16,
        /// <summary>
        /// The flush operation can occur asynchronously. This flag only applies to the MAPI spooler's status object. 
        /// </summary>
        ASYNC_OK = 32
    }

    /// <summary>
    /// Type of MAPI object
    /// </summary>
    public enum ObjectType : uint
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// Message Store
        /// </summary>
        MAPI_STORE = 0x00000001,
        /// <summary>
        /// Address Book
        /// </summary>
        MAPI_ADDRBOOK = 0x00000002,
        /// <summary>
        /// Folder
        /// </summary>
        MAPI_FOLDER = 0x00000003,
        /// <summary>
        /// Address Book Container
        /// </summary>
        MAPI_ABCONT = 0x00000004,
        /// <summary>
        /// Message
        /// </summary>
        MAPI_MESSAGE = 0x00000005,
        /// <summary>
        /// Individual Recipient
        /// </summary>
        MAPI_MAILUSER = 0x00000006,
        /// <summary>
        /// Attachment
        /// </summary>
        MAPI_ATTACH = 0x00000007,
        /// <summary>
        /// Distribution List Recipient
        /// </summary>
        MAPI_DISTLIST = 0x00000008,
        /// <summary>
        /// Profile Section
        /// </summary>
        MAPI_PROFSECT = 0x00000009,
        /// <summary>
        ///  Status Object
        /// </summary>
        MAPI_STATUS = 0x0000000A,
        /// <summary>
        /// Session
        /// </summary>
        MAPI_SESSION = 0x0000000B,
        /// <summary>
        /// Form Information
        /// </summary>
        MAPI_FORMINFO = 0x0000000C,
    }
    
    #endregion

    #region internal definitions
    [StructLayout(LayoutKind.Explicit)]
    internal struct _PV
    {
        [FieldOffset(0)]
        public Int16 i;
        [FieldOffset(0)]
        public int l;
        [FieldOffset(0)]
        public uint ul;
        [FieldOffset(0)]
        public float flt;
        [FieldOffset(0)]
        public double dbl;
        [FieldOffset(0)]
        public UInt16 b;
        [FieldOffset(0)]
        public double at;
        [FieldOffset(0)]
        public IntPtr lpszA;
        [FieldOffset(0)]
        public IntPtr lpszW;
        [FieldOffset(0)]
        public IntPtr lpguid;
        /*[FieldOffset(0)]
        public IntPtr bin;*/
        [FieldOffset(0)]
        public UInt64 li;
        [FieldOffset(0)]
        public SBinary bin;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SPropValue
    {
        public UInt32 ulPropTag;
        public UInt32 dwAlignPad;
        public _PV Value;
    }

    /// <summary>
    ///  The bookmark identifying the starting position for the seek operation.
    /// </summary>
    public enum BookMark
    {
        /// <summary>
        /// Starts the seek operation from the beginning of the table.
        /// </summary>
        BEGINNING = 0,
        /// <summary>
        /// Starts the seek operation from the row in the table where the cursor is located.
        /// </summary>
        CURRENT = 1,
        /// <summary>
        /// Starts the seek operation from the end of the table.
        /// </summary>
        END = 2
    }

    delegate void OnAdviseCallbackHandler(IntPtr pContext, uint cNotification, IntPtr lpNotifications);

    #endregion
}
