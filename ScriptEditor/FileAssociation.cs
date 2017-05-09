using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScriptEditor
{
    class FileAssociation
    {
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        private const string FILE_EXTENSION = ".ssl";
        private const int SHCNE_ASSOCCHANGED = 0x8000000;
        private const uint SHCNF_IDLIST = 0x0U;

        private static readonly string[] extAllowed = { FILE_EXTENSION, ".msg", ".h", ".int", ".ini", ".txt", ".xshd" };

        public static bool CheckFileAllow(string ext)
        {
            bool result = false;
            ext = System.IO.Path.GetExtension(ext).ToLower();
            foreach (string e in extAllowed)
            {
                if (e == ext) {
                    result = true;
                    break;
                }
            }
            if (!result) MessageBox.Show("You can not open this file type in the editor.", "Error - file is not allowed");
            return result;
        }

        public static void Associate()
        {
            string appName = "SfallScriptEditor";
            if (IsAssociated) {
                string value = Registry.ClassesRoot.CreateSubKey(FILE_EXTENSION).GetValue("").ToString();
                if (value == appName) return;
                Registry.ClassesRoot.DeleteSubKeyTree(value);
            }
            Registry.ClassesRoot.CreateSubKey(FILE_EXTENSION).SetValue("", appName);
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(appName))
            {
                key.SetValue("", "Sfall Script Editor v.4.0");
                key.SetValue("AlwaysShowExt", "");
                key.CreateSubKey("DefaultIcon").SetValue("", Settings.ResourcesFolder + "\\icon_ssl.ico");
                key.CreateSubKey("Shell").SetValue("", "OpenSSEditor");
                key.CreateSubKey(@"Shell\OpenSSEditor").SetValue("", "Open in Sfall ScriptEditor");
                key.CreateSubKey(@"Shell\OpenSSEditor\Command").SetValue("", Application.ExecutablePath + " \"%1\"");
            }
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }

        private static bool IsAssociated
        {
            get { return (Registry.ClassesRoot.OpenSubKey(FILE_EXTENSION, false) != null); }
        }
    }
}
