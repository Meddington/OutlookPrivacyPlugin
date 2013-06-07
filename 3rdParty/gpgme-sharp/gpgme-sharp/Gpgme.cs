/*
 * gpgme-sharp - .NET wrapper classes for libgpgme (GnuPG Made Easy)
 *  Copyright (C) 2008 Daniel Mueller <daniel@danm.de>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */
#define REQUIRE_GPGME_VERSION

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security;
using System.IO;
using System.ComponentModel;

using Libgpgme.Interop;

namespace Libgpgme
{
    public sealed class Gpgme
    {
        
        public static GpgmeVersion Version
        {
            get 
			{ 
				return libgpgme.gpgme_version; 
			}
        }

        public static string CheckVersion()
        {
            return libgpgme.gpgme_version_str;
        }


        public static string GetProtocolName(Protocol proto)
        {
            IntPtr ret = libgpgme.gpgme_get_protocol_name((gpgme_protocol_t)proto);

            if (ret == (IntPtr)0)
                throw new InvalidProtocolException("The specified protocol is invalid.");

            return PtrToStringAnsi(ret);
        }

        public static bool EngineCheckVersion(Protocol proto)
        {
            int err;
            err = libgpgme.gpgme_engine_check_version((gpgme_protocol_t)proto);

            gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);

            if (errcode == gpg_err_code_t.GPG_ERR_NO_ERROR)
                return true;
            else
                return false;
        }
        public static EngineInfo GetEngineInfo()
        {
            int err;
            IntPtr infoPtr;

            err = libgpgme.gpgme_get_engine_info(out infoPtr);
            gpg_err_code_t errcode = libgpgme.gpgme_err_code(err);

            if (errcode != gpg_err_code_t.GPG_ERR_NO_ERROR)
                throw new GpgmeException("System error: "
                    + err.ToString(), err);

            EngineInfo info = null;
            if (infoPtr != (IntPtr)0)
                info = new EngineInfo(infoPtr);

            return info;
        }

        internal static string PtrToStringAnsi(IntPtr ptr)
        {
            string str = null;
            if (ptr != (IntPtr)0)
            {
                str = Marshal.PtrToStringAnsi(ptr);
                str = str.Replace("\r", "");
            }
            return str;
        }
        internal static string PtrToStringAnsi(IntPtr ptr, int len)
        {
            string str = null;
            if (ptr != (IntPtr)0)
            {
                str = Marshal.PtrToStringAnsi(ptr, len);
            }
            return str;
        }

        internal static string PtrToStringUTF8(IntPtr ptr)
        {
            if (ptr != (IntPtr)0)
            {
                // calculate utf8 string size
                int size = 0;
                while (Marshal.ReadByte((IntPtr)((long)ptr + (long)size)) != '\0')
                    size++;
                return PtrToStringUTF8(ptr, size);
            }
            return null;
        }
        
        internal static string PtrToStringUTF8(IntPtr ptr, int size)
        {
            if (ptr != (IntPtr)0 && size > 0)
            {
                byte[] barray = new byte[size];
                
                // copy utf8 encoded string to byte array
                Marshal.Copy(ptr, barray, 0, size);

                // convert the UTF8 encoded string to unicode
                UTF8Encoding utf8encoding = new UTF8Encoding();
                int newsize = utf8encoding.GetCharCount(barray);
                char[] darray = new char[newsize];
                Decoder decoder = utf8encoding.GetDecoder();
                int bytesUsed, charsUsed;
                bool completed;
                decoder.Convert(barray, 0, barray.Length, darray, 0, newsize, true, out bytesUsed, out charsUsed, out completed);
                return new string(darray);
            }
            return null;
        }
        internal static IntPtr StringToCoTaskMemUTF8(string str)
        {
            if (str == null) 
				return (IntPtr)0;
            UTF8Encoding utf8 = new UTF8Encoding();
            char[] carray = str.ToCharArray();
            
			int	size = utf8.GetByteCount(carray);
            
            // Encode unicode string to UTF8 byte array
            byte[] barray = new byte[size + 1]; // + Null char
            Encoder encoder = utf8.GetEncoder();
            int charsUsed, bytesUsed;
            bool completed;
			
			if (size > 0) {
            	encoder.Convert(
                	carray, 0, carray.Length,   // source (UTF8 encoded string)
                	barray, 0, size,            // destination (bytes)
                	true, 
                	out charsUsed, 
                	out bytesUsed, 
                	out completed);
			}

            IntPtr ptr = Marshal.AllocCoTaskMem(size + 1);
            if (ptr == (IntPtr)0)
                throw new OutOfMemoryException("Could not allocate " + size + " bytes of memory.");
            
			Marshal.Copy(barray, 0, ptr, size + 1);
            
            return ptr;
        }
        internal static byte[] ConvertCharArrayAnsi(char[] carray)
        {
            if (carray == null)
                return null;

            byte[] b = new byte[carray.Length];
            for (int i = 0; i < carray.Length; i++)
                b[i] = (byte)carray[i];
            
            return b;
        }
        internal static byte[] ConvertCharArrayToUTF8(char[] carray, int additionalsize)
        {
            if (carray == null)
                return null;

            UTF8Encoding utf8 = new UTF8Encoding();
            int size = utf8.GetByteCount(carray);

            // Encode unicode string to UTF8 encoded byte array
            byte[] barray = new byte[size + additionalsize]; 
            
            Encoder encoder = utf8.GetEncoder();
            
            int charsUsed, bytesUsed;
            bool completed;
            
            encoder.Convert(
                carray, 0, carray.Length,   // source
                barray, 0, size,            // destination
                true, 
                out charsUsed, 
                out bytesUsed, 
                out completed);

            return barray;
        }
        internal static IntPtr[] KeyArrayToIntPtrArray(Key[] keyarray)
        {
            if (keyarray == null)
                return null;
            IntPtr[] parray = new IntPtr[keyarray.Length + 1];
            for (int i = 0; i < keyarray.Length; i++)
                parray[i] = keyarray[i].KeyPtr;
			parray[keyarray.Length] = IntPtr.Zero;

            return parray;
        }
        internal static IntPtr[] StringToCoTaskMemUTF8(string[] strarray)
        {
            if (strarray == null || strarray.Length == 0)
                return null;
            IntPtr[] parray = new IntPtr[strarray.Length + 1];
            for (int i = 0; i < strarray.Length; i++)
                parray[i] = StringToCoTaskMemUTF8(strarray[i]);
			parray[strarray.Length] = IntPtr.Zero;
            
            return parray;
        }
        internal static void FreeStringArray(IntPtr[] parray)
        {
            if (parray == null)
                return;
            for (int i=0; i < parray.Length; i++)
                if (!(parray[i].Equals(IntPtr.Zero)))
                {
                    Marshal.FreeCoTaskMem(parray[i]);
                    parray[i] = IntPtr.Zero;
                }
        }

