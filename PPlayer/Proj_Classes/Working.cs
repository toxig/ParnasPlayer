using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace PPlayer
{
    public class Working
    {
        public Thread           FW_Thread; // Поток - отображение выполняющихся операций
        public Form_Working     FW_Form = new Form_Working();          // Фоновое окно операций        
                
        /// <summary>Текст выполняемой операции</summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                FW_Form.param_Operation_Text = _text;                
            }
        }
        private string _text = "";

        /// <summary>Максимальное значение прогрес бара</summary>
        public int val_max 
        { 
            get 
            {
                return _val_max;
            }

            set
            { 
                _val_max = value; 
            }
        }
        public int _val_max = 0;

        /// <summary>Текущее значение прогрес бара</summary>
        public int val_cur 
        { 
            get 
            {
                return _val_cur;
            }

            set
            {
                _val_cur = value;
            }
        }
        public int _val_cur = 0;

        /// <summary>Запуск фонового окна</summary>
        private void FW_ShowDialog()
        {
            FW_Form.Visible = false;
            try { FW_Form.ShowDialog(); }
            catch { this.Abort(); }
        }

        /// <summary>Запуск диалогового окна</summary>
        public void Start()
        {
            Start(FormStartPosition.CenterScreen);
        }

        /// <summary>Запуск диалогового окна</summary>
        private void Start(FormStartPosition pos)
        {
            FW_Form = new Form_Working();
            FW_Form.param_Operation_Text = Text;
            FW_Form.val_max = val_max;
            FW_Form.val_min = val_cur;
            FW_Form.StartPosition = pos;

            FW_Thread = new Thread(new ThreadStart(FW_ShowDialog));
            FW_Thread.Start();
        }

        /// <summary>Остановить показ уведомлений</summary>
        public void Abort()
        {
            if (FW_Thread.ThreadState == System.Threading.ThreadState.Running) FW_Thread.Abort();
        }
        

        private void Check_FW_Thread()
        {
            if (FW_Thread == null)
            {
                FW_Thread = new Thread(new ThreadStart(FW_ShowDialog));
                FW_Thread.Start();
            }
        }
        
    }
}
