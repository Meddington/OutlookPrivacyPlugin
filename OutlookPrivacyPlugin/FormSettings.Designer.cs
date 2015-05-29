namespace OutlookPrivacyPlugin
{
  internal partial class FormSettings
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
			this.MainContainer = new System.Windows.Forms.SplitContainer();
			this.MainTabControl = new System.Windows.Forms.TabControl();
			this.ComposeTab = new System.Windows.Forms.TabPage();
			this.ComposeTableLayout = new System.Windows.Forms.TableLayoutPanel();
			this.SignCheckBox = new System.Windows.Forms.CheckBox();
			this.EncryptCheckBox = new System.Windows.Forms.CheckBox();
			this.Encrypt2SelfCheckBox = new System.Windows.Forms.CheckBox();
			this.Default2PlainTextCheckBox = new System.Windows.Forms.CheckBox();
			this.ReadTab = new System.Windows.Forms.TabPage();
			this.ReadTableLayout = new System.Windows.Forms.TableLayoutPanel();
			this.VerifyCheckBox = new System.Windows.Forms.CheckBox();
			this.DecryptCheckBox = new System.Windows.Forms.CheckBox();
			this.IgnoreIntegrityCheckBox = new System.Windows.Forms.CheckBox();
			this.ExchangeServerTab = new System.Windows.Forms.TabPage();
			this.ExchangeServerTableLayout = new System.Windows.Forms.TableLayoutPanel();
			this.DefaultDomainLabel = new System.Windows.Forms.Label();
			this.DefaultDomainTextBox = new System.Windows.Forms.TextBox();
			this.tabPageCrypto = new System.Windows.Forms.TabPage();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxDigest = new System.Windows.Forms.ComboBox();
			this.comboBoxCipher = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.ButtonsContainer = new System.Windows.Forms.SplitContainer();
			this.OkButton = new System.Windows.Forms.Button();
			this.Errors = new System.Windows.Forms.ErrorProvider(this.components);
			this.GnuPgExeFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.checkBoxSaveDecrypted = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.MainContainer)).BeginInit();
			this.MainContainer.Panel1.SuspendLayout();
			this.MainContainer.Panel2.SuspendLayout();
			this.MainContainer.SuspendLayout();
			this.MainTabControl.SuspendLayout();
			this.ComposeTab.SuspendLayout();
			this.ComposeTableLayout.SuspendLayout();
			this.ReadTab.SuspendLayout();
			this.ReadTableLayout.SuspendLayout();
			this.ExchangeServerTab.SuspendLayout();
			this.ExchangeServerTableLayout.SuspendLayout();
			this.tabPageCrypto.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ButtonsContainer)).BeginInit();
			this.ButtonsContainer.Panel2.SuspendLayout();
			this.ButtonsContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Errors)).BeginInit();
			this.SuspendLayout();
			// 
			// MainContainer
			// 
			resources.ApplyResources(this.MainContainer, "MainContainer");
			this.MainContainer.Name = "MainContainer";
			// 
			// MainContainer.Panel1
			// 
			this.MainContainer.Panel1.Controls.Add(this.MainTabControl);
			// 
			// MainContainer.Panel2
			// 
			this.MainContainer.Panel2.Controls.Add(this.ButtonsContainer);
			// 
			// MainTabControl
			// 
			this.MainTabControl.Controls.Add(this.ComposeTab);
			this.MainTabControl.Controls.Add(this.ReadTab);
			this.MainTabControl.Controls.Add(this.ExchangeServerTab);
			this.MainTabControl.Controls.Add(this.tabPageCrypto);
			resources.ApplyResources(this.MainTabControl, "MainTabControl");
			this.MainTabControl.Name = "MainTabControl";
			this.MainTabControl.SelectedIndex = 0;
			// 
			// ComposeTab
			// 
			this.ComposeTab.Controls.Add(this.ComposeTableLayout);
			resources.ApplyResources(this.ComposeTab, "ComposeTab");
			this.ComposeTab.Name = "ComposeTab";
			this.ComposeTab.UseVisualStyleBackColor = true;
			// 
			// ComposeTableLayout
			// 
			resources.ApplyResources(this.ComposeTableLayout, "ComposeTableLayout");
			this.ComposeTableLayout.Controls.Add(this.SignCheckBox, 1, 0);
			this.ComposeTableLayout.Controls.Add(this.EncryptCheckBox, 1, 1);
			this.ComposeTableLayout.Controls.Add(this.Encrypt2SelfCheckBox, 1, 2);
			this.ComposeTableLayout.Controls.Add(this.Default2PlainTextCheckBox, 1, 3);
			this.ComposeTableLayout.Name = "ComposeTableLayout";
			// 
			// SignCheckBox
			// 
			resources.ApplyResources(this.SignCheckBox, "SignCheckBox");
			this.SignCheckBox.Name = "SignCheckBox";
			this.SignCheckBox.UseVisualStyleBackColor = true;
			// 
			// EncryptCheckBox
			// 
			resources.ApplyResources(this.EncryptCheckBox, "EncryptCheckBox");
			this.EncryptCheckBox.Name = "EncryptCheckBox";
			this.EncryptCheckBox.UseVisualStyleBackColor = true;
			// 
			// Encrypt2SelfCheckBox
			// 
			resources.ApplyResources(this.Encrypt2SelfCheckBox, "Encrypt2SelfCheckBox");
			this.Encrypt2SelfCheckBox.Name = "Encrypt2SelfCheckBox";
			this.Encrypt2SelfCheckBox.UseVisualStyleBackColor = true;
			// 
			// Default2PlainTextCheckBox
			// 
			resources.ApplyResources(this.Default2PlainTextCheckBox, "Default2PlainTextCheckBox");
			this.Default2PlainTextCheckBox.Name = "Default2PlainTextCheckBox";
			this.Default2PlainTextCheckBox.UseVisualStyleBackColor = true;
			// 
			// ReadTab
			// 
			this.ReadTab.Controls.Add(this.ReadTableLayout);
			resources.ApplyResources(this.ReadTab, "ReadTab");
			this.ReadTab.Name = "ReadTab";
			this.ReadTab.UseVisualStyleBackColor = true;
			// 
			// ReadTableLayout
			// 
			resources.ApplyResources(this.ReadTableLayout, "ReadTableLayout");
			this.ReadTableLayout.Controls.Add(this.VerifyCheckBox, 1, 0);
			this.ReadTableLayout.Controls.Add(this.DecryptCheckBox, 1, 1);
			this.ReadTableLayout.Controls.Add(this.IgnoreIntegrityCheckBox, 1, 3);
			this.ReadTableLayout.Controls.Add(this.checkBoxSaveDecrypted, 1, 4);
			this.ReadTableLayout.Name = "ReadTableLayout";
			// 
			// VerifyCheckBox
			// 
			resources.ApplyResources(this.VerifyCheckBox, "VerifyCheckBox");
			this.VerifyCheckBox.Name = "VerifyCheckBox";
			this.VerifyCheckBox.UseVisualStyleBackColor = true;
			// 
			// DecryptCheckBox
			// 
			resources.ApplyResources(this.DecryptCheckBox, "DecryptCheckBox");
			this.DecryptCheckBox.Name = "DecryptCheckBox";
			this.DecryptCheckBox.UseVisualStyleBackColor = true;
			// 
			// IgnoreIntegrityCheckBox
			// 
			resources.ApplyResources(this.IgnoreIntegrityCheckBox, "IgnoreIntegrityCheckBox");
			this.IgnoreIntegrityCheckBox.Name = "IgnoreIntegrityCheckBox";
			this.IgnoreIntegrityCheckBox.UseVisualStyleBackColor = true;
			// 
			// ExchangeServerTab
			// 
			this.ExchangeServerTab.Controls.Add(this.ExchangeServerTableLayout);
			resources.ApplyResources(this.ExchangeServerTab, "ExchangeServerTab");
			this.ExchangeServerTab.Name = "ExchangeServerTab";
			this.ExchangeServerTab.UseVisualStyleBackColor = true;
			// 
			// ExchangeServerTableLayout
			// 
			resources.ApplyResources(this.ExchangeServerTableLayout, "ExchangeServerTableLayout");
			this.ExchangeServerTableLayout.Controls.Add(this.DefaultDomainLabel, 1, 0);
			this.ExchangeServerTableLayout.Controls.Add(this.DefaultDomainTextBox, 1, 1);
			this.ExchangeServerTableLayout.Name = "ExchangeServerTableLayout";
			// 
			// DefaultDomainLabel
			// 
			resources.ApplyResources(this.DefaultDomainLabel, "DefaultDomainLabel");
			this.DefaultDomainLabel.Name = "DefaultDomainLabel";
			// 
			// DefaultDomainTextBox
			// 
			this.DefaultDomainTextBox.CausesValidation = false;
			resources.ApplyResources(this.DefaultDomainTextBox, "DefaultDomainTextBox");
			this.Errors.SetIconPadding(this.DefaultDomainTextBox, ((int)(resources.GetObject("DefaultDomainTextBox.IconPadding"))));
			this.DefaultDomainTextBox.Name = "DefaultDomainTextBox";
			this.DefaultDomainTextBox.TextChanged += new System.EventHandler(this.DefaultDomainTextBox_TextChanged);
			// 
			// tabPageCrypto
			// 
			this.tabPageCrypto.Controls.Add(this.label3);
			this.tabPageCrypto.Controls.Add(this.comboBoxDigest);
			this.tabPageCrypto.Controls.Add(this.comboBoxCipher);
			this.tabPageCrypto.Controls.Add(this.label2);
			this.tabPageCrypto.Controls.Add(this.label1);
			resources.ApplyResources(this.tabPageCrypto, "tabPageCrypto");
			this.tabPageCrypto.Name = "tabPageCrypto";
			this.tabPageCrypto.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// comboBoxDigest
			// 
			this.comboBoxDigest.FormattingEnabled = true;
			this.comboBoxDigest.Items.AddRange(new object[] {
            resources.GetString("comboBoxDigest.Items"),
            resources.GetString("comboBoxDigest.Items1"),
            resources.GetString("comboBoxDigest.Items2"),
            resources.GetString("comboBoxDigest.Items3"),
            resources.GetString("comboBoxDigest.Items4")});
			resources.ApplyResources(this.comboBoxDigest, "comboBoxDigest");
			this.comboBoxDigest.Name = "comboBoxDigest";
			// 
			// comboBoxCipher
			// 
			this.comboBoxCipher.FormattingEnabled = true;
			this.comboBoxCipher.Items.AddRange(new object[] {
            resources.GetString("comboBoxCipher.Items"),
            resources.GetString("comboBoxCipher.Items1"),
            resources.GetString("comboBoxCipher.Items2"),
            resources.GetString("comboBoxCipher.Items3"),
            resources.GetString("comboBoxCipher.Items4")});
			resources.ApplyResources(this.comboBoxCipher, "comboBoxCipher");
			this.comboBoxCipher.Name = "comboBoxCipher";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// ButtonsContainer
			// 
			resources.ApplyResources(this.ButtonsContainer, "ButtonsContainer");
			this.ButtonsContainer.Name = "ButtonsContainer";
			// 
			// ButtonsContainer.Panel2
			// 
			this.ButtonsContainer.Panel2.Controls.Add(this.OkButton);
			// 
			// OkButton
			// 
			this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.OkButton, "OkButton");
			this.OkButton.Name = "OkButton";
			this.OkButton.UseVisualStyleBackColor = true;
			// 
			// Errors
			// 
			this.Errors.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.Errors.ContainerControl = this;
			// 
			// GnuPgExeFolderDialog
			// 
			this.GnuPgExeFolderDialog.ShowNewFolderButton = false;
			// 
			// checkBoxSaveDecrypted
			// 
			resources.ApplyResources(this.checkBoxSaveDecrypted, "checkBoxSaveDecrypted");
			this.checkBoxSaveDecrypted.Name = "checkBoxSaveDecrypted";
			this.checkBoxSaveDecrypted.UseVisualStyleBackColor = true;
			// 
			// FormSettings
			// 
			this.AcceptButton = this.OkButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.MainContainer);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "FormSettings";
			this.Load += new System.EventHandler(this.Settings_Load);
			this.MainContainer.Panel1.ResumeLayout(false);
			this.MainContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.MainContainer)).EndInit();
			this.MainContainer.ResumeLayout(false);
			this.MainTabControl.ResumeLayout(false);
			this.ComposeTab.ResumeLayout(false);
			this.ComposeTab.PerformLayout();
			this.ComposeTableLayout.ResumeLayout(false);
			this.ComposeTableLayout.PerformLayout();
			this.ReadTab.ResumeLayout(false);
			this.ReadTab.PerformLayout();
			this.ReadTableLayout.ResumeLayout(false);
			this.ReadTableLayout.PerformLayout();
			this.ExchangeServerTab.ResumeLayout(false);
			this.ExchangeServerTab.PerformLayout();
			this.ExchangeServerTableLayout.ResumeLayout(false);
			this.ExchangeServerTableLayout.PerformLayout();
			this.tabPageCrypto.ResumeLayout(false);
			this.tabPageCrypto.PerformLayout();
			this.ButtonsContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ButtonsContainer)).EndInit();
			this.ButtonsContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Errors)).EndInit();
			this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer MainContainer;
    private System.Windows.Forms.SplitContainer ButtonsContainer;
    private System.Windows.Forms.Button OkButton;
    private System.Windows.Forms.TabControl MainTabControl;
    private System.Windows.Forms.TabPage ComposeTab;
    private System.Windows.Forms.TabPage ReadTab;
    private System.Windows.Forms.TableLayoutPanel ComposeTableLayout;
    private System.Windows.Forms.CheckBox SignCheckBox;
    private System.Windows.Forms.CheckBox EncryptCheckBox;
	private System.Windows.Forms.CheckBox Encrypt2SelfCheckBox;
    private System.Windows.Forms.TableLayoutPanel ReadTableLayout;
    private System.Windows.Forms.CheckBox VerifyCheckBox;
	private System.Windows.Forms.CheckBox DecryptCheckBox;
    private System.Windows.Forms.ErrorProvider Errors;
    private System.Windows.Forms.FolderBrowserDialog GnuPgExeFolderDialog;
    private System.Windows.Forms.TabPage ExchangeServerTab;
    private System.Windows.Forms.TableLayoutPanel ExchangeServerTableLayout;
    private System.Windows.Forms.Label DefaultDomainLabel;
	private System.Windows.Forms.TextBox DefaultDomainTextBox;
	private System.Windows.Forms.CheckBox Default2PlainTextCheckBox;
	private System.Windows.Forms.CheckBox IgnoreIntegrityCheckBox;
	private System.Windows.Forms.TabPage tabPageCrypto;
	private System.Windows.Forms.ComboBox comboBoxDigest;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.Label label1;
	public System.Windows.Forms.ComboBox comboBoxCipher;
	private System.Windows.Forms.Label label3;
	private System.Windows.Forms.CheckBox checkBoxSaveDecrypted;
  }
}
