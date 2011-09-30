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
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Globalization;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;

namespace Starksoft.Cryptography.OpenPGP
{

  /// <summary>
  /// GnuPG output itemType.
  /// </summary>
  public enum OutputTypes
  {
    /// <summary>
    /// Ascii armor output.
    /// </summary>
    AsciiArmor,
    /// <summary>
    /// Binary output.
    /// </summary>
    Binary
  };

  /// <summary>
  /// Interface class for GnuPG.
  /// </summary>
  /// <remarks>
  /// <para>
  /// GNU Privacy Guard from the GNU Project (also called GnuPG or GPG for short) is a highly regarded and supported opensource project that provides a complete and free implementation of the OpenPGP standard as defined by RFC2440. 
  /// GnuPG allows you to encrypt and sign your data and communication, manage your public and privde OpenPGP keys as well 
  /// as access modules for all kind of public key directories. GPG.EXE, is a command line tool that is installed with GnuPG and contains features for easy integration with other applications. 
  /// </para>
  /// <para>
  /// The Starksoft OpenPGP Component for .NET provides classes that interface with the GPG.EXE command line tool.  The Starksoft OpenPGP libraries allows any .NET application to use GPG.EXE to encrypt or decypt data using
  /// .NET IO Streams.  No temporary files are required and everything is handled through streams.  Any .NET Stream object can be used as long as the source stream can be read and the 
  /// destination stream can be written to.  But, in order for the Starksoft OpenPGP Component for .NET to work you must first install the lastest version of GnuPG which includes GPG.EXE.  
  /// You can obtain the latest version at http://www.gnupg.org/.  See the GPG.EXE tool documentation for information
  /// on how to add keys to the GPG key ring and creating your public and private keys.
  /// </para>
  /// <para>
  /// If you are new to GnuPG please install the application and then read how to generate new key pair or importing existing OpenPGP keys. 
  /// You can rad more about key generation and importing at http://www.gnupg.org/gph/en/manual.html#AEN26
  /// </para>
  /// <para>
  /// Encrypt File Example:
  /// <code>
  /// // create a new GnuPG object
  /// GnuPG gpg = new GnuPG();
  /// // specify a recipient that is already on the key-ring 
  /// gpg.Recipient = "myfriend@domain.com";
  /// // create an IO.Stream object to the source of the data and open it
  /// FileStream sourceFile = new FileStream(@"c:\temp\source.txt", FileMode.Open);
  /// // create an IO.Stream object to a where I want the encrypt data to go
  /// FileStream outputFile = new FileStream(@"c:\temp\output.txt", FileMode.Create);
  /// // encrypt the data using IO Streams - any type of input and output IO Stream can be used
  /// // as long as the source (input) stream can be read and the destination (output) stream 
  /// // can be written to
  /// gpg.Encrypt(sourceFile, outputFile);
  /// // close the files
  /// sourceFile.Close();
  /// outputFile.Close();
  /// </code>
  /// </para>
  /// <para>
  /// Decrypt File Example:
  /// <code>
  /// // create a new GnuPG object
  /// GnuPG gpg = new GnuPG();
  /// // create an IO.Stream object to the encrypted source of the data and open it 
  /// FileStream encryptedFile = new FileStream(@"c:\temp\output.txt", FileMode.Open);
  /// // create an IO.Stream object to a where you want the decrypted data to go
  /// FileStream unencryptedFile = new FileStream(@"c:\temp\unencrypted.txt", FileMode.Create);
  /// // specify our secret passphrase (if we have one)
  /// gpg.Passphrase = "secret passphrase";            
  /// // decrypt the data using IO Streams - any type of input and output IO Stream can be used
  /// // as long as the source (input) stream can be read and the destination (output) stream 
  /// // can be written to
  /// gpg.Decrypt(encryptedFile, unencryptedFile);
  /// // close the files
  /// encryptedFile.Close();
  /// unencryptedFile.Close();
  /// </code>
  /// </para>
  /// </remarks>
  public class GnuPG : IDisposable
  {
    private string _passphrase;
    private string _sender;
    private IList<string> _recipients;

    private string _homePath;
    private string _binaryPath;
    private string _gpgExe;
    private OutputTypes _outputType;
    private int _timeout = 10000; // 10 seconds
    private Process _proc;
    private bool _outputStatus;
    private bool _outputDataClosed;
    private string _userCmdOptions = "";
    private Encoding _outputDataEncoding = Encoding.ASCII;

