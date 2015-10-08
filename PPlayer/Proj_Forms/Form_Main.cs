using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using TagLib;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using System.Diagnostics;


namespace PPlayer
{  
    public partial class Form_Main : XtraForm
    {
        // версия
        Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        private string AboutInfo = "Парнас плеер";
        private string AboutVersion;        

        // Переменные     
        public string v_dir_program = Directory.GetCurrentDirectory();
        public string v_dir_log;
        private string v_file_log;   
        internal bool? v_initDefaultDevice;   // канал вывода звука        
        bool v_RTEdit_is_focused = false;     // фокус на окне текста песни
        bool v_RTEdit_no_resize = false;     // не масштабировать текст
        public int v_play_list_id_active = 0;        // Активный плейлист        
        //int v_play_track_id;                // проигрываемый трек
        int v_WaytDelay = 0;                  //задержка перерисовки информации о треке в статус баре                
        //int v_PL_Widh_prim_scale = 400;       // ширина плейлиста
        int v_PL_Widh_min_scale = 250;        // ширина плейлиста minimal
        int v_Line_max_width = 0;             // длина максимальной строки в текстовом файле (pix)
        double v_Text_Width_koef = 0.96;      // коэфициент увеличения текста / Пример: 1 - 100% ширины, 0.5 - 50%
        int v_Logo_NoActions_Time = 0;
        string v_Name_HotList = "H";          //Заголовок горячего плейлиста
        private int v_MainWindowTread = Thread.CurrentThread.ManagedThreadId; // ID главного потока              

        //string v_PlaingFileName;                                  // название проигрываемого трека                
        Working            FWorking = new Working();              // Отображение статуса выполнения
        Form_TagEditor      FTags = new Form_TagEditor();           // Редактор Тэгов
        Form_Settings       FSettings = new Form_Settings();        // Редактор настроек
        FormView            FView = new FormView();                 // настройки отображения формы                  
        Control_EQ          EQ_Main;                                // настройки эквалайзера
        Control_PlayList[]  All_PlayLists = new Control_PlayList[16]; // массив плейлистов
        StreamClass         MainStream = new StreamClass();         // основной поток
        StreamClass         SlaveStream = new StreamClass();        // поток для сведения фейдера
        Mod_Rich_Edit       Text_RTEditor = new Mod_Rich_Edit();    // редактор текстов
        Form_History        FChangeLog = new Form_History();        // История изменений списка
        Form_Update         FUpdate = new Form_Update();        

        // Константы
        Keys v_Key_to_HotList = Keys.Enter; // клавиша добавления трека в горячий список
        int v_FullScreen_Delta = 10;        // отступ от края экрана при fullscreen
        //float v_PL_FontSize = 13.8f;        // размер шрифта def
        Font Default_Font = new Font("Arial", 18, FontStyle.Bold);
        //bool iTime_Inverse_Check.Checked  // инверсия отсчета времени
        //bool iCheck_AutoScrollText.Checked     // активация автоскрола
        int v_Scroll_Lines_Delta = 1;       // кол-во строк для одного скрола по тексту
        //int v_Scroll_lines = 12;          // размер скрола строк для автотрекинга
        int v_Scroll_Delta_Sec = 1;         // изменение времени активации автоскрола
        int v_FadeTime_Pause = 3000;        // затухание при паузе
        int v_FadeTime_Hot = 2000;          // затухание при горячем пуске
        int v_Logo_StartTime = 5000;        //20 сек
        float[] level_max = new float[2];
        float[] level_min = new float[2];
        string[] v_arg_pl;                  // плейлист, переданный в параметрах запуска (cmd)


        #region Scroll DLL init

        // SCROLL
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

        [DllImport("user32.dll")]
        static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern int SetScrollRange(IntPtr hWnd, int nBar, int lpMinPos, int lpMaxPos, bool Redraw);

        const int EM_LINESCROLL = 0x00B6;
        const int SB_HORZ = 0;
        const int SB_VERT = 1;
        const int SB_CTL = 2;

        #endregion

        // Пользовательские настройки программы
        private static MySettings Settings = new MySettings();

        public Form_Main(string[] args)
        {
            AboutVersion = curVersion.Major.ToString() + "." 
                + curVersion.Minor.ToString() + "."
                + curVersion.Build.ToString() + "."
                + curVersion.Revision.ToString();

            FWorking.Text = "Программа запускается...\n" + AboutInfo + " v." + AboutVersion;            

            FWorking.Start(); // Запуск потока оповещений            
            
            Thread.SpinWait(2000);

            try
            {
                InitializeComponent();
            }
            catch { }

            #region Инициализация звука
            // registering BASS.NET API
            BassNet.Registration("toxig@ya.ru", "2X182415113438");
            //BassInit();

            // Инициализация устройства воспроизведения
            try
            {
                if (v_initDefaultDevice.HasValue == false)
                {
                    v_initDefaultDevice = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                }
            }
            catch (NullReferenceException)
            {
                XtraMessageBox.Show("Не удалось иниициировать звуковое устройство!", "Инициализация звука", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
            #endregion
            
            try 
	        {
                // Обновление настроек старых версий
                if (Settings.p_update_settings) // для каждого нового обновления - будет загружен default - true
                {
                    Settings.Upgrade(); // загружаем настройки старой версии программы
                    Settings.p_update_settings = false; // необходимость дальнейшего обновления - выкл
                    Settings.p_App_Version = AboutVersion; // версия настроек
                    Settings.Save();
                }
	        }
	        catch (Exception)
	        {                                
            }            

            // Инициализация таблиц для плейлистов
            //FW.param_Operation_Text = "Загрузка плейлистов...";
            Init_PlayLists();

            // Инициализация эквалайзера
            FWorking.Text = "Загрузка эквалайзера...";
            Init_EQ();                                                                       

            // Настройки текста
            FWorking.Text = "Загрузка текста...";
            Init_Text();

            // Инициализация элементов главного окна
            FWorking.Text = "Загрузка главного окна...";
            Init_MainForm();

            // Парсинг входящих аргументов
            v_arg_pl = Parse_arguments_PL(args); // поиск плейлистов

            Init_Icon();

            // Лог воспроизведения
            Init_PlayLog();
        }

        #region Проверка обновлений
        // Поток проверка обновлений        
        private void FU_StartUpdate_Hidden() //// Запуск проверки без сообщений ошибок
        {
            CheckUpdates(false);
            if (FUpdate.NeedUpdate)
            {
                if (!Save_Main_Settings()) return;
                
                Application.Exit();
            }
        }

        // Поток проверка обновлений        
        private void FU_StartUpdate() //// Запуск проверки c сообщениями
        {
            try
            {
                CheckUpdates(true);
            }
            catch (Exception)
            {                
                throw;
            }                           

            if (FUpdate.NeedUpdate)
            {
                if (!Save_Main_Settings()) return;
                Application.Exit();
            }
        }

        // инициализация потока обновлений
        //Thread FU_Thread = new Thread(new ThreadStart(FU_StartUpdate or FU_StartUpdate_Hidden));
        //FU_Thread.Start();
        #endregion

        // загрузка программы
        private void Form_Main_Load(object sender, EventArgs e)
        {
            this.Text = "Парнас Плеер [" + curVersion.ToString() + "]";

            #region проверка обновления
            // проверка обновления без сообщений об ошибках
            if (Settings.p_check_updates)
            {
                //FWorking.Text = "Проверка обновлений...";
                FUpdate.Owner = this;
                Thread FU_Thread = new Thread(new ThreadStart(FU_StartUpdate_Hidden));
                FU_Thread.Start();
            }
            #endregion

            FWorking.Text = "Загрузка настроек...";
            Load_Prog_Settings();

            FWorking.Abort();
        }

        // Закрытие программы
        private void FormMain_Closing(object sender, FormClosingEventArgs e)
        {
            bool main_thread = Thread.CurrentThread.ManagedThreadId == v_MainWindowTread;

            // Сохранение настроек - только закрытие в главном потоке (не доп потоки)
            if (main_thread && !Save_Main_Settings()) e.Cancel = true;

            // Запуск обработчика обновлений
            if (!FUpdate.Start_Update()) e.Cancel = true;
        }

        /// <summary>Загрузка пользовательских настроек </summary>
        private void Load_Prog_User_Settings()
        {
            // палитра плей листов
            for (int i = 0; i < All_PlayLists.Length; i++)
            {
                if (All_PlayLists[i] != null) All_PlayLists[i].PM_Load_Design(Settings);                
            }            
        }

        /// <summary>Загрузка настроек </summary>
        private void Load_Prog_Settings()
        {
            try
            {
                FWorking.Text = "Загрузка настроек:\nЭквалайзер";
                #region Настройка эквалайзера
                panelControl_EQ.Visible = Settings.p_bar_show_EQ;
                panelControl_PlayBack.Visible = Settings.p_bar_show_PLAY;
                #endregion

                Pic_Logo.Dock = DockStyle.Fill;

                FWorking.Text = "Загрузка настроек:\nНонстоп";
                #region Настройка повтора (Нонстоп)
                if (Settings.p_Repeat_Status)
                {
                    sbtn_Repeat.Tag = "1";
                    //sbtn_Repeat.Text = "N (вкл)";
                    sbtn_Repeat.ImageIndex = 4;
                    sbt_status_plist.ImageIndex = 4;
                    sbtn_Repeat.ToolTip = "Нонстоп (включен)";
                    iRepeat_Check.Checked = true;
                }
                else
                {
                    sbtn_Repeat.Tag = "0";
                    //sbtn_Repeat.Text = "N";
                    sbtn_Repeat.ImageIndex = 5;
                    sbt_status_plist.ImageIndex = 5;
                    sbtn_Repeat.ToolTip = "Нонстоп (отключен)";
                    iRepeat_Check.Checked = false;
                }
                #endregion

                // Фейдер время
                FWorking.Text = "Загрузка настроек:\nФейдер";
                textEdit_FadeTime.Text = Settings.p_Fade_time.ToString();
                v_FadeTime_Pause = Settings.p_Fade_time * 1000; // затухание при паузе
                v_FadeTime_Hot = Settings.p_Fade_time * 1000; // затухание при горячем пуске

                // загружаем арг плейлист в горячий список
                if (v_arg_pl.Length > 0)
                {
                    All_PlayLists[0].PM_Load_List(v_arg_pl[0]);
                    if (xTabCtrl_PlayLists.TabPages[0].PageVisible) xTabCtrl_PlayLists.SelectedTabPageIndex = 0;
                    //v_play_list_id_active = Settings.p_PL_ActiveListID;
                    //xTabCtrl_PlayLists.SelectedTabPageIndex = v_play_list_id_active;
                }

                // загрузка плейлистов
                string[] PL_List_Path = Settings.p_PL_OpenFiles.Split('\n'); //список файлов

                v_play_list_id_active = 0;

                int pl_num = 0;
                for (int i = 0; i < PL_List_Path.Length; i++) // парсинг ранее открытых ПЛ из настроек
                {
                    if (i == 0 && PL_List_Path[i].Trim() == "") pl_num++; // первый для горячего листа

                    if (PL_List_Path[i].Trim() != "" && All_PlayLists.Length > pl_num)
                    {                                               
                        // создаем вкладку                        
                        if (xTabCtrl_PlayLists.TabPages.Count < pl_num+1) // нет вкладок - создаем
                        {
                            xTabCtrl_PlayLists.TabPages.Add("[" + (pl_num) + "]");
                            Init_New_PlayList(pl_num);
                        }

                        All_PlayLists[pl_num].PM_Load_List(PL_List_Path[i]);

                        if (Settings.p_PL_ActiveListID == i) v_play_list_id_active = pl_num;

                        pl_num++; 
                    }
                }

                // активный плей лист
                xTabCtrl_PlayLists.SelectedTabPageIndex = v_play_list_id_active;               

                FWorking.Text = "Загрузка настроек:\nРазмер окна";

                if (Settings.p_Form_Maximized) this.WindowState = FormWindowState.Maximized;
                else this.WindowState = FormWindowState.Normal;

                panelControl_Right.Width = Settings.p_PL_PanelWidth;                
                panelControl_Hot_PL.Height = Settings.p_HotList_Height;
                iCheck_Tags_Files.Checked = Settings.p_Check_Tags;

                Load_Prog_User_Settings();
            }
            catch (Exception e)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    e.Message + "\n" + e.StackTrace, 
                    "Ошибка загрузки настроек приложения", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }

        // Сохранение пользовательских настроек
        public bool Save_Main_Settings()
        {
            // Признак главного потока
            bool is_main_thread = Thread.CurrentThread.ManagedThreadId == v_MainWindowTread;

            // Освобождаем медиа данные
            MainStream.v_stream_status = StreamStatus.FREE;
            SlaveStream.v_stream_status = StreamStatus.FREE;
            Bass.BASS_Free();

            #region Настройка плейлистов
            int cur_list = v_play_list_id_active;
            Settings.p_PL_OpenFiles = "";

            for (int i = 0; i < xTabCtrl_PlayLists.TabPages.Count; i++)
            {
                // Если главный поток - меняем вкладки 
                if (is_main_thread && All_PlayLists[i].is_Changed) 
                    xTabCtrl_PlayLists.SelectedTabPageIndex = i; // меняем ативную вкладку ПЛ

                if (!PL_PlayLists_Save(i)) // закрытие списков
                {
                    FUpdate.NeedUpdate = false;
                    return false;
                }

                Settings.p_PL_OpenFiles += All_PlayLists[i].pl_FilePath + "\n";
            }

            // возврат к начальному активному плейлисту if (is_main_thread)            
            if (is_main_thread && xTabCtrl_PlayLists.SelectedTabPageIndex != cur_list) 
                xTabCtrl_PlayLists.SelectedTabPageIndex = cur_list; 
            #endregion

            #region Остальные настройки
            Settings.p_bar_show_EQ = panelControl_EQ.Visible;
            Settings.p_bar_show_PLAY = panelControl_PlayBack.Visible;
            Settings.p_PL_ActiveListID = xTabCtrl_PlayLists.SelectedTabPageIndex;
            Settings.p_PL_PanelWidth = panelControl_Right.Width;

            if (this.WindowState == FormWindowState.Maximized)
                Settings.p_Form_Maximized = true;
            else
                Settings.p_Form_Maximized = false;

            if (sbtn_Repeat.Tag.ToString() == "1")
                Settings.p_Repeat_Status = true;
            else
                Settings.p_Repeat_Status = false;

            Settings.p_Fade_time = int.Parse(textEdit_FadeTime.Text);

            Settings.p_Check_Tags = iCheck_Tags_Files.Checked;

            Settings.p_HotList_Height = panelControl_Hot_PL.Height; 
            #endregion

            Settings.Save();           

            return true;
        }

        #region Инициализация элементов
        private void Init_PlayLists()
        {
            v_play_list_id_active = 0;
            
            for (int i = 0; i < xTabCtrl_PlayLists.TabPages.Count; i++)
            {
                Init_New_PlayList(i);
            }

            Update_Visual_List_Color();
        }

        // новый плей лист
        private void Init_New_PlayList(int i)
        {
            
            All_PlayLists[i] = new Control_PlayList();
            Control_PlayList PL = All_PlayLists[i];

            PL.Dock = DockStyle.Fill;
            PL.grid_PlayList.Tag = i;            
            PL.FWorking = FWorking;
            PL.v_PList_ID = i;
            PL.v_Check_Tags = Settings.p_Check_Tags;
            //PL.v_Check_Exist = Settings.p_Check_Exist; // всегда включено (нет опции в меню)

            int max_pl = xTabCtrl_PlayLists.TabPages.Count - 1;

            // горячий список
            if (i == 0)
            {
                if (Settings.p_HotList_Position)
                {
                    panelControl_Hot_PL.Controls.Add(PL);
                    xTabCtrl_PlayLists.TabPages[i].PageVisible = false;
                    panelControl_Hot_PL.Visible = true;
                }
                else
                {
                    xTabCtrl_PlayLists.TabPages[i].Controls.Add(PL);
                    xTabCtrl_PlayLists.TabPages[i].PageVisible = true;
                    panelControl_Hot_PL.Visible = false;
                }
                
                xTabCtrl_PlayLists.TabPages[i].Text = "[!]";
                PL.labelControl_header.Text = "Горячий список";

                All_PlayLists[i].pl_HotList = true;
            }

            // остальные листы
            if (i > 0 /*&& i != max_pl*/)
            {
                xTabCtrl_PlayLists.TabPages[i].Controls.Add(All_PlayLists[i]);                
                xTabCtrl_PlayLists.TabPages[i].Text = "[" + (i) + "]";
            }

            /*if (i == max_pl)
            {
                xTabCtrl_PlayLists.TabPages[i].Controls.Add(All_PlayLists[i]);
                All_PlayLists[i].labelControl_header.Text = "История треков";
                xTabCtrl_PlayLists.TabPages[i].Text = "[H]";
                All_PlayLists[i].pl_History = true;
            }*/

            // события списка
            All_PlayLists[i].grid_PlayList.DoubleClick += new System.EventHandler(this.grid_PlayList_DoubleClick);                        
            All_PlayLists[i].gv_PlayList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_PlayList_KeyDown);
            //All_PlayLists[i].grid_PlayList.MouseEnter += new System.EventHandler(this.grid_PlayList_MouseEnter);            
            All_PlayLists[i].grid_PlayList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.grid_PlayList_MouseClick);
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

        // Главное окно
        private void Init_MainForm()
        {
            // Настройки отображения окна
            FView_Save();

            // скролл для настроек фейдера
            textEdit_FadeTime.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.TEdit_MouseWheel);
        }

