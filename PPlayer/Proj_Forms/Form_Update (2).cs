﻿using System;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace PPlayer
{
    public partial class Form_Update : DevExpress.XtraEditors.XtraForm
    {
        //public string xmlURL = "file://192.168.0.2/PPlayer_Setup/PPlayer.application";
        public string xmlURL; // = "http://zigzag-muz.ru/pplayer/PPlayer.application";
        public bool NeedUpdate = false;
        public UpdateVersion uv = new UpdateVersion();
        private DataTable dt_File_List = Get_FileTable();
        public bool detect_new_version = false;
        private string UpdateFolder = "Updates";
        private bool UpdateError = false;

        //класс для проерки версии
        public class UpdateVersion
        {
            public Version v_cur_version = new Version("0.0.0.0"); // установленная версия
            public Version v_new_version = new Version("0.0.0.0"); //последняя версия программы
            public string  v_program_name; // название программы
            public string  v_folder_program; // папка программы
            public string  v_folder_update; // папка обновлений
            public string  v_whatnew; //что нового в программе
            public string  v_url_xml_main; //путь к программе		
            public string  v_url_xml_new; //путь к программе
            public string  v_url_folder; //папка с файлами обновления            
            public int v_size = 0; //папка с файлами обновления  
            public string v_size_text = "";
        }

        static DataTable Get_FileTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Size", typeof(int));
            table.Columns.Add("SizeLocal", typeof(int));
            table.Columns.Add("SizeLoaded", typeof(int));
            table.Columns.Add("NeedLoad", typeof(bool));
            table.Columns.Add("SizeLoadNow", typeof(int));

            // Add DataRows.
            //table.Rows.Add(25, "Indocin", "David", DateTime.Now);
            
            return table;
        }

        public Form_Update(string Url_Update)
        {
            xmlURL = Url_Update;
            InitializeComponent();
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            string Program_folder = System.IO.Directory.GetCurrentDirectory(); // текущая папка
            string Update_folder = Program_folder + "\\Updates"; // папка для загрузки обновлений

            uv.v_folder_program = Program_folder;
            uv.v_folder_update = Update_folder;            

            Check_NewVersion();            
        }

        // отмена
        private void btn_cancel_Click(object sender, EventArgs e)
        {
            NeedUpdate = false;
            this.Close();            
        }

        // проверка наличия новой версии
        private void Check_NewVersion()
        {
            this.Height = 150;

            #region Проверка новой версии
            //потоки для чтения
            System.IO.Stream st = null;
            System.IO.StreamReader sr = null;
            //логин и пароль, если требуется установка авторизации на прокси сервере
            string username = null, password = null;

            XmlTextReader reader = null;
            try
            {                
                string elementName = "";

                uv.v_url_xml_main = xmlURL;
                //подготавливаем запрос
                System.Net.WebRequest req = System.Net.WebRequest.Create(xmlURL);
                //т.к. в данном примере логин и пароль пустые, ничего не заносим
                if (username != null && password != null)
                    req.Credentials = new System.Net.NetworkCredential(username, password);

                //пытаемся получить файл
                System.Net.WebResponse resp = req.GetResponse();

                //подключаемся к потоку
                st = resp.GetResponseStream();
                //читаем поток.. не забываем про кодировку
                sr = new System.IO.StreamReader(st, Encoding.Default);

                reader = new XmlTextReader(sr);
                reader.MoveToContent();

                //читаем xml
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        elementName = reader.Name;

                        // читаем свойства элементов
                        switch (elementName)
                        {
                            case "assemblyIdentity": // version                                                               
                                uv.v_new_version = new Version(reader.GetAttribute("version"));
                                break;
                            case "description": // version                               
                                uv.v_program_name = reader.GetAttribute("asmv2:product");
                                break;
                            case "dependentAssembly": // xml новой версии
                                uv.v_url_xml_new = reader.GetAttribute("codebase");
                                uv.v_url_xml_new = xmlURL.Substring(0, xmlURL.LastIndexOf('/') + 1) + uv.v_url_xml_new.Replace('\\', '/');
                                uv.v_url_folder = uv.v_url_xml_new.Substring(0, uv.v_url_xml_new.LastIndexOf('/') + 1);
                                break;
                        }
                    }
                    else
                    {
                        // читаем значения элементов...  
                        if ((reader.NodeType == XmlNodeType.Text) &&
                            (reader.HasValue))
                        {
                            // we check what the name of the node was  
                            /*switch (elementName)
                            {
                                case "url":
                                    url = reader.Value;
                                    break;
                            }*/
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Ошибка чтения последней версии.\n" + e.Message, "Ошибка загрузки обновления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateError = true;
                this.Close();
                return;
            }
            finally
            {
                //закрываем все потоки
                if (sr != null)
                    sr.Close();

                if (st != null)
                    st.Close();

                if (reader != null)
                    reader.Close();
            }
            #endregion            

            // текущая версия
            uv.v_cur_version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = "Обновление программы (" + uv.v_cur_version + ")";

            #region Спрашиваем пользователя
            //спрашиваем у пользователя что делать дальше
            if (uv.v_cur_version.CompareTo(uv.v_new_version) < 0)
            {
                // анализ списка файлов
                Calc_NewVersion();

                detect_new_version = true;
                label_info.Text = "Вышла новая версия \"" + uv.v_program_name + "\" (" + uv.v_new_version + ")";
                if (uv.v_size_text != "") label_info.Text += " - " + uv.v_size_text;

                sbtn_update.Visible = true;
                //sbtn_cancel.Left = 367;

                if (uv.v_whatnew != null)
                {
                    this.Height = 330;
                    memoEdit_info.Text = "Что нового:\n" + uv.v_whatnew;
                }
                else
                {
                    this.Height = 150;
                    memoEdit_info.Visible = false;
                }

                //пытаемся перейти по ссылке открыв браузер
                //Process.Start(uv.p_url_xml_filelist);
            }
            else
            {
                //DevExpress.XtraEditors.XtraMessageBox.Show("Установлена последняя версия.", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                NeedUpdate = false;
                this.Close();
                return;
            }
            #endregion
        }

        // расчет размера необходимых файлов
        private void Calc_NewVersion()
        {
            #region Парсинг XML новой версии
            //потоки для чтения
            System.IO.Stream st = null;
            System.IO.StreamReader sr = null;
            string username = null, password = null; //логин и пароль, если требуется установка авторизации на прокси сервере
            
            XmlTextReader reader = null;
            string start_exe = "";

            try
            {
                string xmlURL = uv.v_url_xml_new;
                string elementName = "";

                uv.v_url_xml_main = xmlURL;
                //подготавливаем запрос
                System.Net.WebRequest req = System.Net.WebRequest.Create(xmlURL);
                if (username != null && password != null) req.Credentials = new System.Net.NetworkCredential(username, password);

                //пытаемся получить файл
                System.Net.WebResponse resp = req.GetResponse();

                //подключаемся к потоку
                st = resp.GetResponseStream();
                //читаем поток.. не забываем про кодировку
                sr = new System.IO.StreamReader(st, Encoding.Default);

                reader = new XmlTextReader(sr);
                reader.MoveToContent();

                //читаем xml
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        elementName = reader.Name;

                        // читаем свойства элементов
                        switch (elementName)
                        {
                            case "dependentAssembly": // блок описания файла
                                string need_install = reader.GetAttribute("dependencyType");
                                if (need_install == "install")
                                {
                                    string file_name = reader.GetAttribute("codebase");
                                    int file_bsize = Convert.ToInt32(reader.GetAttribute("size"));

                                    dt_File_List.Rows.Add(file_name, file_bsize);
                                    //uv.v_size += file_bsize;
                                }
                                break;
                            case "file": // блок описания файла                                
                                string file_name2 = reader.GetAttribute("name");
                                int file_bsize2 = Convert.ToInt32(reader.GetAttribute("size"));
                                dt_File_List.Rows.Add(file_name2, file_bsize2);                                                                    
                                break;
                            case "commandLine":
                                start_exe = reader.GetAttribute("file");
                                break;
                        }
                    }
                    else
                    {
                        // читаем значения элементов...  
                        if ((reader.NodeType == XmlNodeType.Text) &&
                            (reader.HasValue))
                        {
                            // we check what the name of the node was  
                            /*switch (elementName)
                            {
                                case "url":
                                    url = reader.Value;
                                    break;
                            }*/
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Ошибка загрузки списка файлов.\n" + e.Message, "Ошибка загрузки обновления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                NeedUpdate = false;
                this.Close();
                return;
            }
            finally
            {
                //закрываем все потоки
                if (sr != null)
                    sr.Close();

                if (st != null)
                    st.Close();

                if (reader != null)
                    reader.Close();
            }
            #endregion

            #region Расчет размера загрузки
            //Проверка локальных файлов
            uv.v_size = 0;

            for (int i = 0; i < dt_File_List.Rows.Count; i++)
            {
                string path = uv.v_folder_program + @"\" + dt_File_List.Rows[i]["Name"];
                string path_upload = uv.v_folder_program + @"\" + UpdateFolder + @"\" + dt_File_List.Rows[i]["Name"];

                dt_File_List.Rows[i]["NeedLoad"] = true;

                FileInfo fi = new FileInfo(path);
                if (fi.Exists)
                    dt_File_List.Rows[i]["SizeLocal"] = fi.Length;

                // сравнение размера нового файла с имеющимся
                if (dt_File_List.Rows[i]["Size"].ToString() != dt_File_List.Rows[i]["SizeLocal"].ToString())
                {
                    FileInfo fu = new FileInfo(path_upload);
                    if (fu.Exists)
                        dt_File_List.Rows[i]["SizeLoaded"] = fu.Length;
                    else
                        dt_File_List.Rows[i]["SizeLoaded"] = 0;

                    // сравнение нового файла с загруженным в папку Updates
                    if (dt_File_List.Rows[i]["Size"].ToString() == dt_File_List.Rows[i]["SizeLoaded"].ToString())
                    {
                        dt_File_List.Rows[i]["NeedLoad"] = false;
                    }
                }
                else
                {
                    dt_File_List.Rows[i]["NeedLoad"] = false;
                }

                // исполняемый файл всегда обновляем
                if (dt_File_List.Rows[i]["Name"].ToString() == start_exe)
                {
                    dt_File_List.Rows[i]["NeedLoad"] = true;
                }                    

                if (Convert.ToBoolean(dt_File_List.Rows[i]["NeedLoad"])) uv.v_size += Convert.ToInt32(dt_File_List.Rows[i]["Size"]);
            }

            uv.v_size_text = size_to_text(uv.v_size);
            
            #endregion
        }

        // загрузка файлов с сервера
        private void Upload_NewVersion()
        {
            // прогрес бар
            pbc_upload.Properties.Maximum = 10000;//uv.v_size;
            pbc_upload.Position = 0;            

            for (int i = 0; i < dt_File_List.Rows.Count; i++)
            {
                // нужна загрузка файла
                if (Convert.ToBoolean(dt_File_List.Rows[i]["NeedLoad"]))
                {
                    string path_from = uv.v_url_folder + dt_File_List.Rows[i]["Name"].ToString().Replace(@"\","/");
                    string path_to = uv.v_folder_program + @"\" + UpdateFolder + @"\" + dt_File_List.Rows[i]["Name"];
                    string cur_filename = dt_File_List.Rows[i]["Name"].ToString();

                    string dir = System.IO.Path.GetDirectoryName(path_to);
                    if (!System.IO.Directory.Exists(dir))
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }   

                    try
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                        webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                        
                        int cur_filesize = Convert.ToInt32(dt_File_List.Rows[i]["Size"]);
                        
                        label_info.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        label_info.Text = "Загрузка обновления: 0 %"; //pbc_upload.Position

                        webClient.Headers.Add("FileName", cur_filename);
                        webClient.DownloadFileAsync(new Uri(path_from), path_to);

                        dt_File_List.Rows[i]["SizeLoadNow"] = 0;
                    }
                    catch (Exception e)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Ошибка загрузки файла: " + cur_filename + "\n" + e.Message, "Ошибка загрузки обновления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        NeedUpdate = false;
                        this.Close();
                        return;
                    }
                    finally
                    {
                    }                    
                }
            }
        }

        // состояние загрузки
        private void Check_Loading_Status()
        {
            double cur_load_all = 0;
            double need_load = 0;

            foreach (DataRow Row in dt_File_List.Rows)
            {
                if (Convert.ToBoolean(Row["NeedLoad"]))
                {
                    cur_load_all += Convert.ToInt32(Row["SizeLoadNow"].ToString());
                    need_load += Convert.ToInt32(Row["Size"].ToString());

                    // файл Updater загружен
                    if (Row["Name"].ToString() == "Updater.exe" && Row["Size"].ToString() == Row["SizeLoadNow"].ToString())
                    {
                        FileInfo old_file = new FileInfo(uv.v_folder_program + "\\" + Row["Name"].ToString());
                        FileInfo new_file = new FileInfo(uv.v_folder_update + "\\" + Row["Name"].ToString());

                        // удаляем старый файл
                        if (old_file.Exists)
                        try
                        {
                            old_file.Delete();
                        }
                        catch (Exception) {}

                        // копируем новый файл
                        if (!old_file.Exists)
                        try
                        {
                            new_file.CopyTo(old_file.FullName);
                            Row["NeedLoad"] = false;
                        }
                        catch (Exception) { }

                    }
                }
            }

            pbc_upload.Position = Convert.ToInt32((cur_load_all / need_load) * 10000);
            //double pers = Math.Round((double)pbc_upload.Position / 100,2);
            int pers = pbc_upload.Position / 100;
            string size_text = size_to_text_point(cur_load_all);

            label_info.Text = "Загрузка обновления: " + pers.ToString() + " % (" + size_text + ")"; //pbc_upload.Position
            
            if (pbc_upload.Position == pbc_upload.Properties.Maximum)
            {
                label_info.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                label_info.Text = "Загрузка завершена";
                sbtn_Close.Text = "Закрыть";
            }
        }

        // модификаторы размера данных
        private string size_to_text(int size)
        {
            string size_text = "";

            if (size == 0) size_text = "";
            else
                if (size < 1024) size_text = size.ToString() + " байт";
                else
                    if (size < 1024 * 1024) size_text = Math.Round(Convert.ToDouble(size / 1024), 2).ToString() + " Кб";
                    else
                        if (size < 1024 * 1024 * 1024) size_text = Math.Round(Convert.ToDouble(size / (1024 * 1024)), 2).ToString() + " Мб";
                        else
                            size_text = Math.Round(Convert.ToDouble(size / (1024 * 1024 * 1024)), 2).ToString() + " Гб";

            return size_text;
        }

        private string size_to_text_point(double size)
        {
            string size_text = "";

            if (size == 0) size_text = "";
            else
                if (size < 1024) size_text = size.ToString() + " байт";
                else
                    if (size < 1024 * 1024) size_text = Math.Round(size / 1024, 2, MidpointRounding.AwayFromZero).ToString() + " Кб";
                    else
                        if (size < 1024 * 1024 * 1024) size_text = Math.Round(size / (1024 * 1024), 2, MidpointRounding.AwayFromZero).ToString("#.00") + " Мб";
                        else
                            size_text = Math.Round(size / (1024 * 1024 * 1024), 2, MidpointRounding.AwayFromZero).ToString("#.00") + " Гб";

            return size_text;

        }

        // запуск обновления
        private void btn_update_Click(object sender, EventArgs e)
        {            
            Upload_NewVersion();
            sbtn_Close.Visible = true;
            sbtn_Close.Text = "Отмена";

            sbtn_update.Visible = false;
            sbtn_cancel.Visible = false;
        }
        
        // прогресс потока загрузки файла
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            string filename = (sender as WebClient).Headers.GetValues(0)[0];

            foreach (DataRow Row in dt_File_List.Rows)
            {
                if (Row["Name"].ToString() == filename)
                {
                    int cur_load = e.ProgressPercentage * Convert.ToInt32(Row["Size"].ToString()) / 100;
                    Row["SizeLoadNow"] = cur_load;                        
                    break;
                }
            }

            Check_Loading_Status();
        }

        // завершение загрузки файла
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            string filename = (sender as WebClient).Headers.GetValues(0)[0];

            foreach (DataRow Row in dt_File_List.Rows)
            {
                if (Row["Name"].ToString() == filename)
                {
                    int cur_load = Convert.ToInt32(Row["Size"].ToString());
                    Row["SizeLoadNow"] = cur_load;                        
                    break;
                }
            }
        }

        // закрытие обновления
        private void sbtn_Close_Click(object sender, EventArgs e)
        {
            NeedUpdate = true;
            this.Close();
        }
        
    }
}
