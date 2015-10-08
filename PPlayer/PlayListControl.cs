using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace PPlayer
{
    public partial class PlayListControl : DevExpress.XtraEditors.XtraUserControl
    {
        public DataTable dt_FileData;
        public DataTable dt_ListData;
        public GridViewFilterHelper filterHelper;        
        int OldRow = 0;

        public PlayListControl()
        {
            InitializeComponent();            

            // таблицы данных
            Init_Table_Data();
            
            // поиск по листу
            filterHelper = new GridViewFilterHelper(gv_PlayList);

            // скрыть поиск
            gv_PlayList.OptionsFind.AlwaysVisible = false;
            
        }

        public void Init_Table_Data()
        {
            dt_FileData = Get_PlayList_FileTable();
            dt_ListData = Get_PlayList_GridTable();
            grid_PlayList.DataSource = dt_ListData;

            gv_PlayList.ActiveFilter.Clear();
        }

        // Таблица плейлистов Parnas Machine
        static DataTable Get_PlayList_FileTable()
        {
            //
            // Create a DataTable with columns.
            //
            DataTable table = new DataTable();
            table.Columns.Add("MuzFile", typeof(string));
            table.Columns.Add("TextFile", typeof(string));
            table.Columns.Add("EqLines", typeof(string));
            table.Columns.Add("EqPreamp", typeof(string));
            table.Columns.Add("Volume", typeof(string));
            table.Columns.Add("LRBalance", typeof(string));
            table.Columns.Add("def1", typeof(string));
            table.Columns.Add("def2", typeof(string));
            //
            // Add DataRows.
            //
            //table.Rows.Add(25, "Indocin", "David", DateTime.Now);
            return table;
        }

        // Таблица плейлистов Parnas Machine
        static DataTable Get_PlayList_GridTable()
        {
            //
            // Create a DataTable with columns.
            //
            DataTable table = new DataTable();
            table.Columns.Add("MuzFile", typeof(string));
            table.Columns.Add("TextFile", typeof(string));
            table.Columns.Add("EqLines", typeof(string));
            table.Columns.Add("EqPreamp", typeof(string));
            table.Columns.Add("Volume", typeof(string));
            table.Columns.Add("LRBalance", typeof(string));
            table.Columns.Add("def1", typeof(string));
            table.Columns.Add("def2", typeof(string));
            table.Columns.Add("Num", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Artist", typeof(string));
            //
            // Add DataRows.
            //
            //table.Rows.Add(25, "Indocin", "David", DateTime.Now);
            return table;
        }

        private void gv_PlayList_Click(object sender, EventArgs e)
        {
            /*if (dt_ListData != null && dt_ListData.Rows.Count > 0)
            {
                int Row_ID = Convert.ToInt32(gv_PlayList.GetFocusedDataRow()["Num"]);
                string msg = System.IO.Path.GetFileName(dt_FileData.Rows[Row_ID - 1][0].ToString());
                toolTipController.ShowHint(msg);
            }*/
        }

        // Всплывающая подсказка
        private void gv_PlayList_MouseMove(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hti = gv_PlayList.CalcHitInfo(e.X, e.Y);

            if (hti.RowHandle >= 0)
            {
                int Row_Handle = hti.RowHandle;

                if (OldRow != Row_Handle)
                {
                    toolTipController.HideHint();
                    OldRow = hti.RowHandle;
                }

                DataRow Row = gv_PlayList.GetDataRow(Row_Handle);
                string msg = System.IO.Path.GetFileNameWithoutExtension(Row[0].ToString());                
                toolTipController.ShowHint(msg);

                /*info = new ToolTipControlInfo(new CellToolTipInfo(hi.RowHandle, hi.Column, "cell")
                    , "123456789",ToolTipIconType.Information);*/
            }
        }

        private void gv_PlayList_MouseLeave(object sender, EventArgs e)
        {
            toolTipController.HideHint();
        }
        
    }
}
