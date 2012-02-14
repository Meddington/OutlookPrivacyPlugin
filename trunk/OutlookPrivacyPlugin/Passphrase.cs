using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace OutlookPrivacyPlugin
{
	internal partial class Passphrase : Form
	{
		private readonly string _defaultKey;
		private const string _enterPhrase = " < enter passphrase > ";
		private char _passwordChar = '*'; // Global password char, if case it's changed 
		// before PreparePrivateKeyField()

		internal string SelectedKey
		{
			get { return KeyBox.SelectedValue.ToString(); }
		}

		internal string EnteredPassphrase
		{
			get { return PrivateKey.Text; }
		}

		internal Passphrase(string defaultKey, string buttonText)
		{
			_defaultKey = defaultKey;
			InitializeComponent();

			OkButton.Text = buttonText;
			PrivateKey.Focus();
			ActiveControl = PrivateKey;
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
			PrivateKey.Focus();
			ActiveControl = PrivateKey;
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
			if (PrivateKey.Text == _enterPhrase)
				PreparePrivateKeyField();
		}

		private void KeyBox_Enter(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(PrivateKey.Text))
				EmptyPrivateKeyField(KeyBox);
		}

		private void OkButton_Enter(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(PrivateKey.Text))
				EmptyPrivateKeyField(OkButton);
		}

		private void OkButton_Click(object sender, EventArgs e)
		{
			if ((PrivateKey.Text == _enterPhrase) || (string.IsNullOrEmpty(PrivateKey.Text)))
			{
				// No Passphrase provided, complain!
				Errors.SetError(PrivateKey, "No passphrase provided!");
				PrivateKey.Margin = new Padding(3, 3, 20, 3);
			}
			else
			{
				Errors.SetError(PrivateKey, string.Empty);
				PrivateKey.Margin = new Padding(3, 3, 3, 3);

				// Hide the form and let our main addin take over again
				Hide();
			}
		}

		private void ShowCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (ShowCheckBox.Checked == true)
				_passwordChar = (char)0;
			else
				_passwordChar = '*';
			PrivateKey.PasswordChar = _passwordChar;
		}
	}
}
