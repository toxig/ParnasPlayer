using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using Un4seen.Bass;

namespace PPlayer
{

    public partial class FrmMain : XtraForm
    {
        private bool fl_eqLoading = false;
        internal bool? initDefaultDevice;
        internal int stream;
        Random r = new Random();
        bool manualStopped = false;
        int play_list_id = 0; // Активный плейлист
        int Sroll_lines = 12;
        int play_id;
        int fxHandle;
        FormView FView = new FormView();
        Loader_FileList FLoader = new Loader_FileList();
        string PlaingFileName;
                
        // const
        BASS_DX8_PARAMEQ eq = new BASS_DX8_PARAMEQ();  // переменная эквалайзера Bass.dll
        float EQBandwidth = 7f; // Пропускная способность полутонов, 1 .. 36 def 18 
        int[] EQfGain = new int[17]; // Состояние эквалайзера
        float[] EQCenter =  {0, // частотные полосы эквалайзера
                             80,   125,   175,   250, 
                             350,  500,   700,   1000, 
                             1400, 2000,  2800,  4000, 
                             5600, 8000, 11200, 16000};

        PlayListControl[] All_PlayLists = new PlayListControl[10];
        int WaytDelay = 0;
        
        public FrmMain()
        {
            InitializeComponent();

            // registering BASS.NET API
            BassNet.Registration("toxig@ya.ru", "2X182415113438");

            if (initDefaultDevice.HasValue == false)
            {
                initDefaultDevice = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                pbc_equal_main.Position = int.Parse(Math.Truncate((Bass.BASS_GetVolume() * 100)).ToString());
            }

            // Таблицы для плейлистов
            Init_PlayLists();

            // Настройки Эквалайзера
            Eq_array_Save();
            // Настройки отображения окна
            FView_Save();                                                 

            panelControl_EQ.Visible = false;
            panelControl_PlayBack.Visible = false;            
        }

        #region Инициализация элементов
        private void Init_PlayLists()
        {            
            for (int i = 0; i < xTabCtrl_PlayLists.TabPages.Count; i++)
            {
                Init_New_PlayList(i);
            }
        }

        // yjdsq gktq kbcn
        private void Init_New_PlayList(int i)
        {
            All_PlayLists[i] = new PlayListControl();            
            All_PlayLists[i].Dock = DockStyle.Fill;

            xTabCtrl_PlayLists.TabPages[i].Controls.Add(All_PlayLists[i]);
            if (i != 0) xTabCtrl_PlayLists.TabPages[i].Text = "[" + (i) + "]";
            else xTabCtrl_PlayLists.TabPages[i].Text = "[ГОРЯЧИЙ]";
            All_PlayLists[i].grid_PlayList.DoubleClick += new System.EventHandler(this.grid_PlayList_DoubleClick);
        }

        // Поиск таблицы по номеру
        private Object Find_TabGV(int id)
        {
            //name = name.ToLower();
            foreach (Control c in xTabCtrl_PlayLists.TabPages[id].Controls) //assuming this is a Form
            {
                //if (c.GetType(). Name.ToLower() == name)
                    return c; //found                 
            }
            return null; //not found
        }

        #endregion

        #region Операции с файлом плейлиста
        // Загрузка файлов из папки
        private void File_Load_Dir(string DirPath)
        {
            //comboBoxPlaybackOrder.SelectedIndex = 0;
            if (initDefaultDevice.HasValue == false)
            {
                initDefaultDevice = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                pbc_equal_main.Position = int.Parse(Math.Truncate((Bass.BASS_GetVolume() * 100)).ToString());
            }
            try
            {
                if (DirPath.Length != 0)
                {                                           
                    xTabCtrl_PlayLists.SelectedTabPage.Text = DirPath.Substring(DirPath.LastIndexOf('\\') + 1, DirPath.Length - DirPath.LastIndexOf('\\') - 1);
                    xTabCtrl_PlayLists.SelectedTabPage.Tag = "DirFiles";

                    string[] pathFiles = FLoader.GetAllFilesFromDirectory(DirPath, "*.mp3");
                    foreach (string s in pathFiles)
                    {
                        //PM_AddGridRow(DT_PlayList1, s);
                    }
                }
            }
            catch (NullReferenceException)
            {
                System.IO.File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\KPlayerOptions.ini", new string[] { string.Empty });
            }            
        }

