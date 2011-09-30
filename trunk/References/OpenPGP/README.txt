-------------------------------------------------------------------------------------------------------------------
STARKSOFT OPENPGP LIBRARY FOR GNUPG 

Starksoft.Cryptography.OpenPGP.dll
Documentation.chm
Examples (VB and C#)
README.txt

The Starksoft OpenPGP Code Library is a software library built on the Microsoft .NET 2.0 Framework and coded in 100% 
C# managed code.  This library enables C# and VB.NET users to take advantage of the free and open source 
GNU Privacy Guard (GnuPGP or GPG for short) application which is a replacement for Pretty Good Privacy (PGP).  The
library allows programmers simple programmatic access to the GPG.EXE command line application using .NET streams.

http://www.starksoft.com for the latest version

----------------------------------------------------------------------------------------------------------------------
ABOUT STARKSOFT OPENPGP LIBRARY AND GNU Privacy Guard

GNU Privacy Guard from the GNU Project (also called GnuPG or GPG for short) is a highly regarded open source 
project that provides a complete and free implementation of the OpenPGP standard as defined by RFC2440.  It is intended
as a replacement for Pretty Good Privacy (PGP).  GnuPG allows you to encrypt and sign your data and communication, 
manage your public and private OpenPGP keys as well as access modules for all kind of public key directories. GPG.EXE, 
is a command line tool that is installed with GnuPG and contains features for easy integration with other applications. 

The Starksoft OpenPGP library provides classes that interface with the GPG.EXE command line tool.  The Starksoft OpenPGP 
libraries allows any .NET application to use GPG.EXE to encrypt or decrypt data using .NET IO Streams.  No temporary files 
are required and everything is handled through streams.  Any .NET Stream object can be used as long as the source stream 
can be read and the destination stream can be written to.  In addition the Starksoft OpenPGP Library supports 
asynchronous operations and the ability to read all the keys in the key ring into a simple collection or DataTable 
object.

----------------------------------------------------------------------------------------------------------------------
INSTALLING GNU PRIVACY GUARD (GNUPG)

In order for the Starksoft OpenPGP library to work you must first install the lastest version of GnuPG which includes 
GPG.EXE. You can obtain the latest version at http://www.gnupg.org/.  Please install the W32 client version for the 
easiest integration with Starksoft OpenPGP Library.  If did not install the Windows Installer version and have GPG.EXE
located somewhere else on your machine, Starksoft OpenPGP Library allows you to specify the binary path of GPG.EXE and 
well as your home path.  

A direct link to the windows download of GnuPG can be found below.
ftp://ftp.gnupg.org/gcrypt/binary/gnupg-w32cli-1.4.7.exe

See the GPG.EXE tool documentation for information on how to add keys to the GPG key ring and creating your public and private keys.

If you are new to GnuPG please install the application and then read how to generate new key pair or import existing 
PGP keys. You can read more about key generation and importing at http://www.gnupg.org/gph/en/manual.html#AEN26


-------------------------------------------------------------------------------------------------------------------
REQUIREMENTS

    * Microsoft .NET Framework version 2.0 or higher.
    * GNU PRIVACY GUARD (GNUPG) for Windows version 1.4.7 or higher.

-------------------------------------------------------------------------------------------------------------------
LICENSE


* Copyright (c) 2007-2009, Starksoft, LLC (http://www.starksoft.com)
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
