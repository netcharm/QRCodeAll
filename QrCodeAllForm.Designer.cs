namespace QrCodeAny
{
  partial class QrCodeAllForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing ) {
      if( disposing && ( components != null ) ) {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
            this.picUser = new System.Windows.Forms.PictureBox();
            this.picPass = new System.Windows.Forms.PictureBox();
            this.picNote = new System.Windows.Forms.PictureBox();
            this.picUrl = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUrl)).BeginInit();
            this.SuspendLayout();
            // 
            // picUser
            // 
            this.picUser.Location = new System.Drawing.Point(8, 8);
            this.picUser.Name = "picUser";
            this.picUser.Size = new System.Drawing.Size(256, 256);
            this.picUser.TabIndex = 0;
            this.picUser.TabStop = false;
            // 
            // picPass
            // 
            this.picPass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picPass.Location = new System.Drawing.Point(328, 8);
            this.picPass.Name = "picPass";
            this.picPass.Size = new System.Drawing.Size(256, 256);
            this.picPass.TabIndex = 1;
            this.picPass.TabStop = false;
            // 
            // picNote
            // 
            this.picNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picNote.Location = new System.Drawing.Point(328, 310);
            this.picNote.Name = "picNote";
            this.picNote.Size = new System.Drawing.Size(256, 256);
            this.picNote.TabIndex = 3;
            this.picNote.TabStop = false;
            // 
            // picUrl
            // 
            this.picUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picUrl.Location = new System.Drawing.Point(8, 310);
            this.picUrl.Name = "picUrl";
            this.picUrl.Size = new System.Drawing.Size(256, 256);
            this.picUrl.TabIndex = 2;
            this.picUrl.TabStop = false;
            // 
            // QrCodeAllForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(594, 575);
            this.Controls.Add(this.picNote);
            this.Controls.Add(this.picUrl);
            this.Controls.Add(this.picPass);
            this.Controls.Add(this.picUser);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QrCodeAllForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "QrCodeForm";
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUrl)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    public System.Windows.Forms.PictureBox picUser;
    public System.Windows.Forms.PictureBox picPass;
    public System.Windows.Forms.PictureBox picNote;
    public System.Windows.Forms.PictureBox picUrl;


  }
}