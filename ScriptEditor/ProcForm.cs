using System;
using System.Windows.Forms;

namespace ScriptEditor
{
    public partial class ProcForm : Form
    {
        public ProcForm()
        {
            InitializeComponent();
        }

        private void ProcForm_Shown(object sender, EventArgs e)
        {
            if (ProcedureName.Text == "procedure"
               || ProcedureName.Text == "begin"
               || ProcedureName.Text == "end"
               || ProcedureName.Text == "variable"
               || ProcedureName.Text.Length == 0)
            {
               ProcedureName.Text = "unnamed_proc";
            }
            
        }

        internal static bool CreateRenameForm(ref string name, string tile = "")
        {
            ProcForm RenameFrm = new ProcForm();
            RenameFrm.groupBox1.Enabled = false;
            RenameFrm.ProcedureName.Text = name;  
            RenameFrm.Text = "Rename " + tile;
            RenameFrm.Create.Text = "OK";
            if (RenameFrm.ShowDialog() == DialogResult.Cancel) {
                return false; 
            }
            name = RenameFrm.ProcedureName.Text.Trim();
            RenameFrm.Dispose();
            return true;
        }
    }
}
