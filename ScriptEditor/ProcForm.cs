using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
    }
}