    private Stream _outputStream;
    private Stream _errorStream;

    private const string GPG_EXECUTABLE = "gpg.exe";
    private const string GPG2_EXECUTABLE = "gpg2.exe";
    private const string GPG_REGISTRY_KEY_UNINSTALL = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\GnuPG";
    private const string GPG_REGISTRY_VALUE_INSTALLLOCATION = "InstallLocation";
    private const string GPG_REGISTRY_VALUE_DISPLAYVERSION = "DisplayVersion";
    private const string GPG_COMMON_INSTALLATION_PATH = @"C:\Program Files\GNU\GnuPG";
    private const string GPG_DEFAULT_CMD_OPTIONS = "--batch --no-tty --no-auto-check-trustdb ";

    /// <summary>
    /// GnuPG actions.
    /// </summary>
    private enum ActionTypes
    {
      /// <summary>
      /// Encrypt data.
      /// </summary>
      Encrypt,
      /// <summary>
      /// Decrypt data.
      /// </summary>
      Decrypt,
      /// <summary>
      /// Sign data.
      /// </summary>
      Sign,
      /// <summary>
      /// Verify signed data.
      /// </summary>
      Verify,
      /// <summary>
      /// Sign and Encrypt data.
      /// </summary>
      SignAndEncrypt
    };

    /// <summary>
    /// GnuPG interface class default constructor.
    /// </summary>
    /// <remarks>
    /// The GPG executable location is obtained by information in the windows registry.  Home path is set to the same as the
    /// GPG executable path.  Output itemType defaults to Ascii Armour.
    /// </remarks>
    public GnuPG()
    {
      SetDefaults();
    }

    /// <summary>
    /// GnuPG interface class constuctor.
    /// </summary>
    /// <remarks>Output itemType defaults to Ascii Armour.</remarks>
    /// <param name="homePath">The home directory where files to encrypt and decrypt are located.</param>
    /// <param name="binaryPath">The GnuPG executable binary directory.</param>
    public GnuPG(string homePath, string binaryPath)
    {
      _homePath = homePath;
      _binaryPath = binaryPath;
      SetDefaults();
    }

    /// <summary>
    /// GnuPG interface class constuctor.
    /// </summary>
    /// <param name="homePath">The home directory where files to encrypt and decrypt are located.</param>
    /// <remarks>
    /// The GPG executable location is obtained by information in the windows registry.  Output itemType defaults to Ascii Armour.
    /// </remarks>
    public GnuPG(string homePath)
    {
      _homePath = homePath;
      SetDefaults();
    }

    public bool OutputStatus
    {
      get { return _outputStatus; }
      set { _outputStatus = value; }
    }

    public string UserCmdOptions
    {
      get { return _userCmdOptions; }
      set { _userCmdOptions = value; }
    }

    /// <summary>
    /// Get or set the timeout value for the GnuPG operations in milliseconds. 
    /// </summary>
    /// <remarks>
    /// The default timeout is 10000 milliseconds (10 seconds).
    /// </remarks>
    public int Timeout
    {
      get { return _timeout; }
      set { _timeout = value; }
    }

    /// <summary>
    /// Recipient name of the encrypted data.
    /// </summary>
    public IList<string> Recipients
    {
      get { return _recipients; }
      set { _recipients = value; }
    }

    /// <summary>
    /// Sender name of the encrypted data.
    /// </summary>
    public string Sender
    {
      get { return _sender; }
      set { _sender = value; }
    }

    /// <summary>
    /// Secret passphrase text value.
    /// </summary>
    public string Passphrase
    {
      get { return _passphrase; }
      set { _passphrase = value; }
    }

    /// <summary>
    /// The itemType of output that GPG should generate.
    /// </summary>
    public OutputTypes OutputType
    {
      get { return _outputType; }
      set { _outputType = value; }
    }

    /// <summary>
    /// Path to your home directory.
    /// </summary>
    public string HomePath
    {
      get { return _homePath; }
      set { _homePath = value; }
    }

    /// <summary>
    /// Path to the location of the GPG.EXE binary executable.
    /// </summary>
    public string BinaryPath
    {
      get { return _binaryPath; }
      set { _binaryPath = value; SetExe(); }
    }