        // Настройка текстового редактора
        private void Init_Text()
        {
            this.panelControl_All.Dock = DockStyle.None;
            this.panelControl_All.Left = v_FullScreen_Delta; // Location.X = 10;
            this.panelControl_All.Top = v_FullScreen_Delta - 2; // Location.Y = 10;
            this.panelControl_All.Height -= v_FullScreen_Delta * 2 - 3;
            this.panelControl_All.Width -= v_FullScreen_Delta;
            this.panelControl_All.Dock = DockStyle.Fill;

            Text_RTEditor.sbtn_Close.Click += new EventHandler(sbtn_TextEditor_Close_Click);
        }

        // Асоциация файлов
        private void Init_Icon()
        {
            FileAssociation FIcon = new FileAssociation();

            if (!FIcon.IsAssociated)
            {
                FIcon.Remove();
                FIcon.Associate("Плейлист для Парнас Плеера", Path.GetDirectoryName(Application.ExecutablePath) + "\\pm_file.ico");
            }
        }
        
        // Лог воспроизведения - txt
        private void Init_PlayLog()
        {
            v_dir_program = System.IO.Directory.GetCurrentDirectory(); // текущая папка программы;
            v_dir_log = v_dir_program + "\\Log"; // папка для созранения логов

            if (!Directory.Exists(v_dir_log)) Directory.CreateDirectory(v_dir_log);

            v_file_log = String.Format("{0}\\Log_play_{1}{2}", v_dir_log,System.DateTime.Now.ToString("yyyy.MM.dd"),".txt");
        }
        #endregion

        #region Операции с файлом плейлиста

        private void gv_PlayList_AddHistory(int list_id, int row_id)
        {
            if (All_PlayLists[list_id].gv_PlayList.FocusedRowHandle < 0) return;

            DataRow FRow = All_PlayLists[list_id].dt_FileData.Rows[row_id];
            DataRow LRow = All_PlayLists[list_id].dt_ListData.Rows[row_id];

            int max_pl = xTabCtrl_PlayLists.TabPages.Count - 1;
            int row_max = All_PlayLists[max_pl].dt_ListData.Rows.Count;

            All_PlayLists[max_pl].dt_FileData.Rows.Add(FRow.ItemArray);
            All_PlayLists[max_pl].dt_ListData.Rows.Add(LRow.ItemArray);

            All_PlayLists[max_pl].dt_ListData.Rows[row_max]["Date"] = System.DateTime.Now;
                        
            System.IO.File.AppendAllText(v_file_log,
                                         String.Format("{0} {1}\r\n",System.DateTime.Now.ToString(),LRow[0].ToString()),
                                         System.Text.Encoding.GetEncoding(1251));
            
            //FileInfo file_log = new FileInfo(v_file_log);
            //if (!file_log.Exists) file_log.Create();

            //StreamWriter sw = new StreamWriter(v_file_log); //(@".\arSYCalendars.txt")
            //StreamWriter sw = file_log.AppendText();
            //sw.WriteLine("{0}  {1}\n", System.DateTime.Now.ToString(), LRow[0].ToString());
            //sw.Close();
        }

        private void gv_PlayList_KeyDown(object sender, KeyEventArgs e)
        {
            if (All_PlayLists[v_play_list_id_active].gv_PlayList.FocusedRowHandle < 0) return;

            int Row_ID = All_PlayLists[v_play_list_id_active].dt_ListData.Rows.IndexOf(All_PlayLists[v_play_list_id_active].gv_PlayList.GetFocusedDataRow());

            // добавление в горячий лист
            if (e.KeyCode == v_Key_to_HotList && v_play_list_id_active != 0)
            {                                                    
                DataRow FRow = All_PlayLists[v_play_list_id_active].dt_FileData.Rows[Row_ID];                    
                DataRow LRow = All_PlayLists[v_play_list_id_active].dt_ListData.Rows[Row_ID];

                if (LRow[11].ToString().StartsWith("10"))
                {
                    All_PlayLists[v_play_list_id_active].Focused_Row_Blink(2);
                    return;
                }

                foreach (DataRow ChRow in All_PlayLists[0].dt_ListData.Rows)
                {
                    // остановка
                    if (ChRow[0] == LRow[0])
                    {
                        All_PlayLists[v_play_list_id_active].Focused_Row_Blink(1);
                        return;
                    }
                }

                All_PlayLists[0].dt_FileData.Rows.Add(FRow.ItemArray);
                All_PlayLists[0].dt_ListData.Rows.Add(LRow.ItemArray);

                // мигание строки
                All_PlayLists[v_play_list_id_active].Focused_Row_Blink();
            }

            // изменение тэгов
            if (e.KeyCode == Keys.F2)
            {
                FTags.File_Path = All_PlayLists[v_play_list_id_active].dt_FileData.Rows[Row_ID][0].ToString();
                FTags.MainStream = MainStream;
                FTags.ShowDialog();                
                All_PlayLists[v_play_list_id_active].Check_Exists_Row(Row_ID);
            }

            // Добавление муз файла
            if (e.KeyCode == Keys.Insert)
            {
                iEdit_AddMuzFile_ItemClick(null, null);
            }

            // Удаление муз файла
            if (e.KeyCode == Keys.Delete)
            {
                iEdit_DelMuzFile_ItemClick(null, null);
                iSave_PlayList.Enabled = true;
            }            
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
                    sbtn_Repeat.Text = ""; //N (вкл)
                    sbtn_Repeat.ImageIndex = 4;
                    sbt_status_plist.ImageIndex = 4;
                    sbtn_Repeat.ToolTip = "Нонстоп (включен)";
                    iRepeat_Check.Checked = true;
                    break;
                case "1":                    
                    sbtn_Repeat.Tag = "0";
                    sbtn_Repeat.Text = ""; // N
                    sbtn_Repeat.ImageIndex = 5;
                    sbt_status_plist.ImageIndex = 5;
                    sbtn_Repeat.ToolTip = "Нонстоп (отключен)";
                    iRepeat_Check.Checked = false;
                    break;
                /*case "2":
                    sbtn_Repeat.Tag = "0";
                    sbtn_Repeat.Text = "R (1)";
                    sbtn_Repeat.ToolTip = "Повтор (один трек)";
                    break;*/
            }
            
