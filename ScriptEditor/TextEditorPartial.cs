using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ScriptEditor
{
    partial class TextEditor
    {
        #region ParseFunction

        private bool firstParse;

        public void intParserPrint(string info) {
            if (!Settings.enableParser) tbOutputParse.Text = info + tbOutputParse.Text;
        }

        // Parse first open script
        private void FirstParseScript(TabInfo cTab)
        {
            while (parserRunning) System.Threading.Thread.Sleep(1); //Avoid stomping on files while the parser is running
            tbOutputParse.Text = string.Empty;
            Parser.InternalParser(cTab, this);
            cTab.textEditor.Document.FoldingManager.UpdateFoldings(cTab.filename, cTab.parseInfo);
            cTab.textEditor.Document.FoldingManager.NotifyFoldingsChanged(null);
            if (tbOutputParse.Text.Length > 0)
                tabControl2.SelectedIndex = 0;
            firstParse = true; // prevention of repeated passage with parser turned off
        }

        private void ParseScript(int delay = 1)
        {
            if (!Settings.enableParser && !firstParse) {
                if (delay > 1) timer2Next = DateTime.Now + TimeSpan.FromSeconds(2);
                if (!timer2.Enabled) timer2.Start();
            }
            timerNext = DateTime.Now + TimeSpan.FromSeconds(delay);
            if (!timer.Enabled) timer.Start(); // External Parser begin

            firstParse = false;
        }

        // Delay timer for internal parsing
        void timer2_Tick(object sender, EventArgs e)
        {
            if (currentTab == null /*|| !currentTab.shouldParse*/) {
                timer2.Stop();
                return;
            }
            if (DateTime.Now > timer2Next && !parserRunning) {
                timer2.Stop();
                tbOutputParse.Text = string.Empty;
                Parser.InternalParser(currentTab, this);
                currentTab.textEditor.Document.FoldingManager.UpdateFoldings(currentTab.filename, currentTab.parseInfo);
                currentTab.textEditor.Document.FoldingManager.NotifyFoldingsChanged(null);
                UpdateNames();
            }
        }

        // Timer for parsing
        void timer_Tick(object sender, EventArgs e)
        {
            if (currentTab == null || !currentTab.shouldParse) {
                timer.Stop();
                return;
            }
            if (DateTime.Now > timerNext && !bwSyntaxParser.IsBusy && !parserRunning) {
                parserLabel.Text = (Settings.enableParser) ? "Parser: Working" : "Parser: Get only macros";
                parserLabel.ForeColor = Color.Crimson;
                parserRunning = true;
                bwSyntaxParser.RunWorkerAsync(new WorkerArgs(currentTab.textEditor.Document.TextContent, currentTab));
                timer.Stop();
            }
        }

        // Parse Start
        private void bwSyntaxParser_DoWork(object sender, System.ComponentModel.DoWorkEventArgs eventArgs)
        {
            WorkerArgs args = (WorkerArgs)eventArgs.Argument;
            var compiler = new Compiler();
            args.tab.parseInfo = compiler.Parse(args.text, args.tab.filepath, args.tab.parseInfo);
            eventArgs.Result = args.tab;
            parserRunning = false;
        }

        // Parse Stop
        private void bwSyntaxParser_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (currentTab.needsParse)
                    dgvErrors.Rows.Clear();
            if (File.Exists("errors.txt")) {
                try { //в случаях ошибки в parser.dll, не закрывается созданный им файл, что приводит к ошибке доступа
                    List<Error> errors = new List<Error>();
                    tbOutputParse.Text = Error.ParserLog(File.ReadAllText("errors.txt"), currentTab, errors); //+tbOutputParse.Text;
                    File.Delete("errors.txt");
                    foreach (Error err in errors)
                        dgvErrors.Rows.Insert(0, err.type.ToString(), Path.GetFileName(err.fileName), err.line, err);
                } catch {}
            }
            if (!(e.Result is TabInfo)) {
                throw new Exception("TabInfo is expected!");
            }
            var tab = e.Result as TabInfo;
            if (currentTab == tab) {
                if (tab.filepath != null) {
                    if (tab.parseInfo.parsed) {
                        tab.textEditor.Document.FoldingManager.UpdateFoldings(currentTab.filename, tab.parseInfo);
                        tab.textEditor.Document.FoldingManager.NotifyFoldingsChanged(null);
                        if (tab.parseInfo.procs.Length > 0) Outline_toolStripButton.Enabled = true;
                        UpdateNames(); // Update Tree Variables/Pocedures
                        parserLabel.Text = (Settings.enableParser) ? "Parser: Complete": parseoff + " [only macros]";
                        tab.needsParse = false;
                    } else {
                        parserLabel.Text = (Settings.enableParser) ? "Parser: Failed parsing (see parser errors tab)" : parseoff + " [only macros]";
                        //currentTab.needsParse = true; // требуется обновление
                    }
                } else {
                    parserLabel.Text = (Settings.enableParser) ? "Parser: Only local macros" : parseoff;
                }
            }
        }

        private void textChanged(object sender, EventArgs e)
        {
            if (!currentTab.changed) {
                currentTab.changed = true;
                SetTabTextChange(currentTab.index);
            }
            if (currentTab.shouldParse /*&& Settings.enableParser*/) { // if the parser is disabled then nothing
                if (currentTab.shouldParse && !currentTab.needsParse) {
                    currentTab.needsParse = true;
                    parserLabel.Text = "Parser: Out of date";
                }
                // Update parse info
                ParseScript(4);
            }
            var caret = currentTab.textEditor.ActiveTextAreaControl.Caret;
            string word = TextUtilities.GetWordAt(currentTab.textEditor.Document, caret.Offset - 1);
            if (word.Length < 2) {
                if (lbAutocomplete.Visible) {
                    lbAutocomplete.Hide();
                }
                if (toolTipAC.Active) {
                    toolTipAC.Hide(panel1);
                }
            }
        }
        #endregion

        #region Search Function
        private bool SubSearchInternal(List<int> offsets, List<int> lengths)
        {
            RegexOptions option = RegexOptions.None;
            Regex regex = null;

            if (!sf.cbCase.Checked)
                option = RegexOptions.IgnoreCase;

            if (sf.cbRegular.Checked)
                regex = new Regex(sf.tbSearch.Text, option);
            else if (sf.cbWord.Checked)
                regex = new Regex(@"\b" + sf.tbSearch.Text + @"\b", option);

            if (sf.rbFolder.Checked && Settings.lastSearchPath == null) {
                MessageBox.Show("No search path set.", "Error");
                return false;
            }
            if (!sf.cbFindAll.Checked) {
                if (sf.rbCurrent.Checked || (sf.rbAll.Checked && tabs.Count < 2)) {
                    if (currentTab == null)
                        return false;
                    if (Utilities.SearchAndScroll(currentTab, regex, sf.tbSearch.Text, sf.cbCase.Checked, ref PosChangeType))
                        return true;
                } else if (sf.rbAll.Checked) {
                    int starttab = currentTab == null ? 0 : currentTab.index;
                    int endtab = starttab == 0 ? tabs.Count - 1 : starttab - 1;
                    int tab = starttab - 1;
                    int caretOffset = currentTab.textEditor.ActiveTextAreaControl.Caret.Offset;
                    do {
                        if (++tab == tabs.Count)
                            tab = 0; //restart tab
                        int start, len;
                        if (Utilities.Search(tabs[tab].textEditor.Text, sf.tbSearch.Text, regex, caretOffset + 1, false, sf.cbCase.Checked, out start, out len)) {
                            Utilities.FindSelected(tabs[tab], start, len, ref PosChangeType);
                            if (currentTab == null || currentTab.index != tab)
                                tabControl1.SelectTab(tab);
                            return true;
                        }
                        caretOffset = 0; // search from begin 
                    } while (tab != endtab);
                } else {
                    sf.lbFindFiles.Items.Clear();
                    sf.lbFindFiles.Tag = regex;
                    List<string> files = sf.GetFolderFiles();
                    for (int i = 0; i < files.Count; i++)
                    {
                        if (Utilities.Search(File.ReadAllText(files[i]), sf.tbSearch.Text, regex, sf.cbCase.Checked))
                            sf.lbFindFiles.Items.Add(files[i]);
                    }
                    sf.labelCount.Text = sf.lbFindFiles.Items.Count.ToString();
                    if (sf.lbFindFiles.Items.Count > 0) {
                        sf.Height = 468;
                        return true;
                    }
                }
            } else {
                DataGridView dgv = CommonDGV.DataGridCreate();
                dgv.DoubleClick += dgvErrors_DoubleClick;

                if (sf.rbCurrent.Checked || (sf.rbAll.Checked && tabs.Count < 2)) {
                    if (currentTab == null)
                        return false;
                    Utilities.SearchForAll(currentTab, sf.tbSearch.Text, regex, sf.cbCase.Checked, dgv, offsets, lengths);
                } else if (sf.rbAll.Checked) {
                    for (int i = 0; i < tabs.Count; i++)
                        Utilities.SearchForAll(tabs[i], sf.tbSearch.Text, regex, sf.cbCase.Checked, dgv, offsets, lengths);
                } else {
                    List<string> files = sf.GetFolderFiles();
                    for (int i = 0; i < files.Count; i++)
                        Utilities.SearchForAll(File.ReadAllLines(files[i]), Path.GetFullPath(files[i]), sf.tbSearch.Text, regex, sf.cbCase.Checked, dgv);
                }
                if (dgv.RowCount > 0) {
                    TabPage tp = new TabPage("Search results");
                    tp.ToolTipText = "Find text: " + sf.tbSearch.Text;
                    tp.Controls.Add(dgv);
                    dgv.Dock = DockStyle.Fill;
                    tabControl2.TabPages.Add(tp);
                    tabControl2.SelectTab(tp);
                    maximize_log();
                    return true;
                }
            }
            MessageBox.Show("Search string not found", "Search");
            return false;
        }
        #endregion

        #region Search&Replace function form
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sf == null) {
                sf = new SearchForm();
                sf.Owner = this;
                sf.FormClosing += delegate(object a1, FormClosingEventArgs a2) { sf = null; };
                sf.KeyUp += delegate(object a1, KeyEventArgs a2) {
                    if (a2.KeyCode == Keys.Escape) {
                        sf.Close();
                    }
                };
                sf.rbFolder.CheckedChanged += delegate(object a1, EventArgs a2) {
                    sf.bChange.Enabled = sf.cbSearchSubfolders.Enabled = sf.rbFolder.Checked;
                    sf.bReplace.Enabled = !sf.rbFolder.Checked;
                };
                sf.tbSearch.KeyPress += delegate(object a1, KeyPressEventArgs a2) { if (a2.KeyChar == '\r') { bSearch_Click(null, null); a2.Handled = true; } };
                sf.bChange.Click += delegate(object a1, EventArgs a2) {
                    if (sf.fbdSearchFolder.ShowDialog() != DialogResult.OK)
                        return;
                    Settings.lastSearchPath = sf.fbdSearchFolder.SelectedPath;
                    sf.textBox1.Text = Settings.lastSearchPath;
                };
                sf.lbFindFiles.MouseDoubleClick += delegate (object a1, MouseEventArgs a2) {
                    string file = sf.lbFindFiles.SelectedItem.ToString();
                    Utilities.SearchAndScroll(Open(file, OpenType.File), (Regex)sf.lbFindFiles.Tag, sf.tbSearch.Text, sf.cbCase.Checked, ref PosChangeType);
                };
                sf.bSearch.Click += new EventHandler(bSearch_Click);
                sf.bReplace.Click += new EventHandler(bReplace_Click);
                sf.Show();
            } else {
                sf.WindowState = FormWindowState.Normal;
                sf.Focus();
                sf.tbSearch.Focus();
            }
            string str = "";
            if (currentTab != null) {
                str = currentTab.textEditor.ActiveTextAreaControl.SelectionManager.SelectedText;
            }
            if (str.Length == 0 || str.Length > 255) {
                str = Clipboard.GetText();
            }
            if (str.Length > 0 && str.Length < 255) {
                sf.tbSearch.Text = str;
                sf.tbSearch.SelectAll();
            }
        }

        private void bSearch_Click(object sender, EventArgs e)
        {
            sf.tbSearch.Text = sf.tbSearch.Text.Trim();
            if (sf.tbSearch.Text.Length == 0) return;
            SubSearchInternal(null, null);
        }

        void bReplace_Click(object sender, EventArgs e)
        {
            sf.tbSearch.Text = sf.tbSearch.Text.Trim();
            if (sf.rbFolder.Checked || sf.tbSearch.Text.Length == 0)
                return;
            if (sf.cbFindAll.Checked) {
                List<int> lengths = new List<int>(), offsets = new List<int>();
                if (!SubSearchInternal(offsets, lengths))
                    return;
                for (int i = offsets.Count - 1; i >= 0; i--) {
                    currentTab.textEditor.Document.Replace(offsets[i], lengths[i], sf.tbReplace.Text);
                }
            } else {
                currentTab.textEditor.ActiveTextAreaControl.Caret.Column--;
                if (!SubSearchInternal(null, null))
                    return;
                ISelection selected = currentTab.textEditor.ActiveTextAreaControl.SelectionManager.SelectionCollection[0];
                currentTab.textEditor.Document.Replace(selected.Offset, selected.Length, sf.tbReplace.Text);
                selected.EndPosition = new TextLocation(selected.StartPosition.Column + sf.tbReplace.Text.Length, selected.EndPosition.Line);
                currentTab.textEditor.ActiveTextAreaControl.SelectionManager.SetSelection(selected);
            }
        }

        // Search for quick panel
        private void FindForwardButton_Click(object sender, EventArgs e)
        {
            string find = SearchTextComboBox.Text.Trim();
            if (find.Length == 0 || currentTab == null) return;
            int z = Utilities.SearchPanel(currentTab.textEditor.Text, find, currentTab.textEditor.ActiveTextAreaControl.Caret.Offset + 1, CaseButton.Checked, WholeWordButton.Checked);
            if (z != -1) Utilities.FindSelected(currentTab, z, find.Length, ref PosChangeType);
            else DontFind.Play();
            addSearchTextComboBox(find);
        }

        private void FindBackButton_Click(object sender, EventArgs e)
        {
            string find = SearchTextComboBox.Text.Trim();
            if (find.Length == 0 || currentTab == null) return;
            int offset = currentTab.textEditor.ActiveTextAreaControl.Caret.Offset;
            string text = currentTab.textEditor.Text.Remove(offset);
            int z = Utilities.SearchPanel(text, find, offset - 1, CaseButton.Checked, WholeWordButton.Checked, true);
            if (z != -1) Utilities.FindSelected(currentTab, z, find.Length, ref PosChangeType);
            else DontFind.Play();
            addSearchTextComboBox(find);
        }

        private void ReplaceButton_Click(object sender, EventArgs e)
        {
            string find = SearchTextComboBox.Text.Trim();
            if (find.Length == 0) return;
            string replace = ReplaceTextBox.Text.Trim();
            int z = Utilities.SearchPanel(currentTab.textEditor.Text, find, currentTab.textEditor.ActiveTextAreaControl.Caret.Offset, CaseButton.Checked, WholeWordButton.Checked);
            if (z != -1) Utilities.FindSelected(currentTab, z, find.Length, ref PosChangeType, replace);
            else DontFind.Play();
            addSearchTextComboBox(find);
        }

        private void ReplaceAllButton_Click(object sender, EventArgs e)
        {
            string find = SearchTextComboBox.Text.Trim();
            if (find.Length == 0) return;
            string replace = ReplaceTextBox.Text.Trim();
            int z, offset = 0;
            do {
                z = Utilities.SearchPanel(currentTab.textEditor.Text, find, offset, CaseButton.Checked, WholeWordButton.Checked);
                if (z != -1) currentTab.textEditor.ActiveTextAreaControl.Document.Replace(z, find.Length, replace);
                offset = z + 1;
            } while (z != -1);
            addSearchTextComboBox(find);
        }

        private void SendtoolStripButton_Click(object sender, EventArgs e)
        {
            string word = currentTab.textEditor.ActiveTextAreaControl.SelectionManager.SelectedText;
            if (word == string.Empty) word = TextUtilities.GetWordAt(currentTab.textEditor.Document, currentTab.textEditor.ActiveTextAreaControl.Caret.Offset);
            if (word != string.Empty) SearchTextComboBox.Text = word;
        }

        private void quickFindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;
            SendtoolStripButton.PerformClick();
            FindForwardButton.PerformClick();
            if (!SearchToolStrip.Visible) {
                SearchToolStrip.Visible = true;
                TabClose_button.Top += (SearchToolStrip.Visible) ? 25 : -25;
            }
        }

        private void Search_Panel(object sender, EventArgs e)
        {
            if (currentTab == null && !SearchToolStrip.Visible) {
                findToolStripMenuItem_Click(null, null);
                return;
            }
            SearchToolStrip.Visible = !SearchToolStrip.Visible;
            TabClose_button.Top += (SearchToolStrip.Visible) ? 25 : -25;
        }
        #endregion

        #region References/DeclerationDefinition & Include function
        private void findReferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextLocation tl = (TextLocation)editorMenuStrip.Tag;
            string word = TextUtilities.GetWordAt(currentTab.textEditor.Document, currentTab.textEditor.Document.PositionToOffset(tl));

            Reference[] refs = currentTab.parseInfo.LookupReferences(word, currentTab.filename, tl.Line);
            if (refs == null)
                return;
            if (refs.Length == 0) {
                MessageBox.Show("No references found", "Message");
                return;
            }
            DataGridView dgv = CommonDGV.DataGridCreate();
            dgv.DoubleClick += dgvErrors_DoubleClick;

            foreach (var r in refs) {
                Error error = new Error() {
                    fileName = r.file,
                    line = r.line,
                    message = String.Compare(Path.GetFileName(r.file), currentTab.filename, true) == 0 ? TextUtilities.GetLineAsString(currentTab.textEditor.Document, r.line - 1).TrimStart() : word
                };
                dgv.Rows.Add(r.file, error.line.ToString(), error);
            }

            TabPage tp = new TabPage("'" + word + "' references");
            tp.Controls.Add(dgv);
            dgv.Dock = DockStyle.Fill;
            tabControl2.TabPages.Add(tp);
            tabControl2.SelectTab(tp);
            maximize_log();
            TextArea_SetFocus(null, null);
        }

        private void findDeclerationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextLocation tl = (TextLocation)editorMenuStrip.Tag;
            string word = TextUtilities.GetWordAt(currentTab.textEditor.Document, currentTab.textEditor.Document.PositionToOffset(tl));
            string file;
            int line;
            currentTab.parseInfo.LookupDecleration(word, currentTab.filename, tl.Line, out file, out line);
            if (file.ToLower() == Compiler.parserPath.ToLower()) file = currentTab.filepath;
            SelectLine(file, line);
        }

        private void findDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string word, file = currentTab.filepath;
            int line;
            if (((ToolStripDropDownItem)sender).Tag != null) { // "Button"
                if (!currentTab.shouldParse) return;
                Parser.UpdateParseSSL(currentTab.textEditor.Text);
                TextLocation tl = currentTab.textEditor.ActiveTextAreaControl.Caret.Position;
                word = TextUtilities.GetWordAt(currentTab.textEditor.Document, currentTab.textEditor.Document.PositionToOffset(tl));
                line = Parser.GetProcBeginEndBlock(word, 0, true).begin;
                if (line != -1) line++; else return;
            } else {
                TextLocation tl = (TextLocation)editorMenuStrip.Tag;
                word = TextUtilities.GetWordAt(currentTab.textEditor.Document, currentTab.textEditor.Document.PositionToOffset(tl));
                currentTab.parseInfo.LookupDefinition(word, out file, out line);
            }
            SelectLine(file, line);
        }

        private void openIncludeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextLocation tl = (TextLocation)editorMenuStrip.Tag;
            string[] line = TextUtilities.GetLineAsString(currentTab.textEditor.Document, tl.Line).Split('"');
            if (line.Length < 2)
                return;
            if (Path.IsPathRooted(line[1]) && File.Exists(line[1]))
                Open(line[1], OpenType.File, false);
            else {
                if (currentTab.filepath == null) {
                    MessageBox.Show("Cannot open includes given via a relative path for an unsaved script", "Error");
                    return;
                }
                Parser.includePath(ref line[1], Path.GetDirectoryName(currentTab.filepath));
                Open(line[1], OpenType.File, false);
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.RefactorRename((IParserInfo)renameToolStripMenuItem.Tag, currentTab.textEditor);
        }

        private void highlightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.HighlightingSelectedText(currentTab.textEditor);
            currentTab.textEditor.Refresh();
        }
        #endregion

        #region Autocomplete function
        private void lbAutoCompleteKey(PreviewKeyDownEventArgs a2)
        {
            if (lbAutocomplete.Visible) {
                if (a2.KeyCode == Keys.Tab || a2.KeyCode == Keys.Down) {
                    lbAutocompleteShiftCaret = true;
                    lbAutocomplete.SelectedIndex = 0;
                    lbAutocomplete.Focus();
                }
                else if (a2.KeyCode == Keys.Enter && lbAutocomplete.SelectedIndex != -1) {
                    lbAutocomplete_Paste(null, null);
                }
                else if (a2.KeyCode == Keys.Enter || a2.KeyCode == Keys.Up || a2.KeyCode == Keys.Escape)
                    lbAutocomplete.Hide();
            } else {
                if (toolTipAC.Active && a2.KeyCode != Keys.Left && a2.KeyCode != Keys.Right)
                    toolTipAC.Hide(panel1);
            }
        }

        private void lbAutocomplete_Paste(object sender, MouseEventArgs e)
        {
            lbAutocompleteShiftCaret = false;
            KeyValuePair<int, string> selection = (KeyValuePair<int, string>)lbAutocomplete.Tag;
            AutoCompleteItem item = (AutoCompleteItem)lbAutocomplete.SelectedItem;
            int startOffs = selection.Key - selection.Value.Length;
            currentTab.textEditor.Document.Replace(startOffs, selection.Value.Length, item.name);
            currentTab.textEditor.ActiveTextAreaControl.TextArea.Focus();
            currentTab.textEditor.ActiveTextAreaControl.Caret.Position = currentTab.textEditor.Document.OffsetToPosition(startOffs + item.name.Length);
            lbAutocomplete.Hide();
        }

        void LbAutocompleteKeyDown(object snd, KeyEventArgs evt)
        {
            if (evt.KeyCode == Keys.Enter && lbAutocomplete.SelectedIndex != -1) {
                lbAutocomplete_Paste(null, null);
            } else if (evt.KeyCode == Keys.Escape) {
                currentTab.textEditor.ActiveTextAreaControl.TextArea.Focus();
                lbAutocomplete.Hide();
            }
        }

        void LbAutocompleteSelectedIndexChanged(object sender, EventArgs e)
        {
            AutoCompleteItem acItem = (AutoCompleteItem)lbAutocomplete.SelectedItem;
            if (acItem != null) {
                toolTipAC.Show(acItem.hint, panel1, lbAutocomplete.Left + lbAutocomplete.Width + 10, lbAutocomplete.Top, 50000);
            }
        }

        void LbAutocompleteVisibleChanged(object sender, EventArgs e)
        {
            if (toolTipAC.Active) toolTipAC.Hide(panel1);
        }

        private void lbAutocomplete_MouseMove(object sender, MouseEventArgs e)
        {
            int item = 0;
            if (e.Y != 0)
                item = e.Y / lbAutocomplete.ItemHeight;
            lbAutocomplete.SelectedIndex = lbAutocomplete.TopIndex + item;
        }

        private void lbAutocomplete_MouseEnter(object sender, EventArgs e)
        {
            if (lbAutocomplete.SelectedIndex < 0) return;
            lbAutocomplete.Focus();
        }

        private void TextArea_MouseWheel(object sender, MouseEventArgs e)
        {
           if (lbAutocomplete.Visible && e.Delta != 0) {
               int h = 50 + currentTab.textEditor.ActiveTextAreaControl.Height;
               var tePos = currentTab.textEditor.ActiveTextAreaControl.FindForm().PointToClient(currentTab.textEditor.ActiveTextAreaControl.Parent.PointToScreen(currentTab.textEditor.ActiveTextAreaControl.Location));
               var caretPos = currentTab.textEditor.ActiveTextAreaControl.Caret.ScreenPosition;
               tePos.Offset(caretPos);
               if (e.Delta < 0) tePos.Offset(-5, -32);
               else tePos.Offset(-5, 70);
               if (tePos.Y > h || tePos.Y < 50) lbAutocomplete.Hide();
               lbAutocomplete.Location = tePos;
           }
        }
        #endregion

        #region Function Back/Forward
        internal enum PositionType { AddPos, NoSave, SaveChange, Disabled }
        // AddPos - При перемещении добавлять новую позицию в историю.
        // NoSave - Не сохранять следующее перемещение в историю.
        // SaveChange - Изменить следующее перемещение в текущей позиции истории.
        // Disabled - Не сохранять все последуюшие перемещения в историю (до явного включения функции).

        private void SetBackForwardButtonState() 
        {
            if (currentTab.history.pointerCur > 0) {
                Back_toolStripButton.Enabled = true;
            } else {
                Back_toolStripButton.Enabled = false;
            }
            if (currentTab.history.pointerCur == currentTab.history.pointerEnd || currentTab.history.pointerCur < 0) {
                Forward_toolStripButton.Enabled = false;
            } else if (currentTab.history.pointerCur > 0 || currentTab.history.pointerCur < currentTab.history.pointerEnd) { 
                Forward_toolStripButton.Enabled = true;
            }
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            string ext = Path.GetExtension(currentTab.filename).ToLower();
            if (ext != ".ssl" && ext != ".h") return;

            TextLocation _position = currentTab.textEditor.ActiveTextAreaControl.Caret.Position;
            int curLine = _position.Line + 1;
            LineStripStatusLabel.Text = "Line: " + curLine;
            ColStripStatusLabel.Text = "Col: " + (_position.Column + 1);
            if (PosChangeType >= PositionType.Disabled) return;
            if (PosChangeType >= PositionType.NoSave) {
                if (PosChangeType == PositionType.SaveChange && currentTab.history.pointerCur != -1) {
                    currentTab.history.linePosition[currentTab.history.pointerCur] = _position;
                }
                PosChangeType = PositionType.AddPos; // set default
                return;
            }
            if (curLine != currentTab.history.prevPosition) {
                currentTab.history.pointerCur++;
                int size = currentTab.history.linePosition.Length;
                if (currentTab.history.pointerCur >= size) {
                    Array.Resize(ref currentTab.history.linePosition, size + 1); 
                }
                currentTab.history.linePosition[currentTab.history.pointerCur] = _position;
                currentTab.history.prevPosition = curLine;
                currentTab.history.pointerEnd = currentTab.history.pointerCur;
            }
            SetBackForwardButtonState();  
        }

        private void Back_toolStripButton_Click(object sender, EventArgs e)
        {
            if (currentTab == null || currentTab.history.pointerCur == 0) return;
            currentTab.history.pointerCur--;
            GotoViewLine(); 
        }

        private void Forward_toolStripButton_Click(object sender, EventArgs e)
        {
            if (currentTab == null || currentTab.history.pointerCur >= currentTab.history.pointerEnd) return;
            currentTab.history.pointerCur++;
            GotoViewLine();
        }

        private void GotoViewLine()
        {
            PosChangeType = PositionType.NoSave;
            currentTab.textEditor.ActiveTextAreaControl.Caret.Position = currentTab.history.linePosition[currentTab.history.pointerCur];
            currentTab.textEditor.ActiveTextAreaControl.CenterViewOn(currentTab.textEditor.ActiveTextAreaControl.Caret.Line, 0);
            SetBackForwardButtonState();
        }
        #endregion

        #region Create/Rename/Delete/Move Procedure Function
        // Create Handlers Procedures
        public void CreateProcBlock(string name)
        {
            Parser.UpdateParseSSL(currentTab.textEditor.Text);
            if (Parser.CheckExistsProcedureName(name)) return;
            byte inc = 0;
            if (name == "look_at_p_proc" || name == "description_p_proc") inc++;
            ProcForm CreateProcFrm = new ProcForm();
            CreateProcFrm.ProcedureName.Text = name;
            CreateProcFrm.ProcedureName.ReadOnly = true;
            if (ProcTree.SelectedNode != null && ProcTree.SelectedNode.Tag is Procedure) {
                CreateProcFrm.checkBox1.Enabled = false;
            } else CreateProcFrm.groupBox1.Enabled = false;
            ProcTree.HideSelection = false;
            if (CreateProcFrm.ShowDialog() == DialogResult.Cancel) {
                ProcTree.HideSelection = true;
                return;
            }
            ProcBlock block = new ProcBlock();
            if (CreateProcFrm.radioButton2.Checked) {
                block = Parser.GetProcBeginEndBlock(((Procedure)ProcTree.SelectedNode.Tag).name);
            }
            InsertProcedure(CreateProcFrm.ProcedureName.Text, block, CreateProcFrm.radioButton2.Checked, inc);
            CreateProcFrm.Dispose();
        }

        // Create Procedures
        private void createProcedureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcForm CreateProcFrm = new ProcForm();
            TextLocation textloc = currentTab.textEditor.ActiveTextAreaControl.Caret.Position;
            string word = TextUtilities.GetWordAt(currentTab.textEditor.Document, currentTab.textEditor.Document.PositionToOffset(textloc));
            CreateProcFrm.ProcedureName.Text = word;
            if (ProcTree.SelectedNode != null && ProcTree.SelectedNode.Tag is Procedure) {}
            else CreateProcFrm.groupBox1.Enabled = false;
            ProcTree.HideSelection = false;
            if (CreateProcFrm.ShowDialog() == DialogResult.Cancel) {
                ProcTree.HideSelection = true;
                return;
            }
            Parser.UpdateParseSSL(currentTab.textEditor.Text);
            string name = CreateProcFrm.ProcedureName.Text;
            if (Parser.CheckExistsProcedureName(name)) return;
            ProcBlock block = new ProcBlock();
            if (CreateProcFrm.checkBox1.Checked || CreateProcFrm.radioButton2.Checked) {
                block = Parser.GetProcBeginEndBlock(((Procedure)ProcTree.SelectedNode.Tag).name);
                block.copy = CreateProcFrm.checkBox1.Checked;
            }
            InsertProcedure(name, block, CreateProcFrm.radioButton2.Checked);
            CreateProcFrm.Dispose();
        }

        // Create procedure block
        private void InsertProcedure(string name, ProcBlock block, bool after = false, byte overrides = 0)
        {
            currentTab.textEditor.Document.UndoStack.StartUndoGroup();
            int findLine, caretline = 2;
            string procbody;
            //Copy from procedure
            if (block.copy) {
                procbody = GetSelectBlockText(block.begin + 1, block.end, 0);
                overrides = 1;
            } else procbody = "script_overrides;\r\n\r\n".PadLeft(Settings.tabSize);
            string procblock = (overrides > 0)
                       ? "\r\nprocedure " + name + " begin\r\n" + procbody + "end\r\n"
                       : "\r\nprocedure " + name + " begin\r\n\r\nend\r\n";
            if (after) findLine = Parser.GetDeclarationProcedureLine(((Procedure)ProcTree.SelectedNode.Tag).name) + 1;
                else findLine = Parser.GetEndLineProcDeclaration(); 
            if (findLine == -1) {
                findLine = 0;
                MessageBox.Show("The declaration procedure is broken, declaration written to beginning of script.", "Warning");
            }
            currentTab.textEditor.ActiveTextAreaControl.SelectionManager.ClearSelection();
            int offset = currentTab.textEditor.Document.PositionToOffset(new TextLocation(0, findLine));
            currentTab.textEditor.Document.Insert(offset, "procedure " + name + ";" + Environment.NewLine);
            // proc body
            if (after) findLine = block.end + 1 ; // after current procedure
                else findLine = currentTab.textEditor.Document.TotalNumberOfLines - 1; // paste to end script
            int len = TextUtilities.GetLineAsString(currentTab.textEditor.Document, findLine).Length;
            if (len > 0) {
                procblock = Environment.NewLine + procblock;
                caretline++;
            }
            offset = currentTab.textEditor.Document.PositionToOffset(new TextLocation(len, findLine));
            currentTab.textEditor.Document.Insert(offset, procblock);
            currentTab.textEditor.ActiveTextAreaControl.Caret.Column = 0;
            currentTab.textEditor.ActiveTextAreaControl.Caret.Line = findLine + (caretline + overrides);
            currentTab.textEditor.ActiveTextAreaControl.CenterViewOn(findLine + (caretline + overrides), 0);
            currentTab.textEditor.Document.UndoStack.EndUndoGroup();
            SetFocusDocument();
        }

        // Rename Procedures
        private void renameProcedureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProcTree.SelectedNode == null) return;
            string oldName = ((Procedure)ProcTree.SelectedNode.Tag).name; //original name
            ProcTree.HideSelection = false;
            Utilities.RenameProcedure(oldName, currentTab.textEditor);
            ProcTree.HideSelection = true;
            SetFocusDocument();
        }

        // Delete Procedures
        private void deleteProcedureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProcTree.SelectedNode == null) return;
            if (MessageBox.Show("Are you sure you want to delete \"" + ((Procedure)ProcTree.SelectedNode.Tag).name + "\" procedure?",
                "Warning", MessageBoxButtons.YesNo) == DialogResult.No) return;
            Parser.UpdateParseSSL(currentTab.textEditor.Text);
            string def_poc;
            string procName = ((Procedure)ProcTree.SelectedNode.Tag).name;
            ProcBlock block = Parser.GetProcBeginEndBlock(procName, 0, true);
            currentTab.textEditor.Document.UndoStack.StartUndoGroup();
            DeleteProcedure(procName, block, out def_poc);
            currentTab.textEditor.Document.UndoStack.EndUndoGroup();
            SetFocusDocument();
        }

        private void DeleteProcedure(string procName, ProcBlock block, out string def_poc)
        {
            int offset = currentTab.textEditor.Document.PositionToOffset(new TextLocation(0, block.begin));
            int len = currentTab.textEditor.Document.PositionToOffset(new TextLocation(0, block.end)) - offset;
            len += TextUtilities.GetLineAsString(currentTab.textEditor.Document, block.end).Length;
            currentTab.textEditor.Document.Remove(offset, len + 2);
            // declare
            int declarLine = Parser.GetDeclarationProcedureLine(procName);
            if (declarLine > -1 & declarLine != block.begin) {
                def_poc = TextUtilities.GetLineAsString(currentTab.textEditor.Document, declarLine);
                offset = currentTab.textEditor.Document.PositionToOffset(new TextLocation(0, declarLine));
                currentTab.textEditor.Document.Remove(offset, def_poc.Length + 2);
            }
            else def_poc = null;
            currentTab.textEditor.ActiveTextAreaControl.SelectionManager.ClearSelection();
        }

        //Select block and return text
        private string GetSelectBlockText(int _begin, int _end, int _ecol = -1, int _bcol = 0)
        {
            if (_ecol == -1) _ecol = TextUtilities.GetLineAsString(currentTab.textEditor.Document, _end).Length;
            currentTab.textEditor.ActiveTextAreaControl.SelectionManager.SetSelection(new TextLocation(_bcol, _begin), new TextLocation(_ecol, _end));
            return currentTab.textEditor.ActiveTextAreaControl.SelectionManager.SelectedText;
        }

        //Update Procedure Tree
        private void SetFocusDocument()
        {
            TextArea_SetFocus(null, null);
            if (Settings.enableParser) {
                timerNext = DateTime.Now;
                timer.Start(); // Parser begin
            } else {
                ParseScript();
                Outline_toolStripButton.Enabled = true;
            }
            ProcTree.HideSelection = true;
        }

        private void ProcMnContext_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ProcTree.SelectedNode != null && ProcTree.SelectedNode.Tag is Procedure) {
                ProcMnContext.Items[1].Enabled = true;
                ProcMnContext.Items[3].Enabled = true; // moved disabled
                ProcMnContext.Items[4].Enabled = true;
                ProcMnContext.Items[4].Text = "Delete: " + ((Procedure)ProcTree.SelectedNode.Tag).name;
            } else {
                ProcMnContext.Items[1].Enabled = false;
                ProcMnContext.Items[3].Enabled = false;
                ProcMnContext.Items[4].Enabled = false;
                ProcMnContext.Items[4].Text = "Delete procedure";
            }
        }

        private void MoveProcedure(int sIndex)
        {
            bool moveToEnd = false;
            int root = ProcTree.Nodes.Count - 1;
            Parser.UpdateParseSSL(currentTab.textEditor.Text);
            string moveName = ProcTree.Nodes[root].Nodes[moveActive].Text;
            if (sIndex > moveActive) {
                if (sIndex >= (ProcTree.Nodes[root].Nodes.Count - 1))
                    moveToEnd = true;
                else 
                    sIndex++;
            }
            // copy body
            ProcBlock block = Parser.GetProcBeginEndBlock(moveName, 0, true);
            string copy_procbody = Environment.NewLine + GetSelectBlockText(block.begin, block.end, 1000);
            string copy_defproc;
            currentTab.textEditor.Document.UndoStack.StartUndoGroup();
            //
            DeleteProcedure(moveName, block, out copy_defproc);
            Parser.UpdateParseSSL(currentTab.textEditor.Text);
            //
            string name = ProcTree.Nodes[root].Nodes[sIndex].Text;
            // insert declration
            int offset;
            if (copy_defproc != null) {
                int p_def = Parser.GetDeclarationProcedureLine(name);
                if (moveToEnd) p_def++;
                offset = currentTab.textEditor.Document.PositionToOffset(new TextLocation(0, p_def));
                currentTab.textEditor.Document.Insert(offset, copy_defproc + Environment.NewLine);
            }
            //paste proc block
            block = Parser.GetProcBeginEndBlock(name, 0, true);
            int p_begin;
            if (moveToEnd) {
                p_begin = block.end + 1;
                copy_procbody = Environment.NewLine + copy_procbody;
            } else {
                p_begin = block.begin;
                copy_procbody += Environment.NewLine;
            }
            offset = currentTab.textEditor.Document.PositionToOffset(new TextLocation(0, p_begin));
            offset += TextUtilities.GetLineAsString(currentTab.textEditor.Document, p_begin).Length;
            currentTab.textEditor.Document.Insert(offset, copy_procbody);
            currentTab.textEditor.Document.UndoStack.EndUndoGroup();
            //
            if (sIndex > moveActive && !moveToEnd) sIndex--;
            TreeNode nd = ProcTree.Nodes[root].Nodes[moveActive];
            ProcTree.Nodes[root].Nodes.RemoveAt(moveActive);
            ProcTree.Nodes[root].Nodes.Insert(sIndex, nd);
            ProcTree.SelectedNode = ProcTree.Nodes[root].Nodes[sIndex];
            ProcTree.Focus();
            ProcTree.Select();
            Parser.UpdateProcInfo(ref currentTab.parseInfo, currentTab.textEditor.Text, currentTab.filepath);
            currentTab.textEditor.Document.FoldingManager.UpdateFoldings(currentTab.filename, currentTab.parseInfo);
            currentTab.textEditor.Document.FoldingManager.NotifyFoldingsChanged(null);
        }

        private void moveProcedureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ProcTree.SelectedNode == null) return;
            if (moveActive == -1) {
                moveActive = ProcTree.SelectedNode.Index;
                ProcTree.SelectedNode.ForeColor = Color.Red;
                ProcTree.Cursor = Cursors.Hand;
                ProcTree.AfterSelect -= TreeView_AfterSelect;
                ProcTree.SelectedNode = ProcTree.Nodes[0];
                ProcTree.AfterSelect += new TreeViewEventHandler(ProcTree_AfterSelect);
            }
        }

        private void ProcTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null || e.Node.Parent.Text != TREEPROCEDURES[1]) return;
            ProcTree.AfterSelect -= ProcTree_AfterSelect;
            currentTab.textEditor.TextChanged -= textChanged;
            MoveProcedure(e.Node.Index);
            currentTab.textEditor.TextChanged += textChanged;
            ProcTree.AfterSelect += TreeView_AfterSelect;
            ProcTree.SelectedNode.ForeColor = Color.Black;
            ProcTree.Cursor = Cursors.Default;
            moveActive = -1;
        }

        private void ProcTree_MouseLeave(object sender, EventArgs e)
        {
            if (moveActive != -1) {
                ProcTree.AfterSelect -= ProcTree_AfterSelect;
                ProcTree.AfterSelect += TreeView_AfterSelect;
                ProcTree.Nodes[ProcTree.Nodes.Count - 1].Nodes[moveActive].ForeColor = Color.Black;
                ProcTree.Cursor = Cursors.Default;
                moveActive = -1;
            }
        }

        private void ProcTree_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                ProcTree_MouseLeave(null, null);
            }
        }
        #endregion
    }
}
