using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OutlookPrivacyPlugin
{
	/// <summary>
	/// Called to retreive list of keys to display.
	/// </summary>
	/// <returns></returns>
	public delegate IList<GnuKey> KeySelectionCallback();

	public partial class FormKeySelection : Form
	{
		IList<GnuKey> _keys;
		KeySelectionCallback _keysCallback;
		List<string> _mailRecipients;

		public List<GnuKey> SelectedKeys 
		{
			get
			{
				var selectedKeys = new List<GnuKey>();

				foreach(KeyListViewItem item in listViewKeys.Items)
				{
					if (item.Checked)
						selectedKeys.Add(item.Key);
				}

				return selectedKeys;
			}
		}
		public bool Encrypt { get { return checkBoxSendEncrypted.Checked; } }
		public bool Sign { get { return checkBoxSendSigned.Checked; } }

		public FormKeySelection(List<string> mailRecipients,
			KeySelectionCallback keysCallback, bool encrypt, bool sign)
		{
			InitializeComponent();

			for (int cnt = 0; cnt < mailRecipients.Count; cnt++)
				mailRecipients[cnt] = mailRecipients[cnt].ToLower();

			checkBoxSendEncrypted.Checked = encrypt;
			checkBoxSendSigned.Checked = sign;

			DialogResult = System.Windows.Forms.DialogResult.Ignore;

			_mailRecipients = mailRecipients;
			_keysCallback = keysCallback;
			buttonRefreshKey_Click(null, null);
		}

		void PopulateListBox()
		{
			listViewKeys.Items.Clear();

			foreach(var key in _keys)
			{
				var item = new KeyListViewItem(key.KeyDisplay, key);
				item.Checked = _mailRecipients.Contains(key.Key.ToLower());

				listViewKeys.Items.Add(item);
			}
		}

		private void buttonRefreshKey_Click(object sender, EventArgs e)
		{
			_keys = _keysCallback();
			PopulateListBox();

			if(SelectedKeys.Count == _mailRecipients.Count)
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;
			}
			else
			{
				textBoxMsg.Text = "";
				var foundKeys = SelectedKeys;
				foreach(var email in _mailRecipients.Where(r => (foundKeys.Where(k => k.Key.ToLower() == r).Count() == 0)))
				{
					textBoxMsg.Text += email + " not found; ";
				}
			}
		}
	}

	internal class KeyListViewItem : ListViewItem
	{
		public GnuKey Key { get; set; }

		internal KeyListViewItem(string txt, GnuKey key): base(txt)
		{
			Key = key;

			SubItems.Add(new ListViewItem.ListViewSubItem(this, key.Expiry));
			SubItems.Add(new ListViewItem.ListViewSubItem(this, key.KeyId));
		}
	}
}
