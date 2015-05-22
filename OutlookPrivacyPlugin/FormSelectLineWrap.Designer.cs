namespace OutlookPrivacyPlugin
{
	partial class FormSelectLineWrap
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
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButtonEdit = new System.Windows.Forms.RadioButton();
			this.radioButtonMime = new System.Windows.Forms.RadioButton();
			this.radioButtonAsIs = new System.Windows.Forms.RadioButton();
			this.radioButtonWrap = new System.Windows.Forms.RadioButton();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(524, 34);
			this.label1.TabIndex = 0;
			this.label1.Text = "You have very long lines in your mail. They should be handled differently to avoi" +
    "d \r\nbad signatures.";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonEdit);
			this.groupBox1.Controls.Add(this.radioButtonMime);
			this.groupBox1.Controls.Add(this.radioButtonAsIs);
			this.groupBox1.Controls.Add(this.radioButtonWrap);
			this.groupBox1.Location = new System.Drawing.Point(16, 65);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(521, 153);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "How do you want to proceed?";
			// 
			// radioButtonEdit
			// 
			this.radioButtonEdit.AutoSize = true;
			this.radioButtonEdit.Location = new System.Drawing.Point(7, 112);
			this.radioButtonEdit.Name = "radioButtonEdit";
			this.radioButtonEdit.Size = new System.Drawing.Size(113, 21);
			this.radioButtonEdit.TabIndex = 3;
			this.radioButtonEdit.Text = "Edit manually";
			this.radioButtonEdit.UseVisualStyleBackColor = true;
			// 
			// radioButtonMime
			// 
			this.radioButtonMime.AutoSize = true;
			this.radioButtonMime.Location = new System.Drawing.Point(7, 85);
			this.radioButtonMime.Name = "radioButtonMime";
			this.radioButtonMime.Size = new System.Drawing.Size(471, 21);
			this.radioButtonMime.TabIndex = 2;
			this.radioButtonMime.Text = "Use PGP/MIME (good signature, but some receivers cannot decode it)";
			this.radioButtonMime.UseVisualStyleBackColor = true;
			this.radioButtonMime.Visible = false;
			// 
			// radioButtonAsIs
			// 
			this.radioButtonAsIs.AutoSize = true;
			this.radioButtonAsIs.Location = new System.Drawing.Point(7, 58);
			this.radioButtonAsIs.Name = "radioButtonAsIs";
			this.radioButtonAsIs.Size = new System.Drawing.Size(262, 21);
			this.radioButtonAsIs.TabIndex = 1;
			this.radioButtonAsIs.Text = "Send as is (and risk a bad signature)";
			this.radioButtonAsIs.UseVisualStyleBackColor = true;
			// 
			// radioButtonWrap
			// 
			this.radioButtonWrap.AutoSize = true;
			this.radioButtonWrap.Checked = true;
			this.radioButtonWrap.Location = new System.Drawing.Point(7, 31);
			this.radioButtonWrap.Name = "radioButtonWrap";
			this.radioButtonWrap.Size = new System.Drawing.Size(406, 21);
			this.radioButtonWrap.TabIndex = 0;
			this.radioButtonWrap.TabStop = true;
			this.radioButtonWrap.Text = "Wrap automatically (good signature, but may not look good)";
			this.radioButtonWrap.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(354, 224);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(87, 32);
			this.buttonOk.TabIndex = 3;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(447, 224);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(90, 32);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// SelectLineWrap
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(557, 269);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Name = "SelectLineWrap";
			this.Text = "Select Line Wrap";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		public System.Windows.Forms.RadioButton radioButtonEdit;
		public System.Windows.Forms.RadioButton radioButtonMime;
		public System.Windows.Forms.RadioButton radioButtonAsIs;
		public System.Windows.Forms.RadioButton radioButtonWrap;
	}
}