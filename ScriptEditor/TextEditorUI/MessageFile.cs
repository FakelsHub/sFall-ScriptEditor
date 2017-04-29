using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ScriptEditor.TextEditorUI
{
    public class MessageFile
    {
        public static readonly string MessageTextSubPath = "..\\text\\" + Settings.language + "\\dialog\\";
        
        public static bool Assossciate(TabInfo tab, bool create, out string path)
        {
            bool found = true;
            string defaultDir = Path.Combine(Settings.outputDir, MessageTextSubPath);
            // primary check in output dir
            path = Path.Combine(defaultDir, Path.ChangeExtension(tab.filename, ".msg"));
            if (!File.Exists(path)) {
                found = false;
                // second check in msg list path
                for (int i = 0; i < Settings.msgListPath.Count; i++)
                { 
                    string pth = Path.Combine(Settings.msgListPath[i], Path.ChangeExtension(tab.filename, ".msg"));
                    if (File.Exists(pth)) {
                        path = pth;
                        found = true;
                        break;
                    }
                }
            }
            if (!found) {
                if (!create) return false;
                else {
                    if (!Directory.Exists(defaultDir)) {
                        MessageBox.Show("Failed to open or create associated message file in directory\n" + defaultDir, "Error: Directory does not exist");
                        return false;
                    } else if (MessageBox.Show("The associated message file this script could not be found.\nDo you want to create a new file?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                        File.WriteAllText(path, "{100}{}{}");
                    } else return false;
                }       
            }
            return true;
        }
        
        public static void ParseMessages(TabInfo ti)
        {
            ti.messages.Clear();
            char[] split = new char[] { '}' };
            for (int i = 0; i < ti.msgFileTab.textEditor.Document.TotalNumberOfLines; i++)
            {
                string[] line = ti.msgFileTab.textEditor.Document.GetText(ti.msgFileTab.textEditor.Document.GetLineSegment(i)).Split(split, StringSplitOptions.RemoveEmptyEntries);
                if (line.Length != 3)
                    continue;
                for (int j = 0; j < 3; j += 2)
                {
                    line[j] = line[j].Trim();
                    if (line[j].Length == 0 || line[j][0] != '{')
                        continue;
                    line[j] = line[j].Substring(1);
                }
                int index;
                if (!int.TryParse(line[0], out index))
                    continue;
                ti.messages[index] = line[2];
            }
        }
    }
}
