using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Org.BouncyCastle.Bcpg.OpenPgp;


namespace OutlookPrivacyPlugin
{
	internal partial class FormPassphrase : Form
	{
		internal FormPassphrase(PgpSecretKey masterKey, PgpSecretKey key)
		{
			TopMost = true;

			InitializeComponent();

			var userIds = masterKey.UserIds.GetEnumerator();
			userIds.MoveNext();
			var userId = userIds.Current.ToString();
			var strength = key.PublicKey.BitStrength.ToString();
			var createDate = key.PublicKey.CreationTime.ToShortDateString();
			var alg = key.PublicKey.Algorithm.ToString().Replace("Algorithm", "");
			var fingerPrint = key.PublicKey.GetFingerprint();
			var fingerPrintLength = fingerPrint.Length;
			var keyId = 
				fingerPrint[fingerPrintLength - 4].ToString("X2") +
				fingerPrint[fingerPrintLength - 3].ToString("X2") +
				fingerPrint[fingerPrintLength - 2].ToString("X2") +
				fingerPrint[fingerPrintLength - 1].ToString("X2");
 
			labelKeyInfo.Text = string.Format("\"{0}\"\n{1}-{2} key, ID {3}\n{4}",
				userId, strength, alg, keyId, createDate);
		}
	}
}
