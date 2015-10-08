using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Un4seen.Bass;

namespace PPlayer
{
    public partial class Control_EQ : DevExpress.XtraEditors.XtraUserControl
    {
        public int[] fx;
        public DataRow Row;

        private bool fl_eqLoading = false;
        private int eq_on_off = 1;
        private BASS_DX8_PARAMEQ eq = new BASS_DX8_PARAMEQ();  // переменная эквалайзера Bass.dll        
        private int[] EQfGain = new int[11 + 1]; // Preamp + Состояние эквалайзера (11 полос)
        private float[] EQCenter =  {0, // частотные полосы эквалайзера
                                    80,  125, 170,   
                                    310,  600,  1000,  
                                    3000, 6000, 12000, 
                                    14000, 16000};
        private float[] EQBandwidth = {0, // Пропускная способность полутонов, 1 .. 36 def 18 
                                    9, 10, 8,   
                                    11, 8, 20,  
                                    12, 12, 3, 
                                    1, 4};
/*
        60 - 100гц: 9 полутонов
100 - 170гц: 10
170 - 310гц: 8
310 - 600гц: 11
600 - 1кгц: 8
1 - 3кгц: 20
3 - 6кгц: 12
6 - 12кгц: 12
12 - 14кгц: 3
14 - 16кгц: 1 полутон
16 до края около 4 полутонов.
*/

        public Control_EQ()
        {
            InitializeComponent();
            Init_Labels();
            Init_ProgressBars();
        }

        // Инициализация полос эквалайзера
        public void Init_fx(int channel)
        {
            fx = new int[EQCenter.Length];

            for (int i = 1; i < EQCenter.Length; i++)
            {
                fx[i] = Bass.BASS_ChannelSetFX(channel, BASSFXType.BASS_FX_DX8_PARAMEQ, 1);
            }           
        }

        // Инициализация подписей для полос эквалайзера
        private void Init_Labels()
        {
            object sender;                        

            // перебор подписей полос эквалайзера
            for (int i = 1; i < EQCenter.Length; i++)
            {
                sender = FindEqBarByName("lc_eq" + i.ToString());
                if (sender != null)
                {
                    string text = "";

                    if (EQCenter[i] < 1000) text = EQCenter[i].ToString();
                    else text = ((float)EQCenter[i] / 1000).ToString() + "k";

                    ((LabelControl)sender).Text = text;                    
                    //((DevExpress.XtraEditors.Controls.RadioGroupItem)((new System.Collections.ArrayList.ArrayListDebugView(((System.Collections.CollectionBase)(radioGroup.Properties.Items)).InnerList)).Items[i])).Description = text;
                    radioGroup.Properties.Items[i-1].Description = text;
                }
            }

            fl_eqLoading = false;
        }

        private void Init_ProgressBars()
        {
            // перебор всех прогресс баров
            foreach (Control c in panelControl_EQ.Controls) //assuming this is a Form
            {
                if (c.GetType().Name == "ProgressBarControl")
                {
                    ((ProgressBarControl)c).MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pbc_equal_main_MouseWheel);                    
                }
            }            
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

        // вкл/откл эквалайзера
        private void btn_eq_on_off_CheckedChanged(object sender, EventArgs e)
        {
            if (btn_eq_on_off.Checked)
            {
                btn_eq_on_off.Text = "ВКЛЮЧЕН";
                eq_on_off = 1;
            }
            else
            {
                btn_eq_on_off.Text = "ОТКЛЮЧЕН";
                eq_on_off = 0;
            }

            Eq_array_to_stream();
        }

        // изменение уровней мышкой (visual)
        private void pbc_equal_main_MouseMove(object sender, MouseEventArgs e)
        {
            int max = ((ProgressBarControl)sender).Properties.Maximum;
            int min = ((ProgressBarControl)sender).Properties.Minimum;
            int pos = ((ProgressBarControl)sender).Position;

            ((ProgressBarControl)sender).Focus();

            #region Одиночное изменение уровня
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                pos = max * (e.Y) / ((ProgressBarControl)sender).Height;

                if (pos > max) { pos = max; }
                if (pos < min) { pos = min; }

                pos = max - pos;

                ((ProgressBarControl)sender).Position = pos;
            }
            #endregion

            #region Групповое изменение уровней
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                int dx_pos = pos;

                ((ProgressBarControl)sender).Cursor = Cursors.NoMoveVert;

                pos = max * (e.Y) / ((ProgressBarControl)sender).Height;