    /// <summary>
    /// Check if the required binary is available.
    /// </summary>
    /// <returns>true if present or false otherwise</returns>
    public bool BinaryExists()
    {
      return File.Exists(_gpgExe);
    }

    public bool BinaryExists(string gnuPath)
    {
      if (File.Exists(Path.Combine(gnuPath, GPG2_EXECUTABLE)))
        return true;
      if (File.Exists(Path.Combine(gnuPath, GPG_EXECUTABLE)))
        return true;
      return false;
    }

    /// <summary>
    /// Encrypt OpenPGP data using IO Streams.
    /// </summary>
    /// <param name="inputStream">Input stream data containing the data to encrypt.</param>
    /// <param name="outputStream">Output stream which will contain encrypted data.</param>
    /// <remarks>
    /// You must add the recipient's public key to your GnuPG key ring before calling this method.  Please see the GnuPG documentation for more information.
    /// </remarks>
    public void Encrypt(Stream inputStream, Stream outputStream)
    {
      if (inputStream == null)
        throw new ArgumentNullException("Argument inputStream can not be null.");

      if (outputStream == null)
        throw new ArgumentNullException("Argument outputStream can not be null.");

      if (!inputStream.CanRead)
        throw new ArgumentException("Argument inputStream must be readable.");

      if (!outputStream.CanWrite)
        throw new ArgumentException("Argument outputStream must be writable.");

      ExecuteGPG(ActionTypes.Encrypt, inputStream, outputStream, new MemoryStream());
    }

    /// <summary>
    /// Decrypt OpenPGP data using IO Streams.
    /// </summary>
    /// <param name="inputStream">Input stream containing encrypted data.</param>
    /// <param name="outputStream">Output stream which will contain decrypted data.</param>
    public void Decrypt(Stream inputStream, Stream outputStream, Stream errorStream)
    {
      if (inputStream == null)
        throw new ArgumentNullException("Argument inputStream can not be null.");

      if (outputStream == null)
        throw new ArgumentNullException("Argument outputStream can not be null.");

      if (!inputStream.CanRead)
        throw new ArgumentException("Argument inputStream must be readable.");

      if (!outputStream.CanWrite)
        throw new ArgumentException("Argument outputStream must be writable.");

      ExecuteGPG(ActionTypes.Decrypt, inputStream, outputStream, errorStream);
    }

    /// <summary>
    /// Sign input stream data with default user key.
    /// </summary>
    /// <param name="inputStream">Input stream containing data to sign.</param>
    /// <param name="outputStream">Output stream containing signed data.</param>
    public void Sign(Stream inputStream, Stream outputStream)
    {
      if (inputStream == null)
        throw new ArgumentNullException("Argument inputStream can not be null.");

      if (outputStream == null)
        throw new ArgumentNullException("Argument outputStream can not be null.");

      if (!inputStream.CanRead)
        throw new ArgumentException("Argument inputStream must be readable.");

      if (!outputStream.CanWrite)
        throw new ArgumentException("Argument outputStream must be writable.");

      ExecuteGPG(ActionTypes.Sign, inputStream, outputStream, new MemoryStream());
    }

    /// <summary>
    /// Sign and encrypt input stream data with default user key.
    /// </summary>
    /// <param name="inputStream">Input stream containing data to sign and encrypt.</param>
    /// <param name="outputStream">Output stream containing signed and encrypted data.</param>
    public void SignAndEncrypt(Stream inputStream, Stream outputStream)
    {
      if (inputStream == null)
        throw new ArgumentNullException("Argument inputStream can not be null.");

      if (outputStream == null)
        throw new ArgumentNullException("Argument outputStream can not be null.");

      if (!inputStream.CanRead)
        throw new ArgumentException("Argument inputStream must be readable.");

      if (!outputStream.CanWrite)
        throw new ArgumentException("Argument outputStream must be writable.");

      ExecuteGPG(ActionTypes.SignAndEncrypt, inputStream, outputStream, new MemoryStream());
    }

    /// <summary>
    /// Verify signed input stream data with default user key.
    /// </summary>
    /// <param name="inputStream">Input stream containing signed data to verify.</param>
    public void Verify(Stream inputStream, Stream outputStream, Stream errorStream)
    {
      if (inputStream == null)
        throw new ArgumentNullException("Argument inputStream can not be null.");

      if (outputStream == null)
        throw new ArgumentNullException("Argument outputStream can not be null.");

      if (!inputStream.CanRead)
        throw new ArgumentException("Argument inputStream must be readable.");

      if (!outputStream.CanWrite)
        throw new ArgumentException("Argument outputStream must be writable.");

      ExecuteGPG(ActionTypes.Verify, inputStream, outputStream, errorStream);
    }


