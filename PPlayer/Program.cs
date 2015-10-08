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
        /// Главная точка входа для приложения (с параметрами).
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {            
            #region Загрузка параметров (откл)
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

            #region Запуск основной программы
            //Application.Run(new Form_Main(args));

            try
            {
                Application.Run(new Form_Main(args));
            }
            catch (Exception e)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(e.Message + "/r/n" + e.StackTrace, "Ошибка запуска приложения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }
    }
}
