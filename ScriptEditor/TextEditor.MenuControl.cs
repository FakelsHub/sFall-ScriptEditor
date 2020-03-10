using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor
{
    partial class TextEditor
    {
        #region Menu control events
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool p = Settings.enableParser; //save prev.state
            int f = Settings.selectFont;
            (new SettingsDialog()).ShowDialog();

            ApplySettingsTabs(f != Settings.selectFont);
            if (currentTab != null) tabControl1_Selected(null, null);

            if (Settings.enableParser != p && !Settings.enableParser) {
                parserLabel.Text = parseoff;
                foreach (TabInfo t in tabs)
                {
                    t.treeExpand.Clear();
                }
                if (currentTab != null ) {
                    if (ProcTree.Nodes.Count > 0)
                        ProcTree.Nodes[0].Expand();
                    if (tabControl3.TabPages.Count > 2 && !currentTab.parseInfo.parseData) {
                        tabControl3.TabPages.RemoveAt(1); // удалить вкладку Variables если нет данных
                    }
                }
            } else if (Settings.enableParser != p) {
                //parserLabel.Text = "Parser: Get updated parsing data...";
                //parserLabel.ForeColor = Color.Green;
                foreach (TabInfo t in tabs)
                {
                    t.treeExpand.Clear();
                    if (t.shouldParse && t.parseInfo == null || !t.parseInfo.parseData)
                        t.needsParse = true; //for next parsing
                }
                if (currentTab != null) {
                    if (ProcTree.Nodes.Count > 0) {
                        ProcTree.Nodes[0].Expand();
                        ProcTree.Nodes[1].Expand();
                    }
                    if (VarTree.Nodes.Count > 0) {
                        VarTree.Nodes[0].Expand();
                        VarTree.Nodes[1].Expand();
                    }
                    if (tabControl3.TabPages.Count < 3) {
                        CreateTabVarTree();
                    }
                }
            }
            if (Settings.pathHeadersFiles != null) Headers_toolStripSplitButton.Enabled = true;

            autoComplete.Colored = Settings.autocompleteColor;
            autoComplete.UpdateColor();

            if (Settings.enableParser) ParseScript(1);
        }

        private void ApplySettingsTabs(bool alsoFont = false)
        {
            ColorTheme.SetTheme();

            // Apply settings to all open documents
            foreach (TabInfo ct in tabs) {
                ct.textEditor.TextEditorProperties.TabIndent = Settings.tabSize;
                ct.textEditor.TextEditorProperties.IndentationSize = Settings.tabSize;
                if (!String.Equals(Path.GetExtension(ct.filename), ".msg", StringComparison.OrdinalIgnoreCase)) {
                    ct.textEditor.TextEditorProperties.ConvertTabsToSpaces = Settings.tabsToSpaces;
                    ct.textEditor.TextEditorProperties.ShowVerticalRuler = Settings.showVRuler;
                    ct.textEditor.TextEditorProperties.VerticalRulerRow = Settings.tabSize;
                    ct.textEditor.SetHighlighting(ColorTheme.HighlightingScheme);

                    if (alsoFont)
                        Settings.SetTextAreaFont(ct.textEditor);
                    //ct.textEditor.Refresh();
                    ct.textEditor.Document.ExtraWordList.UpdateColor(ct.textEditor.Document);
                } else {
                    ct.textEditor.Encoding = Settings.EncCodePage;
                    ct.textEditor.SetHighlighting(ColorTheme.IsDarkTheme ? "MessageDark" : "Message");
                }
                ct.textEditor.DarkScheme = ColorTheme.IsDarkTheme; //Установка с обновлением параметров.
            }
        }

        private void compileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (currentTab != null) {
                dgvErrors.Rows.Clear();
                string msg;
                Compile(currentTab, out msg);
                tbOutput.Text = currentTab.buildLog = msg;
            }
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) {
                for (int i = 0; i < tabs.Count; i++)
                {
                    if (tabControl1.GetTabRect(i).Contains(e.Location)) {
                        if (e.Button == MouseButtons.Middle)
                            Close(tabs[i]);
                        else if (e.Button == MouseButtons.Right) {
                            cmsTabControls.Tag = i;

                            foreach (ToolStripItem item in cmsTabControls.Items)
                                item.Visible = true;

                            cmsTabControls.Show(tabControl1, e.Location);
                        }
                        return;
                    }
                }
            }
        }

        private void tabControl2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) {
                for (int i = 3; i < tabControl2.TabPages.Count; i++)
                {
                    if (tabControl2.GetTabRect(i).Contains(e.Location)) {
                        if (e.Button == MouseButtons.Middle) {
                            int stbi = tabControl2.SelectedIndex;
                            if (stbi == i)
                                tabControl2.Hide();
                            tabControl2.TabPages.RemoveAt(i--);
                            if (stbi == i + 1) {
                                tabControl2.SelectedIndex = (stbi == tabControl2.TabCount) ? stbi - 1 : stbi;
                                tabControl2.Show();
                            }
                        } else if (e.Button == MouseButtons.Right) {
                            cmsTabControls.Tag = i ^ 0x10000000;

                            foreach (ToolStripItem item in cmsTabControls.Items)
                                item.Visible = (item.Text == "Close");

                            cmsTabControls.Show(tabControl2, e.Location);
                        }
                        return;
                    }
                }
            }
            else if (e.Button == MouseButtons.Left && minimizelogsize != 0 )
                minimizelog_button.PerformClick();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(currentTab);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open(null, OpenType.None);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdScripts.ShowDialog() == DialogResult.OK) {
                foreach (string s in ofdScripts.FileNames)
                {
                    Open(s, OpenType.File);
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs(currentTab);
        }

        private void saveAsTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null || Path.GetExtension(currentTab.filepath).ToLower() != ".ssl")
                return;

            SaveFileDialog sfdTemplate = new SaveFileDialog();
            sfdTemplate.Title = "Enter file name for script template";
            sfdTemplate.Filter = "Template file|*.ssl";
            string path = Path.Combine(Settings.ResourcesFolder, "templates");
            sfdTemplate.InitialDirectory = path;

            if (sfdTemplate.ShowDialog() == DialogResult.OK) {
                string fname = Path.GetFileName(sfdTemplate.FileName);
                File.WriteAllText(path + "\\" + fname, currentTab.textEditor.Text, System.Text.Encoding.ASCII);
            }
            sfdTemplate.Dispose();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close(tabs[tabControl1.SelectedIndex]);
        }

        private void recentItem_Click(object sender, EventArgs e)
        {
            Open(((ToolStripMenuItem)sender).Text, OpenType.File, recent: true);
        }

        private void Template_Click(object sender, EventArgs e)
        {
            Open(((ToolStripMenuItem)sender).Tag.ToString(), OpenType.File, false, true);
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                Save(tabs[i]);
                if (tabs[i].changed)
                    break;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new AboutBox()).ShowDialog();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(".\\docs\\");
        }

        private void massCompileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.outputDir == null) {
                MessageBox.Show("No output path selected.\nPlease select your scripts directory before compiling", "Error");
                return;
            }
            bool option = Settings.ignoreCompPath;
            Settings.ignoreCompPath = false;
            if (Settings.lastMassCompile != null)
                fbdMassCompile.SelectedPath = Settings.lastMassCompile;

            if (fbdMassCompile.ShowDialog() != DialogResult.OK)
                return;

            Settings.lastMassCompile = fbdMassCompile.SelectedPath;
            BatchCompiler.CompileFolder(fbdMassCompile.SelectedPath);
            Settings.ignoreCompPath = option;
        }

        private void compileAllOpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder FullMsg = new StringBuilder();
            dgvErrors.Rows.Clear();
            string msg;
            for (int i = 0; i < tabs.Count; i++) {
                //FullMsg.AppendLine("*** " + tabs[i].filename);
                Compile(tabs[i], out msg, false);
                tabs[i].buildLog = msg;
                FullMsg.AppendLine(msg);
                FullMsg.AppendLine();
            }
            tbOutput.Text = FullMsg.ToString();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab != null)
                currentActiveTextAreaCtrl.TextArea.ClipboardHandler.Cut(null, null);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab != null)
                currentActiveTextAreaCtrl.TextArea.ClipboardHandler.Copy(null, null);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab != null)
                currentActiveTextAreaCtrl.TextArea.ClipboardHandler.Paste(null, null);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab != null) {
                currentTab.textEditor.Undo();
                if (!currentDocument.UndoStack.CanUndo) {
                    currentTab.changed = false;
                    SetTabTextChange(currentTab.index);
                }
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab != null) {
                if (currentDocument.UndoStack.CanRedo) {
                    currentTab.changed = true;
                    SetTabTextChange(currentTab.index);
                }
                currentTab.textEditor.Redo();
            }
        }

        private void outlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;

            int cline = currentActiveTextAreaCtrl.Caret.Line;
            foreach (FoldMarker fm in currentDocument.FoldingManager.FoldMarker)
            {
                if (cline >= fm.StartLine && cline <= fm.EndLine)
                    continue;
                if (fm.FoldType == FoldType.MemberBody)
                    fm.IsFolded = !fm.IsFolded;
            }
            currentDocument.FoldingManager.NotifyFoldingsChanged(null);
            currentActiveTextAreaCtrl.CenterViewOn(cline, 0);
        }

        private void registerScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;

            if (currentTab.filepath == null) {
                MessageBox.Show("You cannot register an unsaved script.", "Error");
                return;
            }
            string fName = Path.GetExtension(currentTab.filename).ToLower();
            if (fName != ".ssl" && fName != ".int") {
                MessageBox.Show("You cannot register this file.", "Error");
                return;
            }
            fName = Path.ChangeExtension(currentTab.filename, "int");
            if (fName.Length > 12) {
                MessageBox.Show("Script file names must be 8 characters or under to be registered.", "Error");
                return;
            }
            if (currentTab.filename.Length >= 2 && string.Compare(currentTab.filename.Substring(0, 2), "gl", true) == 0) {
                if (MessageBox.Show("This script starts with 'gl', and will be treated by sfall as a global script and loaded automatically.\n" +
                                    "If it's being used as a global script, it does not need to be registered.\n" +
                                    "If it isn't, the script should be renamed before registering it.\n" +
                                    "Are you sure you wish to continue?", "Error") != DialogResult.Yes)
                    return;
            }
            if (fName.IndexOf(' ') != -1) {
                MessageBox.Show("Cannot register a script name that contains a space.", "Error");
                return;
            }
            RegisterScript.Registration(fName);
        }

        private void dgvErrors_DoubleClick(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.SelectedCells.Count != 1)
                return;

            Error error = dgv.Rows[dgv.SelectedCells[0].RowIndex].Cells[dgv == dgvErrors ? 3 : 2].Value as Error;
            if (error != null && error.line != -1)
                SelectLine(error.fileName, error.line, false, error.column, error.len);
        }

        private void preprocessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;

            dgvErrors.Rows.Clear();
            string msg;
            bool result = Compile(currentTab, out msg, true, true);
            tbOutput.Text = currentTab.buildLog = msg;
            if (!result) {
                MessageBox.Show("Pre-processed failed! See build tab log.");
                return;
            }

            string file = Compiler.GetPreprocessedFile(currentTab.filename);
            if (file != null)
                Open(file, OpenType.File, false);
            else
                MessageBox.Show("Failed to fetch preprocessed file");
        }

        private void roundtripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;

            if (Settings.userCmdCompile) {
                MessageBox.Show("It is required to turn off the compilation option via a user cmd file.");
                return;
            }
            dgvErrors.Rows.Clear();
            string msg;
            roundTrip = true;
            bool result = Compile(currentTab, out msg, showIcon: false);
            tbOutput.Text = currentTab.buildLog = msg;
            if (result) {
                Open(new Compiler(true).GetOutputPath(currentTab.filepath), OpenType.File, false);
            }
            roundTrip = false;
        }

        private void editRegisteredScriptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegisterScript.Registration(null);
        }

        private void associateMsgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;

            if (msgAutoOpenEditorStripMenuItem.Checked) {
                MessageEditor msgForm = MessageEditor.MessageEditorInit(currentTab, this);
                if (msgForm != null)
                    msgForm.SendMsgLine += AcceptMsgLine;
            } else
                AssociateMsg(currentTab, true);
        }

        private void editorMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (currentTab == null /*&& !treeView1.Focused*/) {
                e.Cancel = true;
                return;
            }
            if (currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected) {
                highlightToolStripMenuItem.Visible = true;
                renameToolStripMenuItem.Visible = false;
            } else {
                highlightToolStripMenuItem.Visible = false;
                renameToolStripMenuItem.Visible = true;
                renameToolStripMenuItem.Text = "Rename";
                renameToolStripMenuItem.Enabled = false;
                renameToolStripMenuItem.ToolTipText = (currentTab.needsParse) ? "Waiting get parsing data..." : "";
            }
            //openIncludeToolStripMenuItem.Enabled = false;
            findReferencesToolStripMenuItem.Enabled = false;
            findDeclerationToolStripMenuItem.Enabled = false;
            findDefinitionToolStripMenuItem.Enabled = false;
            UpdateEditorToolStripMenu();
        }

        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int i = (int)cmsTabControls.Tag;
            if ((i & 0x10000000) != 0)
                tabControl2.TabPages.RemoveAt(i ^ 0x10000000);
            else
                Close(tabs[i]);
        }

        void GoToLineToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (currentTab == null || goToLine != null) return;
            goToLine = new GoToLine();
            AddOwnedForm(goToLine);
            goToLine.tbLine.Maximum = currentDocument.TotalNumberOfLines;
            goToLine.tbLine.Select(0, 1);
            goToLine.bGo.Click += delegate(object a1, EventArgs a2) {
                TextAreaControl tac = currentActiveTextAreaCtrl;
                tac.Caret.Column = 0;
                tac.Caret.Line = Convert.ToInt32(goToLine.tbLine.Value - 1);
                tac.CenterViewOn(tac.Caret.Line, 0);
                goToLine.tbLine.Select();
            };
            goToLine.FormClosed += delegate(object a1, FormClosedEventArgs a2) { goToLine = null; };
            goToLine.Show();
        }

        void UPPERCASEToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected)
                new ICSharpCode.TextEditor.Actions.ToUpperCase().Execute(currentActiveTextAreaCtrl.TextArea);
        }

        void LowecaseToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected)
                new ICSharpCode.TextEditor.Actions.ToLowerCase().Execute(currentActiveTextAreaCtrl.TextArea);
        }

        private void ToggleBlockCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;
            if (currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected) {
                new ICSharpCode.TextEditor.Actions.ToggleBlockComment().Execute(
                    currentActiveTextAreaCtrl.TextArea);
            }
        }

        private void capitalizeCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;
            if (currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected)
                new ICSharpCode.TextEditor.Actions.CapitalizeAction().Execute(
                    currentActiveTextAreaCtrl.TextArea);
        }

        private void allTabsSpacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;
            new ICSharpCode.TextEditor.Actions.ConvertTabsToSpaces().Execute(currentActiveTextAreaCtrl.TextArea);
        }

        private void leadingTabsSpacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;
            new ICSharpCode.TextEditor.Actions.ConvertLeadingTabsToSpaces().Execute(currentActiveTextAreaCtrl.TextArea);
        }

        private void showTabsAndSpacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.showTabsChar = showTabsAndSpacesToolStripMenuItem.Checked;
            ShowTabsSpaces();
        }

        private void trailingSpacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.autoTrailingSpaces = trailingSpacesToolStripMenuItem.Checked;
        }

        private void ShowTabsSpaces()
        {
            if (currentTab == null)
                return;

            if (Path.GetExtension(currentTab.filename).ToLower() != ".msg")
                currentDocument.TextEditorProperties.ShowSpaces = showTabsAndSpacesToolStripMenuItem.Checked;

            currentDocument.TextEditorProperties.ShowTabs = showTabsAndSpacesToolStripMenuItem.Checked;;
            currentTab.textEditor.Refresh();
        }

        void CloseAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            for (int i = tabs.Count - 1; i >= 0; i--)
                Close(tabs[i]);
        }

        void CloseAllButThisToolStripMenuItemClick(object sender, EventArgs e)
        {
            int thisIndex = (int)cmsTabControls.Tag;
            for (int i = tabs.Count - 1; i >= 0; i--)
            {
                if (i != thisIndex)
                    Close(tabs[i]);
            }
        }

        void TextEditorDragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                Open(file, OpenType.File);
            }

            Activate();
            if (currentTab != null)
                currentActiveTextAreaCtrl.TextArea.AllowDrop = true;
        }

        void TextEditorDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            if (currentTab != null)
                currentActiveTextAreaCtrl.TextArea.AllowDrop = false;
        }

        private void minimize_log_button_Click(object sender, EventArgs e)
        {
            if (minimizelogsize == 0) {
                minimizelogsize = splitContainer1.SplitterDistance;
                splitContainer1.SplitterDistance = Size.Height;
                Settings.editorSplitterPosition = minimizelogsize;
            } else {
                int hs = Size.Height - (Size.Height / 4);
                if (Settings.editorSplitterPosition == -1)
                    Settings.editorSplitterPosition = hs;
                if (minimizelogsize > (hs + 100))
                    splitContainer1.SplitterDistance = hs;
                else
                    splitContainer1.SplitterDistance = Settings.editorSplitterPosition;
                minimizelogsize = 0;
            }
        }

        private void maximize_log()
        {
            if (currentTab == null && splitContainer1.Panel2Collapsed) {
                showLogWindowToolStripMenuItem.Checked = true;
                splitContainer1.Panel2Collapsed = false;
            }
            if (minimizelogsize == 0) return;

            if (Settings.editorSplitterPosition == -1) {
                Settings.editorSplitterPosition = Size.Height - (Size.Height / 4);
            }
            splitContainer1.SplitterDistance = Settings.editorSplitterPosition;
            minimizelogsize = 0;
        }

        private void showLogWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !(Settings.showLog = showLogWindowToolStripMenuItem.Checked);
        }

        private void Headers_toolStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            if (Settings.pathHeadersFiles == null || !Directory.Exists(Settings.pathHeadersFiles)) {
                MessageBox.Show("The headers directory does not exist. Check the correctness of the path setting.");
                return;
            }

            Headers Headfrm = new Headers(Headers_toolStripSplitButton.Bounds.Location);
            if (currentTab != null)
                Headfrm.Tag = currentActiveTextAreaCtrl;
            Headfrm.SelectHeaderFile += delegate(string sHeaderfile) {
                if (sHeaderfile != null)
                    Open(sHeaderfile, OpenType.File, false);
            };
            Headfrm.Show();
        }

        private void openHeaderFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofdHeaders = new OpenFileDialog();
            ofdHeaders.Title = "Select header files to open";
            ofdHeaders.Filter = "Header files|*.h";
            ofdHeaders.Multiselect = true;
            ofdHeaders.RestoreDirectory = true;
            ofdHeaders.InitialDirectory = Settings.pathHeadersFiles;
            if (ofdHeaders.ShowDialog() == DialogResult.OK) {
                foreach (string s in ofdHeaders.FileNames)
                {
                    Open(s, OpenType.File, false);
                }
            }
            ofdHeaders.Dispose();
        }

        private void openIncludesScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab.filepath != null) {
                foreach (string s in ParserInternal.GetAllIncludes(currentTab))
                {
                    Open(s, OpenType.File, addToMRU: false, seltab: false);
                }
            }
        }

        private void SplitDoc_Click(object sender, EventArgs e)
        {
            if (currentTab != null) {
                currentTab.textEditor.Split();
                if (!SplitEvent) {
                    SplitEvent = true;
                    SetActiveAreaEvents(currentTab.textEditor);
                }
                TextArea_SetFocus(null, null);
            }
        }

        private void ShowLineNumbers(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;

            PosChangeType = PositionType.AddPos;

            if (!currentTab.shouldParse) { // for not ssl files
                PosChangeType = PositionType.Disabled;
                splitContainer2.Panel2Collapsed = true;
            } else if (browserToolStripMenuItem.Checked)
                    splitContainer2.Panel2Collapsed = false;

            if (Path.GetExtension(currentTab.filename).ToLower() != ".msg") {
                currentDocument.TextEditorProperties.ShowLineNumbers = textLineNumberToolStripMenuItem.Checked;
                currentTab.textEditor.Refresh();
                tsmMessageTextChecker.Enabled = false;
            } else
                tsmMessageTextChecker.Enabled = true;
        }

        private void EncodingMenuItem_Click(object sender, EventArgs e)
        {
            if (((ToolStripMenuItem)sender).Tag != null /*&& ((ToolStripMenuItem)sender).Tag.ToString() == "dos"*/) {
                EncodingDOSmenuItem.Checked = true;
                windowsDefaultMenuItem.Checked = false;
                Settings.encoding = (byte)EncodingType.OEM866;
            } else {
                EncodingDOSmenuItem.Checked = false;
                windowsDefaultMenuItem.Checked = true;
                Settings.encoding = (byte)EncodingType.Default;
            }
            Settings.EncCodePage = (Settings.encoding == (byte)EncodingType.OEM866) ? Encoding.GetEncoding("cp866") : Encoding.Default;
            ApplySettingsTabs();
        }

        private void defineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.allowDefine = defineToolStripMenuItem.Checked;
        }

        private void addSearchTextComboBox(string world)
        {
            if (world.Length == 0)
                return;
            bool addSearchText = true;
            foreach (var item in SearchTextComboBox.Items)
            {
                if (world == item.ToString()) {
                    addSearchText = false;
                    break;
                }
            }
            if (addSearchText)
                SearchTextComboBox.Items.Insert(0, world);
        }

        private void DecIndentStripButton_Click(object sender, EventArgs e)
        {
            Utilities.DecIndent(currentActiveTextAreaCtrl);
        }

        private void CommentStripButton_Click(object sender, EventArgs e)
        {
            new ICSharpCode.TextEditor.Actions.ToggleComment().Execute(currentActiveTextAreaCtrl.TextArea);
        }

        private void CommentTextStripButton_Click(object sender, EventArgs e)
        {
            Utilities.CommentText(currentActiveTextAreaCtrl);
        }

        private void UnCommentTextStripButton_Click(object sender, EventArgs e)
        {
            Utilities.UnCommentText(currentActiveTextAreaCtrl);
        }

        private void AlignToLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.AlignToLeft(currentActiveTextAreaCtrl);
        }

        private void highlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.HighlightingSelectedText(currentActiveTextAreaCtrl);
            currentTab.textEditor.Refresh();
        }

        private void msgFileEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!msgAutoOpenEditorStripMenuItem.Checked && currentTab != null) {
                MessageEditor msgForm = MessageEditor.MessageEditorInit(currentTab, this);
                if (msgForm != null) {
                    msgForm.SendMsgLine += AcceptMsgLine;
                    return;
                }
            }
            MessageEditor.MessageEditorInit(null, this);
        }

        public void AcceptMsgLine(string line)
        {
            if (currentTab != null) {
                Utilities.InsertText(line, currentActiveTextAreaCtrl);
                this.Focus();
            }
        }

        private void pDefineStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
             Settings.preprocDef = (pDefineStripComboBox.SelectedIndex > 0)
                                     ? pDefineStripComboBox.SelectedItem.ToString()
                                     : null;
            if (currentTab != null)
                this.Text = SSE + currentTab.filepath + ((Settings.preprocDef != null)
                                                        ? " [" + Settings.preprocDef + "]" : "");
        }

        private void FunctionsTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (e.Node.Tag != null && currentTab != null) {
                if (!Functions.NodeHitCheck(e.Location, e.Node.Bounds))
                    return;

                string code = e.Node.Tag.ToString();
                int posCR = code.IndexOf("<cr>");
                if (posCR != -1) {
                    string space = new string(' ', currentActiveTextAreaCtrl.Caret.Column);
                    code = code.Remove(posCR) + Environment.NewLine + space;
                }
                if (!currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected) {
                    char c = currentDocument.GetCharAt(currentActiveTextAreaCtrl.Caret.Offset - 1);
                    if (char.IsLetterOrDigit(c)) code = " " + code;
                    if (posCR == -1) {
                        c = currentDocument.GetCharAt(currentActiveTextAreaCtrl.Caret.Offset);
                        if (char.IsLetterOrDigit(c)) code += " ";
                    }
                }
                currentActiveTextAreaCtrl.TextArea.InsertString(code);
            } else if (Functions.NodeHitCheck(e.Location, e.Node.Bounds))
                        e.Node.Toggle();
        }

        private void FunctionTree_MouseMove(object sender, MouseEventArgs e)
        {
            var treeView = (TreeView)sender;
            TreeNode node = treeView.GetNodeAt(e.Location);
            if (node != null && node.Tag != null && Functions.NodeHitCheck(e.Location, node.Bounds))
                node.TreeView.Cursor = Cursors.Hand;
            else if (treeView.Cursor != Cursors.Default)
                treeView.Cursor = Cursors.Default;
        }

        private void addUserFunctionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Functions.AddFunction(FunctionTreeLeft.SelectedNode);
        }

        private void editDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Functions.EditFunction(FunctionTreeLeft.SelectedNode);
        }

        private void cmsFunctions_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            addUserFunctionToolStripMenuItem.Enabled = false;
            editFunctionToolStripMenuItem.Enabled = false;
            addTreeNodeToolStripMenuItem.Enabled = false;
            renameTreeNodeToolStripMenuItem.Enabled = false;
            deleteNodeFuncToolStripMenuItem.Enabled = false;

            var node = FunctionTreeLeft.SelectedNode;
            if (node != null) {
                if (node.Tag != null)
                    editFunctionToolStripMenuItem.Enabled = true;

                if (Functions.IsUserFunction(node)) {
                    addUserFunctionToolStripMenuItem.Enabled = true;
                    if (node.Tag == null) {
                        if (node.Level < 2)
                            addTreeNodeToolStripMenuItem.Enabled = true;
                        renameTreeNodeToolStripMenuItem.Enabled = true;
                    }
                    if (node.Level > 0 && (node.Nodes.Count == 0 || node.Tag != null))
                        deleteNodeFuncToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void addTreeNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Functions.AddNode(FunctionTreeLeft.SelectedNode);
        }

        private void renameTreeNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Functions.RenameNode(FunctionTreeLeft.SelectedNode);
        }

        private void deleteNodeFuncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Functions.DeleteNode(FunctionTreeLeft.SelectedNode);
        }

        private void FunctionButton_Click(object sender, EventArgs e)
        {
            Control activeFocus = FindFocus(this.ActiveControl);

            splitContainer3.Hide();

            if (fuctionPanel > 0) {
                splitContainer3.Panel1Collapsed = true;
                fuctionPanel = 0;
            } else {
                if (fuctionPanel == -1) {
                    Functions.CreateTree(FunctionTreeLeft);
                    splitContainer3.Panel2MinSize = 900;
                    splitContainer3.SplitterDistance = 220;
                    fuctionPanel = 220;
                }
                splitContainer3.Panel1Collapsed = false;
                fuctionPanel = splitContainer3.SplitterDistance;
            }

            splitContainer3.Show();
            if (activeFocus != null)
                activeFocus.Select();
        }

        private Control FindFocus(Control cnt)
        {
            if (cnt == null)
                return null;

            foreach (Control c in cnt.Controls)
            {
                if (c.CanFocus && c.Focused)
                    return c;

                Control fc = FindFocus(c);

                if (fc != null)
                    return fc;
            }
            return null;
        }

        private void funcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FunctionButton.PerformClick();
        }

        private void browserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null ||!currentTab.shouldParse)
                return;
            splitContainer2.Panel2Collapsed = !browserToolStripMenuItem.Checked;
        }

        private void formatingCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.FormattingCode(currentActiveTextAreaCtrl);
        }

        private void GoBeginStripButton_Click(object sender, EventArgs e)
        {
            currentTab.textEditor.BeginUpdate();
            int beginLine = 1;
            foreach (FoldMarker fm in currentDocument.FoldingManager.FoldMarker) {
                if (fm.FoldType == FoldType.Region) {
                    beginLine = fm.StartLine + 1;
                    break;
                }
            }
            SelectLine(currentTab.filepath, beginLine);
            currentActiveTextAreaCtrl.SelectionManager.ClearSelection();
            currentTab.textEditor.EndUpdate();
        }

        void TextArea_SetFocus(object sender, EventArgs e)
        {
            if (!this.ContainsFocus || SearchTextComboBox.Focused || ReplaceTextBox.Focused)
                return;

            if (autoComplete.ShiftCaret) {
                autoComplete.ShiftCaret = false;
                currentActiveTextAreaCtrl.Caret.Position = currentDocument.OffsetToPosition(autoComplete.WordPosition.Key);
                currentActiveTextAreaCtrl.Caret.UpdateCaretPosition();
            }
            currentActiveTextAreaCtrl.TextArea.Focus();
            currentActiveTextAreaCtrl.TextArea.Select();
        }

        private void ViewArgsStripButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateNames();
        }

        private void SearchToolStrip_Resize(object sender, EventArgs e)
        {
            int w = ((ToolStrip)sender).Width;
            int size = (w / 2) - 150;
            SearchTextComboBox.Width = size + 50;
            ReplaceTextBox.Width = size;
        }

        private void ParsingErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parsingErrors = ParsingErrorsToolStripMenuItem.Checked;
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "/n, /select, " + tabs[(int)cmsTabControls.Tag].filepath);
        }

        private void tsmiClearAllLog_Click(object sender, EventArgs e)
        {
            dgvErrors.Rows.Clear();
            if (currentTab != null) {
                currentTab.buildErrors.Clear();
                currentTab.parserErrors.Clear();
            }
        }

        private void tsmCopyLogText_Click(object sender, EventArgs e)
        {
            if (dgvErrors.Rows.Count > 0 && dgvErrors.CurrentCell != null)
                Clipboard.SetText(dgvErrors.CurrentCell.Value.ToString(), TextDataFormat.Text);
        }

        private void RefreshLog_Click(object sender, EventArgs e)
        {
            if (currentTab != null) OutputErrorLog(currentTab);
        }

        private void showIndentLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.showVRuler = showIndentLineToolStripMenuItem.Checked;
            ApplySettingsTabs();
        }

        private void saveUTF8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.saveScriptUTF8 = saveUTF8ToolStripMenuItem.Checked;
        }

        private void tsmMessageTextChecker_Click(object sender, EventArgs e)
        {
            List<Error> report = MessageStructure.CheckStructure(currentActiveTextAreaCtrl, currentTab.filepath);
            if (currentTab.parserErrors.Count > 0 || report.Count > 0)
                dgvErrors.Rows.Clear();

            foreach (Error err in report)
                dgvErrors.Rows.Add(err.type.ToString(), Path.GetFileName(err.fileName), err.line, err);

            if (report.Count > 0) {
                currentTab.parserErrors = report;
                tabControl2.SelectedIndex = 2;
                maximize_log();
            } else
                MessageBox.Show("No mistakes!", "Checker");
        }

        private void FontSizeStripStatusLabel_Click(object sender, EventArgs e)
        {
            if (ctrlKeyPress) {
                if (--Settings.sizeFont < -5) Settings.sizeFont = 20;
            } else {
                if (++Settings.sizeFont > 20) Settings.sizeFont = -5;
            }
            SizeFontToString();
        }

        private void SizeFontToString()
        {
            // base 10 (min 5,  max 30)
            float percent = (float)((10 + Settings.sizeFont) / 10.0f) * 100.0f;
            FontSizeStripStatusLabel.Text = percent.ToString() + '%';

            if (currentTab != null) {
                var fontName = currentTab.textEditor.TextEditorProperties.Font.Name;
                var font = new Font(fontName, 10.0f + Settings.sizeFont, FontStyle.Regular);
                currentTab.textEditor.TextEditorProperties.Font = font;
                currentTab.textEditor.Refresh();
                currentActiveTextAreaCtrl.Caret.RecreateCaret();
            }
        }

        private void decompileF1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.decompileF1 = decompileF1ToolStripMenuItem.Checked;
        }

        private void win32RenderTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.winAPITextRender = win32RenderTextToolStripMenuItem.Checked;
        }

        private void caretModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Caret.GraphicsMode = (caretSoftwareModeToolStripMenuItem.Checked) ? ImplementationMode.SoftwareMode : ImplementationMode.Win32Mode;
            foreach (var tb in tabs) {
                tb.textEditor.ActiveTextAreaControl.Caret.RecreateGraphicsMode();
            }
        }

        private void openInExternalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.OpenInExternalEditor(tabs[(int)cmsTabControls.Tag].filepath);
        }

        private void includeFileToCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Settings.pathHeadersFiles == null || !Directory.Exists(Settings.pathHeadersFiles)) {
                MessageBox.Show("The headers directory does not exist. Check the correctness of the path setting.");
                return;
            }

            Headers Headfrm = new Headers(Headers_toolStripSplitButton.Bounds.Location);
            Headfrm.SelectHeaderFile += delegate(string sHeaderfile)
            {
                Utilities.PasteIncludeFile(sHeaderfile, currentActiveTextAreaCtrl);
            };
            Headfrm.Show();
        }

        private void oldDecompileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.oldDecompile = oldDecompileToolStripMenuItem.Checked;
        }

        private void convertHexDecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!currentActiveTextAreaCtrl.SelectionManager.HasSomethingSelected) return;
            bool isConvert = false;

            string text = currentActiveTextAreaCtrl.SelectionManager.SelectedText;
            if (text.IndexOf("0x", StringComparison.CurrentCultureIgnoreCase) != -1) {
                try {
                    text = Convert.ToInt32(text, 16).ToString(); // hex -> dec
                    isConvert = true;
                } catch (Exception) {}
            } else {
                int value;
                if (int.TryParse(text, out value)) {
                    text = "0x" + Convert.ToString(value, 16).ToUpper(); // dec -> hex
                    isConvert = true;
                }
            }
            if (isConvert) {
                ISelection sel = currentActiveTextAreaCtrl.SelectionManager.SelectionCollection[0];
                currentDocument.Replace(sel.Offset, sel.Length, text);
                currentActiveTextAreaCtrl.TextArea.Caret.Column = sel.StartPosition.Column;
                currentActiveTextAreaCtrl.SelectionManager.ClearSelection();
            }
        }
        #endregion
    }
}