    /// <summary>
    /// Retrieves a collection of secret keys from the GnuPG application.
    /// </summary>
    /// <returns>Collection of GnuPGKey objects.</returns>
    public GnuPGKeyCollection GetSecretKeys()
    {
      return new GnuPGKeyCollection(GetCommand("--list-secret-keys --display-charset utf-8"));
    }

    /// <summary>
    /// Retrieves a collection of all keys from the GnuPG application.
    /// </summary>
    /// <returns>Collection of GnuPGKey objects.</returns>
    public GnuPGKeyCollection GetKeys()
    {
      return new GnuPGKeyCollection(GetCommand("--list-keys --display-charset utf-8"));
    }

    private StreamReader GetCommand(string command)
    {
      StringBuilder options = new StringBuilder();

      // Apply default command line options.
      options.Append(GPG_DEFAULT_CMD_OPTIONS);

      //  set a home directory if the user specifies one
      if (!string.IsNullOrEmpty(_homePath))
        options.Append(String.Format(CultureInfo.InvariantCulture, "--homedir \"{0}\" ", _homePath));

      options.Append(command);

#if OFF
      string gpgExe = Path.Combine(GetGPGInstallLocation(), GPG2_EXECUTABLE);
      if (!File.Exists(gpgExe))
        gpgExe = Path.Combine(GetGPGInstallLocation(), GPG_EXECUTABLE);
      if (!File.Exists(gpgExe))
        throw new GnuPGException(String.Format(CultureInfo.InvariantCulture, "Unable to find GPG.EXE.  The path '{0}' does not contain the GPG(2).EXE executable program.  If this path is incorrect, please set the binary path on the GnuPG object to the correct GnuPG directory", GetGPGInstallLocation()));
#else
      if (string.IsNullOrEmpty(_gpgExe))
        throw new GnuPGException(String.Format(CultureInfo.InvariantCulture, "Unable to find GPG(2).EXE.  The path '{0}' does not contain the GPG(2).EXE executable program.  If this path is incorrect, please set the binary path on the GnuPG object to the correct GnuPG directory", GetGPGInstallLocation()));
#endif

      //  create a process info object with command line options
      ProcessStartInfo procInfo = new ProcessStartInfo(_gpgExe, options.ToString());

      //  init the procInfo object
      procInfo.CreateNoWindow = true;
      procInfo.UseShellExecute = false;
      procInfo.RedirectStandardInput = true;
      procInfo.RedirectStandardOutput = true;
      procInfo.RedirectStandardError = true;

      if (_gpgExe.EndsWith(GPG2_EXECUTABLE) == true && procInfo.EnvironmentVariables.ContainsKey("LANG") == false)
        procInfo.EnvironmentVariables.Add("LANG", "C");

      if (command.Contains("utf-8"))
        procInfo.StandardOutputEncoding = _outputDataEncoding = Encoding.UTF8;
      else
        procInfo.StandardOutputEncoding = _outputDataEncoding = Encoding.ASCII;

      MemoryStream outputStream = new MemoryStream();
      _outputStream = outputStream;
      try
      {
        //  start the gpg process and get back a process start info object
        _proc = Process.Start(procInfo);

        //  push passphrase onto stdin with a CRLF
        //_proc.StandardInput.WriteLine("");
        _proc.StandardInput.Flush();

        // Prepare asynchronous reading of standard output (to handle long output of gpg command)
        _outputDataClosed = false;
        _proc.OutputDataReceived += new DataReceivedEventHandler(GetCommandOutputDataReceived);
        _proc.BeginOutputReadLine();

        //  wait for the process to return with an exit code (with a timeout variable)
        if (!_proc.WaitForExit(Timeout))
        {
          throw new GnuPGException("A time out event occurred while executing the GPG program.");
        }

        //  if the process exit code is not 0 then read the error text from the gpg.exe process and throw an exception
        if (_proc.ExitCode != 0)
          throw new GnuPGException(_proc.StandardError.ReadToEnd());

        // Wait and of process output stream...
        while (_outputDataClosed == false) { }

        // No longer required with asynchronous reading.
        // CopyStream(_proc.StandardOutput.BaseStream, outputStream);
      }
      catch (Exception exp)
      {
        throw new GnuPGException(String.Format("An error occurred while trying to execute command {0}.\r\n{1}", command, exp.Message));
      }
      finally
      {
        Dispose();
      }

      StreamReader reader = new StreamReader(outputStream);
      reader.BaseStream.Position = 0;
      return reader;
    }

