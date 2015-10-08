namespace PPlayer
{
    partial class Form_TagEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_TagEditor));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.memoEdit_Comments = new DevExpress.XtraEditors.MemoEdit();
            this.TE_Artist = new DevExpress.XtraEditors.TextEdit();
            this.TE_Name = new DevExpress.XtraEditors.TextEdit();
            this.TE_FilePath = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.CheckList_TagsGrops = new DevExpress.XtraEditors.CheckedListBoxControl();
            this.Save_Changes = new DevExpress.XtraEditors.SimpleButton();
            this.Cancel_Changes = new DevExpress.XtraEditors.SimpleButton();
            this.Close_form = new DevExpress.XtraEditors.SimpleButton();
            this.Clear_Tags = new DevExpress.XtraEditors.SimpleButton();
            this.Select_Tags = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit_Comments.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TE_Artist.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TE_Name.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TE_FilePath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckList_TagsGrops)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.memoEdit_Comments);
            this.layoutControl1.Controls.Add(this.TE_Artist);
            this.layoutControl1.Controls.Add(this.TE_Name);
            this.layoutControl1.Controls.Add(this.TE_FilePath);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(726, 245);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // memoEdit_Comments
            // 
            this.memoEdit_Comments.Location = new System.Drawing.Point(133, 108);
            this.memoEdit_Comments.Name = "memoEdit_Comments";
            this.memoEdit_Comments.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.memoEdit_Comments.Properties.Appearance.Options.UseFont = true;
            this.memoEdit_Comments.Properties.ReadOnly = true;
            this.memoEdit_Comments.Size = new System.Drawing.Size(581, 125);
            this.memoEdit_Comments.StyleController = this.layoutControl1;
            this.memoEdit_Comments.TabIndex = 7;
            this.memoEdit_Comments.EditValueChanged += new System.EventHandler(this.TE_EditValueChanged);
            // 
            // TE_Artist
            // 
            this.TE_Artist.Location = new System.Drawing.Point(133, 44);
            this.TE_Artist.Name = "TE_Artist";
            this.TE_Artist.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TE_Artist.Properties.Appearance.Options.UseFont = true;
            this.TE_Artist.Properties.ReadOnly = true;
            this.TE_Artist.Size = new System.Drawing.Size(581, 28);
            this.TE_Artist.StyleController = this.layoutControl1;
            this.TE_Artist.TabIndex = 6;
            this.TE_Artist.EditValueChanged += new System.EventHandler(this.TE_EditValueChanged);
            // 
            // TE_Name
            // 
            this.TE_Name.Location = new System.Drawing.Point(133, 76);
            this.TE_Name.Name = "TE_Name";
            this.TE_Name.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TE_Name.Properties.Appearance.Options.UseFont = true;
            this.TE_Name.Properties.ReadOnly = true;
            this.TE_Name.Size = new System.Drawing.Size(581, 28);
            this.TE_Name.StyleController = this.layoutControl1;
            this.TE_Name.TabIndex = 5;
            this.TE_Name.EditValueChanged += new System.EventHandler(this.TE_EditValueChanged);
            // 
            // TE_FilePath
            // 
            this.TE_FilePath.Location = new System.Drawing.Point(133, 12);
            this.TE_FilePath.Name = "TE_FilePath";
            this.TE_FilePath.Properties.AllowFocused = false;
            this.TE_FilePath.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TE_FilePath.Properties.Appearance.Options.UseFont = true;
            this.TE_FilePath.Properties.ReadOnly = true;
            this.TE_FilePath.Size = new System.Drawing.Size(581, 28);
            this.TE_FilePath.StyleController = this.layoutControl1;
            this.TE_FilePath.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.layoutControlItem4});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(726, 245);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem2.Control = this.TE_Name;
            this.layoutControlItem2.CustomizationFormText = "Название";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 64);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(706, 32);
            this.layoutControlItem2.Text = "Название";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(117, 21);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.Control = this.TE_FilePath;
            this.layoutControlItem1.CustomizationFormText = "Файл:";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(706, 32);
            this.layoutControlItem1.Text = "Файл:";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(117, 21);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.layoutControlItem3.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem3.Control = this.TE_Artist;
            this.layoutControlItem3.CustomizationFormText = "Исполнитель";
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 32);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(706, 32);
            this.layoutControlItem3.Text = "Исполнитель";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(117, 21);
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.layoutControlItem4.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem4.AppearanceItemCaption.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.layoutControlItem4.Control = this.memoEdit_Comments;
            this.layoutControlItem4.CustomizationFormText = "Информация";
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 96);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(706, 129);
            this.layoutControlItem4.Text = "Информация";
            this.layoutControlItem4.TextSize = new System.Drawing.Size(117, 21);
            // 
            // CheckList_TagsGrops
            // 
            this.CheckList_TagsGrops.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.CheckList_TagsGrops.Appearance.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CheckList_TagsGrops.Appearance.Options.UseFont = true;
            this.CheckList_TagsGrops.CheckOnClick = true;
            this.CheckList_TagsGrops.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[] {
            new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Быстрые"),
            new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Медленные"),
            new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Медленные"),
            new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Медленные"),
            new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Медленные"),
            new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Медленные"),
            new DevExpress.XtraEditors.Controls.CheckedListBoxItem("Очень медленные")});
            this.CheckList_TagsGrops.Location = new System.Drawing.Point(12, 251);
            this.CheckList_TagsGrops.MultiColumn = true;
            this.CheckList_TagsGrops.Name = "CheckList_TagsGrops";
            this.CheckList_TagsGrops.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.CheckList_TagsGrops.Size = new System.Drawing.Size(704, 141);
            this.CheckList_TagsGrops.TabIndex = 1;
            this.CheckList_TagsGrops.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.CheckList_TagsGrops_ItemCheck);
            // 
            // Save_Changes
            // 
            this.Save_Changes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Save_Changes.Appearance.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Save_Changes.Appearance.Options.UseFont = true;
            this.Save_Changes.Location = new System.Drawing.Point(597, 431);
            this.Save_Changes.Name = "Save_Changes";
            this.Save_Changes.Size = new System.Drawing.Size(119, 39);
            this.Save_Changes.TabIndex = 2;
            this.Save_Changes.Text = "Сохранить";
            this.Save_Changes.Click += new System.EventHandler(this.Save_Tags);
            // 
            // Cancel_Changes
            // 
            this.Cancel_Changes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel_Changes.Appearance.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Cancel_Changes.Appearance.Options.UseFont = true;
            this.Cancel_Changes.Location = new System.Drawing.Point(481, 431);
            this.Cancel_Changes.Name = "Cancel_Changes";
            this.Cancel_Changes.Size = new System.Drawing.Size(110, 39);
            this.Cancel_Changes.TabIndex = 3;
            this.Cancel_Changes.Text = "Отмена";
            this.Cancel_Changes.Click += new System.EventHandler(this.Cancel_Changes_Click);
            // 
            // Close_form
            // 
            this.Close_form.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Close_form.Appearance.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Close_form.Appearance.Options.UseFont = true;
            this.Close_form.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close_form.Location = new System.Drawing.Point(13, 431);
            this.Close_form.Name = "Close_form";
            this.Close_form.Size = new System.Drawing.Size(462, 39);
            this.Close_form.TabIndex = 4;
            this.Close_form.Text = "Закрыть";
            this.Close_form.Click += new System.EventHandler(this.Form_Close);
            // 
            // Clear_Tags
            // 
            this.Clear_Tags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Clear_Tags.Location = new System.Drawing.Point(13, 400);
            this.Clear_Tags.Name = "Clear_Tags";
            this.Clear_Tags.Size = new System.Drawing.Size(462, 23);
            this.Clear_Tags.TabIndex = 5;
            this.Clear_Tags.Text = "очистить";
            this.Clear_Tags.Click += new System.EventHandler(this.Clear_All);
            // 
            // Select_Tags
            // 
            this.Select_Tags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Select_Tags.Location = new System.Drawing.Point(481, 400);
            this.Select_Tags.Name = "Select_Tags";
            this.Select_Tags.Size = new System.Drawing.Size(233, 23);
            this.Select_Tags.TabIndex = 6;
            this.Select_Tags.Text = "выбрать все";
            this.Select_Tags.Click += new System.EventHandler(this.Select_All);
            // 
            // Form_TagEditor
            // 
            this.AcceptButton = this.Save_Changes;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.Close_form;
            this.ClientSize = new System.Drawing.Size(726, 480);
            this.Controls.Add(this.Select_Tags);
            this.Controls.Add(this.Clear_Tags);
            this.Controls.Add(this.Close_form);
            this.Controls.Add(this.Cancel_Changes);
            this.Controls.Add(this.Save_Changes);
            this.Controls.Add(this.CheckList_TagsGrops);
            this.Controls.Add(this.layoutControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "Form_TagEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Редактор Тэгов";
            this.Load += new System.EventHandler(this.Form_TagEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit_Comments.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TE_Artist.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TE_Name.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TE_FilePath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CheckList_TagsGrops)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.MemoEdit memoEdit_Comments;
        private DevExpress.XtraEditors.TextEdit TE_Artist;
        private DevExpress.XtraEditors.TextEdit TE_Name;
        private DevExpress.XtraEditors.TextEdit TE_FilePath;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraEditors.CheckedListBoxControl CheckList_TagsGrops;
        private DevExpress.XtraEditors.SimpleButton Save_Changes;
        private DevExpress.XtraEditors.SimpleButton Cancel_Changes;
        private DevExpress.XtraEditors.SimpleButton Close_form;
        private DevExpress.XtraEditors.SimpleButton Clear_Tags;
        private DevExpress.XtraEditors.SimpleButton Select_Tags;
    }
}