        public static string GetPubkeyAlgoName(KeyAlgorithm algo)
        {
            IntPtr ret;
            ret = libgpgme.gpgme_pubkey_algo_name((gpgme_pubkey_algo_t)algo);
            if (ret == (IntPtr)0)
                throw new InvalidPubkeyAlgoException("The public key algorithm is unknown.");

            return PtrToStringAnsi(ret);
        }

        public static string GetHashAlgoName(HashAlgorithm algo)
        {
            IntPtr ret;
            ret = libgpgme.gpgme_hash_algo_name((gpgme_hash_algo_t)algo);
            if (ret == (IntPtr)0)
                throw new InvalidHashAlgoException("The hash algorithm is unknown.");

            return PtrToStringAnsi(ret);
        }

        public static AlgorithmCapability GetAlgorithmCapability<T> (T attr)
        {
            FieldInfo fieldinf = attr.GetType().GetField(attr.ToString());
            AlgorithmCapabilityAttribute[] types = (AlgorithmCapabilityAttribute[])
                fieldinf.GetCustomAttributes(
                typeof(AlgorithmCapabilityAttribute), false);
            return (types.Length > 0) ? types[0].Type : AlgorithmCapability.CanNothing;
        }

        public static string GetAttrDesc<T>(T attr)
        {
            FieldInfo fieldinf = attr.GetType().GetField(attr.ToString());
            try
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldinf.GetCustomAttributes(
                        typeof(DescriptionAttribute),
                        false);
                return (attributes.Length > 0) ? attributes[0].Description : attr.ToString();
            }
            catch 
            {
                return "Unknown attribute/description.";
            }
        }
        
        public static string GetStrError(int err)
        {
            lock ("_GnuPG.Lib.libgpgme.GetStrError")
            {
                IntPtr ret;

                ret = libgpgme.gpgme_strerror(err);

                return PtrToStringUTF8(ret);
            }
        }

        public static void GetStrError(int err, out string message)
        {
            const int ERANGE = 34;
            int bufsize = 512;
            IntPtr ptr = IntPtr.Zero;
            int reterr = 0;
            do
            {
                if (!ptr.Equals(IntPtr.Zero))
                {
                    Marshal.FreeCoTaskMem(ptr);
                    ptr = IntPtr.Zero;
                    bufsize *= 2;
                }
                ptr = Marshal.AllocCoTaskMem(bufsize);
                reterr = libgpgme.gpgme_strerror_r(err, out ptr, (UIntPtr)bufsize);
            } while (reterr == ERANGE);

			if (ptr != IntPtr.Zero) {
				message = PtrToStringUTF8(ptr);
				Marshal.FreeCoTaskMem(ptr);
                ptr = IntPtr.Zero;
			} else
				message = null;
        }

        public static string GetStrSource(int err)
        {
            IntPtr ret;

            ret = libgpgme.gpgme_strsource(err);

            return PtrToStringAnsi(ret);
        }

        /// <summary>
        /// Creates a new GPGME context.
        /// </summary>
        /// <returns></returns>
        public static Context CreateContext()
        {
            return new Context();
        }

        internal static DateTime ConvertFromUnix(long timestamp) 
        {
            DateTime unixdate =  new DateTime(1970, 1, 1, 0, 0, 0, 0);

            // difference between UTC an local time
            TimeSpan t = new TimeSpan(DateTime.UtcNow.Ticks - DateTime.Now.Ticks);
            return unixdate.AddSeconds(timestamp - t.TotalSeconds);
        }
        internal static DateTime ConvertFromUnixUTC(long timestamp)
        {
            DateTime unixdate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return unixdate.AddSeconds(timestamp);
        }
        internal static long ConvertToUnix(DateTime tm)
        {
            TimeSpan t = (tm - new DateTime(1970, 1, 1));
            int timestamp = (int)t.TotalSeconds;
            return timestamp;
        }
        internal static Stream ConvertToStream(int fd, FileAccess access)
        {
			if (!libgpgme.IsWindows)
            {
				// Mono has no SafeFileHandle classes yet
				return new Unix.UnixFDStream(fd);

                //return new FileStream(ptr, access); // TODO: does not work..	
            }
            else
            {
				IntPtr ptr = (IntPtr)fd;
                return new FileStream(
                    new Microsoft.Win32.SafeHandles.SafeFileHandle(ptr, false), 
                    access);
            
			}
        }
    }
}
