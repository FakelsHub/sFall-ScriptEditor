using System;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;

namespace ScriptEditor
{
    public partial class ProcForm : Form
    {
        private bool proc;
        
        public string CheckName { get; private set; }
        
        public string ProcedureName
        {
            get { return tbName.Text; }
        }
        
        public ProcForm(string name, bool readOnly = false, bool proc = false)
        {
            InitializeComponent();

            this.proc = proc;
            tbName.Text = name;
            tbName.ReadOnly = readOnly;
        }

        private void ProcForm_Shown(object sender, EventArgs e)
        {
            if (tbName.Text.IndexOf(' ') > -1
               || tbName.Text == "procedure"
               || tbName.Text == "begin"
               || tbName.Text == "end"
               || tbName.Text == "variable"
               || tbName.Text.Length == 0)
            {
               tbName.Text = "unnamed_proc()";
               tbName.Select();
               tbName.Select(0, 7);
            }  
        }
        
        private void ProcForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK)
                return;

            tbName.Text = tbName.Text.Trim();

            CheckName = tbName.Text;
            if (proc) {
                int z = tbName.Text.IndexOf('(');
                if (z > -1) 
                    CheckName = tbName.Text.Remove(z);
            }

            for (int i = 0; i < CheckName.Length; i++)
            {
                char ch = CheckName[i];
                if (!TextUtilities.IsLetterDigitOrUnderscore(ch)/*&& !(proc && (ch == '(' || ch == ')'))*/) {
                    e.Cancel = true;
                    break;
                }
            }

            if (e.Cancel)
                MessageBox.Show("Was used incorrect name.\nThe name can only contain alphanumeric characters and the underscore character.", "Incorrect name");
                        
        }

        internal static bool CreateRenameForm(ref string name, string tile = "")
        {
            ProcForm RenameFrm = new ProcForm(name);
            RenameFrm.groupBox1.Enabled = false;
            RenameFrm.Text = "Rename " + tile;
            RenameFrm.Create.Text = "OK";
            if (RenameFrm.ShowDialog() == DialogResult.Cancel) {
                return false; 
            }
            name = RenameFrm.ProcedureName.Trim();
            RenameFrm.Dispose();
            return true;
        }
    }
}
