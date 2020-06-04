using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;

namespace ScriptEditor
{
    internal enum InsertAt {
        End   = 0,
        After = 1,
        Caret = 2
    }

    public partial class ProcForm : Form
    {
        private bool proc;

        public string CheckName { get; private set; }

        public string ProcedureName
        {
            get { return tbName.Text; }
        }

        internal InsertAt PlaceAt
        {
            get {
                if (!cbCopyBodyProc.Enabled) return InsertAt.Caret;
                return (rbPasteAtEnd.Checked) ? InsertAt.End : InsertAt.After;
            }
        }

        public bool SetInsertAtArter
        {
            set { rbAfterSelProcedure.Checked = value; }
        }

        /// <summary>
        /// Получает установленное значение копировать ли тело процедуры.
        /// Включает или Выключает элемент управления.
        /// </summary>
        public bool CopyProcedure
        {
            get { return cbCopyBodyProc.Checked; }
            set { cbCopyBodyProc.Enabled = value; }
        }

        public ProcForm(string name, bool readOnly = false, bool proc = false)
        {
            InitializeComponent();

            this.proc = proc;

            if (proc && name != null)
                IncrementNumber(ref name);

            tbName.Text = name;
            tbName.ReadOnly = readOnly;
        }

        private void IncrementNumber(ref string name)
        {
            int lenName = name.Length - 1;
            if (Char.IsDigit(name[lenName])) {
                int i;
                for (i = lenName; i > 0; i--) {
                    if (!Char.IsDigit(name[i]))
                        break;
                }
                int numZero = lenName - i;
                int numb = int.Parse(name.Substring(++i));
                numb++;
                name = name.Remove(i) + numb.ToString(new string('0', numZero));
            }
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
            if (DialogResult != DialogResult.OK) return;

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
                if (!TextUtilities.IsLetterDigitOrUnderscore(ch) /*&& !(proc && (ch == '(' || ch == ')'))*/) {
                    e.Cancel = true;
                    break;
                }
            }

            if (e.Cancel)
                MessageBox.Show("Was used incorrect name.\nThe name can only contain alphanumeric characters and the underscore character.", "Incorrect name");
            else {
                // вставляем ключевые слова 'variable' для аргументов процедуры
                int z = tbName.Text.IndexOf('(');
                if (z != -1) {
                    z++;
                    string text = tbName.Text;
                    int y = text.LastIndexOf(')');
                    if (z == y) return; // no args

                    string pName = text.Substring(0, z - 1);

                    List<byte> args = new List<byte>();
                    for (byte i = (byte)z; i < y; i++) {
                        if (text[i] == ',') args.Add(i);
                    }
                    args.Add((byte)y);

                    // извлекаем имена аргументов
                    string argNames = string.Empty;
                    for (byte i = 0; i < args.Count; i++)
                    {
                        int x = args[i];
                        string argName = text.Substring(z, x - z).Trim();
                        z = x + 1;
                        if (!argName.StartsWith("variable ")) argName = argName.Insert(0, "variable ");
                        if (i > 0) argNames += ", ";
                        argNames += argName;
                    }
                    tbName.Text = string.Format("{0}({1})", pName, argNames);
                }
            }
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
