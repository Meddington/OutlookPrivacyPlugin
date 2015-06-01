using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Org.BouncyCastle.Bcpg.OpenPgp;

using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Reflection;
using Exception = System.Exception;
using MimeKit;

using Deja.Crypto.BcPgp;
using NLog;
using NLog.Config;
using NLog.Targets;
using OutlookPrivacyPlugin.Language;

namespace OutlookPrivacyPlugin
{
    public partial class OutlookPrivacyPlugin
    {
		#region VSTO generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(OutlookGnuPG_Startup);
			this.Shutdown += new System.EventHandler(OutlookGnuPG_Shutdown);
		}

		#endregion

		static NLog.Logger logger = LogManager.GetCurrentClassLogger();


		private Properties.Settings _settings;
		private GnuPGCommandBar _gpgCommandBar = null;
		private bool _autoDecrypt = false;
		private Outlook.Explorers _explorers;
		private Outlook.Inspectors _inspectors;        // Outlook inspectors collection
		private System.Text.Encoding _encoding = UTF8Encoding.UTF8;
		// This dictionary holds our Wrapped Inspectors, Explorers, MailItems
		private Dictionary<Guid, object> _WrappedObjects;

        private static Dictionary<string, Dictionary<string, object>> _conversationState =
            new Dictionary<string, Dictionary<string, object>>(); 

		//char[] Passphrase { get; set; }
		/// <summary>
		/// Cache of passphrases. Key is keyid.
		/// </summary>
		Dictionary<long, char[]> PassphraseCache = new Dictionary<long, char[]>();

		// PGP Headers
		// http://www.ietf.org/rfc/rfc4880.txt page 54
		const string _pgpSignedHeader = "BEGIN PGP SIGNED MESSAGE";
		const string _pgpEncryptedHeader = "BEGIN PGP MESSAGE";
		const string _pgpHeaderPattern = "BEGIN PGP( SIGNED)? MESSAGE";

		private void OutlookGnuPG_Startup(object sender, EventArgs e)
		{
			// NLOG

			//var nconfig = new LoggingConfiguration();
			//var consoleTarget = new FileTarget();
			//consoleTarget.FileName = "c:\\temp\\opp.txt";
			//nconfig.AddTarget("console", consoleTarget);
			//consoleTarget.Layout = "${logger} ${message}";

			//var rule = new LoggingRule("*", LogLevel.Trace, consoleTarget);
			//nconfig.LoggingRules.Add(rule);

			//LogManager.Configuration = nconfig;

			// NLOG END


			_settings = new Properties.Settings();

			_WrappedObjects = new Dictionary<Guid, object>();

			// Initialize command bar.
			// Must be saved/closed in Explorer MyClose event.
			// See http://social.msdn.microsoft.com/Forums/en-US/vsto/thread/df53276b-6b44-448f-be86-7dd46c3786c7/
			AddGnuPGCommandBar(this.Application.ActiveExplorer());

			// Register an event for ItemSend
			Application.ItemSend += Application_ItemSend;

			// Initialize the outlook explorers
			_explorers = this.Application.Explorers;
			_explorers.NewExplorer += new Outlook.ExplorersEvents_NewExplorerEventHandler(OutlookGnuPG_NewExplorer);
			for (int i = _explorers.Count; i >= 1; i--)
			{
				WrapExplorer(_explorers[i]);
			}

			// Initialize the outlook inspectors
			_inspectors = this.Application.Inspectors;
			_inspectors.NewInspector += new Outlook.InspectorsEvents_NewInspectorEventHandler(OutlookGnuPG_NewInspector);
		}

		/// <summary>
		/// Shutdown the Add-In.
		/// Note: some closing statements must happen before this event, see OutlookGnuPG_ExplorerClose().
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OutlookGnuPG_Shutdown(object sender, EventArgs e)
		{
			// Unhook event handler
			_inspectors.NewInspector -= new Outlook.InspectorsEvents_NewInspectorEventHandler(OutlookGnuPG_NewInspector);
			_explorers.NewExplorer -= new Outlook.ExplorersEvents_NewExplorerEventHandler(OutlookGnuPG_NewExplorer);

			_WrappedObjects.Clear();
			_WrappedObjects = null;
			_inspectors = null;
			_explorers = null;
		}

		string GetResourceText(string key)
		{
			try
			{
				var a = Assembly.GetExecutingAssembly();
				var s = a.GetManifestResourceStream(key);

				byte[] buff = new byte[s.Length];
				var cnt = s.Read(buff, 0, buff.Length);

				return UTF8Encoding.UTF8.GetString(buff, 0, cnt);
			}
			catch
			{
				MessageBox.Show("Error, unable to get resource text for key '{0}'.", key);
				return string.Format("Error, unable to get resource text for key '{0}'.", key);
			}
		}

		private GnuPGRibbon ribbon;

		protected override object RequestService(Guid serviceGuid)
		{
			if (serviceGuid == typeof(Office.IRibbonExtensibility).GUID)
			{
				if (ribbon == null)
					ribbon = new GnuPGRibbon();
				return ribbon;
			}

			return base.RequestService(serviceGuid);
		}

		#region Explorer Logic
		/// <summary>
		/// The NewExplorer event fires whenever a new explorer is created. We use
		/// this event to toggle the visibility of the commandbar.
		/// </summary>
		/// <param name="explorer">the new created Explorer</param>
		void OutlookGnuPG_NewExplorer(Outlook.Explorer explorer)
		{
			WrapExplorer(explorer);
		}

		/// <summary>
		/// Wrap Explorer object to managed Explorer events.
		/// </summary>
		/// <param name="explorer">the outlook explorer to manage</param>
		private void WrapExplorer(Outlook.Explorer explorer)
		{
			if (_WrappedObjects.ContainsValue(explorer) == true)
				return;

			ExplorerWrapper wrappedExplorer = new ExplorerWrapper(explorer);
			wrappedExplorer.Dispose += new OutlookWrapperDisposeDelegate(ExplorerWrapper_Dispose);
			wrappedExplorer.ViewSwitch += new ExplorerViewSwitchDelegate(wrappedExplorer_ViewSwitch);
			wrappedExplorer.SelectionChange += new ExplorerSelectionChangeDelegate(wrappedExplorer_SelectionChange);
			wrappedExplorer.Close += new ExplorerCloseDelegate(wrappedExplorer_Close);
			_WrappedObjects[wrappedExplorer.Id] = explorer;

			AddGnuPGCommandBar(explorer);
		}

		/// <summary>
		/// WrapEvent to dispose the wrappedExplorer
		/// </summary>
		/// <param name="id">the UID of the wrappedExplorer</param>
		/// <param name="o">the wrapped Explorer object</param>
		private void ExplorerWrapper_Dispose(Guid id, object o)
		{
			ExplorerWrapper wrappedExplorer = o as ExplorerWrapper;
			wrappedExplorer.Dispose -= new OutlookWrapperDisposeDelegate(ExplorerWrapper_Dispose);
			wrappedExplorer.ViewSwitch -= new ExplorerViewSwitchDelegate(wrappedExplorer_ViewSwitch);
			wrappedExplorer.SelectionChange -= new ExplorerSelectionChangeDelegate(wrappedExplorer_SelectionChange);
			wrappedExplorer.Close -= new ExplorerCloseDelegate(wrappedExplorer_Close);
			_WrappedObjects.Remove(id);
		}

		/// <summary>
		/// WrapEvent fired for MyClose event.
		/// </summary>
		/// <param name="explorer">the explorer for which a close event fired.</param>
		void wrappedExplorer_Close(Outlook.Explorer explorer)
		{
			if (_gpgCommandBar != null && explorer == _gpgCommandBar.Explorer)
				_gpgCommandBar.SavePosition(_settings);
		}

		/// <summary>
		/// WrapEvent fired for ViewSwitch event.
		/// </summary>
		/// <param name="explorer">the explorer for which a switchview event fired.</param>
		void wrappedExplorer_ViewSwitch(Outlook.Explorer explorer)
		{
			if (_gpgCommandBar == null)
				return;
			if (explorer.CurrentFolder.DefaultMessageClass == "IPM.Note")
			{
				_gpgCommandBar.CommandBar.Visible = true;
			}
			else
			{
				_gpgCommandBar.CommandBar.Visible = false;
			}
		}

