using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using System.IO;
using System.Diagnostics;

namespace PPlayer
{
    /*internal sealed class Storage
    {
        public static string path;
    }*/

    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string UpdatePath = "http://zigzag-muz.ru/pplayer/PPlayer.application";

            #region Загрузка параметров
            /*
            try
            {
                Storage.path = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\PPlayerOptions.ini")[0];
            }
            catch (FileNotFoundException)
            {
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\PPlayerOptions.ini", string.Empty);
            }
            catch (IndexOutOfRangeException)
            {
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\PPlayerOptions.ini", string.Empty);
            }
            */
            #endregion

            #region Оформление - тема
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.UserSkins.BonusSkins.Register();
            UserLookAndFeel.Default.SetSkinStyle("Office 2010 Blue");
            //Darkroom, Sharp Plus, Sharp, High Contrast, Dark Side, press Dark Style, Office 2010 Blue

            #endregion

            #region проверка обновления
            // проверка обновления
            Form_Update uf = new Form_Update(UpdatePath);
            try
            {                
                uf.ShowDialog();

                if (uf.NeedUpdate)
                {
                    //создание параметров
                    var startInfo = new ProcessStartInfo
                    {
                        //имя файла
                        FileName = "Updater.exe"//uf.uv.v_folder_update
                        //скрытое окно
                        //WindowStyle = ProcessWindowStyle.Hidden,
                        //ваши аргументы
                        //Arguments = "-command"
                    };
                    //запуск процесса
                    Process.Start(startInfo);
                    Application.Exit();
                }
            }
            catch (Exception e)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(e.Message, "Ошибка запуска обновления", MessageBoxButtons.OK, MessageBoxIcon.Error);
                uf.NeedUpdate = false;
            } 
            #endregion

            #region Запуск основной программы
            if (!uf.NeedUpdate)
                try
                {
                    Application.Run(new Form_Main());
                }
                catch (Exception e)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(e.StackTrace, "Ошибка запуска приложения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
            #endregion
        }
    }
}
