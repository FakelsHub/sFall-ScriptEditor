using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor
{
    public delegate void SendLineHandler(string msgLine);

    partial class MessageEditor : Form
    {
        public event SendLineHandler SendMsgLine;
        
        private List<string> linesMsg;
        private string msgPath;
        private bool returnLine;
        private bool allow;

        private Color editColor;
        private bool editAllowed = true;

        private const string COMMENT = "#";
        private const string lineMarker = "\u25B2";
        private const char pcMarker = '\u25CF';
        private Color pcColor = Color.FromArgb(0, 0, 220);
        private Color cmColor;

        private Encoding enc = Settings.EncCodePage;

        private TabInfo associateTab;
        private Form scriptForm;

        internal bool closeOnSend = false;

        private cell SelectLine = new cell();

        private struct cell
        {
            public int row;
            public int col;
        }
            
        public enum CommentColor { LightYellow, PaleGreen, Lavender }
        
        #region Entry DGV 
        private class Entry
        {
            public string msgLine = string.Empty;
            public string msglip = string.Empty;
            public string msgText = string.Empty;
            public string description = string.Empty;

            public bool pcMark = false;
            public bool commentLine = false;

            public int tabCount = 1;

            public Entry() { }

            public Entry(string line)
            {
                if (line.TrimStart().StartsWith(COMMENT)) {
                    msgLine = "-";
                    msgText = line;
                    commentLine = true;
                } else {
                    string[] splitLine = line.Split(new char[] {'}'}, StringSplitOptions.RemoveEmptyEntries);
                    if (splitLine.Length < 3) {
                        if (line.Length == 0)
                            return;
                        msgLine = lineMarker;
                        msgText = line.TrimEnd('}');
                    } else {
                        string mark = String.Empty;
                        if (splitLine[0].StartsWith("\t")) {
                            tabCount = splitLine[0].Length;
                            splitLine[0] = splitLine[0].TrimStart('\t');
                            tabCount -= splitLine[0].Length;
                            mark = pcMarker + " ";
                            pcMark = true;
                        }    
                        int z = splitLine[0].IndexOf('{');
                        msgLine = splitLine[0].Substring(z + 1);     //номер строки
                        z = splitLine[1].IndexOf('{');
                        msglip = splitLine[1].Substring(z + 1);
                        z = splitLine[2].IndexOf('{');
                        msgText = mark + splitLine[2].Substring(z + 1);
                        if (splitLine.Length > 3)
                            description = splitLine[3].TrimEnd();
                    }
                }
            }

            public string ToString(out bool prev)
            {
                prev = false;
                int result;
                if (int.TryParse(msgLine, out result)) {
                    string text, tab = String.Empty;
                    if (msgText.TrimStart().StartsWith(pcMarker.ToString())) {
                        text = msgText.TrimStart(pcMarker, ' ');
                        tab = new String('\t', tabCount);
                    } else {
                        text = msgText;
                        pcMark = false;
                    }
                    return (tab + "{" + (msgLine) + "}{" + msglip + "}{" + text + "}" + description);
                }
                else if (msgLine == lineMarker) {
                    prev = true;
                    return (msgText + "}" + description);
                } else 
                    return (msgText + description);  
            }
        }
        #endregion

        private void AddRow(Entry e)
        {
            dgvMessage.Rows.Add(e, e.msgLine, e.msgText, e.msglip);
            int row = dgvMessage.Rows.Count - 1;
            dgvMessage.Rows[row].Cells[2].ToolTipText = e.description.Trim();
            if (e.pcMark) 
                dgvMessage.Rows[row].Cells[2].Style.ForeColor = pcColor;
            if (e.commentLine)
                MarkCommentLine(row);
        }

        private void InsertRow(int i, Entry e)
        {
            if (i >= dgvMessage.Rows.Count) {
                SelectLine.row = dgvMessage.Rows.Count;
                AddRow(e);
            }
            else dgvMessage.Rows.Insert(i, e, e.msgLine, e.msgText, e.msglip);
            if (e.commentLine)
                MarkCommentLine(i);
        }

        private void MarkCommentLine(int row)
        {
            if (!Settings.msgHighlightComment) 
                return;

            foreach (DataGridViewCell cell in dgvMessage.Rows[row].Cells)
                cell.Style.BackColor = cmColor;
        }

        private void HighlightingCommentUpdate()
        {
            Color clr = (Settings.msgHighlightComment) ? cmColor : dgvMessage.RowsDefaultCellStyle.BackColor;
            for (int row = 0; row < dgvMessage.Rows.Count; row++)
            {
                Entry ent = (Entry)dgvMessage.Rows[row].Cells[0].Value;
                if (ent.commentLine) {
                    for (int col = 1; col < dgvMessage.Rows[row].Cells.Count; col++)
                        dgvMessage.Rows[row].Cells[col].Style.BackColor = clr;
                }
            }
        }
                
        private void SetCommentColor()
        {
            switch ((CommentColor)Settings.msgHighlightColor)
            {
                case CommentColor.LightYellow:
                    cmColor = Color.FromArgb(255, 255, 200);
                    break;
                case CommentColor.Lavender:
                    cmColor = Color.FromArgb(240, 240, 250);
                    break;
                case CommentColor.PaleGreen:
                    cmColor = Color.FromArgb(195, 255, 195);
                    break;
            }
        }

        #region Initial form
        // call from testing dialog tools and node code editor
        public static MessageEditor MessageEditorInit(string msgPath, int line, bool sendState = false)
        {
            MessageEditor msgEdit = new MessageEditor(msgPath, null);

            for (int i = 0; i < msgEdit.dgvMessage.RowCount; i++)
            {
                int number;
                if (int.TryParse(msgEdit.dgvMessage.Rows[i].Cells[1].Value.ToString(), out number))
                    if (number == line) {
                        msgEdit.dgvMessage.Rows[i].Cells[2].Selected = true;
                        msgEdit.dgvMessage.FirstDisplayedScrollingRowIndex = (i <= 5) ? i : i - 5;
                        break;
                    }
            }
            msgEdit.SendStripButton.Enabled = sendState;
            
            return msgEdit;
        }
        
        // call from main script editor
        public static MessageEditor MessageEditorInit(TabInfo ti, Form frm)
        {
            string msgPath = null;
            if (ti != null) {
                if (!MessageFile.GetAssociatePath(ti, true, out msgPath)) 
                    return null;

                ti.msgFilePath = msgPath;
            }
            
            // Show form
            MessageEditor msgEdit = new MessageEditor(msgPath, ti);
            msgEdit.scriptForm = frm;
            if (ti != null) 
                msgEdit.alwaysOnTopToolStripMenuItem.Checked = true;
            msgEdit.Show();
            //if (Settings.autoOpenMsgs && msgEdit.scrptEditor.msgAutoOpenEditorStripMenuItem.Checked)
            //    msgEdit.WindowState = FormWindowState.Minimized;

            return msgEdit;
        }

        // for open custom message file
        public static MessageEditor MessageEditorOpen(string msgPath, Form frm)
        {
            if (msgPath == null)
                MessageBox.Show("No output path selected.", "Error");
            
            // Show form
            MessageEditor msgEdit = new MessageEditor(msgPath, null);
            msgEdit.scriptForm = frm;
            msgEdit.WindowState = FormWindowState.Maximized;
            frm.TopMost = false;
            msgEdit.Show();

            return msgEdit;
        }
        #endregion

        #region Constructor
        public MessageEditor(string msgfile) : this (msgfile, null)
        {
            SendStripButton.Enabled = false;
        }

        private MessageEditor(string msg, TabInfo ti)
        {
            InitializeComponent();

            FontSizeComboBox.SelectedIndex = Settings.msgFontSize;
            if (Settings.msgFontSize != 0)
                FontSizeChanged(null, null);
            FontSizeComboBox.SelectedIndexChanged += FontSizeChanged;

            ColorComboBox.SelectedIndex = Settings.msgHighlightColor;
            HighlightingCommToolStripMenuItem.Checked = Settings.msgHighlightComment;

            if (Settings.encoding == (byte)EncodingType.OEM866)
                encodingTextDOSToolStripMenuItem.Checked = true;
            
            StripComboBox.SelectedIndex = 2;
            if (!Settings.msgLipColumn) {
                dgvMessage.Columns[3].Visible = false;
                showLIPColumnToolStripMenuItem.Checked = false;
            }

            UpdateRecentList();

            msgPath = msg;
            if (msgPath != null)
                readMsgFile();
            else {
                this.Text = "Empty" + this.Tag;
                AddRow(new Entry());
                linesMsg = new List<string>();
            }

            associateTab = ti;
            if (associateTab != null) 
                MessageFile.ParseMessages(associateTab, linesMsg.ToArray());
        }
        #endregion

        #region Load/Save Msg File
        private void readMsgFile()
        {
            linesMsg = new List<string>(File.ReadAllLines(msgPath, enc));
            dgvMessage.Visible = false;
            this.Update();

            ProgressBarForm progress = null;
            if (linesMsg.Count > 1000)
                progress = new ProgressBarForm(this, linesMsg.Count);

            for (int i = 0; i < linesMsg.Count; i++)
            {
                AddRow(new Entry(linesMsg[i]));
                if (progress != null)
                    progress.SetProgress = i;
            }
            
            if (progress != null)
                progress.Dispose();

            dgvMessage.AutoResizeColumns();
            dgvMessage.Visible = true;
 
            this.Text = Path.GetFileName(msgPath) + this.Tag;
            groupBox.Text = msgPath;
            msgSaveButton.Enabled = false;
        }

        private void saveFileMsg()
        {
            bool prevLine;
            bool replaceX = (enc.CodePage == 866);
            dgvMessage.EndEdit();
            linesMsg.Clear();
            for (int i = 0; i < dgvMessage.Rows.Count; i++)
            {
                Entry entries = (Entry)dgvMessage.Rows[i].Cells[0].Value;
                string line = entries.ToString(out prevLine);
                
                if (replaceX) 
                    line = line.Replace('\u0425', '\u0058'); //Replacement of Russian letter "X", to English letter
                
                linesMsg.Add(line);
                
                if (prevLine) 
                    linesMsg[i - 1] = linesMsg[i - 1].TrimEnd('}');
                
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
                                cells.Style.ForeColor = dgvMessage.RowsDefaultCellStyle.ForeColor;
                            break;
                    }
                }
            }
            File.WriteAllLines(msgPath, linesMsg.ToArray(), enc);
            msgSaveButton.Enabled = false;
        }
        #endregion

        private void dgvMessage_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1 || returnLine)
                return;

            DataGridViewCell cell = dgvMessage.Rows[e.RowIndex].Cells[e.ColumnIndex];
            Entry entry = (Entry)dgvMessage.Rows[e.RowIndex].Cells[0].Value;
            
            string val = (string)cell.Value;
            if (val == null) 
                val = string.Empty;

            switch (e.ColumnIndex) {
                case 1: // line
                    int result;
                    if (!int.TryParse(val, out result)) {
                        MessageBox.Show("Line must contain only numbers.", "Line Error");
                        returnLine = true;
                        cell.Value = entry.msgLine;
                    } else
                        entry.msgLine = val;
                    break;
                case 2: // desc
                    if (val.IndexOfAny(new char[] { '{', '}' }) != -1) {
                        returnLine = true;
                        cell.Value = entry.msgText;
                    } else
                        entry.msgText = val;
                    break;
                case 3: // lipfile
                    if (val.IndexOfAny(new char[] { '{', '}' }) != -1) {
                        returnLine = true;
                        cell.Value = entry.msglip;
                    } else
                        entry.msglip = val;
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

            editAllowed = false;
        }

        private void SendStripButton_Click(object sender, EventArgs e)
        {
           if (SendMsgLine == null)
               return;
           string line = (string)dgvMessage.Rows[SelectLine.row].Cells[1].Value;
           int result;
           if (int.TryParse(line, out result)) {
               SendMsgLine(line);
               if (closeOnSend)
                   this.Close();
           }
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
            msgSaveButton.Enabled = false;
        }

        private void msgOpenButton_ButtonClick(object sender, EventArgs e)
        {
            string path = msgPath;
            if (path == null && Settings.outputDir != null) 
                path = Path.Combine(Settings.outputDir, MessageFile.MessageTextSubPath);
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
            if (msgPath == null) 
                SaveAsStripButton_Click(null, null);
            else 
                saveFileMsg();
        }

        private void SaveAsStripButton_Click(object sender, EventArgs e)
        {
            string path = msgPath;
            if (path == null && Settings.outputDir != null) 
                path = Path.Combine(Settings.outputDir, MessageFile.MessageTextSubPath);
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
            bool isEdit = dgvMessage.IsCurrentCellInEditMode;
            
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

            dgvMessage.EndEdit();

            if ((string)dgvMessage.Rows[SelectLine.row].Cells[1].Value != string.Empty) SelectLine.row++;
            InsertRow(SelectLine.row, new Entry("{" + Line + "}{}{}"));
            msgSaveButton.Enabled = true;
            allow = false;
            try { dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Selected = true; }
            catch { };
            
            if (isEdit)
                dgvMessage.BeginEdit(false);
        }

        private void InsertEmptyStripButton_Click(object sender, EventArgs e)
        {
            if (sender != null)
                SelectLine.row++;
            
            InsertRow(SelectLine.row, new Entry(""));
            allow = false;
            
            if (sender == null)
                SelectLine.row++;
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
            if (dgvMessage.IsCurrentCellInEditMode || dgvMessage.MultiSelect)
                return;

            string comment = COMMENT;
            if ((string)dgvMessage.Rows[SelectLine.row].Cells[1].Value == String.Empty) {
                comment += dgvMessage.Rows[SelectLine.row].Cells[2].Value;
                dgvMessage.Rows.RemoveAt(SelectLine.row);
            } else
                SelectLine.row++;
            
            InsertRow(SelectLine.row, new Entry(comment));
            allow = false;
            try { dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Selected = true; }
            catch { };
            msgSaveButton.Enabled = true;
        }
        
        #region Search function 
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
        #endregion

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
            if (msgPath != null) {
                dgvMessage.SelectionChanged -= dgvMessage_SelectionChanged;

                dgvMessage.Rows.Clear();
                readMsgFile();
                dgvMessage.FirstDisplayedScrollingRowIndex = (SelectLine.row <= 5) ? SelectLine.row : SelectLine.row - 5;
                if (SelectLine.row > 0 && SelectLine.col > 0)
                    dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].Selected = true;

                dgvMessage.SelectionChanged += dgvMessage_SelectionChanged;
            }
        }

        private void MessageEditor_Deactivate(object sender, EventArgs e)
        {
            if (associateTab != null)
                MessageFile.ParseMessages(associateTab, linesMsg.ToArray());
        }

        private void MessageEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) {
                e.Handled = true;
                Close();
            } else if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control
                       && !dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col].IsInEditMode)
                DeleteLineStripButton_Click(sender, e);
            else if (e.Control && e.KeyCode == Keys.Subtract) {
                if (FontSizeComboBox.SelectedIndex == 0)
                    return;
                FontSizeComboBox.SelectedIndex--;
            }
            else if (e.Control && e.KeyCode == Keys.Add) {
                if (FontSizeComboBox.SelectedIndex == FontSizeComboBox.Items.Count - 1)
                    return;
                FontSizeComboBox.SelectedIndex++;
            }
        }

        private void MessageEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (msgSaveButton.Enabled) {
                var result = MessageBox.Show("Do you want to save changes to message file?", "Warning", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) {
                    msgSaveButton_ButtonClick(null, null);
                }
                else if (result == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private void dgvMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar)) {
                if (SelectLine.col == 1)
                    dgvMessage.BeginEdit(true);
                else
                    dgvMessage.BeginEdit(false);
            } else if (e.KeyChar == 8 && !dgvMessage.MultiSelect) { //Backspace
                if (SelectLine.col == 1)
                    dgvMessage.BeginEdit(false);
                else {
                    BackspaceEdit();
                }
            }
        }

        private void BackspaceEdit()
        {
            var _cell = dgvMessage.Rows[SelectLine.row].Cells[SelectLine.col];
            var data = _cell.Value;
            returnLine = true;
            _cell.Value = "";
            dgvMessage.BeginEdit(false);
            _cell.Value = data;
            returnLine = false;
        }

        private void dgvMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Control && !dgvMessage.MultiSelect) {
                InsertEmptyStripButton_Click(sender, EventArgs.Empty);
                e.SuppressKeyPress = true;
            } else if (e.KeyCode == Keys.Enter && e.Shift && !dgvMessage.MultiSelect) {
                InsertEmptyStripButton_Click(null, EventArgs.Empty);
                e.SuppressKeyPress = true;
            } else if (e.KeyCode == Keys.Enter && !dgvMessage.MultiSelect) {
                IncAddStripButton_Click(null, EventArgs.Empty);
                e.SuppressKeyPress = true;
            } else if (e.KeyCode == Keys.Delete && !e.Control && !dgvMessage.MultiSelect) {
                BackspaceEdit();
            }
        }
 
        private void playerMarkerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Entry entry = (Entry)dgvMessage.Rows[SelectLine.row].Cells[0].Value;
            if (entry.msgLine == "-")
                return;

            dgvMessage.CellValueChanged -= dgvMessage_CellValueChanged;

            bool changed = (dgvMessage.Rows[SelectLine.row].Cells[2].Style.ForeColor == Color.Red);

            if (entry.pcMark) {
                entry.msgText = entry.msgText.TrimStart(pcMarker, ' ');
                entry.pcMark = false;
                if (!changed)
                    dgvMessage.Rows[SelectLine.row].Cells[2].Style.ForeColor = dgvMessage.RowsDefaultCellStyle.ForeColor;
            } else {
                entry.msgText = pcMarker + " " + entry.msgText;
                entry.pcMark = true;
                if (!changed)
                    dgvMessage.Rows[SelectLine.row].Cells[2].Style.ForeColor = pcColor;
            }

            dgvMessage.Rows[SelectLine.row].Cells[2].Value = entry.msgText;
            dgvMessage.CellValueChanged += dgvMessage_CellValueChanged;

            msgSaveButton.Enabled = true;
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
            addDescriptionToolStripMenuItem.Enabled = status;
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
            var _cell = dgvMessage.Rows[e.RowIndex].Cells[e.ColumnIndex];
            editColor = _cell.Style.BackColor;
            _cell.Style.BackColor = Color.Beige;
        }

        private void dgvMessage_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dgvMessage.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = editColor;
        }

        private void alwaysOnTopToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (alwaysOnTopToolStripMenuItem.Checked)
                this.Owner = scriptForm; //scrptEditor;
            else
                this.Owner = null;
        }

        private void FontSizeChanged(object sender, EventArgs e)
        {
            int size = int.Parse(FontSizeComboBox.Text);
            dgvMessage.Columns[2].DefaultCellStyle.Font = new Font(dgvMessage.Columns[2].DefaultCellStyle.Font.Name, size, FontStyle.Regular);

            if (size > 10)
                size = (int)(size / 1.5f);
            else
                size--;
            dgvMessage.Columns[1].DefaultCellStyle.Font = new Font(dgvMessage.Columns[1].DefaultCellStyle.Font.Name, size, FontStyle.Bold);

            dgvMessage.AutoResizeColumns();
            dgvMessage.AutoResizeRows();

            Settings.msgFontSize = (byte)FontSizeComboBox.SelectedIndex;
        }

        private void HighlightingCheck(object sender, EventArgs e)
        {
            Settings.msgHighlightComment = HighlightingCommToolStripMenuItem.Checked;

            if (dgvMessage.RowCount > 1)
                HighlightingCommentUpdate();
        }

        private void ColorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvMessage.RowCount > 1)
                Settings.msgHighlightColor = (byte)ColorComboBox.SelectedIndex;

            SetCommentColor();

            if (dgvMessage.RowCount > 1)
                HighlightingCommentUpdate();
        }

        private void dgvMessage_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1 && e.Button == MouseButtons.Middle) {
                dgvMessage.Rows[e.RowIndex].Selected = true;
                SelectLine.row = e.RowIndex;
                sendLineToolStripMenuItem.PerformClick();
            }
        }

        private void dgvMessage_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
                dgvMessage.BeginEdit(true);
        }

        private void dgvMessage_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            if (editAllowed && dgvMessage.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected)
                dgvMessage.BeginEdit(false);
            
            editAllowed = true;
        }

        private void addDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Entry entry = (Entry)dgvMessage.Rows[SelectLine.row].Cells[0].Value;
            string desc = entry.description;
            if (InputBox.ShowDialog("Add/Edit Description line", ref desc, 125) == DialogResult.OK) {
                entry.description = desc;
                dgvMessage.Rows[SelectLine.row].Cells[2].ToolTipText = desc.Trim();

                msgSaveButton.Enabled = true;
            }
        }

        private void OpenNotepadtoolStripButton_Click(object sender, EventArgs e)
        {
            if (msgPath != null)
                Settings.OpenInExternalEditor(msgPath);
        }
    }
}