            toolTipController.ShowHint(sbtn_Repeat.ToolTip);            
        }        

        // ГОРЯЧИЙ ФЕЙДЕР
        private void sbtn_FadeNow_Click(object sender, EventArgs e)
        {
            // трек играет + не включен фейдер
            if (MainStream.v_stream_status == StreamStatus.PLAY && MainStream.v_FadeActive == false)
            {
                sbtn_FadeNow.ImageIndex = 6; // 3 - Fade Down; 6 - Fade Up
                // длина фейдера = 0: вкл паузу
                if (v_FadeTime_Hot <= 0) MainStream.v_stream_status = StreamStatus.PAUSE;
                // иначе вкл фейдер
                else
                {
                    timer_playng.Interval = 100; // увеличиваем частоту проверки
                    MainStream.p_fade_channel(0, v_FadeTime_Hot);
                    MainStream.v_FadeStopTime = v_FadeTime_Hot+100;
                    MainStream.v_FadePostAction = StreamStatus.PAUSE;
                    MainStream.v_FadeActive = true;

                    toolTipController.ShowHint("Фейдер: вниз [" + Math.Round((double)v_FadeTime_Hot / 1000, 1).ToString() + " сек]");                    
                }
            }
            // трек не играет или фейдер был включен 
            else
            {
                sbtn_Pause.ImageIndex = 0;

                if (MainStream.v_stream == 0)
                {
                    PlayCurFile_FromList(v_play_list_id_active, 0);
                    MainStream.p_fade_channel(MainStream.v_stream_volume, v_FadeTime_Hot, 0);

                    toolTipController.ShowHint("Фейдер: вверх [" + Math.Round((double)v_FadeTime_Hot / 1000, 1).ToString() + " сек]");
                    //toolTipController.ShowHint("Фейдер: нет трека");
                }
                else
                {
                    sbtn_FadeNow.ImageIndex = 3; // 3 - Fade Down; 6 - Fade Up                    

                    if (v_FadeTime_Hot <= 0) MainStream.v_stream_status = StreamStatus.PLAY;
                    else
                    {
                        // если уже пауза
                        switch (MainStream.v_stream_status)
                        {
                            case StreamStatus.PLAY:
                                MainStream.p_fade_channel(MainStream.v_stream_volume, v_FadeTime_Hot); // с текущ громкости
                                break;
                            default:
                                MainStream.p_fade_channel(MainStream.v_stream_volume, v_FadeTime_Hot, 0); // громкость в ноль (для корректного фейдером вывода в плей, с нуля)
                                break;
                        }

                        MainStream.v_FadeActive = false;
                        MainStream.v_FadeStopTime = 0;
                        MainStream.v_FadePostAction = StreamStatus.NONE;

                        toolTipController.ShowHint("Фейдер: вверх [" + Math.Round((double)v_FadeTime_Hot / 1000, 1).ToString() + " сек]");

                        Show_Logo(false);
                    }
                }
            }
        }

        // ПЛЕЙ
        private void buttonPlay_Click(object sender, EventArgs e)
        {            
            PlayCurFile_FromList(v_play_list_id_active, 0); // текущий трек
        }

