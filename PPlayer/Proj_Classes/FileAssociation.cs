﻿using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System;

namespace PPlayer
{
    public class FileAssociation
    {
        public const string FILE_EXTENSION = ".pmp";
        private const long SHCNE_ASSOCCHANGED = 0x8000000L;
        private const uint SHCNF_IDLIST = 0x0U;

        public void Associate(string description, string icon)
        {
            Registry.ClassesRoot.CreateSubKey(FILE_EXTENSION).SetValue("", Application.ProductName);

            if (Application.ProductName != null && Application.ProductName.Length > 0)
            {
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(Application.ProductName))
                {
                    if (description != null)
                        key.SetValue("", description);

                    if (icon != null)
                        key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));

                    key.CreateSubKey(@"Shell\Open\Command").SetValue("", ToShortPathName(Application.ExecutablePath) + " %1");
                }
            }

            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }

        public bool IsAssociated
        {
            get { return (Registry.ClassesRoot.OpenSubKey(FILE_EXTENSION, false) != null); }
        }

        public void Remove()
        {
            if (Registry.ClassesRoot.OpenSubKey(FILE_EXTENSION, false) != null)
                Registry.ClassesRoot.DeleteSubKeyTree(FILE_EXTENSION);
            if (Registry.ClassesRoot.OpenSubKey(Application.ProductName, false) != null) 
                Registry.ClassesRoot.DeleteSubKeyTree(Application.ProductName);
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SHChangeNotify(long wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        [DllImport("Kernel32.dll")]
        private static extern uint GetShortPathName(string lpszLongPath, [Out]StringBuilder lpszShortPath, uint cchBuffer);

        private static string ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(1000);
            uint iSize = (uint)s.Capacity;
            uint iRet = GetShortPathName(longName, s, iSize);
            return s.ToString();
        }
    }
}
