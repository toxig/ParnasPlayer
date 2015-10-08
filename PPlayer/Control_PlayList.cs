using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;

namespace PPlayer
{
    public partial class Control_PlayList : DevExpress.XtraEditors.XtraUserControl
    {
        public string pl_FilePath = "";
        public DataTable dt_FileData;
        public DataTable dt_ListData;
        public GridViewFilterHelper filterHelper;
        public int tooltip_secons = 2; // кол-во секунд, которые держится тултип
        int OldRow = 0;

        private int timer_ticks = 3;
        private Color row_color_default;
        private Color row_color_blink;        
        private int tooltip_hide_secons;
        private string tooltip_text = "";
        private int Row_Handle = -1;
        private MouseEventArgs e_last;
        private bool is_scroll = false;
        private bool list_is_focused = false;
        private bool find_is_focused = false;

        // debug        

        public Control_PlayList()
        {
            InitializeComponent();

            row_color_default = gv_PlayList.Appearance.FocusedRow.BackColor;
            row_color_blink = Color.DarkGray;
            timer_add_row.Interval = 100;

            // таблицы данных
            Init_Table_Data();
            
            // поиск по листу
            filterHelper = new GridViewFilterHelper(gv_PlayList);
            filterHelper.Allwords = true; // искать все слова через запятую (иначе точное совпадение фразы)
        }

        #region Таблицы плейлистов
        // Инициализация таблиц
        public void Init_Table_Data()
        {
            dt_FileData = Get_PlayList_FileTable();
            dt_ListData = Get_PlayList_GridTable();
            grid_PlayList.DataSource = dt_ListData;

            gv_PlayList.ActiveFilter.Clear();
        }

        // Таблица плейлиста из файла Parnas Machine
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

        // Таблица плейлиста для GridView
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
        #endregion

        #region Всплывающая подсказка
        private void gv_PlayList_MouseMove(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hti = gv_PlayList.CalcHitInfo(e.X, e.Y);
            e_last = e;

            if (hti.RowHandle >= 0 && (hti.RowHandle != Row_Handle || is_scroll))
            {
                is_scroll = false;
                Row_Handle = hti.RowHandle;                

                if (OldRow != Row_Handle)
                {
                    toolTipController.HideHint();
                    OldRow = hti.RowHandle;
                }

                DataRow Row = gv_PlayList.GetDataRow(Row_Handle);
                string msg = System.IO.Path.GetFileNameWithoutExtension(Row[0].ToString());

                tooltip_text = msg;
                toolTipController.ShowHint(msg);

                /*info = new ToolTipControlInfo(new CellToolTipInfo(hi.RowHandle, hi.Column, "cell")
                    , "123456789",ToolTipIconType.Information);*/
                
                tooltip_hide_secons = tooltip_secons - 1;
                timer_tooltip.Start();
            }            
        }        
        #endregion

        #region Поиск (фильтр)
        // Изменение фильтра
        private void btnEdit_Find_EditValueChanged(object sender, EventArgs e)
        {
            if (btnEdit_Find.Text.Trim() != "") // пусто
            {
                filterHelper.ActiveFilter = btnEdit_Find.Text.ToUpper().Trim();
            }
            else
            {
                filterHelper.ActiveFilter = "";
            }
        }

        // Очистка фильтра
        private void btnEdit_Find_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnEdit_Find.Text = "";
        }

        // Изменение типа поиска в фильтре
        private void checkButton_FilterType_CheckedChanged(object sender, EventArgs e)
        {
            filterHelper.Allwords = checkButton_FilterType.Checked;
            if (filterHelper.Allwords) checkButton_FilterType.ImageIndex = 0;
            else checkButton_FilterType.ImageIndex = 1;
        }

        public void Toggle_Show_Filter()
        {
            panelControl_Filter.Visible = panelControl_Filter.Visible ? false : true;
        }
        #endregion

        // Запуск мигания
        public void Focused_Row_Blink()
        {
            timer_add_row.Enabled = false;
            timer_ticks = 3;
            timer_add_row.Enabled = true;
        }

        // мигание выделенной строки ****
        private void timer_add_row_Tick(object sender, EventArgs e)
        {
            if (timer_ticks % 2 != 0)
            {
                gv_PlayList.Appearance.FocusedRow.BackColor = row_color_blink;                
            }
            else 
            {
                gv_PlayList.Appearance.FocusedRow.BackColor = row_color_default;
            }

            if (timer_ticks > 0) timer_ticks--;
            else timer_add_row.Enabled = false;                
        }

        // отключение всплывающей подсказки
        private void timer_tooltip_Tick(object sender, EventArgs e)
        {
            if (tooltip_hide_secons > 0)
            {
                tooltip_hide_secons--;
            }
            else
            {
                toolTipController.HideHint();
                timer_tooltip.Stop();
            }
        }

        // скролл мышки (переход на другую строку плейлиста)
        private void gv_PlayList_MouseWheel(object sender, MouseEventArgs e)
        {                        
            is_scroll = true;
            toolTipController.HideHint();
            gv_PlayList_MouseMove(sender, e_last);
        }

        // потеря мышки на плейлисте
        private void gv_PlayList_MouseLeave(object sender, EventArgs e)
        {
            toolTipController.HideHint();
        }

        // установка фокуса на плейлист при наведении
        private void grid_PlayList_MouseEnter(object sender, EventArgs e)
        {
            if (!list_is_focused)
            {
                list_is_focused = true;                
                gv_PlayList.Focus();
            }
        }        

        // потеря фокуса плейлистом
        private void gv_PlayList_LostFocus(object sender, EventArgs e)
        {
            list_is_focused = false;            
        }

        private void btnEdit_Find_MouseEnter(object sender, EventArgs e)
        {
            if (!find_is_focused)
            {
                find_is_focused = true;                                
                btnEdit_Find.Focus();
                btnEdit_Find.SelectAll();
            }
        }

        private void btnEdit_Find_Leave(object sender, EventArgs e)
        {
            find_is_focused = false;            
        }
        
    }
}
