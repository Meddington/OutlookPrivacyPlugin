namespace OutlookPrivacyPlugin
{
	partial class FormKeySelection
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormKeySelection));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBoxMsg = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.listViewKeys = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.checkBoxSendEncrypted = new System.Windows.Forms.CheckBox();
			this.checkBoxSendSigned = new System.Windows.Forms.CheckBox();
			this.buttonRefreshKey = new System.Windows.Forms.Button();
			this.buttonSend = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textBoxMsg);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// textBoxMsg
			// 
			resources.ApplyResources(this.textBoxMsg, "textBoxMsg");
			this.textBoxMsg.Name = "textBoxMsg";
			this.textBoxMsg.ReadOnly = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.listViewKeys);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// listViewKeys
			// 
			this.listViewKeys.CheckBoxes = true;
			this.listViewKeys.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			this.listViewKeys.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listViewKeys.Items")))});
			resources.ApplyResources(this.listViewKeys, "listViewKeys");
			this.listViewKeys.Name = "listViewKeys";
			this.listViewKeys.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewKeys.UseCompatibleStateImageBehavior = false;
			this.listViewKeys.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			resources.ApplyResources(this.columnHeader1, "columnHeader1");
			// 
			// columnHeader2
			// 
			resources.ApplyResources(this.columnHeader2, "columnHeader2");
			// 
			// columnHeader3
			// 
			resources.ApplyResources(this.columnHeader3, "columnHeader3");
			// 
			// checkBoxSendEncrypted
			// 
			resources.ApplyResources(this.checkBoxSendEncrypted, "checkBoxSendEncrypted");
			this.checkBoxSendEncrypted.Name = "checkBoxSendEncrypted";
			this.checkBoxSendEncrypted.UseVisualStyleBackColor = true;
			// 
			// checkBoxSendSigned
			// 
			resources.ApplyResources(this.checkBoxSendSigned, "checkBoxSendSigned");
			this.checkBoxSendSigned.Name = "checkBoxSendSigned";
			this.checkBoxSendSigned.UseVisualStyleBackColor = true;
			// 
			// buttonRefreshKey
			// 
			resources.ApplyResources(this.buttonRefreshKey, "buttonRefreshKey");
			this.buttonRefreshKey.Name = "buttonRefreshKey";
			this.buttonRefreshKey.UseVisualStyleBackColor = true;
			this.buttonRefreshKey.Click += new System.EventHandler(this.buttonRefreshKey_Click);
			// 
			// buttonSend
			// 
			this.buttonSend.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.buttonSend, "buttonSend");
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// FormKeySelection
			// 
			this.AcceptButton = this.buttonSend;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonSend);
			this.Controls.Add(this.buttonRefreshKey);
			this.Controls.Add(this.checkBoxSendSigned);
			this.Controls.Add(this.checkBoxSendEncrypted);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "FormKeySelection";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxMsg;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ListView listViewKeys;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.CheckBox checkBoxSendEncrypted;
		private System.Windows.Forms.CheckBox checkBoxSendSigned;
		private System.Windows.Forms.Button buttonRefreshKey;
		private System.Windows.Forms.Button buttonSend;
		private System.Windows.Forms.Button buttonCancel;
	}
}