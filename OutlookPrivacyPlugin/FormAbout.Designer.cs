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
			this.AboutLabel.Location = new System.Drawing.Point(13, 28);
			this.AboutLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.AboutLabel.Name = "AboutLabel";
			this.AboutLabel.Size = new System.Drawing.Size(357, 36);
			this.AboutLabel.TabIndex = 0;
			this.AboutLabel.Text = "Outlook Privacy Plugin";
			// 
			// InitialByLabel
			// 
			this.InitialByLabel.AutoSize = true;
			this.InitialByLabel.BackColor = System.Drawing.Color.White;
			this.InitialByLabel.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.InitialByLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(147)))), ((int)(((byte)(149)))), ((int)(((byte)(152)))));
			this.InitialByLabel.Location = new System.Drawing.Point(16, 267);
			this.InitialByLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.InitialByLabel.Name = "InitialByLabel";
			this.InitialByLabel.Size = new System.Drawing.Size(245, 17);
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
			this.ForkLabel.Location = new System.Drawing.Point(16, 86);
			this.ForkLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.ForkLabel.Name = "ForkLabel";
			this.ForkLabel.Size = new System.Drawing.Size(261, 17);
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
			this.BuildLabel.Location = new System.Drawing.Point(20, 64);
			this.BuildLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.BuildLabel.Name = "BuildLabel";
			this.BuildLabel.Size = new System.Drawing.Size(371, 22);
			this.BuildLabel.TabIndex = 9;
			this.BuildLabel.Text = "-29-Sept-2010-";
			this.BuildLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.InitialImage = null;
			this.pictureBox1.Location = new System.Drawing.Point(20, 106);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(392, 158);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 10;
			this.pictureBox1.TabStop = false;
			// 
			// FormAbout
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(428, 308);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.BuildLabel);
			this.Controls.Add(this.ForkLabel);
			this.Controls.Add(this.InitialByLabel);
			this.Controls.Add(this.AboutLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAbout";
			this.Padding = new System.Windows.Forms.Padding(12, 11, 12, 11);
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
    private System.Windows.Forms.Label InitialByLabel;
    private System.Windows.Forms.LinkLabel ForkLabel;
    private System.Windows.Forms.Label BuildLabel;
	private System.Windows.Forms.PictureBox pictureBox1;

  }
}
