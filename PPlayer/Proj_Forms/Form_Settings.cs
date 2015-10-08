using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

namespace PPlayer
{
    public partial class Form_Settings : DevExpress.XtraEditors.XtraForm
    {        
        private MySettings CurSettings;
        private DataTable dt_ListData;
        private bool _v_need_save = false;
        private bool v_need_save
        {
            get
            {
                return _v_need_save;
            }
            set
            {
                _v_need_save = value;
                btn_Save_Changes.Enabled = value;
                btn_Cancel_Changes.Enabled = value;
            }
        }
        public bool v_settings_updated = false;

        public Form_Settings()
        {
            InitializeComponent();
        }

        #region Управляющие функции
       
        // получаем настройки из главной формы
        internal void InitSettings(MySettings UserSettings)
        {
            CurSettings = UserSettings; // начальные настройки                       
            Load_file_settings();

            dt_ListData = Get_PlayList_Test_GridTable();
            grid_PlayList.DataSource = dt_ListData;
        }

        // Таблица плейлиста для GridView
        static DataTable Get_PlayList_Test_GridTable()
        {
            //
            // Create a DataTable with columns.
            //
            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            //table.Columns.Add("Artist", typeof(string));
            table.Columns.Add("FileExistsFlag", typeof(string));
            
            //
            // Add DataRows.
            // массив флагов: 0 или 1 [наличие обработки][есть mp3][есть текст]
            //table.Rows.Add(25, "Indocin", "David", DateTime.Now);
            table.Rows.Add("Медведи - Кавказская пленница (ok)", "111"); // Все ок
            table.Rows.Add("Сибирячка - Кемеровский (ok)", "111"); // Все ок
            table.Rows.Add("Come together - The Beatles (ok)", "111"); // Все ок                   
            table.Rows.Add("Мосты - Шуфутинский (нет mp3)", "101"); // нет mp3
            table.Rows.Add("Пусть - Буйнов (нет текста)", "110"); // нет текста
            table.Rows.Add("Bora Bora - Arash (ok)", "111"); // Все ок
            table.Rows.Add("Folling im love - Presli (ok)", "111"); // Все ок            

            return table;
        }

        // загрузка настроек в редактор
        private void Load_file_settings()
        {
            checkEdit_updates.Checked = CurSettings.p_check_updates; // обновление при старте
            checkEdit_FileAssociate.Checked = CurSettings.p_file_associate; // асоциация файлов
            PL_Font.EditValue = CurSettings.p_PL_FontName; // шрифт
            comboBoxEdit_FontSize.EditValue = CurSettings.p_PL_FontSize; // размер шрифта
            checkButton_bold.Checked = CurSettings.p_PL_FontBold; // жирный

            // загрузка цветов из настроек
            PL_colorEdit_Back.Color = Color.FromArgb(CurSettings.p_PL_FontColor_back); // цвет - фон списка
            PL_colorEdit_Back.Color = Color.FromArgb(CurSettings.p_PL_FontColor_back); // цвет - фон списка
            PL_colorEdit_Back_select.Color = Color.FromArgb(CurSettings.p_PL_FontColor_back_select); // фон выделение
            PL_colorEdit_Text.Color = Color.FromArgb(CurSettings.p_PL_FontColor_text); // цвет текста
            PL_colorEdit_Text_select.Color = Color.FromArgb(CurSettings.p_PL_FontColor_text_select); // цвет текста - выделение
            PL_colorEdit_Text_no_mp3.Color = Color.FromArgb(CurSettings.p_PL_FontColor_text_no_mp3); // цвет текста - нет mp3
            PL_colorEdit_Text_no_rtf.Color = Color.FromArgb(CurSettings.p_PL_FontColor_text_no_rtf); // цвет текста - нет rtf

            v_need_save = false; // необходимость сохранения
            v_settings_updated = false;
        }

