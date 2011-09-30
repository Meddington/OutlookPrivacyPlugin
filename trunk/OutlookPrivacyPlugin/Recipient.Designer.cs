namespace OutlookPrivacyPlugin
{
  internal partial class Recipient
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
      this.label1 = new System.Windows.Forms.Label();
      this.KeyBox = new System.Windows.Forms.CheckedListBox();
      this.Errors = new System.Windows.Forms.ErrorProvider(this.components);
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.Errors)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.OkButton, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.KeyBox, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 91);
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
      this.OkButton.Location = new System.Drawing.Point(3, 65);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(278, 23);
      this.OkButton.TabIndex = 3;
      this.OkButton.Text = "Select";
      this.OkButton.UseVisualStyleBackColor = true;
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
      this.label1.Text = "Select recipients";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // KeyBox
      // 
      this.KeyBox.CheckOnClick = true;
      this.KeyBox.Dock = System.Windows.Forms.DockStyle.Top;
      this.KeyBox.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.KeyBox.FormattingEnabled = true;
      this.KeyBox.Location = new System.Drawing.Point(3, 25);
      this.KeyBox.Name = "KeyBox";
      this.KeyBox.Size = new System.Drawing.Size(278, 34);
      this.KeyBox.TabIndex = 4;
      // 
      // Errors
      // 
      this.Errors.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.Errors.ContainerControl = this;
      // 
      // Recipient
      // 
      this.AcceptButton = this.OkButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 91);
      this.Controls.Add(this.tableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "Recipient";
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
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ErrorProvider Errors;
    private System.Windows.Forms.Button OkButton;
    private System.Windows.Forms.CheckedListBox KeyBox;
  }
}