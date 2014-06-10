namespace OutlookPrivacyPlugin
{
  internal partial class Settings
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
			this.ButtonsContainer = new System.Windows.Forms.SplitContainer();
			this.OkButton = new System.Windows.Forms.Button();
			this.Errors = new System.Windows.Forms.ErrorProvider(this.components);
			this.GnuPgExeFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
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
			((System.ComponentModel.ISupportInitialize)(this.ButtonsContainer)).BeginInit();
			this.ButtonsContainer.Panel2.SuspendLayout();
			this.ButtonsContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Errors)).BeginInit();
			this.SuspendLayout();
			// 
			// MainContainer
			// 
			this.MainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainContainer.Location = new System.Drawing.Point(0, 0);
			this.MainContainer.Name = "MainContainer";
			this.MainContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// MainContainer.Panel1
			// 
			this.MainContainer.Panel1.Controls.Add(this.MainTabControl);
			// 
			// MainContainer.Panel2
			// 
			this.MainContainer.Panel2.Controls.Add(this.ButtonsContainer);
			this.MainContainer.Size = new System.Drawing.Size(353, 166);
			this.MainContainer.SplitterDistance = 137;
			this.MainContainer.TabIndex = 0;
			// 
			// MainTabControl
			// 
			this.MainTabControl.Controls.Add(this.ComposeTab);
			this.MainTabControl.Controls.Add(this.ReadTab);
			this.MainTabControl.Controls.Add(this.ExchangeServerTab);
			this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.MainTabControl.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MainTabControl.Location = new System.Drawing.Point(0, 0);
			this.MainTabControl.Name = "MainTabControl";
			this.MainTabControl.SelectedIndex = 0;
			this.MainTabControl.Size = new System.Drawing.Size(353, 137);
			this.MainTabControl.TabIndex = 0;
			// 
			// ComposeTab
			// 
			this.ComposeTab.Controls.Add(this.ComposeTableLayout);
			this.ComposeTab.Location = new System.Drawing.Point(4, 22);
			this.ComposeTab.Name = "ComposeTab";
			this.ComposeTab.Padding = new System.Windows.Forms.Padding(3);
			this.ComposeTab.Size = new System.Drawing.Size(345, 111);
			this.ComposeTab.TabIndex = 0;
			this.ComposeTab.Text = "Compose";
			this.ComposeTab.UseVisualStyleBackColor = true;
			// 
			// ComposeTableLayout
			// 
			this.ComposeTableLayout.AutoSize = true;
			this.ComposeTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ComposeTableLayout.ColumnCount = 2;
			this.ComposeTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 18F));
			this.ComposeTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.ComposeTableLayout.Controls.Add(this.SignCheckBox, 1, 0);
			this.ComposeTableLayout.Controls.Add(this.EncryptCheckBox, 1, 1);
			this.ComposeTableLayout.Controls.Add(this.Encrypt2SelfCheckBox, 1, 2);
			this.ComposeTableLayout.Controls.Add(this.Default2PlainTextCheckBox, 1, 3);
			this.ComposeTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ComposeTableLayout.Location = new System.Drawing.Point(3, 3);
			this.ComposeTableLayout.Name = "ComposeTableLayout";
			this.ComposeTableLayout.RowCount = 6;
			this.ComposeTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ComposeTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ComposeTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ComposeTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ComposeTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ComposeTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ComposeTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.ComposeTableLayout.Size = new System.Drawing.Size(339, 105);
			this.ComposeTableLayout.TabIndex = 0;
			// 
			// SignCheckBox
			// 
			this.SignCheckBox.AutoSize = true;
			this.SignCheckBox.Location = new System.Drawing.Point(21, 3);
			this.SignCheckBox.Name = "SignCheckBox";
			this.SignCheckBox.Size = new System.Drawing.Size(186, 17);
			this.SignCheckBox.TabIndex = 0;
			this.SignCheckBox.Text = "Automatically Sign New Mail";
			this.SignCheckBox.UseVisualStyleBackColor = true;
			// 
			// EncryptCheckBox
			// 
			this.EncryptCheckBox.AutoSize = true;
			this.EncryptCheckBox.Location = new System.Drawing.Point(21, 26);
			this.EncryptCheckBox.Name = "EncryptCheckBox";
			this.EncryptCheckBox.Size = new System.Drawing.Size(204, 17);
			this.EncryptCheckBox.TabIndex = 1;
			this.EncryptCheckBox.Text = "Automatically Encrypt New Mail";
			this.EncryptCheckBox.UseVisualStyleBackColor = true;
			// 
			// Encrypt2SelfCheckBox
			// 
			this.Encrypt2SelfCheckBox.AutoSize = true;
			this.Encrypt2SelfCheckBox.Location = new System.Drawing.Point(21, 49);
			this.Encrypt2SelfCheckBox.Name = "Encrypt2SelfCheckBox";
			this.Encrypt2SelfCheckBox.Size = new System.Drawing.Size(138, 17);
			this.Encrypt2SelfCheckBox.TabIndex = 1;
			this.Encrypt2SelfCheckBox.Text = "Encrypt Mail To Self";
			this.Encrypt2SelfCheckBox.UseVisualStyleBackColor = true;
			// 
			// Default2PlainTextCheckBox
			// 
			this.Default2PlainTextCheckBox.AutoSize = true;
			this.Default2PlainTextCheckBox.Location = new System.Drawing.Point(21, 72);
			this.Default2PlainTextCheckBox.Name = "Default2PlainTextCheckBox";
			this.Default2PlainTextCheckBox.Size = new System.Drawing.Size(241, 17);
			this.Default2PlainTextCheckBox.TabIndex = 4;
			this.Default2PlainTextCheckBox.Text = "Automatically Change to Plain Format";
			this.Default2PlainTextCheckBox.UseVisualStyleBackColor = true;
			// 
			// ReadTab
			// 
			this.ReadTab.Controls.Add(this.ReadTableLayout);
			this.ReadTab.Location = new System.Drawing.Point(4, 22);
			this.ReadTab.Name = "ReadTab";
			this.ReadTab.Padding = new System.Windows.Forms.Padding(3);
			this.ReadTab.Size = new System.Drawing.Size(345, 111);
			this.ReadTab.TabIndex = 1;
			this.ReadTab.Text = "Read";
			this.ReadTab.UseVisualStyleBackColor = true;
			// 
			// ReadTableLayout
			// 
			this.ReadTableLayout.AutoSize = true;
			this.ReadTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ReadTableLayout.ColumnCount = 2;
			this.ReadTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 18F));
			this.ReadTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.ReadTableLayout.Controls.Add(this.VerifyCheckBox, 1, 0);
			this.ReadTableLayout.Controls.Add(this.DecryptCheckBox, 1, 1);
			this.ReadTableLayout.Controls.Add(this.IgnoreIntegrityCheckBox, 1, 3);
			this.ReadTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ReadTableLayout.Location = new System.Drawing.Point(3, 3);
			this.ReadTableLayout.Name = "ReadTableLayout";
			this.ReadTableLayout.RowCount = 4;
			this.ReadTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ReadTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ReadTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ReadTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ReadTableLayout.Size = new System.Drawing.Size(339, 105);
			this.ReadTableLayout.TabIndex = 1;
			// 
			// VerifyCheckBox
			// 
			this.VerifyCheckBox.AutoSize = true;
			this.VerifyCheckBox.Location = new System.Drawing.Point(21, 3);
			this.VerifyCheckBox.Name = "VerifyCheckBox";
			this.VerifyCheckBox.Size = new System.Drawing.Size(214, 17);
			this.VerifyCheckBox.TabIndex = 0;
			this.VerifyCheckBox.Text = "Automatically Verify Opened Mail";
			this.VerifyCheckBox.UseVisualStyleBackColor = true;
			// 
			// DecryptCheckBox
			// 
			this.DecryptCheckBox.AutoSize = true;
			this.DecryptCheckBox.Location = new System.Drawing.Point(21, 26);
			this.DecryptCheckBox.Name = "DecryptCheckBox";
			this.DecryptCheckBox.Size = new System.Drawing.Size(226, 17);
			this.DecryptCheckBox.TabIndex = 1;
			this.DecryptCheckBox.Text = "Automatically Decrypt Opened Mail";
			this.DecryptCheckBox.UseVisualStyleBackColor = true;
			// 
			// IgnoreIntegrityCheckBox
			// 
			this.IgnoreIntegrityCheckBox.AutoSize = true;
			this.IgnoreIntegrityCheckBox.Location = new System.Drawing.Point(21, 49);
			this.IgnoreIntegrityCheckBox.Name = "IgnoreIntegrityCheckBox";
			this.IgnoreIntegrityCheckBox.Size = new System.Drawing.Size(204, 17);
			this.IgnoreIntegrityCheckBox.TabIndex = 2;
			this.IgnoreIntegrityCheckBox.Text = "Ignore Integrity Check Failures";
			this.IgnoreIntegrityCheckBox.UseVisualStyleBackColor = true;
			// 
			// ExchangeServerTab
			// 
			this.ExchangeServerTab.Controls.Add(this.ExchangeServerTableLayout);
			this.ExchangeServerTab.Location = new System.Drawing.Point(4, 22);
			this.ExchangeServerTab.Name = "ExchangeServerTab";
			this.ExchangeServerTab.Padding = new System.Windows.Forms.Padding(3);
			this.ExchangeServerTab.Size = new System.Drawing.Size(345, 111);
			this.ExchangeServerTab.TabIndex = 3;
			this.ExchangeServerTab.Text = "Exchange Server";
			this.ExchangeServerTab.UseVisualStyleBackColor = true;
			// 
			// ExchangeServerTableLayout
			// 
			this.ExchangeServerTableLayout.AutoSize = true;
			this.ExchangeServerTableLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ExchangeServerTableLayout.ColumnCount = 2;
			this.ExchangeServerTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 18F));
			this.ExchangeServerTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.ExchangeServerTableLayout.Controls.Add(this.DefaultDomainLabel, 1, 0);
			this.ExchangeServerTableLayout.Controls.Add(this.DefaultDomainTextBox, 1, 1);
			this.ExchangeServerTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ExchangeServerTableLayout.Location = new System.Drawing.Point(3, 3);
			this.ExchangeServerTableLayout.Name = "ExchangeServerTableLayout";
			this.ExchangeServerTableLayout.RowCount = 3;
			this.ExchangeServerTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ExchangeServerTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.ExchangeServerTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.ExchangeServerTableLayout.Size = new System.Drawing.Size(339, 105);
			this.ExchangeServerTableLayout.TabIndex = 0;
			// 
			// DefaultDomainLabel
			// 
			this.DefaultDomainLabel.AutoSize = true;
			this.DefaultDomainLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DefaultDomainLabel.Location = new System.Drawing.Point(21, 0);
			this.DefaultDomainLabel.Name = "DefaultDomainLabel";
			this.DefaultDomainLabel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
			this.DefaultDomainLabel.Size = new System.Drawing.Size(315, 23);
			this.DefaultDomainLabel.TabIndex = 0;
			this.DefaultDomainLabel.Text = "Default Domain Name";
			// 
			// DefaultDomainTextBox
			// 
			this.DefaultDomainTextBox.CausesValidation = false;
			this.DefaultDomainTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Errors.SetIconPadding(this.DefaultDomainTextBox, 2);
			this.DefaultDomainTextBox.Location = new System.Drawing.Point(21, 26);
			this.DefaultDomainTextBox.MaxLength = 64;
			this.DefaultDomainTextBox.Name = "DefaultDomainTextBox";
			this.DefaultDomainTextBox.Size = new System.Drawing.Size(315, 20);
			this.DefaultDomainTextBox.TabIndex = 1;
			this.DefaultDomainTextBox.TextChanged += new System.EventHandler(this.DefaultDomainTextBox_TextChanged);
			// 
			// ButtonsContainer
			// 
			this.ButtonsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ButtonsContainer.Location = new System.Drawing.Point(0, 0);
			this.ButtonsContainer.Name = "ButtonsContainer";
			// 
			// ButtonsContainer.Panel2
			// 
			this.ButtonsContainer.Panel2.Controls.Add(this.OkButton);
			this.ButtonsContainer.Size = new System.Drawing.Size(353, 25);
			this.ButtonsContainer.SplitterDistance = 166;
			this.ButtonsContainer.SplitterWidth = 5;
			this.ButtonsContainer.TabIndex = 0;
			// 
			// OkButton
			// 
			this.OkButton.Dock = System.Windows.Forms.DockStyle.Fill;
			this.OkButton.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OkButton.Location = new System.Drawing.Point(0, 0);
			this.OkButton.Name = "OkButton";
			this.OkButton.Size = new System.Drawing.Size(182, 25);
			this.OkButton.TabIndex = 0;
			this.OkButton.Text = "Ok";
			this.OkButton.UseVisualStyleBackColor = true;
			this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
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
			// Settings
			// 
			this.AcceptButton = this.OkButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(353, 166);
			this.Controls.Add(this.MainContainer);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "Settings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Settings";
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
  }
}
