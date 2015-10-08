using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace PPlayer
{
    public partial class Control_EQ : DevExpress.XtraEditors.XtraUserControl
    {
        public int[] fx;
        public int v_stream;
        public DataRow Row;
        public StreamClass MainStream;

        private bool fl_eqLoading = false;
        private bool fl_scroll_Loading = false;
        private int eq_on_off = 1;
        private int eq_volume = 0;
        private int eq_balance = 0;
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
/* добротность частот
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

        [DllImport("bass_fx.dll")]
        static extern bool BASS_ChannelSetAttribute(int handle, BASSAttribute attr,float pitch);

        public static int LoWord(int dwValue)
        {
            return (dwValue & 0xFFFF);
        }

        public static int HiWord(int dwValue)
        {
            return (dwValue >> 16) & 0xFFFF;
        }


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

                if (c.GetType().Name == "TrackBarControl")
                {
                    ((TrackBarControl)c).MouseWheel += new System.Windows.Forms.MouseEventHandler(this.track_bar_MouseWheel);
                }

                if (c.GetType().Name == "TextEdit")
                {
                    ((TextEdit)c).MouseWheel += new System.Windows.Forms.MouseEventHandler(this.TEdit_MouseWheel);
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
                btn_eq_on_off.ToolTip = "Эквалайзер РАБОТАЕТ";
                eq_on_off = 1;                
            }
            else
            {
                btn_eq_on_off.Text = "ОТКЛ";
                btn_eq_on_off.ToolTip = "Эквалайзер ОТКЛЮЧЕН\nРавнозначно установке полос - все в ноль";                
                eq_on_off = 0;                
            }

            toolTipController.ShowHint(btn_eq_on_off.ToolTip);
            Eq_array_to_stream(); // эквалайзер в поток (зависит от eq_on_off)
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

        // объект в поток
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
                //Eq_array_to_row();                
            }

            btn_eq_save.Enabled = true;
            btn_eq_cancel.Enabled = true;
        }

        // Preamp
        private void pbc_equal_preamp_value_changed(object sender, EventArgs e)
        {
            if (fl_eqLoading) return;

            double pos = ((ProgressBarControl)sender).Position;
            int pos_max = ((ProgressBarControl)sender).Properties.Maximum;

            EQfGain[0] = (int)(((pos / pos_max) - 0.5) * 30);
            
            Eq_array_to_stream();

            btn_eq_save.Enabled = true;
            btn_eq_cancel.Enabled = true;
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

            // громкость
            pbc_equal_main.Position = pbc_equal_main.Properties.Maximum / 2;
            // баланс
            trackBar_balance.Value = trackBar_balance.Properties.Maximum / 2;
            // Тональность
            textEdit_Tone.Text = "0";
            // Подстройка
            textEdit_ToneTune.Text = "0";
            // Темп
            textEdit_Temp.Text = "0";

            fl_eqLoading = false;

            Eq_object_to_array();

            btn_eq_save.Enabled = true;
            btn_eq_cancel.Enabled = true;

            //Eq_array_to_row();
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

            // громкость
            pbc_equal_main.Position = eq_volume;

            // баланс
            trackBar_balance.Value = eq_balance;

            // временное решение - привязать к массивам            
            textEdit_Tone.Text = "0";
            textEdit_ToneTune.Text = "0";
            textEdit_Temp.Text = "0";

            fl_eqLoading = false;
        }

        // массив в поток
        public void Eq_array_to_stream()
        {
            if (MainStream == null) return;

            fl_eqLoading = true; // флаг загрузки            

            if (fx != null)
            for (int i = 1; i < EQCenter.Length; i++)
            {
                Bass.BASS_FXGetParameters(fx[i], eq);

                eq.fBandwidth = EQBandwidth[i]; // Пропускная способность полутонов, 1 .. 36 def 18                
                eq.fCenter = EQCenter[i]; // 100f; // центральная (или рабочая) частота в герцах (Гц); (80 .. 16000)
                eq.fGain = (float)(EQfGain[i] + EQfGain[0]) * eq_on_off; // 0f; // уровень усиления или ослабления выбранной полосы в децибелах (дБ); (range -15..15 def 0 )

                Bass.BASS_FXSetParameters(fx[i], eq);
            }

            // громкость для потока
            MainStream.v_stream_volume = (float)eq_volume / pbc_equal_main.Properties.Maximum;
            Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_VOL, MainStream.v_stream_volume);                        

            // Баланс для потока                        
            int max = trackBar_balance.Properties.Maximum;
            float real_balance = (float)(eq_balance - (max / 2)) / (max / 2);
            Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_PAN, real_balance);            

            // временно - тональность потока
            //trackBar_Pitch.Value = trackBar_Pitch.Properties.Maximum / 2;
            float value = float.Parse(textEdit_Tone.Text) + float.Parse(textEdit_ToneTune.Text);
            if (value >= -60 && value <= 60)
            {
                Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, value);
            }

            // темп
            value = float.Parse(textEdit_Temp.Text);
            if (value >= -95 && value <= 5000)
            {
                Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_TEMPO, value);
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

            // громкость
            Row[4] = eq_volume / 10;

            // баланс
            Row[5] = eq_balance;
        }

        // строка в массив (из файла)
        public void Eq_row_to_array()
        {
            if (Row == null) return; 

            string eq_line = Row[2].ToString();
            string eq_preamp = Row[3].ToString();            

            // преамп + эквалайзеры
            string[] EQfGain_str = (eq_preamp + ':' + eq_line).Split(':');

            for (int i = 0; i < EQfGain.Length; i++)
            {
                EQfGain[i] = Convert.ToInt32(EQfGain_str[i]) * (-1);
            }

            //5|10
            // громкость трека
            if (Row[4].ToString() != "") eq_volume =  int.Parse(Row[4].ToString()) * 10;
            else eq_volume = 50; // середина

            // баланс трека
            if (Row[5].ToString() != "") eq_balance = int.Parse(Row[5].ToString());
            else eq_balance = 10; // середина            
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

            // громкость
            eq_volume = pbc_equal_main.Position;

            // баланс
            eq_balance = trackBar_balance.Value;

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
            if (fl_eqLoading) return;

            int pos = (int)((ProgressBarControl)sender).Position;
            int max = ((ProgressBarControl)sender).Properties.Maximum;
            eq_volume = pos;

            MainStream.v_stream_volume = (float)pos / max;
            Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_VOL, (float)pos / max);

            if (Row != null && Row[4].ToString() != (pos / max * 10).ToString())
            {
                btn_eq_save.Enabled = true;
                btn_eq_cancel.Enabled = true;                                
            }
        }

        private void radioGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            pbc_eq_dob.Position = (int)EQBandwidth[radioGroup.SelectedIndex + 1];
            toolTipController.ShowHint("[" + radioGroup.Properties.Items[radioGroup.SelectedIndex].Description + "]");

            lc_dobr.Text = "Добр. " + EQCenter[radioGroup.SelectedIndex + 1] + " [" + EQBandwidth[radioGroup.SelectedIndex + 1] + "pt]";            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {            
            XtraMessageBox.Show(
                  "BASSVERSION = " + Bass.BASSVERSION +
                  "\nBASS_GetVersion = " + Bass.BASS_GetVersion() + " HiWord: " + HiWord(Bass.BASS_GetVersion()) +
                  "\nBASS_FX_GetVersion = " + Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_GetVersion() + " HiWord: " + HiWord(BassFx.BASS_FX_GetVersion())
                , "Версия Bass.dll + Bass_FX.dll", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //v_stream = BassFx.BASS_FX_TempoCreate(v_stream, BASSFlag.BASS_SAMPLE_LOOP | BASSFlag.BASS_FX_FREESOURCE);

            /*
            {
                MESS(ID_OPEN, WM_SETTEXT, 0, "click here to open a file && play it...");
                Error("Couldn't create a resampled stream!");
                BASS_StreamFree(chan);
                BASS_MusicFree(chan);
                break;
            }*/

        }

        public bool Check_BASS_version()
        {
            if (HiWord(Bass.BASS_GetVersion()) != Bass.BASSVERSION)
            {
                XtraMessageBox.Show("Загружена некорректная версия BASS.DLL (требуется версия 2.4)", "Ошибка BASS.dll", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // check the correct BASS_FX was loaded
            if (HiWord(BassFx.BASS_FX_GetVersion()) != Bass.BASSVERSION)
            {
                XtraMessageBox.Show("Загружена некорректная версия BASS_FX.DLL (требуется версия 2.4)", "Ошибка BASS_FX.dll", MessageBoxButtons.OK, MessageBoxIcon.Error);                
                return false;
            }

            return true;
        }

        // Трек бары - наведение мыши
        private void trackBar_MouseMove(object sender, MouseEventArgs e)
        {
            int bar_val = ((TrackBarControl)sender).Value - (((TrackBarControl)sender).Properties.Maximum / 2);
            toolTipController.ShowHint(((TrackBarControl)sender).Tag.ToString() + " [" + bar_val + "]");

            ((TrackBarControl)sender).Focus();
        }

        // Баланс - изменено значение
        private void trackBar_Balance_EditValueChanged(object sender, EventArgs e)
        {
            if (fl_eqLoading) return;
            if (fl_scroll_Loading) return;

            int pos = trackBar_balance.Value;
            int max = trackBar_balance.Properties.Maximum;
            eq_balance = pos;

            float real_balance = (float)(pos - (max / 2)) / (max / 2);            
            Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_PAN, real_balance);

            if (Row != null && Row[5].ToString() != (pos).ToString())
            {
                btn_eq_save.Enabled = true;
                btn_eq_cancel.Enabled = true;                
            }

            toolTipController.ShowHint("Баланс [" + (pos - (max / 2)) + "]");
        }        

        // Трек бары - скролл мышкой
        private void track_bar_MouseWheel(object sender, MouseEventArgs e)
        {
            
            int max = ((TrackBarControl)sender).Properties.Maximum;
            int min = ((TrackBarControl)sender).Properties.Minimum;
            int pos = ((TrackBarControl)sender).Value;

            fl_scroll_Loading = true;
            
            if (e.Delta < 0)
                ((TrackBarControl)sender).Value += 2;
            else
                ((TrackBarControl)sender).Value -= 2;
            
            fl_scroll_Loading = false;

            toolTipController.ShowHint(((TrackBarControl)sender).Tag.ToString() + " [" + (pos - (max / 2)) + "]");
        }

        // Тональность - изменение значения
        private void sbt_Tone_Edit_Click(object sender, EventArgs e)
        {
            double tone_value = double.Parse(textEdit_Tone.Text);
            double tune_value = double.Parse(textEdit_ToneTune.Text);
            double tone_delta = double.Parse(((SimpleButton)sender).Tag.ToString());            

            tone_value += tone_delta;
            if (tone_value < -60) tone_value = -60;
            if (tone_value > 60) tone_value = 60; // ограничение допустимого значения     

            textEdit_Tone.Text = tone_value.ToString();

            Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, (float)(tone_value + tune_value/100));
            toolTipController.ShowHint("Тональность [" + (tone_value) + "]");
        }

        // Подстройка - изменение значения
        private void sbt_ToneTune_Edit_Click(object sender, EventArgs e)
        {
            double tone_value = double.Parse(textEdit_ToneTune.Text);
            double tune_value = double.Parse(textEdit_Tone.Text);
            double tone_delta = double.Parse(((SimpleButton)sender).Tag.ToString());

            tone_value += tone_delta;
            if (tone_value < -99 || tone_value > 99) return; // ограничение допустимого значения 
            textEdit_ToneTune.Text = tone_value.ToString();

            Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, (float)(tune_value + tone_value / 100));
            toolTipController.ShowHint("Подстройка [" + (tone_value) + "]");
        }

        // Изменение темпа
        private void sbt_Temp_EditClick(object sender, EventArgs e)
        {
            double temp_value = double.Parse(textEdit_Temp.Text);
            double temp_delta = double.Parse(((SimpleButton)sender).Tag.ToString());

            temp_value += temp_delta * 1;
            if (temp_value < -95 || temp_value > 5000) return;

            textEdit_Temp.Text = temp_value.ToString();

            Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_TEMPO, (float)temp_value);
            toolTipController.ShowHint("Темп [" + (temp_value) + "%]");
        }

        // Скрол на текстовом поле
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
                case "Tone":
                    if (value < -60 || value > 60) return; // ограничение допустимого значения   
                    double tune_value = double.Parse(textEdit_ToneTune.Text);  
                    Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, (float)(value + tune_value/100));
                    toolTipController.ShowHint("Транспозер [" + (value) + "]");
                    break;
                case "ToneMini":
                    if (value < -99 || value > 99) return; // ограничение допустимого значения                       
                    double tone_value = double.Parse(textEdit_Tone.Text);
                    Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, (float)(tone_value + (float)value / 100));
                    toolTipController.ShowHint("Подстройка [" + (value) + "]");
                    break;
                case "Temp":
                    if (value < -95 || value > 5000) return;                    
                    Bass.BASS_ChannelSetAttribute(v_stream, BASSAttribute.BASS_ATTRIB_TEMPO, (float)value);
                    toolTipController.ShowHint("Темп [" + (value) + "%]");
                    break;
            }

            ((TextEdit)sender).Text = value.ToString();
        }

        // Убираем мышку с объекта
        private void obj_MouseLeave(object sender, EventArgs e)
        {
            toolTipController.HideHint();
            panelControl_EQ.Focus();
        }

        // Наведение мышки на элемент
        private void obj_MouseEnter(object sender, EventArgs e)
        {
            ((TextEdit)sender).Focus();
        }

        // Сохранение настроек
        private void btn_eq_save_Click(object sender, EventArgs e)
        {
            Eq_array_to_row();
            btn_eq_save.Enabled = false;
            btn_eq_cancel.Enabled = false;
        }

        private void btn_eq_cancel_Click(object sender, EventArgs e)
        {
            Eq_row_to_array();
            Eq_array_to_object();
            Eq_array_to_stream();

            btn_eq_save.Enabled = false;
            btn_eq_cancel.Enabled = false;
        }

        private void btn_eq_on_off_DoubleClick(object sender, EventArgs e)
        {
            btn_eq_clear_Click(null, null);
        }

    }
}
