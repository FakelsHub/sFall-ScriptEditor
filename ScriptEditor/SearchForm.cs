using System;
using System.Windows.Forms;

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
    }
}
