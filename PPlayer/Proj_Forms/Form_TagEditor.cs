using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace PPlayer
{
    public partial class Form_TagEditor : DevExpress.XtraEditors.XtraForm
    {
        public string File_Path;
        public string ResaultMsg;
        public StreamClass MainStream;

        private bool fl_loading = false;
        private bool fl_isChanged = false;
        private string Tag_Artist;
        private string Tag_Title;
        private string Tag_Comments;
        //private UltraID3 Tag_File;
        private TagLib.File Tag_File;

        private bool _TagsActive = true;
        private bool TagsActive
        {
            get { return _TagsActive; }
            set
            {
                if (value)
                {   // редактор доступен
                    TE_Artist.Properties.ReadOnly = false;
                    TE_Name.Properties.ReadOnly = false;
                    memoEdit_Comments.Properties.ReadOnly = false;
                    CheckList_TagsGrops.Enabled = true;
                }
                else // редактор не доступен
                {
                    TE_Artist.Properties.ReadOnly = true;
                    TE_Name.Properties.ReadOnly = true;
                    memoEdit_Comments.Properties.ReadOnly = true;
                    CheckList_TagsGrops.Enabled = false;
                }                
                _TagsActive = value;
            }
        }

        //TagLib.File Tag_File;


        string[] Tags_Groups = { "Медленные", "Быстрые",
                                 "Женские", "Мужские", "Детские",
                                 "Гена", "Элла", "Сергей",
                                 "Зарубежные", "Русские", "Народные","Армянские",
                                 "Военные","Свадебные", "ДРожд", "НГод", "ДляЖ", "ДляМ",
                                 "Ретро", "Лаундж", "Рок", "Романсы", "Шансон", "Молодежное",
                                 "Другое",};

        // Кодировка русских символов в тэгах
        public static string to_UTF8(string unknown)
        {
            if (unknown == null) return "";
            else
            {
                //return new string(unknown.ToCharArray().
                //    Select(x => ((x + 848) >= 'А' && (x + 848) <= 'ё') ? (char)(x + 848) : x).
                //    ToArray());

                string val = "";
                foreach (Char x in unknown.ToCharArray())
                {
                    if ((x + 848) >= 'А' && (x + 848) <= 'ё')
                    {
                        val += (char)(x + 848);
                    }
                    else val += x;
                }

                return val;
            }
        }
        
        public Form_TagEditor()
        {
            InitializeComponent();                                               
        }

        // Загрузка редактора
        private void Form_TagEditor_Load(object sender, EventArgs e)
        {
            fl_loading = true;
            Load_TagsList();

            if (File.Exists(File_Path))
            {
                //Tag_File = new UltraID3();
                //Tag_File.Read(File_Path);
                Tag_File = TagLib.File.Create(File_Path);
                
                Load_FileTags();
                string Ext = Path.GetExtension(File_Path).ToLower();

                if (Ext != ".mp3")
                {
                    TagsActive = false;
                    memoEdit_Comments.Text = "Файл \"" + Ext +"\" не содержит тегов!";
                }
                else
                    TagsActive = true;
            }
            else
            {
                TagsActive = false;
                Tag_File = null;
                ResaultMsg = "Файл не найден: " + File_Path;
                memoEdit_Comments.Text = "Файл не найден!";

                TE_FilePath.Text = Path.GetFileName(File_Path);
                TE_FilePath.ToolTip = File_Path;
                TE_Artist.Text = "";
                TE_Name.Text = "";                
            }
            fl_loading = false;

            Test_IsDataChanged();

            TE_FilePath.DeselectAll();
        }

        // Загрузка тэгов из файла
        private void Load_FileTags()
        {
            // Исполнитель
            try 
            { 
                //Tag_Artist = Tag_File.Artist; 
                Tag_Artist = to_UTF8(Tag_File.Tag.Performers[0]);
            } // Учавствующие исполниетели
            catch (Exception) 
            {
               Tag_Artist = "";
            }

            // Название трека
            try 
            { 
                //Tag_Title = Tag_File.ID3v2Tag.Title; 
                Tag_Title = to_UTF8(Tag_File.Tag.Title);
            }
            catch (Exception) 
            { 
                Tag_Title = ""; 
            }
            

            // Комментарии
            try 
            { 
                //Tag_Comments = to_UTF8(Tag_File.ID3v2Tag.Comments); 
                
                Tag_Comments = to_UTF8(Tag_File.Tag.Comment);
                //byte[] array = Encoding.Default.GetBytes(Tag_File.Tag.Comment);
                //Tag_Comments = Encoding.UTF8.GetString(array);
                //Tag_Comments = Encoding.UTF8.GetString(Tag_File.Tag.Comment);
            }
            catch (Exception) 
            { 
                Tag_Comments = ""; 
            }
            
            TE_FilePath.Text = Path.GetFileName(File_Path);
            TE_FilePath.ToolTip = File_Path;

            TE_Artist.Text = Tag_Artist;
            TE_Name.Text = Tag_Title;
            memoEdit_Comments.Text = Tag_Comments;

            CheckList_SetChecked();
            Test_IsDataChanged();
        }

        // загрузка предустановленных типов меток
        private void Load_TagsList()
        {
            CheckList_TagsGrops.Items.Clear();

            foreach (string tag in Tags_Groups)
            {
                CheckList_TagsGrops.Items.Add(tag);
            } 
        }

        // Изменение тэга в списке чеклиста
        private void CheckList_TagsGrops_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
            if (fl_loading) return;

            string TagsList = "";

            // удалить все
            for (int i = 0; i < CheckList_TagsGrops.Items.Count; i++)
            {
                memoEdit_Comments.Text = memoEdit_Comments.Text.Replace(CheckList_TagsGrops.Items[i].ToString().Trim() + ";", "");                
            }
            memoEdit_Comments.Text = memoEdit_Comments.Text.Replace("  "," ").Trim();

            for (int i = 0; i < CheckList_TagsGrops.Items.Count; i++)
            {
                if (CheckList_TagsGrops.Items[i].CheckState == CheckState.Checked)
                {                    
                    TagsList += CheckList_TagsGrops.Items[i].ToString().Trim() + "; ";
                }
            }

            if (TagsList.Trim() != "")
            {
                memoEdit_Comments.Text = TagsList.Trim() + " " + memoEdit_Comments.Text;
            }

            Test_IsDataChanged();
        }

        // Отмечаем тэги, которые есть в комментах
        private void CheckList_SetChecked()
        {
            fl_loading = true;

            for (int i = 0; i < CheckList_TagsGrops.Items.Count; i++)
            {
                if (memoEdit_Comments.Text.Contains(CheckList_TagsGrops.Items[i].ToString()+";"))
                {
                    CheckList_TagsGrops.Items[i].CheckState = CheckState.Checked;
                }
                else
                {
                    CheckList_TagsGrops.Items[i].CheckState = CheckState.Unchecked;
                }
            }

            fl_loading = false;
        }

        // Закрываем окно
        private void Form_Close(object sender, EventArgs e)
        {
            if (fl_isChanged)
                switch (XtraMessageBox.Show("Данные изменились, сохранить?", "Вопрос", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case System.Windows.Forms.DialogResult.Yes: Save_Tags(null, null); break;
                    case System.Windows.Forms.DialogResult.Cancel: return;
                }

            this.Close();
        }

        // Очистка выбора тэгов
        private void Clear_All(object sender, EventArgs e)
        {
            for (int i = 0; i < CheckList_TagsGrops.Items.Count; i++)
            {
                CheckList_TagsGrops.Items[i].CheckState = CheckState.Unchecked;
            }
        }

        // Выбрать все тэги
        private void Select_All(object sender, EventArgs e)
        {
            for (int i = 0; i < CheckList_TagsGrops.Items.Count; i++)
            {
                CheckList_TagsGrops.Items[i].CheckState = CheckState.Checked;
            }
        }

        // Сохранение настроек
        private void Save_Tags(object sender, EventArgs e)
        {
            if (Tag_File != null)
            {
                if (MainStream != null)
                {
                    if (MainStream.v_stream != 0) MainStream.v_stream_status = StreamStatus.FREE;
                }

                try 
                {
                    //Tag_File.ID3v2Tag.Artist = TE_Artist.Text;                    
                    //Tag_File.ID3v2Tag.Title = TE_Name.Text;                    
                    //Tag_File.ID3v2Tag.Comments = memoEdit_Comments.Text;
                    //Tag_File.Write();

                    if (Tag_File.Writeable)
                    {
                        Tag_File.Tag.Performers = new string[1] {TE_Artist.Text};
                        Tag_File.Tag.Title = TE_Name.Text;                        
                        Tag_File.Tag.Comment = memoEdit_Comments.Text;

                        Tag_File.Save();

                        Tag_Artist = TE_Artist.Text;
                        Tag_Title = TE_Name.Text;
                        Tag_Comments = memoEdit_Comments.Text;
                    }
                    else XtraMessageBox.Show("Файл занят другим процессом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                catch (Exception) 
                {
                    XtraMessageBox.Show("Ошибка при сохранинии тэгов.\nФайл занят другим процессом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            Test_IsDataChanged();
        }

        // Проверка изменений
        private void Test_IsDataChanged()
        {
            fl_isChanged = false;

            if (Tag_Artist != TE_Artist.Text) fl_isChanged = true;

            if (Tag_Title != TE_Name.Text) fl_isChanged = true;

            if (Tag_Comments != memoEdit_Comments.Text) fl_isChanged = true;

            if (fl_isChanged && Tag_File != null)
            {
                Cancel_Changes.Enabled = true;
                Save_Changes.Enabled = true;
            }
            else
            {
                Cancel_Changes.Enabled = false;
                Save_Changes.Enabled = false;
            }
        }

        private void Cancel_Changes_Click(object sender, EventArgs e)
        {
            Load_FileTags();
        }

        private void TE_EditValueChanged(object sender, EventArgs e)
        {
            Test_IsDataChanged();
        }

    }
}