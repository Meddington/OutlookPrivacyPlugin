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
			this.label2 = new System.Windows.Forms.Label();
			this.Errors = new System.Windows.Forms.ErrorProvider(this.components);
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Errors)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.OkButton, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this.PrivateKey, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 6;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 80);
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
			this.OkButton.Location = new System.Drawing.Point(3, 51);
			this.OkButton.Name = "OkButton";
			this.OkButton.Size = new System.Drawing.Size(278, 26);
			this.OkButton.TabIndex = 1;
			this.OkButton.Text = "OK";
			this.OkButton.UseVisualStyleBackColor = true;
			// 
			// PrivateKey
			// 
			this.PrivateKey.CausesValidation = false;
			this.PrivateKey.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Errors.SetIconPadding(this.PrivateKey, 2);
			this.PrivateKey.Location = new System.Drawing.Point(3, 25);
			this.PrivateKey.Name = "PrivateKey";
			this.PrivateKey.PasswordChar = '*';
			this.PrivateKey.Size = new System.Drawing.Size(278, 20);
			this.PrivateKey.TabIndex = 0;
			this.PrivateKey.Enter += new System.EventHandler(this.PrivateKey_Enter);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Passphrase:";
			// 
			// Errors
			// 
			this.Errors.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.Errors.ContainerControl = this;
			// 
			// Passphrase
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 80);
			this.Controls.Add(this.tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Passphrase";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Enter Passphrase";
			this.Load += new System.EventHandler(this.Passphrase_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.Errors)).EndInit();
			this.ResumeLayout(false);

    }

    #endregion

	private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.ErrorProvider Errors;
        private System.Windows.Forms.Button OkButton;
		private System.Windows.Forms.TextBox PrivateKey;
        private System.Windows.Forms.Label label2;
  }
}
