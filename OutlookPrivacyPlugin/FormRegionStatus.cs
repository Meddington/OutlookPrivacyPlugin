using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Office = Microsoft.Office.Core;
using Outlook = Microsoft.Office.Interop.Outlook;

using Deja.Crypto.BcPgp;
using System.Text.RegularExpressions;
using OutlookPrivacyPlugin.Language;

namespace OutlookPrivacyPlugin
{
	partial class FormRegionStatus
	{
		#region Form Region Factory

		[Microsoft.Office.Tools.Outlook.FormRegionMessageClass(Microsoft.Office.Tools.Outlook.FormRegionMessageClassAttribute.Note)]
		[Microsoft.Office.Tools.Outlook.FormRegionName("OutlookPrivacyPlugin.FormRegionStatus")]
		public partial class FormRegionStatusFactory
		{
			// Occurs before the form region is initialized.
			// To prevent the form region from appearing, set e.Cancel to true.
			// Use e.OutlookItem to get a reference to the current Outlook item.
			private void FormRegionStatusFactory_FormRegionInitializing(object sender, Microsoft.Office.Tools.Outlook.FormRegionInitializingEventArgs e)
			{
				//var mailItem = e.OutlookItem as Outlook.MailItem;
				//if (mailItem != null)
				//{
				//	System.Diagnostics.Debugger.Break();
				//}

			}
		}

		#endregion

		string origionalBody;
		System.Drawing.Color origionalBackColor;
		System.Drawing.Color GoodColor = System.Drawing.Color.LightGreen;
		System.Drawing.Color BadColor = System.Drawing.Color.LightSalmon;
		System.Drawing.Color WarningColor = System.Drawing.Color.LightYellow;
		bool hadCrypto = false;

		// Occurs before the form region is displayed.
		// Use this.OutlookItem to get a reference to the current Outlook item.
		// Use this.OutlookFormRegion to get a reference to the form region.
		private void FormRegionStatus_FormRegionShowing(object sender, System.EventArgs e)
		{
			origionalBackColor = this.BackColor;

			var mailItem = this.OutlookItem as Outlook.MailItem;

			if (mailItem.BodyFormat == Outlook.OlBodyFormat.olFormatPlain)
				origionalBody = mailItem.Body;
			else
				origionalBody = mailItem.HTMLBody;

			var Context = OutlookPrivacyPlugin.Instance.HandleOpenRead(mailItem);
			if (Context == null)
			{
				this.Hide();
				return;
			}
			else
				this.Show();

			hadCrypto = true;

			var DecryptAndVerifyHeaderMessage = "** ";

			if (Context.IsEncrypted)
			{
				DecryptAndVerifyHeaderMessage += Localized.MsgDecrypt + " ";
				this.BackColor = GoodColor;
			}

			if (Context.FailedIntegrityCheck)
			{
				DecryptAndVerifyHeaderMessage += Localized.MsgFailedIntegrityCheck + " ";
				this.BackColor = WarningColor;
			}

			if (Context.IsSigned && Context.SignatureValidated)
			{
				DecryptAndVerifyHeaderMessage += string.Format(Localized.MsgValidSig,
					Context.SignedByUserId, Context.SignedByKeyId);
				this.BackColor = GoodColor;
			}
			else if (Context.IsSigned)
			{
				DecryptAndVerifyHeaderMessage += string.Format(Localized.MsgInvalidSig,
					Context.SignedByUserId, Context.SignedByKeyId);

				this.BackColor = BadColor;
			}
			else
				DecryptAndVerifyHeaderMessage += Localized.MsgUnsigned;

			this.label1.Text = DecryptAndVerifyHeaderMessage;
		}

		// Occurs when the form region is closed.
		// Use this.OutlookItem to get a reference to the current Outlook item.
		// Use this.OutlookFormRegion to get a reference to the form region.
		private void FormRegionStatus_FormRegionClosed(object sender, System.EventArgs e)
		{
			if (!hadCrypto)
				return;

			var mailItem = this.OutlookItem as Outlook.MailItem;
			if (mailItem.BodyFormat == Outlook.OlBodyFormat.olFormatPlain)
				mailItem.Body = origionalBody;
			else
				mailItem.HTMLBody = origionalBody;

			if (true)
			{
				// NOTE: Cannot call mailItem.Close from Close event handler
				//       instead we will start a timer and call it after we
				//       return. There is a small race condition, but 250
				//       milliseconds should be enough even on slow machines.

				var timer = new System.Windows.Forms.Timer { Interval = 250 };
				timer.Tick += new EventHandler((o, ee) =>
				{
					timer.Stop();
					((Outlook._MailItem)mailItem).Close(Microsoft.Office.Interop.Outlook.OlInspectorClose.olDiscard);
				});

				timer.Start();
			}
		}
	}
}