        // СТОП
        private void buttonStop_Click(object sender, EventArgs e)
        {            
            if (MainStream.v_stream_status == StreamStatus.FREE)
            {                            
                // registering BASS.NET API
                try
                {
                    Bass.BASS_Free();
                    v_initDefaultDevice = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                    Label_InfoLine.Text = "Ининциализация звуковой карты (default)";
                }
                catch (NullReferenceException)
                {
                    XtraMessageBox.Show("Не удалось иниициировать звуковое устройство!", "Инициализация звука", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
            }

            MainStream.v_stream_status = StreamStatus.FREE;            

            Label_InfoLine.Text = "Остановлено"; v_WaytDelay = 3;                        
            //sbtn_Pause.Text = "Play";
            sbtn_Pause.ImageIndex = 0; // 0 - Play; 1 - Pause

            Update_Paly_Status();
        }

        // ВПЕРЕД
        private void buttonNext_Click(object source, EventArgs e)
        {                  
            Update_Paly_Status();
            PlayCurFile_FromList(MainStream.v_PL_List_ID_played, 1);  // следующий трек          
        }

        // НАЗАД
        private void buttonPrev_Click(object sender, EventArgs e)
        {            
            Update_Paly_Status();
            PlayCurFile_FromList(MainStream.v_PL_List_ID_played, -1); // предыдущий трек
        }

        // ПАУЗА / PLAY
        private void btn_PausePlay_Click(object sender, EventArgs e)
        {
            int Row_ID = All_PlayLists[v_play_list_id_active].dt_ListData.Rows.IndexOf(All_PlayLists[v_play_list_id_active].gv_PlayList.GetFocusedDataRow());
            
            if (MainStream.v_stream_status == StreamStatus.PLAY) // трек играет
            {
                MainStream.v_stream_status = StreamStatus.PAUSE; // ставим на паузу
                Label_InfoLine.Text = "Пауза";                
                sbtn_Pause.ImageIndex = 0; // 0 - Play; 1 - Pause
                sbtn_FadeNow.ImageIndex = 6; // 3 - Fade Down; 6 - Fade Up    

                MainStream.v_FadeActive = false;
            }
            else // не играет
                if (MainStream.v_stream_status == StreamStatus.PAUSE // трек на паузе
                    && MainStream.v_PL_List_ID_played == v_play_list_id_active  // ПЛ листы совпадают
                    && MainStream.v_PL_Row_ID == Row_ID) // треки совпадают
                { 
                    // продолжение pause -> start - (с полной громкостью / без фейдера)
                    int pos = EQ_Main.pbc_equal_main.Position;
                    int max = EQ_Main.pbc_equal_main.Properties.Maximum;

                    MainStream.v_stream_volume = (float)pos / max;
                    MainStream.v_stream_status = StreamStatus.PLAY;

                    Label_InfoLine.Text = "Проигрывается";
                    
                    sbtn_Pause.ImageIndex = 1; // 0 - Play; 1 - Pause
                    sbtn_FadeNow.ImageIndex = 3; // 3 - Fade Down; 6 - Fade Up
                }
                else //инициируем новое воспроизведение
                {
                    GC.Collect();
                    GC.WaitForFullGCComplete();
                    PlayCurFile_FromList(v_play_list_id_active, 0); // текущий трек
                    Label_InfoLine.Text = "Проигрывается";                    
                    sbtn_Pause.ImageIndex = 1; // 0 - Play; 1 - Pause
                    sbtn_FadeNow.ImageIndex = 3; // 3 - Fade Down; 6 - Fade Up
                }

            Update_Paly_Status();

            // Откл Заставки
            Show_Logo(false);
        }

        private void Show_Logo(bool need_show)
        {
            if (need_show || !System.IO.File.Exists(MainStream.v_TextFileName))
            { // вкл лого
                Pic_Logo.Visible = true;
                RTBox_TextFile.Visible = false;
                Pic_Logo.Dock = DockStyle.Fill;
            }
            else
            {
                Pic_Logo.Visible = false;
                RTBox_TextFile.Visible = true;
                v_Logo_NoActions_Time = 0;
            }
        }

        // Фейдер - Изменение интервла фейдера
        private void sbt_FadeTime_Change(object sender, EventArgs e)
        {
            int fader_time = int.Parse(textEdit_FadeTime.Text);
            int fader_delta = int.Parse(((SimpleButton)sender).Tag.ToString());

            fader_time += fader_delta;
            if (fader_time < 0) fader_time = 0;
            if (fader_time > 60) fader_time = 60; // ограничение допустимого значения 
            textEdit_FadeTime.Text = fader_time.ToString();

            v_FadeTime_Pause = fader_time * 1000; // затухание при паузе
            v_FadeTime_Hot = fader_time * 1000; // затухание при горячем пуске

            toolTipController.ShowHint("Фейдер [" + (fader_time).ToString() + " сек]");
        }

        // Фейдер - Скрол на текстовом поле
        private void TEdit_MouseWheel(object sender, MouseEventArgs e)
        {
            int value = 0;
            try
            {
                value = int.Parse(((TextEdit)sender).Text);
            }
            catch { }

            if (e.Delta > 0)
            {
                value += 1;
            }
            else
            {
                value -= 1;
            }

            switch (((TextEdit)sender).Tag.ToString())
            {
                case "Fader":
                    // ограничение допустимого значения 
                    if (value < 0) value = 0;
                    if (value > 60) value = 60;
                    v_FadeTime_Pause = value * 1000; // затухание при паузе
                    v_FadeTime_Hot = value * 1000; // затухание при горячем пуске
                    toolTipController.ShowHint("Фейдер [" + (value).ToString() + " сек]");
                    break;
            }

            ((TextEdit)sender).Text = value.ToString();
        }

        // Перейти на последний запущенный трек
        private void sbt_show_cur_play(object sender, EventArgs e)
        {
            if (MainStream != null && MainStream.v_FileName !="")
            {
                if (MainStream.v_PL_List_ID_played == 0 && xTabCtrl_PlayLists.TabPages[0].PageVisible == false)
                {
                    panelControl_Hot_PL.Focus();
                    All_PlayLists[0].gv_PlayList.Focus();
                    v_play_list_id_active = 0;
                }
                else // меняем ативную вкладку ПЛ 
                {
                    xTabCtrl_PlayLists.SelectedTabPageIndex = MainStream.v_PL_List_ID_played;
                }

                int Row_Handle = All_PlayLists[MainStream.v_PL_List_ID_played].gv_PlayList.GetRowHandle(MainStream.v_PL_Row_ID);
                All_PlayLists[MainStream.v_PL_List_ID_played].gv_PlayList.FocusedRowHandle = Row_Handle;
                
                
                Update_Visual_List_Color();

                All_PlayLists[MainStream.v_PL_List_ID_played].Focused_Row_Blink(3);
            }
        }
        #endregion

        #region Трекеры / ползунки

        // Трек Время - переход на указанную точку
        private void progressBarControl_PlayPosition_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && MainStream.v_stream != 0)
            {
                v_WaytDelay = 2;

                int pos = (int)((ProgressBarControl)sender).Properties.Maximum * (e.X + 3) / ((ProgressBarControl)sender).Width;
                int max = ((ProgressBarControl)sender).Properties.Maximum;
                int min = ((ProgressBarControl)sender).Properties.Minimum;
                if (pos > max) { pos = max; }
                if (pos < min) { pos = min; }

                ((ProgressBarControl)sender).Position = pos;
                Bass.BASS_ChannelSetPosition(MainStream.v_stream, (double)((ProgressBarControl)sender).Position);
                
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

                v_WaytDelay = 10;
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

                v_WaytDelay = 3;
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

        public void InitPlayFile(int list_id, int row_shift)
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

            DataRow Row = All_PlayLists[list_id].dt_ListData.Rows[Row_ID];
            
            // параметры файла
            MainStream.v_PL_List_ID_played = list_id;
            MainStream.v_PL_Row_ID = Row_ID;
            MainStream.v_FileName = Row[0].ToString();
            MainStream.v_FileNameSrt = Path.GetFileName(MainStream.v_FileName);
            MainStream.v_TextFileName = Row[1].ToString();
            MainStream.v_FadeActive = false;

            #region Загрузка текста
            Label_InfoLine.ToolTip = "";
            string TextMsg = "";

            if (System.IO.File.Exists(MainStream.v_TextFileName))
            {
                RTBox_TextFile.LoadFile(MainStream.v_TextFileName);                                
                RTBox_TextFile.Rtf = RTBox_TextFile.Rtf.Insert(RTBox_TextFile.Rtf.LastIndexOf('}') - 1, "\\par"); //"\\par" - перенос строки в конце файла                
                v_RTEdit_no_resize = false; // флаг масштабирования текста
                RTBox_TextFile_TextChanged(null, null);
                Label_InfoLine.ToolTip = "Текст файл: " + MainStream.v_TextFileName;

                Show_Logo(false);
            }
            else
            {
                v_Logo_NoActions_Time = v_Logo_StartTime;
                Show_Logo(true);

                v_WaytDelay = 3;
                RTBox_TextFile.Font = new Font("Tahoma", 20, FontStyle.Bold);

                if (MainStream.v_TextFileName == "")
                {
                    TextMsg = " * Текст-файл: не выбран"; //Текстовый файл не присвоен
                    Label_InfoLine.ToolTip = "Текст файл: не выбран";
                }
                else
                {
                    TextMsg = " * Текст-файл: нет на Диске \n *** " + MainStream.v_TextFileName;
                    Label_InfoLine.ToolTip = "Текста нет на диске:" + MainStream.v_TextFileName;
                }
            }
            #endregion

            #region Загрузка музыки
            if (System.IO.File.Exists(MainStream.v_FileName))
            {
                Label_InfoLine.ToolTip = "Муз файл: " + MainStream.v_FileName + "\n" + Label_InfoLine.ToolTip;
                MainStream.v_FileNameSrt = Path.GetFileName(MainStream.v_FileName); // муз файл

                MainStream.p_Init_NewPlayStream(MainStream.v_FileName, v_initDefaultDevice); // инициализация звукового потока                    
                MainStream.v_streem_need_free = true; // поток занят (необходимо освобождать после использования)

                // иниц эквалайзера для потока
                EQ_Main.MainStream = MainStream;
                EQ_Main.v_stream = MainStream.v_stream;
                EQ_Main.Init_fx(MainStream.v_stream);
                EQ_Main.Row = Row;
                EQ_Main.Eq_row_to_array(); // загрузка эквалайзера
                EQ_Main.Eq_array_to_object();
                EQ_Main.Eq_array_to_stream();
                EQ_Main.Cur_PL = All_PlayLists[v_play_list_id_active];

                // PlayFile(); // запуск воспроизведения

                if (list_id == 0) 
                    //lbc_playlist.Text = v_Name_HotList.Substring(0, 1);
                    sbt_status_plist.Text = v_Name_HotList.Substring(0, 1);
                else 
                    //lbc_playlist.Text = list_id.ToString();
                    sbt_status_plist.Text = list_id.ToString();
            }
            else
            {
                MainStream.v_stream = 0;
                Label_InfoLine.ToolTip = "Нет Муз файл: " + MainStream.v_FileName + "\n" + Label_InfoLine.ToolTip;
                Label_InfoLine.Text = "Нет файла: " + Path.GetFileName(MainStream.v_FileName);
                if (TextMsg != "") TextMsg += "\n\n * Муз-файл: нет на Диске\n *** " + MainStream.v_FileName;
            }
            
            if (TextMsg != "")
            {
                //v_RTEdit_no_resize = true;                          
                RTBox_TextFile.ZoomFactor = 1;
                RTBox_TextFile.Text = TextMsg;                
            }
            #endregion

            #region Обновление таймера
            timerForCurrentPosition_Tick(null, null);

            timer_playng.Enabled = false;
            timer_playng.Interval = 100;
            //Label_InfoLine.Text = "Проигрывается";

            //string[] tagID3V1 = Bass.BASS_ChannelGetTagsID3V1(MainStream.v_stream);
            //string[] tagID3V2 = Bass.BASS_ChannelGetTagsID3V2(MainStream.v_stream);
            progressBarControl_PlayPosition.Properties.Minimum = 0;
            progressBarControl_PlayPosition.Properties.Maximum = int.Parse(((Bass.BASS_ChannelBytes2Seconds(MainStream.v_stream, Bass.BASS_ChannelGetLength(MainStream.v_stream)).ToString().Split(',', '.'))[0]));
            timer_playng.Enabled = true;
            #endregion

            // Очистка фильтра
            All_PlayLists[list_id].btnEdit_Find.Text = "";

            // Добавляем в историю
            // gv_PlayList_AddHistory(list_id, Row_ID);
        }

        //Запуск воспроизведения
        public void PlayCurFile_FromList(int list_id, int row_shift)
        {
            // играет трек
            if (MainStream.v_stream_status == StreamStatus.PLAY)
            {
                int Row_ID = All_PlayLists[list_id].dt_ListData.Rows.IndexOf(All_PlayLists[list_id].gv_PlayList.GetFocusedDataRow()) + row_shift;

                // без нонстопа или тот же трек
                if ( (sbtn_Repeat.Tag.ToString() != "1") || // нет нонстопа
                     (MainStream.v_PL_List_ID_played == list_id && MainStream.v_PL_Row_ID == Row_ID) // или запуск того же трека
                    )
                {
                    // мгновенная остановка трека
                    MainStream.v_stream_status = StreamStatus.FREE;
                }
                else
                {
                    // плавная остановка трека
                    // кросс фейдер между треками при нонстопе
                    #region Кросс фейдер между треками - ручной переход
                    toolTipController.ShowHint("Фейдер + нонстоп: " + v_FadeTime_Pause / 1000 + " сек");

                    // копируем текущий поток
                    SlaveStream = MainStream;

                    // сводим в ноль текущий поток
                    SlaveStream.p_fade_channel(0, v_FadeTime_Pause); // громкость в 0
                    SlaveStream.v_FadeStopTime = v_FadeTime_Pause;
                    SlaveStream.v_FadePostAction = StreamStatus.FREE; // статус после фейдера
                    SlaveStream.v_FadeActive = true;
                    #endregion
                }                
            }


            // запуск нового трека
            MainStream.v_streem_need_free = false;
            InitPlayFile(list_id, row_shift);

            if (MainStream.v_stream != 0)
            {
                int pos = EQ_Main.pbc_equal_main.Position;
                int max = EQ_Main.pbc_equal_main.Properties.Maximum;

                MainStream.v_stream_volume = (float)pos / max;
                //запуск трека
                MainStream.p_fade_channel(MainStream.v_stream_volume, 0, 0);                
                if (!MainStream.v_FadeActive) MainStream.v_FadePostAction = StreamStatus.NONE; 
            }
            else MainStream.v_FadePostAction = StreamStatus.FREE;

            if (panelControl_TextEditor.Visible) TextEditor_Hide(); // скрываем редактор - если был запущен
            sbtn_FadeNow.ImageIndex = 3; // 3 - Fade Down; 6 - Fade Up

            Update_Paly_Status();
        }        

        //Запуск воспроизведения
        public void PlayFile()
        {            
            if (v_initDefaultDevice.Value) // активация устройства воспроизведения
            {
                MainStream.v_stream_status = StreamStatus.PLAY;                  
            }

            timerForCurrentPosition_Tick(null, null);

            timer_playng.Enabled = false;
            timer_playng.Interval = 100;
            Label_InfoLine.Text = "Проигрывается";

            //string[] tagID3V1 = Bass.BASS_ChannelGetTagsID3V1(MainStream.v_stream);
            //string[] tagID3V2 = Bass.BASS_ChannelGetTagsID3V2(MainStream.v_stream);
            
            progressBarControl_PlayPosition.Properties.Minimum = 0;
            progressBarControl_PlayPosition.Properties.Maximum = int.Parse(((Bass.BASS_ChannelBytes2Seconds(MainStream.v_stream, Bass.BASS_ChannelGetLength(MainStream.v_stream)).ToString().Split(',', '.'))[0]));

            Update_Paly_Status();
            timer_playng.Enabled = true;                        
        }

        // играть выбранную строку
        private void grid_PlayList_DoubleClick(object sender, EventArgs e)
        {            
            if (All_PlayLists[v_play_list_id_active].dt_ListData != null && 
                All_PlayLists[v_play_list_id_active].dt_ListData.Rows.Count != 0)
            {                
                buttonPlay_Click(null, null);
                sbtn_Pause.ImageIndex = 1; // 0 - Play; 1 - Pause
            }
        }

        // играть выбранную строку
        private void listBoxPlayList_DoubleClick(object sender, EventArgs e)
        {
            //PlayFile(((FileInListBox)listBoxPlayList.Items[listBoxPlayList.SelectedIndex]).FileName);
            buttonPlay_Click(null, null);
        }

        // очистка списка ???
        private void buttonClearList_Click(object sender, EventArgs e)
        {
            ClearList(ActListID());
            xtraTabPage1.Text = "[1]";
        }

        private int ActListID()
        {
            return xTabCtrl_PlayLists.SelectedTabPageIndex;
        }

        private void ClearList(int ListID) // ???
        {
            //ListID
            No_Func();
        }

        private void iOpen_PlayList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!All_PlayLists[v_play_list_id_active].PL_Save_Changes()) return;

            openFileDialog.Title = "Выбор плейлиста";
            openFileDialog.Filter = "Плейлист (*.pmp)|*.pmp|Все файлы (*.*)|*.*";
            openFileDialog.InitialDirectory = Settings.p_DefFolder_PList;
            openFileDialog.FileName = "";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
            
