using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ScriptEditor.TextEditorUI;

namespace ScriptEditor
{
    partial class MessageEditor : Form
    {
        private List<string> linesMsg;
        private string msgPath;
        private bool returnLine;
        private bool allow;
        private bool editMode;

        private const char pcMarker = (char)0x25CF;
        private Color pcColor = Color.FromArgb(0, 0, 220);

        private Encoding enc = (Settings.encoding == (byte)EncodingType.OEM866) ? Encoding.GetEncoding("cp866") : Encoding.Default;

        private TabInfo associateTab;
        private TextEditor scrptEditor;

        private cell SelectLine = new cell();

        private struct cell
        {
            public int row;
            public int col;
        }
       
        private class Entry
        {
            public string msgLine = string.Empty;
            public string msglip = string.Empty;
            public string desc = string.Empty;
            public string debris = string.Empty;

            public bool pcMark = false;
            
            public Entry() { }

            public Entry(string line)
            {
                if (line.TrimStart().StartsWith("#")) {
                    msgLine = "-";
                    desc = line;
                } else {
                    string[] splitLine = line.Split(new char[] {'}'}, StringSplitOptions.RemoveEmptyEntries);
                    if (splitLine.Length < 3) {
                        if (line.Length == 0) return;
                        msgLine = "^";
                        desc = line.TrimEnd('}');
                    } else {
                        string mark = String.Empty;
                        if (splitLine[0].StartsWith("\t")) {
                            mark = pcMarker + " ";
                            pcMark = true;
                        }
                        msgLine = splitLine[0].TrimStart(' ', '\t', '{'); //номер строки
                        msglip = splitLine[1].TrimStart(' ', '{');
                        desc = mark + splitLine[2].TrimStart('{');
                        if (splitLine.Length > 3)
                            debris = splitLine[3];
                    }
                }
            }

            public string ToString(out bool prev)
            {
                prev = false;
                int result;
                if (int.TryParse(msgLine, out result)) {
                    string text, tab = String.Empty;
                    if (desc.TrimStart().StartsWith(Convert.ToString(pcMarker))) {
                        text = desc.TrimStart(pcMarker, ' ');
                        tab = "\t";
                    } else {
                        text = desc;
                        pcMark = false;
                    }
                    return (tab + "{" + (msgLine) + "}{" + msglip + "}{" + text + "}" + debris);
                }
                else if (msgLine == "^") {
                    prev = true;
                    return (desc + "}" + debris);
                }
                else return (desc + debris);  
            }
        }
     
        private void AddRow(Entry e)
        {
            dgvMessage.Rows.Add(e, e.msgLine, e.desc, e.msglip);
            if (e.pcMark) 
                dgvMessage.Rows[dgvMessage.Rows.Count - 1].Cells[2].Style.ForeColor = pcColor;
        }

        private void InsertRow(int i, Entry e)
        {
            if (i >= dgvMessage.Rows.Count) {
                SelectLine.row = dgvMessage.Rows.Count;
                AddRow(e);
            }
            else dgvMessage.Rows.Insert(i, e, e.msgLine, e.desc, e.msglip);
        }

        public static void MessageEditorInit(string msgPath, int line)
        {
            MessageEditor msgEdit = new MessageEditor(msgPath, null);

            for (int i = 0; i < msgEdit.dgvMessage.RowCount; i++)
            {
                int number;
                if (int.TryParse(msgEdit.dgvMessage.Rows[i].Cells[1].Value.ToString(), out number))
                    if (number == line) {
                        msgEdit.dgvMessage.Rows[i].Cells[2].Selected = true;
                        msgEdit.dgvMessage.FirstDisplayedScrollingRowIndex = i;
                        break;
                    }
            }
            msgEdit.ShowDialog();
        }

        public static void MessageEditorInit(TabInfo ti, Form frm)
        {
            string msgPath = null;
            if (ti != null) {
                if (!MessageFile.Assossciate(ti, true, out msgPath)) return;
            }
            // Show form
            MessageEditor msgEdit = new MessageEditor(msgPath, ti);
            msgEdit.Owner = frm;
            msgEdit.scrptEditor = frm as TextEditor;
            msgEdit.Show();
            //if (Settings.autoOpenMsgs && msgEdit.scrptEditor.msgAutoOpenEditorStripMenuItem.Checked)
            //    msgEdit.WindowState = FormWindowState.Minimized;
        }