                if (pos > max) { pos = max; }
                if (pos < min) { pos = min; }

                pos = max - pos;

                dx_pos = pos - dx_pos;

                ((ProgressBarControl)sender).Position = pos;

                #region Группа уровней
                if (((ProgressBarControl)sender).Tag != null)
                {
                    string eq_name = ((ProgressBarControl)sender).Name.ToString();

                    /*if (eq_name == "pbc_eq16" || eq_name == "pbc_eq16" || eq_name == "pbc_eq15" || eq_name == "pbc_eq14" || eq_name == "pbc_eq13")
                    {
                        if (pbc_eq16.Name != eq_name) pbc_eq16.Position += dx_pos;
                        if (pbc_eq15.Name != eq_name) pbc_eq15.Position += dx_pos;
                        if (pbc_eq14.Name != eq_name) pbc_eq14.Position += dx_pos;
                        if (pbc_eq13.Name != eq_name) pbc_eq13.Position += dx_pos;
                        if (pbc_eq12.Name != eq_name) pbc_eq12.Position += dx_pos;
                    }*/

                    if (eq_name == "pbc_eq11" || eq_name == "pbc_eq10" || eq_name == "pbc_eq9")
                    {
                        if (pbc_eq11.Name != eq_name) pbc_eq11.Position += dx_pos;
                        if (pbc_eq10.Name != eq_name) pbc_eq10.Position += dx_pos;
                        if (pbc_eq9.Name != eq_name) pbc_eq9.Position += dx_pos;
                    }

                    if (eq_name == "pbc_eq8" || eq_name == "pbc_eq7" || eq_name == "pbc_eq6")
                    {
                        if (pbc_eq8.Name != eq_name) pbc_eq8.Position += dx_pos;
                        if (pbc_eq7.Name != eq_name) pbc_eq7.Position += dx_pos;
                        if (pbc_eq6.Name != eq_name) pbc_eq6.Position += dx_pos;
                    }

                    if (eq_name == "pbc_eq5" || eq_name == "pbc_eq4" || eq_name == "pbc_eq3")
                    {
                        if (pbc_eq5.Name != eq_name) pbc_eq5.Position += dx_pos;
                        if (pbc_eq4.Name != eq_name) pbc_eq4.Position += dx_pos;
                        if (pbc_eq3.Name != eq_name) pbc_eq3.Position += dx_pos;
                    }

                    if (eq_name == "pbc_eq2" || eq_name == "pbc_eq1")
                    {
                        if (pbc_eq2.Name != eq_name) pbc_eq2.Position += dx_pos;
                        if (pbc_eq1.Name != eq_name) pbc_eq1.Position += dx_pos;
                    }
                }
                #endregion

            }
            else ((ProgressBarControl)sender).Cursor = Cursors.HSplit;

            #endregion
           
            int eq_number = Int16.Parse(((ProgressBarControl)sender).Tag.ToString());            

