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
using System.Xml;
using System.Threading;

namespace PPlayer
{    
    // класс списка плейлиста
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
                    try 
                    { 
                        labelControl_header.Text = tName;
                        labelControl_header.ToolTip = value;
                    }
                    catch (Exception) { }                                            
                    pl_FolderPath = Path.GetDirectoryName(value);
                }
                else
                {
                    try 
                    {
                        labelControl_header.Text = "Новый плейлист";
                        labelControl_header.ToolTip = "";
                    }
                    catch (Exception) { }   
                }
            }
        }

        private bool _pl_History = false; // Список в формате истории
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

        public bool _pl_HotList = false; // горячий список
        public bool pl_HotList
        {
          get
          {
            return _pl_HotList;
          }
          set
          {
            if (_pl_HotList == value) return;
            _pl_HotList = value;

            if (_pl_HotList)
            {
                // отключаем сортировку в горячем списке
                gridColumn_name.SortOrder = DevExpress.Data.ColumnSortOrder.None;
                gridColumn_artist.SortOrder = DevExpress.Data.ColumnSortOrder.None;
                gv_PlayList.OptionsCustomization.AllowSort = false;
            }
          }
        }                              

        public DataTable dt_FileData;
        public DataTable dt_ListData;
        public GridViewFilterHelper filterHelper;
        public string pl_FolderPath;
        public int v_PList_ID = 0;  // номер списка

        // константы
        public int tooltip_hide_ms = 2000; // кол-во секунд, которые держится тултип
        public int tooltip_wait_ms = 500; // кол-во мс (мили секунд, перед появлением тултипа)
        public int search_clear_ms = 1500; // кол-во секунд, которые держится тултип
        private int timer_ticks_count = 5;

        public DateTime tooltip_hide_dt;
        public DateTime tooltip_wait_dt;
        public DateTime search_clear_dt;
        public bool tooltip_started = true;

        private int OldRow = 0;
        private int timer_ticks = 5;         // колво миганий строки при переносе в hotlist
        private int timer_type = 0;
        private Color row_color_default;     // цвет мигания ok/error
        private Color row_color_blink;
        private Color row_color_err_blink;        
 
        private string tooltip_text = "";     // подсказка для трека
        private int Row_Handle = -1;          // последняя выделенная запись
        private int Row_Handle_ManualSort = -1; // ручная сотрировка
        private int Row_Count_ManualSort = 0; // ручная сотрировка
        private MouseEventArgs e_last;
        private bool is_scroll = false;       // актиен скролл
        private bool list_is_focused = false; // активный лист
        private bool find_is_focused = false; // активна форма поиска
        private TagLib.File Tag_File;         // Тэги в файле
        public bool v_Check_Tags = false;      // проверять тэги в файлах   
        public bool v_Check_Exist = true;      // проверять наличие файлов на диске        
        
        public Change_History PLog_history = new Change_History(); // История изменений
        private bool _is_Changed = false;    // Наличие изменений
        public bool is_Changed
        {
            get
            {
                return _is_Changed;
            }
            set
            {
                _is_Changed = value;
                if (value == false)
                {
                    PLog_history.clear();
                    Row_Count_ManualSort = 0;
                }                
            }
        }

        public Working FWorking;
        public Form_History FChangeLog;
        private Loader_FileList FLoader = new Loader_FileList(); // загрузчик файлов         
        public Thread FW_Thread;        

        // Инициализация класса
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
            if (e == null)
            {
                return; // ошибка при скроле в другой области при активном плейлисте
            }

            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hti = gv_PlayList.CalcHitInfo(e.X, e.Y);

            if (e.X > gv_PlayList.ViewRect.Width || // выход за края справа
                e.X < gv_PlayList.ViewRect.Width * 0.5) // не показывать подсказку на первой половине трека, только на второй //e.X <= 0
            {
                if (timer_tooltip.Tag == null || timer_tooltip.Tag == "") toolTipController.HideHint();
                return;                
            }

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
            #region Старая логика
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
            
            #endregion

            if (DateTime.Now > tooltip_wait_dt && DateTime.Now < tooltip_hide_dt && !tooltip_started)
            {
                toolTipController.ShowHint(tooltip_text);                
                tooltip_started = true;
            }

            if (tooltip_hide_dt < DateTime.Now)
            {
                timer_tooltip.Stop();
                toolTipController.HideHint();
                timer_tooltip.Tag = "";
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
            timer_tooltip.Stop();
            toolTipController.HideHint();
        }

        #endregion

        #region Поиск (фильтр)

        // обработчик текста
        private char key_update(char ch)
        {
            switch (ch)
            {
                case 'ё': ch='е'; break;
                case 'Ё': ch = 'Е'; break;
                default: break;
            }

            return ch;
        }

        // активация фильтрации на списке
        public void gv_PlayList_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (search_clear_dt < DateTime.Now) btnEdit_Find.Text = "";// очистка поиска (после интервала ожидания)            

            if (!char.IsControl(e.KeyChar))            
                btnEdit_Find.Text += key_update(e.KeyChar);            
            else
                if (e.KeyChar == '\b' && btnEdit_Find.Text.Length > 0) // бэкспейс - очистка последнего символа
                    btnEdit_Find.Text = btnEdit_Find.Text.Substring(0, btnEdit_Find.Text.Length - 1);
                else
                    if (e.KeyChar == 27 && btnEdit_Find.Text.Length > 0) // эскейп - очистка всего поиска
                        btnEdit_Find.Text = "";

            if (!char.IsControl(e.KeyChar) || e.KeyChar == 8 || e.KeyChar == 27)
            if (btnEdit_Find.Text == "" && panelControl_Filter.Visible)
            {
                checkButton_Toggle_FindPanel.Checked = false;
                panelControl_Filter.Visible = false;
            }
            else
            {
                checkButton_Toggle_FindPanel.Checked = true;
                panelControl_Filter.Visible = true;
            }

            btnEdit_Find.DeselectAll();
        }

        private void btnEdit_Find_KeyPress(object sender, KeyPressEventArgs e)
        {
            // обработка управляющих клавиш
            if (char.IsControl(e.KeyChar))
	        {
                if (e.KeyChar == 27) btnEdit_Find.Text = ""; // эскейп - очистка всего поиска                
	        } 
            else
            {
                e.KeyChar = key_update(e.KeyChar);
            }
            
        }

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

            search_clear_dt = DateTime.Now.AddMilliseconds(search_clear_ms);            
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
                checkButton_Toggle_PLColumns.Text = "Н";
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

            if (color_type == 3) // поиск текущего трека
            {
                tooltip_text = "";
                color_type = 0;
            }
            else
            {
                tooltip_wait_dt = DateTime.Now.AddMilliseconds(tooltip_wait_ms - timer_tooltip.Interval);
                tooltip_hide_dt = tooltip_wait_dt.AddMilliseconds(tooltip_hide_ms);
                timer_tooltip.Tag = "NoHide";
                tooltip_started = false;
                timer_tooltip.Start();                
            }

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
            list_is_focused = true;
            gv_PlayList.Focus();
        }

        private void grid_PlayList_MouseClick(object sender, MouseEventArgs e)
        {
            if (!list_is_focused)
            {
                list_is_focused = true;
                gv_PlayList.Focus();
            }            
        }

        // Сортировка - Захвачен трек
        private void grid_PlayList_MouseDown(object sender, MouseEventArgs e)
        {
            if (pl_HotList && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e == null) return;
                
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hti = gv_PlayList.CalcHitInfo(e.X, e.Y);
                Row_Handle_ManualSort = hti.RowHandle;
            }
        }

        // Сортировка - Перетаскивание трека
        private void grid_PlayList_MouseMove(object sender, MouseEventArgs e)
        {
            if (e == null || Row_Handle_ManualSort < 0) return; // ошибка при скроле в другой области при активном плейлисте            

            if (e.X > gv_PlayList.ViewRect.Width ) {return;}

            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hti = gv_PlayList.CalcHitInfo(e.X, e.Y);

            grid_PlayList.Cursor = Cursors.NoMoveVert;
            //toolTipController1.ShowHint("CatchRow: " + Row_Handle_ManualSort.ToString()  + " CurRow: " + hti.RowHandle.ToString());

            if (hti.RowHandle >= 0 && (hti.RowHandle != Row_Handle_ManualSort)) // меняем строки местами
            {
                DT_Change_Rows_Pos(dt_ListData, Row_Handle_ManualSort, hti.RowHandle);
                DT_Change_Rows_Pos(dt_FileData, Row_Handle_ManualSort, hti.RowHandle);               

                gv_PlayList.FocusedRowHandle = hti.RowHandle;
                Row_Handle_ManualSort = hti.RowHandle; // новая позиция

                if (v_PList_ID != 0)
                {
                    is_Changed = true;
                    if (Row_Count_ManualSort == 0) PLog_history.add("Сортировка: \"изменение списка\"");
                }

                Row_Count_ManualSort++;
            } 
        }

        // Сортировка - фиксация перетаскивания трека
        private void grid_PlayList_MouseUp(object sender, MouseEventArgs e)
        {
            if (pl_HotList) // очистка ручной сортировки
            {
                Row_Handle_ManualSort = -1;
                grid_PlayList.Cursor = Cursors.Default;
            }
        }

        // потеря фокуса плейлистом
        private void gv_PlayList_LostFocus(object sender, EventArgs e)
        {
            list_is_focused = false;
            timer_tooltip.Stop();
            toolTipController.HideHint();

            if (pl_HotList)
            {
                Row_Handle_ManualSort = -1;
                grid_PlayList.Cursor = Cursors.Default;
            }
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

        #region Наличие файлов на диске
        /// <summary>
        /// Проверка наличия файлов на диске для списка
        /// </summary>
        public void Check_Exists_Data()
        {
            int RowsCount = dt_ListData.Rows.Count;
            if (RowsCount > 0 && v_Check_Exist)
            {
                FWorking.val_max = RowsCount;
                FWorking.Text = "Загрузка плейлиста: [" + v_PList_ID.ToString() + "] \""
                                                + Path.GetFileNameWithoutExtension(pl_FilePath) + "\"";                                
                
                if (v_Check_Tags) // с проверкой тэгов
                    FWorking.Text += "\nПроверка тэгов [" + dt_ListData.Rows.Count + " треков]";
                else
                    FWorking.Text += "\nПроверка наличия [" + dt_ListData.Rows.Count + " треков]";

                for (int i = 0; i < RowsCount; i++)
                {
                    Check_Exists_Row(i);
                    FWorking.val_cur = i;
                }
                                
                FWorking.val_cur = FWorking.val_max;
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
                if (v_Check_Tags)
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

        #region Управление списком
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
            PLog_history.add("Внимание!!! Список был [ОЧИЩЕН] (сохранить изменения в файл?)");
        }

        // набор данных для сохранения в формате PM
        private string[] PL_GetSaveData_PM()
        {
            string[] Data = new string[dt_ListData.Rows.Count];

            if (dt_ListData.Rows.Count > 0)
            {
                for (int i = 0; i < dt_ListData.Rows.Count; i++)
                {
                    Data[i] += dt_ListData.Rows[i][0] + "|"; // MuzFile   
                    Data[i] += dt_ListData.Rows[i][1] + "|"; // TextFile
                    Data[i] += (dt_ListData.Rows[i][2].ToString() == "" ? "0:0:0:0:0:0:0:0:0:0:0:" : dt_ListData.Rows[i][2]) + "|"; // EqLines
                    Data[i] += (dt_ListData.Rows[i][3].ToString() == "" ? "0" : dt_ListData.Rows[i][3]) + "|";  // EqPreamp
                    Data[i] += (dt_ListData.Rows[i][4].ToString() == "" ? "5" : dt_ListData.Rows[i][4]) + "|";  // Volume
                    Data[i] += (dt_ListData.Rows[i][5].ToString() == "" ? "10" : dt_ListData.Rows[i][5]) + "|"; // LRBalance
                    Data[i] += dt_ListData.Rows[i][6] + "|";
                    Data[i] += dt_ListData.Rows[i][7];

                    //dt_FileData 
                    /*
                    DrowL["MuzFile"] = FilePath;
                    DrowL["EqLines"] = "0:0:0:0:0:0:0:0:0:0:0:";
                    DrowL["EqPreamp"] = "0";
                    DrowL["Volume"] = "5";
                    DrowL["LRBalance"] = "10";
                    DrowL["Num"] = dt_ListData.Rows.Count + 1; ;
                    DrowL["FileExistsFlag"] = "000";
                    */
                }
            }

            return Data;
        }

        // Загрузка плей листа формата ПМ
        public bool PM_Load_List(string PL_file)
        {
            if (!PL_Save_Changes()) return false;

            if (PL_file.Length != 0 && Path.GetExtension(PL_file).ToLower() == ".pmp")
            {
                if (FLoader.Load_FileData(PL_file))
                {
                    #region Старый код
                    /*
                    string tName = Path.GetFileNameWithoutExtension(PMFilePath);
                    tName = tName[0].ToString().ToUpper() + tName.Substring(1, tName.Length - 1);

                    if (v_play_list_id == 0) xTabCtrl_PlayLists.TabPages[v_play_list_id].Text = "[" + v_Name_HotList + "] " + tName;
                    else xTabCtrl_PlayLists.TabPages[v_play_list_id].Text = "[" + (v_play_list_id) + "] " + tName;
                    */                    
                    #endregion

                    Init_Table_Data(); // инициализация нового списка                   

                    pl_FilePath = PL_file; // линк к файлу                    
                    
                    FWorking.Text = "Загрузка плейлиста: [" + v_PList_ID.ToString() + "] \""
                                                   + Path.GetFileNameWithoutExtension(pl_FilePath) + "\"";                                                                    
                    
                    string[] files_in_pl = FLoader.Data; // список в файле плейлиста

                    int i = 0;
                    int RowsCount = files_in_pl.Length;
                    FWorking.Text += "\nЗагрузка файлов [" + RowsCount + " треков]";
                    FWorking.val_max = RowsCount;

                    foreach (string s in files_in_pl)
                    {                        
                        PM_AddFileRow(dt_FileData, s);
                        PM_AddGridRow(dt_ListData, s);

                        i++;
                        FWorking.val_cur = i;
                    }

                    FWorking.val_cur = FWorking.val_max;
                }
                else
                {
                    XtraMessageBox.Show(FLoader.ResaultMsg, "Загрузка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Проверка наличия файлов
                if (v_Check_Exist) Check_Exists_Data();                               
            }
            else
            {
                XtraMessageBox.Show("Некорректный тип файла!", "Загрузка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        // Парсинг строки файла PM в строку таблицы
        private void PM_AddFileRow(DataTable PM, string s)
        {
            DataRow Drow = PM.NewRow();
            int i = 0;
            int pos = 0;

            while (s.Length != 0)
            {
                pos = s.IndexOf('|');
                if (pos >= 0)
                {
                    Drow[i] = s.Substring(0, pos);
                    s = s.Remove(0, pos + 1);
                    i++;
                }
                else
                {
                    Drow[i] = s;
                    s = "";
                }
            }
            PM.Rows.Add(Drow);
        }

        // Добавление строки в таблицу плейлист
        private void PM_AddGridRow(DataTable PL, string s)
        {
            DataRow Drow = PL.NewRow();
            int i = 0;
            int pos = 0;

            while (s.Length != 0)
            {
                pos = s.IndexOf('|');
                if (pos >= 0)
                {
                    Drow[i] = s.Substring(0, pos);
                    s = s.Remove(0, pos + 1);
                    i++;
                }
                else
                {
                    Drow[i] = s;
                    s = "";
                }
            }

            Drow["Num"] = PL.Rows.Count + 1;
            Drow["FileExistsFlag"] = "000";
            string fname = System.IO.Path.GetFileNameWithoutExtension(Drow[0].ToString());

            fname = fname.Replace("  ", " ");
            fname = fname.Replace("ё", "е");
            fname = fname.Replace("( ", "(");
            fname = fname.Replace("(-", "[[");

            pos = fname.IndexOf('-');

            if (pos == -1)
            {
                string track_all = fname.Trim().ToUpper();
                Drow["Name"] = track_all;
                Drow["Artist"] = track_all;
            }
            else
            {
                string track_name = fname.Substring(0, pos).Trim().ToUpper();
                string track_artist = fname.Substring(pos + 1, fname.Length - pos - 1).Replace('-', ' ').Trim().ToUpper();

                Drow["Name"] = track_name + " - " + track_artist;
                Drow["Artist"] = track_artist + " - " + track_name;
            }

            if (pos == -1)
            {
                string track_all = fname.Trim().Replace("[[", "(-").ToUpper();

                Drow["Name"] = track_all;
                Drow["Artist"] = track_all;
            }
            else
            {
                string track_name = fname.Substring(0, pos).Trim().ToUpper().Replace("[[", "(-");
                string track_artist = fname.Substring(pos + 1, fname.Length - pos - 1).Replace('-', ' ').Trim().ToUpper().Replace("[[", "(-");

                Drow["Name"] = track_name + " - " + track_artist;
                Drow["Artist"] = track_artist + " - " + track_name;
            }

            PL.Rows.Add(Drow);
        }

        // Замена строк местами - для сортировки
        private void DT_Change_Rows_Pos(DataTable DTable, int Pos, int PosNew)
        {
            // Копия записи
            DataRow Row = DTable.Rows[Pos];
            DataRow Row_New = DTable.NewRow();
            Row_New.ItemArray = Row.ItemArray;

            if (PosNew < Pos) // Перенос вверх - к началу списка
            {
                DTable.Rows.InsertAt(Row_New, PosNew);
                DTable.Rows.Remove(Row);
            }
            else // перенос к концу списка
            {
                DTable.Rows.Remove(Row);
                DTable.Rows.InsertAt(Row_New, PosNew);
            }
        }

        // Сохранение в указанный файл
        public bool PL_SaveDataAs(string FilePath)
        {
            if (FilePath == "" || FilePath == null) return false;

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

        // Удаление трека
        public void PL_DelTrack(int RowHandle)
        {
            if (RowHandle > dt_ListData.Rows.Count - 1) return;

            is_Changed = true;
            PLog_history.add("Муз [удален]: \"" + dt_ListData.Rows[RowHandle]["Name"] + "\"");

            dt_ListData.Rows[RowHandle].Delete();
            dt_FileData.Rows[RowHandle].Delete();            
        }

        // Удаление текста
        public void PL_DelText(int RowHandle)
        {
            if (RowHandle > dt_ListData.Rows.Count - 1 || RowHandle < 0 || dt_ListData.Rows[RowHandle]["TextFile"].ToString() == "") return;

            PLog_history.add("Текст [удален]: \"" + Path.GetFileName(dt_ListData.Rows[RowHandle]["TextFile"].ToString()) +
                               "\" для Муз \"" + dt_ListData.Rows[RowHandle]["Name"] + "\"");

            is_Changed = true;

            dt_ListData.Rows[RowHandle]["TextFile"] = "";
            dt_FileData.Rows[RowHandle]["TextFile"] = "";

            Check_Exists_Row(RowHandle);            
        }

        public void PL_Changed_EQ(int RowHandle)
        {
            if (RowHandle > dt_ListData.Rows.Count - 1 || RowHandle < 0) return;

            PLog_history.add("Экв [изменен]: муз \"" + dt_ListData.Rows[RowHandle]["Name"] + "\"");
            is_Changed = true;
        }

        // Добавление текста
        public void PL_AddText(int RowHandle, String FilePath)
        {
            if (RowHandle > dt_ListData.Rows.Count - 1 || RowHandle < 0) return;

            PLog_history.add("Текст [добавлен]: \"" + Path.GetFileName(FilePath) + "\"" +
                               "\" для Муз \"" + dt_ListData.Rows[RowHandle]["Name"] + "\"");
            is_Changed = true;

            dt_ListData.Rows[RowHandle]["TextFile"] = FilePath;
            dt_FileData.Rows[RowHandle]["TextFile"] = FilePath;

            Check_Exists_Row(RowHandle);            
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
            DrowL["LRBalance"] = "10";
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

            PLog_history.add("Муз [добавлен]: \"" + DrowL["Name"] + "\"");
            is_Changed = true;
        }

        public void PL_AddMuz_Folder(string Folder_Path)
        {
            string[] pathFiles;

            pathFiles = FLoader.GetFilesFromDir(Folder_Path, "*.mp3");
            foreach (string s in pathFiles) PL_AddMuz(s);

            pathFiles = FLoader.GetFilesFromDir(Folder_Path, "*.wav");
            foreach (string s in pathFiles) PL_AddMuz(s);
            
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
        
        // Драг файлов над списком (фильтр)
        private void grid_PlayList_DragEnter(object sender, DragEventArgs e)
        {
            // список файлов
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string ex;

            //int files_group = 0;
            int file_playlist = 0;
            int file_text = 0;
            int file_music = 0;
            int file_none = 0;
            int file_folder = 0;

            if (files == null) return;

            foreach (string file in files)
            {
                ex = Path.GetExtension(file).ToLower();
                if (!System.IO.File.Exists(file) && // нет файла
                    !System.IO.Directory.Exists(file + "\\")) // нет папки
                 return; // мало ли что закинуть пробуют                                

                if (System.IO.Directory.Exists(file + "\\")) // если папка
                    file_folder++;
                else
                    switch (ex)
                    {
                        case ".pmp": file_playlist++; break;
                        case ".mp3": file_music++; break;
                        case ".vaw": file_music++; break;
                        case ".rtf": file_text++; break;
                        case ".txt": file_text++; break;
                        //case ".doc": file_text++; break;
                        //case ".docx": file_text++; break;
                        default: file_none++; break;
                    }
            }

            if ((file_none != 0) // есть некорректные файлы
             || (file_playlist > 0 && files.Length != 1) // есть плейлист (больше одного или вместе с др. файлами)
             || (file_text > 1) // много текста (больше одного)
             || (file_music > 0 && file_music != files.Length) // есть музыка вместе с др файлами
             || (file_folder > 0 && file_folder != files.Length) // есть папка вместе c др файлами
               )
            { e.Effect = DragDropEffects.None; }
            else
            { e.Effect = DragDropEffects.Move; }

            //if (ex == "pm" && files.Length == 1) files_group = 1; // playlist
        }

        // DragDrop файлов в плейлист
        private void grid_PlayList_DragDrop(object sender, DragEventArgs e)
        {
            // список файлов
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string ex;

            if (files.Length == 0) return;

            foreach (string file in files)
            {
                ex = Path.GetExtension(file).ToLower();
                //if (!File.Exists(file)) return; // мало ли что закинуть пробуют (проверка в процедуре DragEnter)

                switch (ex)
                {
                    case ".pmp":
                        // !! Перенести обработчики в класс PL
                        if (!PL_Save_Changes()) return;
                        PL_CloseList();
                        
                        FWorking.Start();
                                                
                        PM_Load_List(file); // загрузка плей листа ПМ 
                        
                        FWorking.Abort();                        

                        this.Refresh(); 
                        break;
                    case ".mp3": PL_AddMuz(file); 
                        break;
                    case ".vaw": PL_AddMuz(file); 
                        break;
                    case ".rtf": PL_AddText(dt_ListData.Rows.IndexOf(gv_PlayList.GetFocusedDataRow()), file);
                        break;
                    case ".txt": PL_AddText(dt_ListData.Rows.IndexOf(gv_PlayList.GetFocusedDataRow()), file); 
                        break;
                    /*case ".doc": PL_AddText(dt_ListData.Rows.IndexOf(gv_PlayList.GetFocusedDataRow()), file);
                        break;
                    case ".docx": PL_AddText(dt_ListData.Rows.IndexOf(gv_PlayList.GetFocusedDataRow()), file);
                        break;     */                                                               
                    default:
                        if (System.IO.Directory.Exists(file + "\\")) PL_AddMuz_Folder(file); // если это папка                        
                        break;
                }
            }

        }           

        #endregion


        // Сохранение Плейлиста
        public bool PL_Save_Changes()
        {            
            if (is_Changed)
            {
                // название списка
                string playlist_name = "[" + v_PList_ID + "] " +
                                        (pl_FilePath != "" ?
                                        "\"" + Path.GetFileNameWithoutExtension(pl_FilePath) + "\"" :
                                        "\"Новый плейлист\"");

                FChangeLog = new Form_History();
                FChangeLog.v_list_name = playlist_name;
                FChangeLog.v_list_change_log = PLog_history.info();
                FChangeLog.v_init_dir = pl_FolderPath;
                FChangeLog.v_file_name = pl_FilePath;

                FChangeLog.v_resault = 0;

                FChangeLog.ShowDialog();

                // Если нет файла плейлиста, то сохраняем как (3)
                if (FChangeLog.v_resault == 2 && pl_FilePath == "") FChangeLog.v_resault = 3;

                switch (FChangeLog.v_resault)
                {
                    case 3: // сохранить как
                        if (PL_SaveDataAs(FChangeLog.v_file_name)) // если сохр успешно
                            pl_FilePath = FChangeLog.v_file_name;
                        else
                            return false;
                        break;
                    case 2: // сохранить
                        if (!PL_SaveData()) return false;// сохранить файл
                        break;
                    case 1: // без сохранения
                        break;
                    default: return false; // тут cancel - отмена  
                }                
            }

            return true;
        }

        public void PM_Load_Design(MySettings CurSettings)
        {
            gv_PlayList.Appearance.Row.Font = new Font(CurSettings.p_PL_FontName, // тип шрифта
                                                       CurSettings.p_PL_FontSize, // Размер шрифта
                                                      (CurSettings.p_PL_FontBold ? FontStyle.Bold : FontStyle.Regular)); // жирный

            gv_PlayList.Appearance.FocusedRow.Font = gv_PlayList.Appearance.Row.Font;

            // загрузка цветов из настроек
            gv_PlayList.Appearance.Empty.BackColor = Color.FromArgb(CurSettings.p_PL_FontColor_back); // цвет - фон списка
            gv_PlayList.Appearance.FocusedRow.BackColor = Color.FromArgb(CurSettings.p_PL_FontColor_back_select); // фон выделение        
            gv_PlayList.Appearance.Row.ForeColor = Color.FromArgb(CurSettings.p_PL_FontColor_text); // цвет текста
            gv_PlayList.Appearance.FocusedRow.ForeColor = Color.FromArgb(CurSettings.p_PL_FontColor_text_select); // цвет текста - выделение
            gv_PlayList.FormatConditions[0].Appearance.ForeColor = Color.FromArgb(CurSettings.p_PL_FontColor_text_no_mp3); // цвет текста - нет mp3
            gv_PlayList.FormatConditions[1].Appearance.ForeColor = Color.FromArgb(CurSettings.p_PL_FontColor_text_no_mp3); // цвет текста - нет mp3
            gv_PlayList.FormatConditions[2].Appearance.ForeColor = Color.FromArgb(CurSettings.p_PL_FontColor_text_no_rtf); // цвет текста - нет rtf

            row_color_default = gv_PlayList.Appearance.FocusedRow.BackColor;
        }

    }

    // Класс - История изменений списка
    public class Change_History
    {
        string[] hist = new string[2000]; // список изменений
        int rows = 0; // кол-во строк истории

        public void add(string info)
        {
            hist[rows] = info;
            rows++;                 
        }


        public string info()
        {
            string inf = "";

            for (int i = 0; i < rows; i++)
            {
                inf += "* " + hist[i] + "\r\n";
            }

            return inf;
        }

        public void clear()
        {
            hist = new string[2000];
            rows = 0;
        }
    }
}