        public static void MessageEditorOpen(string msgPath, Form frm)
        {
            if (msgPath == null)
                MessageBox.Show("No output path selected.", "Error");
            // Show form
            MessageEditor msgEdit = new MessageEditor(msgPath, null);
            msgEdit.scrptEditor = frm as TextEditor;
            msgEdit.WindowState = FormWindowState.Maximized;
            frm.TopMost = false;
            msgEdit.Show();
        }

        public MessageEditor(string msgfile) : this (msgfile, null)
        {
            SendStripButton.Enabled = false;
        }

        private MessageEditor(string msg, TabInfo ti)
        {
            InitializeComponent();
            if (Settings.encoding == (byte)EncodingType.OEM866) encodingTextDOSToolStripMenuItem.Checked = true;
            StripComboBox.SelectedIndex = 2;
            if (!Settings.msgLipColumn) {
                dgvMessage.Columns[3].Visible = false;
                showLIPColumnToolStripMenuItem.Checked = false;
            }
            UpdateRecentList();
            msgPath = msg;
            if (msgPath != null){
                readMsgFile();
            } else {
                this.Text = "Empty" + this.Tag;
                AddRow(new Entry());
                linesMsg = new List<string>();
            }
            associateTab = ti;
            if (associateTab != null) MessageFile.ParseMessages(associateTab, linesMsg.ToArray());
        }

        private void readMsgFile()
        {
            linesMsg = new List<string>(File.ReadAllLines(msgPath, enc));
            dgvMessage.Visible = false;
            for (int i = 0; i < linesMsg.Count; i++)
            {
                AddRow(new Entry(linesMsg[i]));
            }
            dgvMessage.Visible = true;

            this.Text = Path.GetFileName(msgPath) + this.Tag;
            groupBox.Text = msgPath;
            msgSaveButton.Enabled = false;
        }

        private void saveFileMsg()
        {
            bool prevLine;
            linesMsg.Clear();
            for (int i = 0; i < dgvMessage.Rows.Count; i++)
            {
                Entry entries = (Entry)dgvMessage.Rows[i].Cells[0].Value;
                linesMsg.Add(entries.ToString(out prevLine));
                if (prevLine) linesMsg[i - 1] = linesMsg[i - 1].TrimEnd('}');
                foreach (DataGridViewCell cells in dgvMessage.Rows[i].Cells)
                {
                    switch (cells.ColumnIndex) {
                        case 1:
                            cells.Style.ForeColor = SystemColors.HotTrack;
                            break;
                        case 3:
                            cells.Style.ForeColor = Color.Gray;
                            break;
                        default:
                            if (entries.pcMark)
                                cells.Style.ForeColor = pcColor;
                            else
                                cells.Style.ForeColor = Color.Black;
                            break;
                    }
                }
            }
            File.WriteAllLines(msgPath, linesMsg.ToArray(), enc);
            msgSaveButton.Enabled = false;
        }

