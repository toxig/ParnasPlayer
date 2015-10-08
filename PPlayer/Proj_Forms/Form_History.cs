using System;
using System.Text;
using System.Xml;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace PPlayer
{
    public partial class Form_History : DevExpress.XtraEditors.XtraForm
    {
        public string v_list_name;
        public string v_list_change_log;
        public string v_init_dir;
        public string v_file_name;
        public int    v_resault = 0; // 0/1/2/3 - отмена/не соранять/сохранить/сохранить как

        public Form_History()
        {            
            InitializeComponent();
        }

        // Загрузка окна
        private void Form_History_Load(object sender, EventArgs e)
        {
            memoEdit_info.Text = v_list_change_log;
            label_info.Text = v_list_name;
            memoEdit_info.DeselectAll();
        }

        // Назад (отмена) - 0
        private void btn_cancel_Click(object sender, EventArgs e)
        {
            v_resault = 0;
            this.Close();
        }

        // Не сохранять - 1
        private void sbtn_No_Save_Click(object sender, EventArgs e)
        {            
            v_resault = 1;
            this.Close();
        }

        // Сохранить - 2
        private void sbtn_save_Click(object sender, EventArgs e)
        {
            if (v_file_name == "" || v_file_name == null)
                sbtn_save_as_Click(null, null);
            else
            {
                v_resault = 2;
                this.Close();
            }
        }

        // Сохранить как... - 3
        private void sbtn_save_as_Click(object sender, EventArgs e)
        {            
            saveFileDialog.InitialDirectory = v_init_dir;
            string name = v_list_name.Substring(4,v_list_name.Length - 4);
            saveFileDialog.FileName = name.Replace("\"","");
            saveFileDialog.Filter = "PlayList Parnas Machine (*.pmp)|*.pmp|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                v_file_name = saveFileDialog.FileName;
                v_resault = 3;
                this.Close();
                
            }
        } 
    }
}
