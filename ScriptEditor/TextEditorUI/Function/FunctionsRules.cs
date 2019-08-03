using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ScriptEditor.CodeTranslation;

namespace ScriptEditor.TextEditorUI.Function {

    public partial class FunctionsRules : Form
    {
        public FunctionsRules()
        {
            InitializeComponent();

            if (Settings.hintsLang != 0) label.Text = label.Tag.ToString();

            foreach (var item in DialogFunctionsRules.opcodeTemplates)
            {
                var v = item.Value;
                if (v.isDefault) continue;

                dgvTemplates.Rows.Add(v.opcode.ToString(), v.opcodeName, v.totalArgs, v.msgArg + 1, v.nodeArg + 1, v.iqArg + 1, v.msgFileArg + 1);
                dgvTemplates.Rows[dgvTemplates.Rows.Count - 2].Tag = item;
            }
        }

        private void ChangeToTemplate(DataGridViewRow row, string name) {
            KeyValuePair<string, OpcodeTemplate> pair = (KeyValuePair<string, OpcodeTemplate>)row.Tag;

            string key = pair.Key;
            if (key != name) {
                KeyValuePair<string, OpcodeTemplate> p = new KeyValuePair<string, OpcodeTemplate>(name, pair.Value);
                pair = p;
                DialogFunctionsRules.opcodeTemplates.Remove(key);
                DialogFunctionsRules.opcodeTemplates.Add(p.Key, p.Value);
            }
            pair.Value.opcode = DialogueParser.GetOpcodeType(row.Cells[0].Value.ToString());
            pair.Value.opcodeName = row.Cells[1].Value.ToString();
            pair.Value.totalArgs = (int)row.Cells[2].Value;
            pair.Value.msgArg = (int)row.Cells[3].Value - 1;
            pair.Value.nodeArg = (int)row.Cells[4].Value - 1;
            pair.Value.iqArg = (int)row.Cells[5].Value - 1;
            pair.Value.msgFileArg = (int)row.Cells[6].Value - 1;
        }

        private void AddToTemplate(DataGridViewRow row, string key) {
            OpcodeTemplate template = new OpcodeTemplate(
                    DialogueParser.GetOpcodeType(row.Cells[0].Value.ToString()),
                    row.Cells[1].Value.ToString(),
                    int.Parse(row.Cells[3].Value.ToString()),
                    int.Parse(row.Cells[6].Value.ToString()),
                    int.Parse(row.Cells[4].Value.ToString()),
                    int.Parse(row.Cells[5].Value.ToString()),
                    int.Parse(row.Cells[2].Value.ToString())
                );
            template.isDefault = false;

            if (DialogFunctionsRules.opcodeTemplates.ContainsKey(key))
                DialogFunctionsRules.opcodeTemplates[key] = template;
            else
                DialogFunctionsRules.opcodeTemplates.Add(key, template);
        }

        private void RulesDialog_FormClosing(object sender, FormClosingEventArgs e) {
            bool needSave = false;
            int last = dgvTemplates.Rows.Count -1;

            List<string> keyList = new List<string>();

            foreach (DataGridViewRow row in dgvTemplates.Rows)
            {
                if (row.Index == last) break;
                if (row.Cells[0].Value == null || row.Cells[1].Value == null) continue;

                for (int i = 2; i < row.Cells.Count; i++) {
                    if (row.Cells[i].Value == null) row.Cells[i].Value = 0;
                }

                string key = row.Cells[1].Value.ToString().ToLowerInvariant();
                keyList.Add(key);

                if (row.Tag == null) {
                    AddToTemplate(row, key);
                    needSave = true;
                    continue;
                }

                KeyValuePair<string, OpcodeTemplate> pair = (KeyValuePair<string, OpcodeTemplate>)row.Tag;
                if (row.Cells[0].Value.ToString() != pair.Value.opcode.ToString()
                    || key != pair.Key
                    || (int)row.Cells[2].Value != pair.Value.totalArgs
                    || (int)row.Cells[3].Value != pair.Value.msgArg + 1
                    || (int)row.Cells[4].Value != pair.Value.nodeArg + 1
                    || (int)row.Cells[5].Value != pair.Value.iqArg + 1
                    || (int)row.Cells[6].Value != pair.Value.msgFileArg + 1)
                {
                    ChangeToTemplate(row, key);
                    needSave = true;
                }
            }
            // remove
            List<string> removeList = new List<string>();
            foreach (var item in DialogFunctionsRules.opcodeTemplates) {
                if (item.Value.isDefault == false && !keyList.Exists(key => (key == item.Key))) removeList.Add(item.Key);
            }
            foreach (var key in removeList) {
                DialogFunctionsRules.opcodeTemplates.Remove(key);
            }
            // save to file
            if (needSave || removeList.Count > 0) {
                DialogFunctionsRules.SaveTemplates();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            if (dgvTemplates.SelectedRows.Count == 0 || dgvTemplates.CurrentRow.Index == dgvTemplates.Rows.Count - 1) return;
            dgvTemplates.Rows.Remove(dgvTemplates.CurrentRow);

        }
    }
}
