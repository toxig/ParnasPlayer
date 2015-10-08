using System;
using System.IO;
using System.Windows.Forms;

namespace PPlayer
{
    public class Loader_FileList
    {
        public string[] Data;
        public string ResaultMsg;
        public string[] FilesList = null;        

        // ��������� �� ��������� Windiws 1251
        public bool Load_FileData(string MyFile)
        {
            return Load_FileData(MyFile, System.Text.Encoding.GetEncoding(1251));
        }

        // �������������� ��������� �����
        public bool Load_FileData(string MyFile, System.Text.Encoding CodePage)
        {
            if (!File.Exists(MyFile))
            {
                ResaultMsg = "���� �� ������: " + MyFile;
                return false;
            }
            else
            {
                Data = File.ReadAllLines(MyFile, CodePage);
                ResaultMsg = "���� " + Path.GetFileName(MyFile) + " �������� (" + Data.Length.ToString() + " �������)";                                         
            }
            return true;
        }

        // ���������� ��������
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

        // ����� ������ � ����� � ���������
        public string[] GetFilesFromDir(string path, string mask)
        {
            FilesList = new string[0];

            return GetAllFilesFromDirectory(path, mask);
        }

        // ����� ������ � �����
        private string[] GetAllFilesFromDirectory(string path, string mask)
        {                                       
            FilesList = ArrayConcat(FilesList, Directory.GetFiles(path, mask));

            string[] temp = Directory.GetDirectories(path);
            foreach (string s in temp)
            {
                GetAllFilesFromDirectory(s, mask);
            }
            return FilesList;
        }

        // ���������� ������ � ����
        public bool Save_To_File(string FileName, string[] Data)
        {
            #region ������ � ����
            try
            {
                File.WriteAllLines(FileName, Data, System.Text.Encoding.GetEncoding(1251));                                                
            }
            catch (Exception em)
            {
                
                MessageBox.Show(em.Message + "\n\n������ ���������� ������ � ����!" +
                                             "\n������������:" +
                                             "\n  - ��������� ���� � ������ ����������" +
                                             "\n  - ��������� ����, �������� ���������� ����� �� ���� ���������"
                                             , "������", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            #endregion
            return true;
        }
    }
}
