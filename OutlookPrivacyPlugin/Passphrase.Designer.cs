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
			this.Errors = new System.Windows.Forms.ErrorProvider(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.labelKeyInfo = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxPassphrase = new System.Windows.Forms.TextBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.Errors)).BeginInit();
			this.SuspendLayout();
			// 
			// Errors
			// 
			this.Errors.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.Errors.ContainerControl = this;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(526, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Please enter the passphrase to unlock the secret key for the OpenPGP certificate:" +
    "";
			// 
			// labelKeyInfo
			// 
			this.labelKeyInfo.AutoSize = true;
			this.labelKeyInfo.Location = new System.Drawing.Point(45, 39);
			this.labelKeyInfo.Name = "labelKeyInfo";
			this.labelKeyInfo.Size = new System.Drawing.Size(193, 51);
			this.labelKeyInfo.TabIndex = 1;
			this.labelKeyInfo.Text = "\"Test <test@test.com>\"\r\n2048-RSA key, ID 9F4EF918,\r\ncreated 2015-05-20";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 121);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(87, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "Passphrase:";
			// 
			// textBoxPassphrase
			// 
			this.textBoxPassphrase.Location = new System.Drawing.Point(107, 121);
			this.textBoxPassphrase.Name = "textBoxPassphrase";
			this.textBoxPassphrase.Size = new System.Drawing.Size(431, 22);
			this.textBoxPassphrase.TabIndex = 3;
			this.textBoxPassphrase.UseSystemPasswordChar = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(438, 159);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(99, 30);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(333, 159);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(99, 30);
			this.buttonOk.TabIndex = 5;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// Passphrase
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(552, 201);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.textBoxPassphrase);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelKeyInfo);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "Passphrase";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Enter Passphrase";
			((System.ComponentModel.ISupportInitialize)(this.Errors)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

	private System.Windows.Forms.ErrorProvider Errors;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.Label labelKeyInfo;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.Button buttonOk;
	private System.Windows.Forms.Button buttonCancel;
	public System.Windows.Forms.TextBox textBoxPassphrase;
  }
}
