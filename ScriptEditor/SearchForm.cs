using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace ScriptEditor
{
    public partial class SearchForm : Form
    {
        private bool isHide = false;

        public SearchForm()
        {
            InitializeComponent();
            if (Settings.lastSearchPath == null) {
                tbSearchPath.Text = "<unset>";
            } else {
                tbSearchPath.Text = Settings.lastSearchPath;
            }
            cbFileMask.SelectedIndex = 0;

            cbCase.Checked = !Settings.searchIgnoreCase;
            cbWord.Checked = Settings.searchWholeWord;

            this.KeyUp += delegate(object a1, KeyEventArgs a2) {
                if (a2.KeyCode == Keys.Escape) this.bHide.PerformClick();
            };

            this.rbFolder.CheckedChanged += delegate(object a1, EventArgs a2) {
                this.bChange.Enabled = this.cbSearchSubfolders.Enabled = this.rbFolder.Checked;
                this.bReplace.Enabled = !this.rbFolder.Checked;
            };

            this.bChange.Click += delegate(object a1, EventArgs a2) {
                fbdSearchFolder.SelectedPath = Settings.lastSearchPath;
                if (this.fbdSearchFolder.ShowDialog() != DialogResult.OK) return;
                Settings.lastSearchPath = this.fbdSearchFolder.SelectedPath;
                this.tbSearchPath.Text = Settings.lastSearchPath;
            };

            this.tbSearch.KeyPress += delegate(object a1, KeyPressEventArgs a2) {
                if (a2.KeyChar == '\r') {
                    a2.Handled = true;
                    this.bSearch.PerformClick();
                }
            };
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
            if (!isHide && WindowState == FormWindowState.Minimized) Opacity = 0.6;
        }

        private void SearchForm_Activated(object sender, EventArgs e)
        {
            Opacity = 1;
            isHide = false;
        }

        private void bHide_Click(object sender, EventArgs e)
        {
            isHide = true;
            this.Hide();
        }

        private void rbFolder_CheckedChanged(object sender, EventArgs e)
        {
            cbFileMask.Enabled = rbFolder.Checked;
            tbSearchPath.Enabled = rbFolder.Checked;
        }

        private void cbRegular_CheckedChanged(object sender, EventArgs e)
        {
            cbWord.Enabled = !cbRegular.Checked;
        }

        private void tbSearchPath_Leave(object sender, EventArgs e)
        {
            Settings.lastSearchPath = this.tbSearchPath.Text;
        }

        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            tbSearchPath_Leave(null, null);
        }

        private void cbCase_Click(object sender, EventArgs e)
        {
            Settings.searchIgnoreCase = !cbCase.Checked;
        }

        private void cbWord_Click(object sender, EventArgs e)
        {
            Settings.searchWholeWord = cbWord.Checked;
        }
    }
}