        // Применение настроек и сохранение в файл
        private void Save_file_settings()
        {
            // проверять обновления
            CurSettings.p_check_updates = checkEdit_updates.Checked;

            // асоциация файлов
            if (CurSettings.p_file_associate != checkEdit_FileAssociate.Checked) f_associate_icon(checkEdit_FileAssociate.Checked);
            CurSettings.p_file_associate = checkEdit_FileAssociate.Checked;
            
            // шрифт
            CurSettings.p_PL_FontName = PL_Font.EditValue.ToString();
            CurSettings.p_PL_FontSize = float.Parse(comboBoxEdit_FontSize.Text);
            CurSettings.p_PL_FontBold = checkButton_bold.Checked;

            // сохранение палитры цветов
            CurSettings.p_PL_FontColor_back = PL_colorEdit_Back.Color.ToArgb(); // цвет - фон списка
            CurSettings.p_PL_FontColor_back_select = PL_colorEdit_Back_select.Color.ToArgb(); // фон выделение
            CurSettings.p_PL_FontColor_text = PL_colorEdit_Text.Color.ToArgb(); // цвет текста
            CurSettings.p_PL_FontColor_text_select = PL_colorEdit_Text_select.Color.ToArgb(); // цвет текста - выделение
            CurSettings.p_PL_FontColor_text_no_mp3 = PL_colorEdit_Text_no_mp3.Color.ToArgb(); // цвет текста - нет mp3
            CurSettings.p_PL_FontColor_text_no_rtf = PL_colorEdit_Text_no_rtf.Color.ToArgb(); // цвет текста - нет rtf

            CurSettings.Save();
            v_need_save = false;
            v_settings_updated = true;
        }

        // ассоциация файлов с программой
        private void f_associate_icon(bool flag)
        {
            FileAssociation FIcon = new FileAssociation();

            if (flag)
            {
                FIcon.Remove();
                FIcon.Associate("Плейлист для Парнас Плеера", System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\pm_file.ico");
            }
            else
            {
                FIcon.Remove();
            }             
        }
        #endregion

        #region Общее управление формой

        // изменение формы созраняем в настройки
        private void btn_Save_Changes_Click(object sender, EventArgs e)
        {
            Save_file_settings();
        }

        // отмена изменений настроек
        private void btn_Cancel_Changes_Click(object sender, EventArgs e)
        {
            Load_file_settings();
        }

        // закрыть форму
        private void btn_Close_form_Click(object sender, EventArgs e)
        {
            this.Close();
        } 

        #endregion

        #region События элементов формы
        private void checkEdit_FileAssociate_CheckedChanged(object sender, EventArgs e)
        {            
            v_need_save = false;
        }

        // выбор шрифта
        private void PL_Font_ValueChanged(object sender, EventArgs e)
        {
            gv_PlayList.Appearance.Row.Font = new Font(PL_Font.EditValue.ToString(), // тип шрифта
                                      float.Parse(comboBoxEdit_FontSize.Text), // Размер шрифта
                                      (checkButton_bold.Checked ? FontStyle.Bold : FontStyle.Regular)); // жирный

            gv_PlayList.Appearance.FocusedRow.Font = gv_PlayList.Appearance.Row.Font;
            
            v_need_save = true;
        }

        // проверка размера шрифта
        private void comboBoxEdit_FontSize_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            string val = e.NewValue.ToString();
            
            val = val.Replace(".", ",");
            if (val == "") { val = "1"; }

            e.NewValue = val;

            try
            {
                float sz = float.Parse(val);
            }
            catch (Exception)
            {
                e.Cancel = true;
            }            
        }

        // изменение цвета
        private void PL_color_ValueChanged(object sender, EventArgs e)
        {
            gv_PlayList.Appearance.Empty.BackColor = PL_colorEdit_Back.Color; // фон
            gv_PlayList.Appearance.FocusedRow.BackColor = PL_colorEdit_Back_select.Color; // фон выделение            
            gv_PlayList.Appearance.Row.ForeColor = PL_colorEdit_Text.Color;
            gv_PlayList.Appearance.FocusedRow.ForeColor = PL_colorEdit_Text_select.Color;
            gv_PlayList.FormatConditions[0].Appearance.ForeColor = PL_colorEdit_Text_no_mp3.Color;
            gv_PlayList.FormatConditions[1].Appearance.ForeColor = PL_colorEdit_Text_no_mp3.Color;
            gv_PlayList.FormatConditions[2].Appearance.ForeColor = PL_colorEdit_Text_no_rtf.Color;

            v_need_save = true;
        }
        #endregion

    }
}

