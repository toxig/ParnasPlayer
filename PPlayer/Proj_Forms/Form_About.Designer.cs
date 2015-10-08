namespace PPlayer
{
    partial class Form_About
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
            this.linkMailTo = new System.Windows.Forms.LinkLabel();
            this.lbAbout = new DevExpress.XtraEditors.LabelControl();
            this.lbCopyright = new DevExpress.XtraEditors.LabelControl();
            this.lbVersion = new DevExpress.XtraEditors.LabelControl();
            this.toolTip = new System.Windows.Forms.ToolTip();
            this.pictureEdit_close = new DevExpress.XtraEditors.PictureEdit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit_close.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // linkMailTo
            // 
            this.linkMailTo.BackColor = System.Drawing.Color.Transparent;
            this.linkMailTo.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkMailTo.ForeColor = System.Drawing.Color.Transparent;
            this.linkMailTo.LinkColor = System.Drawing.Color.SkyBlue;
            this.linkMailTo.Location = new System.Drawing.Point(12, 137);
            this.linkMailTo.Name = "linkMailTo";
            this.linkMailTo.Size = new System.Drawing.Size(262, 22);
            this.linkMailTo.TabIndex = 13;
            this.linkMailTo.TabStop = true;
            this.linkMailTo.Tag = "chernikov.a.s@yandex.ru";
            this.linkMailTo.Text = "Письмо разработчику";
            this.linkMailTo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkMailTo.VisitedLinkColor = System.Drawing.Color.RoyalBlue;
            this.linkMailTo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMailTo_LinkClicked);
            this.linkMailTo.MouseLeave += new System.EventHandler(this.linkMailTo_MouseLeave);
            this.linkMailTo.MouseHover += new System.EventHandler(this.linkMailTo_MouseHover);
            this.linkMailTo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.linkMailTo_MouseMove);
            // 
            // lbAbout
            // 
            this.lbAbout.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbAbout.Appearance.ForeColor = System.Drawing.Color.White;
            this.lbAbout.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lbAbout.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lbAbout.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lbAbout.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lbAbout.Location = new System.Drawing.Point(12, 101);
            this.lbAbout.Name = "lbAbout";
            this.lbAbout.Size = new System.Drawing.Size(262, 36);
            this.lbAbout.TabIndex = 14;
            this.lbAbout.Text = "Название программы";
            this.lbAbout.Click += new System.EventHandler(this.AboutForm_Close);
            // 
            // lbCopyright
            // 
            this.lbCopyright.Appearance.Font = new System.Drawing.Font("Tahoma", 7F);
            this.lbCopyright.Appearance.ForeColor = System.Drawing.Color.White;
            this.lbCopyright.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lbCopyright.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lbCopyright.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lbCopyright.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lbCopyright.Location = new System.Drawing.Point(10, 162);
            this.lbCopyright.Name = "lbCopyright";
            this.lbCopyright.Size = new System.Drawing.Size(262, 23);
            this.lbCopyright.TabIndex = 15;
            this.lbCopyright.Text = "© Копирайт 2014 Черников А.С.";
            this.lbCopyright.Click += new System.EventHandler(this.AboutForm_Close);
            // 
            // lbVersion
            // 
            this.lbVersion.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbVersion.Appearance.ForeColor = System.Drawing.Color.White;
            this.lbVersion.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lbVersion.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.lbVersion.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lbVersion.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lbVersion.Location = new System.Drawing.Point(4, 8);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(91, 42);
            this.lbVersion.TabIndex = 16;
            this.lbVersion.Text = "Версия";
            this.lbVersion.Click += new System.EventHandler(this.AboutForm_Close);
            // 
            // pictureEdit_close
            // 
            this.pictureEdit_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureEdit_close.EditValue = global::PPlayer.Properties.Resources.close2;
            this.pictureEdit_close.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.pictureEdit_close.Location = new System.Drawing.Point(254, 5);
            this.pictureEdit_close.Name = "pictureEdit_close";
            this.pictureEdit_close.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.pictureEdit_close.Properties.Appearance.Options.UseBackColor = true;
            this.pictureEdit_close.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pictureEdit_close.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
            this.pictureEdit_close.Size = new System.Drawing.Size(20, 20);
            this.pictureEdit_close.TabIndex = 18;
            this.pictureEdit_close.Click += new System.EventHandler(this.AboutForm_Close);
            // 
            // Form_About
            // 
            this.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Stretch;
            this.BackgroundImageStore = global::PPlayer.Properties.Resources.About;
            this.ClientSize = new System.Drawing.Size(279, 189);
            this.Controls.Add(this.pictureEdit_close);
            this.Controls.Add(this.lbVersion);
            this.Controls.Add(this.lbCopyright);
            this.Controls.Add(this.lbAbout);
            this.Controls.Add(this.linkMailTo);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form_About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "О программе";
            this.Click += new System.EventHandler(this.AboutForm_Close);
            this.Leave += new System.EventHandler(this.AboutForm_Close);
            ((System.ComponentModel.ISupportInitialize)(this.pictureEdit_close.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkMailTo;
        private DevExpress.XtraEditors.LabelControl lbAbout;
        private DevExpress.XtraEditors.LabelControl lbCopyright;
        private DevExpress.XtraEditors.LabelControl lbVersion;
        private System.Windows.Forms.ToolTip toolTip;
        private DevExpress.XtraEditors.PictureEdit pictureEdit_close;

    }
}