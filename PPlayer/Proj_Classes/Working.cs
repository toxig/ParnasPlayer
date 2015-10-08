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

                try
                {
                    FW_Form.param_Operation_Text = _text;
                }
                catch (ThreadAbortException abortException)
                {
                    MessageBox.Show("Ошибка обновления текста: " + (string)abortException.ExceptionState);                
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ошибка обновления текста 2: " + (string)e.Message);                
                }                
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

                try
                {
                    FW_Form.val_max = _val_max;
                }
                catch (ThreadAbortException abortException)
                {
                    MessageBox.Show("Ошибка обновления _val_max: " + (string)abortException.ExceptionState);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ошибка обновления _val_max 2: " + (string)e.Message);
                }                 
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

                try
                {
                    FW_Form.val_min = _val_cur;
                }
                catch (ThreadAbortException abortException)
                {
                    MessageBox.Show("Ошибка обновления _val_cur: " + (string)abortException.ExceptionState);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ошибка обновления _val_cur 2: " + (string)e.Message);
                }                  
            }
        }
        public int _val_cur = 0;

        /// <summary>Запуск фонового окна</summary>
        private void FW_ShowDialog()
        {
            //FW_Form.Visible = false;  
            try { FW_Form.ShowDialog(); }
            catch (ThreadAbortException)
            {
                //MessageBox.Show("Ошибка остановки процесса: " + (string)abortException.ExceptionState);                
            }
            catch (Exception)
            {
                //MessageBox.Show("Ошибка отрисовки окна: " + (string)e.Message);                
            }                        
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

            if (FW_Thread != null && FW_Thread.ThreadState != System.Threading.ThreadState.Aborted) 
                FW_Thread.Abort();
            FW_Thread = new Thread(new ThreadStart(FW_ShowDialog));
            FW_Thread.Start();
        }

        /// <summary>Остановить показ уведомлений</summary>
        public void Abort()
        {
            FW_Form.param_Form_Close = true;

            if (FW_Thread.ThreadState == System.Threading.ThreadState.Running)
            {
                FW_Thread.Abort("Остановка окна информации");

                // Wait for the thread to terminate.
                // FW_Thread.Join();
                // MessageBox.Show("New thread terminated - Main exiting.");                   

                /*while (FW_Thread.ThreadState != ThreadState.Aborted) // ждем отрисовким всех элементов - только потом останавливаем поток
                {
                    try
                    {
                        if (FW_Thread.ThreadState != ThreadState.AbortRequested) FW_Thread.Abort(); // потом отключаем окно
                    }
                    catch (ThreadStateException e)
                    {
                        MessageBox.Show("Ошибка остановки процесса: " + e.Message);
                    }

                    Thread.Sleep(200); // ждем 0,2 сек для надежности
                }     */           
            }
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
