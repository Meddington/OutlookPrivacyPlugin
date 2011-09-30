namespace OutlookPrivacyPlugin
{
  internal partial class Passphrase
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.OkButton = new System.Windows.Forms.Button();
      this.PrivateKey = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.KeyBox = new System.Windows.Forms.ComboBox();
      this.ShowCheckBox = new System.Windows.Forms.CheckBox();
      this.Errors = new System.Windows.Forms.ErrorProvider(this.components);
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.Errors)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.OkButton, 0, 4);
      this.tableLayoutPanel1.Controls.Add(this.PrivateKey, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.KeyBox, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.ShowCheckBox, 0, 3);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 5;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 132);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // OkButton
      // 
      this.OkButton.AutoSize = true;
      this.OkButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.OkButton.CausesValidation = false;
      this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OkButton.Dock = System.Windows.Forms.DockStyle.Fill;
      this.OkButton.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.OkButton.Location = new System.Drawing.Point(3, 101);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(278, 28);
      this.OkButton.TabIndex = 3;
      this.OkButton.Text = "Sign";
      this.OkButton.UseVisualStyleBackColor = true;
      this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
      this.OkButton.Enter += new System.EventHandler(this.OkButton_Enter);
      // 
      // PrivateKey
      // 
      this.PrivateKey.CausesValidation = false;
      this.PrivateKey.Dock = System.Windows.Forms.DockStyle.Fill;
      this.Errors.SetIconPadding(this.PrivateKey, 2);
      this.PrivateKey.Location = new System.Drawing.Point(3, 52);
      this.PrivateKey.Name = "PrivateKey";
      this.PrivateKey.PasswordChar = '*';
      this.PrivateKey.Size = new System.Drawing.Size(278, 20);
      this.PrivateKey.TabIndex = 2;
      this.PrivateKey.Enter += new System.EventHandler(this.PrivateKey_Enter);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label1.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(3, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(278, 22);
      this.label1.TabIndex = 3;
      this.label1.Text = "Select a private key";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // KeyBox
      // 
      this.KeyBox.CausesValidation = false;
      this.KeyBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.KeyBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.KeyBox.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.KeyBox.FormattingEnabled = true;
      this.KeyBox.Location = new System.Drawing.Point(3, 25);
      this.KeyBox.Name = "KeyBox";
      this.KeyBox.Size = new System.Drawing.Size(278, 21);
      this.KeyBox.TabIndex = 1;
      this.KeyBox.Enter += new System.EventHandler(this.KeyBox_Enter);
      // 
      // ShowCheckBox
      // 
      this.ShowCheckBox.AutoSize = true;
      this.ShowCheckBox.Location = new System.Drawing.Point(30, 78);
      this.ShowCheckBox.Margin = new System.Windows.Forms.Padding(30, 3, 3, 3);
      this.ShowCheckBox.Name = "ShowCheckBox";
      this.ShowCheckBox.Size = new System.Drawing.Size(168, 17);
      this.ShowCheckBox.TabIndex = 0;
      this.ShowCheckBox.Text = "Show passphrase while typing";
      this.ShowCheckBox.UseVisualStyleBackColor = true;
      this.ShowCheckBox.CheckedChanged += new System.EventHandler(this.ShowCheckBox_CheckedChanged);
      // 
      // Errors
      // 
      this.Errors.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.Errors.ContainerControl = this;
      // 
      // Passphrase
      // 
      this.AcceptButton = this.OkButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 132);
      this.Controls.Add(this.tableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "Passphrase";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Key Selection";
      this.Load += new System.EventHandler(this.Passphrase_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.Errors)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Button OkButton;
    private System.Windows.Forms.TextBox PrivateKey;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox KeyBox;
    private System.Windows.Forms.CheckBox ShowCheckBox;
    private System.Windows.Forms.ErrorProvider Errors;
  }
}
