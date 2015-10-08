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
            this.components = new System.ComponentModel.Container();
            this.linkMailTo = new System.Windows.Forms.LinkLabel();
            this.lbAbout = new DevExpress.XtraEditors.LabelControl();
            this.lbCopyright = new DevExpress.XtraEditors.LabelControl();
            this.lbVersion = new DevExpress.XtraEditors.LabelControl();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // linkMailTo
            // 
            this.linkMailTo.BackColor = System.Drawing.Color.Transparent;
            this.linkMailTo.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.linkMailTo.ForeColor = System.Drawing.Color.Transparent;
            this.linkMailTo.LinkColor = System.Drawing.Color.SkyBlue;
            this.linkMailTo.Location = new System.Drawing.Point(14, 169);
            this.linkMailTo.Name = "linkMailTo";
            this.linkMailTo.Size = new System.Drawing.Size(306, 27);
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
            this.lbAbout.Location = new System.Drawing.Point(14, 124);
            this.lbAbout.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lbAbout.Name = "lbAbout";
            this.lbAbout.Size = new System.Drawing.Size(306, 44);
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
            this.lbCopyright.Location = new System.Drawing.Point(12, 199);
            this.lbCopyright.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lbCopyright.Name = "lbCopyright";
            this.lbCopyright.Size = new System.Drawing.Size(306, 28);
            this.lbCopyright.TabIndex = 15;
            this.lbCopyright.Text = "© Копирайт 2012 Черников А.С.";
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
            this.lbVersion.Location = new System.Drawing.Point(5, 10);
            this.lbVersion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(106, 52);
            this.lbVersion.TabIndex = 16;
            this.lbVersion.Text = "Версия";
            this.lbVersion.Click += new System.EventHandler(this.AboutForm_Close);
            // 
            // Form_About
            // 
            this.Appearance.BackColor = System.Drawing.Color.DimGray;
            this.Appearance.Options.UseBackColor = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Stretch;
            this.BackgroundImageStore = global::PPlayer.Properties.Resources.About;
            this.ClientSize = new System.Drawing.Size(326, 233);
            this.Controls.Add(this.lbVersion);
            this.Controls.Add(this.lbCopyright);
            this.Controls.Add(this.lbAbout);
            this.Controls.Add(this.linkMailTo);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form_About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "О программе";
            this.Click += new System.EventHandler(this.AboutForm_Close);
            this.Leave += new System.EventHandler(this.AboutForm_Close);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkMailTo;
        private DevExpress.XtraEditors.LabelControl lbAbout;
        private DevExpress.XtraEditors.LabelControl lbCopyright;
        private DevExpress.XtraEditors.LabelControl lbVersion;
        private System.Windows.Forms.ToolTip toolTip;

    }
}