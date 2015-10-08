using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PPlayer
{
    public partial class Form_Settings : DevExpress.XtraEditors.XtraForm
    {        
        private MySettings CurSettings;



        public Form_Settings()
        {
            InitializeComponent();
        }

        // настройки в форму
        internal void InitSettings(MySettings UserSettings)
        {
            CurSettings = UserSettings; // начальные настройки

            checkEdit_uodates.Checked = CurSettings.p_check_updates;            
        }

        // изменение формы созраняем в настройки
        private void btn_Save_Changes_Click(object sender, EventArgs e)
        {
            CurSettings.p_check_updates = checkEdit_uodates.Checked;

            CurSettings.Save();
        }

        private void btn_Close_form_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkEdit_FileAssociate_CheckedChanged(object sender, EventArgs e)
        {
            FileAssociation FIcon = new FileAssociation();

            if (checkEdit_FileAssociate.Checked)
            {
                FIcon.Remove();
                FIcon.Associate("Плейлист для Парнас Плеера", System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\pm_file.ico");
            }
            else
            {
                FIcon.Remove();
            }
             
        }

    }
}