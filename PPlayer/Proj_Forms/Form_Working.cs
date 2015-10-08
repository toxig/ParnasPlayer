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
    // процедура подкючения к системе
    public partial class Form_Working : DevExpress.XtraEditors.XtraForm
    {
        public String param_Operation_Text;
        public bool param_Show_Cansel = false;
        public bool param_Form_Close = false;
        public bool param_Form_Cansel = false;
        private String Operation_OLD_Text;

        private int Height_noCancel = 135; // без кнопки отмена - с учетом windows крупный текст - 150%
        private int Height_wCancel = 185; // высота с кнопкой отмена

        public int val_max = 0;
        public int val_min = 0;

        public Form_Working()
        {            
            InitializeComponent();            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            param_Form_Close = true;
            this.Hide();
        }

        //включение информера
        private void Form_Working_Shown(object sender, EventArgs e)
        {
            this.Text = "Запрос";

            label_Operatipon.Text = param_Operation_Text;
            Operation_OLD_Text = param_Operation_Text;            

            if (!param_Show_Cansel)
            {
                this.Height = Height_noCancel;
                simpleButton1.Visible = false;
            }

            // обнуляем параметры
            param_Form_Close = false;
            param_Form_Cansel = false;

            timer1.Start();
        }

        // обновление информера о состоянии подключения
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (param_Form_Close)
            {//Недопустимая операция в нескольких потоках: попытка доступа к элементу управления "Form_Working" не из того потока, в котором он был создан.
                timer1.Stop();
                this.Close();
                return;
            }
            else
                if (param_Operation_Text != Operation_OLD_Text)
                {
                    try
                    {
                        label_Operatipon.Text = param_Operation_Text;
                        Operation_OLD_Text = param_Operation_Text;
                    }
                    catch (Exception) { }
                }
                else
                {
                    if (!param_Show_Cansel && simpleButton1.Visible)
                    {
                        this.Height = Height_noCancel;
                        simpleButton1.Visible = false;
                    }
                    else
                        if (param_Show_Cansel && !simpleButton1.Visible)
                        {
                            this.Height = Height_wCancel;
                            simpleButton1.Visible = true;
                        }
                }

            if (val_min < val_max)
            {
                marqueeProgressBarControl1.Visible = false;
                progressBarControl1.Visible = true;
                progressBarControl1.Properties.Maximum = val_max;
                progressBarControl1.Position = val_min;
            }
            else
            {
                marqueeProgressBarControl1.Visible = true;
                progressBarControl1.Visible = false;
            }

        }
    }
}