    private void GetCommandOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
      if (e.Data != null)
      {
        Byte[] bytes = _outputDataEncoding.GetBytes(e.Data + Environment.NewLine);
        _outputStream.Write(bytes, 0, bytes.Length);
      }
      else
      {
        _outputDataClosed = true;
      }
    }

    private string GetCmdLineSwitches(ActionTypes action)
    {
      StringBuilder options = new StringBuilder();

      // Apply default command line options.
      options.Append(GPG_DEFAULT_CMD_OPTIONS);

      //  set a home directory if the user specifies one
      if (_homePath != null && _homePath.Length != 0)
        options.Append(String.Format(CultureInfo.InvariantCulture, "--homedir \"{0}\" ", _homePath));

      //  read the passphrase from the standard input
      options.Append("--passphrase-fd 0 ");

      //  turn off verbose statements
      options.Append("--no-verbose ");

      //  always use the trusted model so we don't get an interactive session with gpg.exe
      //options.Append("--trust-model always ");
      // Note: better be provided by the user in the UserCmdOptions property.

      // Apply user defined command options.
      if (string.IsNullOrEmpty(_userCmdOptions) != true)
        options.Append(_userCmdOptions + " ");

      if (_outputStatus)
        options.Append("--status-fd 1 ");


      //  handle the action
      switch (action)
      {
        case ActionTypes.SignAndEncrypt:
          if ((_recipients == null || _recipients.Count == 0) && action == ActionTypes.Encrypt)
            throw new GnuPGException("A Recipient is required before encrypting data.  Please specify a valid recipient list using the Recipients property on the GnuPG object.");

          //  check to see if the user wants ascii armor output or binary output (binary is the default mode for gpg)
          if (_outputType == OutputTypes.AsciiArmor)
            options.Append("--armor ");

          options.Append("--encrypt ");
          foreach (string recipient in _recipients)
            options.Append(String.Format(CultureInfo.InvariantCulture, "--recipient {0} ", recipient));

          options.Append(" --sign -u ");
          options.AppendFormat(_sender);

          break;
        case ActionTypes.Encrypt:
          if ((_recipients == null || _recipients.Count == 0) && action == ActionTypes.Encrypt)
            throw new GnuPGException("A Recipient is required before encrypting data.  Please specify a valid recipient list using the Recipients property on the GnuPG object.");

          //  check to see if the user wants ascii armor output or binary output (binary is the default mode for gpg)
          if (_outputType == OutputTypes.AsciiArmor)
            options.Append("--armor ");

          options.Append("--encrypt ");
          foreach (string recipient in _recipients)
            options.Append(String.Format(CultureInfo.InvariantCulture, "--recipient {0} ", recipient));

          break;
        case ActionTypes.Decrypt:
          options.Append("--decrypt ");
          break;
        case ActionTypes.Sign:
          if (_outputType == OutputTypes.AsciiArmor)
            options.Append("--clearsign -u ");
          else
            options.Append("--sign -u ");

          options.AppendFormat(_sender);
          break;
        case ActionTypes.Verify:
          options.Append("--verify ");
          break;
      }

      return options.ToString();
    }

