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
using System.Runtime.Serialization;

namespace Starksoft.Cryptography.OpenPGP
{
  /// <summary>
  /// This exception is thrown when a bad passphrase is given resulting in an error condition when running the GPG.EXE program.   
  /// </summary>
  [Serializable()]
  public class GnuPGBadPassphraseException : Exception
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    public GnuPGBadPassphraseException()
    { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">Exception message text.</param>
    public GnuPGBadPassphraseException(string message)
      : base(message)
    { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">Exception message text.</param>
    /// <param name="innerException">The inner exception object.</param>
    public GnuPGBadPassphraseException(string message, Exception innerException)
      : base(message, innerException)
    { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="info">Serialization information.</param>
    /// <param name="context">Stream context information.</param>
    protected GnuPGBadPassphraseException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    { }
  }

}

