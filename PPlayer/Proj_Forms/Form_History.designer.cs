namespace PPlayer
{
    partial class Form_History
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
            this.label_info = new System.Windows.Forms.Label();
            this.memoEdit_info = new DevExpress.XtraEditors.MemoEdit();
            this.sbtn_cancel = new DevExpress.XtraEditors.SimpleButton();
            this.sbtn_save = new DevExpress.XtraEditors.SimpleButton();
            this.sbtn_no_save = new DevExpress.XtraEditors.SimpleButton();
            this.sbtn_save_as = new DevExpress.XtraEditors.SimpleButton();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit_info.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // label_info
            // 
            this.label_info.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_info.Location = new System.Drawing.Point(12, 4);
            this.label_info.Name = "label_info";
            this.label_info.Size = new System.Drawing.Size(456, 23);
            this.label_info.TabIndex = 51;
            this.label_info.Text = "Плейлист изменен!";
            this.label_info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // memoEdit_info
            // 
            this.memoEdit_info.Cursor = System.Windows.Forms.Cursors.Default;
            this.memoEdit_info.EditValue = "";
            this.memoEdit_info.Location = new System.Drawing.Point(12, 32);
            this.memoEdit_info.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.memoEdit_info.Name = "memoEdit_info";
            this.memoEdit_info.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.memoEdit_info.Properties.Appearance.Options.UseFont = true;
            this.memoEdit_info.Properties.ReadOnly = true;
            this.memoEdit_info.Size = new System.Drawing.Size(453, 173);
            this.memoEdit_info.TabIndex = 54;
            // 
            // sbtn_cancel
            // 
            this.sbtn_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sbtn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.sbtn_cancel.Location = new System.Drawing.Point(369, 209);
            this.sbtn_cancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.sbtn_cancel.Name = "sbtn_cancel";
            this.sbtn_cancel.Size = new System.Drawing.Size(94, 27);
            this.sbtn_cancel.TabIndex = 56;
            this.sbtn_cancel.Text = "Назад";
            this.sbtn_cancel.ToolTip = "Вернуться в программу";
            this.sbtn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // sbtn_save
            // 
            this.sbtn_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sbtn_save.Location = new System.Drawing.Point(12, 209);
            this.sbtn_save.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.sbtn_save.Name = "sbtn_save";
            this.sbtn_save.Size = new System.Drawing.Size(101, 27);
            this.sbtn_save.TabIndex = 57;
            this.sbtn_save.Text = "Сохранить";
            this.sbtn_save.Click += new System.EventHandler(this.sbtn_save_Click);
            // 
            // sbtn_no_save
            // 
            this.sbtn_no_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sbtn_no_save.Location = new System.Drawing.Point(232, 209);
            this.sbtn_no_save.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.sbtn_no_save.Name = "sbtn_no_save";
            this.sbtn_no_save.Size = new System.Drawing.Size(131, 27);
            this.sbtn_no_save.TabIndex = 58;
            this.sbtn_no_save.Text = "Не сохранять!";
            this.sbtn_no_save.ToolTip = "Закрыть, не сохраняя изменения.";
            this.sbtn_no_save.Click += new System.EventHandler(this.sbtn_No_Save_Click);
            // 
            // sbtn_save_as
            // 
            this.sbtn_save_as.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sbtn_save_as.Location = new System.Drawing.Point(119, 209);
            this.sbtn_save_as.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.sbtn_save_as.Name = "sbtn_save_as";
            this.sbtn_save_as.Size = new System.Drawing.Size(107, 27);
            this.sbtn_save_as.TabIndex = 59;
            this.sbtn_save_as.Text = "Сохранить как...";
            this.sbtn_save_as.Click += new System.EventHandler(this.sbtn_save_as_Click);
            // 
            // Form_History
            // 
            this.AcceptButton = this.sbtn_save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.sbtn_cancel;
            this.ClientSize = new System.Drawing.Size(475, 241);
            this.Controls.Add(this.sbtn_save_as);
            this.Controls.Add(this.sbtn_no_save);
            this.Controls.Add(this.sbtn_cancel);
            this.Controls.Add(this.sbtn_save);
            this.Controls.Add(this.memoEdit_info);
            this.Controls.Add(this.label_info);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.LookAndFeel.SkinName = "Office 2010 Blue";
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_History";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Плейлист изменен (история)";
            this.Load += new System.EventHandler(this.Form_History_Load);
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit_info.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label_info;
        private DevExpress.XtraEditors.MemoEdit memoEdit_info;
        private DevExpress.XtraEditors.SimpleButton sbtn_cancel;
        private DevExpress.XtraEditors.SimpleButton sbtn_save;
        private DevExpress.XtraEditors.SimpleButton sbtn_no_save;
        private DevExpress.XtraEditors.SimpleButton sbtn_save_as;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

