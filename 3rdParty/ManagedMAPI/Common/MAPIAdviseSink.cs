using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ManagedMAPI
{
    /// <summary>
    /// Describes information that relate to the arrival of a new message.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NEWMAIL_NOTIFICATION
    {
        /// <summary>
        /// Count of bytes in the entry identifier pointed to by the lpEntryID member.
        /// </summary>
        public uint cbEntryID;
        /// <summary>
        /// Pointer to the entry identifier of the newly arrived message.
        /// </summary>
        public IntPtr pEntryID;
        /// <summary>
        /// Count of bytes in the entry identifier pointed to by the lpParentID member.
        /// </summary>
        public uint cbParentID;
        /// <summary>
        /// Pointer to the entry identifier of the receive folder for the newly arrived message.
        /// </summary>
        public IntPtr pParentID;
        /// <summary>
        /// Bitmask of flags used to describe the format of the string properties included with the message.
        /// </summary>
        public uint Flags;
        /// <summary>
        /// The message class of the newly arrived message.
        /// </summary>
        public IntPtr MessageClass;
        /// <summary>
        /// Bitmask of flags that describes the current state of the newly arrived message.
        /// </summary>
        public uint MessageFlags;
    }

      
    /// <summary>
    /// .Net Object implement IMAPIAdviseSink interface
    /// </summary>
    class MAPIAdviseSink : IMAPIAdviseSink
    {
        OnAdviseCallbackHandler callbackHandler_;
        IntPtr pContext_;
        /// <summary>
        /// Initializes a new instance of the MAPIAdviseSink class. 
        /// </summary>
        /// <param name="pContext">object pointer</param>
        /// <param name="callbackHandler">callback delegate</param>
        public MAPIAdviseSink(IntPtr pContext, OnAdviseCallbackHandler callbackHandler)
        {
            pContext_ = pContext;
            callbackHandler_ = callbackHandler;
        }
        /// <summary>
        /// Responds to a notification by performing one or more tasks.
        /// </summary>
        /// <param name="cNotify">The count of NOTIFICATION structures pointed to by the lpNotifications parameter.</param>
        /// <param name="lpNotifications">A pointer to one or more NOTIFICATION structures that provide information about the events that have occurred.</param>
        /// <returns>S_OK, if the notification was processed successfully; otherwise, failed.</returns>
        public HRESULT OnNotify(uint cNotify, IntPtr lpNotifications)
        {
            if (callbackHandler_ != null)
                callbackHandler_(pContext_, cNotify, lpNotifications);
            return HRESULT.S_OK;
        }
    }
}
