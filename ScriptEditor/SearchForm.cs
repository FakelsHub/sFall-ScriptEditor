using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace ScriptEditor
{
    public partial class SearchForm : Form
    {
        public SearchForm()
        {
            InitializeComponent();
            if (Settings.lastSearchPath == null) {
                textBox1.Text = "<unset>";
            } else {
                textBox1.Text = Settings.lastSearchPath;
                fbdSearchFolder.SelectedPath = Settings.lastSearchPath;
            }
            cbFileMask.SelectedIndex = 0;
        }

        public List<string> GetFolderFiles()
        {
            List<string> files = new List<string>();
            SearchOption so = cbSearchSubfolders.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            if (cbFileMask.SelectedIndex == 0) {
                for (int i = 1; i < cbFileMask.Items.Count; i++)
                   files.AddRange(Directory.GetFiles(Settings.lastSearchPath, cbFileMask.Items[i].ToString(), so));
            } else
                files.AddRange(Directory.GetFiles(Settings.lastSearchPath, cbFileMask.Text, so));

            return files;
        }

        private void SearchForm_Deactivate(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) Opacity = 0.6;
        }

        private void SearchForm_Activated(object sender, EventArgs e)
        {
            Opacity = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void rbFolder_CheckedChanged(object sender, EventArgs e)
        {
            cbFileMask.Enabled = rbFolder.Checked;
        }

        private void cbRegular_CheckedChanged(object sender, EventArgs e)
        {
            cbWord.Enabled = !cbRegular.Checked;
        }
    }
}
