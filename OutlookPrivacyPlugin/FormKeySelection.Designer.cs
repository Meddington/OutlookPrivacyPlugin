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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "aaa",
            "2015-05-01",
            "AABBCCDD"}, -1);
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
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(528, 51);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Recipients not valid, not trusted or not found";
			// 
			// textBoxMsg
			// 
			this.textBoxMsg.Location = new System.Drawing.Point(6, 21);
			this.textBoxMsg.Name = "textBoxMsg";
			this.textBoxMsg.ReadOnly = true;
			this.textBoxMsg.Size = new System.Drawing.Size(515, 22);
			this.textBoxMsg.TabIndex = 0;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.listViewKeys);
			this.groupBox2.Location = new System.Drawing.Point(12, 69);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(528, 302);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Select Recipients for Encryption";
			// 
			// listViewKeys
			// 
			this.listViewKeys.CheckBoxes = true;
			this.listViewKeys.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			listViewItem1.StateImageIndex = 0;
			this.listViewKeys.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
			this.listViewKeys.Location = new System.Drawing.Point(6, 21);
			this.listViewKeys.Name = "listViewKeys";
			this.listViewKeys.Size = new System.Drawing.Size(515, 271);
			this.listViewKeys.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewKeys.TabIndex = 0;
			this.listViewKeys.UseCompatibleStateImageBehavior = false;
			this.listViewKeys.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Account / User ID";
			this.columnHeader1.Width = 300;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Expiry";
			this.columnHeader2.Width = 100;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Key ID";
			this.columnHeader3.Width = 100;
			// 
			// checkBoxSendEncrypted
			// 
			this.checkBoxSendEncrypted.AutoSize = true;
			this.checkBoxSendEncrypted.Location = new System.Drawing.Point(18, 388);
			this.checkBoxSendEncrypted.Name = "checkBoxSendEncrypted";
			this.checkBoxSendEncrypted.Size = new System.Drawing.Size(130, 21);
			this.checkBoxSendEncrypted.TabIndex = 2;
			this.checkBoxSendEncrypted.Text = "Send encrypted";
			this.checkBoxSendEncrypted.UseVisualStyleBackColor = true;
			// 
			// checkBoxSendSigned
			// 
			this.checkBoxSendSigned.AutoSize = true;
			this.checkBoxSendSigned.Location = new System.Drawing.Point(18, 416);
			this.checkBoxSendSigned.Name = "checkBoxSendSigned";
			this.checkBoxSendSigned.Size = new System.Drawing.Size(109, 21);
			this.checkBoxSendSigned.TabIndex = 3;
			this.checkBoxSendSigned.Text = "Send signed";
			this.checkBoxSendSigned.UseVisualStyleBackColor = true;
			// 
			// buttonRefreshKey
			// 
			this.buttonRefreshKey.Location = new System.Drawing.Point(18, 444);
			this.buttonRefreshKey.Name = "buttonRefreshKey";
			this.buttonRefreshKey.Size = new System.Drawing.Size(138, 28);
			this.buttonRefreshKey.TabIndex = 4;
			this.buttonRefreshKey.Text = "Refresh Key List";
			this.buttonRefreshKey.UseVisualStyleBackColor = true;
			this.buttonRefreshKey.Click += new System.EventHandler(this.buttonRefreshKey_Click);
			// 
			// buttonSend
			// 
			this.buttonSend.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonSend.Location = new System.Drawing.Point(384, 470);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(75, 31);
			this.buttonSend.TabIndex = 5;
			this.buttonSend.Text = "Send";
			this.buttonSend.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(465, 470);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 31);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// FormKeySelection
			// 
			this.AcceptButton = this.buttonSend;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(552, 514);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonSend);
			this.Controls.Add(this.buttonRefreshKey);
			this.Controls.Add(this.checkBoxSendSigned);
			this.Controls.Add(this.checkBoxSendEncrypted);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "FormKeySelection";
			this.Text = "Key Selection";
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