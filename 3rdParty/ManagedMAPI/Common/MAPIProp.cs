#region File Info
//
// File       : mapiprop.cs
// Description: MAPIProp class
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
    /// Provides an interface to manage MAPI property value
    /// </summary>
    public interface IPropValue
    {
        /// <summary>
        /// Gets type of value
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// Gets property tag
        /// </summary>
        uint Tag { get; }
        /// <summary>
        /// Gets string value
        /// </summary>
        string AsString { get; }
        /// <summary>
        /// Gets Int32 value
        /// </summary>
        int AsInt32 { get; }
        /// <summary>
        /// Gets Byte array value
        /// </summary>
        byte[] AsBinary { get; }
        /// <summary>
        /// Gets UInt64 value
        /// </summary>
        UInt64 AsUInt64 { get; }
        /// <summary>
        /// Gets boolean value
        /// </summary>
        bool AsBool { get; }
        /// <summary>
        /// Gets short value
        /// </summary>
        short AsShort { get; }
        /// <summary>
        /// Gets nullable DateTime value
        /// </summary>
        DateTime? AsDateTime { get; }
    }
    /// <summary>
    /// Represents a property of various MAPI objects. 
    /// </summary>
    public class MAPIProp : IPropValue
    {
        private uint tag;
        private Type t;
        private uint ul = 0;
        private byte[] binary;
        private string str = null;
        private UInt64 p_li;

        /// <summary>
        ///  Initializes a new instance of the MAPIProp class.
        /// </summary>
        /// <param name="prop">pSPropValue structure</param>
        internal MAPIProp(SPropValue prop)
        {
            this.tag = prop.ulPropTag;
            switch ((PT)((uint)this.tag & 0xFFFF))
            {
                case PT.PT_TSTRING:
                    this.t = typeof(string);
                    this.str = Marshal.PtrToStringUni(prop.Value.lpszW);
                    break;
                case PT.PT_STRING8:
                    this.t = typeof(string);
                    this.str = Marshal.PtrToStringAnsi(prop.Value.lpszA);
                    break;
                case PT.PT_LONG:
                case PT.PT_I2:
                case PT.PT_BOOLEAN:
                    this.t = typeof(int);
                    this.ul = prop.Value.ul;
                    break;
                case PT.PT_BINARY:
                    this.t = typeof(Byte[]);
                    this.binary = prop.Value.bin.AsBytes;
                    break;
                case PT.PT_SYSTIME:
                    this.t = typeof(DateTime);
                    this.p_li = prop.Value.li;
                    break;
                case PT.PT_I8:
                    this.t = typeof(UInt64);
                    this.p_li = prop.Value.li;
                    break;
                default:
                    this.t = null;
                    break;
            }
        }
        /// <summary>
        ///  Initializes a new instance of the MAPIProp class.
        /// </summary>
        /// <param name="tag">Property tag</param>
        /// <param name="value">Property value</param>
        public MAPIProp(uint tag, object value)
        {
            if ((PT)(tag & 0xFFFF) == PT.PT_UNSPECIFIED)
            {
                if (value is string)
                    this.tag = tag | (uint)PT.PT_UNICODE;
                else if (value is DateTime? || value is DateTime)
                    this.tag = tag | (uint)PT.PT_SYSTIME;
                else if (value is byte[])
                    this.tag = tag | (uint)PT.PT_BINARY;
                else if (value is int)
                    this.tag = tag | (uint)PT.PT_LONG;
                else if (value is short)
                    this.tag = tag | (uint)PT.PT_I2;
                else if (value is bool)
                    this.tag = tag | (uint)PT.PT_BOOLEAN;
                else if (value is UInt64 || value is Int64)
                    this.tag = tag | (uint)PT.PT_I8;
                else
                    this.tag = tag;

            }
            else
                this.tag = tag;
            this.t = null;

            switch ((PT)((uint)this.tag & 0xFFFF))
            {
                case PT.PT_TSTRING:
                case PT.PT_STRING8:
                    {
                        string v = value as string;
                        if (v != null)
                        {
                            this.t = typeof(string);
                            this.str = v;
                        }
                    }
                    break;
                case PT.PT_LONG:
                case PT.PT_I2:
                case PT.PT_BOOLEAN:
                    try
                    {
                        this.t = typeof(int);
                        this.ul = (uint)(int)value;
                    }
                    catch
                    {
                        this.t = null;
                    }
                    break;
                case PT.PT_BINARY:
                    try
                    {
                        this.t = typeof(Byte[]);
                        this.binary = value as Byte[];
                    }
                    catch
                    {
                        this.t = null;
                    }

                    break;
                case PT.PT_SYSTIME:
                    try
                    {
                        this.t = typeof(DateTime);
                        DateTime dt = ((DateTime)value).ToUniversalTime();
                        this.p_li = (ulong)(dt.ToFileTimeUtc());
                    }
                    catch
                    {
                        this.t = null;
                    }
                    break;
                case PT.PT_I8:
                    try
                    {
                        this.t = typeof(UInt64);
                        this.p_li = (UInt64)value;
                    }
                    catch
                    {
                        this.t = null;
                    }
                    break;
                default:
                    this.t = null;
                    break;
            }
        }

        /// <summary>
        /// Gets type of value
        /// </summary>
        public Type Type { get { return this.t; } }
        /// <summary>
        /// Gets property tag
        /// </summary>
        public uint Tag { get { return this.tag; } }
        /// <summary>
        /// Gets string value
        /// </summary>
        public string AsString
        {
            get
            {
                if (this.t == typeof(string))
                    return this.str;
                return null;
            }
        }
        /// <summary>
        /// Gets Int32 value
        /// </summary>
        public int AsInt32
        {
            get
            {
                if (this.t == typeof(int))
                    return (int)this.ul;
                else
                    throw new Exception("Invalid type request");
            }
        }

        /// <summary>
        /// Gets byte array value
        /// </summary>
        public byte[] AsBinary
        {
            get
            {
                if (this.t == typeof(byte[]))
                    return this.binary;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets unsigned Int64 value
        /// </summary>
        public UInt64 AsUInt64
        {
            get
            {
                if (this.t == typeof(UInt64) || this.t == typeof(DateTime))
                    return this.p_li;
                else
                    throw new Exception("Invalid type request");
            }
        }
        /// <summary>
        /// Gets nullable DateTime value
        /// </summary>
        public DateTime? AsDateTime
        {
            get
            {
                if (this.t == typeof(DateTime))
                {
                    try
                    {
                        DateTime dt = DateTime.FromFileTimeUtc((long)this.p_li);
                        dt = dt.ToLocalTime();
                        return dt;
                    }
                    catch { }
                }
                return null;
            }
        }
        /// <summary>
        /// Gets boolean value
        /// </summary>
        public bool AsBool
        {
            get
            {
                if (this.t == typeof(int))
                    return (short)this.ul != 0;
                else
                    throw new Exception("Invalid type request");
            }
        }
        /// <summary>
        /// Gets short value
        /// </summary>
        public short AsShort
        {
            get
            {
                if (this.t == typeof(int))
                    return (short)(int)this.ul;
                else
                    throw new Exception("Invalid type request");
            }
        }
    }
}
