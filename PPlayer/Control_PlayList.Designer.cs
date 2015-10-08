namespace PPlayer
{
    partial class Control_PlayList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Control_PlayList));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            this.grid_PlayList = new DevExpress.XtraGrid.GridControl();
            this.gv_PlayList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
            this.panelControl_Filter = new DevExpress.XtraEditors.PanelControl();
            this.checkButton_FilterType = new DevExpress.XtraEditors.CheckButton();
            this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            this.btnEdit_Find = new DevExpress.XtraEditors.ButtonEdit();
            this.timer_add_row = new System.Windows.Forms.Timer(this.components);
            this.timer_tooltip = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grid_PlayList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_PlayList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Filter)).BeginInit();
            this.panelControl_Filter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEdit_Find.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // grid_PlayList
            // 
            this.grid_PlayList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_PlayList.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grid_PlayList.Location = new System.Drawing.Point(0, 44);
            this.grid_PlayList.MainView = this.gv_PlayList;
            this.grid_PlayList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grid_PlayList.Name = "grid_PlayList";
            this.grid_PlayList.Size = new System.Drawing.Size(367, 368);
            this.grid_PlayList.TabIndex = 1;
            this.grid_PlayList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_PlayList});
            this.grid_PlayList.MouseEnter += new System.EventHandler(this.grid_PlayList_MouseEnter);
            // 
            // gv_PlayList
            // 
            this.gv_PlayList.Appearance.Empty.BackColor = System.Drawing.Color.Black;
            this.gv_PlayList.Appearance.Empty.Options.UseBackColor = true;
            this.gv_PlayList.Appearance.Row.BackColor = System.Drawing.Color.Transparent;
            this.gv_PlayList.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gv_PlayList.Appearance.Row.ForeColor = System.Drawing.Color.White;
            this.gv_PlayList.Appearance.Row.Options.UseBackColor = true;
            this.gv_PlayList.Appearance.Row.Options.UseFont = true;
            this.gv_PlayList.Appearance.Row.Options.UseForeColor = true;
            this.gv_PlayList.Appearance.SelectedRow.BackColor = System.Drawing.Color.Chocolate;
            this.gv_PlayList.Appearance.SelectedRow.BackColor2 = System.Drawing.Color.Lime;
            this.gv_PlayList.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gv_PlayList.Appearance.SelectedRow.Options.UseBorderColor = true;
            this.gv_PlayList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3});
            this.gv_PlayList.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gv_PlayList.GridControl = this.grid_PlayList;
            this.gv_PlayList.Name = "gv_PlayList";
            this.gv_PlayList.OptionsHint.ShowCellHints = false;
            this.gv_PlayList.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gv_PlayList.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gv_PlayList.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gv_PlayList.OptionsView.ShowGroupPanel = false;
            this.gv_PlayList.OptionsView.ShowIndicator = false;
            this.gv_PlayList.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.gv_PlayList_MouseWheel);
            this.gv_PlayList.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_PlayList_MouseMove);
            this.gv_PlayList.MouseLeave += new System.EventHandler(this.gv_PlayList_MouseLeave);
            this.gv_PlayList.LostFocus += new System.EventHandler(this.gv_PlayList_LostFocus);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn1.AppearanceHeader.Options.UseFont = true;
            this.gridColumn1.Caption = "№";
            this.gridColumn1.FieldName = "Num";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Width = 30;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn2.AppearanceHeader.Options.UseFont = true;
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "Название";
            this.gridColumn2.FieldName = "Name";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 0;
            this.gridColumn2.Width = 119;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn3.AppearanceHeader.Options.UseFont = true;
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = "Исполнитель";
            this.gridColumn3.FieldName = "Artist";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 1;
            this.gridColumn3.Width = 117;
            // 
            // toolTipController
            // 
            this.toolTipController.Appearance.Font = new System.Drawing.Font("Tahoma", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolTipController.Appearance.Options.UseFont = true;
            this.toolTipController.InitialDelay = 1000;
            this.toolTipController.ReshowDelay = 1000;
            this.toolTipController.ToolTipLocation = DevExpress.Utils.ToolTipLocation.TopRight;
            // 
            // panelControl_Filter
            // 
            this.panelControl_Filter.Controls.Add(this.checkButton_FilterType);
            this.panelControl_Filter.Controls.Add(this.btnEdit_Find);
            this.panelControl_Filter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl_Filter.Location = new System.Drawing.Point(0, 0);
            this.panelControl_Filter.Name = "panelControl_Filter";
            this.panelControl_Filter.Size = new System.Drawing.Size(367, 44);
            this.panelControl_Filter.TabIndex = 2;
            // 
            // checkButton_FilterType
            // 
            this.checkButton_FilterType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkButton_FilterType.Checked = true;
            this.checkButton_FilterType.ImageIndex = 0;
            this.checkButton_FilterType.ImageList = this.imageCollection1;
            this.checkButton_FilterType.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.checkButton_FilterType.Location = new System.Drawing.Point(320, 11);
            this.checkButton_FilterType.Name = "checkButton_FilterType";
            this.checkButton_FilterType.Size = new System.Drawing.Size(42, 23);
            this.checkButton_FilterType.TabIndex = 1;
            this.checkButton_FilterType.ToolTip = "Искать все слова через пробел";
            this.checkButton_FilterType.CheckedChanged += new System.EventHandler(this.checkButton_FilterType_CheckedChanged);
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.Images.SetKeyName(0, "Show2.png");
            this.imageCollection1.Images.SetKeyName(1, "Show.png");
            // 
            // btnEdit_Find
            // 
            this.btnEdit_Find.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit_Find.Location = new System.Drawing.Point(6, 6);
            this.btnEdit_Find.Name = "btnEdit_Find";
            this.btnEdit_Find.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnEdit_Find.Properties.Appearance.Options.UseFont = true;
            this.btnEdit_Find.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("btnEdit_Find.Properties.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, true)});
            this.btnEdit_Find.Size = new System.Drawing.Size(308, 30);
            this.btnEdit_Find.TabIndex = 0;
            this.btnEdit_Find.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnEdit_Find_ButtonClick);
            this.btnEdit_Find.EditValueChanged += new System.EventHandler(this.btnEdit_Find_EditValueChanged);
            this.btnEdit_Find.Leave += new System.EventHandler(this.btnEdit_Find_Leave);
            this.btnEdit_Find.MouseEnter += new System.EventHandler(this.btnEdit_Find_MouseEnter);
            // 
            // timer_add_row
            // 
            this.timer_add_row.Interval = 125;
            this.timer_add_row.Tick += new System.EventHandler(this.timer_add_row_Tick);
            // 
            // timer_tooltip
            // 
            this.timer_tooltip.Interval = 1000;
            this.timer_tooltip.Tick += new System.EventHandler(this.timer_tooltip_Tick);
            // 
            // Control_PlayList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grid_PlayList);
            this.Controls.Add(this.panelControl_Filter);
            this.Name = "Control_PlayList";
            this.Size = new System.Drawing.Size(367, 412);
            ((System.ComponentModel.ISupportInitialize)(this.grid_PlayList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_PlayList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Filter)).EndInit();
            this.panelControl_Filter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEdit_Find.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public DevExpress.XtraGrid.GridControl grid_PlayList;
        public DevExpress.XtraGrid.Views.Grid.GridView gv_PlayList;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.Utils.ToolTipController toolTipController;
        private DevExpress.XtraEditors.PanelControl panelControl_Filter;
        private DevExpress.XtraEditors.ButtonEdit btnEdit_Find;
        private DevExpress.XtraEditors.CheckButton checkButton_FilterType;
        private DevExpress.Utils.ImageCollection imageCollection1;
        public System.Windows.Forms.Timer timer_add_row;
        private System.Windows.Forms.Timer timer_tooltip;
    }
}
