namespace OutlookPrivacyPlugin
{
  internal partial class FormPassphrase
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPassphrase));
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
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// labelKeyInfo
			// 
			resources.ApplyResources(this.labelKeyInfo, "labelKeyInfo");
			this.labelKeyInfo.Name = "labelKeyInfo";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// textBoxPassphrase
			// 
			resources.ApplyResources(this.textBoxPassphrase, "textBoxPassphrase");
			this.textBoxPassphrase.Name = "textBoxPassphrase";
			this.textBoxPassphrase.UseSystemPasswordChar = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// FormPassphrase
			// 
			this.AcceptButton = this.buttonOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.textBoxPassphrase);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelKeyInfo);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "FormPassphrase";
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