		/// <summary>
		/// WrapEvent fired for SelectionChange event.
		/// </summary>
		/// <param name="explorer">the explorer for which a selectionchange event fired.</param>
		void wrappedExplorer_SelectionChange(Outlook.Explorer explorer)
		{
			Outlook.Selection Selection = explorer.Selection;
			if (Selection.Count != 1)
				return;

			Outlook.MailItem mailItem = Selection[1] as Outlook.MailItem;
			if (mailItem == null || mailItem.Body == null)
				return;

			if (mailItem.BodyFormat == Outlook.OlBodyFormat.olFormatPlain)
			{
				Match match = Regex.Match(mailItem.Body, _pgpHeaderPattern);

				_gpgCommandBar.GetButton("Verify").Enabled = (match.Value == _pgpSignedHeader);
				_gpgCommandBar.GetButton("Decrypt").Enabled = (match.Value == _pgpEncryptedHeader);
			}
			else
			{
				_gpgCommandBar.GetButton("Verify").Enabled = false;
				_gpgCommandBar.GetButton("Decrypt").Enabled = false;
			}
		}
		#endregion

		#region Inspector Logic
		/// <summary>
		/// The NewInspector event fires whenever a new inspector is displayed. We use
		/// this event to initialize button to mail item inspectors.
		/// The inspector logic handles the registration and execution of mailItem
		/// events (Open, MyClose and Write) to initialize, maintain and save the
		/// ribbon button states per mailItem.
		/// </summary>
		/// <param name="Inspector">the new created Inspector</param>
		private void OutlookGnuPG_NewInspector(Outlook.Inspector inspector)
		{
			var mailItem = inspector.CurrentItem as Outlook.MailItem;
			if (mailItem != null)
			{
				WrapMailItem(inspector);
			}
		}

		/// <summary>
		/// Wrap mailItem object to managed mailItem events.
		/// </summary>
		/// <param name="explorer">the outlook explorer to manage</param>
		private void WrapMailItem(Outlook.Inspector inspector)
		{
			if (_WrappedObjects.ContainsValue(inspector) == true)
				return;

			var wrappedMailItem = new MailItemInspector(inspector);
			wrappedMailItem.Dispose += new OutlookWrapperDisposeDelegate(MailItemInspector_Dispose);
			wrappedMailItem.MyClose += new MailItemInspectorCloseDelegate(mailItem_Close);
			wrappedMailItem.Open += new MailItemInspectorOpenDelegate(mailItem_Open);
			wrappedMailItem.Save += new MailItemInspectorSaveDelegate(mailItem_Save);
			wrappedMailItem.Close += new InspectorCloseDelegate(wrappedMailItem_Close);
			_WrappedObjects[wrappedMailItem.Id] = inspector;
		}

		void wrappedMailItem_Close(Outlook.Inspector inspector)
		{
			//string a = "A";
		}

		/// <summary>
		/// WrapEvent to dispose the wrappedMailItem
		/// </summary>
		/// <param name="id">the UID of the wrappedMailItem</param>
		/// <param name="o">the wrapped mailItem object</param>
		private void MailItemInspector_Dispose(Guid id, object o)
		{
			var wrappedMailItem = o as MailItemInspector;
			wrappedMailItem.Dispose -= new OutlookWrapperDisposeDelegate(MailItemInspector_Dispose);
			wrappedMailItem.MyClose -= new MailItemInspectorCloseDelegate(mailItem_Close);
			wrappedMailItem.Open -= new MailItemInspectorOpenDelegate(mailItem_Open);
			wrappedMailItem.Save -= new MailItemInspectorSaveDelegate(mailItem_Save);
			_WrappedObjects.Remove(id);
		}

