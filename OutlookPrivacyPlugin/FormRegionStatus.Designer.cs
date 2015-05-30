namespace OutlookPrivacyPlugin
{
	[System.ComponentModel.ToolboxItemAttribute(false)]
	partial class FormRegionStatus : Microsoft.Office.Tools.Outlook.FormRegionBase
	{
		public FormRegionStatus(Microsoft.Office.Interop.Outlook.FormRegion formRegion)
			: base(Globals.Factory, formRegion)
		{
			this.InitializeComponent();
		}

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(118, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "                      ";
			// 
			// FormRegionStatus
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.label1);
			this.Name = "FormRegionStatus";
			this.Size = new System.Drawing.Size(337, 21);
			this.FormRegionShowing += new System.EventHandler(this.FormRegionStatus_FormRegionShowing);
			this.FormRegionClosed += new System.EventHandler(this.FormRegionStatus_FormRegionClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		#region Form Region Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private static void InitializeManifest(Microsoft.Office.Tools.Outlook.FormRegionManifest manifest, Microsoft.Office.Tools.Outlook.Factory factory)
		{
			manifest.FormRegionName = "Outlook Privacy Plugin";
			manifest.FormRegionType = Microsoft.Office.Tools.Outlook.FormRegionType.Adjoining;
			manifest.ShowInspectorCompose = false;

		}

		#endregion

		private System.Windows.Forms.Label label1;

		public partial class FormRegionStatusFactory : Microsoft.Office.Tools.Outlook.IFormRegionFactory
		{
			public event Microsoft.Office.Tools.Outlook.FormRegionInitializingEventHandler FormRegionInitializing;

			private Microsoft.Office.Tools.Outlook.FormRegionManifest _Manifest;

			[System.Diagnostics.DebuggerNonUserCodeAttribute()]
			public FormRegionStatusFactory()
			{
				this._Manifest = Globals.Factory.CreateFormRegionManifest();
				FormRegionStatus.InitializeManifest(this._Manifest, Globals.Factory);
				this.FormRegionInitializing += new Microsoft.Office.Tools.Outlook.FormRegionInitializingEventHandler(this.FormRegionStatusFactory_FormRegionInitializing);
			}

			[System.Diagnostics.DebuggerNonUserCodeAttribute()]
			public Microsoft.Office.Tools.Outlook.FormRegionManifest Manifest
			{
				get
				{
					return this._Manifest;
				}
			}

			[System.Diagnostics.DebuggerNonUserCodeAttribute()]
			Microsoft.Office.Tools.Outlook.IFormRegion Microsoft.Office.Tools.Outlook.IFormRegionFactory.CreateFormRegion(Microsoft.Office.Interop.Outlook.FormRegion formRegion)
			{
				FormRegionStatus form = new FormRegionStatus(formRegion);
				form.Factory = this;
				return form;
			}

			[System.Diagnostics.DebuggerNonUserCodeAttribute()]
			byte[] Microsoft.Office.Tools.Outlook.IFormRegionFactory.GetFormRegionStorage(object outlookItem, Microsoft.Office.Interop.Outlook.OlFormRegionMode formRegionMode, Microsoft.Office.Interop.Outlook.OlFormRegionSize formRegionSize)
			{
				throw new System.NotSupportedException();
			}

			[System.Diagnostics.DebuggerNonUserCodeAttribute()]
			bool Microsoft.Office.Tools.Outlook.IFormRegionFactory.IsDisplayedForItem(object outlookItem, Microsoft.Office.Interop.Outlook.OlFormRegionMode formRegionMode, Microsoft.Office.Interop.Outlook.OlFormRegionSize formRegionSize)
			{
				if (this.FormRegionInitializing != null)
				{
					Microsoft.Office.Tools.Outlook.FormRegionInitializingEventArgs cancelArgs = Globals.Factory.CreateFormRegionInitializingEventArgs(outlookItem, formRegionMode, formRegionSize, false);
					this.FormRegionInitializing(this, cancelArgs);
					return !cancelArgs.Cancel;
				}
				else
				{
					return true;
				}
			}

			[System.Diagnostics.DebuggerNonUserCodeAttribute()]
			Microsoft.Office.Tools.Outlook.FormRegionKindConstants Microsoft.Office.Tools.Outlook.IFormRegionFactory.Kind
			{
				get
				{
					return Microsoft.Office.Tools.Outlook.FormRegionKindConstants.WindowsForms;
				}
			}
		}
	}

	partial class WindowFormRegionCollection
	{
		internal FormRegionStatus FormRegionStatus
		{
			get
			{
				foreach (var item in this)
				{
					if (item.GetType() == typeof(FormRegionStatus))
						return (FormRegionStatus)item;
				}
				return null;
			}
		}
	}
}