            if (openFileDialog.FileName.Length != 0)
            {
                FWorking.Text = "Загрузка плейлиста: \"" + Path.GetFileNameWithoutExtension(openFileDialog.FileName) + "\"";
                FWorking.Start();                
                
                Control_PlayList Cur_PL = All_PlayLists[v_play_list_id_active];
                Cur_PL.pl_FilePath = openFileDialog.FileName; // файл плейлиста                
                Cur_PL.PM_Load_List(Cur_PL.pl_FilePath); // Загрузка файла                

                // Папка плейлистов "по умолчанию"
                Settings.p_DefFolder_PList = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                FWorking.Abort();
            }
        }

        private void grid_PlayList_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                v_play_list_id_active = Convert.ToInt32((sender as DevExpress.XtraGrid.GridControl).Tag);
            }
            catch
            {
                v_play_list_id_active = 0;
            }

            Update_Visual_List_Color();
        }        

        private void xTabCtrl_PlayLists_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            v_play_list_id_active = xTabCtrl_PlayLists.SelectedTabPageIndex;
            Update_Visual_List_Color();
        }

        /// <summary>Цвет заголовка активной вкладки</summary>
        private void Update_Visual_List_Color()
        {
            for (int i = 0; i <  All_PlayLists.Length; i++)
            {
                if (All_PlayLists[i] != null)
                {
                    if (i == v_play_list_id_active)
                    {
                        All_PlayLists[i].labelControl_header.Appearance.BackColor = Color.Lime;
                        All_PlayLists[i].labelControl_header.Appearance.ForeColor = Color.Black;
                    }
                    else
                    {
                        All_PlayLists[i].labelControl_header.Appearance.BackColor = Color.Empty;
                        All_PlayLists[i].labelControl_header.Appearance.ForeColor = Color.Empty;
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
            if (v_initDefaultDevice.HasValue != false && MainStream.v_stream != 0)
            {
                Proces_Play_Mods();
                Update_Paly_Status();
            }
        }

        // Информационные статусы воспроизведения трека
        private void Update_Paly_Status()
        {
            // значение времени проигрывания
            int d = MainStream.v_play_pos;
            int all = MainStream.v_play_max;
                        
            if (d < 0) d = 0;
            if (all < 0) all = 0;
            if (d > all) d = all;

            // синзронизация таймера (100, если первая секунда, иначе 1000)
            if (d == 0) timer_playng.Interval = 100;
            else if (timer_playng.Interval != 1000) timer_playng.Interval = 1000;

            progressBarControl_PlayPosition.Position = d;
            progressBarControl_PlayPosition.Properties.DisplayFormat.FormatString = StrToEscapeStr("- "+SecondsToTimeFormat(all-d));

            // инверсия времени
            if (!iTime_Inverse_Check.Checked)
            {
                lbc_time_back.Text = "-" + SecondsToTimeFormat(all - d);
                lbc_time.Text = SecondsToTimeFormat(d);
                lbc_time_minus.Text = " ";
            }
            else
            {
                lbc_time_back.Text = SecondsToTimeFormat(d);
                lbc_time.Text = SecondsToTimeFormat(all - d);
                lbc_time_minus.Text = "-";
            }
            
            // Название файла в инфо панели
            if (MainStream.v_FileNameSrt != null && MainStream.v_FileNameSrt.Length > 0 && v_WaytDelay == 0)
                Label_InfoLine.Text = MainStream.v_FileNameSrt;

            // Статус воспроизведения
            switch (MainStream.v_stream_status)
            {                
                case StreamStatus.PAUSE:
                        if (MainStream.v_FadeEnd)
                        {
                            sbt_status_play.ImageIndex = 1; // 0-play; 1-pause; 2-stop; 3-fpause;
                            sbtn_FadeNow.ImageIndex = 6; // 3 - Fade Down; 6 - Fade Up
                        }
                        else
                        {
                            sbt_status_play.ImageIndex = 1; // 0-play; 1-pause; 2-stop; 3-fpause;                            
                        }
                        sbtn_Pause.ImageIndex = 0; // 0 - Play; 1 - Pause
                    break;
                case StreamStatus.PLAY: 
                        sbt_status_play.ImageIndex = 0; // 0-play; 1-pause; 2-stop; 3-fpause;
                        sbtn_Pause.ImageIndex = 1; // 0 - Play; 1 - Pause                        
                    break;
                default: 
                        sbt_status_play.ImageIndex = 2; // 0-play; 1-pause; 2-stop; 3-fpause;                        
                        sbtn_Pause.ImageIndex = 0; // 0 - Play; 1 - Pause                        
                        sbtn_FadeNow.ImageIndex = 6; // 3 - Fade Down; 6 - Fade Up
                    break;
            }                        

            // задержка отображения информации
            if (v_WaytDelay != 0) v_WaytDelay--;            
        }

        // Модификаторы воспроизведения
        private void Proces_Play_Mods()
        {
            // Автоскрол текста песен
            if (iCheck_AutoScrollText.Checked && MainStream.v_stream_status == StreamStatus.PLAY)
            {
                RTBox_Scroll_ToText(SecondsToTimeFormat(MainStream.v_play_pos + v_Scroll_Delta_Sec));
            }

            // Нонстоп лист откл (все треки)
            if (sbtn_Repeat.Tag.ToString() == "0") // Нонстоп откл
            {
               if (MainStream.v_stream_status == StreamStatus.END) // конец трека
               {
                   MainStream.v_stream_status = StreamStatus.FREE;
                   timer_playng.Enabled = false;                        
                   sbtn_Pause.ImageIndex = 0; // 0 - Play; 1 - Pause
               }
            }

            // включен нонстоп + играет трек
            if (sbtn_Repeat.Tag.ToString() == "1" && MainStream.v_stream_status == StreamStatus.PLAY)
            {
                // секунд до конца
                int sec_to_end = MainStream.v_play_max - MainStream.v_play_pos;

                // пора запускать фейдер
                // если осталось заданное значение до конца и фейдер еще не запущен
                if (MainStream.v_FadeActive == false && sec_to_end <= (v_FadeTime_Pause / 1000))
                {
                    #region данная реализация не актуальна (фейдер + стоп)
                    /* федер в конце каждого трека + стоп (но оставим на память)
                    if (sbtn_Repeat.Tag.ToString() == "0") // нет нонстопа - выключаем текущий трек фейдером
                    {
                        toolTipController.ShowHint("Фейдер без нонстопа: " + sec_to_end + " сек");
                        MainStream.p_fade_channel(0, sec_to_end); // громкость в 0
                        MainStream.v_FadeStopTime = v_FadeTime_Pause;
                        MainStream.v_FadePostAction = StreamStatus.END; // статус после фейдера
                        MainStream.v_FadeActive = true;                        
                    }                    
                    else //есть нонстоп - запуск след трека
                    */                    
                    #endregion
                    
                    // кросс фейдер между треками при нонстопе
                    #region Кросс фейдер между треками
                    toolTipController.ShowHint("Фейдер + нонстоп: " + sec_to_end + " сек");
                    
                    // копируем текущий поток
                    SlaveStream = MainStream;                    

                    // сводим в ноль текущий поток
                    SlaveStream.p_fade_channel(0, v_FadeTime_Pause); // громкость в 0
                    SlaveStream.v_FadeStopTime = v_FadeTime_Pause;
                    SlaveStream.v_FadePostAction = StreamStatus.FREE; // статус после фейдера
                    SlaveStream.v_FadeActive = true;

                    // создаем новый поток
                    MainStream.v_streem_need_free = false;
                    MainStream = new StreamClass();

                    // текущий выделенный трек
                    int Row_ID = All_PlayLists[v_play_list_id_active].dt_ListData.Rows.IndexOf(All_PlayLists[v_play_list_id_active].gv_PlayList.GetFocusedDataRow());                    
                    
                    // выделение не менялось
                    if (v_play_list_id_active == SlaveStream.v_PL_List_ID_played && Row_ID == SlaveStream.v_PL_Row_ID)
                    {
                        // следующий
                        InitPlayFile(v_play_list_id_active, 1);
                    }
                    else
                    {
                        // текущий выделенный
                        InitPlayFile(v_play_list_id_active, 0);
                    }

                    // если новый трек не запустился - запускаем следущий за ним
                    int loop_count = 1;
                    int List_Rows = All_PlayLists[MainStream.v_PL_List_ID_played].dt_ListData.Rows.Count;                                       
                    while (MainStream.v_stream == 0 && loop_count < List_Rows)
	                {
                        InitPlayFile(v_play_list_id_active, 1);
                        loop_count ++;
	                }

                    // если трек стартовал, грузим доп настройки
                    if (MainStream.v_stream != 0)
                    {
                        int pos = EQ_Main.pbc_equal_main.Position;
                        int max = EQ_Main.pbc_equal_main.Properties.Maximum;
                        float volume = (float)pos / max;
                        MainStream.v_stream_volume = 0; //установка громкости                        
                        MainStream.p_fade_channel(volume, v_FadeTime_Pause);
                    }
                    else MainStream.v_stream_status = StreamStatus.STOP;
                    #endregion
                }
            }

            // обработка активного фейдера (Hot Fader) с паузой после обработки
            if (MainStream.v_FadeActive) // активность пост обработки
            {
                if (MainStream.v_FadeStopTime > 0) // начать обработку
                {
                    MainStream.v_FadeStopTime -= timer_playng.Interval;
                    if (MainStream.v_FadeStopTime <= 0) timer_playng.Interval = 1000; // возвращаем частоту таймера
                    string tmp = MainStream.v_FadePostAction.ToString();
                }                
            }

            // обработка активного фейдера для второго потока
            if (SlaveStream.v_FadeActive && SlaveStream.v_stream != MainStream.v_stream) // активность пост обработки
            {
                if (SlaveStream.v_FadeStopTime > 0) // начать обработку
                {
                    SlaveStream.v_FadeStopTime -= timer_playng.Interval;                    
                }
            }

            // Обработка заставки
            if (MainStream.v_stream_status != StreamStatus.PLAY || MainStream.v_TextFileName == "")
            {
                if (!Pic_Logo.Visible && !panelControl_TextEditor.Visible)
                {
                    if (v_Logo_NoActions_Time >= 0)
                        v_Logo_NoActions_Time += timer_playng.Interval;

                    if (v_Logo_NoActions_Time > v_Logo_StartTime)
                    {
                        Pic_Logo.Visible = true;
                        RTBox_TextFile.Visible = false;
                        Pic_Logo.Dock = DockStyle.Fill;                    
                    }                                    
                }
            }
            else v_Logo_NoActions_Time = 0;
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

        // парсинг входящих аргументов
        private string[] Parse_arguments_PL(string[] args)
        {            
            string arg_new;
            string arg_line = "";
            char[] charSeparators = new char[] { '|' };
            
            for (int i = 0; i < args.Length; i++)
            {                
                arg_new = args[i].Trim().ToLower();
                if (System.IO.File.Exists(arg_new) && Path.GetExtension(arg_new) == ".pmp") 
                    arg_line += arg_new + charSeparators[0];
            }            

            return arg_line.Trim().Split( charSeparators, StringSplitOptions.RemoveEmptyEntries);
        }
        #endregion        

        #region Эквалайзер

        private void Init_EQ()
        {
            EQ_Main = new Control_EQ();
            EQ_Main.Dock = DockStyle.Left;

            // Контрол на форму
            panelControl_EQ.Controls.Add(EQ_Main);

            // Уровень Громкости            
            if (v_initDefaultDevice.HasValue == false)
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
                /*if (FView.IsActive)
                {                    
                    FView_Save();
                    FView.IsActive = false;
                }*/

                //this.WindowState = FormWindowState.Normal;
                //this.panelControl_All.Dock = DockStyle.None;
                //this.panelControl_All.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;                                
                //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

                this.WindowState = FormWindowState.Maximized;
                FView_Save();             
            }
            else
            {                
                //this.panelControl_All.Dock = DockStyle.Fill;
                //FView_Load();                

                this.WindowState = FormWindowState.Normal;
                FView_Save();
            }            
        }

        private void FView_Save()
        {
            //FView.FormBorderStyle = this.FormBorderStyle;            
            //FView.EQ_Visible = panelControl_EQ.Visible;

            FView.WindowState = this.WindowState;
        }

        private void FView_Load()
        {
            this.WindowState = FView.WindowState;
            //this.FormBorderStyle = FView.FormBorderStyle;           
            panelControl_EQ.Visible = FView.EQ_Visible;
            iEQ_Open_Check.Checked = FView.EQ_Visible;
            //FView.IsActive = true;
        }

        private void iAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form_About af = new Form_About(AboutInfo, AboutVersion);
            af.ShowDialog();
        }

        private void iExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // проверка изменений плейлистов в событии закрытия - FormMain_Closing
            this.Close();
        }

        private void iEQ_Open_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (panelControl_EQ.Visible)
                panelControl_EQ.Visible = false;
            else
                panelControl_EQ.Visible = true;

            iEQ_Open_Check.Checked = panelControl_EQ.Visible;
        }

        private void iPlayBack_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (panelControl_PlayBack.Visible)
                panelControl_PlayBack.Visible = false;
            else
                panelControl_PlayBack.Visible = true;

            iPlayBack_Check.Checked = panelControl_PlayBack.Visible;            
        }

        // поиск по списку
        private void iTagsEditor_ItemClick(object sender, ItemClickEventArgs e)
        {
            KeyEventArgs keyEventArgs = new KeyEventArgs(Keys.F2);            
            gv_PlayList_KeyDown(null, keyEventArgs);
        }

        #endregion

        #region Текст песен
        // Правая Панель
        private void panelControl_Right_SizeChanged(object sender, EventArgs e)
        {
            // ограничение минимальной ширины листа
            if (panelControl_Right.Width < v_PL_Widh_min_scale) panelControl_Right.Width = v_PL_Widh_min_scale;

            // Шрифт записей плейлиста
            /*float PL_Scale = (float)Math.Round((float)panelControl_Right.Width / v_PL_Widh_prim_scale, 2);
            for (int i = 0; i < xTabCtrl_PlayLists.TabPages.Count; i++)
            {
                if (All_PlayLists[i] != null)
                {
                    Font PL_Font = All_PlayLists[i].gv_PlayList.Appearance.Row.Font;
                    if (PL_Scale > 1) PL_Scale = 1;

                    PL_Font = ChangeFontSize(PL_Font, v_PL_FontSize * PL_Scale);

                    All_PlayLists[i].gv_PlayList.Appearance.Row.Font = PL_Font;
                }
            } */   
        }  

        // Изменение размера редактора
        private void RTBox_TextFile_SizeChanged(object sender, EventArgs e)
        {
            // пересчет масштаба текста
            RTBox_CorrectScale();                      
        }

        // Изменение текста в редакторе
        private void RTBox_TextFile_TextChanged(object sender, EventArgs e)
        {
            if (v_RTEdit_no_resize) return; // не масштабировать - служ инфо в тексте

            // изменение шрифта
            RTBox_CorrectFont();
            // поиск максимальной строки
            RTBox_Text_CalcMaxWidth();
            // изменение масштаба
            RTBox_CorrectScale();
            // начальная позиция
            RTBox_TextFile.Tag = -1;
            RTBox_TextFile_Scroll(0);
        }

        // Перерасчет масштаба отображения
        private void RTBox_CorrectScale()
        {
            v_Text_Width_koef = 0.96;

            if (v_Line_max_width > 0 && RTBox_TextFile.ClientSize.Width > 0)
            {
                RTBox_TextFile.ZoomFactor = (float)((double)RTBox_TextFile.ClientSize.Width / v_Line_max_width * v_Text_Width_koef); // 0.95
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
            v_Line_max_width = 0;

            foreach (string LineText in RTBox_TextFile.Lines)
            {
                int line_width = Text_CalcWidth(LineText.Replace("\t","          "), RTBox_TextFile.Font /*Default_Font*/);
                if (v_Line_max_width < line_width)
                    v_Line_max_width = line_width;                
                //Graphics.MeasureString();                
            }
        }

        // Скрол на процент
        private void RTBox_TextFile_Scroll(double pos_persent)
        {
            /*
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
             * */
        }

        private void RTBox_Scroll_ToText(string find)
        {
            string text = RTBox_TextFile.Text;
            
            if (find.StartsWith("0")) find = find.Substring(1,find.Length - 1);            

            int start = text.IndexOf(find);
            if (start < 0) start = text.IndexOf(find.Replace(':', '.'));

            if (start > 0) // разделитель - точка
            {
                RTBox_TextFile.Select(start, find.Length);
                RTBox_TextFile.SelectionStart = start;
                RTBox_TextFile.ScrollToCaret();
                //RTBox_TextFile_ScrollTo(1);

                int cur_pos = GetScrollPos(RTBox_TextFile.Handle, SB_VERT);

                // баг - если последняя строка - доп скрол на 1 позицию
                /*if (RTBox_TextFile.Lines[RTBox_TextFile.Lines.Length - 1].Contains(find) ||
                    RTBox_TextFile.Lines[RTBox_TextFile.Lines.Length - 1].Contains(find.Replace(':', '.')))
                {
                    RTBox_TextFile_ScrollTo(1);
                }*/
            }            
                        
        }

        private void RTBox_Scroll_ToText2(string find)
        {
            string text = RTBox_TextFile.Text;            

            if (find.StartsWith("0")) find = find.Substring(1, find.Length - 1);
            string find_double = find.Replace(':', '.');

            int start = text.IndexOf(find);
            if (start < 0) start = text.IndexOf(find.Replace(':', '.'));

            if (start >= 0) // текст найден
            {
                int row_scroll = -1;

                for (int i = 0; i < RTBox_TextFile.Lines.Length && row_scroll == -1; i++)
                {
                    if (RTBox_TextFile.Lines[i].Contains(find_double) || RTBox_TextFile.Lines[i].Contains(find))
                    {                        
                        row_scroll = i-1;
                    }
                }

                if (row_scroll >= 0)
                {
                    RTBox_TextFile.SelectionStart = 0;
                    RTBox_TextFile.ScrollToCaret();
                    RTBox_TextFile.Select(start, find.Length);
                    RTBox_TextFile.SelectionStart = start;

                    RTBox_TextFile_ScrollTo(row_scroll);
                }
            }

        }

        // Переход на строку
        private void RTBox_TextFile_ScrollTo(int delta)
        {
            // получаем размеры скрола
            int min;
            int max;
            GetScrollRange(RTBox_TextFile.Handle, SB_VERT, out min, out max);
            //SetScrollRange(RTBox_TextFile.Handle, SB_VERT, min, max, true);            

            // текущее положение скрола
            int cur_pos = GetScrollPos(RTBox_TextFile.Handle, SB_VERT); // / Default_Font.Height;

            if (cur_pos >= 0 && cur_pos <= max)
            {                 
                //SetScrollPos(RTBox_TextFile.Handle, SB_VERT, delta, true);
                SendMessage(RTBox_TextFile.Handle, EM_LINESCROLL, 0, delta);
            }
            
            int new_pos = GetScrollPos(RTBox_TextFile.Handle, SB_VERT);

            /*
            if (cur_pos != new_pos)
            {
                int line_num = 0;                
                try
                {
                    line_num = int.Parse(RTBox_TextFile.Tag.ToString());
                }
                catch {RTBox_TextFile.Tag = 0;}

                if (line_num < 0) line_num = 0;
                line_num += delta;

                RTBox_Caret2Row(line_num);

                RTBox_TextFile.Tag = line_num;
            }*/

            #region Старая обработка Select'om (оставил на память)
            /*
            int text_pos = 0;
            for (int i = 0; i < rownum; i++)
            {
                text_pos += RTBox_TextFile.Lines[i].Length;
            }            
            RTBox_TextFile.SelectionStart = text_pos + rownum;
            RTBox_TextFile.ScrollToCaret();  
            */
            #endregion

        }

        private void RTBox_Caret2Row(int row_number)
        {
            if (row_number < 0 || row_number > RTBox_TextFile.Lines.Length) return;

            int text_pos = 0;
            for (int i = 0; i < row_number; i++)
            {
                text_pos += RTBox_TextFile.Lines[i].Length;
            }
            RTBox_TextFile.SelectionStart = text_pos + row_number;
            RTBox_TextFile.SelectionLength = 0;
            //RTBox_TextFile.ScrollToCaret(); 
            
        }

        // установка фокуса на текстовый редактор
        private void RTBox_TextFile_MouseEnter(object sender, EventArgs e)
        {
            if (!v_RTEdit_is_focused)
            {
                v_RTEdit_is_focused = true;
                RTBox_TextFile.Focus();
            }
        }

        // потеря фокуса текстовым редактором
        private void RTBox_TextFile_LostFocus(object sender, EventArgs e)
        {
            v_RTEdit_is_focused = false;
        }

        private void RTBoxEdit_EditText()
        {
            if (MainStream.v_TextFileName != "" && System.IO.File.Exists(MainStream.v_TextFileName))
            {
                if (!panelControl_TextEditor.Visible)
                {
                    TextEditor_Show();
                }
                else
                {
                    // Text_RTEditor.RTE_Text.CanUndo
                    if (Text_RTEditor.v_need_save) // были изменения                        
                    {
                        switch (XtraMessageBox.Show("Данные изменились, сохранить?", "Вопрос", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                        {
                            case System.Windows.Forms.DialogResult.Yes: if (!Text_RTEditor.SaveData()) { return; } break;
                            case System.Windows.Forms.DialogResult.Cancel: return;
                        }
                    }

                    if (Text_RTEditor.v_need_reload)
                    {
                        RTBox_TextFile.LoadFile(MainStream.v_TextFileName);
                    }

                    TextEditor_Hide();
                }
            }
        }

        private void sbtn_TextEditor_Close_Click(object sender, EventArgs e)
        {
            RTBoxEdit_EditText();
        }

        private void TextEditor_Show()
        {
            Text_RTEditor.Parent = panelControl_TextEditor;
            Text_RTEditor.Dock = DockStyle.Fill;
            Text_RTEditor.v_FilePath = MainStream.v_TextFileName;

            panelControl_TextEditor.Visible = true;

            RTBox_TextFile.Visible = false;
            Pic_Logo.Visible = false;
            v_Logo_NoActions_Time = 0;

            panelControl_TextEditor.Dock = DockStyle.Fill;

            iEdit_Text_Data.ImageIndex = 22;

            // очищаем горячие клавиши
            iNext_Track.ItemShortcut = null;
            iPrev_Track.ItemShortcut = null;
            iNext_PlayList.ItemShortcut = null;
            iPrev_PlayList.ItemShortcut = null;
            iNext_Text.ItemShortcut = null;
            iPrev_Text.ItemShortcut = null;
            iHotFader.ItemShortcut = null;

        }

        private void TextEditor_Hide()
        {
            RTBox_TextFile.Visible = true;
            RTBox_TextFile.Dock = DockStyle.Fill;

            panelControl_TextEditor.Visible = false;
            Pic_Logo.Visible = false;
            v_Logo_NoActions_Time = 0;
            iEdit_Text_Data.ImageIndex = 21;

            // возвращаем горячие клавиши
            iNext_Track.ItemShortcut = new BarShortcut(Keys.Down);
            iPrev_Track.ItemShortcut = new BarShortcut(Keys.Up);
            iNext_PlayList.ItemShortcut = new BarShortcut(Keys.Right);
            iPrev_PlayList.ItemShortcut = new BarShortcut(Keys.Left);
            iNext_Text.ItemShortcut = new BarShortcut(Keys.PageDown);
            iPrev_Text.ItemShortcut = new BarShortcut(Keys.PageUp);
            iHotFader.ItemShortcut = new BarShortcut(Shortcut.CtrlX);
        }

        // нажатие клавиш передаем активному плей листу
        private void RTBox_TextFile_KeyPress(object sender, KeyPressEventArgs e)
        {
            All_PlayLists[v_play_list_id_active].gv_PlayList_KeyPress(sender, e);            
        }
        #endregion        

        #region Горячие клавиши

        // следующий трек (активный плейлист)
        private void iNext_Track_ItemClick(object sender, ItemClickEventArgs e)
        {
            // если включен редактор текста - отмена hotkeys
            if (!panelControl_TextEditor.Visible)
            {
                All_PlayLists[v_play_list_id_active].gv_PlayList.Focus();

                int Rows_count = All_PlayLists[v_play_list_id_active].gv_PlayList.RowCount;
                if (Rows_count == 0) return; // нет записей

                int Row_focus = All_PlayLists[v_play_list_id_active].gv_PlayList.FocusedRowHandle;

                Row_focus++; // сдвиг трека, след/предыдущ/текущий

                if (Row_focus > Rows_count - 1) Row_focus = 0;
                if (Row_focus < 0) Row_focus = Rows_count - 1;

                All_PlayLists[v_play_list_id_active].gv_PlayList.FocusedRowHandle = Row_focus;
            }
        }

        // предыдущий трек (активный плейлист)
        private void iPrev_Track_ItemClick(object sender, ItemClickEventArgs e)
        {
            // если включен редактор текста - отмена hotkeys
            if (!panelControl_TextEditor.Visible)
            {
                All_PlayLists[v_play_list_id_active].gv_PlayList.Focus();

                int Rows_count = All_PlayLists[v_play_list_id_active].gv_PlayList.RowCount;
                if (Rows_count == 0) return; // нет записей

                int Row_focus = All_PlayLists[v_play_list_id_active].gv_PlayList.FocusedRowHandle;

                Row_focus--; // сдвиг трека, след/предыдущ/текущий

                if (Row_focus > Rows_count - 1) Row_focus = 0;
                if (Row_focus < 0) Row_focus = Rows_count - 1;

                All_PlayLists[v_play_list_id_active].gv_PlayList.FocusedRowHandle = Row_focus;
            }
        }

        // Следующий плейлист
        private void iNext_PlayList_ItemClick(object sender, ItemClickEventArgs e)
        {
             // если включен редактор текста - отмена hotkeys
            if (!panelControl_TextEditor.Visible)
            {
                int TabCount = xTabCtrl_PlayLists.TabPages.Count;
                int TabSelected = xTabCtrl_PlayLists.SelectedTabPageIndex;

                TabSelected++;

                if (TabSelected > TabCount - 1) TabSelected = 0;
                if (TabSelected < 0) TabSelected = TabCount - 1;

                xTabCtrl_PlayLists.SelectedTabPageIndex = TabSelected;
            }
        }

        // Предыдущий плейлист
        private void iPrev_PlayList_ItemClick(object sender, ItemClickEventArgs e)
        {
             // если включен редактор текста - отмена hotkeys
            if (!panelControl_TextEditor.Visible)
            {
                int TabCount = xTabCtrl_PlayLists.TabPages.Count;
                int TabSelected = xTabCtrl_PlayLists.SelectedTabPageIndex;

                TabSelected--;

                if (TabSelected > TabCount - 1) TabSelected = 0;
                if (TabSelected < 0) TabSelected = TabCount - 1;

                xTabCtrl_PlayLists.SelectedTabPageIndex = TabSelected;
            }
        }

        // Прокрута текста (вниз)
        private void iNext_Text_ItemClick(object sender, ItemClickEventArgs e)
        {
             // если включен редактор текста - отмена hotkeys
            if (!panelControl_TextEditor.Visible)
            {
                RTBox_TextFile_ScrollTo(v_Scroll_Lines_Delta);
            }
        }

        // Прокрута текста (вверх)
        private void iPrev_Text_ItemClick(object sender, ItemClickEventArgs e)
        {
             // если включен редактор текста - отмена hotkeys
            if (!panelControl_TextEditor.Visible)
            {
                RTBox_TextFile_ScrollTo(-v_Scroll_Lines_Delta);
            }
        }

        #endregion
                
        #region Всплывающее меню

        private void iPlay_ItemClick(object sender, ItemClickEventArgs e)
        {
            btn_PausePlay_Click(null, null);
        }

        private void iPause_ItemClick(object sender, ItemClickEventArgs e)
        {
            btn_PausePlay_Click(null, null);
        }

        private void iStop_ItemClick(object sender, ItemClickEventArgs e)
        {
            buttonStop_Click(null, null);
        }

        private void iRepeat_Click(object sender, ItemClickEventArgs e)
        {
            sbtn_Repeat_Click(null, null);
            iCheck_ItemClick(sender, e);
        }

        // очистка плейлиста
        private void iClear_PlayList_ItemClick(object sender, ItemClickEventArgs e)
        {
            // если включен редактор текста - отмена hotkeys
            if (panelControl_TextEditor.Visible) return;

            if (XtraMessageBox.Show("Очистить текущий плейлист?",
                                    "Подтверждение",
                                    MessageBoxButtons.OKCancel,
                                    MessageBoxIcon.Question)
                          == System.Windows.Forms.DialogResult.OK)
            {
                All_PlayLists[v_play_list_id_active].PL_ClearList();                
            }
        }

        private void iClose_PlayList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (PL_PlayLists_Save(v_play_list_id_active))
                All_PlayLists[v_play_list_id_active].PL_CloseList(); // очистка PL;
        }

        // Закрытие списка плейлиста
        public bool PL_PlayLists_Save(int play_list_id)
        {
            Control_PlayList Cur_PL = All_PlayLists[play_list_id];

            if (!Cur_PL.PL_Save_Changes()) return false;

            iSave_PlayList.Enabled = false;
            return true;

            #region Старый код
		/*if (Cur_PL.is_Changed)
            {
                // название списка
                string playlist_name = "[" + play_list_id + "] " + 
                                        (Cur_PL.pl_FilePath != "" ?
                                        "\"" + Path.GetFileNameWithoutExtension(Cur_PL.pl_FilePath) + "\"" : 
                                        "\"Новый плейлист\"");                

                FChangeLog = new Form_History();
                FChangeLog.v_list_name = playlist_name;
                FChangeLog.v_list_change_log = Cur_PL.Сhange_history.info();
                FChangeLog.v_init_dir = openFileDialog.InitialDirectory;

                FChangeLog.v_resault = 0;

                FChangeLog.ShowDialog();

                // Если нет файла плейлиста, то сохраняем как (3)
                if (FChangeLog.v_resault == 2 && Cur_PL.pl_FilePath == "") FChangeLog.v_resault = 3;

                switch (FChangeLog.v_resault)
                {
                    case 3: // сохранить как
                        if (Cur_PL.PL_SaveDataAs(FChangeLog.v_file_name)) // если сохр успешно
                            Cur_PL.pl_FilePath = FChangeLog.v_file_name;                            
                        else 
                            return false;
                        break;
                    case 2: // сохранить
                        if (!Cur_PL.PL_SaveData()) return false;// сохранить файл
                        break;
                    case 1: // без сохранения
                        break;
                    default: return false; // тут cancel - отмена  
                }

                if (FChangeLog.v_resault != 0)
                {                    
                    iSave_PlayList.Enabled = false;
                }
            }

            return true;*/ 
	#endregion
        }

        private void iSave_PlayList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!All_PlayLists[v_play_list_id_active].PL_SaveData())
            {
                return;// сохранить файл                
            }

            Check_PL_Status(v_play_list_id_active);

            //XtraMessageBox.Show("Данные успешно сохранены!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // сохранить как...
        private void iSave_PlayList_AS_ItemClick(object sender, ItemClickEventArgs e)
        {
            saveFileDialog.InitialDirectory = openFileDialog.InitialDirectory;
            saveFileDialog.FileName = Path.GetFileName(All_PlayLists[v_play_list_id_active].pl_FilePath);
            saveFileDialog.Filter = "PlayList Parnas Machine (*.pmp)|*.pmp|All files (*.*)|*.*";
            if (saveFileDialog.FileName == "") saveFileDialog.FileName = "Новый плейлист";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (All_PlayLists[v_play_list_id_active].PL_SaveDataAs(saveFileDialog.FileName))
                {
                    All_PlayLists[v_play_list_id_active].pl_FilePath = saveFileDialog.FileName;                    
                }
            }
        }

        private void iEdit_DelMuzFile_ItemClick(object sender, ItemClickEventArgs e)
        {            
            int Row_ID = All_PlayLists[v_play_list_id_active].dt_ListData.Rows.IndexOf(
                         All_PlayLists[v_play_list_id_active].gv_PlayList.GetFocusedDataRow()
                         );

            // выделяем след элемент с учетом сортировки (иначе после удаления текущ записи список прыгнет)
            int max_rows = All_PlayLists[v_play_list_id_active].gv_PlayList.RowCount;
            int cur_row = All_PlayLists[v_play_list_id_active].gv_PlayList.FocusedRowHandle;

            if (cur_row == max_rows - 1) // последняя запись
                cur_row--; // на строчку назад
            else
                cur_row++; // на строку ниже                

            All_PlayLists[v_play_list_id_active].gv_PlayList.FocusedRowHandle = cur_row; 

            All_PlayLists[v_play_list_id_active].PL_DelTrack(Row_ID);            

            Check_PL_Status(v_play_list_id_active);
        }

        private void iEdit_DelTextFile_ItemClick(object sender, ItemClickEventArgs e)
        {            
            int Row_ID = All_PlayLists[v_play_list_id_active].dt_ListData.Rows.IndexOf(All_PlayLists[v_play_list_id_active].gv_PlayList.GetFocusedDataRow());

            All_PlayLists[v_play_list_id_active].PL_DelText(Row_ID);
            Check_PL_Status(v_play_list_id_active);
        }

        private void iEdit_AddTextFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            int Row_ID = All_PlayLists[v_play_list_id_active].dt_ListData.Rows.IndexOf(All_PlayLists[v_play_list_id_active].gv_PlayList.GetFocusedDataRow());
            if (Row_ID < 0) return;

            openFileDialog.Title = "Выбор файла - Текст песни";
            openFileDialog.Filter = "Текстовые файлы (*.rtf;*.doc;*.txt)|*.rtf;*.doc;*.txt|Все файлы (*.*)|*.*";
            openFileDialog.InitialDirectory = Settings.p_DefFolder_Texts;
            openFileDialog.FileName = "";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            if (openFileDialog.FileName.Length != 0)
            {
                All_PlayLists[v_play_list_id_active].PL_AddText(Row_ID, openFileDialog.FileName);              
                Check_PL_Status(v_play_list_id_active);
                Settings.p_DefFolder_Texts = System.IO.Path.GetDirectoryName(openFileDialog.FileName);

                // Загрузка текста decimal редакто - если изменен текущий открытый муз файл
                if (MainStream.v_PL_List_ID_played == v_play_list_id_active &&
                    MainStream.v_PL_Row_ID == Row_ID)
                {                    
                    MainStream.v_TextFileName = openFileDialog.FileName;
                    RTBox_TextFile.LoadFile(MainStream.v_TextFileName);                     
                }
            }

        }

        private void iEdit_AddMuzFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            openFileDialog.Title = "Выбор музыкального файла";
            openFileDialog.Filter = "Музыкальные файлы (*.mp3;*.wav)|*.mp3;*.wav|Все файлы (*.*)|*.*";
            openFileDialog.InitialDirectory = Settings.p_DefFolder_Music;
            openFileDialog.FileName = "";

            DialogResult dr = this.openFileDialog.ShowDialog();
            
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                int add_rows_count = 0;
                int max_rows = All_PlayLists[v_play_list_id_active].gv_PlayList.RowCount;

                foreach (String file in openFileDialog.FileNames)
                {
                    All_PlayLists[v_play_list_id_active].PL_AddMuz(file);                    
                    Settings.p_DefFolder_Music = System.IO.Path.GetDirectoryName(file);
                    add_rows_count++;
                }

                Check_PL_Status(v_play_list_id_active);

                int Row_HID = All_PlayLists[v_play_list_id_active].gv_PlayList.GetRowHandle(max_rows + add_rows_count - 1);                    
                if (add_rows_count != 0) All_PlayLists[v_play_list_id_active].gv_PlayList.FocusedRowHandle = Row_HID;
            }            
            
        }

        private void iEdit_AddMuzFolder_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            All_PlayLists[v_play_list_id_active].PL_AddMuz_Folder(folderBrowserDialog.SelectedPath);            

            Check_PL_Status(v_play_list_id_active);
        }

        // Обновление элементов списка
        private void Check_PL_Status(int pl_check_id)
        {
            if (All_PlayLists[v_play_list_id_active].pl_FilePath != "")
            {
                //iSave_PlayList.Enabled = All_PlayLists[v_play_list_id].is_Changed;
                iSave_PlayList.Enabled = true; // всегда доступен
            }
            else iSave_PlayList.Enabled = true; // всегда доступен
        }

        private void iCheck_ItemClick(object sender, ItemClickEventArgs e)
        {
            string status = "ОТКЛ";
            if (((BarCheckItem)e.Item).Checked) status = "ВКЛ";
            toolTipController.ShowHint(status + " " + ((BarCheckItem)e.Item).Tag.ToString());
        }

        private void iHotFader_ItemClick(object sender, ItemClickEventArgs e)
        {
            sbtn_FadeNow_Click(null, null);
        }

        private void iPause_Logo_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Pic_Logo.Visible)
            {
                Pic_Logo.Visible = false;
                RTBox_TextFile.Visible = true;
                v_Logo_NoActions_Time = -1;
            }
            else
            {
                Pic_Logo.Visible = true;
                RTBox_TextFile.Visible = false;
                Pic_Logo.Dock = DockStyle.Fill;
            }
        }

        private void iProgSettings_ItemClick(object sender, ItemClickEventArgs e)
        {
            FSettings.InitSettings(Settings);
            FSettings.ShowDialog();
            if (FSettings.v_settings_updated) Load_Prog_User_Settings();
        }

        private void iEdit_Text_Data_ItemClick(object sender, ItemClickEventArgs e)
        {
            RTBoxEdit_EditText();
        }
        #endregion         

        #region Управляющие процедуры
        // Убираем мышку с объекта
        private void obj_MouseLeave(object sender, EventArgs e)
        {
            toolTipController.HideHint();
            xTabCtrl_PlayLists.Focus();
        }

        // Наведение мышки на элемент
        private void obj_MouseEnter_Focus(object sender, EventArgs e)
        {
            if (sender.GetType().Name == "ProgressBarControl")
            {
                ((ProgressBarControl)sender).Focus();
            }

            if (sender.GetType().Name == "TrackBarControl")
            {
                ((TrackBarControl)sender).Focus();
            }

            if (sender.GetType().Name == "TextEdit")
            {
                ((TextEdit)sender).Focus();
                ((TextEdit)sender).DeselectAll();                
            }
            
        }
        #endregion

        #region Аналайзер
        private void Analizer_Calc()
        {
            //int[] fft = new int[512]; // fft data buffer
            //Bass.BASS_ChannelGetData(MainStream.v_stream, fft, 512); //Bass.BASS_DA BASS_DATA_FFT1024

            if (MainStream.v_stream_status == StreamStatus.PLAY)
            {
                float[] level = new float[2];
                Bass.BASS_ChannelGetLevel(MainStream.v_stream, level);

                pbc_analizer_L.EditValue = level[0] * pbc_analizer_L.Properties.Maximum;
                pbc_analizer_R.EditValue = level[1] * pbc_analizer_R.Properties.Maximum;

                if (level[0] > level_max[0]) level_max[0] = level[0];
                if (level[1] > level_max[1]) level_max[1] = level[1];
                if (level[0] < level_min[0]) level_min[0] = level[0];
                if (level[1] < level_min[1]) level_min[1] = level[1];
            }
            else
            {
                pbc_analizer_L.EditValue = 0;
                pbc_analizer_R.EditValue = 0;
            }

            #region спектро аналайзер
            /*
            //procedure draw(....; fft: array [0..511] of extended);
            Height:= H-1; 
            X:= 0; 
            Y:= 0;

            //if(FrameClear) then with PaintBox1.Canvas do begin
            pictureBox1. Pen.Color  := BackColor; 
            Brush.Color:= BackColor; 
            Rectangle(0, 0, W, H)  
            */

            //Graphics g = new Graphics();


            // Draw a line in the PictureBox.
            //g.DrawLine(System.Drawing.Pens.Red, pictureBox1.Left, pictureBox1.Top,
            //pictureBox1.Right, pictureBox1.Bottom);            

            /*
            pbc_analizer.EditValue = Convert.ToInt32(fft[10] / 100000000000 / 100000000000 / 100000000000 * (-1));

            int Ypos, PeakFallOff = 2, Res = 2, LineFallOff = 4, Width = 3, Height=100;
            short X, Y;
            float Yval;
            int[] FFTPeacks = new int[512];
            int[] FFTFallOff = new int[512];

            for (int i = 0; i <= 128; i++)
            {                
                Yval = fft[i * 2 + 5];
                if (Yval < 0.0) Yval = -Yval;
                if (Yval.ToString() == "NaN") Yval = 0;

                Ypos = Convert.ToInt32(Yval);
                if (Ypos > Height) Ypos = Height;

                if (Ypos >= FFTPeacks[i]) 
                    FFTPeacks[i] = Ypos;
                else
                    FFTPeacks[i] -= PeakFallOff;

                if (Ypos >= FFTFallOff[i])
                    FFTFallOff[i] = Ypos;
                else
                    FFTFallOff[i] -= LineFallOff;

                //Rectangle(X + i * (Width + 1), Y + H - FFTFallOff[i], X + i * (Width + 1) + Width, Y + H);
            } */

            #endregion
        }

        private void timer_peaklevel_Tick(object sender, EventArgs e)
        {
            // Аналайзер
            Analizer_Calc();
        }
        #endregion       

        #region Кнопки\меню

        private void iToggle_PL_Show_ItemClick(object sender, ItemClickEventArgs e)
        {
            Settings.p_HotList_Position = Settings.p_HotList_Position ? false : true;
            Settings.Save();

            if (Settings.p_HotList_Position)
            {
                panelControl_Hot_PL.Controls.Add(All_PlayLists[0]);
                xTabCtrl_PlayLists.TabPages[0].PageVisible = false;
                panelControl_Hot_PL.Visible = true;
            }
            else
            {
                xTabCtrl_PlayLists.TabPages[0].Controls.Add(All_PlayLists[0]);
                xTabCtrl_PlayLists.TabPages[0].PageVisible = true;
                panelControl_Hot_PL.Visible = false;
            }
        }

        private void CheckUpdates(bool show_err_msg)
        {
            FUpdate = new Form_Update();
            FUpdate.Check_NewVersion(show_err_msg); // показывать сообщения об ошибках
            /*if (FUpdate.NeedUpdate)
            {
                FUpdate.Start_Update(); // перенесено в событие закрытия программы                
                //Application.Exit();
            }*/
        }

        private void iCheckUpdates_ItemClick(object sender, ItemClickEventArgs e)
        {
            // инициализация потока обновлений
            Thread FU_Thread = new Thread(new ThreadStart(FU_StartUpdate));
            FU_Thread.Start();
        }

        private void iFindFilter_ItemClick(object sender, ItemClickEventArgs e)
        {
            All_PlayLists[v_play_list_id_active].checkButton_Filter_Plus.Checked =
            All_PlayLists[v_play_list_id_active].checkButton_Filter_Plus.Checked ? false : true;

        }

        private void iPList_Width_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            if (iPList_Width.Checked)
                panelControl_Right.Width = panelControl_Right.Width * 2;
            else
                panelControl_Right.Width = panelControl_Right.Width / 2;
        }

        private void Global_KeyPress(object sender, KeyPressEventArgs e)
        {
            All_PlayLists[v_play_list_id_active].gv_PlayList_KeyPress(sender, e);
        }

        private void iCheck_Tags_Files_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            for (int i = 0; i < xTabCtrl_PlayLists.TabPages.Count; i++)
            {
                All_PlayLists[i].v_Check_Exist = true; // нет в меню, всегда вкл
                All_PlayLists[i].v_Check_Tags = iCheck_Tags_Files.Checked;
                if (iCheck_Tags_Files.Checked) All_PlayLists[i].Check_Exists_Data();
            }


        }

        // Домашняя страница
        private void iHomePage_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://pp.zigzag-muz.ru/");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        // Поддержка проекта
        private void iSupportPage_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.teamer.ru/projects/100977/");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        // Показать текстовый фал в проводнике
        private void iShowExplorer_TextFile_ItemClick(object sender, ItemClickEventArgs e)
        {
            int Row_ID = All_PlayLists[v_play_list_id_active].dt_ListData.Rows.IndexOf(
             All_PlayLists[v_play_list_id_active].gv_PlayList.GetFocusedDataRow()
             );

            string FilePath = All_PlayLists[v_play_list_id_active].dt_ListData.Rows[Row_ID][1].ToString();

            if (FilePath != "") ShowInExplorer(FilePath);
            else DevExpress.XtraEditors.XtraMessageBox.Show("Текстовый файл не выбран!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Показать Муз фал в проводнике Windows
        private void iShowExplorer_Mp3File_ItemClick(object sender, ItemClickEventArgs e)
        {
            int Row_ID = All_PlayLists[v_play_list_id_active].dt_ListData.Rows.IndexOf(
             All_PlayLists[v_play_list_id_active].gv_PlayList.GetFocusedDataRow()
             );

            string FilePath = All_PlayLists[v_play_list_id_active].dt_ListData.Rows[Row_ID][0].ToString();

            ShowInExplorer(FilePath);
        }

        private void ShowInExplorer(string FilePath)
        {
            if (!System.IO.File.Exists(FilePath))
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Файла нет на диске!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                //создание параметров
                var startInfo = new ProcessStartInfo
                {
                    //имя файла
                    FileName = "explorer.exe",
                    //скрытое окно
                    //WindowStyle = ProcessWindowStyle.Hidden,
                    //ваши аргументы
                    Arguments = "/select," + FilePath /*<path-to-file>*/
                };

                //запуск процесса переноса загруженных файлов
                Process.Start(startInfo);
                //ParentForm.Close();                    
                //Application.Exit();                
            }
            catch (Exception e)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(e.Message, "Ошибка запуска проводника", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }                    
        #endregion

        private void xTabCtrl_PlayLists_CustomHeaderButtonClick(object sender, DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventArgs e)
        {
            int PagesCount = xTabCtrl_PlayLists.TabPages.Count;

            if (e.Button.Index == 0) // добавить вкладку
            {
                if (All_PlayLists.Length == PagesCount)
                {
                    XtraMessageBox.Show("Создано максимальное количество ПЛистов!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                xTabCtrl_PlayLists.TabPages.Add("["+ (PagesCount) + "]");
                Init_New_PlayList(PagesCount);
                xTabCtrl_PlayLists.SelectedTabPageIndex = PagesCount;
                Update_Visual_List_Color();
            }

            if (e.Button.Index == 1) // закрыть вкладку
            {
                int remove_pl = v_play_list_id_active;

                if (remove_pl == 0) // Hot List - только очистка
                {
                    if (XtraMessageBox.Show("Очистить Горячий список?",
                        "Подтверждение",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question)
                    == System.Windows.Forms.DialogResult.OK)
                    {
                        All_PlayLists[0].PL_CloseList();
                    }
                    return;
                }
                else // другие плей листы
                {

                    if (All_PlayLists[remove_pl].pl_FilePath != "" &&
                        XtraMessageBox.Show("Закрыть " + xTabCtrl_PlayLists.TabPages[remove_pl].Text + " " + All_PlayLists[remove_pl].labelControl_header.Text + " ?", 
                            "Подтверждение", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) 
                            != System.Windows.Forms.DialogResult.OK
                        )
                    {
                        return; // отмена
                    }

                    // попытка сохранения изменений ПЛ
                    if (!PL_PlayLists_Save(remove_pl)) 
                    {
                        FUpdate.NeedUpdate = false;
                        return;
                    }

                    // закрываем ПЛ
                    xTabCtrl_PlayLists.TabPages.RemoveAt(remove_pl);

                    for (int i = remove_pl; i < xTabCtrl_PlayLists.TabPages.Count; i++)
                    {
                        xTabCtrl_PlayLists.TabPages[i].Text = "[" + (i) + "]";
                        All_PlayLists[i] = All_PlayLists[i + 1]; // сдвиг массивов

                        // перелинковка переменных
                        All_PlayLists[i].v_PList_ID = i;
                        All_PlayLists[i].grid_PlayList.Tag = i;
                        if (MainStream.v_PL_List_ID_played == i + 1) MainStream.v_PL_List_ID_played = i;
                    }

                    All_PlayLists[xTabCtrl_PlayLists.TabPages.Count] = null; // очистка последнего

                    xTabCtrl_PlayLists.SelectedTabPageIndex = remove_pl;
                }

            }
    
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