        /// <summary>
        /// Helper function to read the content type from the email.
        /// </summary>
        /// <param name="mailItem"></param>
        /// <returns></returns>
        private string ReadContentType(Outlook.MailItem mailItem)
        {
            object contentType = null;
            try
            {
                contentType = mailItem.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/content-type/0x0000001F");
            }
            catch (Exception)
            {
                object[] schemanames = { "http://schemas.microsoft.com/mapi/proptag/0x007D001F" };
                object[] allProperties = mailItem.PropertyAccessor.GetProperties(schemanames);
                string allHeaders = (string)allProperties[0];
                foreach (var str in allHeaders.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
                {
                    const string strContentType = "Content-Type: ";
                    if (str.StartsWith(strContentType))
                        contentType = str.Substring(strContentType.Length);
                }
            }
            return (string)contentType;
        }

		/// <summary>
		/// WrapperEvent fired when a mailItem is opened.
		/// This handler is designed to initialize the state of the compose button
		/// states (Sign/Encrypt) with recorded values, if present, or with default
		/// settings values.
		/// </summary>
		/// <param name="mailItem">the opened mailItem</param>
		/// <param name="Cancel">False when the event occurs. If the event procedure sets this argument to True,
		/// the open operation is not completed and the inspector is not displayed.</param>
		void mailItem_Open(Outlook.MailItem mailItem, ref bool Cancel)
		{
			if (mailItem == null)
				return;

            // TODO - use dictionary of conversation id to mapp these properties!
            //mailItem.ConversationID

			_encoding = Encoding.GetEncoding(mailItem.InternetCodepage);

			// New mail (Compose)
			if (mailItem.Sent == false)
			{
				ribbon.SignButton.Checked = _settings.AutoSign || (bool) GetProperty(mailItem, "GnuPGSetting.Sign", false);
                ribbon.EncryptButton.Checked = _settings.AutoEncrypt || (bool)GetProperty(mailItem, "GnuPGSetting.Encrypt", false);

				if (mailItem.Body.IndexOf("\n** " + Localized.MsgDecryptValidSig) > -1)
				{
					ribbon.SignButton.Checked = true;
					ribbon.EncryptButton.Checked = true;
				}
				else if (mailItem.Body.IndexOf("\n** " + Localized.MsgDecrypt) > -1)
				{
					ribbon.EncryptButton.Checked = true;
				}

				SetProperty(mailItem, "GnuPGSetting.Sign", ribbon.SignButton.Checked);
				SetProperty(mailItem, "GnuPGSetting.Encrypt", ribbon.EncryptButton.Checked);

				ribbon.InvalidateButtons();

				if (ribbon.EncryptButton.Checked || ribbon.SignButton.Checked)
					mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatPlain;
			}
			else
			// Read mail
			{
                SetProperty(mailItem, "GnuPGSetting.Sign", false);
                SetProperty(mailItem, "GnuPGSetting.Encrypt", false);

                // Default: disable read-buttons
				ribbon.DecryptButton.Enabled = ribbon.VerifyButton.Enabled = false;

				// Look for PGP headers
				Match match = null;
				if (mailItem.Body != null)
					match = Regex.Match(mailItem.Body, _pgpHeaderPattern);

				if (match != null && (_autoDecrypt || _settings.AutoDecrypt) && match.Value == _pgpEncryptedHeader)
				{
					if (mailItem.BodyFormat != Outlook.OlBodyFormat.olFormatPlain)
					{
						StringBuilder body = new StringBuilder(mailItem.Body);

						Stack<int> indexes = new Stack<int>();
						for (int cnt = 0; cnt < body.Length; cnt++)
						{
							if (body[cnt] > 127)
								indexes.Push(cnt);
						}

						while (true)
						{
							if (indexes.Count == 0)
								break;

							int index = indexes.Pop();
							body.Remove(index, 1);
						}

						mailItem.Body = body.ToString();
						//mailItem.Save();
					}

					_autoDecrypt = false;
					DecryptEmail(mailItem);
					// Update match again, in case decryption failed/cancelled.
					match = Regex.Match(mailItem.Body, _pgpHeaderPattern);

					SetProperty(mailItem, "GnuPGSetting.Decrypted", true);
				}
				else if (match != null && _settings.AutoVerify && match.Value == _pgpSignedHeader)
				{
					if (mailItem.BodyFormat != Outlook.OlBodyFormat.olFormatPlain)
					{
						StringBuilder body = new StringBuilder(mailItem.Body);
						mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatPlain;

						Stack<int> indexes = new Stack<int>();
						for (int cnt = 0; cnt < body.Length; cnt++)
						{
							if (body[cnt] > 127)
								indexes.Push(cnt);
						}

						while (true)
						{
							if (indexes.Count == 0)
								break;

							int index = indexes.Pop();
							body.Remove(index, 1);
						}

						mailItem.Body = body.ToString();
						//mailItem.Save();
					}

					VerifyEmail(mailItem);
				}
				else
				{
					bool foundPgpMime = false;
					string sigHash = "sha1";
					Microsoft.Office.Interop.Outlook.Attachment encryptedMime = null;
					Microsoft.Office.Interop.Outlook.Attachment sigMime = null;

					var contentType = ReadContentType(mailItem);

					logger.Trace("MIME: Message content-type: " + (string)contentType);

					if (((string)contentType).Contains("application/pgp-signature"))
					{
						// PGP MIM Signed message it looks like
						//multipart/signed; micalg=pgp-sha1; protocol="application/pgp-signature"; boundary="Iq9CNK2GBN9g0PCsVJK4WdkEAR0887CbX"; charset="iso-8859-1"

						logger.Trace("MIME: Found application/pgp-signature: " + contentType);

						var sigMatch = Regex.Match((string)contentType, @"micalg=pgp-([^; ]*)");
						sigHash = sigMatch.Groups[1].Value;

						logger.Trace("MIME: sigHash: " + sigHash);
					}

					// Look for PGP MIME
					foreach (Microsoft.Office.Interop.Outlook.Attachment attachment in mailItem.Attachments)
					{
						var mimeEncoding = attachment.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x370E001F");

						logger.Trace("MIME: Attachment type: " + mimeEncoding);

						if (mimeEncoding == "application/pgp-encrypted")
						{
							logger.Trace("MIME: Found application/pgp-encrypted.");
							foundPgpMime = true;
						}
						else if (mimeEncoding == "application/pgp-signature")
						{
							logger.Trace("MIME: Found application/pgp-signature");
							sigMime = attachment;
						}
						else if (foundPgpMime && encryptedMime == null && mimeEncoding == "application/octet-stream")
						{
							// Should be first attachment *after* PGP_MIME version identification

							logger.Trace("MIME: Found octet-stream following pgp-encrypted.");
							encryptedMime = attachment;
						}
					}

					if (encryptedMime != null || sigMime != null)
					{
						logger.Trace("MIME: Calling HandlePgpMime");
						HandlePgpMime(mailItem, encryptedMime, sigMime, sigHash);

						if(encryptedMime != null)
							SetProperty(mailItem, "GnuPGSetting.Decrypted", true);
					}
				}

				if (match != null)
				{
					ribbon.VerifyButton.Enabled = (match.Value == _pgpSignedHeader);
					ribbon.DecryptButton.Enabled = (match.Value == _pgpEncryptedHeader);
				}

				if(ribbon.VerifyButton.Enabled || ribbon.DecryptButton.Enabled)
				{
					if (_settings.SaveDecrypted)
						mailItem.Save();
				}
			}

			ribbon.InvalidateButtons();
		}

		public static void SetProperty(Outlook.MailItem mailItem, string name, object value)
		{
			//var schema = "http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/" + name;
            var schema = "http://schemas.microsoft.com/mapi/string/{27EE45DA-1B2C-4E5B-B437-93E9820CC1FA}/" + name;
			
            mailItem.PropertyAccessor.SetProperty(schema, value);
		}

		public static object GetProperty(Outlook.MailItem mailItem, string name, object defaultReturn = null)
		{
			try
			{
				//var schema = "http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/" + name;
				var schema = "http://schemas.microsoft.com/mapi/string/{27EE45DA-1B2C-4E5B-B437-93E9820CC1FA}/" + name;

				return mailItem.PropertyAccessor.GetProperty(schema);
			}
			catch(Exception)
			{
				return defaultReturn;
			}
		}

		/// <summary>
		/// Add "- " prefix as needed
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		string PgpClearDashEscapeAndQuoteEncode(string msg)
		{
			var writer = new StringWriter();
			using (var reader = new StringReader(msg))
			{
				while(true)
				{
					var line = EncodeQuotedPrintable(reader.ReadLine());
					if(line == null)
						break;

					if (line.Length > 0 && line[0] == '-')
						writer.Write("- ");

					writer.WriteLine(line);
				}
			}

			return writer.ToString();
		}

		Encoding GetEncodingFromMail(Outlook.MailItem mailItem)
		{
		    var contentType = ReadContentType(mailItem);

			var match = Regex.Match(contentType, "charset=\"([^\"]+)\"");
			if (!match.Success)
				return Encoding.UTF8;

			return Encoding.GetEncoding(match.Groups[1].Value);
		}

		public static string EncodeQuotedPrintable(string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			StringBuilder builder = new StringBuilder();

			char[] bytes = value.ToCharArray();
			foreach (char v in bytes)
			{
				// The following are not required to be encoded:
				// - Tab (ASCII 9)
				// - Space (ASCII 32)
				// - Characters 33 to 126, except for the equal sign (61).

				if (v == '\n' || v == '\r')
					builder.Append(v);

				else if ((v == 9) || ((v >= 32) && (v <= 60)) || ((v >= 62) && (v <= 126)))
					builder.Append(v);

				else
				{
					builder.Append('=');
					builder.Append(((int)v).ToString("X2"));
				}
			}

			char lastChar = builder[builder.Length - 1];
			if (char.IsWhiteSpace(lastChar))
			{
				builder.Remove(builder.Length - 1, 1);
				builder.Append('=');
				builder.Append(((int)lastChar).ToString("X2"));
			}

			return builder.ToString();
		}

		string AddMessageToHtmlBody(string htmlBody, string msg)
		{
			var htmlBodyLower = htmlBody.ToLower();
			var bodyStartIndex = htmlBodyLower.IndexOf("<body");
			var bodyEndIndex = htmlBodyLower.IndexOf(">", bodyStartIndex);

			// Inject decrypt messaage
			var sb = new StringBuilder(htmlBody);
			sb.Insert(bodyEndIndex + 1, "<p>" + msg + "</p>");

			return sb.ToString();
		}

		void HandlePgpMime(Outlook.MailItem mailItem, Microsoft.Office.Interop.Outlook.Attachment encryptedMime,
			Microsoft.Office.Interop.Outlook.Attachment sigMime, string sigHash = "sha1")
		{
			logger.Trace("> HandlePgpMime");
			CryptoContext Context = null;

			byte[] cyphertext = null;
			byte[] clearbytes = null;
			string cleartext = mailItem.Body;

			// 1. Decrypt attachement

			if (encryptedMime != null)
			{
				logger.Trace("Decrypting cypher text.");

				var tempfile = Path.GetTempFileName();
				encryptedMime.SaveAsFile(tempfile);
				cyphertext = File.ReadAllBytes(tempfile);
				File.Delete(tempfile);

				clearbytes = DecryptAndVerify(mailItem.To, cyphertext, out Context);
				if (clearbytes == null)
					return;

				cleartext = this._encoding.GetString(clearbytes);
			}

			// 2. Verify signature

			if (sigMime != null)
			{
				Context = new CryptoContext(PasswordCallback, _settings.Cipher, _settings.Digest);
				var Crypto = new PgpCrypto(Context);
				Outlook.OlBodyFormat mailType = mailItem.BodyFormat;

				try
				{
					logger.Trace("Verify detached signature");

					var tempfile = Path.GetTempFileName();
					sigMime.SaveAsFile(tempfile);
					var detachedsig = File.ReadAllText(tempfile);
					File.Delete(tempfile);

					// Build up a clearsignature format for validation
					// the rules for are the same with the addition of two heaer fields.
					// Ultimately we need to get these fields out of email itself.

					// NOTE: encoding could be uppercase or lowercase. Try both.
					//       this is definetly hacky :/

					var encoding = GetEncodingFromMail(mailItem);

					var clearsigUpper = new StringBuilder();

					clearsigUpper.Append(string.Format("-----BEGIN PGP SIGNED MESSAGE-----\r\nHash: {0}\r\nCharset: {1}\r\n\r\n", sigHash, encoding.BodyName.ToUpper()));
					clearsigUpper.Append("Content-Type: text/plain; charset=");
					clearsigUpper.Append(encoding.BodyName.ToUpper());
					clearsigUpper.Append("\r\nContent-Transfer-Encoding: quoted-printable\r\n\r\n");

					clearsigUpper.Append(PgpClearDashEscapeAndQuoteEncode(
						encoding.GetString(
						(byte[])mailItem.PropertyAccessor.GetProperty(
							"http://schemas.microsoft.com/mapi/string/{4E3A7680-B77A-11D0-9DA5-00C04FD65685}/Internet Charset Body/0x00000102"))));

					clearsigUpper.Append("\r\n");
					clearsigUpper.Append(detachedsig);

					var clearsigLower = new StringBuilder(clearsigUpper.Length);

					clearsigLower.Append(string.Format("-----BEGIN PGP SIGNED MESSAGE-----\r\nHash: {0}\r\nCharset: {1}\r\n\r\n", sigHash, encoding.BodyName.ToUpper()));
					clearsigLower.Append("Content-Type: text/plain; charset=");
					clearsigLower.Append(encoding.BodyName.ToLower());
					clearsigLower.Append("\r\nContent-Transfer-Encoding: quoted-printable\r\n\r\n");

					clearsigLower.Append(PgpClearDashEscapeAndQuoteEncode(
						encoding.GetString(
						(byte[])mailItem.PropertyAccessor.GetProperty(
							"http://schemas.microsoft.com/mapi/string/{4E3A7680-B77A-11D0-9DA5-00C04FD65685}/Internet Charset Body/0x00000102"))));

					clearsigLower.Append("\r\n");
					clearsigLower.Append(detachedsig);

					logger.Trace(clearsigUpper.ToString());

					if (Crypto.VerifyClear(_encoding.GetBytes(clearsigUpper.ToString())) || Crypto.VerifyClear(_encoding.GetBytes(clearsigLower.ToString())))
					{
						Context = Crypto.Context;

						var message = "** " + string.Format(Localized.MsgValidSig,
							Context.SignedByUserId, Context.SignedByKeyId) + "\n\n";

						if (mailType == Outlook.OlBodyFormat.olFormatPlain)
							mailItem.Body = message + mailItem.Body;
						else
							mailItem.HTMLBody = AddMessageToHtmlBody(mailItem.HTMLBody, message);
					}
					else
					{
						Context = Crypto.Context;

						var message = "** " + string.Format(Localized.MsgInvalidSig,
							Context.SignedByUserId, Context.SignedByKeyId) + "\n\n";

						if (mailType == Outlook.OlBodyFormat.olFormatPlain)
							mailItem.Body = message + mailItem.Body;
						else
							mailItem.HTMLBody = AddMessageToHtmlBody(mailItem.HTMLBody, message);
					}
				}
				catch (PublicKeyNotFoundException ex)
				{
					logger.Debug(ex.ToString());

					Context = Crypto.Context;

					var message = "** " + Localized.MsgSigMissingPubKey + "\n\n";

					if (mailType == Outlook.OlBodyFormat.olFormatPlain)
						mailItem.Body = message + mailItem.Body;
					else
						mailItem.HTMLBody = AddMessageToHtmlBody(mailItem.HTMLBody, message);
				}
				catch (Exception ex)
				{
					logger.Debug(ex.ToString());

					WriteErrorData("VerifyEmail", ex);
					MessageBox.Show(
						ex.Message,
						"Outlook Privacy Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}

				return;

			}

			if (Context == null)
				return;

			// Extract files from MIME data

			MimeMessage msg = null;
			TextPart textPart = null;
			MimeEntity htmlPart = null;
			bool isHtml = false;

			using(var sin = new MemoryStream(clearbytes))
			{
				var parser = new MimeParser(sin);
				msg = parser.ParseMessage();
				var iter = new MimeIterator(msg);

				while(iter.MoveNext())
				{
					var part = iter.Current as TextPart;
					if (part == null)
						continue;

					if (part.IsAttachment)
						continue;

					// message could include both text and html
					// if we find html use that over text.
					if (part.IsHtml)
					{
						htmlPart = part;
						isHtml = true;
					}
					else
					{
						textPart = part;
					}
				}
			}

			var DecryptAndVerifyHeaderMessage = "** ";

			if (Context.IsEncrypted)
				DecryptAndVerifyHeaderMessage += Localized.MsgDecrypt + " ";

			if (Context.FailedIntegrityCheck)
				DecryptAndVerifyHeaderMessage += Localized.MsgFailedIntegrityCheck + " ";

			if (Context.IsSigned && Context.SignatureValidated)
			{
				DecryptAndVerifyHeaderMessage += string.Format(Localized.MsgValidSig,
					Context.SignedByUserId, Context.SignedByKeyId);
			}
			else if (Context.IsSigned)
			{
				DecryptAndVerifyHeaderMessage +=  string.Format(Localized.MsgInvalidSig,
					Context.SignedByUserId, Context.SignedByKeyId);
			}
			else
				DecryptAndVerifyHeaderMessage += Localized.MsgUnsigned;

			DecryptAndVerifyHeaderMessage += "\n\n";

			if(isHtml)
			{
				var htmlBody = msg.HtmlBody;
				var related = msg.Body as MultipartRelated;
				var doc = new HtmlAgilityPack.HtmlDocument();
				var savedImages = new List<MimePart>();

				doc.LoadHtml(htmlBody);

				// Find any embedded images
				foreach (var img in doc.DocumentNode.SelectNodes("//img[@src]"))
				{
					var src = img.Attributes["src"];
					Uri uri;

					if (src == null || src.Value == null)
						continue;

					// parse the <img src=...> attribute value into a Uri
					if (Uri.IsWellFormedUriString(src.Value, UriKind.Absolute))
						uri = new Uri(src.Value, UriKind.Absolute);
					else
						uri = new Uri(src.Value, UriKind.Relative);

					// locate the index of the attachment within the multipart/related (if it exists)
					string imageCid = src.Value.Substring(4);

					var iter = new MimeIterator(msg);
					MimePart attachment = null;
					while (iter.MoveNext())
					{
						if (iter.Current.ContentId == imageCid)
						{
							attachment = iter.Current as MimePart;
							break;
						}
					}

					if (attachment == null)
						continue;

					string fileName;

					// save the attachment (if we haven't already saved it)
					if (!savedImages.Contains(attachment))
					{
						fileName = attachment.FileName;

						if (string.IsNullOrEmpty(fileName))
							fileName = Guid.NewGuid().ToString();

						using (var stream = File.Create(fileName))
							attachment.ContentObject.DecodeTo(stream);

						try
						{
							var att = mailItem.Attachments.Add(fileName, Outlook.OlAttachmentType.olEmbeddeditem, null, "");
							att.PropertyAccessor.SetProperty("http://schemas.microsoft.com/mapi/proptag/0x3712001E", imageCid);
							savedImages.Add(attachment);
						}
						finally
						{
							// try not to leak temp files :)
							File.Delete(fileName);
						}
					}
				}

				mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
				mailItem.HTMLBody = AddMessageToHtmlBody(htmlBody, DecryptAndVerifyHeaderMessage);
			}
			else
			{
				// NOTE: For some reason we cannot change the BodyFormat once it's set.
				//       So if we are set to HTML we need to wrap the plain text so it's
				//       displayed okay. Also of course prevent XSS.

				if (mailItem.BodyFormat == Outlook.OlBodyFormat.olFormatPlain)
				{
					mailItem.Body = DecryptAndVerifyHeaderMessage + msg.TextBody;
				}
				else
				{
					var sb = new StringBuilder(msg.TextBody.Length + 100);
					sb.Append("<html><body><pre>");
					sb.Append(System.Net.WebUtility.HtmlEncode(DecryptAndVerifyHeaderMessage));
					sb.Append(System.Net.WebUtility.HtmlEncode(msg.TextBody));
					sb.Append("</pre></body></html>");

					mailItem.HTMLBody = sb.ToString();
				}
			}

			// NOTE: Removing existing attachments is perminant, even if the message
			//       is not saved.

			foreach (var mimeAttachment in msg.Attachments)
			{
				var fileName = mimeAttachment.FileName;
				var tempFile = Path.Combine(Path.GetTempPath(), fileName);

				using (FileStream fout = File.OpenWrite(tempFile))
				{
					mimeAttachment.ContentObject.DecodeTo(fout);
				}

				mailItem.Attachments.Add(tempFile, Outlook.OlAttachmentType.olByValue, 1, fileName);

				File.Delete(tempFile);
			}
		}

		/// <summary>
		/// WrapperEvent fired when a mailItem is closed.
		/// </summary>
		/// <param name="mailItem">the mailItem to close</param>
		/// <param name="Cancel">False when the event occurs. If the event procedure sets this argument to True,
		/// the open operation is not completed and the inspector is not displayed.</param>
		void mailItem_Close(Outlook.MailItem mailItem, ref bool Cancel)
		{
			try
			{
				if (mailItem == null)
					return;

				// New mail (Compose)
				if (mailItem.Sent == false)
				{
					bool toSave = false;
					var signProperpty = GetProperty(mailItem, "GnuPGSetting.Sign", false);
					if (signProperpty == null || (bool)signProperpty != ribbon.SignButton.Checked)
					{
						toSave = true;
					}

					var encryptProperpty = GetProperty(mailItem, "GnuPGSetting.Decrypted", false);
					if (encryptProperpty == null || (bool)encryptProperpty != ribbon.EncryptButton.Checked)
					{
						toSave = true;
					}

                    SetProperty(mailItem, "GnuPGSetting.Sign", false);
                    SetProperty(mailItem, "GnuPGSetting.Encrypt", false);

					if (toSave == true)
					{
#if DISABLED
		BoolEventArgs ev = e as BoolEventArgs;
		DialogResult res = MessageBox.Show("Do you want to save changes?",
										   "OutlookGnuPG",
										   MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
		if (res == DialogResult.Yes)
		{
		  // Must call mailItem.Write event handler (mailItem_Save) explicitely as it is not always called
		  // from the mailItem.Save() method. Mail is effectly saved only if a property changed.
		  mailItem_Save(sender, EventArgs.Empty);
		  mailItem.Save();
		}
		if (res == DialogResult.Cancel)
		{
		  ev.Value = true;
		}
#else
						// Invalidate the mailItem to force Outlook to ask to save the mailItem, hence calling
						// the mailItem_Save() handler to record the buttons state.
						// Note: the reason (button state property change) to save the mailItem is not necessairy obvious
						// to the user, certainly if nothing has been updated/changed by the user. If specific notification
						// is required see DISABLED code above. Beware, it might open 2 dialog boxes: the add-in custom and
						// the regular Outlook save confirmation.
						mailItem.Subject = mailItem.Subject;
					}
#endif
				}
				else
				{
                    SetProperty(mailItem, "GnuPGSetting.Sign", false);
                    SetProperty(mailItem, "GnuPGSetting.Encrypt", false);

                    var signProperty = GetProperty(mailItem, "GnuPGSetting.Decrypted");
                    SetProperty(mailItem, "GnuPGSetting.Decrypted", false);

                    if (signProperty != null && (bool)signProperty)
					{
                        // NOTE: Cannot call mailItem.Close from Close event handler
                        //       instead we will start a timer and call it after we
                        //       return. There is a small race condition, but 250
                        //       milliseconds should be enough even on slow machines.

                        var timer = new System.Windows.Forms.Timer { Interval = 250 };
                        timer.Tick += new EventHandler((o, e) =>
						{
                            timer.Stop();
                            ((Outlook._MailItem)mailItem).Close(
								_settings.SaveDecrypted ? Microsoft.Office.Interop.Outlook.OlInspectorClose.olSave :
                                Microsoft.Office.Interop.Outlook.OlInspectorClose.olDiscard);
                        });

					    timer.Start();

						Cancel = true;
					}
				}
			}
			catch
			{
				// Ignore random COM errors
			}
		}

		/// <summary>
		/// WrapperEvent fired when a mailItem is saved.
		/// This handler is designed to record the compose button state (Sign/Encrypt)
		/// associated to this mailItem.
		/// </summary>
		/// <param name="mailItem">the mailItem to save</param>
		/// <param name="Cancel">False when the event occurs. If the event procedure sets this argument to True,
		/// the open operation is not completed and the inspector is not displayed.</param>
		void mailItem_Save(Outlook.MailItem mailItem, ref bool Cancel)
		{
			if (mailItem == null)
				return;

			// New mail (Compose)
			if (mailItem.Sent == false)
			{
				// Record compose button states.
				SetProperty(mailItem, "GnuPGSetting.Sign", ribbon.SignButton.Checked);
				SetProperty(mailItem, "GnuPGSetting.Encrypt", ribbon.EncryptButton.Checked);
			}
			else
			{
                SetProperty(mailItem, "GnuPGSetting.Sign", false);
                SetProperty(mailItem, "GnuPGSetting.Encrypt", false);
			}
		}
		#endregion

		#region CommandBar Logic
		private void AddGnuPGCommandBar(Outlook.Explorer activeExplorer)
		{
			if (_gpgCommandBar != null)
				return;
			try
			{
				_gpgCommandBar = new GnuPGCommandBar(activeExplorer);
				_gpgCommandBar.Remove();
				_gpgCommandBar.Add();
				_gpgCommandBar.GetButton("Verify").Click += VerifyButton_Click;
				_gpgCommandBar.GetButton("Decrypt").Click += DecryptButton_Click;
				_gpgCommandBar.GetButton("Settings").Click += SettingsButton_Click;
				_gpgCommandBar.GetButton("About").Click += AboutButton_Click;
				_gpgCommandBar.RestorePosition(_settings);
			}
			catch (Exception ex)
			{
				WriteErrorData("AddGnuPGCommandBar", ex);
				MessageBox.Show(ex.Message);
			}
		}

		private void VerifyButton_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
		{
			// Get the selected item in Outlook and determine its type.
			Outlook.Selection outlookSelection = Application.ActiveExplorer().Selection;
			if (outlookSelection.Count <= 0)
				return;

			object selectedItem = outlookSelection[1];
			Outlook.MailItem mailItem = selectedItem as Outlook.MailItem;

			if (mailItem == null)
			{
				MessageBox.Show(
					"OutlookGnuPG can only verify mails.",
					"Invalid Item Type",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				return;
			}

			VerifyEmail(mailItem);
		}

		private void DecryptButton_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
		{
			// Get the selected item in Outlook and determine its type.
			Outlook.Selection outlookSelection = Application.ActiveExplorer().Selection;
			if (outlookSelection.Count <= 0)
				return;

			object selectedItem = outlookSelection[1];
			Outlook.MailItem mailItem = selectedItem as Outlook.MailItem;

			if (mailItem == null)
			{
				MessageBox.Show(
					"OutlookGnuPG can only decrypt mails.",
					"Invalid Item Type",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				return;
			}

			// Force open of mailItem with auto decrypt.
			_autoDecrypt = true;
			mailItem.Display(true);
			//      DecryptEmail(mailItem);
		}

		private void AboutButton_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
		{
			Globals.OutlookPrivacyPlugin.About();
		}

		private void SettingsButton_Click(Office.CommandBarButton Ctrl, ref bool CancelDefault)
		{
			Globals.OutlookPrivacyPlugin.Settings();
		}
		#endregion

		public string GetSMTPAddress(Outlook.MailItem mailItem)
		{
			if (mailItem.SendUsingAccount != null &&
				!string.IsNullOrWhiteSpace(mailItem.SendUsingAccount.SmtpAddress))
				return mailItem.SendUsingAccount.SmtpAddress;

			if (!string.IsNullOrWhiteSpace(mailItem.SenderEmailAddress) &&
				!mailItem.SenderEmailType.ToUpper().Equals("EX"))
				return mailItem.SenderEmailAddress;

			// This can be x509 for exchange accounts
			if (mailItem.SendUsingAccount != null &&
				mailItem.SendUsingAccount.AccountType != 0 && /* Verify not exchange account */
				mailItem.SendUsingAccount.CurrentUser != null &&
				mailItem.SendUsingAccount.CurrentUser.Address != null)
				return mailItem.SendUsingAccount.CurrentUser.Address;

			Microsoft.Office.Interop.Outlook.Recipient recip;
			Outlook.ExchangeUser exUser;
			Microsoft.Office.Interop.Outlook.Application oOutlook =
				   new Microsoft.Office.Interop.Outlook.Application();
			Outlook.NameSpace oNS = oOutlook.GetNamespace("MAPI");

			if (mailItem.SenderEmailType.ToUpper().Equals("EX"))
			{
				recip = oNS.CreateRecipient(mailItem.SenderName);
				exUser = recip.AddressEntry.GetExchangeUser();
				return exUser.PrimarySmtpAddress;
			}

			throw new Exception(Localized.ErrorUnableToDetermineSenderAddress);
		}

		#region Send Logic
		private void Application_ItemSend(object Item, ref bool Cancel)
		{
			var mailItem = Item as Outlook.MailItem;
			if (mailItem == null)
				return;

			GnuPGRibbon currentRibbon = ribbon;
			if (currentRibbon == null)
				return;

			string mail = mailItem.Body;
			Outlook.OlBodyFormat mailType = mailItem.BodyFormat;
			bool needToEncrypt = currentRibbon.EncryptButton.Checked;
			bool needToSign = currentRibbon.SignButton.Checked;

			// Early out when we don't need to sign/encrypt
			if (!needToEncrypt && !needToSign)
				return;

			// DEFAULT TO CANCEL
			Cancel = true;

			try
			{
				if (mailType != Outlook.OlBodyFormat.olFormatPlain)
				{
					MessageBox.Show(
						Localized.ErrorInvalidFormat,
						"Invalid Mail Format",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);

					Cancel = true; // Prevent sending the mail
					return;
				}

				IList<string> recipients = new List<string>();

				if (needToEncrypt)
				{
					// Popup UI to select the encryption targets 
					List<string> mailRecipients = new List<string>();
					foreach (Outlook.Recipient mailRecipient in mailItem.Recipients)
						mailRecipients.Add(GetSmtpAddressForRecipient((Outlook.Recipient)mailRecipient));

					var recipientDialog = new FormKeySelection(
						mailRecipients,
						GetKeysForEncryption,
						needToEncrypt,
						needToSign);

					recipientDialog.TopMost = true;

					DialogResult recipientResult = recipientDialog.DialogResult;

					if(recipientDialog.DialogResult == DialogResult.Ignore)
						recipientResult = recipientDialog.ShowDialog();
					
					if (recipientResult != DialogResult.OK)
					{
						// The user closed the recipient dialog, prevent sending the mail
						Cancel = true;
						return;
					}

					needToEncrypt = recipientDialog.Encrypt;
					needToSign = recipientDialog.Sign;

					foreach (var r in recipientDialog.SelectedKeys)
						recipients.Add(r.Key);

					if (needToEncrypt && recipients.Count == 0)
					{
						MessageBox.Show(
							Localized.ErrorInvalidRecipientKey,
							"Invalid Recipient Key",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);

						Cancel = true; // Prevent sending the mail
						return;
					}

					recipients.Add(GetSMTPAddress(mailItem));
				}

				var attachments = new List<Attachment>();

				// Sign and encrypt the plaintext mail
				if ((needToSign) && (needToEncrypt))
				{
					mail = SignAndEncryptEmail(mail, GetSMTPAddress(mailItem), recipients);
					if (mail == null)
						return;

					var mailAttachments = new List<Outlook.Attachment>();
					foreach (Outlook.Attachment attachment in mailItem.Attachments)
						mailAttachments.Add(attachment);

					foreach (var attachment in mailAttachments)
					{
						var a = new Attachment();

						a.TempFile = Path.GetTempPath();
						a.FileName = attachment.FileName;
						a.DisplayName = attachment.DisplayName;
						a.AttachmentType = attachment.Type;

						a.TempFile = Path.Combine(a.TempFile, a.FileName);
						a.TempFile = a.TempFile + ".pgp";
						attachment.SaveAsFile(a.TempFile);
						attachment.Delete();

						// Encrypt file
						byte[] cleartext = File.ReadAllBytes(a.TempFile);
						string cyphertext = SignAndEncryptAttachment(cleartext, GetSMTPAddress(mailItem), recipients);
						File.WriteAllText(a.TempFile, cyphertext);

						a.Encrypted = true;
						attachments.Add(a);
					}
				}
				else if (needToSign)
				{
					// Sign the plaintext mail if needed

					// 1. Verify text lines are not too long
					bool longLines = false;
					var mailLines = mail.Split('\n');
					foreach (var line in mailLines)
					{
						if (line.Length > 70)
						{
							longLines = true;
							break;
						}
					}

					if(longLines)
					{
						var dialog = new FormSelectLineWrap();
						var result = dialog.ShowDialog();
						if (result == DialogResult.Cancel)
							return;

						if (dialog.radioButtonWrap.Checked)
						{
							var lines = new List<string>(mailLines);
							for(int i = 0; i<lines.Count; i++)
							{
								var line = lines[i];
								if(line.Length>70)
								{
									var newLine = line.Substring(70);
									line = line.Substring(0, 70);

									lines[i] = line;
									lines.Insert(i + 1, newLine);
								}
							}

							var sb = new StringBuilder(mail.Length+20);
							foreach (var line in lines)
								sb.AppendLine(line);

							mail = sb.ToString();
						}
						if (dialog.radioButtonMime.Checked)
						{
							// todo
						}
						if (dialog.radioButtonEdit.Checked)
							return;
					}

					mail = SignEmail(mail, GetSMTPAddress(mailItem));
					if (mail == null)
						return;
				}
				else if (needToEncrypt)
				{
					// Encrypt the plaintext mail if needed
					mail = EncryptEmail(mail, recipients);
					if (mail == null)
						return;

					var mailAttachments = new List<Outlook.Attachment>();
					foreach (Outlook.Attachment attachment in mailItem.Attachments)
						mailAttachments.Add(attachment);

					foreach (var attachment in mailAttachments)
					{
						var a = new Attachment();

						a.TempFile = Path.GetTempPath();
						a.FileName = attachment.FileName;
						a.DisplayName = attachment.DisplayName;
						a.AttachmentType = attachment.Type;

						a.TempFile = Path.Combine(a.TempFile, a.FileName);
						a.TempFile = a.TempFile + ".pgp";
						attachment.SaveAsFile(a.TempFile);
						attachment.Delete();

						// Encrypt file
						byte[] cleartext = File.ReadAllBytes(a.TempFile);
						string cyphertext = EncryptEmail(cleartext, recipients);
						File.WriteAllText(a.TempFile, cyphertext);

						a.Encrypted = true;
						attachments.Add(a);
					}
				}

				foreach (var a in attachments)
				{
					mailItem.Attachments.Add(a.TempFile, a.AttachmentType, 1, a.DisplayName);
				}
			}
			catch (Exception ex)
			{
				if (ex.Message.ToLower().StartsWith("checksum"))
				{
					MessageBox.Show(
						Localized.ErrorBadPassphrase,
						"Outlook Privacy Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return;
				}

				WriteErrorData("Application_ItemSend", ex);
				MessageBox.Show(
					ex.Message,
					"Outlook Privacy Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				// Cancel sending
				return;
			}

			if (mail == null)
				return;

			Cancel = false;
			mailItem.Body = mail;

			currentRibbon.EncryptButton.Checked = false;
			currentRibbon.SignButton.Checked = false;

			SetProperty(mailItem, "GnuPGSetting.Sign", false);
			SetProperty(mailItem, "GnuPGSetting.Encrypt", false);
		}

		private string SignEmail(string data, string key)
		{
			try
			{
				var context = new CryptoContext(PasswordCallback, _settings.Cipher, _settings.Digest);
				var crypto = new PgpCrypto(context);
				var headers = new Dictionary<string, string>();
				headers["Version"] = "Outlook Privacy Plugin";

				return crypto.SignClear(data, key, this._encoding, headers);
			}
			catch (CryptoException ex)
			{
				WriteErrorData("SignEmail", ex);
				MessageBox.Show(
					ex.Message,
					"Outlook Privacy Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				return null;
			}
		}

		private string EncryptEmail(string mail, IList<string> recipients)
		{
			return EncryptEmail(this._encoding.GetBytes(mail), recipients);
		}

		private string EncryptEmail(byte[] data, IList<string> recipients)
		{
			try
			{
				var context = new CryptoContext(PasswordCallback, _settings.Cipher, _settings.Digest);
				var crypto = new PgpCrypto(context);
				var headers = new Dictionary<string, string>();
				headers["Version"] = "Outlook Privacy Plugin";
				headers["Charset"] = _encoding.WebName;

				return crypto.Encrypt(data, recipients, headers);
			}
			catch (CryptoException ex)
			{
				WriteErrorData("EncryptEmail", ex);
				MessageBox.Show(
					ex.Message,
					"Outlook Privacy Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				return null;
			}
			catch (Exception e)
			{
				WriteErrorData("EncryptEmail", e);
				MessageBox.Show(
					e.Message,
					"Outlook Privacy Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				return null;
			}
		}

		private string SignAndEncryptAttachment(byte[] data, string key, IList<string> recipients)
		{
			try
			{
				var context = new CryptoContext(PasswordCallback, _settings.Cipher, _settings.Digest);
				var crypto = new PgpCrypto(context);
				var headers = new Dictionary<string, string>();
				headers["Version"] = "Outlook Privacy Plugin";
				headers["Charset"] = _encoding.WebName;

				return crypto.SignAndEncryptBinary(data, key, recipients, headers);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					ex.Message,
					"Outlook Privacy Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				throw;
			}
		}

		private string SignAndEncryptEmail(string data, string key, IList<string> recipients)
		{
			return SignAndEncryptEmail(this._encoding.GetBytes(data), key, recipients);
		}

		private string SignAndEncryptEmail(byte[] data, string key, IList<string> recipients)
		{
			try
			{
				var context = new CryptoContext(PasswordCallback, _settings.Cipher, _settings.Digest);
				var crypto = new PgpCrypto(context);
				var headers = new Dictionary<string, string>();
				headers["Version"] = "Outlook Privacy Plugin";
				headers["Charset"] = _encoding.WebName;

				return crypto.SignAndEncryptText(data, key, recipients, headers);
			}
			catch (Exception ex)
			{
				WriteErrorData("SignAndEncryptEmail", ex);
				MessageBox.Show(
					ex.Message,
					"Outlook Privacy Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				throw;
			}
		}
		#endregion

		#region Receive Logic
		internal void VerifyEmail(Outlook.MailItem mailItem)
		{
			string mail = mailItem.Body;
			Outlook.OlBodyFormat mailType = mailItem.BodyFormat;

			if (Regex.IsMatch(mailItem.Body, _pgpSignedHeader) == false)
			{
				MessageBox.Show(
					Localized.ErrorMsgNotSigned,
					"Mail is not signed",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);

				return;
			}

			var Context = new CryptoContext(PasswordCallback, _settings.Cipher, _settings.Digest);
			var Crypto = new PgpCrypto(Context);

			try
			{
				if (Crypto.Verify(_encoding.GetBytes(mail)))
				{
					Context = Crypto.Context;

					var message = "** " + string.Format(Localized.MsgValidSig,
						Context.SignedByUserId, Context.SignedByKeyId) + "\n\n";

					if (mailType == Outlook.OlBodyFormat.olFormatPlain)
					{
						mailItem.Body = message + mailItem.Body;
					}
				}
				else
				{
					Context = Crypto.Context;

					var message = "** " + string.Format(Localized.MsgInvalidSig,
						Context.SignedByUserId, Context.SignedByKeyId) + "\n\n";

					if (mailType == Outlook.OlBodyFormat.olFormatPlain)
					{
						mailItem.Body = message + mailItem.Body;
					}
				}
			}
			catch (PublicKeyNotFoundException)
			{
				Context = Crypto.Context;

				string message = "** "+ Localized.MsgSigMissingPubKey+"\n\n";

				if (mailType == Outlook.OlBodyFormat.olFormatPlain)
				{
					mailItem.Body = message + mailItem.Body;
				}
			}
			catch (Exception ex)
			{
				WriteErrorData("VerifyEmail", ex);
				MessageBox.Show(
					ex.Message,
					"Outlook Privacy Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		void WriteErrorData(string msg, Exception ex)
		{
			try
			{
				var logFile = System.Environment.GetEnvironmentVariable("APPDATA");
				logFile = Path.Combine(logFile, "OutlookPrivacyPlugin");

				if (!Directory.Exists(logFile))
					Directory.CreateDirectory(logFile);

				logFile = Path.Combine(logFile, "log.txt");

				File.AppendAllText(logFile, "\n-------- " +
					DateTime.Now.ToString() +
					" --------\n" +
					msg +
					"\n\n" +
					ex.ToString());
			}
			catch
			{
			}
		}

		internal void DecryptEmail(Outlook.MailItem mailItem)
		{
			if (Regex.IsMatch(mailItem.Body, _pgpEncryptedHeader) == false)
			{
				MessageBox.Show(
					Localized.ErrorMsgNotEncrypted,
					"Mail is not encrypted",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
				return;
			}

			// Sometimes messages could contain multiple message blocks.  In that case just use the 
			// very first one.

			string firstPgpBlock = mailItem.Body;
			int endMessagePosition = firstPgpBlock.IndexOf("-----END PGP MESSAGE-----") + "-----END PGP MESSAGE-----".Length;
			if (endMessagePosition != -1)
				firstPgpBlock = firstPgpBlock.Substring(0, endMessagePosition);

			string charset = null;
			try
			{
				charset = Regex.Match(firstPgpBlock, @"Charset:\s+([^\s\r\n]+)").Groups[1].Value;
			}
			catch
			{
			}

			// Set default encoding if charset was missing from 
			// message.
			if (string.IsNullOrWhiteSpace(charset))
				charset = "ISO-8859-1";

			var encoding = Encoding.GetEncoding(charset);

			CryptoContext Context;
			byte[] cleardata = DecryptAndVerify(mailItem.To, ASCIIEncoding.ASCII.GetBytes(firstPgpBlock), out Context);
			if (cleardata != null)
			{
				mailItem.Body = DecryptAndVerifyHeaderMessage + encoding.GetString(cleardata);

				if (mailItem.BodyFormat == Outlook.OlBodyFormat.olFormatHTML)
				{
					// Don't HMTL encode or we will encode emails already in HTML format.
					// Office has a safe html module they use to prevent security issues.
					// Not encoding here should be no worse then reading a standard HTML
					// email.
					var html = DecryptAndVerifyHeaderMessage.Replace("<", "&lt;").Replace(">", "&gt;") + encoding.GetString(cleardata);
					html = html.Replace("\n", "<br/>");
					mailItem.HTMLBody = "<html><body>" + html + "</body></html>";
				}

				// Decrypt all attachments
				List<Microsoft.Office.Interop.Outlook.Attachment> mailAttachments = new List<Outlook.Attachment>();
				foreach (Microsoft.Office.Interop.Outlook.Attachment attachment in mailItem.Attachments)
					mailAttachments.Add(attachment);

				List<Attachment> attachments = new List<Attachment>();

				foreach (var attachment in mailAttachments)
				{
					Attachment a = new Attachment();

					// content id

					if (attachment.FileName.StartsWith("Attachment") && attachment.FileName.EndsWith(".pgp"))
					{
						var property = attachment.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x3712001F");
						a.FileName = property.ToString();

						if (a.FileName.Contains('@'))
						{
							a.FileName = a.FileName.Substring(0, a.FileName.IndexOf('@'));
						}

						a.TempFile = Path.GetTempPath();
						a.AttachmentType = attachment.Type;

						a.TempFile = Path.Combine(a.TempFile, a.FileName);

						attachment.SaveAsFile(a.TempFile);

						// Decrypt file
						var cyphertext = File.ReadAllBytes(a.TempFile);
						File.Delete(a.TempFile);

						try
						{
							var plaintext = DecryptAndVerify(mailItem.To, cyphertext, out Context);

							File.WriteAllBytes(a.TempFile, plaintext);

							attachments.Add(a);
						}
						catch
						{
							// Assume attachment wasn't encrypted
						}
					}
					else
					{
						a.FileName = Regex.Replace(attachment.FileName, @"\.(pgp\.asc|gpg\.asc|pgp|gpg|asc)$", "");
						a.DisplayName = Regex.Replace(attachment.DisplayName, @"\.(pgp\.asc|gpg\.asc|pgp|gpg|asc)$", ""); ;
						a.TempFile = Path.GetTempPath();
						a.AttachmentType = attachment.Type;

						a.TempFile = Path.Combine(a.TempFile, a.FileName);

						attachment.SaveAsFile(a.TempFile);

						// Decrypt file
						var cyphertext = File.ReadAllBytes(a.TempFile);
						File.Delete(a.TempFile);

						try
						{
							var plaintext = DecryptAndVerify(mailItem.To, cyphertext, out Context);

							File.WriteAllBytes(a.TempFile, plaintext);

							attachments.Add(a);
						}
						catch
						{
							// Assume attachment wasn't encrypted
						}
					}

				}

				foreach (var attachment in attachments)
					mailItem.Attachments.Add(attachment.TempFile, attachment.AttachmentType, 1, attachment.FileName);
			}
		}

		#endregion

		char[] PasswordCallback(PgpSecretKey masterKey, PgpSecretKey key)
		{
			if (PassphraseCache.ContainsKey(key.PublicKey.KeyId))
				return PassphraseCache[key.KeyId];

			// Loop until correct password or user selects cancel
			do
			{
				var passphraseDialog = new FormPassphrase(masterKey, key);
				var result = passphraseDialog.ShowDialog();
				if (result == DialogResult.Cancel)
					return null;

				var pass = passphraseDialog.textBoxPassphrase.Text.ToCharArray();

				try
				{
					key.ExtractPrivateKey(pass);
					PassphraseCache[key.PublicKey.KeyId] = pass;
					return pass;
				}
				catch (Exception)
				{
				}
			}
			while (true);
		}

		string DecryptAndVerifyHeaderMessage = "";

		byte[] DecryptAndVerify(string to, byte[] data, out CryptoContext outContext)
		{
			DecryptAndVerifyHeaderMessage = "";
			outContext = null;

			var Context = new CryptoContext(PasswordCallback, _settings.Cipher, _settings.Digest);
			var Crypto = new PgpCrypto(Context);

			try
			{
				var cleartext = Crypto.DecryptAndVerify(data, _settings.IgnoreIntegrityCheck);
				Context = Crypto.Context;

				DecryptAndVerifyHeaderMessage = "** ";

				if (Context.IsEncrypted)
					DecryptAndVerifyHeaderMessage += Localized.MsgDecrypt + " ";

				if (Context.IsSigned && Context.SignatureValidated)
				{
					DecryptAndVerifyHeaderMessage += string.Format(Localized.MsgValidSig,
						Context.SignedByUserId, Context.SignedByKeyId);
				}
				else if (Context.IsSigned)
				{
					DecryptAndVerifyHeaderMessage += string.Format(Localized.MsgInvalidSig,
						Context.SignedByUserId, Context.SignedByKeyId);
				}
				else
					DecryptAndVerifyHeaderMessage += Localized.MsgUnsigned;

				DecryptAndVerifyHeaderMessage += "\n\n";

				outContext = Context;
				return cleartext;
			}
			catch (CryptoException ex)
			{
				WriteErrorData("DecryptAndVerify", ex);
				MessageBox.Show(
					ex.Message,
					"Outlook Privacy Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

			}
			catch (Exception e)
			{
				WriteErrorData("DecryptAndVerify", e);

				if (e.Message.ToLower().StartsWith("checksum"))
				{
					MessageBox.Show(
						Localized.ErrorBadPassphrase,
						"Outlook Privacy Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
				else
				{
					MessageBox.Show(
						e.Message,
						"Outlook Privacy Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}

			return null;
		}

		#region General Logic
		internal void About()
		{
			FormAbout aboutBox = new FormAbout();
			aboutBox.TopMost = true;
			aboutBox.ShowDialog();
		}

		internal void Settings()
		{
			var settingsBox = new FormSettings(_settings);
			settingsBox.TopMost = true;

			if (settingsBox.ShowDialog() != DialogResult.OK)
				return;

			_settings.Encrypt2Self = settingsBox.Encrypt2Self;
			_settings.AutoDecrypt = settingsBox.AutoDecrypt;
			_settings.AutoVerify = settingsBox.AutoVerify;
			_settings.AutoEncrypt = settingsBox.AutoEncrypt;
			_settings.AutoSign = settingsBox.AutoSign;
			_settings.DefaultKey = settingsBox.DefaultKey;
			_settings.DefaultDomain = settingsBox.DefaultDomain;
			_settings.Default2PlainFormat = settingsBox.Default2PlainFormat;
			_settings.IgnoreIntegrityCheck = settingsBox.IgnoreIntegrityCheck;
			_settings.Cipher = settingsBox.Cipher;
			_settings.Digest = settingsBox.Digest;
			_settings.SaveDecrypted = settingsBox.SaveDecrypted;
			_settings.Save();
		}

		#endregion

		#region Key Management

		public IList<GnuKey> GetKeysForEncryption()
		{
			var crypto = new PgpCrypto(new CryptoContext());
			var keys = new List<GnuKey>();

			foreach (PgpPublicKey key in crypto.GetPublicKeyUserIdsForEncryption())
			{
				foreach (string user in key.GetUserIds())
				{
					var match = Regex.Match(user, @"<(.*)>");
					if (!match.Success)
						continue;

					var k = new GnuKey();
					k.Key = match.Groups[1].Value;
					k.KeyDisplay = user;

					var fingerprint = key.GetFingerprint();
					k.KeyId =
						fingerprint[fingerprint.Length - 4].ToString("X2") +
						fingerprint[fingerprint.Length - 3].ToString("X2") +
						fingerprint[fingerprint.Length - 2].ToString("X2") +
						fingerprint[fingerprint.Length - 1].ToString("X2");

					if (key.GetValidSeconds() != 0)
						k.Expiry = key.CreationTime.AddSeconds(key.GetValidSeconds()).ToShortDateString();

					keys.Add(k);
				}
			}

			return keys;
		}

		public IList<GnuKey> GetKeysForSigning()
		{
			var crypto = new PgpCrypto(new CryptoContext());
			var keys = new List<GnuKey>();

			foreach (var key in crypto.GetPublicKeyUserIdsForSign())
			{
				var match = Regex.Match(key, @"<(.*)>");
				if (!match.Success)
					continue;

				var k = new GnuKey();
				k.Key = match.Groups[1].Value;
				k.KeyDisplay = key;

				keys.Add(k);
			}

			return keys;
		}

		/// <summary>
		/// Get the SMTP address for a recipient.
		/// </summary>
		/// <remarks>
		/// Code from:
		/// https://msdn.microsoft.com/en-us/library/office/ff868695.aspx
		/// </remarks>
		/// <param name="recipient"></param>
		/// <returns></returns>
		string GetSmtpAddressForRecipient(Outlook.Recipient recipient)
		{
			const string PR_SMTP_ADDRESS = "http://schemas.microsoft.com/mapi/proptag/0x39FE001E";

			Outlook.PropertyAccessor pa = recipient.PropertyAccessor;
			return pa.GetProperty(PR_SMTP_ADDRESS).ToString();
		}

		#endregion

		#region Helper Logic

		private string RemoveInvalidAka(string msg)
		{
			char[] delimiters = { '\r', '\n' };
			string result = string.Empty;

			Regex r = new Regex("aka.*jpeg image of size");

			foreach (string s in msg.Split(delimiters))
			{
				if (string.IsNullOrEmpty(s) || r.IsMatch(s))
					continue;
				result += s + Environment.NewLine;
			}
			return result;
		}

		#endregion
	}
}
