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
            DevExpress.XtraGrid.StyleFormatCondition styleFormatCondition1 = new DevExpress.XtraGrid.StyleFormatCondition();
            DevExpress.XtraGrid.StyleFormatCondition styleFormatCondition2 = new DevExpress.XtraGrid.StyleFormatCondition();
            DevExpress.XtraGrid.StyleFormatCondition styleFormatCondition3 = new DevExpress.XtraGrid.StyleFormatCondition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Control_PlayList));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            this.gridColumn_exists = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grid_PlayList = new DevExpress.XtraGrid.GridControl();
            this.gv_PlayList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_num = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_name = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_artist = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_date = new DevExpress.XtraGrid.Columns.GridColumn();
            this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
            this.panelControl_Filter = new DevExpress.XtraEditors.PanelControl();
            this.checkButton_Filter_Plus = new DevExpress.XtraEditors.CheckButton();
            this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
            this.btnEdit_Find = new DevExpress.XtraEditors.ButtonEdit();
            this.checkButton_Toggle_PLColumns = new DevExpress.XtraEditors.CheckButton();
            this.timer_add_row = new System.Windows.Forms.Timer(this.components);
            this.timer_tooltip = new System.Windows.Forms.Timer(this.components);
            this.panelControl_header = new DevExpress.XtraEditors.PanelControl();
            this.checkButton_Toggle_FindPanel = new DevExpress.XtraEditors.CheckButton();
            this.labelControl_header = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.grid_PlayList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_PlayList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Filter)).BeginInit();
            this.panelControl_Filter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEdit_Find.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_header)).BeginInit();
            this.panelControl_header.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridColumn_exists
            // 
            this.gridColumn_exists.Caption = "FilesExists";
            this.gridColumn_exists.FieldName = "FileExistsFlag";
            this.gridColumn_exists.Name = "gridColumn_exists";
            // 
            // grid_PlayList
            // 
            this.grid_PlayList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_PlayList.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grid_PlayList.Location = new System.Drawing.Point(0, 72);
            this.grid_PlayList.MainView = this.gv_PlayList;
            this.grid_PlayList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grid_PlayList.Name = "grid_PlayList";
            this.grid_PlayList.Size = new System.Drawing.Size(367, 340);
            this.grid_PlayList.TabIndex = 1;
            this.grid_PlayList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_PlayList});
            this.grid_PlayList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.grid_PlayList_MouseClick);
            this.grid_PlayList.MouseEnter += new System.EventHandler(this.grid_PlayList_MouseEnter);
            // 
            // gv_PlayList
            // 
            this.gv_PlayList.Appearance.Empty.BackColor = System.Drawing.Color.Black;
            this.gv_PlayList.Appearance.Empty.Options.UseBackColor = true;
            this.gv_PlayList.Appearance.FocusedRow.BackColor = System.Drawing.Color.MediumBlue;
            this.gv_PlayList.Appearance.FocusedRow.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gv_PlayList.Appearance.FocusedRow.ForeColor = System.Drawing.Color.White;
            this.gv_PlayList.Appearance.FocusedRow.Options.UseBackColor = true;
            this.gv_PlayList.Appearance.FocusedRow.Options.UseFont = true;
            this.gv_PlayList.Appearance.FocusedRow.Options.UseForeColor = true;
            this.gv_PlayList.Appearance.HorzLine.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.gv_PlayList.Appearance.HorzLine.Options.UseBackColor = true;
            this.gv_PlayList.Appearance.Row.BackColor = System.Drawing.Color.Transparent;
            this.gv_PlayList.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gv_PlayList.Appearance.Row.ForeColor = System.Drawing.Color.Lime;
            this.gv_PlayList.Appearance.Row.Options.UseBackColor = true;
            this.gv_PlayList.Appearance.Row.Options.UseFont = true;
            this.gv_PlayList.Appearance.Row.Options.UseForeColor = true;
            this.gv_PlayList.Appearance.SelectedRow.BackColor = System.Drawing.Color.Chocolate;
            this.gv_PlayList.Appearance.SelectedRow.BackColor2 = System.Drawing.Color.Lime;
            this.gv_PlayList.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gv_PlayList.Appearance.SelectedRow.Options.UseBorderColor = true;
            this.gv_PlayList.Appearance.VertLine.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.gv_PlayList.Appearance.VertLine.Options.UseBackColor = true;
            this.gv_PlayList.ColumnPanelRowHeight = 27;
            this.gv_PlayList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_num,
            this.gridColumn_name,
            this.gridColumn_artist,
            this.gridColumn_exists,
            this.gridColumn_date});
            this.gv_PlayList.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            styleFormatCondition1.Appearance.BackColor = System.Drawing.Color.Black;
            styleFormatCondition1.Appearance.ForeColor = System.Drawing.Color.Salmon;
            styleFormatCondition1.Appearance.Options.UseBackColor = true;
            styleFormatCondition1.Appearance.Options.UseForeColor = true;
            styleFormatCondition1.ApplyToRow = true;
            styleFormatCondition1.Column = this.gridColumn_exists;
            styleFormatCondition1.Condition = DevExpress.XtraGrid.FormatConditionEnum.Equal;
            styleFormatCondition1.Value1 = "100";
            styleFormatCondition2.Appearance.BackColor = System.Drawing.Color.Black;
            styleFormatCondition2.Appearance.ForeColor = System.Drawing.Color.Salmon;
            styleFormatCondition2.Appearance.Options.UseBackColor = true;
            styleFormatCondition2.Appearance.Options.UseForeColor = true;
            styleFormatCondition2.ApplyToRow = true;
            styleFormatCondition2.Column = this.gridColumn_exists;
            styleFormatCondition2.Condition = DevExpress.XtraGrid.FormatConditionEnum.Equal;
            styleFormatCondition2.Value1 = "101";
            styleFormatCondition3.Appearance.BackColor = System.Drawing.Color.Black;
            styleFormatCondition3.Appearance.ForeColor = System.Drawing.Color.DarkGray;
            styleFormatCondition3.Appearance.Options.UseBackColor = true;
            styleFormatCondition3.Appearance.Options.UseForeColor = true;
            styleFormatCondition3.ApplyToRow = true;
            styleFormatCondition3.Column = this.gridColumn_exists;
            styleFormatCondition3.Condition = DevExpress.XtraGrid.FormatConditionEnum.Equal;
            styleFormatCondition3.Value1 = "110";
            this.gv_PlayList.FormatConditions.AddRange(new DevExpress.XtraGrid.StyleFormatCondition[] {
            styleFormatCondition1,
            styleFormatCondition2,
            styleFormatCondition3});
            this.gv_PlayList.GridControl = this.grid_PlayList;
            this.gv_PlayList.Name = "gv_PlayList";
            this.gv_PlayList.OptionsHint.ShowCellHints = false;
            this.gv_PlayList.OptionsMenu.EnableColumnMenu = false;
            this.gv_PlayList.OptionsMenu.EnableFooterMenu = false;
            this.gv_PlayList.OptionsMenu.EnableGroupPanelMenu = false;
            this.gv_PlayList.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gv_PlayList.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gv_PlayList.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gv_PlayList.OptionsView.ShowGroupPanel = false;
            this.gv_PlayList.OptionsView.ShowIndicator = false;
            this.gv_PlayList.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn_name, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.gv_PlayList.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_PlayList_PopupMenuShowing);
            this.gv_PlayList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gv_PlayList_KeyPress);
            this.gv_PlayList.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.gv_PlayList_MouseWheel);
            this.gv_PlayList.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_PlayList_MouseMove);
            this.gv_PlayList.MouseLeave += new System.EventHandler(this.gv_PlayList_MouseLeave);
            this.gv_PlayList.LostFocus += new System.EventHandler(this.gv_PlayList_LostFocus);
            // 
            // gridColumn_num
            // 
            this.gridColumn_num.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridColumn_num.AppearanceHeader.Options.UseFont = true;
            this.gridColumn_num.Caption = "№";
            this.gridColumn_num.FieldName = "Num";
            this.gridColumn_num.Name = "gridColumn_num";
            this.gridColumn_num.OptionsColumn.AllowEdit = false;
            this.gridColumn_num.Width = 30;
            // 
            // gridColumn_name
            // 
            this.gridColumn_name.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gridColumn_name.AppearanceHeader.Options.UseFont = true;
            this.gridColumn_name.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_name.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_name.Caption = "Песня - Артист";
            this.gridColumn_name.FieldName = "Name";
            this.gridColumn_name.Name = "gridColumn_name";
            this.gridColumn_name.OptionsColumn.AllowEdit = false;
            this.gridColumn_name.OptionsFilter.AllowAutoFilter = false;
            this.gridColumn_name.OptionsFilter.AllowFilter = false;
            this.gridColumn_name.Visible = true;
            this.gridColumn_name.VisibleIndex = 0;
            this.gridColumn_name.Width = 119;
            // 
            // gridColumn_artist
            // 
            this.gridColumn_artist.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.gridColumn_artist.AppearanceHeader.Options.UseFont = true;
            this.gridColumn_artist.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_artist.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_artist.Caption = "Артист - Песня";
            this.gridColumn_artist.FieldName = "Artist";
            this.gridColumn_artist.Name = "gridColumn_artist";
            this.gridColumn_artist.OptionsColumn.AllowEdit = false;
            this.gridColumn_artist.Width = 117;
            // 
            // gridColumn_date
            // 
            this.gridColumn_date.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Bold);
            this.gridColumn_date.AppearanceHeader.Options.UseFont = true;
            this.gridColumn_date.Caption = "Дата";
            this.gridColumn_date.DisplayFormat.FormatString = "dd.MM.yy HH:m:ss";
            this.gridColumn_date.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.gridColumn_date.FieldName = "Date";
            this.gridColumn_date.Name = "gridColumn_date";
            this.gridColumn_date.OptionsColumn.AllowEdit = false;
            // 
            // toolTipController
            // 
            this.toolTipController.Appearance.Font = new System.Drawing.Font("Tahoma", 22.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolTipController.Appearance.ForeColor = System.Drawing.Color.Navy;
            this.toolTipController.Appearance.Options.UseFont = true;
            this.toolTipController.Appearance.Options.UseForeColor = true;
            this.toolTipController.AppearanceTitle.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.toolTipController.AppearanceTitle.Options.UseForeColor = true;
            this.toolTipController.InitialDelay = 1000;
            this.toolTipController.ReshowDelay = 1000;
            this.toolTipController.ToolTipLocation = DevExpress.Utils.ToolTipLocation.TopRight;
            // 
            // panelControl_Filter
            // 
            this.panelControl_Filter.Controls.Add(this.checkButton_Filter_Plus);
            this.panelControl_Filter.Controls.Add(this.btnEdit_Find);
            this.panelControl_Filter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl_Filter.Location = new System.Drawing.Point(0, 28);
            this.panelControl_Filter.Name = "panelControl_Filter";
            this.panelControl_Filter.Size = new System.Drawing.Size(367, 44);
            this.panelControl_Filter.TabIndex = 2;
            this.panelControl_Filter.Visible = false;
            // 
            // checkButton_Filter_Plus
            // 
            this.checkButton_Filter_Plus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkButton_Filter_Plus.ImageIndex = 1;
            this.checkButton_Filter_Plus.ImageList = this.imageCollection1;
            this.checkButton_Filter_Plus.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.checkButton_Filter_Plus.Location = new System.Drawing.Point(320, 9);
            this.checkButton_Filter_Plus.Name = "checkButton_Filter_Plus";
            this.checkButton_Filter_Plus.Size = new System.Drawing.Size(42, 23);
            this.checkButton_Filter_Plus.TabIndex = 1;
            this.checkButton_Filter_Plus.ToolTip = "Режим поиска (простой - продвинутый)";
            this.checkButton_Filter_Plus.CheckedChanged += new System.EventHandler(this.checkButton_Filter_Plus_CheckedChanged);
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.Images.SetKeyName(0, "Show2.png");
            this.imageCollection1.Images.SetKeyName(1, "Show.png");
            this.imageCollection1.Images.SetKeyName(2, "See3.png");
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
            // checkButton_Toggle_PLColumns
            // 
            this.checkButton_Toggle_PLColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkButton_Toggle_PLColumns.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkButton_Toggle_PLColumns.Appearance.Options.UseFont = true;
            this.checkButton_Toggle_PLColumns.ImageList = this.imageCollection1;
            this.checkButton_Toggle_PLColumns.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.checkButton_Toggle_PLColumns.Location = new System.Drawing.Point(334, 2);
            this.checkButton_Toggle_PLColumns.Name = "checkButton_Toggle_PLColumns";
            this.checkButton_Toggle_PLColumns.Size = new System.Drawing.Size(28, 23);
            this.checkButton_Toggle_PLColumns.TabIndex = 2;
            this.checkButton_Toggle_PLColumns.Text = "П";
            this.checkButton_Toggle_PLColumns.ToolTip = "Название - Исполниетль";
            this.checkButton_Toggle_PLColumns.ToolTipTitle = "Переключение списка";
            this.checkButton_Toggle_PLColumns.CheckedChanged += new System.EventHandler(this.checkButton_Toggle_PLColumn);
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
            // panelControl_header
            // 
            this.panelControl_header.Controls.Add(this.checkButton_Toggle_FindPanel);
            this.panelControl_header.Controls.Add(this.checkButton_Toggle_PLColumns);
            this.panelControl_header.Controls.Add(this.labelControl_header);
            this.panelControl_header.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl_header.Location = new System.Drawing.Point(0, 0);
            this.panelControl_header.Name = "panelControl_header";
            this.panelControl_header.Size = new System.Drawing.Size(367, 28);
            this.panelControl_header.TabIndex = 2;
            // 
            // checkButton_Toggle_FindPanel
            // 
            this.checkButton_Toggle_FindPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkButton_Toggle_FindPanel.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkButton_Toggle_FindPanel.Appearance.Options.UseFont = true;
            this.checkButton_Toggle_FindPanel.ImageIndex = 2;
            this.checkButton_Toggle_FindPanel.ImageList = this.imageCollection1;
            this.checkButton_Toggle_FindPanel.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.checkButton_Toggle_FindPanel.Location = new System.Drawing.Point(304, 2);
            this.checkButton_Toggle_FindPanel.Name = "checkButton_Toggle_FindPanel";
            this.checkButton_Toggle_FindPanel.Size = new System.Drawing.Size(28, 23);
            this.checkButton_Toggle_FindPanel.TabIndex = 3;
            this.checkButton_Toggle_FindPanel.ToolTip = "Поиск";
            this.checkButton_Toggle_FindPanel.ToolTipTitle = "Переключение списка";
            this.checkButton_Toggle_FindPanel.CheckedChanged += new System.EventHandler(this.checkButton_Toggle_FindPanel_CheckedChanged);
            // 
            // labelControl_header
            // 
            this.labelControl_header.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelControl_header.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.labelControl_header.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelControl_header.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.labelControl_header.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.labelControl_header.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
            this.labelControl_header.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.labelControl_header.Location = new System.Drawing.Point(2, 2);
            this.labelControl_header.Name = "labelControl_header";
            this.labelControl_header.Size = new System.Drawing.Size(296, 24);
            this.labelControl_header.TabIndex = 0;
            this.labelControl_header.Text = "новый плейлист";
            this.labelControl_header.ToolTipController = this.toolTipController;
            // 
            // Control_PlayList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grid_PlayList);
            this.Controls.Add(this.panelControl_Filter);
            this.Controls.Add(this.panelControl_header);
            this.Name = "Control_PlayList";
            this.Size = new System.Drawing.Size(367, 412);
            ((System.ComponentModel.ISupportInitialize)(this.grid_PlayList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_PlayList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_Filter)).EndInit();
            this.panelControl_Filter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnEdit_Find.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl_header)).EndInit();
            this.panelControl_header.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public DevExpress.XtraGrid.GridControl grid_PlayList;
        public DevExpress.XtraGrid.Views.Grid.GridView gv_PlayList;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn_num;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn_name;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn_artist;
        private DevExpress.XtraEditors.PanelControl panelControl_Filter;
        private DevExpress.XtraEditors.CheckButton checkButton_Filter_Plus;
        private DevExpress.Utils.ImageCollection imageCollection1;
        public System.Windows.Forms.Timer timer_add_row;
        private System.Windows.Forms.Timer timer_tooltip;
        public DevExpress.Utils.ToolTipController toolTipController;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_exists;
        private DevExpress.XtraEditors.PanelControl panelControl_header;
        private DevExpress.XtraEditors.CheckButton checkButton_Toggle_PLColumns;
        public DevExpress.XtraEditors.ButtonEdit btnEdit_Find;
        public DevExpress.XtraEditors.LabelControl labelControl_header;
        private DevExpress.XtraEditors.CheckButton checkButton_Toggle_FindPanel;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_date;
    }
}
