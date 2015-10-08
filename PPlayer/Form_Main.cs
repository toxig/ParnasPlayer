using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using TagLib;
using Un4seen.Bass;

namespace PPlayer
{

    public partial class Form_Main : XtraForm
    {        
        internal bool? initDefaultDevice;
        internal int stream;
        Random r = new Random();
        bool manualStopped = false;        
        int play_list_id = 0; // Активный плейлист
        int play_list_id_played = 0; // Активный плейлист
        int Sroll_lines = 12; // размер скрола строк для автотрекинга
        int play_id;        
        FormView FView = new FormView();
        Loader_FileList FLoader = new Loader_FileList();
        string PlaingFileName;        
        Control_EQ EQ_Main;                                
        Control_PlayList[] All_PlayLists = new Control_PlayList[10];
        int WaytDelay = 0;
        string Name_HotList = "H";
        bool RTEdit_is_focused = false;

        int PL_Widh_prim_scale = 350;
        int PL_Widh_min_scale = 180;
        int Txt_Widh_prim_scale = 800;
        int Line_max_width = 0;

        // версия
        string ver = "0.4 (beta)";

        // Константы
        Keys Key_to_HotList = Keys.Enter;        
        int FullScreen_Delta = 10;
        float PL_FontSize = 13.8f;
        Font Default_Font = new Font("Arial", 18, FontStyle.Bold);
        
        // Пользовательские настройки программы
        //public static MySettings mySettings = new MySettings();

