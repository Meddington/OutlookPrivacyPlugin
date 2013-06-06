#region File Info
//
// File       : mapisession.cs
// Description: MAPISession class
// Package    : ManagedMAPI
//
// Authors    : Fred Song
//
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ManagedMAPI
{

    /// <summary>
    /// IMAPISession .Net wrapper object
    /// </summary>
    public class MAPISession : IDisposable
    {
        /// <summary>
        /// New mail event
        /// </summary>
        IMAPISession session_ = null;
        MAPITable content_ = null;

        /// <summary>
        /// Initializes a new instance of the MAPISession class. 
        /// </summary>
        public MAPISession()
        {
            Initialize();
        }

        #region Public Properties

        public MessageStore CurrentStore { get; private set; }

        public StoreInfo DefaultStore
        {
            get
            {
                if (Content != null)
                {
                    Content.SeekRow(BookMark.BEGINNING, 0);
                    if (Content.SetColumns(new PropTags[] { PropTags.PR_DISPLAY_NAME, PropTags.PR_ENTRYID, PropTags.PR_DEFAULT_STORE }))
                    {
                        SRow[] sRows;

                        while (Content.QueryRows(1, out sRows))
                        {
                            if (sRows.Length != 1)
                                break;
                            if (sRows[0].propVals[2].AsBool)
                                return new StoreInfo(this, sRows[0].propVals[0].AsString, new EntryID(sRows[0].propVals[1].AsBinary));
                        }
                    }
                }
                return null;
            }
        }
      
        #endregion


        #region Public Methods
                
        public List<StoreInfo> GetMessageStores()
        {
            List<StoreInfo> stores = new List<StoreInfo>();
            try
            {
                if (Content != null)
                {
                    Content.SeekRow(BookMark.BEGINNING, 0);
                    if (Content.SetColumns(new PropTags[] { PropTags.PR_DISPLAY_NAME, PropTags.PR_ENTRYID }))
                    {
                        SRow[] sRows;

                        while (Content.QueryRows(1, out sRows))
                        {
                            if (sRows.Length != 1)
                                break;
                            stores.Add(new StoreInfo(this, sRows[0].propVals[0].AsString,  new EntryID (sRows[0].propVals[1].AsBinary)));
                         }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return stores;
        }


        /// <summary>
        /// Opens a message store.
        /// </summary>
        /// <param name="storeName">store name</param>
        /// <returns>true, if the message store was successfully opened; otherwise, failed.</returns>
        public bool OpenMessageStore(string storeName)
        {
            bool bResult = false;
            try
            {
                if (Content != null)
                {
                    Content.SeekRow(BookMark.BEGINNING, 0);
                    if (Content.SetColumns(new PropTags[] { PropTags.PR_DISPLAY_NAME, PropTags.PR_ENTRYID, PropTags.PR_DEFAULT_STORE }))
                    {
                        SRow[] sRows;
                         while (Content.QueryRows(1, out sRows))
                        {
                            if (sRows.Length != 1)
                                break;
                            if (string.IsNullOrEmpty(storeName))
                            {
                                if (sRows[0].propVals[2].AsBool)
                                    bResult = true;
                            }
                            else if (sRows[0].propVals[0].AsString.IndexOf(storeName) > -1)
                                bResult = true;
                            if (bResult)
                                break;
                        }
                        if (bResult)
                        {
                            if (CurrentStore != null)
                            {
                                CurrentStore.Dispose();
                                CurrentStore = null;
                            }
                            SBinary entryId = SBinary.SBinaryCreate(sRows[0].propVals[1].AsBinary);
                            storeName = sRows[0].propVals[0].AsString;
                            IntPtr pStore = IntPtr.Zero;
                            if (session_.OpenMsgStore(0, entryId.cb, entryId.lpb, IntPtr.Zero, (uint)MAPIFlag.BEST_ACCESS, out pStore) == HRESULT.S_OK)
                            {
                                if (pStore != IntPtr.Zero)
                                {
                                    IMsgStore msgStore = Marshal.GetObjectForIUnknown(pStore) as IMsgStore;
                                    CurrentStore = new MessageStore(this, msgStore, new EntryID(entryId.AsBytes), storeName);
                                }
                            }
                            SBinary.SBinaryRelease(ref entryId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            return bResult;
        }

        public bool CompareEntryIDs(EntryID entryid1, EntryID entryid2)
        {
            SBinary sb1 = SBinary.SBinaryCreate(entryid1.AsByteArray);
            SBinary sb2 = SBinary.SBinaryCreate(entryid2.AsByteArray);
            bool result;
            session_.CompareEntryIDs(sb1.cb, sb1.lpb, sb2.cb, sb2.lpb, 0, out result);
            SBinary.SBinaryRelease(ref sb1);
            SBinary.SBinaryRelease(ref sb2);
            return result;
        }
      
        #endregion

        #region Implement IDisposable Interface

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion 

        #region private methods/events

        /// <summary>
        /// MAPI session intialization and logon.
        /// </summary>
        /// <returns>true if successful; otherwise, false</returns>
        private bool Initialize()
        {
            IntPtr pSession = IntPtr.Zero;
            if (MAPINative.MAPIInitialize(IntPtr.Zero) == HRESULT.S_OK)
            {
                MAPINative.MAPILogonEx(0, null, null, (uint)(MAPIFlag.EXTENDED | MAPIFlag.USE_DEFAULT), out pSession);
                if (pSession == IntPtr.Zero)
                    MAPINative.MAPILogonEx(0, null, null, (uint)(MAPIFlag.EXTENDED | MAPIFlag.NEW_SESSION | MAPIFlag.USE_DEFAULT), out pSession);
            }

            if (pSession != IntPtr.Zero)
            {
                object sessionObj = null;
                try
                {
                    sessionObj = Marshal.GetObjectForIUnknown(pSession);
                    session_ = sessionObj as IMAPISession;
                }
                catch { }
            }

            return session_ != null;
        }

          
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (session_ != null)
                {
                    if (CurrentStore != null)
                    {
                        CurrentStore.Dispose();
                        CurrentStore = null;
                    }
                    if (content_ != null)
                    {
                        content_.Dispose();
                        content_ = null;
                    }
                    Marshal.ReleaseComObject(session_);
                    session_ = null;
                    MAPINative.MAPIUninitialize();
                }
            }
        }

        MAPITable Content
        {
            get
            {
                if (content_ == null)
                {
                    if (session_ != null)
                    {
                        IntPtr pTable = IntPtr.Zero;
                        session_.GetMsgStoresTable(0, out pTable);
                        if (pTable != IntPtr.Zero)
                        {
                            object tableObj = null;
                            tableObj = Marshal.GetObjectForIUnknown(pTable);
                            content_ = new MAPITable(tableObj as IMAPITable);
                        }
                    }
                }
                return content_;
            }
        }
        #endregion

    }
}

