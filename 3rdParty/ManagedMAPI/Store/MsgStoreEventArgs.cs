using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ManagedMAPI
{
    /// <summary>
    /// Provides data for the arrival of a new message.
    /// </summary>
    public class MsgStoreNewMailEventArgs : EventArgs
    {
        /// <summary>
        /// Entry identification of message store.
        /// </summary>
        public EntryID StoreID { get; private set; }
        /// <summary>
        /// Entry identification of the newly arrived message.
        /// </summary>
        public EntryID EntryID { get; private set; }
        /// <summary>
        /// The entry identifier of the receive folder for the newly arrived messag.
        /// </summary>
        public EntryID ParentID { get; private set; }
        /// <summary>
        /// Bitmask of flags that describes the current state of the newly arrived message.
        /// </summary>
        public int MessageFlags { get; private set; }
        /// <summary>
        /// The message class of the newly arrived message.
        /// </summary>
        public string MessageClass { get; private set; }
        /// <summary>
        /// Initializes a new instance of the MsgStoreNewMailEventArgs class. 
        /// </summary>
        /// <param name="storeID">The entry identification of message store.</param>
        /// <param name="notification">The new mail notification structure.</param>
        public MsgStoreNewMailEventArgs(EntryID storeID, NEWMAIL_NOTIFICATION notification)
        {
            StoreID = storeID;
            SBinary sbEntry = new SBinary() { cb = notification.cbEntryID, lpb = notification.pEntryID };
            SBinary sbParent = new SBinary() { cb = notification.cbParentID, lpb = notification.pParentID };
            EntryID = sbEntry.cb > 0 ? new EntryID(sbEntry.AsBytes) : null;
            ParentID = sbParent.cb > 0 ? new EntryID(sbParent.AsBytes) : null;
            MessageFlags = (int)notification.MessageFlags;
            if ((notification.Flags & (uint)CharacterSet.UNICODE) == (uint)CharacterSet.UNICODE)
                MessageClass = Marshal.PtrToStringUni(notification.MessageClass);
            else
                MessageClass = Marshal.PtrToStringAnsi(notification.MessageClass);
        }
    }
}
