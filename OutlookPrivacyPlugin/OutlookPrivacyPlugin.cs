using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Reflection;
using OutlookPrivacyPlugin.Properties;
using Exception = System.Exception;
using anmar.SharpMimeTools;
using System.Timers;

using Deja.Crypto.BcPgp;
using NLog;
using NLog.Config;
using NLog.Targets;

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

		char[] Passphrase { get; set; }

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
			var a = Assembly.GetExecutingAssembly();
			var s = a.GetManifestResourceStream(key);

			byte[] buff = new byte[s.Length];
			var cnt = s.Read(buff, 0, buff.Length);

			return UTF8Encoding.UTF8.GetString(buff, 0, cnt);
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
			Outlook.MailItem mailItem = inspector.CurrentItem as Outlook.MailItem;
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

			MailItemInspector wrappedMailItem = new MailItemInspector(inspector);
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
			MailItemInspector wrappedMailItem = o as MailItemInspector;
			wrappedMailItem.Dispose -= new OutlookWrapperDisposeDelegate(MailItemInspector_Dispose);
			wrappedMailItem.MyClose -= new MailItemInspectorCloseDelegate(mailItem_Close);
			wrappedMailItem.Open -= new MailItemInspectorOpenDelegate(mailItem_Open);
			wrappedMailItem.Save -= new MailItemInspectorSaveDelegate(mailItem_Save);
			_WrappedObjects.Remove(id);
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

			OutlookPrivacyPlugin.SetProperty(mailItem, "GnuPGSetting.Sign", false);
			OutlookPrivacyPlugin.SetProperty(mailItem, "GnuPGSetting.Encrypt", false);

			// New mail (Compose)
			if (mailItem.Sent == false)
			{
				ribbon.SignButton.Checked = _settings.AutoSign;
				ribbon.EncryptButton.Checked = _settings.AutoEncrypt;

				if (mailItem.Body.IndexOf("\n** Message decrypted. Valid signature") > -1)
				{
					ribbon.SignButton.Checked = true;
					ribbon.EncryptButton.Checked = true;
				}
				else if (mailItem.Body.IndexOf("\n** Message decrypted.") > -1)
				{
					ribbon.EncryptButton.Checked = true;
				}

				OutlookPrivacyPlugin.SetProperty(mailItem, "GnuPGSetting.Sign", ribbon.SignButton.Checked);
				OutlookPrivacyPlugin.SetProperty(mailItem, "GnuPGSetting.Encrypt", ribbon.EncryptButton.Checked);

				ribbon.InvalidateButtons();

				if (ribbon.EncryptButton.Checked || ribbon.SignButton.Checked)
					mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatPlain;
			}
			else
			// Read mail
			{
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
						//mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatPlain;

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

					var contentType = mailItem.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/content-type/0x0000001F");

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
			}

			ribbon.InvalidateButtons();
		}

		public static void SetProperty(Outlook.MailItem mailItem, string name, object value)
		{
//			var schema = "http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/" + name;
			var schema = "http://schemas.microsoft.com/mapi/string/{27EE45DA-1B2C-4E5B-B437-93E9820CC1FA}/" + name;
			
			mailItem.PropertyAccessor.SetProperty(schema, value);
		}

		public static object GetProperty(Outlook.MailItem mailItem, string name)
		{
			//var schema = "http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/" + name;
			var schema = "http://schemas.microsoft.com/mapi/string/{27EE45DA-1B2C-4E5B-B437-93E9820CC1FA}/" + name;

			return mailItem.PropertyAccessor.GetProperty(schema);
			//return null;
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
			var contentType = mailItem.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/string/{00020386-0000-0000-C000-000000000046}/content-type/0x0000001F");

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

		void HandlePgpMime(Outlook.MailItem mailItem, Microsoft.Office.Interop.Outlook.Attachment encryptedMime,
			Microsoft.Office.Interop.Outlook.Attachment sigMime, string sigHash = "sha1")
		{
			logger.Trace("> HandlePgpMime");
			CryptoContext Context = null;

			string sig = null;
			byte[] cyphertext = null;
			string cleartext = mailItem.Body;

			// 1. Decrypt attachement

			if (encryptedMime != null)
			{
				logger.Trace("Decrypting cypher text.");

			var tempfile = Path.GetTempFileName();
			encryptedMime.SaveAsFile(tempfile);
				cyphertext = File.ReadAllBytes(tempfile);
				File.Delete(tempfile);

				var clearbytes = DecryptAndVerify(mailItem.To, cyphertext, out Context);
				if (clearbytes == null)
					return;

				cleartext = this._encoding.GetString(clearbytes);
			}

			// 2. Verify signature

			if (sigMime != null)
			{
				Context = new CryptoContext(Passphrase);
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

					var encoding = GetEncodingFromMail(mailItem);

					var clearsig = string.Format("-----BEGIN PGP SIGNED MESSAGE-----\r\nHash: {0}\r\n\r\n", sigHash);
					//clearsig += "Content-Type: text/plain; charset=ISO-8859-1\r\nContent-Transfer-Encoding: quoted-printable\r\n\r\n";
					clearsig += "Content-Type: text/plain; charset=" + 
						encoding.BodyName.ToUpper()+
						"\r\nContent-Transfer-Encoding: quoted-printable\r\n\r\n";

					clearsig += PgpClearDashEscapeAndQuoteEncode(
						encoding.GetString(
						(byte[])mailItem.PropertyAccessor.GetProperty(
							"http://schemas.microsoft.com/mapi/string/{4E3A7680-B77A-11D0-9DA5-00C04FD65685}/Internet Charset Body/0x00000102")));

					clearsig += "\r\n"+detachedsig;

					logger.Trace(clearsig);


					if (Crypto.VerifyClear(_encoding.GetBytes(clearsig)))
					{
						Context = Crypto.Context;

						var message = "** Valid signature from \"" + Context.SignedByUserId +
							"\" with KeyId " + Context.SignedByKeyId + ".\n\n";

						if (mailType == Outlook.OlBodyFormat.olFormatPlain)
						{
							mailItem.Body = message + mailItem.Body;
						}
					}
					else
					{
						Context = Crypto.Context;

						var message = "** Invalid signature from \"" + Context.SignedByUserId +
							"\" with KeyId " + Context.SignedByKeyId + ".\n\n";

						if (mailType == Outlook.OlBodyFormat.olFormatPlain)
						{
							mailItem.Body = message + mailItem.Body;
						}
					}
				}
				catch (PublicKeyNotFoundException ex)
				{
					logger.Debug(ex.ToString());

					Context = Crypto.Context;

					var message = "** Unable to verify signature, missing public key.\n\n";

					if (mailType == Outlook.OlBodyFormat.olFormatPlain)
					{
						mailItem.Body = message + mailItem.Body;
					}
				}
				catch (Exception ex)
				{
					logger.Debug(ex.ToString());

					this.Passphrase = null;

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


			SharpMessage msg = new SharpMessage(cleartext);
			string body = mailItem.Body;

			var DecryptAndVerifyHeaderMessage = "** ";

			if (Context.IsEncrypted)
				DecryptAndVerifyHeaderMessage += "Message decrypted. ";

			if (Context.FailedIntegrityCheck)
				DecryptAndVerifyHeaderMessage += "Failed integrity check! ";

			if (Context.IsSigned && Context.SignatureValidated)
			{
				DecryptAndVerifyHeaderMessage += "Valid signature from \"" + Context.SignedByUserId +
					"\" with KeyId " + Context.SignedByKeyId;
			}
			else if (Context.IsSigned)
			{
				DecryptAndVerifyHeaderMessage += "Invalid signature from \"" + Context.SignedByUserId +
					"\" with KeyId " + Context.SignedByKeyId + ".";
			}
			else
				DecryptAndVerifyHeaderMessage += "Message was unsigned.";

			DecryptAndVerifyHeaderMessage += "\n\n";

			if (mailItem.BodyFormat == Outlook.OlBodyFormat.olFormatPlain)
			{
				mailItem.Body = DecryptAndVerifyHeaderMessage + msg.Body;
			}
			else if (mailItem.BodyFormat == Outlook.OlBodyFormat.olFormatHTML)
			{
				if (!msg.Body.TrimStart().ToLower().StartsWith("<html"))
				{
					body = DecryptAndVerifyHeaderMessage + msg.Body;
					body = System.Net.WebUtility.HtmlEncode(body);
					body = body.Replace("\n", "<br />");

					mailItem.HTMLBody = "<html><head></head><body>" + body + "</body></html>";
				}
				else
				{
					// Find <body> tag and insert our message.

					var matches = Regex.Match(msg.Body, @"(<body[^<]*>)", RegexOptions.IgnoreCase);
					if (matches.Success)
					{
						var bodyTag = matches.Groups[1].Value;

						// Insert decryption message.
						mailItem.HTMLBody = msg.Body.Replace(
							bodyTag,
							bodyTag + DecryptAndVerifyHeaderMessage.Replace("\n", "<br />"));
					}
					else
						mailItem.HTMLBody = msg.Body;
				}
			}
			else
			{
				// May cause mail item not to open correctly

				mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatPlain;
				mailItem.Body = msg.Body;
			}

			foreach (SharpAttachment mimeAttachment in msg.Attachments)
			{
				mimeAttachment.Stream.Position = 0;
				var fileName = mimeAttachment.Name;
				var tempFile = Path.Combine(Path.GetTempPath(), fileName);

				using (FileStream fout = File.OpenWrite(tempFile))
				{
					mimeAttachment.Stream.CopyTo(fout);
				}

				mailItem.Attachments.Add(tempFile, Outlook.OlAttachmentType.olByValue, 1, fileName);
			}

			//mailItem.Save();
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
					var SignProperpty = GetProperty(mailItem, "GnuPGSetting.Sign");
					if (SignProperpty == null || (bool)SignProperpty != ribbon.SignButton.Checked)
					{
						toSave = true;
					}

					var EncryptProperpty = GetProperty(mailItem, "GnuPGSetting.Decrypted");
					if (EncryptProperpty == null || (bool)EncryptProperpty != ribbon.EncryptButton.Checked)
					{
						toSave = true;
					}
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
					var SignProperty = GetProperty(mailItem, "GnuPGSetting.Decrypted");
					if (SignProperty != null && (bool)SignProperty)
					{
						mailItem.Close(Microsoft.Office.Interop.Outlook.OlInspectorClose.olDiscard);
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

			throw new Exception("Error, unable to determine senders address.");
		}

		#region Send Logic
		private void Application_ItemSend(object Item, ref bool Cancel)
		{
			Outlook.MailItem mailItem = Item as Outlook.MailItem;

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
						"OutlookGnuPG can only sign/encrypt plain text mails. Please change the format, or disable signing/encrypting for this mail.",
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
						mailRecipients.Add(GetAddressCN(((Outlook.Recipient)mailRecipient).Address));

					Recipient recipientDialog = new Recipient(mailRecipients); // Passing in the first addres, maybe it matches
					recipientDialog.TopMost = true;
					DialogResult recipientResult = recipientDialog.ShowDialog();

					if (recipientResult != DialogResult.OK)
					{
						// The user closed the recipient dialog, prevent sending the mail
						Cancel = true;
						return;
					}

					foreach (string r in recipientDialog.SelectedKeys)
						recipients.Add(r);

					recipientDialog.Close();

					if (recipients.Count == 0)
					{
						MessageBox.Show(
							"OutlookGnuPG needs a recipient when encrypting. No keys were detected/selected.",
							"Invalid Recipient Key",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);

						Cancel = true; // Prevent sending the mail
						return;
					}

					recipients.Add(GetSMTPAddress(mailItem));
				}

				List<Attachment> attachments = new List<Attachment>();

				// Sign and encrypt the plaintext mail
				if ((needToSign) && (needToEncrypt))
				{
					mail = SignAndEncryptEmail(mail, GetSMTPAddress(mailItem), recipients);
					if (mail == null)
						return;

					List<Microsoft.Office.Interop.Outlook.Attachment> mailAttachments = new List<Outlook.Attachment>();
					foreach (Microsoft.Office.Interop.Outlook.Attachment attachment in mailItem.Attachments)
						mailAttachments.Add(attachment);

					foreach (var attachment in mailAttachments)
					{
						Attachment a = new Attachment();

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

					List<Microsoft.Office.Interop.Outlook.Attachment> mailAttachments = new List<Outlook.Attachment>();
					foreach (Microsoft.Office.Interop.Outlook.Attachment attachment in mailItem.Attachments)
						mailAttachments.Add(attachment);

					foreach (var attachment in mailAttachments)
					{
						Attachment a = new Attachment();

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
				this.Passphrase = null;

				if (ex.Message.ToLower().StartsWith("checksum"))
				{
					MessageBox.Show(
						"Incorrect passphrase possibly entered.",
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
		}

		private string SignEmail(string data, string key)
		{
			try
			{
				if (!PromptForPasswordAndKey())
					return null;

				var context = new CryptoContext(Passphrase);
				var crypto = new PgpCrypto(context);
				var headers = new Dictionary<string, string>();
				headers["Version"] = "Outlook Privacy Plugin";

				return crypto.SignClear(data, key, this._encoding, headers);
			}
			catch (CryptoException ex)
			{
				this.Passphrase = null;

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
				var context = new CryptoContext();
				var crypto = new PgpCrypto(context);
				var headers = new Dictionary<string, string>();
				headers["Version"] = "Outlook Privacy Plugin";
				headers["Charset"] = _encoding.WebName;

				return crypto.Encrypt(data, recipients, headers);
			}
			catch (CryptoException ex)
			{
				this.Passphrase = null;

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
				this.Passphrase = null;

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
				if (!PromptForPasswordAndKey())
					return null;

				var context = new CryptoContext(this.Passphrase);
				var crypto = new PgpCrypto(context);
				var headers = new Dictionary<string, string>();
				headers["Version"] = "Outlook Privacy Plugin";
				headers["Charset"] = _encoding.WebName;

				return crypto.SignAndEncryptBinary(data, key, recipients, headers);
			}
			catch (Exception ex)
			{
				this.Passphrase = null;

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
				if (!PromptForPasswordAndKey())
					return null;

				var context = new CryptoContext(this.Passphrase);
				var crypto = new PgpCrypto(context);
				var headers = new Dictionary<string, string>();
				headers["Version"] = "Outlook Privacy Plugin";
				headers["Charset"] = _encoding.WebName;

				return crypto.SignAndEncryptText(data, key, recipients, headers);
			}
			catch (Exception ex)
			{
				this.Passphrase = null;

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
					"Outlook Privacy cannot help here.",
					"Mail is not signed",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);

				return;
			}

			var Context = new CryptoContext(Passphrase);
			var Crypto = new PgpCrypto(Context);

			try
			{
				if (Crypto.Verify(_encoding.GetBytes(mail)))
				{
					Context = Crypto.Context;

					var message = "** Valid signature from \"" + Context.SignedByUserId +
						"\" with KeyId " + Context.SignedByKeyId + ".\n\n";

					if (mailType == Outlook.OlBodyFormat.olFormatPlain)
					{
						mailItem.Body = message + mailItem.Body;
					}
				}
				else
				{
					Context = Crypto.Context;

					var message = "** Invalid signature from \"" + Context.SignedByUserId +
						"\" with KeyId " + Context.SignedByKeyId + ".\n\n";

					if (mailType == Outlook.OlBodyFormat.olFormatPlain)
					{
						mailItem.Body = message + mailItem.Body;
					}
				}
			}
			catch (PublicKeyNotFoundException ex)
			{
				Context = Crypto.Context;

				var message = "** Unable to verify signature, missing public key.\n\n";

				if (mailType == Outlook.OlBodyFormat.olFormatPlain)
				{
					mailItem.Body = message + mailItem.Body;
				}
			}
			catch (Exception ex)
			{
				this.Passphrase = null;

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
					"Outlook Privacy Plugin cannot help here.",
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
					//else if (attachment.FileName == "PGPexch.htm.pgp")
					//{
					//	// This is the HTML email message.

					//	var TempFile = Path.GetTempFileName();
					//	attachment.SaveAsFile(TempFile);

					//	// Decrypt file
					//	var cyphertext = File.ReadAllBytes(TempFile);
					//	File.Delete(TempFile);

					//	try
					//	{
					//		var plaintext = DecryptAndVerify(mailItem.To, cyphertext);

					//		mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatHTML;
					//		mailItem.HTMLBody = _encoding.GetString(plaintext);
					//	}
					//	catch
					//	{
					//		// Odd!
					//	}
					//}
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

				// Warning: Saving could save the message back to the server, not just locally
				//mailItem.Save();
			}
		}

		#endregion

		bool PromptForPasswordAndKey()
		{
			if (this.Passphrase != null)
				return true;

			// Popup UI to select the passphrase and private key.
			Passphrase passphraseDialog = new Passphrase(_settings.DefaultKey, "Key");
			passphraseDialog.TopMost = true;
			DialogResult passphraseResult = passphraseDialog.ShowDialog();
			if (passphraseResult != DialogResult.OK)
			{
				// The user closed the passphrase dialog, prevent sending the mail
				return false;
			}

			this.Passphrase = passphraseDialog.EnteredPassphrase.ToCharArray();
			passphraseDialog.Close();

			return true;
		}

		string DecryptAndVerifyHeaderMessage = "";

		byte[] DecryptAndVerify(string to, byte[] data, out CryptoContext outContext)
		{
			DecryptAndVerifyHeaderMessage = "";
			outContext = null;

			if (!PromptForPasswordAndKey())
				return null;

			var Context = new CryptoContext(Passphrase);
			var Crypto = new PgpCrypto(Context);

			try
			{
				var cleartext = Crypto.DecryptAndVerify(data, _settings.IgnoreIntegrityCheck);
				Context = Crypto.Context;

				// NOT USED YET.
				
				//DecryptAndVerifyHeaderMessage = "** ";

				//if (Context.IsEncrypted)
				//	DecryptAndVerifyHeaderMessage += "Message decrypted. ";

				//if (Context.IsSigned && Context.SignatureValidated)
				//{
				//	DecryptAndVerifyHeaderMessage += "Valid signature from \"" + Context.SignedByUserId +
				//		"\" with KeyId " + Context.SignedByKeyId;
				//}
				//else if (Context.IsSigned)
				//{
				//	DecryptAndVerifyHeaderMessage += "Invalid signature from \"" + Context.SignedByUserId +
				//		"\" with KeyId " + Context.SignedByKeyId + ".";
				//}
				//else
				//	DecryptAndVerifyHeaderMessage += "Message was unsigned.";

				//DecryptAndVerifyHeaderMessage += "\n\n";

				outContext = Context;
				return cleartext;
			}
			catch (CryptoException ex)
			{
				this.Passphrase = null;

				WriteErrorData("DecryptAndVerify", ex);
				MessageBox.Show(
					ex.Message,
					"Outlook Privacy Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

			}
			catch (Exception e)
			{
				this.Passphrase = null;
				WriteErrorData("DecryptAndVerify", e);

				if (e.Message.ToLower().StartsWith("checksum"))
				{
					MessageBox.Show(
						"Incorrect passphrase possibly entered.",
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
			About aboutBox = new About();
			aboutBox.TopMost = true;
			aboutBox.ShowDialog();
		}

		internal void Settings()
		{
			Settings settingsBox = new Settings(_settings);
			settingsBox.TopMost = true;
			DialogResult result = settingsBox.ShowDialog();

			if (result != DialogResult.OK)
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
			_settings.Save();
		}

		#endregion

		#region Key Management

		public IList<GnuKey> GetKeysForEncryption()
		{
			var crypto = new PgpCrypto(new CryptoContext());
			List<GnuKey> keys = new List<GnuKey>();

			foreach (string key in crypto.GetPublicKeyUserIdsForEncryption())
			{
				var match = Regex.Match(key, @"<(.*)>");
				if (!match.Success)
					continue;

				GnuKey k = new GnuKey();
				k.Key = match.Groups[1].Value;
				k.KeyDisplay = key;

				keys.Add(k);
			}

			return keys;
		}

		public IList<GnuKey> GetKeysForSigning()
		{
			var crypto = new PgpCrypto(new CryptoContext());
			List<GnuKey> keys = new List<GnuKey>();

			foreach (string key in crypto.GetPublicKeyUserIdsForSign())
			{
				var match = Regex.Match(key, @"<(.*)>");
				if (!match.Success)
					continue;

				GnuKey k = new GnuKey();
				k.Key = match.Groups[1].Value;
				k.KeyDisplay = key;

				keys.Add(k);
			}

			return keys;
		}

		string GetAddressCN(string AddressX400)
		{
			char[] delimiters = { '/' };
			string[] splitAddress = AddressX400.Split(delimiters);
			for (int k = 0; k < splitAddress.Length; k++)
			{
				if (splitAddress[k].StartsWith("cn=", true, null) && !Regex.IsMatch(splitAddress[k], "ecipient", RegexOptions.IgnoreCase))
				{
					string address = Regex.Replace(splitAddress[k], "cn=", string.Empty, RegexOptions.IgnoreCase);
					if (!string.IsNullOrEmpty(_settings.DefaultDomain))
					{
						address += "@" + _settings.DefaultDomain;
						address = address.Replace("@@", "@");
					}
					return address;
				}
			}
			return AddressX400;
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
