using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OutlookPrivacyPlugin
{
	internal partial class Passphrase : Form
	{
		private readonly string _defaultKey;

		internal string SelectedKey
		{
			get { return KeyBox.SelectedValue.ToString(); }
		}

		internal Passphrase(string defaultKey, string buttonText)
		{
			TopMost = true;

			_defaultKey = defaultKey;
			InitializeComponent();

			OkButton.Text = buttonText;
			AcceptButton = OkButton;
		}

		private void Passphrase_Load(object sender, EventArgs e)
		{
			EmptyPrivateKeyField(KeyBox);

			IList<GnuKey> keys = Globals.OutlookPrivacyPlugin.GetPrivateKeys();
			KeyBox.DataSource = keys;
			KeyBox.DisplayMember = "KeyDisplay";
			KeyBox.ValueMember = "Key";

			if (KeyBox.Items.Count <= 0)
			{
				// No keys available, no use in showing this dialog at all
				Hide();
				return;
			}

			// Enlarge dialog to fit the longest key
			using (Graphics g = CreateGraphics())
			{
				int maxSize = Width;
				foreach (GnuKey key in keys)
				{
					int textWidth = (int)g.MeasureString(key.KeyDisplay, KeyBox.Font).Width + 50;
					if (textWidth > maxSize)
						maxSize = textWidth;
				}
				Width = maxSize;
				CenterToScreen();
			}

			KeyBox.SelectedValue = _defaultKey;
		}

		private void EmptyPrivateKeyField(Control focusControl)
		{
			//PrivateKey.PasswordChar = (char)0;
			//PrivateKey.Text = _enterPhrase;
			//PrivateKey.TextAlign = HorizontalAlignment.Center;
			//PrivateKey.ForeColor = Color.LightGray;

			//focusControl.Focus();
		}

		private void PreparePrivateKeyField()
		{
			//PrivateKey.PasswordChar = _passwordChar;
			//PrivateKey.Text = string.Empty;
			//PrivateKey.TextAlign = HorizontalAlignment.Left;
			//PrivateKey.ForeColor = Color.Black;
		}

		private void PrivateKey_Enter(object sender, EventArgs e)
		{
		}

		private void KeyBox_Enter(object sender, EventArgs e)
		{
		}

		private void OkButton_Enter(object sender, EventArgs e)
		{
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			Hide();
		}

		private void ShowCheckBox_CheckedChanged(object sender, EventArgs e)
		{
		}
	}
}
