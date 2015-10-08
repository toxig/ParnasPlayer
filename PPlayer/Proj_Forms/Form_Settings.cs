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

        internal void InitSettings(MySettings UserSettings)
        {
            CurSettings = UserSettings; // начальные настройки            
        }

        private void btn_Close_form_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}