        public Form_Main()
        {
            InitializeComponent();            

            // registering BASS.NET API
            BassNet.Registration("toxig@ya.ru", "2X182415113438");

            // Инициализация устройства воспроизведения
            try
            {
                if (initDefaultDevice.HasValue == false)
                {
                    initDefaultDevice = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                }
            }
            catch (NullReferenceException)
            {
                XtraMessageBox.Show("Не удалось иниициировать звуковое устройство!", "Инициализация звука", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }  

            // Инициализация таблиц для плейлистов
            Init_PlayLists();

            // Инициализация эквалайзера
            Init_EQ();

            // Настройки отображения окна
            FView_Save();                                                 

            panelControl_EQ.Visible = false;
            panelControl_PlayBack.Visible = false;

            // Настройки текста
            Init_Text();            
        }

        #region Инициализация элементов
        private void Init_PlayLists()
        {            
            for (int i = 0; i < xTabCtrl_PlayLists.TabPages.Count; i++)
            {
                Init_New_PlayList(i);
            }
        }

        // новый плей лист
        private void Init_New_PlayList(int i)
        {
            All_PlayLists[i] = new Control_PlayList();            
            All_PlayLists[i].Dock = DockStyle.Fill;

            xTabCtrl_PlayLists.TabPages[i].Controls.Add(All_PlayLists[i]);
            if (i != 0) xTabCtrl_PlayLists.TabPages[i].Text = "[" + (i) + "]";
            else xTabCtrl_PlayLists.TabPages[i].Text = "[ГОРЯЧИЙ]";

            // события списка
            All_PlayLists[i].grid_PlayList.DoubleClick += new System.EventHandler(this.grid_PlayList_DoubleClick);                        
            All_PlayLists[i].gv_PlayList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_PlayList_KeyDown);                        
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

        private void Init_Text()
        {
            this.panelControl_All.Dock = DockStyle.None;
            this.panelControl_All.Left = FullScreen_Delta; // Location.X = 10;
            this.panelControl_All.Top = FullScreen_Delta - 2; // Location.Y = 10;
            this.panelControl_All.Height -= FullScreen_Delta * 2 - 3;
            this.panelControl_All.Width -= FullScreen_Delta;
            this.panelControl_All.Dock = DockStyle.Fill;
        }
        #endregion

        #region Операции с файлом плейлиста

        private void gv_PlayList_KeyDown(object sender, KeyEventArgs e)
        {
            if (All_PlayLists[play_list_id].gv_PlayList.FocusedRowHandle < 0) return;

            int Row_ID = All_PlayLists[play_list_id].dt_ListData.Rows.IndexOf(All_PlayLists[play_list_id].gv_PlayList.GetFocusedDataRow());

            // добавление в горячий лист
            if (e.KeyCode == Key_to_HotList && play_list_id != 0)
            {                                                    
                DataRow FRow = All_PlayLists[play_list_id].dt_FileData.Rows[Row_ID];                    
                DataRow LRow = All_PlayLists[play_list_id].dt_ListData.Rows[Row_ID];                
                
                All_PlayLists[0].dt_FileData.Rows.Add(FRow.ItemArray);
                All_PlayLists[0].dt_ListData.Rows.Add(LRow.ItemArray);

                // мигание строки
                All_PlayLists[play_list_id].Focused_Row_Blink();
            }

            // удаление из листа
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {                
                All_PlayLists[play_list_id].dt_FileData.Rows[Row_ID].Delete();
                All_PlayLists[play_list_id].dt_ListData.Rows[Row_ID].Delete();
            }
        }

        // Загрузка файлов из папки
        private void File_Load_Dir(string DirPath)
        {
            //comboBoxPlaybackOrder.SelectedIndex = 0;
            if (initDefaultDevice.HasValue == false)
            {
                initDefaultDevice = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                EQ_Main.pbc_equal_main.Position = int.Parse(Math.Truncate((Bass.BASS_GetVolume() * 100)).ToString());
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
                //System.IO.File.WriteAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\KPlayerOptions.ini", new string[] { string.Empty });
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

                    if (play_list_id == 0) xTabCtrl_PlayLists.TabPages[play_list_id].Text = "[" + Name_HotList + "] " + tName;
                    else xTabCtrl_PlayLists.TabPages[play_list_id].Text = "[" + (play_list_id) + "] " + tName;

                    All_PlayLists[play_list_id].Init_Table_Data();
                    All_PlayLists[play_list_id].pl_FilePath = PMFilePath;

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

        // ПОВТОР (REPEAT)
        private void sbtn_Repeat_Click(object sender, EventArgs e)
        {
            switch (sbtn_Repeat.Tag.ToString())
            {
                case "0":
                    sbtn_Repeat.Tag = "1";
                    sbtn_Repeat.Text = "R (вкл)";
                    sbtn_Repeat.ToolTip = "Повтор (нонстоп, весь плейлист)";
                    break;
                case "1":
                    sbtn_Repeat.Tag = "0";
                    sbtn_Repeat.Text = "R";
                    sbtn_Repeat.ToolTip = "Повтор (отключен)";
                    break;
                /*case "2":
                    sbtn_Repeat.Tag = "0";
                    sbtn_Repeat.Text = "R (1)";
                    sbtn_Repeat.ToolTip = "Повтор (один трек)";
                    break;*/
            }

            toolTipController.ShowHint(sbtn_Repeat.ToolTip);            
        }
        
        // ПЛЕЙ
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            PlayCurFile_FromList(play_list_id_played, 0); // текущий трек
        }

        // СТОП
        private void buttonStop_Click(object sender, EventArgs e)
        {
            Bass.BASS_StreamFree(stream);
            Label_InfoLine.Text = "Остановлено"; WaytDelay = 3;
            manualStopped = true;
            sbtn_Pause.Text = "Play";

            Update_Paly_Status();
        }

        // ВПЕРЕД
        private void buttonNext_Click(object source, EventArgs e)
        {                  
            Update_Paly_Status();
            PlayCurFile_FromList(play_list_id_played, 1);  // следующий трек          
        }

        // НАЗАД
        private void buttonPrev_Click(object sender, EventArgs e)
        {            
            Update_Paly_Status();
            PlayCurFile_FromList(play_list_id_played, -1); // предыдущий трек
        }

        // ПАУЗА
        private void buttonPause_Click(object sender, EventArgs e)
        {
            int Row_ID = All_PlayLists[play_list_id].dt_ListData.Rows.IndexOf(All_PlayLists[play_list_id].gv_PlayList.GetFocusedDataRow());

            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {                                
                Bass.BASS_ChannelPause(stream);
                Label_InfoLine.Text = "Пауза";
                sbtn_Pause.Text = "Play";
            }
            else
                if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PAUSED 
                    && !manualStopped 
                    && play_list_id == play_list_id_played
                    && play_id == Row_ID)
                {
                    //Bass.BASS_Start();
                    Bass.BASS_ChannelPlay(stream, false);
                    Label_InfoLine.Text = "Проигрывается";
                    sbtn_Pause.Text = "Pause";                    
                }
                else
                {
                    PlayCurFile_FromList(play_list_id_played, 0); // текущий трек
                    Label_InfoLine.Text = "Проигрывается";
                    sbtn_Pause.Text = "Pause";
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
        public void PlayCurFile_FromList(int list_id, int row_shift)
        {
            #region Обработка сдвига в списке

            int Rows_count = All_PlayLists[list_id].gv_PlayList.RowCount;
            if (Rows_count == 0) return; // нет записей

            int Row_focus = All_PlayLists[list_id].gv_PlayList.FocusedRowHandle;

            Row_focus = Row_focus + row_shift; // сдвиг трека, след/предыдущ/текущий

            if (Row_focus > Rows_count - 1) Row_focus = 0;
            if (Row_focus < 0) Row_focus = Rows_count - 1;

            All_PlayLists[list_id].gv_PlayList.FocusedRowHandle = Row_focus;
            
            #endregion
            
            // запись в таблице данных
            int Row_ID = All_PlayLists[list_id].dt_ListData.Rows.IndexOf(All_PlayLists[list_id].gv_PlayList.GetFocusedDataRow());            

            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PAUSED && play_id == Row_ID)
            {
                Bass.BASS_Start();
                Label_InfoLine.Text = "Проигрывается";
            }
            else
                try
                {
                    // если была пацза - освобождаем поток
                    //if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PAUSED) Bass.BASS_StreamFree(stream);

                    string FileName = "";
                    string TextFileName = "";

                    DataRow Row = All_PlayLists[list_id].dt_FileData.Rows[Row_ID];

                    {                                                
                        FileName = Row[0].ToString();
                        TextFileName = Row[1].ToString();
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
                        RTBox_TextFile.Font = new Font("Tahoma", 20, FontStyle.Bold);
                        string TextMsg = "";

                        if (TextFileName == "")
                            TextMsg = "Текстовый файл не присвоен.";
                        else
                            TextMsg = "Нет текстового файла на диске: " + TextFileName;
                        
                        RTBox_TextFile.Text = TextMsg;
                        Label_InfoLine.Text = TextMsg;

                    }

                    // Загрузка музыки
                    if (System.IO.File.Exists(FileName))
                    {
                        PlaingFileName = Path.GetFileName(FileName); // муз файл
                        Init_PlayStream(FileName); // инициализация звукового потока                                               

                        EQ_Main.Row = Row;
                        EQ_Main.Eq_row_to_array(); // загрузка эквалайзера
                        EQ_Main.Eq_array_to_object();
                        EQ_Main.Eq_array_to_stream();

                        PlayFile(); // запуск воспроизведения

                        if (list_id == 0) lbc_playlist.Text = Name_HotList.Substring(0, 1);
                        else lbc_playlist.Text = list_id.ToString();                        
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

        public void Init_PlayStream(string fileName)
        {
            if (initDefaultDevice.Value)
            {
                if (stream != 0)
                {
                    // останавливаем поток
                    manualStopped = true;
                    if (Bass.BASS_ChannelIsActive(stream) != BASSActive.BASS_ACTIVE_STOPPED) Bass.BASS_Stop();
                    // освобождаем поток
                    Bass.BASS_StreamFree(stream);
                }               

                // новый поток
                stream = Bass.BASS_StreamCreateFile(fileName, 0, 0, BASSFlag.BASS_DEFAULT);
                EQ_Main.Init_fx(stream);
            }
        }

        //Запуск воспроизведения
        public void PlayFile()
        {            
            if (initDefaultDevice.Value) // активация устройства воспроизведения
            {                                
                if (stream != 0) // есть поток
                {
                    if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_STOPPED) Bass.BASS_Start();
                    Bass.BASS_ChannelPlay(stream, false);
                    timerForCurrentPosition_Tick(null, null);                    
                }
                else
                {
                    MessageBox.Show(String.Format("Error {0}", Bass.BASS_ErrorGetCode()));
                }
            }
            
            timer_playng.Enabled = false;
            timer_playng.Interval = 100;
            Label_InfoLine.Text = "Проигрывается";

            string[] tagID3V1 = Bass.BASS_ChannelGetTagsID3V1(stream);
            string[] tagID3V2 = Bass.BASS_ChannelGetTagsID3V2(stream);
            
            progressBarControl_PlayPosition.Properties.Minimum = 0;
            progressBarControl_PlayPosition.Properties.Maximum = int.Parse(((Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream)).ToString().Split(',', '.'))[0]));

            Update_Paly_Status();
            timer_playng.Enabled = true;
            
            manualStopped = false;
        }

        // играть выбранную строку
        private void grid_PlayList_DoubleClick(object sender, EventArgs e)
        {
            if (All_PlayLists[play_list_id].dt_ListData != null && 
                All_PlayLists[play_list_id].dt_ListData.Rows.Count != 0)
            {
                play_list_id_played = play_list_id;
                buttonPlay_Click(null, null);
                sbtn_Pause.Text = "Pause";
            }
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

        // выбор списка
        private void xTabCtrl_PlayLists_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            play_list_id = xTabCtrl_PlayLists.SelectedTabPageIndex;
            All_PlayLists[play_list_id].gv_PlayList.Focus();
                
            // подписи вкладок
            for (int i = 1; i < xTabCtrl_PlayLists.TabPages.Count; i++)
            {                
                xTabCtrl_PlayLists.TabPages[i].Text = "[" + (i) + "]";
                
                if (play_list_id == i)
                {
                    string tName = All_PlayLists[play_list_id].pl_FilePath;

                    if (tName != "")
                    {
                        tName = Path.GetFileNameWithoutExtension(tName);
                        tName = tName[0].ToString().ToUpper() + tName.Substring(1, tName.Length - 1);
                        xTabCtrl_PlayLists.TabPages[play_list_id].Text += " " + tName;
                    }
                }
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
                Update_Paly_Status();

                //this.Text = "KPlayer - " + SecondsToTimeFormat(d) + @"/" + SecondsToTimeFormat(all);                

                if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_STOPPED && !manualStopped) // конец трека
                {
                    switch (sbtn_Repeat.Tag.ToString())
                    {
                        // нет повтора
                        case "0": 
                            timer_playng.Enabled = false;
                            sbtn_Pause.Text = "Play";
                            break;
                        // весь плейлист
                        case "1":
                            buttonNext_Click(null, null); // следующий трек
                            break;
                        // один трек
                        /*case "2": 
                            buttonPlay_Click(null, null); // текущий трек
                            break;*/
                    }                                        
                }

            }
        }

        private void Update_Paly_Status()
        {
            int d = int.Parse(((Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream)).ToString().Split(',', '.'))[0]));
            int all = int.Parse(((Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream)).ToString().Split(',', '.'))[0]));
                        
            if (d < 0) d = 0;
            if (all < 0) all = 0;
            if (d > all) d = all;

            // синзронизация таймера (100, если первая секунда, иначе 1000)
            if (d == 0) timer_playng.Interval = 100;
            else if (timer_playng.Interval != 1000) timer_playng.Interval = 1000;

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

            //RTBox_TextFile_Scroll((double)d / all);

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
            //System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\KPlayerOptions.ini", Storage.path);

            //mySettings.Save();
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

        // Изменение размера шрифта
        static public Font ChangeFontSize(Font font, float fontSize)
        {
            if (font != null)
            {
                float currentSize = font.Size;
                if (currentSize != fontSize)
                {
                    font = new Font(font.Name, fontSize, font.Style);
                }
            }
            return font;
        }

        // Размера текстовой строки (в пикселях)
        private int Text_CalcWidth(string s, Font TextFont)
        {
            return TextRenderer.MeasureText(s, Default_Font).Width;
        }
        #endregion        

        #region Эквалайзер

        private void Init_EQ()
        {
            EQ_Main = new Control_EQ();
            EQ_Main.Dock = DockStyle.Fill;

            // Контрол на форму
            panelControl_EQ.Controls.Add(EQ_Main);

            // Уровень Громкости            
            if (initDefaultDevice.HasValue == false)
            {                
                EQ_Main.pbc_equal_main.Position = int.Parse(Math.Truncate((Bass.BASS_GetVolume() * 100)).ToString());
            }

            // Настройки Эквалайзера
            //Eq_array_Save();
        }                
        
        #endregion

        #region Главное Меню

        private void iFullScreen_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            if (iFullScreen.Checked)
            {
                if (FView.IsActive)
                {
                    FView_Save();
                    FView.IsActive = false;
                }
                this.panelControl_All.Dock = DockStyle.None;
                this.panelControl_All.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;       
                
                //this.panelControl_All.Width                        
            }
            else
            {                
                FView_Load();
                this.panelControl_All.Dock = DockStyle.Fill;
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
            this.WindowState = FView.WindowState;
            this.FormBorderStyle = FView.FormBorderStyle;           
            panelControl_EQ.Visible = FView.EQ_Visible;
            iEQ_Open.Checked = FView.EQ_Visible;
            FView.IsActive = true;
        }

        private void iAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form_About af = new Form_About("Парнас плеер", ver);
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
            All_PlayLists[play_list_id].Toggle_Show_Filter();

            /*
            if (All_PlayLists[play_list_id].gv_PlayList.OptionsFind.AlwaysVisible)
                All_PlayLists[play_list_id].gv_PlayList.OptionsFind.AlwaysVisible = false;
            else
                All_PlayLists[play_list_id].gv_PlayList.OptionsFind.AlwaysVisible = true;            
             * */
        }

        #endregion

        #region Текст песен
        // Изменение размера редактора
        private void RTBox_TextFile_SizeChanged(object sender, EventArgs e)
        {
            // пересчет масштаба текста
            RTBox_CorrectScale();
            
            // ограничение минимальной ширины листа
            if (panelControl_Right.Width < PL_Widh_min_scale) panelControl_Right.Width = PL_Widh_min_scale;

            // Шрифт записей плейлиста
            float PL_Scale = (float)Math.Round((float)panelControl_Right.Width / PL_Widh_prim_scale, 2);            
            for (int i = 0; i < xTabCtrl_PlayLists.TabPages.Count; i++)
            {
                if (All_PlayLists[i] != null)
                {
                    Font PL_Font = All_PlayLists[i].gv_PlayList.Appearance.Row.Font;
                    if (PL_Scale > 1) PL_Scale = 1;

                    PL_Font = ChangeFontSize(PL_Font, PL_FontSize * PL_Scale);                

                    All_PlayLists[i].gv_PlayList.Appearance.Row.Font = PL_Font;                   
                }
            }            
        }

        // Изменение текста в редакторе
        private void RTBox_TextFile_TextChanged(object sender, EventArgs e)
        {
            // изменение шрифта
            RTBox_CorrectFont();
            // поиск максимальной строки
            RTBox_Text_CalcMaxWidth();
            // изменение масштаба
            RTBox_CorrectScale();
            // начальная позиция
            RTBox_TextFile_Scroll(0);
        }

        // Перерасчет масштаба отображения
        private void RTBox_CorrectScale()
        {
            if (Line_max_width > 0)
            {
                RTBox_TextFile.ZoomFactor = (float)((double)RTBox_TextFile.ClientSize.Width / Line_max_width * 0.95);
            }
        }

        // Установка шрифта для текстового редактора
        private void RTBox_CorrectFont()
        {
            RTBox_TextFile.Select(0, RTBox_TextFile.Text.Length);
            RTBox_TextFile.SelectionFont = Default_Font;
            RTBox_TextFile.Select(0, 0);
        }

        // Расчет максимальной длины строки в текстовом редакторе
        private void RTBox_Text_CalcMaxWidth()
        {
            Line_max_width = 0;

            foreach (string LineText in RTBox_TextFile.Lines)
            {
                int line_width = Text_CalcWidth(LineText, Default_Font);
                if (Line_max_width < line_width) Line_max_width = line_width;
                //Graphics.MeasureString();                
            }
        }

        // Скрол на процент
        private void RTBox_TextFile_Scroll(double pos_persent)
        {
            Sroll_lines = (int)spinEdit1.Value;
            double max = RTBox_TextFile.TextLength; // всего символов
            int scale = (int)(max * Sroll_lines / 100); // RTBox_TextFile.Lines.Length * Sroll_lines; // ср. символов в строке
            //int selstart = (int)(Math.Round((max * pos_persent) / scale, 0) * scale);
            int cur_pos = (int)((int)(pos_persent * 100 / Sroll_lines) * scale);

            if ((RTBox_TextFile.Tag == null || RTBox_TextFile.Tag.ToString() != cur_pos.ToString()) && cur_pos >= 0)
            {
                RTBox_TextFile.SelectionStart = cur_pos;
                RTBox_TextFile.ScrollToCaret();

                RTBox_TextFile.Tag = cur_pos.ToString();
            }
        }

        // установка фокуса на текстовый редактор
        private void RTBox_TextFile_MouseEnter(object sender, EventArgs e)
        {
            if (!RTEdit_is_focused)
            {
                RTEdit_is_focused = true;
                RTBox_TextFile.Focus();
            }
        }

        // потеря фокуса текстовым редактором
        private void RTBox_TextFile_LostFocus(object sender, EventArgs e)
        {
            RTEdit_is_focused = false;
        }

        #endregion        

        #region Горячие клавиши

        // следующий трек (активный плейлист)
        private void iNext_Track_ItemClick(object sender, ItemClickEventArgs e)
        {
            int Rows_count = All_PlayLists[play_list_id].gv_PlayList.RowCount;
            if (Rows_count == 0) return; // нет записей

            int Row_focus = All_PlayLists[play_list_id].gv_PlayList.FocusedRowHandle;

            Row_focus++; // сдвиг трека, след/предыдущ/текущий

            if (Row_focus > Rows_count - 1) Row_focus = 0;
            if (Row_focus < 0) Row_focus = Rows_count - 1;

            All_PlayLists[play_list_id].gv_PlayList.FocusedRowHandle = Row_focus;
        }

        // предыдущий трек (активный плейлист)
        private void iPrev_Track_ItemClick(object sender, ItemClickEventArgs e)
        {
            int Rows_count = All_PlayLists[play_list_id].gv_PlayList.RowCount;
            if (Rows_count == 0) return; // нет записей

            int Row_focus = All_PlayLists[play_list_id].gv_PlayList.FocusedRowHandle;

            Row_focus--; // сдвиг трека, след/предыдущ/текущий

            if (Row_focus > Rows_count - 1) Row_focus = 0;
            if (Row_focus < 0) Row_focus = Rows_count - 1;

            All_PlayLists[play_list_id].gv_PlayList.FocusedRowHandle = Row_focus;  
        }

        // Следующий плейлист
        private void iNext_PlayList_ItemClick(object sender, ItemClickEventArgs e)
        {
            int TabCount = xTabCtrl_PlayLists.TabPages.Count;
            int TabSelected = xTabCtrl_PlayLists.SelectedTabPageIndex;

            TabSelected++;

            if (TabSelected > TabCount - 1) TabSelected = 0;
            if (TabSelected < 0) TabSelected = TabCount - 1;

            xTabCtrl_PlayLists.SelectedTabPageIndex = TabSelected;
        }

        // Предыдущий плейлист
        private void iPrev_PlayList_ItemClick(object sender, ItemClickEventArgs e)
        {
            int TabCount = xTabCtrl_PlayLists.TabPages.Count;
            int TabSelected = xTabCtrl_PlayLists.SelectedTabPageIndex;

            TabSelected--;

            if (TabSelected > TabCount - 1) TabSelected = 0;
            if (TabSelected < 0) TabSelected = TabCount - 1;

            xTabCtrl_PlayLists.SelectedTabPageIndex = TabSelected;
        }

        // Прокрута текста (вниз)
        private void iNext_Text_ItemClick(object sender, ItemClickEventArgs e)
        {
            
        }

        // Прокрута текста (вверх)
        private void iPrev_Text_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        #endregion
                
        private void Form_Main_Load(object sender, EventArgs e)
        {
           //mySettings
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