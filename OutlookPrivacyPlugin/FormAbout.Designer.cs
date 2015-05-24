namespace OutlookPrivacyPlugin
{
  internal partial class FormAbout
  {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
			this.AboutLabel = new System.Windows.Forms.Label();
			this.IconLabel = new System.Windows.Forms.LinkLabel();
			this.ClipboardLink = new System.Windows.Forms.LinkLabel();
			this.InitialByLabel = new System.Windows.Forms.Label();
			this.ForkLabel = new System.Windows.Forms.LinkLabel();
			this.BuildLabel = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// AboutLabel
			// 
			this.AboutLabel.AutoSize = true;
			this.AboutLabel.BackColor = System.Drawing.Color.White;
			this.AboutLabel.Font = new System.Drawing.Font("Verdana", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AboutLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(149)))), ((int)(((byte)(152)))));
			this.AboutLabel.Location = new System.Drawing.Point(10, 23);
			this.AboutLabel.Name = "AboutLabel";
			this.AboutLabel.Size = new System.Drawing.Size(278, 29);
			this.AboutLabel.TabIndex = 0;
			this.AboutLabel.Text = "Outlook Privacy Plugin";
			// 
			// IconLabel
			// 
			this.IconLabel.ActiveLinkColor = System.Drawing.Color.Black;
			this.IconLabel.AutoSize = true;
			this.IconLabel.BackColor = System.Drawing.Color.White;
			this.IconLabel.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.IconLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.IconLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(149)))), ((int)(((byte)(152)))));
			this.IconLabel.Location = new System.Drawing.Point(12, 242);
			this.IconLabel.Name = "IconLabel";
			this.IconLabel.Size = new System.Drawing.Size(190, 13);
			this.IconLabel.TabIndex = 3;
			this.IconLabel.TabStop = true;
			this.IconLabel.Text = "Silk Icon Set by Mark James";
			this.IconLabel.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(149)))), ((int)(((byte)(152)))));
			this.IconLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ClickLink);
			// 
			// ClipboardLink
			// 
			this.ClipboardLink.ActiveLinkColor = System.Drawing.Color.Black;
			this.ClipboardLink.AutoSize = true;
			this.ClipboardLink.BackColor = System.Drawing.Color.White;
			this.ClipboardLink.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ClipboardLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.ClipboardLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(149)))), ((int)(((byte)(152)))));
			this.ClipboardLink.Location = new System.Drawing.Point(12, 265);
			this.ClipboardLink.Name = "ClipboardLink";
			this.ClipboardLink.Size = new System.Drawing.Size(249, 13);
			this.ClipboardLink.TabIndex = 5;
			this.ClipboardLink.TabStop = true;
			this.ClipboardLink.Text = "Clipboard Wrapper by Alessio Deiana";
			this.ClipboardLink.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(149)))), ((int)(((byte)(152)))));
			this.ClipboardLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ClickLink);
			// 
			// InitialByLabel
			// 
			this.InitialByLabel.AutoSize = true;
			this.InitialByLabel.BackColor = System.Drawing.Color.White;
			this.InitialByLabel.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.InitialByLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(149)))), ((int)(((byte)(152)))));
			this.InitialByLabel.Location = new System.Drawing.Point(12, 217);
			this.InitialByLabel.Name = "InitialByLabel";
			this.InitialByLabel.Size = new System.Drawing.Size(207, 13);
			this.InitialByLabel.TabIndex = 7;
			this.InitialByLabel.Text = "Initial version by David Cumps";
			// 
			// ForkLabel
			// 
			this.ForkLabel.ActiveLinkColor = System.Drawing.Color.Black;
			this.ForkLabel.AutoSize = true;
			this.ForkLabel.BackColor = System.Drawing.Color.White;
			this.ForkLabel.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.ForkLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(149)))), ((int)(((byte)(152)))));
			this.ForkLabel.Location = new System.Drawing.Point(12, 70);
			this.ForkLabel.Name = "ForkLabel";
			this.ForkLabel.Size = new System.Drawing.Size(222, 13);
			this.ForkLabel.TabIndex = 8;
			this.ForkLabel.TabStop = true;
			this.ForkLabel.Text = "Fork version by Deja vu Security";
			this.ForkLabel.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(149)))), ((int)(((byte)(152)))));
			this.ForkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ClickLink);
			// 
			// BuildLabel
			// 
			this.BuildLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BuildLabel.BackColor = System.Drawing.Color.White;
			this.BuildLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.BuildLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(149)))), ((int)(((byte)(152)))));
			this.BuildLabel.Location = new System.Drawing.Point(15, 52);
			this.BuildLabel.Name = "BuildLabel";
			this.BuildLabel.Size = new System.Drawing.Size(278, 18);
			this.BuildLabel.TabIndex = 9;
			this.BuildLabel.Text = "-29-Sept-2010-";
			this.BuildLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.InitialImage = null;
			this.pictureBox1.Location = new System.Drawing.Point(15, 86);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(294, 128);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 10;
			this.pictureBox1.TabStop = false;
			// 
			// About
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(321, 292);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.BuildLabel);
			this.Controls.Add(this.ForkLabel);
			this.Controls.Add(this.InitialByLabel);
			this.Controls.Add(this.ClipboardLink);
			this.Controls.Add(this.IconLabel);
			this.Controls.Add(this.AboutLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "About";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

	private System.Windows.Forms.Label AboutLabel;
	private System.Windows.Forms.LinkLabel IconLabel;
	private System.Windows.Forms.LinkLabel ClipboardLink;
    private System.Windows.Forms.Label InitialByLabel;
    private System.Windows.Forms.LinkLabel ForkLabel;
    private System.Windows.Forms.Label BuildLabel;
	private System.Windows.Forms.PictureBox pictureBox1;

  }
}
