namespace PPlayer
{
    partial class PlayListControl
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
            this.grid_PlayList = new DevExpress.XtraGrid.GridControl();
            this.gv_PlayList = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grid_PlayList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_PlayList)).BeginInit();
            this.SuspendLayout();
            // 
            // grid_PlayList
            // 
            this.grid_PlayList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_PlayList.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grid_PlayList.Location = new System.Drawing.Point(0, 0);
            this.grid_PlayList.MainView = this.gv_PlayList;
            this.grid_PlayList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grid_PlayList.Name = "grid_PlayList";
            this.grid_PlayList.Size = new System.Drawing.Size(367, 412);
            this.grid_PlayList.TabIndex = 1;
            this.grid_PlayList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gv_PlayList});
            // 
            // gv_PlayList
            // 
            this.gv_PlayList.Appearance.Empty.BackColor = System.Drawing.Color.Black;
            this.gv_PlayList.Appearance.Empty.Options.UseBackColor = true;
            this.gv_PlayList.Appearance.Row.BackColor = System.Drawing.Color.Transparent;
            this.gv_PlayList.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.gv_PlayList.Appearance.Row.ForeColor = System.Drawing.Color.White;
            this.gv_PlayList.Appearance.Row.Options.UseBackColor = true;
            this.gv_PlayList.Appearance.Row.Options.UseFont = true;
            this.gv_PlayList.Appearance.Row.Options.UseForeColor = true;
            this.gv_PlayList.Appearance.SelectedRow.BackColor = System.Drawing.Color.Chocolate;
            this.gv_PlayList.Appearance.SelectedRow.Options.UseBackColor = true;
            this.gv_PlayList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3});
            this.gv_PlayList.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gv_PlayList.GridControl = this.grid_PlayList;
            this.gv_PlayList.Name = "gv_PlayList";
            this.gv_PlayList.OptionsFind.AlwaysVisible = true;
            this.gv_PlayList.OptionsFind.ClearFindOnClose = false;
            this.gv_PlayList.OptionsFind.FindDelay = 500;
            this.gv_PlayList.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gv_PlayList.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gv_PlayList.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.gv_PlayList.OptionsView.ShowGroupPanel = false;
            this.gv_PlayList.OptionsView.ShowIndicator = false;
            this.gv_PlayList.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_PlayList_MouseMove);
            this.gv_PlayList.MouseLeave += new System.EventHandler(this.gv_PlayList_MouseLeave);
            this.gv_PlayList.Click += new System.EventHandler(this.gv_PlayList_Click);
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
            this.toolTipController.ToolTipLocation = DevExpress.Utils.ToolTipLocation.TopLeft;
            // 
            // PlayListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grid_PlayList);
            this.Name = "PlayListControl";
            this.Size = new System.Drawing.Size(367, 412);
            ((System.ComponentModel.ISupportInitialize)(this.grid_PlayList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gv_PlayList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public DevExpress.XtraGrid.GridControl grid_PlayList;
        public DevExpress.XtraGrid.Views.Grid.GridView gv_PlayList;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        public DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.Utils.ToolTipController toolTipController;
    }
}
