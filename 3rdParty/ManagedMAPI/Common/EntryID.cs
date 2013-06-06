using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ManagedMAPI
{
    /// <summary>
    /// Represents an entry identifier for a MAPI object. 
    /// </summary>
    public class EntryID
    {
        const int DefaultBufferSize = 256;
        private byte[] id_;
        
        /// <summary>
        /// Initializes a new instance of the EntryID from a byte array.
        /// </summary>
        /// <param name="id"></param>
        public EntryID(byte[] id) 
        {
            id_ = id;
        }
        /// <summary>
        /// Get byte array
        /// </summary>
        public byte[] AsByteArray { get { return this.id_; } }
        /// <summary>
        /// Build EntryID from a unmanaged memory block.
        /// </summary>
        /// <param name="cb">the size of block</param>
        /// <param name="lpb">the pointer of the unmanaged memory block</param>
        /// <returns>EntryID object</returns>
        public static EntryID BuildFromPtr(uint cb, IntPtr lpb)
        {
            byte[] b = new byte[cb];
            for (int i = 0; i < cb; i++)
                b[i] = Marshal.ReadByte(lpb, i);
            return new EntryID(b);
        }

        /// <summary>
        /// Build EntryID from string
        /// </summary>
        /// <param name="entryID">a string of entryID</param>
        /// <returns>EntryID object</returns>
        public static EntryID GetEntryID(string entryID)
        {
            if (string.IsNullOrEmpty(entryID))
                return null;
            int count = entryID.Length / 2;
            StringBuilder s = new StringBuilder(entryID);
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                if ((2 * i + 2) > s.Length)
                    return null;
                string s1 = s.ToString(2 * i, 2);
                if (!Byte.TryParse(s1, System.Globalization.NumberStyles.HexNumber, null as IFormatProvider, out bytes[i]))
                    return null;
            }
            return new EntryID(bytes);
        }

        /// <summary>
        /// Convert EntryID to string
        /// </summary>
        /// <returns>id string</returns>
        public override string ToString()
        {
            StringBuilder s = new StringBuilder(DefaultBufferSize);
            foreach (Byte b in id_)
            {
                s.Append(b.ToString("X2"));
            }
            return s.ToString();
        }
     }
}