    private void ExecuteGPG(ActionTypes action, Stream inputStream, Stream outputStream, Stream errorStream)
    {
      string gpgErrorText = string.Empty;

#if OFF
      string gpgExe = Path.Combine(GetGPGInstallLocation(), GPG2_EXECUTABLE);
      if (!File.Exists(gpgExe))
        gpgExe = Path.Combine(GetGPGInstallLocation(), GPG_EXECUTABLE);
      if (!File.Exists(gpgExe))
        throw new GnuPGException(String.Format(CultureInfo.InvariantCulture, "Unable to find GPG.EXE.  The path '{0}' does not contain the GPG.EXE executable program.  If this path is incorrect, please set the binary path on the GnuPG object to the correct GnuPG directory", GetGPGInstallLocation()));
#else
      if (string.IsNullOrEmpty(_gpgExe))
        throw new GnuPGException(String.Format(CultureInfo.InvariantCulture, "Unable to find GPG(2).EXE.  The path '{0}' does not contain the GPG(2).EXE executable program.  If this path is incorrect, please set the binary path on the GnuPG object to the correct GnuPG directory", GetGPGInstallLocation()));
#endif

      //  create a process info object with command line options
      ProcessStartInfo procInfo = new ProcessStartInfo(_gpgExe, GetCmdLineSwitches(action));

      //  init the procInfo object
      procInfo.CreateNoWindow = true;
      procInfo.UseShellExecute = false;
      procInfo.RedirectStandardInput = true;
      procInfo.RedirectStandardOutput = true;
      procInfo.RedirectStandardError = true;

      if (_gpgExe.EndsWith(GPG2_EXECUTABLE) == true && procInfo.EnvironmentVariables.ContainsKey("LANG") == false)
        procInfo.EnvironmentVariables.Add("LANG", "C");

      try
      {
        //  start the gpg process and get back a process start info object
        _proc = Process.Start(procInfo);

        //  push passphrase onto stdin with a CRLF
        _proc.StandardInput.WriteLine(_passphrase);
        _proc.StandardInput.Flush();

        _outputStream = outputStream;
        _errorStream = errorStream;

        // set up threads to run the output stream and error stream asynchronously
        ThreadStart outputEntry = new ThreadStart(AsyncOutputReader);
        Thread outputThread = new Thread(outputEntry);
        outputThread.Name = "GnuPG Output Thread";
        outputThread.Start();
        ThreadStart errorEntry = new ThreadStart(AsyncErrorReader);
        Thread errorThread = new Thread(errorEntry);
        errorThread.Name = "GnuPG Error Thread";
        errorThread.Start();

        //  copy the input stream to the process standard input object
        CopyStream(inputStream, _proc.StandardInput.BaseStream);

        _proc.StandardInput.Flush();

        // close the process standard input object
        _proc.StandardInput.Close();

        //  wait for the process to return with an exit code (with a timeout variable)
        if (!_proc.WaitForExit(_timeout))
        {
          throw new GnuPGException("A time out event occurred while executing the GPG program.");
        }

        if (!outputThread.Join(_timeout / 2))
          outputThread.Abort();

        if (!errorThread.Join(_timeout / 2))
          errorThread.Abort();

        //  if the process exit code is not 0 then read the error text from the gpg.exe process 
        if (_proc.ExitCode != 0)
        {
          StreamReader rerror = new StreamReader(_errorStream);
          _errorStream.Position = 0;
          gpgErrorText = rerror.ReadToEnd();
        }

      }
      catch (Exception exp)
      {
        throw new GnuPGException(String.Format(CultureInfo.InvariantCulture, "An error occurred while trying to {0} data using GnuPG.  GPG.EXE command switches used: {1}.\r\n{2}", action.ToString(), procInfo.Arguments, exp.Message), exp);
      }
      finally
      {
        Dispose();
      }

      // throw an exception with the error information from the gpg.exe process
      if (gpgErrorText.IndexOf("bad passphrase") != -1)
        throw new GnuPGBadPassphraseException(gpgErrorText);

      if (gpgErrorText.Length > 0)
        throw new GnuPGException(gpgErrorText);
    }

    private string GetGPGInstallLocation()
    {
      // test to see if the binary path is set
      if (string.IsNullOrEmpty(_binaryPath) != true)
        return _binaryPath;

      RegistryKey hKeyLM = Registry.LocalMachine;
      string installLocation;

      try
      {
        hKeyLM = hKeyLM.OpenSubKey(GPG_REGISTRY_KEY_UNINSTALL);
        installLocation = (string)hKeyLM.GetValue(GPG_REGISTRY_VALUE_INSTALLLOCATION);
      }
      catch (Exception)
      {
        // if no registry path information is found try using the common path
        installLocation = GPG_COMMON_INSTALLATION_PATH;
      }
      finally
      {
        hKeyLM.Close();
      }

      // set the binary path and then return the value
      _binaryPath = installLocation;
      return installLocation;
    }