        private void dgvMessage_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1 || returnLine) return;
            DataGridViewCell cell = dgvMessage.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Entry entry = (Entry)dgvMessage.Rows[e.RowIndex].Cells[0].Value;
            string val = (string)cell.Value;
            if (val == null) val = string.Empty;
            switch (e.ColumnIndex) {
                case 1: // line
                    int result;
                    if (!int.TryParse(val, out result)) {
                        MessageBox.Show("Line must contain only numbers.", "Line Error");
                        returnLine = true;
                        cell.Value = entry.msgLine;
                    } else entry.msgLine = val;
                    break;
                case 2: // desc
                    if (val.IndexOfAny(new char[] { '{', '}' }) != -1) {
                        returnLine = true;
                        cell.Value = entry.desc;
                    } else entry.desc = val;
                    break;
                case 3: // lipfile
                    if (val.IndexOfAny(new char[] { '{', '}' }) != -1) {
                        returnLine = true;
                        cell.Value = entry.msglip;
                    } else entry.msglip = val;
                    break;
            }
            if (!returnLine) {
                dgvMessage.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Red; 
                msgSaveButton.Enabled = true;
            }
            returnLine = false;
        }

        private void dgvMessage_SelectionChanged(object sender, EventArgs e)
        {
            if (allow) {
                SelectLine.row = (dgvMessage.CurrentRow == null) ? 0 : dgvMessage.CurrentRow.Index;
                SelectLine.col = (dgvMessage.CurrentRow == null) ? 0 : dgvMessage.CurrentCell.ColumnIndex;
            } else
                allow = true;
        }

        private void SendStripButton_Click(object sender, EventArgs e)
        {
           string line = (string)dgvMessage.Rows[SelectLine.row].Cells[1].Value;
           int result;
           if (int.TryParse(line, out result)) scrptEditor.AcceptMsgLine(line);
        }

        private void NewStripButton_Click(object sender, EventArgs e)
        {
            dgvMessage.Rows.Clear();
            AddRow(new Entry("# Look Name"));
            AddRow(new Entry("{100}{}{}"));
            AddRow(new Entry("# Description"));
            AddRow(new Entry("{101}{}{}"));
            this.Text = "unsaved.msg" + this.Tag;
            this.groupBox.Text = "Messages";
            linesMsg = new List<string>();
            msgPath = null;
            msgSaveButton.Enabled = true;
        }

        private void msgOpenButton_ButtonClick(object sender, EventArgs e)
        {
            string path = msgPath;
            if (path == null && Settings.outputDir != null) path = Path.Combine(Settings.outputDir, MessageFile.MessageTextSubPath);
            openFileDialog.InitialDirectory = path;
            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                msgPath = openFileDialog.FileName;
                dgvMessage.Rows.Clear();
                readMsgFile();
                if (Path.IsPathRooted(msgPath)) {
                    Settings.AddMsgRecentFile(msgPath);
                    UpdateRecentList();
                }
            }
        }
      
        private void msgSaveButton_ButtonClick(object sender, EventArgs e)
        {
            if (msgPath == null) SaveAsStripButton_Click(null, null);
            else saveFileMsg();
        }

        private void SaveAsStripButton_Click(object sender, EventArgs e)
        {
            string path = msgPath;
            if (path == null && Settings.outputDir != null) path = Path.Combine(Settings.outputDir, MessageFile.MessageTextSubPath);
            saveFileDialog.InitialDirectory = path;
            saveFileDialog.FileName = Path.GetFileName(msgPath);
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                msgPath = saveFileDialog.FileName;
                saveFileMsg();
                this.Text = Path.GetFileName(msgPath) + this.Tag;
                groupBox.Text = msgPath;
                Settings.AddMsgRecentFile(msgPath);
                UpdateRecentList();
            }
        }

        private void IncAddStripButton_Click(object sender, EventArgs e)
        {
            int Line = 0, nLine;
            bool _comm = false;
            for (int n = SelectLine.row; n >= 0; n--)
            {
                if (int.TryParse((string)dgvMessage.Rows[n].Cells[1].Value, out Line)) break;
                string val =(string)dgvMessage.Rows[n].Cells[2].Value;
                if (/*val != null &&*/ val.StartsWith("#")) _comm = true; 
            }
            if (_comm) {
                Line = (int)Math.Round((decimal)Line / 10) * 10;
                Line += Convert.ToInt32(StripComboBox.Text);
            } else Line++;
            for (int n = 0; n < dgvMessage.Rows.Count; n++)
            {
                if (int.TryParse((string)dgvMessage.Rows[n].Cells[1].Value, out nLine)) {
                    if (Line == nLine) {
                        System.Media.SystemSounds.Question.Play();
                        return;
                    }
                }
            }
            if ((string)dgvMessage.Rows[SelectLine.row].Cells[1].Value != string.Empty) SelectLine.row++;
            InsertRow(SelectLine.row, new Entry("{" + Line + "}{}{}"));
            msgSaveButton.Enabled = true;
            allow = false;
            try { dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Selected = true; }
            catch { };
        }

        private void InsertEmptyStripButton_Click(object sender, EventArgs e)
        {
            InsertRow(++SelectLine.row, new Entry(""));
            allow = false;
            try { dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Selected = true; }
            catch { };
            msgSaveButton.Enabled = true;
        }

        private void DeleteLineStripButton_Click(object sender, EventArgs e)
        {
            if (dgvMessage.Rows.Count <= 1) return;

            DataGridViewSelectedRowCollection selRows = dgvMessage.SelectedRows;
            if (selRows.Count > 0) {
                foreach (DataGridViewRow row in selRows)
                    dgvMessage.Rows.Remove(row);
                if (dgvMessage.RowCount ==0) AddRow(new Entry());
            } else {
                dgvMessage.Rows.RemoveAt(SelectLine.row);
                if (SelectLine.row >= dgvMessage.Rows.Count) SelectLine.row--;
            }
            msgSaveButton.Enabled = true;
        }

        private void InsertCommentStripButton_Click(object sender, EventArgs e)
        {
            InsertRow(SelectLine.row, new Entry("#"));
            allow = false;
            try { dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Selected = true; }
            catch { };
            msgSaveButton.Enabled = true;
        }

        private cell Finds(int rowStart, int colStart, int rev = 1)
        {
            cell cell = new cell();
            string find_str = SearchStripTextBox.Text.Trim();
            if (find_str.Length == 0) return cell; 
            if (rev == -1 && rowStart == 0) rowStart = dgvMessage.RowCount - 1;
            for (int row = rowStart; row < dgvMessage.RowCount; row += rev)
            {
                if (row < 0) break; 
                for (int col = colStart; col < dgvMessage.ColumnCount; col++)
                {
                    if (dgvMessage.Rows[row].Cells[col].Value == null) 
                        continue;
                    string value = dgvMessage.Rows[row].Cells[col].Value.ToString();
                    if (value.IndexOf(find_str, 0, StringComparison.OrdinalIgnoreCase) != -1) {
                        cell.row = row;
                        cell.col = col;
                        break;
                    }
                }
                if (cell.col != 0) break;
                colStart = 1;
            }
            if (cell.col != 0) {
                dgvMessage.FirstDisplayedScrollingRowIndex = (cell.row <= 5) ? cell.row : cell.row - 5;
                dgvMessage.Rows[cell.row].Cells[cell.col].Selected = true;
            }
            return cell;
        }
 
        private void Downbutton_Click(object sender, EventArgs e)
        {
            cell curfind = Finds(SelectLine.row, SelectLine.col + 1);
            if (curfind.col == 0) {
                 if (SearchStripTextBox.Text.Trim().Length > 0) MessageBox.Show("Nothing found.");
            }
            else SelectLine = curfind;
        }

        private void Upbutton_Click(object sender, EventArgs e)
        {
            cell curfind = Finds(SelectLine.row, SelectLine.col + 1, -1);
            if (curfind.col == 0) {
                if (SearchStripTextBox.Text.Trim().Length > 0) MessageBox.Show("Nothing found.");
            }
            else SelectLine = curfind;
        }

        private void showLIPColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvMessage.Columns[3].Visible = showLIPColumnToolStripMenuItem.Checked;
            Settings.msgLipColumn = showLIPColumnToolStripMenuItem.Checked;
        }

        private void UpdateRecentList()
        {
            string[] items = Settings.GetMsgRecent();
            msgOpenButton.DropDownItems.Clear();
            for (int i = items.Length - 1; i >= 0; i--) {
                msgOpenButton.DropDownItems.Add(items[i], null, MsgRecentClick);
            }
        }
        
        private void MsgRecentClick(object sender, EventArgs e)
        {
            string rFile = ((ToolStripMenuItem)sender).Text;
            // Check recent file
            bool delete = false;
            if (File.Exists(rFile)) {
                msgPath = rFile;
                dgvMessage.Rows.Clear();
                readMsgFile();   
            } else if (MessageBox.Show("Message file not found.\n Delete this recent link?","Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        delete = true; // delete from recent list 
            Settings.AddMsgRecentFile(rFile, delete);
            UpdateRecentList();
        }

        private void encodingTextDOSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enc = (encodingTextDOSToolStripMenuItem.Checked) ? Encoding.GetEncoding("cp866") : Encoding.Default;
        }

        private void MessageEditor_Deactivate(object sender, EventArgs e)
        {
            if (associateTab != null) MessageFile.ParseMessages(associateTab, linesMsg.ToArray());
        }

        private void MessageEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) {
                e.Handled = true;
                Close();
            } else if (e.KeyCode == Keys.Delete && !editMode)
                DeleteLineStripButton_Click(null, null);
        }

        private void MessageEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (msgSaveButton.Enabled) {
                var result = MessageBox.Show("Do you want to save changes to message file?", "Warning", MessageBoxButtons.YesNoCancel);
                  if (result == DialogResult.Yes) {
                      msgSaveButton_ButtonClick(null, null);
                  }
                  else if (result == DialogResult.Cancel) e.Cancel = true;
            }
        }

        private void dgvMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && !dgvMessage.MultiSelect) {
                SelectLine.row--;
                InsertEmptyStripButton_Click(null, null);
            }
        }

        private void playerMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Entry entry = (Entry)dgvMessage.Rows[SelectLine.row].Cells[0].Value;
            if (entry.pcMark) {
                entry.desc = entry.desc.TrimStart(pcMarker, ' ');
                entry.pcMark = false;
            } else {
                entry.desc = pcMarker + " " + entry.desc;
                entry.pcMark = true;
            }
            dgvMessage.Rows[SelectLine.row].Cells[2].Value = entry.desc;
        }

        private void MoveToolStripButton_Click(object sender, EventArgs e)
        {
            if (MoveToolStripButton.Checked) {
                EnabledControls(false);
                dgvMessage.EditMode = DataGridViewEditMode.EditProgrammatically;
                dgvMessage.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvMessage.Cursor = Cursors.Hand;
            } else {
                EnabledControls(true);
                dgvMessage.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                dgvMessage.SelectionMode = DataGridViewSelectionMode.CellSelect;
                dgvMessage.Cursor = DefaultCursor;
            }
        }

        private void EnabledControls(bool status)
        {
            dgvMessage.MultiSelect = !status;
            SendStripButton.Enabled = status;
            IncAddStripButton.Enabled = status;
            InsertEmptyStripButton.Enabled = status;
            InsertCommentStripButton.Enabled = status;
            addToolStripMenuItem.Enabled = status;
            BackStripButton.Enabled = status;
            NextStripButton.Enabled = status;
            playerMarkerToolStripMenuItem.Enabled = status;
            sendLineToolStripMenuItem.Enabled = status;
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvMessage.MultiSelect) {
                DataGridViewSelectedRowCollection rows = dgvMessage.SelectedRows;
                int index_min = Math.Min(rows[0].Index, rows[rows.Count - 1].Index) - 1;
                int index_max = Math.Max(rows[0].Index, rows[rows.Count - 1].Index);

                if (index_min < 0) return; 

                DataGridViewRow row = dgvMessage.Rows[index_min];
                dgvMessage.Rows.RemoveAt(index_min);
                dgvMessage.Rows.Insert(index_max, row);

                msgSaveButton.Enabled = true;

            } else if (SelectLine.row > 0) {
                DataGridViewRow row = dgvMessage.Rows[--SelectLine.row];
                dgvMessage.Rows.RemoveAt(SelectLine.row);
                dgvMessage.Rows.Insert(SelectLine.row + 1, row);

                msgSaveButton.Enabled = true;
            }
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvMessage.MultiSelect) {
                DataGridViewSelectedRowCollection rows = dgvMessage.SelectedRows;
                int index_min = Math.Min(rows[0].Index, rows[rows.Count - 1].Index);
                int index_max = Math.Max(rows[0].Index, rows[rows.Count - 1].Index) + 1;

                if (index_max > dgvMessage.Rows.Count - 1) return; 

                DataGridViewRow row = dgvMessage.Rows[index_max];
                dgvMessage.Rows.RemoveAt(index_max);
                dgvMessage.Rows.Insert(index_min, row);

                msgSaveButton.Enabled = true;

            } else if (SelectLine.row < dgvMessage.Rows.Count - 1) {
                DataGridViewRow row = dgvMessage.Rows[++SelectLine.row];
                dgvMessage.Rows.RemoveAt(SelectLine.row);
                dgvMessage.Rows.Insert(SelectLine.row - 1, row);

                msgSaveButton.Enabled = true;
            }
        }

        private void dgvMessage_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            editMode = true;
        }

        private void dgvMessage_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            editMode = false;
        }
    }
}
