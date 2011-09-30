using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace OutlookPrivacyPlugin
{
  internal partial class Settings : Form
  {
    private readonly int _originalExeWidth;

    internal Settings(Properties.Settings settings)
    {
      InitializeComponent();

      AutoDecrypt = settings.AutoDecrypt;
      AutoVerify = settings.AutoVerify;
      AutoEncrypt = settings.AutoEncrypt;
      AutoSign = settings.AutoSign;
      Encrypt2Self = settings.Encrypt2Self;

      DefaultKey = settings.DefaultKey;
      GnuPgPath = settings.GnuPgPath;
      _originalExeWidth = GnuPgExe.Width;
      DefaultDomain = settings.DefaultDomain;

      // Temporary disable all settings regarding auto-verify/decrypt
      // MainTabControl.TabPages.RemoveByKey(ReadTab.Name);
    }

    private string m_DefaultKey;
    internal string DefaultKey { get { return m_DefaultKey; } set { m_DefaultKey = value; } }

    internal bool AutoDecrypt
    {
      get { return DecryptCheckBox.Checked; }
      private set { DecryptCheckBox.Checked = value; }
    }

    internal bool AutoVerify
    {
      get { return VerifyCheckBox.Checked; }
      private set { VerifyCheckBox.Checked = value; }
    }

    internal bool AutoEncrypt
    {
      get { return EncryptCheckBox.Checked; }
      private set { EncryptCheckBox.Checked = value; }
    }

    internal bool AutoSign
    {
      get { return SignCheckBox.Checked; }
      private set { SignCheckBox.Checked = value; }
    }

    internal bool Encrypt2Self
    {
      get { return Encrypt2SelfCheckBox.Checked; }
      private set { Encrypt2SelfCheckBox.Checked = value; }
    }

    internal string GnuPgPath
    {
      get { return GnuPgExe.Text; }
      private set
      {
        GnuPgExe.Text = value;

        if (string.IsNullOrEmpty(value))
        {
          ComposeTab.Enabled = ReadTab.Enabled = false;
          MainTabControl.TabPages.RemoveByKey(ComposeTab.Name);
          //MainTabControl.TabPages.RemoveByKey(ReadTab.Name);
        }
        else
        {
          ComposeTab.Enabled = ReadTab.Enabled = true;

          if (!MainTabControl.TabPages.ContainsKey(ComposeTab.Name))
            MainTabControl.TabPages.Add(ComposeTab);

          if (!MainTabControl.TabPages.ContainsKey(ReadTab.Name))
              MainTabControl.TabPages.Add(ReadTab);
        }

        PopulatePrivateKeys(!string.IsNullOrEmpty(value));
      }
    }

    internal string DefaultDomain
    {
      get { return DefaultDomainTextBox.Text; }
      set { DefaultDomainTextBox.Text = value; }
    }

    private void BrowseButton_Click(object sender, System.EventArgs e)
    {
      if (!string.IsNullOrEmpty(GnuPgPath))
        GnuPgExeFolderDialog.SelectedPath = GnuPgPath;

      DialogResult result = GnuPgExeFolderDialog.ShowDialog(this);
      if (result != DialogResult.OK)
        return;

      GnuPgPath = GnuPgExeFolderDialog.SelectedPath;
      OkButton.Enabled = ValidateGnuPath();
    }

    private void PopulatePrivateKeys(bool gotGnu)
    {
      IList<GnuKey> keys = gotGnu ? Globals.OutlookPrivacyPlugin.GetPrivateKeys(GnuPgPath) : new List<GnuKey>();

      KeyBox.DataSource = keys;
      KeyBox.DisplayMember = "KeyDisplay";
      KeyBox.ValueMember = "Key";

      if (KeyBox.Items.Count <= 0)
        return;

      KeyBox.SelectedValue = DefaultKey;

      // Enlarge dialog to fit the longest key
      using (Graphics g = CreateGraphics())
      {
        int maxSize = Width;
        foreach (GnuKey key in keys)
        {
          int textWidth = (int)g.MeasureString(key.KeyDisplay, KeyBox.Font).Width + 50 + 27;
          if (textWidth > maxSize)
            maxSize = textWidth;
        }
        Width = maxSize;
        CenterToScreen();
      }
    }

    private void OkButton_Click(object sender, System.EventArgs e)
    {
      if (!ValidateGnuPath())
        return;

      DialogResult = DialogResult.OK;
      Close();
    }

    private bool ValidateGnuPath()
    {
      if (string.IsNullOrEmpty(GnuPgPath))
      {
        // No GnuPath provided, complain!
        Errors.SetError(GnuPgExe, "No GnuPG provided!");
        GnuPgExe.Dock = DockStyle.None;
        GnuPgExe.Width = _originalExeWidth - 17;
        return false;
      }

      if ( !Globals.OutlookPrivacyPlugin.ValidateGnuPath(GnuPgPath) )
      {
        // No gpg.exe found, complain!
        Errors.SetError(GnuPgExe, "No gpg(2).exe found in directory!");
        GnuPgExe.Dock = DockStyle.None;
        GnuPgExe.Width = _originalExeWidth - 17;
        return false;
      }

      // All fine
      Errors.SetError(GnuPgExe, string.Empty);
      GnuPgExe.Dock = DockStyle.Fill;
      GnuPgExe.Width = _originalExeWidth;

      DefaultKey = (KeyBox.Items.Count > 0)
          ? ((KeyBox.SelectedValue != null) ? KeyBox.SelectedValue.ToString() : string.Empty)
          : string.Empty;

      return true;
    }

    private void GnuPgExe_TextChanged(object sender, System.EventArgs e)
    {
    }

    private void DefaultDomainTextBox_TextChanged(object sender, System.EventArgs e)
    {
      DefaultDomain = DefaultDomainTextBox.Text;
    }
  }
}