    private void CopyStream(Stream input, Stream output)
    {
      if (_asyncWorker != null && _asyncWorker.CancellationPending)
        return;

      const int BUFFER_SIZE = 4096;
      byte[] bytes = new byte[BUFFER_SIZE];
      int i;
      while ((i = input.Read(bytes, 0, bytes.Length)) != 0)
      {
        if (_asyncWorker != null && _asyncWorker.CancellationPending)
          break;
        output.Write(bytes, 0, i);
      }
    }

    private void SetDefaults()
    {
      _outputType = OutputTypes.AsciiArmor;
      SetExe();
    }

    private void SetExe()
    {
      _gpgExe = Path.Combine(GetGPGInstallLocation(), GPG2_EXECUTABLE);
      if (!File.Exists(_gpgExe))
        _gpgExe = Path.Combine(GetGPGInstallLocation(), GPG_EXECUTABLE);
      if (!File.Exists(_gpgExe))
        _gpgExe = string.Empty;
    }

    private void AsyncOutputReader()
    {
      Stream input = _proc.StandardOutput.BaseStream;
      Stream output = _outputStream;

      const int BUFFER_SIZE = 4096;
      byte[] bytes = new byte[BUFFER_SIZE];
      int i;
      while ((i = input.Read(bytes, 0, bytes.Length)) != 0)
      {
        output.Write(bytes, 0, i);
      }


      //lock (this)
      //{
      //    CopyStream(_proc.StandardOutput.BaseStream, _outputStream); 
      //}
    }

    private void AsyncErrorReader()
    {
      Stream input = _proc.StandardError.BaseStream;
      Stream output = _errorStream;

      const int BUFFER_SIZE = 4096;
      byte[] bytes = new byte[BUFFER_SIZE];
      int i;
      while ((i = input.Read(bytes, 0, bytes.Length)) != 0)
      {
        output.Write(bytes, 0, i);
      }

      //lock (this)
      //{
      //    CopyStream(_proc.StandardError.BaseStream, _errorStream); 
      //}
    }

