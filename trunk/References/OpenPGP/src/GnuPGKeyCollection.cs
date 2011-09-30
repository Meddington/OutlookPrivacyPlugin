/*
* Copyright (c) 2007-2008, Starksoft, LLC (http://www.starksoft.com)
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of Starsoft, LLC nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY Starksoft, LLC ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL Starksoft, LLC BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

namespace Starksoft.Cryptography.OpenPGP
{
  /// <summary>
  /// Collection of PGP keys stored in the GnuPGP application.
  /// </summary>
  public class GnuPGKeyCollection : IEnumerable<GnuPGKey>
  {
    private List<GnuPGKey> _keyList = new List<GnuPGKey>();
    private string _raw;

    private static string COL_KEY = "Key";
    private static string COL_KEY_EXPIRATION = "KeyExpiration";
    private static string COL_USER_ID = "UserId";
    private static string COL_USER_NAME = "UserName";
    private static string COL_SUB_KEY = "SubKey";
    private static string COL_SUB_KEY_EXPIRATION = "SubKeyExpiration";
    private static string COL_RAW = "Raw";

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="keys">StreamReader object containing GnuPG raw key stream data.</param>
    public GnuPGKeyCollection(StreamReader keys)
    {
      Fill(keys);
      GetRaw(keys);
    }

    /// <summary>
    /// Raw key stream text data.
    /// </summary>
    public string Raw
    {
      get { return _raw; }
    }

    private void GetRaw(StreamReader keys)
    {
      keys.BaseStream.Position = 0;
      _raw = keys.ReadToEnd();
    }

#if DISABLED
    private void Fill(StreamReader data)
    {
      string line = null;
      line = data.ReadLine();
      line = data.ReadLine();
      do
      {
        string line1 = data.ReadLine();
        string line2 = data.ReadLine();
        string line3 = data.ReadLine();
        while (!data.EndOfStream && (!line3.StartsWith("sub") && !line3.StartsWith("ssb")))
        {
          line3 = data.ReadLine();
        }
        GnuPGKey key = new GnuPGKey(String.Format("{0}\r\n{1}\r\n{2}", line1, line2, line3));
        _keyList.Add(key);
        data.ReadLine();
      }
      while (!data.EndOfStream);
    }
#else
    private void Fill(StreamReader data)
    {
      string key = null;
      string sub = null;
      List<string> uids = new List<string>();

      // Skip header lines
      do
      {
        key = data.ReadLine();
      } while (!data.EndOfStream && !key.StartsWith("pub") && !key.StartsWith("sec"));

      // Parse keys
      while (!data.EndOfStream)
      {
        string line = data.ReadLine();

        // Are we done with the key, store it!
        if ( line.StartsWith("pub") 
          || line.StartsWith("sec")
          || string.IsNullOrEmpty(line) )
        {
          if (uids.Count > 0)
          {
            foreach (string uid in uids)
            {
              GnuPGKey gnuKey = new GnuPGKey(String.Format("{0}\r\n{1}\r\n{2}", key, uid, sub));
              _keyList.Add(gnuKey);
            }
          }
          uids.Clear();
          sub = null;
          do
          {
            key = data.ReadLine();
          } while (!data.EndOfStream && !key.StartsWith("pub") && !key.StartsWith("sec"));
        }
        if (line.StartsWith("uid")
          && line.Contains("<")
          && line.Contains(">")
          && !line.Contains("[") )
        {
          uids.Add(line);
        }
        if (line.StartsWith("sub"))
          sub = line;
        if (line.StartsWith("ssb"))
          sub = line;
      }
    }
#endif

    /// <summary>
    ///  Searches for the specified GnuPGKey object and returns the zero-based index of the
    ///  first occurrence within the entire GnuPGKeyCollection colleciton.
    /// </summary>
    /// <param name="item">The GnuPGKeyobject to locate in the GnuPGKeyCollection.</param>
    /// <returns>The zero-based index of the first occurrence of item within the entire GnuPGKeyCollection, if found; otherwise, –1.</returns>
    public int IndexOf(GnuPGKey item)
    {
      return _keyList.IndexOf(item);
    }

    /// <summary>
    ///  Retrieves the specified GnuPGKey object by zero-based index from the GnuPGKeyCollection.        
    /// </summary>
    /// <param name="index">Zero-based index integer value.</param>
    /// <returns>The GnuPGKey object corresponding to the index position.</returns>
    public GnuPGKey GetKey(int index)
    {
      return _keyList[index];
    }

    /// <summary>
    /// Adds a GnuPGKey object to the end of the GnuPGKeyCollection.
    /// </summary>
    /// <param name="item">GnuPGKey item to add to the GnuPGKeyCollection.</param>
    public void AddKey(GnuPGKey item)
    {
      _keyList.Add(item);
    }

    /// <summary>
    /// Gets the number of elements actually contained in the GnuPGKeyCollection.
    /// </summary>
    public int Count
    {
      get { return _keyList.Count; }
    }

    IEnumerator<GnuPGKey> IEnumerable<GnuPGKey>.GetEnumerator()
    {
      return _keyList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _keyList.GetEnumerator();
    }

    /// <summary>
    /// Indexer for the GnuPGKeyCollection collection.
    /// </summary>
    /// <param name="index">Zero-based index value.</param>
    /// <returns></returns>
    public GnuPGKey this[int index]
    {
      get
      {
        return _keyList[index];
      }

    }

    /// <summary>
    /// Convert current GnuPGKeyCollection to a DataTable object to make data binding a minpulation of key data easier.
    /// </summary>
    /// <returns>Data table object.</returns>
    public DataTable ToDataTable()
    {
      DataTable dataTbl = new DataTable();
      CreateColumns(dataTbl);

      foreach (GnuPGKey item in _keyList)
      {
        DataRow row = dataTbl.NewRow();
        row[COL_USER_ID] = item.UserId;
        row[COL_USER_NAME] = item.UserName;
        row[COL_KEY] = item.Key;
        row[COL_KEY_EXPIRATION] = item.KeyExpiration;
        row[COL_SUB_KEY] = item.SubKey;
        row[COL_SUB_KEY_EXPIRATION] = item.SubKeyExpiration;
        dataTbl.Rows.Add(row);
      }

      return dataTbl;
    }

    private void CreateColumns(DataTable dataTbl)
    {
      dataTbl.Columns.Add(new DataColumn(COL_USER_ID, typeof(string)));
      dataTbl.Columns.Add(new DataColumn(COL_USER_NAME, typeof(string)));
      dataTbl.Columns.Add(new DataColumn(COL_KEY, typeof(string)));
      dataTbl.Columns.Add(new DataColumn(COL_KEY_EXPIRATION, typeof(DateTime)));
      dataTbl.Columns.Add(new DataColumn(COL_SUB_KEY, typeof(string)));
      dataTbl.Columns.Add(new DataColumn(COL_SUB_KEY_EXPIRATION, typeof(DateTime)));
      dataTbl.Columns.Add(new DataColumn(COL_RAW, typeof(string)));
    }

  }
}
