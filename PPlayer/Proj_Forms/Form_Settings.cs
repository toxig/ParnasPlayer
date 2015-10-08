using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

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

    }
}