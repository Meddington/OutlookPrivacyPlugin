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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSelectLineWrap));
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
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonEdit);
			this.groupBox1.Controls.Add(this.radioButtonMime);
			this.groupBox1.Controls.Add(this.radioButtonAsIs);
			this.groupBox1.Controls.Add(this.radioButtonWrap);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// radioButtonEdit
			// 
			resources.ApplyResources(this.radioButtonEdit, "radioButtonEdit");
			this.radioButtonEdit.Name = "radioButtonEdit";
			this.radioButtonEdit.UseVisualStyleBackColor = true;
			// 
			// radioButtonMime
			// 
			resources.ApplyResources(this.radioButtonMime, "radioButtonMime");
			this.radioButtonMime.Name = "radioButtonMime";
			this.radioButtonMime.UseVisualStyleBackColor = true;
			// 
			// radioButtonAsIs
			// 
			resources.ApplyResources(this.radioButtonAsIs, "radioButtonAsIs");
			this.radioButtonAsIs.Name = "radioButtonAsIs";
			this.radioButtonAsIs.UseVisualStyleBackColor = true;
			// 
			// radioButtonWrap
			// 
			resources.ApplyResources(this.radioButtonWrap, "radioButtonWrap");
			this.radioButtonWrap.Checked = true;
			this.radioButtonWrap.Name = "radioButtonWrap";
			this.radioButtonWrap.TabStop = true;
			this.radioButtonWrap.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.buttonOk, "buttonOk");
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// FormSelectLineWrap
			// 
			this.AcceptButton = this.buttonOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Name = "FormSelectLineWrap";
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