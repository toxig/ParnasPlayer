using System;
using System.IO;
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
        private string _pl_FilePath = "";
        public string pl_FilePath
        {
            get
            {
                return _pl_FilePath;
            }
            set 
            {
                _pl_FilePath = value;
                if (value != "")
                {
                    string tName = Path.GetFileNameWithoutExtension(value);
                    tName = tName[0].ToString().ToUpper() + tName.Substring(1, tName.Length - 1);
                    labelControl_header.Text = tName;
                    labelControl_header.ToolTip = value;
                }
                else
                {
                    labelControl_header.Text = "новый плейлист";
                    labelControl_header.ToolTip = "";
                }
            }
        }

        private bool _pl_History = false;
        public bool pl_History
        {
            get
            {
                return _pl_History;
            }
            set
            {
                _pl_History = value;
                if (value == true)
                {
                    if (!dt_ListData.Columns.Contains("Date"))
                    {
                        dt_ListData.Columns.Add("Date", typeof(DateTime)); // не обработано наличие
                    }

                    gridColumn_date.Visible = true;
                }
                else
                {
                    gridColumn_date.Visible = false;
                }
            }
        }

        public DataTable dt_FileData;
        public DataTable dt_ListData;
        public GridViewFilterHelper filterHelper;
        public bool is_Changed = false;

        // константы
        public int tooltip_hide_ms = 2000; // кол-во секунд, которые держится тултип
        public int tooltip_wait_ms = 500; // кол-во мс (мили секунд, перед появлением тултипа)
        private int timer_ticks_count = 5;

        public DateTime tooltip_hide_dt;
        public DateTime tooltip_wait_dt;
        public bool tooltip_started = true;

        private int OldRow = 0;
        private int timer_ticks = 5; // колво миганий строки при переносе в hotlist
        private int timer_type = 0; // цвет мигания ok/error
        private Color row_color_default;
        private Color row_color_blink;
        private Color row_color_err_blink;        
        private string tooltip_text = "";
        private int Row_Handle = -1;
        private MouseEventArgs e_last;
        private bool is_scroll = false;
        private bool list_is_focused = false;
        private bool find_is_focused = false;
        private TagLib.File Tag_File; // Тэги в файле
        public bool v_CheckTags = false;        

        public Form_Working FWorking;
        // debug        

        public Control_PlayList()
        {
            InitializeComponent();

            pl_FilePath = "";

            row_color_default = gv_PlayList.Appearance.FocusedRow.BackColor;
            row_color_blink = Color.Lime; // ок блинк
            row_color_err_blink = Color.Red; //err блинк
            timer_add_row.Interval = 100;
            timer_tooltip.Interval = 100;

            // таблицы данных
            Init_Table_Data();
            
            // поиск по листу
            filterHelper = new GridViewFilterHelper(gv_PlayList);
            filterHelper.Filter_words = false; // искать все слова через запятую (иначе точное совпадение фразы)
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
            table.Columns.Add("def", typeof(string));
            table.Columns.Add("def2", typeof(string));            
            table.Columns.Add("Num", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Artist", typeof(string));
            table.Columns.Add("FileExistsFlag", typeof(string)); // не обработано наличие
            table.Columns.Add("TagComments", typeof(string)); // не обработано наличие            
            //
            // Add DataRows.
            //
            //table.Rows.Add(25, "Indocin", "David", DateTime.Now);
            return table;
        } 
        #endregion

        #region Всплывающая подсказка
        
        // значение подсказки при движении указателя
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
                
                /*info = new ToolTipControlInfo(new CellToolTipInfo(hi.RowHandle, hi.Column, "cell")
                    , "123456789",ToolTipIconType.Information);*/

                if (tooltip_text != msg)
                {
                    //toolTipController.ShowHint(msg);

                    timer_tooltip.Stop();
                    toolTipController.HideHint();
                    tooltip_text = msg;

                    tooltip_wait_dt = DateTime.Now.AddMilliseconds(tooltip_wait_ms - timer_tooltip.Interval);
                    tooltip_hide_dt = tooltip_wait_dt.AddMilliseconds(tooltip_hide_ms);
                    tooltip_started = false;

                    timer_tooltip.Start();
                }
            }            
        }

        // включение/отключение всплывающей подсказки
        private void timer_tooltip_Tick(object sender, EventArgs e)
        {
            /*
            if (tooltip_wait_ms_now >= 0)
            {
                tooltip_wait_ms_now -= timer_tooltip.Interval;
            }
            else // задержка прошла
            {
                if (tooltip_wait_ms_now < 0)
                {
                    toolTipController.ShowHint(tooltip_text);
                    tooltip_wait_ms_now = 0;                    
                }

                //tooltip_hide_ms_now -= timer_tooltip.Interval;

                if (tooltip_hide_dt < DateTime.Now)
                {
                    timer_tooltip.Stop();
                    toolTipController.HideHint();                    
                }
            }*/

            if (DateTime.Now > tooltip_wait_dt && DateTime.Now < tooltip_hide_dt && !tooltip_started)
            {
                toolTipController.ShowHint(tooltip_text);                
                tooltip_started = true;
            }

            if (tooltip_hide_dt < DateTime.Now)
            {
                timer_tooltip.Stop();
                toolTipController.HideHint();
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
        private void checkButton_Filter_Plus_CheckedChanged(object sender, EventArgs e)
        {
            if (checkButton_Filter_Plus.Checked)
            {
                checkButton_Filter_Plus.ImageIndex = 0;
                filterHelper.Filter_words = true;
            }
            else
            {
                checkButton_Filter_Plus.ImageIndex = 1;
                filterHelper.Filter_words = false;
            }
            
            filterHelper.Filter_Plus = checkButton_Filter_Plus.Checked;

            btnEdit_Find_EditValueChanged(null, null);
            grid_PlayList.Focus();
        }

        // Изменение типа поиска 2
        private void checkButton_Toggle_PLColumn(object sender, EventArgs e)
        {
            gv_PlayList.ClearSorting();

            if (checkButton_Toggle_PLColumns.Checked)
            {
                checkButton_Toggle_PLColumns.Text = "А";
                gridColumn_name.Visible = false;
                gridColumn_artist.Visible = true;
                gridColumn_artist.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
            }
            else
            {
                checkButton_Toggle_PLColumns.Text = "П";
                gridColumn_name.Visible = true;
                gridColumn_artist.Visible = false;
                gridColumn_name.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;
            }            

            btnEdit_Find_EditValueChanged(null, null);
            grid_PlayList.Focus();
        }

        public void Toggle_Show_Filter()
        {
            panelControl_Filter.Visible = panelControl_Filter.Visible ? false : true;
        }
        #endregion

        #region Горячий список

        // Запуск мигания
        public void Focused_Row_Blink()
        {
            Focused_Row_Blink(0);
        }

        // Запуск мигания с параметром
        public void Focused_Row_Blink(int color_type)
        {
            timer_tooltip.Stop();
            toolTipController.HideHint();

            if (color_type == 0)
            {
                tooltip_text = "Добавлено";
            }

            if (color_type == 1)
            {
                tooltip_text = "Уже есть в списке";
            }

            if (color_type == 2)
            {
                tooltip_text = "Нет муз файла";
            }

            tooltip_wait_dt = DateTime.Now.AddMilliseconds(tooltip_wait_ms - timer_tooltip.Interval);
            tooltip_hide_dt = tooltip_wait_dt.AddMilliseconds(tooltip_hide_ms);
            tooltip_started = false;
            timer_tooltip.Start();

            timer_add_row.Enabled = false;
            timer_ticks = timer_ticks_count;
            timer_type = color_type;
            timer_add_row.Enabled = true;
        }

        // мигание выделенной строки ****
        private void timer_add_row_Tick(object sender, EventArgs e)
        {
            if (timer_ticks % 2 != 0)
            {
                switch (timer_type)
                {
                        // ok
                    case 0: gv_PlayList.Appearance.FocusedRow.BackColor = row_color_blink;
                        break;
                        // err
                    case 1: gv_PlayList.Appearance.FocusedRow.BackColor = row_color_err_blink;
                        break;
                    case 2: gv_PlayList.Appearance.FocusedRow.BackColor = row_color_err_blink;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                gv_PlayList.Appearance.FocusedRow.BackColor = row_color_default;
            }

            if (timer_ticks > 0) timer_ticks--;
            else timer_add_row.Enabled = false;
        }

        #endregion

        #region Фокус элементов

        // установка фокуса на плейлист при наведении
        private void grid_PlayList_MouseEnter(object sender, EventArgs e)
        {
        }

        private void grid_PlayList_MouseClick(object sender, MouseEventArgs e)
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
            timer_tooltip.Stop();
            toolTipController.HideHint();
        }

        // установка фокуса
        private void btnEdit_Find_MouseEnter(object sender, EventArgs e)
        {
            if (!find_is_focused)
            {
                find_is_focused = true;
                btnEdit_Find.Focus();
                btnEdit_Find.SelectAll();
            }
        }

        // потеря фокуса
        private void btnEdit_Find_Leave(object sender, EventArgs e)
        {
            find_is_focused = false;
        }

        #endregion
        
        // активация фильтрации на списке
        private void gv_PlayList_KeyPress(object sender, KeyPressEventArgs e)
        {            
                btnEdit_Find.DeselectAll();
                if (!char.IsControl(e.KeyChar))
                    btnEdit_Find.Text += e.KeyChar;
                else
                    if (e.KeyChar == '\b' && btnEdit_Find.Text.Length > 0) btnEdit_Find.Text = btnEdit_Find.Text.Substring(0, btnEdit_Find.Text.Length - 1);
        }

        #region Наличие файлов на диске
        /// <summary>
        /// Проверка наличия файлов на диске для списка
        /// </summary>
        public void Check_Exists_Data()
        {
            int RowsCount = dt_ListData.Rows.Count;
            if (RowsCount > 0)
            {
                FWorking.val_max = RowsCount;
                for (int i = 0; i < RowsCount; i++)
                {
                    Check_Exists_Row(i);
                    FWorking.val_min = i;
                }
                
                v_CheckTags = true;
                FWorking.val_min = FWorking.val_max;
            }
        }

        /// <summary>
        /// Проверка наличия файлов на диске для записи
        /// </summary>
        public void Check_Exists_Row(int RowHandle)
        {
            if (RowHandle > dt_ListData.Rows.Count - 1) return;

            // массив флагов: 0 или 1 [наличие обработки][есть mp3][есть текст]
            char[] ExFlag = {'0','0','0'};
            DataRow Drow = dt_ListData.Rows[RowHandle];

            // наличие муз файла
            if (File.Exists(Drow["MuzFile"].ToString())) 
            { 
                ExFlag[1] = '1'; 

                // Читаем Тэги
                if (v_CheckTags)
                {
                    Tag_File = TagLib.File.Create(Drow["MuzFile"].ToString());
                    Drow["TagComments"] = Form_TagEditor.to_UTF8(Tag_File.Tag.Comment); // Тег Комментария                 
                }
            }

            // наличие текстового файла
            if (File.Exists(Drow["TextFile"].ToString())) { ExFlag[2] = '1'; }

            // проводилась обработка
            ExFlag[0] = '1';

            Drow["FileExistsFlag"] = ExFlag[0].ToString() + ExFlag[1].ToString() + ExFlag[2].ToString();            
        }

        #endregion

        // Закрытие списка
        public void PL_CloseList()
        {
            Init_Table_Data();
            pl_FilePath = "";
            is_Changed = false;
        }

        // Очистка списка
        public void PL_ClearList()
        {
            Init_Table_Data();            
            is_Changed = true;
        }

        // набор данных для сохранения в формате PM
        private string[] PL_GetSaveData_PM()
        {
            string[] Data = new string[dt_ListData.Rows.Count];

            if (dt_ListData.Rows.Count > 0)
            {
                for (int i = 0; i < dt_ListData.Rows.Count; i++)
                {
                    Data[i] += dt_ListData.Rows[i][0] + "|";
                    Data[i] += dt_ListData.Rows[i][1] + "|";
                    Data[i] += dt_ListData.Rows[i][2] + "|";
                    Data[i] += dt_ListData.Rows[i][3] + "|";
                    Data[i] += dt_ListData.Rows[i][4] + "|";
                    Data[i] += dt_ListData.Rows[i][5] + "|";
                    Data[i] += dt_ListData.Rows[i][6] + "|";
                    Data[i] += dt_ListData.Rows[i][7];

                    //dt_FileData
                }
            }

            return Data;
        }

        // Сохранение в указанный файл
        public bool PL_SaveDataAs(string FilePath)
        {
            if (FilePath == "") return false;

            Loader_FileList FLoader = new Loader_FileList();

            string[] Data = PL_GetSaveData_PM();

            if (FLoader.Save_To_File(FilePath, Data))
            {
                is_Changed = false;
                return true;
            }

            return false;
        }

        // Сохранение в текущий файл
        public bool PL_SaveData()
        {
            return PL_SaveDataAs(pl_FilePath);
        }

        public void PL_DelTrack(int RowHandle)
        {
            if (RowHandle > dt_ListData.Rows.Count - 1) return;

            dt_ListData.Rows[RowHandle].Delete();
            dt_FileData.Rows[RowHandle].Delete();
            is_Changed = true;
        }

        // Удаление текста
        public void PL_DelText(int RowHandle)
        {
            if (RowHandle > dt_ListData.Rows.Count - 1 || RowHandle < 0) return;

            dt_ListData.Rows[RowHandle]["TextFile"] = "";
            dt_FileData.Rows[RowHandle]["TextFile"] = "";    

            Check_Exists_Row(RowHandle);
            is_Changed = true;
        }

        // Добавление текста
        public void PL_AddText(int RowHandle, String FilePath)
        {
            if (RowHandle > dt_ListData.Rows.Count - 1 || RowHandle < 0) return;

            dt_ListData.Rows[RowHandle]["TextFile"] = FilePath;
            dt_FileData.Rows[RowHandle]["TextFile"] = FilePath;

            Check_Exists_Row(RowHandle);
            is_Changed = true;
        }

        // Добавление муз файла
        public void PL_AddMuz(String FilePath)
        {
            DataRow DrowF = dt_FileData.NewRow();
            DrowF["MuzFile"] = FilePath;
            DrowF["EqLines"] = "0:0:0:0:0:0:0:0:0:0:0:";
            DrowF["EqPreamp"] = "0";
            DrowF["Volume"] = "5";
            DrowF["LRBalance"] = "10";
            dt_FileData.Rows.Add(DrowF);


            DataRow DrowL = dt_ListData.NewRow();
            DrowL["MuzFile"] = FilePath;
            DrowL["EqLines"] = "0:0:0:0:0:0:0:0:0:0:0:";
            DrowL["EqPreamp"] = "0";
            DrowL["Volume"] = "5";
            DrowF["LRBalance"] = "10";
            DrowL["Num"] = dt_ListData.Rows.Count + 1; ;
            DrowL["FileExistsFlag"] = "000";

            string fname = System.IO.Path.GetFileNameWithoutExtension(FilePath);

            fname = fname.Replace("  ", " ");
            fname = fname.Replace("ё", "е");
            fname = fname.Replace("( ", "(");
            fname = fname.Replace("(-", "[[");

            int pos = fname.IndexOf('-');

            if (pos == -1)
            {
                string track_all = fname.Trim().ToUpper();
                DrowL["Name"] = track_all;
                DrowL["Artist"] = track_all;
            }
            else
            {
                string track_name = fname.Substring(0, pos).Trim().ToUpper();
                string track_artist = fname.Substring(pos + 1, fname.Length - pos - 1).Replace('-', ' ').Trim().ToUpper();

                DrowL["Name"] = track_name + " - " + track_artist;
                DrowL["Artist"] = track_artist + " - " + track_name;
            }

            dt_ListData.Rows.Add(DrowL);

            Check_Exists_Row(dt_ListData.Rows.Count - 1);
            is_Changed = true;
        }

        private void gv_PlayList_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            tooltip_started = true;
            toolTipController.HideHint();
        }

        private void checkButton_Toggle_FindPanel_CheckedChanged(object sender, EventArgs e)
        {
            Toggle_Show_Filter();
        }

    }
}
