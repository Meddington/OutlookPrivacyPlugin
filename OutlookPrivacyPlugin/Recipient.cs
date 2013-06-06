using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace OutlookPrivacyPlugin
{
	internal partial class Recipient : Form
	{
		private readonly List<string> _defaultKeys;

		internal IList<string> SelectedKeys
		{
			get
			{
				List<string> recipients = new List<string>();

				for (int i = 0; i < KeyBox.Items.Count; i++)
				{
					GnuKey recipient = (GnuKey)KeyBox.Items[i];
					if (KeyBox.GetItemChecked(i))
						recipients.Add(recipient.Key);
				}

				return recipients;
			}
		}

		internal Recipient(List<string> defaultRecipients)
		{
			_defaultKeys = defaultRecipients;
			InitializeComponent();
		}

		private void Passphrase_Load(object sender, EventArgs e)
		{
			// Did we locate all recipients keys?
			bool unfoundKeys = true;

			IList<GnuKey> keys = Globals.OutlookPrivacyPlugin.GetKeys();
			if (keys.Count <= 0)
			{
				// No keys available, no use in showing this dialog at all
				Hide();
				return;
			}

			List<GnuKey> datasource = new List<GnuKey>();
			int selectedCount = 0;

			// 1/ Collect selected keys and sort them.
			foreach (GnuKey gnuKey in keys)
			{
				if (null != _defaultKeys.Find(delegate(string key) { return gnuKey.Key.StartsWith(key, true, null); }))
				{
					selectedCount++;
					datasource.Add(gnuKey);
				}
			}

			datasource.Sort(new GnuKeySorter());

			// If we found all the keys we don't need to show dialog
			if (datasource.Count == _defaultKeys.Count)
			{
				unfoundKeys = false;
			}

			// 2/ Collect unselected keys and sort them.
			List<GnuKey> datasource2 = new List<GnuKey>();
			foreach (GnuKey gnuKey in keys)
			{
				if (null == _defaultKeys.Find(delegate(string key) { return gnuKey.Key.StartsWith(key, true, null); }))
					datasource2.Add(gnuKey);
			}

			datasource2.Sort(new GnuKeySorter());

			// Append unselected keys to datasource.
			foreach (GnuKey gnuKey in datasource2)
				datasource.Add(gnuKey);

			// Setup KeyBox
			KeyBox.DataSource = datasource;
			KeyBox.DisplayMember = "KeyDisplay";
			KeyBox.ValueMember = "Key";

			int boxHeight = (keys.Count > 10) ? KeyBox.ItemHeight * 10 : KeyBox.ItemHeight * keys.Count;
			KeyBox.Height = boxHeight + 5;
			Height = boxHeight + 90;

			// Enlarge dialog to fit the longest key
			using (Graphics g = CreateGraphics())
			{
				int maxSize = Width;
				foreach (GnuKey key in datasource)
				{
					int textWidth = (int)g.MeasureString(key.KeyDisplay, KeyBox.Font).Width + 50;
					if (textWidth > maxSize)
						maxSize = textWidth;
				}
				Width = maxSize;
				CenterToScreen();
			}

			// Note: Keybox sorted property MUST be False!
			//       unless the custom sort strategy is voided!
			for (int i = 0; i < selectedCount; i++)
				KeyBox.SetItemChecked(i, true);

			// If we found all the keys we don't need to show dialog
			if (!unfoundKeys)
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private void OldPassphrase_Load(object sender, EventArgs e)
		{
			IList<GnuKey> keys = Globals.OutlookPrivacyPlugin.GetKeys();
			KeyBox.DataSource = keys;
			KeyBox.DisplayMember = "KeyDisplay";
			KeyBox.ValueMember = "Key";

			if (KeyBox.Items.Count <= 0)
			{
				// No keys available, no use in showing this dialog at all
				Hide();
				return;
			}

			int boxHeight = (keys.Count > 10) ? KeyBox.ItemHeight * 10 : KeyBox.ItemHeight * keys.Count;
			KeyBox.Height = boxHeight + 5;
			Height = boxHeight + 90;

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

			for (int i = 0; i < KeyBox.Items.Count; i++)
			{
				GnuKey recipient = (GnuKey)KeyBox.Items[i];
#if DISABLED
        KeyBox.SetItemChecked(i, _defaultKeys.Contains(recipient.Key));
#else
				// Update to support CN from X.400 mail address format.
				// Enable item if the associated key starts with one of the available _defaultKeys (hence a prefix match).
				KeyBox.SetItemChecked(i,
				  null != _defaultKeys.Find(delegate(string gnuKey) { return recipient.Key.StartsWith(gnuKey); }));
#endif

			}
		}
	}

	#region GnuKey_Sorter
	internal class GnuKeySorter : IComparer<GnuKey>
	{
		public int Compare(GnuKey x, GnuKey y)
		{
			return x.KeyDisplay.CompareTo(y.KeyDisplay);
		}
	}
	#endregion
}
