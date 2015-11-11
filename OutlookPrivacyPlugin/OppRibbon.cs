
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using Outlook = Microsoft.Office.Interop.Outlook;

using Deja.Crypto.BcPgp;
using System.Text.RegularExpressions;

using NLog;

namespace OutlookPrivacyPlugin
{
	public class ButtonStateData
	{
		public bool SignButton = false;
		public bool EncryptButton = false;

		public ButtonStateData(bool sign, bool encrypt)
		{
			SignButton = sign;
			EncryptButton = encrypt;
		}
	}

	[ComVisible(true)]
	public class OppRibbon : Office.IRibbonExtensibility
	{
		static NLog.Logger logger = LogManager.GetCurrentClassLogger();

		private Office.IRibbonUI ribbon;

		public ToggleButton SignButton;
		public ToggleButton EncryptButton;
		public ToggleButton VerifyButton;
		public ToggleButton DecryptButton;
		public ToggleButton AttachPublicKeyButton;

		public Dictionary<string, ButtonStateData> ButtonState = new Dictionary<string, ButtonStateData>();

		private Dictionary<string, ToggleButton> Buttons = new Dictionary<string, ToggleButton>();

		public OppRibbon()
		{
			SignButton = new ToggleButton("signButton");
			EncryptButton = new ToggleButton("encryptButton");
			VerifyButton = new ToggleButton("verifyButton");
			DecryptButton = new ToggleButton("decryptButton");
			AttachPublicKeyButton = new ToggleButton("attachPublicKeyButton");

			Buttons.Add(SignButton.Id, SignButton);
			Buttons.Add(EncryptButton.Id, EncryptButton);
			Buttons.Add(VerifyButton.Id, VerifyButton);
			Buttons.Add(DecryptButton.Id, DecryptButton);
			Buttons.Add(AttachPublicKeyButton.Id, AttachPublicKeyButton);
		}

		#region IRibbonExtensibility Members

		public string GetCustomUI(string ribbonID)
		{
			String ui = null;
			if (ribbonID == "Microsoft.Outlook.Explorer")
			{
				ui = GetResourceText("OutlookPrivacyPlugin.RibbonMain.xml");
			}
			// Examine the ribbonID to see if the current item
			// is a Mail inspector.
			else if (ribbonID == "Microsoft.Outlook.Mail.Read")
			{
				// Retrieve the customized Ribbon XML.
				ui = GetResourceText("OutlookPrivacyPlugin.RibbonRead.xml");
			}
			else if (ribbonID == "Microsoft.Outlook.Mail.Compose")
			{
				// Retrieve the customized Ribbon XML.
				ui = GetResourceText("OutlookPrivacyPlugin.RibbonCompose.xml");
			}
			return ui;
		}

		#endregion

		internal void UpdateButtons(Properties.Settings settings)
		{
			// Compose Mail
			EncryptButton.Checked = settings.AutoEncrypt;
			SignButton.Checked = settings.AutoSign;
			AttachPublicKeyButton.Checked = false;

			// Read Mail
			DecryptButton.Checked = settings.AutoDecrypt;
			VerifyButton.Checked = settings.AutoVerify;
		}

		internal void InvalidateButtons()
		{
			if (ribbon == null)
				return;

			ribbon.InvalidateControl(SignButton.Id);
			ribbon.InvalidateControl(EncryptButton.Id);
			ribbon.InvalidateControl(VerifyButton.Id);
			ribbon.InvalidateControl(DecryptButton.Id);
			ribbon.InvalidateControl(AttachPublicKeyButton.Id);
		}

		#region Ribbon Callbacks

		public void OnLoad(Office.IRibbonUI ribbonUI)
		{
			this.ribbon = ribbonUI;
		}

		public void OnEncryptButton(Office.IRibbonControl control, bool isPressed)
		{
			logger.Trace("OnEncryptButton("+control.Id+", "+isPressed+")");

			var mailItem = ((Outlook.Inspector)control.Context).CurrentItem as Outlook.MailItem;
			if (mailItem == null)
				logger.Trace("OnEncryptButton: mailItem == null");

			if (isPressed == true)
			{
				if (mailItem != null)
				{
					var settings = new Properties.Settings();
					if (settings.Default2PlainFormat)
					{
						string body = mailItem.Body;
						mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatPlain;
						mailItem.Body = body;
					}
				}
			}

			OutlookPrivacyPlugin.SetProperty(mailItem, "GnuPGSetting.Encrypt", isPressed);
			EncryptButton.Checked = isPressed;
			ribbon.InvalidateControl(EncryptButton.Id);
		}

		public void OnDecryptButton(Office.IRibbonControl control)
		{
			var mailItem = ((Outlook.Inspector)control.Context).CurrentItem as Outlook.MailItem;
			if (mailItem != null)
				Globals.OutlookPrivacyPlugin.DecryptEmail(mailItem);
		}

		public void OnSignButton(Office.IRibbonControl control, bool isPressed)
		{
			var mailItem = ((Outlook.Inspector)control.Context).CurrentItem as Outlook.MailItem;

			if (isPressed == true)
			{
				if (mailItem != null)
				{
					var settings = new Properties.Settings();
					if (settings.Default2PlainFormat)
					{
						string body = mailItem.Body;
						mailItem.BodyFormat = Outlook.OlBodyFormat.olFormatPlain;
						mailItem.Body = body;
					}
				}
			}

			OutlookPrivacyPlugin.SetProperty(mailItem, "GnuPGSetting.Sign", isPressed);
			SignButton.Checked = isPressed;
			ribbon.InvalidateControl(SignButton.Id);
		}

