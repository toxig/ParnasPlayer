namespace PPlayer
{
    partial class Form_Update
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Update));
            this.label_info = new System.Windows.Forms.Label();
            this.memoEdit_info = new DevExpress.XtraEditors.MemoEdit();
            this.pbc_upload = new DevExpress.XtraEditors.ProgressBarControl();
            this.sbtn_cancel = new DevExpress.XtraEditors.SimpleButton();
            this.sbtn_update = new DevExpress.XtraEditors.SimpleButton();
            this.sbtn_Close = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit_info.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbc_upload.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // label_info
            // 
            this.label_info.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_info.Location = new System.Drawing.Point(10, 39);
            this.label_info.Name = "label_info";
            this.label_info.Size = new System.Drawing.Size(532, 28);
            this.label_info.TabIndex = 51;
            this.label_info.Text = "Проверка обновления";
            this.label_info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // memoEdit_info
            // 
            this.memoEdit_info.Location = new System.Drawing.Point(14, 70);
            this.memoEdit_info.Name = "memoEdit_info";
            this.memoEdit_info.Properties.ReadOnly = true;
            this.memoEdit_info.Size = new System.Drawing.Size(528, 182);
            this.memoEdit_info.TabIndex = 54;
            // 
            // pbc_upload
            // 
            this.pbc_upload.Location = new System.Drawing.Point(11, 12);
            this.pbc_upload.Name = "pbc_upload";
            this.pbc_upload.Size = new System.Drawing.Size(531, 24);
            this.pbc_upload.TabIndex = 55;
            // 
            // sbtn_cancel
            // 
            this.sbtn_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sbtn_cancel.Location = new System.Drawing.Point(367, 258);
            this.sbtn_cancel.Name = "sbtn_cancel";
            this.sbtn_cancel.Size = new System.Drawing.Size(175, 33);
            this.sbtn_cancel.TabIndex = 56;
            this.sbtn_cancel.Text = "Отмена";
            this.sbtn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // sbtn_update
            // 
            this.sbtn_update.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sbtn_update.Location = new System.Drawing.Point(14, 258);
            this.sbtn_update.Name = "sbtn_update";
            this.sbtn_update.Size = new System.Drawing.Size(187, 33);
            this.sbtn_update.TabIndex = 57;
            this.sbtn_update.Text = "Обновить";
            this.sbtn_update.Click += new System.EventHandler(this.btn_update_Click);
            // 
            // sbtn_Close
            // 
            this.sbtn_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sbtn_Close.Location = new System.Drawing.Point(208, 259);
            this.sbtn_Close.Name = "sbtn_Close";
            this.sbtn_Close.Size = new System.Drawing.Size(153, 32);
            this.sbtn_Close.TabIndex = 58;
            this.sbtn_Close.Text = "Закрыть";
            this.sbtn_Close.Visible = false;
            this.sbtn_Close.Click += new System.EventHandler(this.sbtn_Close_Click);
            // 
            // Form_Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 297);
            this.Controls.Add(this.sbtn_Close);
            this.Controls.Add(this.sbtn_cancel);
            this.Controls.Add(this.sbtn_update);
            this.Controls.Add(this.pbc_upload);
            this.Controls.Add(this.memoEdit_info);
            this.Controls.Add(this.label_info);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.LookAndFeel.SkinName = "Office 2010 Blue";
            this.Name = "Form_Update";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Обновление программы";
            this.Load += new System.EventHandler(this.Form_Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit_info.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbc_upload.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label_info;
        private DevExpress.XtraEditors.MemoEdit memoEdit_info;
        private DevExpress.XtraEditors.ProgressBarControl pbc_upload;
        private DevExpress.XtraEditors.SimpleButton sbtn_cancel;
        private DevExpress.XtraEditors.SimpleButton sbtn_update;
        private DevExpress.XtraEditors.SimpleButton sbtn_Close;
    }
}

