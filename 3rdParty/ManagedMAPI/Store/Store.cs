using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace ManagedMAPI
{
    /// <summary>
    /// IMsgStore .Net Wrapper object
    /// </summary>
    public class MessageStore : IDisposable
    {
        /// <summary>
        /// New mail event
        /// </summary>
        public event EventHandler<MsgStoreNewMailEventArgs> OnNewMail;
  
        OnAdviseCallbackHandler callbackHandler_;
        uint ulConnection_ = 0;
        IMAPIAdviseSink pAdviseSink_ = null;
    
        IMsgStore mapiObj_;
        /// <summary>
        /// EntryID of the object
        /// </summary>
        protected EntryID Id_;

        /// <summary>
        /// Initializes a new instance of the MsgStore class. 
        /// </summary>
        /// <param name="msgStore">IMsgStore object</param>
        /// <param name="entryID">Entry identification of IMsgStore object</param>
        public MessageStore(MAPISession session, IMsgStore msgStore, EntryID entryID, string name)
        {
            ulConnection_ = 0;
            Session = session;
            mapiObj_ = msgStore;
            Id_ = entryID;
            Name = name;
        }

        #region Public Properties
        /// <summary>
        /// Gets entry identificatio of Message Store
        /// </summary>
        public EntryID StoreID { get { return Id_; } }
     

        public MAPISession Session
        { get; private set; }

        public string Name { get; private set; }


        #endregion

        #region Public Methods
     
        /// <summary>
        /// Registers to receive notification of specified events that affect the message store.
        /// </summary>
        /// <param name="eventMask">A mask of values that indicate the types of notification events that the caller is interested in and should be included in the registration. </param>
        /// <returns></returns>
        public bool RegisterEvents(EEventMask eventMask)
        {
            callbackHandler_ = new OnAdviseCallbackHandler(OnNotifyCallback);
            HRESULT hresult = HRESULT.S_OK;
            try
            {
                pAdviseSink_ = new MAPIAdviseSink(IntPtr.Zero, callbackHandler_);
                hresult = MAPIStore.Advise(0, IntPtr.Zero, (uint)eventMask, pAdviseSink_, out ulConnection_);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }
            return hresult == HRESULT.S_OK;
        }


        /// <summary>
        /// Cancels the sending of notifications.
        /// </summary>
        public void UnRegisteEvents()
        {
            if (ulConnection_ != 0)
                MAPIStore.Unadvise(ulConnection_);
            if (pAdviseSink_ != null)
            {
                pAdviseSink_ = null;
            }
        }

        #endregion

        #region Private Properties/Methods/Events

        IMsgStore MAPIStore
        {
            get { return mapiObj_ as IMsgStore; }
        }

        void OnNotifyCallback(IntPtr pContext, uint cNotification, IntPtr lpNotifications)
        {
            EEventMask eventType = (EEventMask)Marshal.ReadInt32(lpNotifications);
            int intSize = Marshal.SizeOf(typeof(int));
            IntPtr sPtr = lpNotifications + intSize * 2; //ulEventType, ulAlignPad
            switch (eventType)
            {
                case EEventMask.fnevNewMail:
                    {
                        Console.WriteLine("New mail");
                        if (this.OnNewMail == null)
                            break;
                        NEWMAIL_NOTIFICATION notification = (NEWMAIL_NOTIFICATION)Marshal.PtrToStructure(sPtr, typeof(NEWMAIL_NOTIFICATION));
                        MsgStoreNewMailEventArgs n = new MsgStoreNewMailEventArgs(StoreID, notification);
                        this.OnNewMail(this, n);
                    }
                    break;
             }
        }

        #endregion

        #region IDisposable Interface
        /// <summary>
        /// Dispose Msgstore object
        /// </summary>
        public void Dispose()
        {
            UnRegisteEvents();
            Session = null;
            if (mapiObj_ != null)
            {
                Marshal.ReleaseComObject(mapiObj_);
                mapiObj_ = null;
            }
        }

        #endregion


    }


}