    /// <summary>
    /// Dispose method for the GnuPG inteface class.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose method for the GnuPG interface class.
    /// </summary>       
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_proc != null)
        {
          //  close all the streams except for our the output stream
          _proc.StandardInput.Close();
          // No longer allowed with asynchronous reading.
          // _proc.StandardOutput.Close();
          _proc.StandardError.Close();
          _proc.Close();
        }
      }

      if (_proc != null)
        _proc.Dispose();
    }

    /// <summary>
    /// Destructor method for the GnuPG interface class.
    /// </summary>
    ~GnuPG()
    {
      Dispose(false);
    }

    #region Asynchronous Methods

    private BackgroundWorker _asyncWorker;
    private Exception _asyncException;
    bool _asyncCancelled;

    /// <summary>
    /// Gets a value indicating whether an asynchronous operation is running.
    /// </summary>
    /// <remarks>Returns true if an asynchronous operation is running; otherwise, false.
    /// </remarks>
    public bool IsBusy
    {
      get { return _asyncWorker == null ? false : _asyncWorker.IsBusy; }
    }

    /// <summary>
    /// Gets a value indicating whether an asynchronous operation is cancelled.
    /// </summary>
    /// <remarks>Returns true if an asynchronous operation is cancelled; otherwise, false.
    /// </remarks>
    public bool IsAsyncCancelled
    {
      get { return _asyncCancelled; }
    }

    /// <summary>
    /// Cancels any asychronous operation that is currently active.
    /// </summary>
    public void CancelAsync()
    {
      if (_asyncWorker != null && !_asyncWorker.CancellationPending && _asyncWorker.IsBusy)
      {
        _asyncCancelled = true;
        _asyncWorker.CancelAsync();
      }
    }

    private void CreateAsyncWorker()
    {
      if (_asyncWorker != null)
        _asyncWorker.Dispose();
      _asyncException = null;
      _asyncWorker = null;
      _asyncCancelled = false;
      _asyncWorker = new BackgroundWorker();
    }

    /// <summary>
    /// Event handler for EncryptAsync method completed.
    /// </summary>
    public event EventHandler<EncryptAsyncCompletedEventArgs> EncryptAsyncCompleted;

    /// <summary>
    /// Starts asynchronous execution to encrypt OpenPGP data using IO Streams.
    /// </summary>
    /// <param name="inputStream">Input stream data containing the data to encrypt.</param>
    /// <param name="outputStream">Output stream which will contain encrypted data.</param>
    /// <remarks>
    /// You must add the recipient's public key to your GnuPG key ring before calling this method.  Please see the GnuPG documentation for more information.
    /// </remarks>
    public void EncryptAsync(Stream inputStream, Stream outputStream)
    {
      if (_asyncWorker != null && _asyncWorker.IsBusy)
        throw new InvalidOperationException("The GnuPG object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

      CreateAsyncWorker();
      _asyncWorker.WorkerSupportsCancellation = true;
      _asyncWorker.DoWork += new DoWorkEventHandler(EncryptAsync_DoWork);
      _asyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EncryptAsync_RunWorkerCompleted);
      Object[] args = new Object[2];
      args[0] = inputStream;
      args[1] = outputStream;
      _asyncWorker.RunWorkerAsync(args);
    }

    private void EncryptAsync_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        Object[] args = (Object[])e.Argument;
        Encrypt((Stream)args[0], (Stream)args[1]);
      }
      catch (Exception ex)
      {
        _asyncException = ex;
      }
    }

    private void EncryptAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (EncryptAsyncCompleted != null)
        EncryptAsyncCompleted(this, new EncryptAsyncCompletedEventArgs(_asyncException, _asyncCancelled));
    }

    /// <summary>
    /// Event handler for DecryptAsync completed.
    /// </summary>
    public event EventHandler<DecryptAsyncCompletedEventArgs> DecryptAsyncCompleted;

    /// <summary>
    /// Starts asynchronous execution to decrypt OpenPGP data using IO Streams.
    /// </summary>
    /// <param name="inputStream">Input stream containing encrypted data.</param>
    /// <param name="outputStream">Output stream which will contain decrypted data.</param>
    public void DecryptAsync(Stream inputStream, Stream outputStream)
    {
      if (_asyncWorker != null && _asyncWorker.IsBusy)
        throw new InvalidOperationException("The GnuPG object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

      CreateAsyncWorker();
      _asyncWorker.WorkerSupportsCancellation = true;
      _asyncWorker.DoWork += new DoWorkEventHandler(DecryptAsync_DoWork);
      _asyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DecryptAsync_RunWorkerCompleted);
      Object[] args = new Object[2];
      args[0] = inputStream;
      args[1] = outputStream;
      _asyncWorker.RunWorkerAsync(args);
    }

    private void DecryptAsync_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        Object[] args = (Object[])e.Argument;
        Decrypt((Stream)args[0], (Stream)args[1], new MemoryStream());
      }
      catch (Exception ex)
      {
        _asyncException = ex;
      }
    }

    private void DecryptAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (DecryptAsyncCompleted != null)
        DecryptAsyncCompleted(this, new DecryptAsyncCompletedEventArgs(_asyncException, _asyncCancelled));
    }

    /// <summary>
    /// Event handler for SignAsync completed.
    /// </summary>
    public event EventHandler<SignAsyncCompletedEventArgs> SignAsyncCompleted;

    /// <summary>
    /// Starts asynchronous execution to Sign OpenPGP data using IO Streams.
    /// </summary>
    /// <param name="inputStream">Input stream containing data to sign.</param>
    /// <param name="outputStream">Output stream which will contain Signed data.</param>
    public void SignAsync(Stream inputStream, Stream outputStream)
    {
      if (_asyncWorker != null && _asyncWorker.IsBusy)
        throw new InvalidOperationException("The GnuPG object is already busy executing another asynchronous operation.  You can only execute one asychronous method at a time.");

      CreateAsyncWorker();
      _asyncWorker.WorkerSupportsCancellation = true;
      _asyncWorker.DoWork += new DoWorkEventHandler(SignAsync_DoWork);
      _asyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SignAsync_RunWorkerCompleted);
      Object[] args = new Object[2];
      args[0] = inputStream;
      args[1] = outputStream;
      _asyncWorker.RunWorkerAsync(args);
    }

    private void SignAsync_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        Object[] args = (Object[])e.Argument;
        Sign((Stream)args[0], (Stream)args[1]);
      }
      catch (Exception ex)
      {
        _asyncException = ex;
      }
    }

    private void SignAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (SignAsyncCompleted != null)
        SignAsyncCompleted(this, new SignAsyncCompletedEventArgs(_asyncException, _asyncCancelled));
    }





    #endregion

  }
}