        // Загрузка плейлиста
        private void File_Load_PM_List(string PMFilePath)
        {          
            if (PMFilePath.Length != 0)
            {
                if (FLoader.Load_FileData(PMFilePath))
                {                    
                    string tName = Path.GetFileNameWithoutExtension(PMFilePath);
                    tName = tName[0].ToString().ToUpper() + tName.Substring(1, tName.Length - 1);
                    xTabCtrl_PlayLists.TabPages[play_list_id].Text = "[" + (play_list_id + 1) + "] " + tName;

                    All_PlayLists[play_list_id].Init_Table_Data();                    

                    string[] pathFiles = FLoader.Data; // список в файле плейлиста
                    foreach (string s in pathFiles)
                    {
                        PM_AddFileRow(All_PlayLists[play_list_id].dt_FileData, s);
                        PM_AddGridRow(All_PlayLists[play_list_id].dt_ListData, s);
                    }                                                                                
                }
                else
                {
                    XtraMessageBox.Show(FLoader.ResaultMsg, "Загрузка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
            string fname = System.IO.Path.GetFileNameWithoutExtension(Drow[0].ToString());
            
            fname = fname.Replace("  ", " ");
            fname = fname.Replace("ё", "е");
            fname = fname.Replace("( ", "(");            

            pos = fname.IndexOf('-');

            if (pos == -1)
            {
                Drow["Name"] = fname.Trim().ToUpper();
            }
            else
            {
                Drow["Name"] = fname.Substring(0, pos).Trim().ToUpper();
                Drow["Artist"] = fname.Substring(pos + 1, fname.Length - pos - 1).Replace('-', ' ').Trim().ToUpper();
            }
                        

            PL.Rows.Add(Drow);
        }        

        #endregion

        #region Панель управления Play/Stop

        // ПЛЕЙ
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            PlayCurFile_FromList(play_list_id);
        }

        // СТОП
        private void buttonStop_Click(object sender, EventArgs e)
        {
            Bass.BASS_StreamFree(stream);
            Label_InfoLine.Text = "Остановлено"; WaytDelay = 3;
            manualStopped = true;

            Update_Paly_Status();
        }

        // ВПЕРЕД
        private void buttonNext_Click(object source, EventArgs e)
        {      
            /*
            if (listBoxPlayList.SelectedIndex != listBoxPlayList.Items.Count - 1)
            {
                listBoxPlayList.SetSelected(listBoxPlayList.SelectedIndex + 1, true);
            }
            else
            {
                listBoxPlayList.SetSelected(0, true);
            }
            */

            Update_Paly_Status();

            buttonPlay_Click(buttonPlay, null);
        }

        // НАЗАД
        private void buttonPrev_Click(object sender, EventArgs e)
        {
            /*
            if (listBoxPlayList.SelectedIndex != 0)
            {
                listBoxPlayList.SetSelected(listBoxPlayList.SelectedIndex - 1, true);
            }
            else
            {
                listBoxPlayList.SetSelected(listBoxPlayList.Items.Count - 1, true);
            }*/

            Update_Paly_Status();
            buttonPlay_Click(buttonPlay, null);
        }

        // ПАУЗА
        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {                
                Bass.BASS_Pause();
                Label_InfoLine.Text = "Пауза";
            }
            else
            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PAUSED)
            {
                Bass.BASS_Start(); 
                Label_InfoLine.Text = "Проигрывается";
            }

            Update_Paly_Status();
        }
        
        #endregion

        #region Трекеры / ползунки

