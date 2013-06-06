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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using Libgpgme.Interop;

namespace Libgpgme
{
	public class UserId: IEnumerable<UserId>
	{
		private UserId next;
		private bool revoked, invalid;
		private Validity validity;
		private string uid, name, comment, email;
		private KeySignature signatures;
		
		internal UserId(IntPtr uidPtr)
		{
			if (uidPtr == (IntPtr)0)
				throw new InvalidPtrException("Invalid user id pointer. Bad programmer! *spank* *spank*");
            
            UpdateFromMem(uidPtr);
		}
		private void UpdateFromMem(IntPtr uidPtr) 
		{
			_gpgme_user_id userid = (_gpgme_user_id)
				Marshal.PtrToStructure(uidPtr, typeof(_gpgme_user_id)); 
			
			revoked = userid.revoked;
			invalid = userid.invalid;
			validity = (Validity)userid.validity;
			uid = Gpgme.PtrToStringUTF8(userid.uid);
			name = Gpgme.PtrToStringUTF8(userid.name);
			comment = Gpgme.PtrToStringUTF8(userid.comment);
			email = Gpgme.PtrToStringUTF8(userid.email);
			
			if (userid.signatures != (IntPtr)0)
				signatures = new KeySignature(userid.signatures);
			else
				signatures = null;
			
			if (userid.next != (IntPtr)0)
				next = new UserId(userid.next);
			else
				next = null;
		}
		public bool Revoked {
			get { return revoked; }
		}
		public bool Invalid {
			get { return invalid; }
		}
		public Validity Validity {
			get { return validity; }
		}
		public string Uid {
			get { return uid; }
		}
		public string Name {
			get { return name; }
		}
		public string Comment {
			get { return comment; }
		}
		public string Email {
			get { return email; }
		}
		public KeySignature Signatures {
			get { return signatures; }
		}
		public UserId Next {
			get { return next; }
		}
		
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();
			if (name != null)
				sb.Append(name);
			if (comment != null)
			{
				sb.Append(" (");
				sb.Append(comment);
				sb.Append(")");
			}
			if (email != null)
			{
				sb.Append(" <");
				sb.Append(email);
				sb.Append(">");
			}
			return sb.ToString();
		}

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<UserId> GetEnumerator()
        {
            UserId id = this;
            while (id != null)
            {
                yield return id;
                id = id.Next;
            }
        }
	}
}