		public void OnVerifyButton(Office.IRibbonControl control)
		{
			var mailItem = ((Outlook.Inspector)control.Context).CurrentItem as Outlook.MailItem;
			if (mailItem != null)
				Globals.OutlookPrivacyPlugin.VerifyEmail(mailItem);
		}

		public void OnAttachPublicKeyButton(Office.IRibbonControl control)
		{
			var mailItem = ((Outlook.Inspector)control.Context).CurrentItem as Outlook.MailItem;
			if (mailItem == null)
			{
				// TODO - Generate a log message
				MessageBox.Show("Error, mailItem was null.");
				return;
			}
			
			var smtp = Globals.OutlookPrivacyPlugin.GetSMTPAddress(mailItem);
			
			var crypto = new PgpCrypto(new CryptoContext());
			
			var headers = new Dictionary<string, string>();
			headers["Version"] = "Outlook Privacy Plugin";

			var publicKey = crypto.PublicKey(smtp, headers);

			if(publicKey == null)
			{
				// TODO - Generate log message
				MessageBox.Show("Error, unable to find public KeyItem to attach.");
				return;
			}

			var attachName = smtp.Replace("@", "_at_") + ".asc";
			attachName = Regex.Replace(attachName, @"[:\\/=+!@#$%^&*(){}[\]|<>,'"";~`]", "_");

			var tempFile = Path.Combine(Path.GetTempPath(), attachName);
			File.WriteAllText(tempFile, publicKey);

			try
			{
				mailItem.Attachments.Add(
					tempFile, 
					Outlook.OlAttachmentType.olByValue, 
					1,
					attachName);
				
				mailItem.Save();
			}
			finally
			{
				File.Delete(tempFile);
			}
		}

		public void OnSettingsButtonRead(Office.IRibbonControl control)
		{
			Globals.OutlookPrivacyPlugin.Settings();
		}

		public void OnSettingsButtonNew(Office.IRibbonControl control)
		{
			Globals.OutlookPrivacyPlugin.Settings();

			// Force an update of button state:
			ribbon.InvalidateControl(SignButton.Id);
			ribbon.InvalidateControl(EncryptButton.Id);
		}

		public void OnAboutButton(Office.IRibbonControl control)
		{
			Globals.OutlookPrivacyPlugin.About();
		}

		public stdole.IPictureDisp
		  GetCustomImage(Office.IRibbonControl control)
		{
			stdole.IPictureDisp pictureDisp = null;
			switch (control.Id)
			{
				case "settingsButtonNew":
				case "settingsButtonRead":
					pictureDisp = ImageConverter.Convert(Properties.Resources.database_gear);
					break;
				case "aboutButtonNew":
				case "aboutButtonRead":
					pictureDisp = ImageConverter.Convert(Properties.Resources.Logo);
					break;
				case "attachPublicKeyButton":
					pictureDisp = ImageConverter.Convert(Properties.Resources.attach);
					break;
				default:
					if ((control.Id == EncryptButton.Id) || (control.Id == DecryptButton.Id))
						pictureDisp = ImageConverter.Convert(Properties.Resources.lock_edit);
					if ((control.Id == SignButton.Id) || (control.Id == VerifyButton.Id))
						pictureDisp = ImageConverter.Convert(Properties.Resources.link_edit);
					break;
			}
			return pictureDisp;
		}

		public bool GetPressed(Office.IRibbonControl control)
		{
			logger.Trace("GetPressed("+control.Id+")");

			if (control.Id == SignButton.Id)
			{
				var mailItem = ((Outlook.Inspector)control.Context).CurrentItem as Outlook.MailItem;
				return (bool)OutlookPrivacyPlugin.GetProperty(mailItem, "GnuPGSetting.Sign", false);
			}
			if (control.Id == EncryptButton.Id)
			{
				var mailItem = ((Outlook.Inspector)control.Context).CurrentItem as Outlook.MailItem;
				return (bool)OutlookPrivacyPlugin.GetProperty(mailItem, "GnuPGSetting.Encrypt", false);
			}

			logger.Trace("GetPressed: Button did not match encrypt or sign!");

			if (Buttons.ContainsKey(control.Id))
				return Buttons[control.Id].Checked;

			return false;
		}

		public bool GetEnabled(Office.IRibbonControl control)
		{
			if (Buttons.ContainsKey(control.Id))
				return Buttons[control.Id].Enabled;
			return false;
		}

		#endregion

		#region Helpers

		private static string GetResourceText(string resourceName)
		{
			var asm = Assembly.GetExecutingAssembly();
			var resourceNames = asm.GetManifestResourceNames();
			foreach (var resource in resourceNames)
			{
				if (string.Compare(resourceName, resource, StringComparison.OrdinalIgnoreCase) != 0) continue;
				
				using (var resourceReader = new StreamReader(asm.GetManifestResourceStream(resource)))
				{
					return resourceReader.ReadToEnd();
				}
			}

			return null;
		}

		#endregion
	}

	internal class ImageConverter : AxHost
	{
		private ImageConverter()
			: base(null)
		{
		}
		public static stdole.IPictureDisp Convert(System.Drawing.Image image)
		{
			return (stdole.IPictureDisp)AxHost.GetIPictureDispFromPicture(image);
		}
	}

	public class ToggleButton
	{
		public ToggleButton(string controlId)
		{
			Checked = false;
			Id = controlId;
		}

		public bool Checked { get; set; }
		public bool Enabled { get; set; }
		public string Id { get; set; }	
	}
}