        // Трек Время - переход на указанную точку
        private void progressBarControl_PlayPosition_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && stream != 0)
            {
                WaytDelay = 2;

                int pos = (int)((ProgressBarControl)sender).Properties.Maximum * (e.X + 3) / ((ProgressBarControl)sender).Width;
                int max = ((ProgressBarControl)sender).Properties.Maximum;
                int min = ((ProgressBarControl)sender).Properties.Minimum;
                if (pos > max) { pos = max; }
                if (pos < min) { pos = min; }

                ((ProgressBarControl)sender).Position = pos;
                Bass.BASS_ChannelSetPosition(stream, (double)((ProgressBarControl)sender).Position);
                
                timerForCurrentPosition_Tick(null, null);
            }
        }

        // Трек Время - Выбор точки
        private void progressBarControl_PlayPosition_MouseMove(object sender, MouseEventArgs e)
        {            
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                int pos = (int)((ProgressBarControl)sender).Properties.Maximum * (e.X + 3) / ((ProgressBarControl)sender).Width;
                int max = ((ProgressBarControl)sender).Properties.Maximum;
                int min = ((ProgressBarControl)sender).Properties.Minimum;
                if (pos > max) { pos = max; }
                if (pos < min) { pos = min; }

                WaytDelay = 10;
                Label_InfoLine.Text = "Позиция [" + (SecondsToTimeFormat(pos) + " (-" + SecondsToTimeFormat(max - pos) + ") / ") + SecondsToTimeFormat(max) + "]";
            }
        }

        // Изменение громкости
        private void pbc_Volume_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {                
                int max = ((ProgressBarControl)sender).Properties.Maximum;
                int min = ((ProgressBarControl)sender).Properties.Minimum;
                int pos = ((ProgressBarControl)sender).Position;

                pos = max * (e.Y) / ((ProgressBarControl)sender).Height;

                if (pos > max) { pos = max; }
                if (pos < min) { pos = min; }

                pos = max - pos;

                ((ProgressBarControl)sender).Position = pos;

                string tt_str = "Громкость [" + pos + "%]";

                WaytDelay = 3;
                Label_InfoLine.Text = tt_str;

                ((ProgressBarControl)sender).ToolTip = tt_str;
                toolTipController.ShowHint(tt_str);
            }

        }

        // Громкость изменена
        private void pbc_Volume_Value_Changed(object sender, EventArgs e)
        {
            int pos = (int)((ProgressBarControl)sender).Position;
            int max = ((ProgressBarControl)sender).Properties.Maximum;                        
            
            Bass.BASS_SetVolume((float)pos / max);            
        }
                
        #endregion

        #region Список воспроизведения

        //Запуск воспроизведения
        public void PlayCurFile_FromList(int list_id)
        {
            int Row_ID = Convert.ToInt32(All_PlayLists[play_list_id].gv_PlayList.GetFocusedDataRow()["Num"]);

            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PAUSED && play_id == Row_ID)
            {
                Bass.BASS_Start();
                Label_InfoLine.Text = "Проигрывается";
            }
            else
                try
                {
                    string FileName = "";
                    string TextFileName = "";

                    // DataSet
                    {                        
                        FileName = All_PlayLists[play_list_id].dt_FileData.Rows[Row_ID - 1][0].ToString();
                        TextFileName = All_PlayLists[play_list_id].dt_FileData.Rows[Row_ID - 1][1].ToString();
                        play_id = Row_ID;
                    }
                    
                    // Загрузка текста
                    if (System.IO.File.Exists(TextFileName)) 
                    { 
                        RTBox_TextFile.LoadFile(TextFileName);                       
                    }
                    else
                    {
                        WaytDelay = 3;
                        Label_InfoLine.Text = "Нет RTF файла: " + Path.GetFileName(TextFileName);
                    }

                    // Загрузка музыки
                    if (System.IO.File.Exists(FileName)) 
                    {
                        PlaingFileName = Path.GetFileName(FileName);
                        PlayFile(FileName);

                        lbc_playlist.Text = (xTabCtrl_PlayLists.SelectedTabPageIndex + 1).ToString();
                    }
                    else Label_InfoLine.Text = "Нет файла: " + Path.GetFileName(FileName);                    
                }
                catch (NullReferenceException)
                {
                    
                }
                catch (Exception exptn)
                {
                    MessageBox.Show(exptn.Message);
                }

            manualStopped = false;
            Update_Paly_Status();
        }

        //Запуск воспроизведения
        public void PlayFile(string fileName)
        {
            if (initDefaultDevice.Value)
            {
                if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
                { // (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING || Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PAUSED)
                    Bass.BASS_StreamFree(stream);
                }

                stream = Bass.BASS_StreamCreateFile(fileName, 0, 0, BASSFlag.BASS_DEFAULT);
                fxHandle = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);

                if (stream != 0)
                {
                    Bass.BASS_ChannelPlay(stream, false);
                    timerForCurrentPosition_Tick(null, null);
                }
                else
                {
                    MessageBox.Show(String.Format("Error {0}", Bass.BASS_ErrorGetCode()));
                }
            }

            timer_playng.Enabled = false;

            Label_InfoLine.Text = "Проигрывается";

            string[] tagID3V1 = Bass.BASS_ChannelGetTagsID3V1(stream);
            string[] tagID3V2 = Bass.BASS_ChannelGetTagsID3V2(stream);

            progressBarControl_PlayPosition.Properties.Minimum = 0;
            progressBarControl_PlayPosition.Properties.Maximum = int.Parse(((Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream)).ToString().Split(',', '.'))[0]));

            timerForCurrentPosition_Tick(null, null);
            timer_playng.Enabled = true;
            manualStopped = false;
        }

        // играть выбранную строку
        private void grid_PlayList_DoubleClick(object sender, EventArgs e)
        {
            if (All_PlayLists[play_list_id].dt_ListData != null && All_PlayLists[play_list_id].dt_ListData.Rows.Count != 0) 
                buttonPlay_Click(null, null);
        }

        // играть выбранную строку
        private void listBoxPlayList_DoubleClick(object sender, EventArgs e)
        {
            //PlayFile(((FileInListBox)listBoxPlayList.Items[listBoxPlayList.SelectedIndex]).FileName);
            buttonPlay_Click(null, null);
        }

        // очистка списка
        private void buttonClearList_Click(object sender, EventArgs e)
        {
            ClearList(ActListID());
            xtraTabPage1.Text = "[1]";
        }

        private int ActListID()
        {
            return xTabCtrl_PlayLists.SelectedTabPageIndex;
        }

        private void ClearList(int ListID)
        {
            //ListID
        }

        // Открыть папку с файлами
        private void im_OpenDir_ItemClick(object sender, ItemClickEventArgs e)
        {
            folderBrowserDialog.ShowDialog();
            Storage.path = folderBrowserDialog.SelectedPath;
            File_Load_Dir(Storage.path);
        }

        private void iOpen_PlayList_ItemClick(object sender, ItemClickEventArgs e)
        {
            openFileDialog.Filter = "Плейлист (*.pmp)|*.pmp|Все файлы (*.*)|*.*";
            openFileDialog.ShowDialog();
            //Storage.path = folderBrowserDialog.SelectedPath;

            if (openFileDialog.FileName.Length != 0)
            {
                File_Load_PM_List(openFileDialog.FileName);
            }
        }
        private void No_Func()
        {
            XtraMessageBox.Show("Данная функция в стадии разработки!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
        
        #region Таймеры

        private void timerForCurrentPosition_Tick(object sender, EventArgs e)
        {
            if (initDefaultDevice.HasValue != false && stream != 0)
            {
                //string sb = string.Empty;
                //sb = Bass.BASS_ChannelGetPosition(stream).ToString();                

                Update_Paly_Status();

                //this.Text = "KPlayer - " + SecondsToTimeFormat(d) + @"/" + SecondsToTimeFormat(all);                

                if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_STOPPED && !manualStopped) buttonNext_Click(null, null);

            }
        }

        private void Update_Paly_Status()
        {
            int d = int.Parse(((Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream)).ToString().Split(',', '.'))[0]));
            int all = int.Parse(((Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream)).ToString().Split(',', '.'))[0]));

            if (d < 0) d = 0;
            if (all < 0) all = 0;
            if (d > all) d = all;

            progressBarControl_PlayPosition.Position = d;
            progressBarControl_PlayPosition.Properties.DisplayFormat.FormatString = StrToEscapeStr("- "+SecondsToTimeFormat(all-d));
            lbc_time_back.Text = "- " + SecondsToTimeFormat(all - d);

            lbc_time.Text = SecondsToTimeFormat(d);
            if (PlaingFileName != null && PlaingFileName.Length > 0 && WaytDelay == 0) Label_InfoLine.Text = PlaingFileName;

            switch (Bass.BASS_ChannelIsActive(stream))
            {
                case BASSActive.BASS_ACTIVE_PAUSED: lbc_curstatus.Text = "PAUSE";
                    break;
                case BASSActive.BASS_ACTIVE_PLAYING: lbc_curstatus.Text = "PLAY";
                    break;
                case BASSActive.BASS_ACTIVE_STALLED: lbc_curstatus.Text = "STOP";
                    break;
                case BASSActive.BASS_ACTIVE_STOPPED: lbc_curstatus.Text = "STOP";
                    break;
                default: lbc_curstatus.Text = "";
                    break;
            }            

            //lbc_timetick.Text = d + " s";
            //lbc_timedown.Text = "-" + SecondsToTimeFormat(all - d);
            //Label_InfoLine.Text = "Позиция [" + (SecondsToTimeFormat(pos) + " (-" + SecondsToTimeFormat(max - pos) + ") / ") + SecondsToTimeFormat(max) + "]";

            RTBox_TextFile_Scroll((double)d / all);

            if (WaytDelay != 0) WaytDelay--;
        }
        #endregion

        #region Модификаторы данных

        private string SecondsToTimeFormat(int time)
        {
            int hours = (time - (time % (60 * 60))) / (60 * 60);
            int minutes = (time - time % 60) / 60 - hours * 60;
            int seconds = time - hours * 60 * 60 - minutes * 60;

            string h = hours.ToString();
            if (hours == 0) { h = ""; }
            else
            {
                if (h.Length == 1) { h = "0" + h; }
                h = h + ":";
            }

            string m = minutes.ToString();
            if (m.Length == 1)
            {
                m = "0" + m;
            }
            m = m + ":";

            string s = seconds.ToString();
            if (s.Length == 1)
            {
                s = "0" + s;
            }

            return (h + m + s);
        }

        private string StrToEscapeStr(string str)
        {
            string ret = "";

            foreach (char s in str)
            {
                ret = ret + "\\" + s;
            }

            return ret;
        }

        // Закрытие программы
        private void FormMain_Closing(object sender, FormClosingEventArgs e)
        {
            Bass.BASS_Free();
            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\KPlayerOptions.ini", Storage.path);
        }

        // Поиск элемента по имени
        private Object FindEqBarByName(string name)
        {
            name = name.ToLower();
            foreach (Control c in panelControl_EQ.Controls) //assuming this is a Form
            {                
                if (c.Name.ToLower() == name) 
                    return c; //found                 
            }
            return null; //not found
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

        private void Load_Data_To_Grid()
        {

        }
        #endregion        

        #region Эквалайзер

        // изменение уровней
        private void pbc_equal_main_MouseMove(object sender, MouseEventArgs e)
        {
            int max = ((ProgressBarControl)sender).Properties.Maximum;
            int min = ((ProgressBarControl)sender).Properties.Minimum;
            int pos = ((ProgressBarControl)sender).Position;

            #region Одиночное изменение уровня
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {   
                pos = max * (e.Y) / ((ProgressBarControl)sender).Height;

                if (pos > max) { pos = max; }
                if (pos < min) { pos = min; }

                pos = max - pos;

                ((ProgressBarControl)sender).Position = pos;
            } 
            #endregion

            #region Групповое изменение уровней
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                int dx_pos = pos;

                ((ProgressBarControl)sender).Cursor = Cursors.NoMoveVert;

                pos = max * (e.Y) / ((ProgressBarControl)sender).Height;

                if (pos > max) { pos = max; }
                if (pos < min) { pos = min; }

                pos = max - pos;

                dx_pos = pos - dx_pos;

                ((ProgressBarControl)sender).Position = pos;

                #region Группа уровней
                if (((ProgressBarControl)sender).Tag != null)
                {
                    string eq_name = ((ProgressBarControl)sender).Name.ToString();

                    if (eq_name == "pbc_eq16" || eq_name == "pbc_eq16" || eq_name == "pbc_eq15" || eq_name == "pbc_eq14" || eq_name == "pbc_eq13")
                    {
                        if (pbc_eq16.Name != eq_name) pbc_eq16.Position += dx_pos;
                        if (pbc_eq15.Name != eq_name) pbc_eq15.Position += dx_pos;
                        if (pbc_eq14.Name != eq_name) pbc_eq14.Position += dx_pos;
                        if (pbc_eq13.Name != eq_name) pbc_eq13.Position += dx_pos;
                    }

                    if (eq_name == "pbc_eq12" || eq_name == "pbc_eq11" || eq_name == "pbc_eq10" || eq_name == "pbc_eq9")
                    {
                        if (pbc_eq12.Name != eq_name) pbc_eq12.Position += dx_pos;
                        if (pbc_eq11.Name != eq_name) pbc_eq11.Position += dx_pos;
                        if (pbc_eq10.Name != eq_name) pbc_eq10.Position += dx_pos;
                        if (pbc_eq9.Name != eq_name) pbc_eq9.Position += dx_pos;
                    }

                    if (eq_name == "pbc_eq8" || eq_name == "pbc_eq7" || eq_name == "pbc_eq6" || eq_name == "pbc_eq5")
                    {
                        if (pbc_eq8.Name != eq_name) pbc_eq8.Position += dx_pos;
                        if (pbc_eq7.Name != eq_name) pbc_eq7.Position += dx_pos;
                        if (pbc_eq6.Name != eq_name) pbc_eq6.Position += dx_pos;
                        if (pbc_eq5.Name != eq_name) pbc_eq5.Position += dx_pos;
                    }

                    if (eq_name == "pbc_eq4" || eq_name == "pbc_eq3" || eq_name == "pbc_eq2" || eq_name == "pbc_eq1")
                    {
                        if (pbc_eq4.Name != eq_name) pbc_eq4.Position += dx_pos;
                        if (pbc_eq3.Name != eq_name) pbc_eq3.Position += dx_pos;
                        if (pbc_eq2.Name != eq_name) pbc_eq2.Position += dx_pos;
                        if (pbc_eq1.Name != eq_name) pbc_eq1.Position += dx_pos;
                    }
                }
                #endregion

            }
            else ((ProgressBarControl)sender).Cursor = Cursors.HSplit;

            #endregion

            #region Всплывающая подсказка
            if (((ProgressBarControl)sender).Tag != null)
            {
                double db_val = Math.Round((((double)pos / max) - 0.5) * 30, 2);
                int eq_number = Int16.Parse(((ProgressBarControl)sender).Tag.ToString());
                string tt_str;
                if (eq_number == 0)
                    tt_str = "Pre [" + db_val.ToString("0.00") + " db]";
                else
                    tt_str = EQCenter[eq_number].ToString() + " Hz [" + db_val.ToString("0.00") + " db]";
                
                //string hz_str = ((ProgressBarControl)sender).Tag.ToString();

                ((ProgressBarControl)sender).ToolTip = tt_str;
                toolTipController.ShowHint(tt_str);
            }  
            #endregion
            
        }

        // установка значения полосы
        private void pbc_equal_main_value_changed(object sender, EventArgs e)
        {           
            if (fl_eqLoading) return;

            if (((ProgressBarControl)sender).Tag != null)
            {
                double pos = ((ProgressBarControl)sender).Position;
                double pos_max = ((ProgressBarControl)sender).Properties.Maximum;
                double fGain_coef = ((pos / pos_max) - 0.5) * 30; // (range -15..15 def 0 )
                int eq_number = Int16.Parse(((ProgressBarControl)sender).Tag.ToString()); // порядковый номер полосы экв.                

                eq.fBandwidth = EQBandwidth; // Пропускная способность полутонов, 1 .. 36 def 18                
                eq.fCenter = EQCenter[eq_number]; // 100f; // центральная (или рабочая) частота в герцах (Гц); (80 .. 16000)
                eq.fGain = (float)fGain_coef; // 0f; // уровень усиления или ослабления выбранной полосы в децибелах (дБ); (range -15..15 def 0 )

                //int fxHandle = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 1);
                Bass.BASS_FXSetParameters(fxHandle, eq);
            }
        }

        // Изменение Preamp
        private void pbc_equal_preamp_value_changed(object sender, EventArgs e)
        {
            double pos = ((ProgressBarControl)sender).Position;
            int pos_max = ((ProgressBarControl)sender).Properties.Maximum;

            EQfGain[0] = (int)(((pos / pos_max) - 0.5) * 30);

            Eq_array_Commit();
        }

        // Сброс настроек эквалайзера
        private void btn_eq_clear_Click(object sender, EventArgs e)
        {
            object eq_line;

            eq.fGain = 0f; // нулевое отклонение

            fl_eqLoading = true; // флаг очистки

            for (int i = 1; i < EQCenter.Length; i++)
            {
                eq.fBandwidth = EQBandwidth;
                eq.fCenter = EQCenter[i];
                Bass.BASS_FXSetParameters(fxHandle, eq);
                
                // установка значения у контрола
                eq_line = FindEqBarByName("pbc_eq" + i.ToString());
                if (eq_line != null)
                {
                    ((ProgressBarControl)eq_line).Position = ((ProgressBarControl)eq_line).Properties.Maximum / 2;
                }                
            }

            eq_line = FindEqBarByName("pbc_eq_preamp");
            if (eq_line != null)
            {
                int pos_max = ((ProgressBarControl)eq_line).Properties.Maximum;
                ((ProgressBarControl)eq_line).Position = ((ProgressBarControl)eq_line).Properties.Maximum / 2;
            }            

            fl_eqLoading = false;
        }

        // Запись настроек EQ в массив
        private void Eq_array_Save()
        {
            object sender;

            // перебор полос эквалайзера
            for (int i = 1; i < EQCenter.Length; i++)
            {
                sender = FindEqBarByName("pbc_eq" + i.ToString());
                if (sender != null)
                {
                    double pos = ((ProgressBarControl)sender).Position;
                    int pos_max = ((ProgressBarControl)sender).Properties.Maximum;
                    int fGain_coef = (int)(((pos / pos_max) - 0.5) * 30);

                    // отклонение (range -15..15 def 0 ) относительно нуля - середины EQ полосы                    
                    EQfGain[i] = fGain_coef; 
                }
            }

            // Preamp
            sender = FindEqBarByName("pbc_eq_preamp");
            if (sender != null)
            {
                double pos = ((ProgressBarControl)sender).Position;
                int pos_max = ((ProgressBarControl)sender).Properties.Maximum;

                EQfGain[0] = (int)(((pos / pos_max) - 0.5) * 30);
            }

        }

        // Чтение настроек из EQ массива
        private void Eq_array_Load()
        {
            object sender;
            fl_eqLoading = true;

            // Preamp
            sender = FindEqBarByName("pbc_eq_preamp");
            if (sender != null)
            {
                int pos_max = ((ProgressBarControl)sender).Properties.Maximum;
                ((ProgressBarControl)sender).Position = (int)((((double)EQfGain[0] / 30) + 0.5) * pos_max);
            }            

            // перебор значений полос эквалайзера
            for (int i = 1; i < EQCenter.Length; i++)
            {
                sender = FindEqBarByName("pbc_eq" + i.ToString());
                if (sender != null)
                {
                    int pos_max = ((ProgressBarControl)sender).Properties.Maximum;
                    //int fGain_coef = (int)(((pos / pos_max) - 0.5) * 30);

                    ((ProgressBarControl)sender).Position = (int)((((double)EQfGain[i] / 30) + 0.5) * pos_max);
                }
            }

            fl_eqLoading = true;
        }

        // Применение настрое EQ
        private void Eq_array_Commit()
        {            
            fl_eqLoading = true; // флаг загрузки            

            for (int i = 1; i < EQCenter.Length; i++)
            {
                eq.fGain = EQfGain[i]  + (EQfGain[0]); // нулевое отклонение
                eq.fBandwidth = EQBandwidth;
                eq.fCenter = EQCenter[i];
                Bass.BASS_FXSetParameters(fxHandle, eq);
            }

            fl_eqLoading = false;
        }

        // сброс EQ
        private void pbc_eq_preamp_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btn_eq_clear_Click(null, null);
        }
        #endregion

        #region Главное Меню
        private void iFullScreen_ItemPress(object sender, ItemClickEventArgs e)
        {
            if (iFullScreen.Checked)
            {
                if (FView.IsActive) // если не сохранены ранее
                {
                    FView_Save();
                    FView.IsActive = false;
                }

                iFullScreen2.Checked = false;
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;                
            }
            else
            {
                FView_Load();
            }            
        }

        private void iFullScreen2_ItemPress(object sender, ItemClickEventArgs e)
        {
            if (iFullScreen2.Checked)
            {
                if (FView.IsActive)
                {
                    FView_Save();
                    FView.IsActive = false;
                }

                iFullScreen.Checked = false;
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;                
            }
            else
            {
                FView_Load();
            }         
        }

        private void FView_Save()
        {
            FView.FormBorderStyle = this.FormBorderStyle;
            FView.WindowState = this.WindowState;
            FView.EQ_Visible = panelControl_EQ.Visible;
        }

        private void FView_Load()
        {
            this.FormBorderStyle = FView.FormBorderStyle;
            this.WindowState = FView.WindowState;
            panelControl_EQ.Visible = FView.EQ_Visible;
            iEQ_Open.Checked = FView.EQ_Visible;
            FView.IsActive = true;
        }

        private void iAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form_About af = new Form_About("Парнас плеер", "0.1 (beta)");
            af.ShowDialog();
        }

        private void iExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void iEQ_Open_CheckedClick(object sender, ItemClickEventArgs e)
        {
            panelControl_EQ.Visible = iEQ_Open.Checked;
        }

        private void iEQ_Open2_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (panelControl_EQ.Visible)
                panelControl_EQ.Visible = false;
            else
                panelControl_EQ.Visible = true;// 

            iEQ_Open.Checked = panelControl_EQ.Visible;
        }

        private void iPlayBack_CheckedClick(object sender, ItemClickEventArgs e)
        {
            panelControl_PlayBack.Visible = iPlayBack.Checked;
        }

        private void iPlayBack2_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (panelControl_PlayBack.Visible)
                panelControl_PlayBack.Visible = false;
            else
                panelControl_PlayBack.Visible = true;// 

            iPlayBack.Checked = panelControl_PlayBack.Visible;
        }

        // поиск по списку
        private void iFind_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (All_PlayLists[play_list_id].gv_PlayList.OptionsFind.AlwaysVisible)
                All_PlayLists[play_list_id].gv_PlayList.OptionsFind.AlwaysVisible = false;
            else
                All_PlayLists[play_list_id].gv_PlayList.OptionsFind.AlwaysVisible = true;            
        }

        #endregion

        #region Текст песен
        private void RTBox_TextFile_SizeChanged(object sender, EventArgs e)
        {
            float Scale = (float)Math.Round((float)RTBox_TextFile.Width / 800, 2);
            if (Scale < 1) Scale = 1;

            RTBox_TextFile.ZoomFactor = Scale;
        }

        private void RTBox_TextFile_Scroll(double pos_persent)
        {
            Sroll_lines = (int)spinEdit1.Value;
            double max = RTBox_TextFile.TextLength; // всего символов
            int scale = (int)(max * Sroll_lines / 100); // RTBox_TextFile.Lines.Length * Sroll_lines; // ср. символов в строке
            //int selstart = (int)(Math.Round((max * pos_persent) / scale, 0) * scale);
            int cur_pos = (int)((int)(pos_persent * 100 / Sroll_lines) * scale);

            if ((RTBox_TextFile.Tag == null || RTBox_TextFile.Tag.ToString() != cur_pos.ToString()) && cur_pos > 0)
            {
                RTBox_TextFile.SelectionStart = cur_pos;
                RTBox_TextFile.ScrollToCaret();

                RTBox_TextFile.Tag = cur_pos.ToString();
            }
        } 
        #endregion

        //тест
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (xTabCtrl_PlayLists.HeaderLocation == DevExpress.XtraTab.TabHeaderLocation.Top)
                xTabCtrl_PlayLists.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Left;
            else
                if (xTabCtrl_PlayLists.HeaderLocation == DevExpress.XtraTab.TabHeaderLocation.Left)
                    xTabCtrl_PlayLists.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Bottom;
                else
                    if (xTabCtrl_PlayLists.HeaderLocation == DevExpress.XtraTab.TabHeaderLocation.Bottom)
                        xTabCtrl_PlayLists.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Right;
                    else xTabCtrl_PlayLists.HeaderLocation = DevExpress.XtraTab.TabHeaderLocation.Top;
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            if (xTabCtrl_PlayLists.HeaderOrientation == DevExpress.XtraTab.TabOrientation.Horizontal)
                xTabCtrl_PlayLists.HeaderOrientation = DevExpress.XtraTab.TabOrientation.Vertical;
            else
                xTabCtrl_PlayLists.HeaderOrientation = DevExpress.XtraTab.TabOrientation.Horizontal;
        }

        private void xTabCtrl_PlayLists_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            play_list_id = xTabCtrl_PlayLists.SelectedTabPageIndex;
        }

    }

    public class FormView
    {
        public FormBorderStyle FormBorderStyle = FormBorderStyle.Sizable;
        public FormWindowState WindowState = FormWindowState.Normal;
        public bool IsActive = true;
        public bool EQ_Visible = true;
    }
}