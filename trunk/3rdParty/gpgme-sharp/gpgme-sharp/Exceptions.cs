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
using System.Collections.Generic;
using System.Text;

namespace Libgpgme
{
    public class GpgmeException : Exception
    {
        public int GPGMEError;
        public GpgmeException() 
            : base() {}
        public GpgmeException(String message)
            : base(message) { }
        public GpgmeException(String message, int GPGMEError)
            : base(message)
        {
            this.GPGMEError = GPGMEError;
        }
    }
    public class GeneralErrorException : GpgmeException
    {
        public GeneralErrorException(string message)
            : base(message) { }
    }
    public class KeyExportException : GpgmeException
    {
        public KeyExportException() : base() { }
        public KeyExportException(string message, int GPGMEError)
            : base(message, GPGMEError) { }
    }
    public class KeyImportException : GpgmeException
    {
        public KeyImportException() : base() { }
        public KeyImportException(string message, int GPGMEError)
            : base(message, GPGMEError) { }
    }
    public class KeyConflictException : GpgmeException
    {
        public KeyConflictException() : base() { }
        public KeyConflictException(string message)
            : base(message) { }
    }

    public class InvalidProtocolException : GpgmeException
    {
        public InvalidProtocolException(string message)
            : base(message) { }
    }
    public class InvalidDataBufferException : GpgmeException
    {
        public InvalidDataBufferException() : base() { }
        public InvalidDataBufferException(string message)
            : base(message) { }
    }
    public class InvalidPubkeyAlgoException : GpgmeException
    {
        public InvalidPubkeyAlgoException(string message)
            : base(message) { }
    }
    public class InvalidHashAlgoException : GpgmeException
    {
        public InvalidHashAlgoException(string message)
            : base(message) { }
    }
    public class InvalidPtrException : GpgmeException
    {
        public InvalidPtrException(string message)
            : base(message) { }
    }
    public class InvalidKeyFprException : GpgmeException
    {
        public InvalidKeyFprException() : base() { }
        public InvalidKeyFprException(string message)
            : base(message) { }
    }
    public class InvalidKeyException : GpgmeException
    {
        public InvalidKeyException() : base() { }
        public InvalidKeyException(string message)
            : base(message) { }
    }
    public class InvalidPassphraseException : GpgmeException
    {
        public InvalidPassphraseException() : base() { }
        public InvalidPassphraseException(string message)
            : base(message) { }
    }
    public class InvalidContextException : GpgmeException
    {
        public InvalidContextException(string message)
            : base(message) { }
        public InvalidContextException()
            : base() { }
    }
    public class InvalidTimestampException: GpgmeException
    {
    	public InvalidTimestampException(string message) 
    		: base(message) { }
    	public InvalidTimestampException() 
    		: base() { }
    }
    public class TimestampNotAvailableException: GpgmeException
    {
    	public TimestampNotAvailableException(string message) 
    		: base(message) { }
    	public TimestampNotAvailableException() 
    		: base() { }
    }
    public class KeyNotFoundException : GpgmeException
    {
        public KeyNotFoundException() : base() { }
        public KeyNotFoundException(string message)
            : base(message) { }
    }
    public class AmbiguousKeyException : GpgmeException
    {
        public AmbiguousKeyException() : base() { }
        public AmbiguousKeyException(string message)
            : base(message) { }
    }
    public class BadPassphraseException : GpgmeException
    {
        public DecryptionResult DecryptionResult = null;
        public PassphraseInfo PassphraseInfo = null;

        public BadPassphraseException() : base() { }
        public BadPassphraseException(DecryptionResult rst) 
            : base() 
        {
            this.DecryptionResult = rst;
        }
        public BadPassphraseException(PassphraseInfo info)
            : base()
        {
            this.PassphraseInfo = info;
        }
    }
    public class EmptyPassphraseException : BadPassphraseException
    {
        public EmptyPassphraseException() : base() { }
        public EmptyPassphraseException(PassphraseInfo info)
            : base()
        {
            this.PassphraseInfo = info;
        }
    }
    public class NoDataException : GpgmeException
    {
        public VerificationResult VerifyResult = null;
        public NoDataException() : base() { }
        public NoDataException(string message) : base(message) { }
        public NoDataException(string message, VerificationResult rst)
            : base(message)
        {
            this.VerifyResult = rst;
        }
    }
    public class DecryptionFailedException : GpgmeException
    {
        public DecryptionResult DecryptionResult = null;
        public DecryptionFailedException() : base() { }
        public DecryptionFailedException(string message) : base(message) { }
        public DecryptionFailedException(DecryptionResult rst) : base() 
        {
            this.DecryptionResult = rst;
        }
    }
    public class AlreadySignedException : GpgmeException
    {
        public string KeyId;
        public AlreadySignedException(string keyid)
            : base()
        {
            this.KeyId = keyid;
        }
    }
}