            #region Всплывающая подсказка
            pbc_equal_show_tooltip(sender, pos, max);
            #endregion
        }

        // изменение уровней скролом мышки (visual)
        private void pbc_equal_main_MouseWheel(object sender, MouseEventArgs e)
        {
            int max = ((ProgressBarControl)sender).Properties.Maximum;
            int min = ((ProgressBarControl)sender).Properties.Minimum;
            int pos = ((ProgressBarControl)sender).Position;

            if ((pos < max && pos > min) ||
                (pos == min && e.Delta > 0) ||
                (pos == max && e.Delta < 0)
                )
            {
                if (e.Delta < 0)
                    ((ProgressBarControl)sender).Position--;
                else
                    ((ProgressBarControl)sender).Position++;                
            }
            
            pos = ((ProgressBarControl)sender).Position;

            pbc_equal_show_tooltip(sender, pos, max);
        }

        // Всплывающая подсказка эквалайзера
        private void pbc_equal_show_tooltip(object sender, int pos, int max)
        {
            if (((ProgressBarControl)sender).Tag != null)
            {
                double db_val = Math.Round((((double)pos / max) - 0.5) * 30, 2);
                int eq_number = Int16.Parse(((ProgressBarControl)sender).Tag.ToString());
                string tt_str = "";
                
                if (eq_number == 0) // преамп
                    tt_str = "Pre [" + db_val.ToString("0.00") + " db]";
                
                if (eq_number == 100) // добротность
                    tt_str = "[" + EQBandwidth[radioGroup.SelectedIndex + 1].ToString() + " pt] - добротность полосы";

                if (eq_number > 0 && eq_number < 100) // полосы эквалайзера
                    tt_str = EQCenter[eq_number].ToString() + " Hz [" + db_val.ToString("0.00") + " db]";

                if (eq_number == 101) // полосы эквалайзера
                    tt_str = "Громкость [" + ((ProgressBarControl)sender).Position.ToString("0") + " %]";

                //string hz_str = ((ProgressBarControl)sender).Tag.ToString();

                ((ProgressBarControl)sender).ToolTip = tt_str;
                toolTipController.ShowHint(tt_str);
            }
        }

        // объект в поток и массив
        private void pbc_equal_main_value_changed(object sender, EventArgs e)
        {
            if (fl_eqLoading) return;

            if (((ProgressBarControl)sender).Tag != null)
            {
                double pos = ((ProgressBarControl)sender).Position;
                double pos_max = ((ProgressBarControl)sender).Properties.Maximum;
                //double fGain_coef = (((pos / pos_max) - 0.5) * 30); // (range -15..15 def 0 ) + preamp
                int eq_number = Int16.Parse(((ProgressBarControl)sender).Tag.ToString()); // порядковый номер полосы экв.                
                EQfGain[eq_number] = Convert.ToInt32(pos) - 15;

                if (eq_number > 0 && eq_number < 100 && radioGroup.SelectedIndex != eq_number - 1)
                    radioGroup.SelectedIndex = eq_number - 1;

                if (fx != null)
                {
                    Bass.BASS_FXGetParameters(fx[eq_number], eq);

                    eq.fBandwidth = EQBandwidth[eq_number]; // Пропускная способность полутонов, 1 .. 36 def 18                
                    eq.fCenter = EQCenter[eq_number]; // 100f; // центральная (или рабочая) частота в герцах (Гц); (80 .. 16000)
                    eq.fGain = (float)(EQfGain[eq_number] + EQfGain[0]) * eq_on_off; // 0f; // уровень усиления или ослабления выбранной полосы в децибелах (дБ); (range -15..15 def 0 )                                                    

                    Bass.BASS_FXSetParameters(fx[eq_number], eq);
                }
                Eq_array_to_row();
            }
        }

        // Preamp
        private void pbc_equal_preamp_value_changed(object sender, EventArgs e)
        {
            if (fl_eqLoading) return;

            double pos = ((ProgressBarControl)sender).Position;
            int pos_max = ((ProgressBarControl)sender).Properties.Maximum;

            EQfGain[0] = (int)(((pos / pos_max) - 0.5) * 30);
            
            Eq_array_to_stream();
            Eq_array_to_row();
        }

        // Сброс настроек эквалайзера
        private void btn_eq_clear_Click(object sender, EventArgs e)
        {
            object eq_line;            

            fl_eqLoading = true; // флаг очистки

            for (int i = 1; i < EQCenter.Length; i++)
            {
                if (fx != null)
                {
                    Bass.BASS_FXGetParameters(fx[i], eq);

                    eq.fGain = 0f; // нулевое отклонение
                    eq.fBandwidth = EQBandwidth[i];
                    eq.fCenter = EQCenter[i];

                    Bass.BASS_FXSetParameters(fx[i], eq);
                }

                // установка значения у контрола
                eq_line = FindEqBarByName("pbc_eq" + i.ToString());
                if (eq_line != null)
                {
                    ((ProgressBarControl)eq_line).Position = ((ProgressBarControl)eq_line).Properties.Maximum / 2;
                }
            }

            eq_line = FindEqBarByName("pbc_eq_preamp");
            if (eq_line != null)
            {
                int pos_max = ((ProgressBarControl)eq_line).Properties.Maximum;
                ((ProgressBarControl)eq_line).Position = ((ProgressBarControl)eq_line).Properties.Maximum / 2;
            }

            fl_eqLoading = false;

            Eq_object_to_array();
            Eq_array_to_row();
        }

        // массив на объекты
        public void Eq_array_to_object()
        {
            object sender;
            fl_eqLoading = true;

            // Preamp
            sender = FindEqBarByName("pbc_eq_preamp");
            if (sender != null)
            {
                int pos_max = ((ProgressBarControl)sender).Properties.Maximum;
                ((ProgressBarControl)sender).Position = (EQfGain[0] + 15);
            }

            // перебор значений полос эквалайзера
            for (int i = 1; i < EQCenter.Length; i++)
            {
                sender = FindEqBarByName("pbc_eq" + i.ToString());
                if (sender != null)
                {
                    int pos_max = ((ProgressBarControl)sender).Properties.Maximum;
                    //int fGain_coef = (int)(((pos / pos_max) - 0.5) * 30);

                    ((ProgressBarControl)sender).Position = (EQfGain[i] + 15);
                }
            }

            fl_eqLoading = false;
        }

        // массив в поток
        public void Eq_array_to_stream()
        {
            fl_eqLoading = true; // флаг загрузки            

            for (int i = 1; i < EQCenter.Length; i++)
            {
                Bass.BASS_FXGetParameters(fx[i], eq);

                eq.fBandwidth = EQBandwidth[i]; // Пропускная способность полутонов, 1 .. 36 def 18                
                eq.fCenter = EQCenter[i]; // 100f; // центральная (или рабочая) частота в герцах (Гц); (80 .. 16000)
                eq.fGain = (float)(EQfGain[i] + EQfGain[0]) * eq_on_off; // 0f; // уровень усиления или ослабления выбранной полосы в децибелах (дБ); (range -15..15 def 0 )

                Bass.BASS_FXSetParameters(fx[i], eq);
            }

            fl_eqLoading = false;
        }

        // массив в строку (из файла)
        public void Eq_array_to_row()
        {
            if (Row == null) return;

            Row[3] = (EQfGain[0] * (-1));

            string EQfGain_str = "";

            for (int i = 1; i < EQfGain.Length; i++)
            {
                EQfGain_str = EQfGain_str + (EQfGain[i] * (-1)).ToString() + ':';
            }
            Row[2] = EQfGain_str;
        }

        // строка в массив (из файла)
        public void Eq_row_to_array()
        {
            string eq_line = Row[2].ToString();
            string eq_preamp = Row[3].ToString();

            string[] EQfGain_str = (eq_preamp + ':' + eq_line).Split(':');

            for (int i = 0; i < EQfGain.Length; i++)
            {
                EQfGain[i] = Convert.ToInt32(EQfGain_str[i]) * (-1);
            }
        }

        // объекты в массив
        public void Eq_object_to_array()
        {
            object sender;
            
            // перебор полос эквалайзера
            for (int i = 1; i < EQCenter.Length; i++)
            {
                sender = FindEqBarByName("pbc_eq" + i.ToString());
                if (sender != null)
                {
                    double pos = ((ProgressBarControl)sender).Position;
                    int pos_max = ((ProgressBarControl)sender).Properties.Maximum;
                    int fGain_coef = (int)(((pos / pos_max) - 0.5) * 30);

                    // отклонение (range -15..15 def 0 ) относительно нуля - середины EQ полосы                    
                    EQfGain[i] = fGain_coef;
                }
            }

            // Preamp
            sender = FindEqBarByName("pbc_eq_preamp");
            if (sender != null)
            {
                double pos = ((ProgressBarControl)sender).Position;
                int pos_max = ((ProgressBarControl)sender).Properties.Maximum;

                EQfGain[0] = (int)(((pos / pos_max) - 0.5) * 30);
            }

        }

        private void btn_eq_save_Click(object sender, EventArgs e)
        {
            Eq_array_to_stream();
        }

        // Изменение добротности
        private void pbc_equal_dobr_value_changed(object sender, EventArgs e)
        {
            double pos = ((ProgressBarControl)sender).Position;
            double pos_max = ((ProgressBarControl)sender).Properties.Maximum;
            float value = (float)((pos / pos_max) * 36); // Пропускная способность полутонов, 1 .. 36 def 18

            EQBandwidth[radioGroup.SelectedIndex + 1] = value;
            lc_dobr.Text = "Добр. " + EQCenter[radioGroup.SelectedIndex + 1] + " [" + value + "pt]";
            
            //Eq_array_Commit();
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

                //WaytDelay = 3;
                //Label_InfoLine.Text = tt_str;

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

        private void radioGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            pbc_eq_dob.Position = (int)EQBandwidth[radioGroup.SelectedIndex + 1];
            toolTipController.ShowHint("[" + radioGroup.Properties.Items[radioGroup.SelectedIndex].Description + "]");

            lc_dobr.Text = "Добр. " + EQCenter[radioGroup.SelectedIndex + 1] + " [" + EQBandwidth[radioGroup.SelectedIndex + 1] + "pt]";            
        }

    }
}
