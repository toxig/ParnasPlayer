using System;
using System.IO;

namespace PPlayer
{
    public class Loader_FileList
    {
        public string[] Data;
        public string ResaultMsg;
        public string[] FilesList = null;

        // Кодировка по умолчанию Windiws 1251
        public bool Load_FileData(string MyFile)
        {
            return Load_FileData(MyFile, System.Text.Encoding.GetEncoding(1251));
        }

        // Альтернативная кодировка файла
        public bool Load_FileData(string MyFile, System.Text.Encoding CodePage)
        {
            if (!File.Exists(MyFile))
            {
                ResaultMsg = "Файл не найден";
                return false;
            }
            else
            {
                Data = File.ReadAllLines(MyFile, CodePage);
                ResaultMsg = "Файл " + Path.GetFileName(MyFile) + " загружен (" + Data.Length.ToString() + " записей)";                                         
            }
            return true;
        }

        private string[] ArrayConcat(string[] arr1, string[] arr2)
        {
            string[] result = new string[arr1.Length + arr2.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                result[i] = arr1[i];
            }
            for (int i = arr1.Length; i < arr1.Length + arr2.Length; i++)
            {
                result[i] = arr2[i - arr1.Length];
            }
            return result;
        }

        public string[] GetAllFilesFromDirectory(string path, string mask)
        {                           
            if (FilesList == null)
            {
                FilesList = new string[0];
            }
            FilesList = ArrayConcat(FilesList, Directory.GetFiles(path, mask));

            string[] temp = Directory.GetDirectories(path);
            foreach (string s in temp)
            {
                GetAllFilesFromDirectory(s, mask);
            }
            return FilesList;
        }

    }
}
