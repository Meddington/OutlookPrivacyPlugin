#region File Info
//
// File       : mapitable.cs
// Description: mapitable class
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
    /// <summary>
    /// .Net wrapper over IMAPITable interface.
    /// </summary>
    public class MAPITable : IDisposable
    {
        Guid IID_IMAPITable = new Guid("00020301-0000-0000-c000-000000000046");

        private IMAPITable tb_;

        /// <summary>
        /// Initializes a new instance of the MAPITable class. 
        /// </summary>
        /// <param name="mapiTable"></param>
        public MAPITable(IMAPITable mapiTable)
        {
            tb_ = mapiTable;
        }
        /// <summary>
        /// Defines the particular properties and order of properties to appear as columns in the table.
        /// </summary>
        /// <param name="tags">An array of property tags identifying properties to be included as columns in the table</param>
        /// <returns></returns>
        public bool SetColumns(PropTags[] tags)
        {
            uint[] t = new uint[tags.Length + 1];
            t[0] = (uint)tags.Length;
            for (int i = 0; i < tags.Length; i++)
                t[i + 1] = (uint)tags[i];
            return tb_.SetColumns(t, 0) == HRESULT.S_OK;
        }
       
        /// <summary>
        /// Returns one or more rows from a table, beginning at the current cursor position.
        /// </summary>
        /// <param name="lRowCount">Maximum number of rows to be returned.</param>
        /// <param name="sRows">an SRow array holding the table rows.</param>
        /// <returns></returns>
        public bool QueryRows(int lRowCount, out SRow[] sRows)
        {
            IntPtr pRowSet = IntPtr.Zero;
            HRESULT hr = tb_.QueryRows(lRowCount, 0, out pRowSet);
            if (hr != HRESULT.S_OK)
            {
                MAPINative.MAPIFreeBuffer(pRowSet);
            }

            uint cRows = (uint)Marshal.ReadInt32(pRowSet);
            sRows = new SRow[cRows];

            if (cRows < 1)
            {
                MAPINative.MAPIFreeBuffer(pRowSet);
                return false;
            }

            int pIntSize = IntPtr.Size, intSize = Marshal.SizeOf(typeof(Int32));
            int sizeOfSRow = 2 * intSize + pIntSize;
            IntPtr rows = pRowSet + intSize;
            for (int i = 0; i < cRows; i++)
            {
                IntPtr pRowOffset = rows + i * sizeOfSRow;
                uint cValues = (uint)Marshal.ReadInt32(pRowOffset + pIntSize);
                IntPtr pProps = Marshal.ReadIntPtr(pRowOffset + pIntSize + intSize);

                IPropValue[] lpProps = new IPropValue[cValues];
                for (int j = 0; j < cValues; j++) // each column
                {
                    SPropValue lpProp = (SPropValue)Marshal.PtrToStructure(pProps + j * Marshal.SizeOf(typeof(SPropValue)), typeof(SPropValue));
                    lpProps[j] = new MAPIProp(lpProp);
                }
                sRows[i].propVals = lpProps;
            }
            MAPINative.MAPIFreeBuffer(pRowSet);
            return true;
        }

        /// Moves the cursor to a specific position in the table.
        /// </summary>
        /// <param name="bookMark">The bookmark identifying the starting position for the seek operation.</param>
        /// <param name="rowCount">The signed count of the number of rows to move, starting from the bookmark.</param>
        /// <returns></returns>
        public bool SeekRow(BookMark bookMark, int rowCount)
        {
            IntPtr pRowsSought;
            HRESULT hResult = tb_.SeekRow((int)bookMark, rowCount, out pRowsSought);
            return hResult == HRESULT.S_OK;
        }
      
   
        #region IDisposable Interface
        /// <summary>
        /// Dispose MAPITable object.
        /// </summary>
        public void Dispose()
        {
            if (tb_ != null)
            {
                Marshal.ReleaseComObject(tb_);
                tb_ = null;
            }
        }

        #endregion
    }
}
