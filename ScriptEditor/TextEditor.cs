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
using ScriptEditor.SyntaxRules;

using ScriptEditor.TextEditorUI;
using ScriptEditor.TextEditorUI.ToolTips;
using ScriptEditor.TextEditorUI.CompleteList;
using ScriptEditor.TextEditorUI.Nodes;

using ScriptEditor.TextEditorUtilities;

namespace ScriptEditor
{
    partial class TextEditor : Form
    {
        private const string SSE = AboutBox.appName + " - ";

        private const string unsaved = "unsaved.ssl";
        private const string treeTipProcedure = "\n\n - Click and hold Ctrl key to paste the procedure name into the script.\n - Double click to goto the procedure.";
        private const string treeTipVariable = "\n\n - Click and hold Ctrl key to paste the variable name into the script.\n - Double click to goto the variable.";

        private static readonly string[] TREEPROCEDURES = new string[] { "Global Procedures", "Local Procedures" };
        private static readonly string[] TREEVARIABLES = new string[] { "Global Variables", "Script Variables" };
        private static readonly System.Media.SoundPlayer DontFind = new System.Media.SoundPlayer(Properties.Resources.DontFind);
        private static readonly System.Media.SoundPlayer CompileFail = new System.Media.SoundPlayer(Properties.Resources.CompileError);

        private readonly List<TabInfo> tabs = new List<TabInfo>();
        private TabInfo currentTab;
        private ToolStripLabel parserLabel;

        private SearchForm sf;
        private GoToLine goToLine;

        private int previousTabIndex = -1;
        private int minimizelogsize;
        private PositionType PosChangeType;
        private int moveActive = -1;
        private int fuctionPanel = -1;
        private FormWindowState wState;
        private readonly string[] commandsArgs;
        private bool SplitEvent;

        private bool ctrlKeyPress;
        private bool dbClick;

        private int showTipsColumn;
        private bool roundTrip = false;

        internal TreeView VarTree = new TreeView();
        private TabPage VarTab = new TabPage("Variables");

        private AutoComplete autoComplete;

        /// <summary>
        /// Сокращенное свойство.
        /// Return: currentTab.textEditor.Document
        /// </summary>
        private IDocument currentDocument { get { return currentTab.textEditor.Document; } }

        /// <summary>
        /// Сокращенное свойство.
        /// Return: currentTab.textEditor.ActiveTextAreaControl
        /// </summary>
        private TextAreaControl currentActiveTextAreaCtrl { get { return currentTab.textEditor.ActiveTextAreaControl; } }

        private void EnableDoubleBuffering()
        {
           // Set the value of the double-buffering style bits to true.
           //this.SetStyle(ControlStyles.DoubleBuffer |
           //              ControlStyles.UserPaint |
           //              ControlStyles.AllPaintingInWmPaint,
           //              true);
           //this.UpdateStyles();

           Program.SetDoubleBuffered(panel1);
           Program.SetDoubleBuffered(dgvErrors);
        }

        #region Main form control
        public TextEditor(string[] args)
        {
            InitializeComponent();
            EnableDoubleBuffering();
            InitControlEvent();

            commandsArgs = args;
            Settings.SetupWindowPosition(SavedWindows.Main, this);

            if (!Settings.firstRun)
                WindowState = FormWindowState.Maximized;

            pDefineStripComboBox.Items.AddRange(File.ReadAllLines(Settings.PreprocDefPath));
            if (Settings.preprocDef != null)
                pDefineStripComboBox.Text = Settings.preprocDef;
            else
                pDefineStripComboBox.SelectedIndex = 0;
            SearchTextComboBox.Items.AddRange(File.ReadAllLines(Settings.SearchHistoryPath));
            SearchToolStrip.Visible = false;
            defineToolStripMenuItem.Checked = Settings.allowDefine;
            msgAutoOpenEditorStripMenuItem.Checked = Settings.openMsgEditor;
            showTabsAndSpacesToolStripMenuItem.Checked = Settings.showTabsChar;
            trailingSpacesToolStripMenuItem.Checked = Settings.autoTrailingSpaces;
            showIndentLineToolStripMenuItem.Checked = Settings.showVRuler;
            decompileF1ToolStripMenuItem.Checked = Settings.decompileF1;
            saveUTF8ToolStripMenuItem.Checked = Settings.saveScriptUTF8;
            win32RenderTextToolStripMenuItem.Checked = Settings.winAPITextRender;
            oldDecompileToolStripMenuItem.Checked = Settings.oldDecompile;
            SizeFontToString();

            toolTips.Active = false;
            toolTips.Draw += delegate(object sender, DrawToolTipEventArgs e) { TipPainter.DrawInfo(e); };

            if (Settings.encoding == (byte)EncodingType.OEM866) {
                EncodingDOSmenuItem.Checked = true;
                windowsDefaultMenuItem.Checked = false;
            }

            // Highlighting
            FileSyntaxModeProvider fsmProvider = new FileSyntaxModeProvider(SyntaxFile.SyntaxFolder); // Create new provider with the highlighting directory.
            HighlightingManager.Manager.AddSyntaxModeFileProvider(fsmProvider); // Attach to the text editor.
            ColorTheme.InitTheme(Settings.highlight == 2, this);

            autoComplete = new AutoComplete(panel1, Settings.autocompleteColor);

            // Recent files
            UpdateRecentList();

            // Templates
            foreach (string file in Directory.GetFiles(Path.Combine(Settings.ResourcesFolder, "templates"), "*.ssl"))
            {
                ToolStripMenuItem mi = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(file));
                mi.Tag = file;
                mi.Click += new EventHandler(Template_Click); // Open Templates file
                New_toolStripDropDownButton.DropDownItems.Add(mi);
            }

            if (Settings.pathHeadersFiles == null)
                Headers_toolStripSplitButton.Enabled = false;

            HandlerProcedure.CreateProcHandlers(ProcMnContext, this);
            Functions.CreateTree(FunctionsTree);
            ProgramInfo.LoadOpcodes();

            DontFind.LoadAsync();
            CompileFail.LoadAsync();

            this.Text += " v." + AboutBox.appVersion;
            tbOutput.Text = "***** " +  AboutBox.appName + " v." + AboutBox.appVersion + AboutBox.appDescription + " *****";
        }

#if !DEBUG
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == SingleInstanceManager.WM_SFALL_SCRIPT_EDITOR_OPEN) {
                TabInfo result = null;
                var commandLineArgs = SingleInstanceManager.LoadCommandLine();
                foreach (var fArg in commandLineArgs)
                {
                    string file = fArg;
                    bool fcd = FileAssociation.CheckFCDFile(ref file);
                    if (file != null)
                        result = Open(file, OpenType.File, commandline: true, fcdOpen: fcd);

                }
                if (result != null && !this.Focused)
                    ShowMe();
            }
            base.WndProc(ref m);
        }

        // activate form only for open ssl file
        private void ShowMe()
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = wState;
            Activate();
            // get our current "TopMost" value (ours will always be false though)
            //bool top = TopMost;
            // make our form jump to the top of everything
            //TopMost = true;
            // set it back to whatever it was
            //TopMost = top;
        }
#else
        private void ShowMe() {}
#endif

#if TRACE
    void DEBUGINFO(string line) { tbOutput.Text = line + "\r\n" + tbOutput.Text; }
#else
    void DEBUGINFO(string line) { }
#endif

        private void TextEditor_Load(object sender, EventArgs e)
        {
            splitContainer3.Panel1Collapsed = true;
            splitContainer2.Panel2Collapsed = true;
            splitContainer1.Panel2Collapsed = true;
            splitContainer2.Panel1MinSize = 300;
            splitContainer2.Panel2MinSize = 150;
            splitContainer1.SplitterDistance = Size.Height;

            if (Settings.editorSplitterPosition == -1)
                minimizelogsize = Size.Height - (Size.Height / 5);
            else
                minimizelogsize = Settings.editorSplitterPosition;

            if (Settings.editorSplitterPosition2 != -1)
                splitContainer2.SplitterDistance = Settings.editorSplitterPosition2;
            else
                splitContainer2.SplitterDistance = Size.Width - 200;

            showLogWindowToolStripMenuItem.Checked = Settings.showLog;
            if (Settings.enableParser)
                CreateTabVarTree();
        }

        private void TextEditor_Shown(object sender, EventArgs e)
        {
            if (!Settings.firstRun)
                Settings_ToolStripMenuItem.PerformClick();

            // open documents passed from command line
            foreach (string fArg in commandsArgs)
            {
                string file = fArg;
                bool fcd = FileAssociation.CheckFCDFile(ref file);
                if (file != null)
                    Open(file, TextEditor.OpenType.File, commandline: true, fcdOpen: fcd);
            }

            this.Activated += TextEditor_Activated;
            this.Deactivate += TextEditor_Deactivate;
            SingleInstanceManager.SendEditorOpenMessage();
        }

        private void TextEditor_Resize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                wState = WindowState;

            if (autoComplete != null)
                autoComplete.Close();
        }

        private void TextEditor_Deactivate(object sender, EventArgs e)
        {
            if (currentTab == null) return;
            currentActiveTextAreaCtrl.TextArea.MouseEnter -= TextArea_SetFocus;
            ctrlKeyPress = false;
        }

        private void TextEditor_Activated(object sender, EventArgs e)
        {
            if (currentTab == null) return;
            currentActiveTextAreaCtrl.TextArea.MouseEnter += TextArea_SetFocus;

            if (WindowState != FormWindowState.Minimized)
                CheckChandedFile();
            else {
                Timer timer = new Timer();
                timer.Interval = 500; // interval time - 0.5 sec
                timer.Tick += delegate(object obj, EventArgs eArg) {
                    timer.Stop();
                    timer.Dispose();
                    CheckChandedFile();
                };
                timer.Start();
            }
            if ((Control.ModifierKeys & Keys.Control) != 0) ctrlKeyPress = true;
        }

        private void TextEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < tabs.Count; i++) {
                bool skip = tabs[i].changed;
                if (tabs[i].changed) {
                    switch (MessageBox.Show("Save changes to " + tabs[i].filename + "?", "Message", MessageBoxButtons.YesNoCancel)) {
                        case DialogResult.Yes:
                            Save(tabs[i], true);
                            if (tabs[i].changed) {
                                e.Cancel = true;
                                return;
                            }
                            break;
                        case DialogResult.No:
                            break;
                        default:
                            e.Cancel = true;
                            return;
                    }
                }
                KeepScriptSetting(tabs[i], skip);
            }

            if (sf != null) sf.Close();

            while (bwSyntaxParser.IsBusy) {
                System.Threading.Thread.Sleep(100); // Avoid stomping on files while the parser is running
                Application.DoEvents();
            }

            splitContainer3.Panel1Collapsed = true;
            int dist = this.Height - (this.Height / 4) + 100;
            Settings.editorSplitterPosition = (splitContainer1.SplitterDistance < dist) ? splitContainer1.SplitterDistance : -1;
            Settings.editorSplitterPosition2 = splitContainer2.SplitterDistance;
            Settings.SaveSettingData(this);
            SyntaxFile.DeleteSyntaxFile();
        }
        #endregion

        private void UpdateRecentList()
        {
            string[] items = Settings.GetRecent();
            int count = Open_toolStripSplitButton.DropDownItems.Count-1;
            for (int i = 3; i <= count; i++) {
                Open_toolStripSplitButton.DropDownItems.RemoveAt(3);
            }
            for (int i = items.Length - 1; i >= 0; i--) {
                Open_toolStripSplitButton.DropDownItems.Add(items[i], null, recentItem_Click);
            }
        }

        public void SetFocusDocument()
        {
            TextArea_SetFocus(null, null);
        }

        public enum OpenType { None, File, Text }

        public TabInfo Open(string file, OpenType type, bool addToMRU = true, bool alwaysNew = false, bool recent = false,
                            bool seltab = true, bool commandline = false, bool fcdOpen = false, bool alreadyOpen = true, bool outputFolder = false)
        {
            if (type == OpenType.File) {
                if (!Path.IsPathRooted(file))
                    file = Path.GetFullPath(file);

                if (commandline && Path.GetExtension(file).ToLower() == ".msg") {
                    if (currentTab == null)
                        wState = FormWindowState.Minimized;
                    MessageEditor.MessageEditorOpen(file, this).SendMsgLine += AcceptMsgLine;
                    return null;
                }
                // Check file
                bool Exists;
                if (!FileAssociation.CheckFileAllow(file, out Exists))
                    return null;
                //Add this file to the recent files list
                if (addToMRU) {
                    if (!Exists && recent && MessageBox.Show("This recent file not found. Delete recent link to file?", "Open file error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        recent = true;
                    else
                        recent = false; // don't delete file link from recent list
                    Settings.AddRecentFile(file, recent);
                    UpdateRecentList();
                }
                if (!Exists)
                    return null;
                //If this is an int, decompile
                if (string.Compare(Path.GetExtension(file), ".int", true) == 0) {
                    if (!this.Focused)
                        ShowMe();
                    string decomp = new Compiler(roundTrip).Decompile(file, outputFolder);
                    if (decomp == null) {
                        MessageBox.Show("Decompilation of '" + file + "' was not successful", "Error");
                        return null;
                    } else {
                        file = decomp;
                        // fix for procedure begin
                        ParserInternal.FixProcedureBegin(file);
                    }
                } else {
                    //Check if the file is already open
                    var tab = CheckTabs(tabs, file);
                    if (tab != null) {
                        if (seltab)
                            tabControl1.SelectTab(tab.index);
                        ShowMe();
                        if (!alreadyOpen || MessageBox.Show("This file is already open!\nDo you want to open another one same file?", "Question",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            return tab;
                    }
                }
            }
            //Create the text editor and set up the tab
            ICSharpCode.TextEditor.TextEditorControl te = new ICSharpCode.TextEditor.TextEditorControl();

            te.TextEditorProperties.LineViewerStyle = LineViewerStyle.FullRow;
            te.TextEditorProperties.TabIndent = Settings.tabSize;
            te.TextEditorProperties.IndentationSize = Settings.tabSize;
            te.TextEditorProperties.ShowTabs = Settings.showTabsChar;
            te.TextEditorProperties.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            te.TextEditorProperties.NativeDrawText = Settings.winAPITextRender;
            te.TextEditorProperties.DarkScheme = ColorTheme.IsDarkTheme;

            if (type == OpenType.File && String.Compare(Path.GetExtension(file), ".msg", true) == 0) {
                te.Document.TextEditorProperties.Encoding = Settings.EncCodePage;
                te.SetHighlighting(ColorTheme.IsDarkTheme ? "MessageDark": "Message");
                te.TextEditorProperties.EnableFolding = false;
                te.TextEditorProperties.ConvertTabsToSpaces = false;
                te.TextEditorProperties.ShowVerticalRuler = false;
                te.TextEditorProperties.IndentStyle = IndentStyle.None;
                te.TextEditorProperties.ShowLineNumbers = false;
                te.TextEditorProperties.Font = new Font("Verdana", 10 + Settings.sizeFont, FontStyle.Regular, GraphicsUnit.Point);
            } else {
                te.SetHighlighting(ColorTheme.HighlightingScheme); // Activate the highlighting, use the name from the SyntaxDefinition node.
                te.Document.FoldingManager.FoldingStrategy = new CodeFolder();
                te.TextEditorProperties.ConvertTabsToSpaces = Settings.tabsToSpaces;
                te.TextEditorProperties.ShowSpaces = Settings.showTabsChar;
                te.TextEditorProperties.IndentStyle = IndentStyle.Smart;
                te.TextEditorProperties.ShowVerticalRuler = Settings.showVRuler;
                te.TextEditorProperties.VerticalRulerRow = Settings.tabSize;
                te.TextEditorProperties.AllowCaretBeyondEOL = true;
                //te.TextEditorProperties.CaretLine = true;
                Settings.SetTextAreaFont(te);
            }

            if (type == OpenType.File)
                te.LoadFile(file, false, true);
            else if (type == OpenType.Text)
                te.Text = file;

            // set tabinfo
            TabInfo ti = new TabInfo();
            ti.index = tabControl1.TabCount;
            ti.history.linePosition = new List<TextLocation>();
            ti.history.pointerCur = -1;
            ti.textEditor = te;

            bool createNew = false;
            if (type == OpenType.None) { // only for new create script
                sfdScripts.FileName = "NewScript";
                if (sfdScripts.ShowDialog() == DialogResult.OK) {
                    file = sfdScripts.FileName;
                    type = OpenType.File;
                    ti.changed = true;
                    te.Text = Properties.Resources.newScript;
                } else
                    return null;
                createNew = true;
            } //else
              //  ti.changed = false;

            if (type == OpenType.File ) { //&& !alwaysNew
                if (alwaysNew) {
                    string temp = Path.Combine(Settings.scriptTempPath, unsaved);
                    File.Copy(file, temp, true);
                    file = temp;
                }
                ti.filepath = file;
                ti.filename = Path.GetFileName(file);
            } else {
                ti.filepath = null;
                ti.filename = unsaved;
            }

            tabs.Add(ti);
            TabPage tp = new TabPage(ti.filename);
            tp.ImageIndex = (ti.changed) ? 1 : 0;
            tp.Controls.Add(te);
            te.Dock = DockStyle.Fill;
            tabControl1.TabPages.Add(tp);
            if (tabControl1.TabPages.Count == 1)
                EnableFormControls();
            if (type == OpenType.File) {
                if (!alwaysNew)
                    tp.ToolTipText = ti.filepath;
                string ext = Path.GetExtension(file).ToLower();
                if (ext == ".ssl" || ext == ".h") {
                    if (formatCodeToolStripMenuItem.Checked)
                        te.Text = Utilities.FormattingCode(te.Text);
                    ti.shouldParse = true;
                    //ti.needsParse = true; // set 'true' only edit text

                    FirstParseScript(ti); // First Parse

                    if (!createNew && Settings.storeLastPosition) {
                        int pos = Settings.GetLastScriptPosition(ti.filename.ToLowerInvariant());
                        te.ActiveTextAreaControl.Caret.Line = pos;
                        te.ActiveTextAreaControl.CenterViewOn(pos, -1);
                    }
                    if (Settings.autoOpenMsgs && ti.filepath != null)
                        AssociateMsg(ti, false);
                }
                ti.FileTime = File.GetLastWriteTime(ti.filepath);
            }
            te.OptionsChanged();
            // TE events
            te.TextChanged += textChanged;
            SetActiveAreaEvents(te);
            te.ContextMenuStrip = editorMenuStrip;
            //
            if (tabControl1.TabPages.Count > 1) {
                if (seltab)
                    tabControl1.SelectTab(tp);
            } else
                tabControl1_Selected(null, null);

            if (fcdOpen)
                dialogNodesDiagramToolStripMenuItem_Click(null, null);

            return ti;
        }

        private void CheckChandedFile()
        {
            if (!currentTab.CheckFileTime()) {
                this.Activated -= TextEditor_Activated;
                DialogResult result = MessageBox.Show(currentTab.filepath +
                                                      "\nThe script file was changed outside the editor." +
                                                      "\nDo you want to update the script file?",
                                                      "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    currentTab.FileTime = File.GetLastWriteTime(currentTab.filepath);
                    int caretLine = currentActiveTextAreaCtrl.Caret.Line;
                    int scrollValue = currentActiveTextAreaCtrl.VScrollBar.Value;
                    currentTab.textEditor.BeginUpdate();
                    currentTab.textEditor.LoadFile(currentTab.filepath, false, true);
                    currentActiveTextAreaCtrl.VScrollBar.Value = scrollValue;
                    currentActiveTextAreaCtrl.Caret.Line = caretLine;
                    currentTab.textEditor.EndUpdate();

                    currentTab.changed = false;
                    SetTabTextChange(currentTab.index);
                } else
                    currentTab.FileTime = File.GetLastWriteTime(currentTab.filepath);
                this.Activated += TextEditor_Activated;
            }
        }

        private void Save(TabInfo tab, bool close = false)
        {
            if (tab != null) {
                if (tab.filepath == null) {
                    SaveAs(tab, close);
                    return;
                }
                while (bwSyntaxParser.IsBusy) {
                    System.Threading.Thread.Sleep(50); // Avoid stomping on files while the parser is running
                    Application.DoEvents();
                }

                bool msg = (Path.GetExtension(tab.filename) == ".msg");

                if (Settings.autoTrailingSpaces && !msg) {
                    new ICSharpCode.TextEditor.Actions.RemoveTrailingWS().Execute(currentActiveTextAreaCtrl.TextArea);
                }
                if (close && tab.textEditor.Document.FoldingManager.FoldMarker.Count > 0) {
                    CodeFolder.SetProceduresCollapsed(tab.textEditor.Document, tab.filename);
                }
                string saveText = tab.textEditor.Text;
                if (msg && Settings.EncCodePage.CodePage == 866) {
                    saveText = saveText.Replace('\u0425', '\u0058'); //Replacement russian letter "X", to english letter
                }
                Utilities.NormalizeDelimiter(ref saveText);

                File.WriteAllText(tab.filepath, saveText, msg ? Settings.EncCodePage
                                                              : (Settings.saveScriptUTF8) ? new UTF8Encoding(false)
                                                                                          : Encoding.Default);
                if (!close) tab.FileTime = File.GetLastWriteTime(tab.filepath);
                tab.changed = false;
                SetTabTextChange(tab.index);
            }

        }

        private void SaveAs(TabInfo tab, bool close = false)
        {
            if (tab == null)
                return;

            switch (Path.GetExtension(tab.filename).ToLower()) {
                case ".ssl":
                    sfdScripts.FilterIndex = 1;
                    break;
                case ".h":
                    sfdScripts.FilterIndex = 2;
                    break;
                case ".msg":
                    sfdScripts.FilterIndex = 3;
                    break;
                default:
                    sfdScripts.FilterIndex = 4;
                    break;
            }
            sfdScripts.FileName = tab.filename;

            if (sfdScripts.ShowDialog() == DialogResult.OK) {
                tab.filepath = sfdScripts.FileName;
                tab.filename = Path.GetFileName(tab.filepath);
                tabControl1.TabPages[tab.index].Text = tabs[tab.index].filename;
                tabControl1.TabPages[tab.index].ToolTipText = tabs[tab.index].filepath;
                Save(tab, close);
                Settings.AddRecentFile(tab.filepath);
                string ext = Path.GetExtension(tab.filepath).ToLower();
                if (Settings.enableParser && (ext == ".ssl" || ext == ".h")) {
                    tab.shouldParse = true;
                    tab.needsParse = true;
                    parserLabel.Text = "Parser: Wait for update";
                    ParseScript();
                }
                this.Text = SSE + tab.filepath + ((pDefineStripComboBox.SelectedIndex > 0) ? " [" + pDefineStripComboBox.Text + "]" : "");
            }
        }

        private void Close(TabInfo tab)
        {
            if (tab == null | tab.index == -1)
                return;

            int i = tab.index;
            var tag = tabControl1.TabPages[i].Tag;
            if (tag != null)
                ((NodeDiagram)tag).Close(); //also close diagram editor

            while (tab.nodeFlowchartTE.Count > 0)
                tab.nodeFlowchartTE[0].CloseEditor(true);

            bool skip = tab.changed; // если изменен, то пропустить сохранение состояний Folds в методе KeepScriptSetting
            if (tab.changed) {
                switch (MessageBox.Show("Save changes to " + tab.filename + "?", "Message", MessageBoxButtons.YesNoCancel)) {
                    case DialogResult.Yes:
                        Save(tab, true);
                        if (tab.changed)
                            return;
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        return;
                }
            }

            KeepScriptSetting(tab, skip);

            if (tabControl1.TabPages.Count > 2 && i == tabControl1.SelectedIndex) {
                if (previousTabIndex != -1) {
                    tabControl1.SelectedIndex = previousTabIndex;
                } else {
                    tabControl1.SelectedIndex = tabControl1.TabCount - 2;
                }
            }
            tabControl1.TabPages.RemoveAt(i);
            tabs.RemoveAt(i);
            for (int j = i; j < tabs.Count; j++)
                tabs[j].index--;
            for (int j = 0; j < tabs.Count; j++) {
                if (tabs[j].msgFileTab == tab) {
                    tabs[j].msgFileTab = null;
                    tabs[j].messages.Clear();
                }
            }
            tab.index = -1;
            if (tabControl1.TabPages.Count == 1) {
                tabControl1_Selected(null, null);
            }
        }

        private void KeepScriptSetting(TabInfo tab, bool skip)
        {
            if (!skip && tab.filepath != null && tab.textEditor.Document.FoldingManager.FoldMarker.Count > 0) {
                CodeFolder.SetProceduresCollapsed(tab.textEditor.Document, tab.filename);
            }
            // store last script position
            if (Path.GetExtension(tab.filepath).ToLowerInvariant() == ".ssl" && tab.filename != unsaved)
                Settings.SetLastScriptPosition(tab.filename.ToLowerInvariant(), tab.textEditor.ActiveTextAreaControl.Caret.Line);
        }

        private void AssociateMsg(TabInfo tab, bool create)
        {
            if (tab.filepath == null || tab.msgFileTab != null)
                return;

            if (Settings.autoOpenMsgs && msgAutoOpenEditorStripMenuItem.Checked && !create) {
                MessageEditor.MessageEditorInit(tab, this);
                Focus();
            } else {
                string path;
                if (MessageFile.GetAssociatePath(tab, create, out path)) {
                    tab.msgFilePath = path;
                    tab.msgFileTab = Open(tab.msgFilePath, OpenType.File, false);
                }
            }
        }

        private bool Compile(TabInfo tab, out string msg, bool showMessages = true, bool preprocess = false, bool showIcon = true)
        {
            msg = String.Empty;
            if (string.Compare(Path.GetExtension(tab.filename), ".ssl", true) != 0) {
                if (showMessages) MessageBox.Show("You cannot compile this file.", "Compile Error");
                return false;
            }
            if (!Settings.ignoreCompPath && !preprocess && Settings.outputDir == null) {
                if (showMessages) MessageBox.Show("No output path selected.\nPlease select your scripts directory before compiling", "Compile Error");
                return false;
            }
            if (tab.changed) Save(tab);
            if (tab.changed || tab.filepath == null) return false;

            bool success = new Compiler(roundTrip).Compile(tab.filepath, out msg, tab.buildErrors, preprocess, tab.parseInfo.ShortCircuitEvaluation);

            foreach (ErrorType et in new ErrorType[] { ErrorType.Error, ErrorType.Warning, ErrorType.Message })
            {
                foreach (Error e in tab.buildErrors)
                {
                    if (e.type == et) {
                        dgvErrors.Rows.Add(e.type.ToString(), Path.GetFileName(e.fileName), e.line, e);
                        if (et == ErrorType.Error) dgvErrors.Rows[dgvErrors.Rows.Count - 1].Cells[0].Style.ForeColor = Color.Red;
                    }
                }
            }

            if (dgvErrors.RowCount > 0) dgvErrors.Rows[0].Cells[0].Selected = false;

            if (preprocess) return success;

            if (!success) {
                parserLabel.Text = "Failed to compiled: " + tab.filename;
                parserLabel.ForeColor = Color.Firebrick;
                msg += "\r\n Compilation Failed! (See the output build and errors window log for details).";
                CompileFail.Play();

                if (showMessages) {
                    if (Settings.warnOnFailedCompile) {
                        tabControl2.SelectedIndex = 2 - Convert.ToInt32(Settings.userCmdCompile);
                        maximize_log();
                    }// else
                     //   new CompiledStatus(false, this).ShowCompileStatus();
                }
            } else {
                if (showMessages && showIcon)
                    new CompiledStatus(true, this).ShowCompileStatus();
                parserLabel.Text = "Compiled: " + tab.filename + " at " + DateTime.Now.ToString("HH:mm:ss");
                parserLabel.ForeColor = Color.DarkGreen;
                msg += "\r\n Compilation Successfully!";
            }
            return success;
        }

        // Called when creating a new document and when switching tabs
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            // останавливаем таймеры парсеров
            intParserTimer.Stop();
            extParserTimer.Stop();

            if (tabControl1.SelectedIndex == -1) {
                currentTab = null;
                parserLabel.Text = (Settings.enableParser) ? "Parser: No file" : parseoff;
                SetFormControlsOff();
            } else {
                if (currentTab != null) {
                    previousTabIndex = currentTab.index;
                }
                currentTab = tabs[tabControl1.SelectedIndex];
                //if (!Settings.enableParser && currentTab.parseInfo != null)
                //    currentTab.parseInfo.parseData = false;

                if (currentTab.msgFileTab != null)
                    MessageFile.ParseMessages(currentTab);

                // Create or Delete Variable treeview
                if (!Settings.enableParser && tabControl3.TabPages.Count > 2) {
                    if (currentTab.parseInfo != null) {
                        if (!currentTab.parseInfo.parseData) {
                            tabControl3.TabPages.RemoveAt(1);
                        }
                    } else {
                        tabControl3.TabPages.RemoveAt(1);
                    }
                } else if (tabControl3.TabPages.Count < 3 && (Settings.enableParser || currentTab.parseInfo != null)) {
                    if (currentTab.parseInfo != null && currentTab.parseInfo.parseData) {
                        CreateTabVarTree();
                    }
                }
                if (currentTab.shouldParse) {
                    if (Settings.enableParser && currentTab.parseInfo.parseError && !currentTab.needsParse) {
                        parserLabel.Text = "Parser: Parsing script error (see parser errors log)";
                    } else
                    if (currentTab.needsParse) {
                        parserLabel.Text = (Settings.enableParser) ? "Parser: Waiting to update..." : parseoff;
                        // Update parse info
                        ParseScript();
                    } else
                        parserLabel.Text = (Settings.enableParser) ? "Parser: Idle" : parseoff;
                } else
                    parserLabel.Text = (Settings.enableParser) ? "Parser: Not an SSL file" : parseoff;

                UpdateLog();
                currentHighlightProc = null;
                UpdateNames(true);
                // text editor set focus
                currentActiveTextAreaCtrl.Select();
                ControlFormStateOn_Off();
                this.Text = SSE + currentTab.filepath + ((pDefineStripComboBox.SelectedIndex > 0) ? " [" + pDefineStripComboBox.Text + "]" : "");

                if (sender != null) CheckChandedFile();
            }
        }

        private void UpdateLog()
        {
            if (autoRefreshToolStripMenuItem.Checked)
                OutputErrorLog(currentTab);
            else {
                if (Settings.enableParser)
                    tbOutputParse.Text = currentTab.parserLog;

                if (currentTab.buildLog != null)
                    tbOutput.Text = currentTab.buildLog;
            }
        }

        #region Tree browser control
        private void CreateTabVarTree() { tabControl3.TabPages.Insert(1, VarTab); }

        private enum TreeStatus { idle, update, local }
        internal static Procedure currentHighlightProc = null;
        private  static TreeNode currentHighlightNode = null;
        private bool updateHighlightPocedure = true;

        // подсветить процедуру в дереве
        private void HighlightCurrentPocedure(int curLine)
        {
            Procedure proc;
            if (curLine == -2) {
                proc = currentHighlightProc;
                currentHighlightProc = null;
                currentHighlightNode = null;
            } else {
                proc = currentTab.parseInfo.GetProcedurePosition(curLine);
            }
            if (proc != null && proc != currentHighlightProc) {
                if (currentHighlightProc != null && currentHighlightProc.name.Equals(proc.name, StringComparison.OrdinalIgnoreCase)) return;
                TreeNodeCollection nodes;
                if (ProcTree.Nodes.Count > 1)
                    nodes = ProcTree.Nodes[1].Nodes;
                else
                    nodes = ProcTree.Nodes[0].Nodes; // for parser off
                foreach (TreeNode node in nodes)
                {
                    string name = ((Procedure)node.Tag).name;
                    if (name == proc.name) {
                        node.Text = node.Text.Insert(0, "► ");
                        node.ForeColor = ColorTheme.HighlightProcedureTree;
                        if (currentHighlightNode != null) {
                            currentHighlightNode.ForeColor = ProcTree.ForeColor;
                            currentHighlightNode.Text = currentHighlightNode.Text.Substring(2);
                        }
                        currentHighlightProc = proc;
                        currentHighlightNode = node;
                        break;
                    }
                }
            } else if (currentHighlightProc != null && currentHighlightProc != proc) {
                currentHighlightNode.Text = currentHighlightNode.Text.Substring(2);
                currentHighlightNode.ForeColor = ProcTree.ForeColor;
                currentHighlightProc = null;
                currentHighlightNode = null;
            }
        }

        // Create names for procedures and variables in treeview
        private void UpdateNames(bool newCreate = false)
        {
            if (currentTab == null || !currentTab.shouldParse || currentTab.parseInfo == null) return;

            object selectedNode = null;
            if (ProcTree.SelectedNode != null)
                selectedNode = ProcTree.SelectedNode.Tag;

            ProcTree.Tag = TreeStatus.update;

            string scrollNode = null;
            if (!newCreate && ProcTree.Nodes.Count != 0) {
                for (int i = ProcTree.Nodes.Count -1; i >= 0; i--)
                {
                    if (!ProcTree.Nodes[i].IsExpanded) continue;
                    for (int j = ProcTree.Nodes[i].Nodes.Count - 1; j >= 0; j--)
                    {
                        if (ProcTree.Nodes[i].Nodes[j].IsVisible) {
                            scrollNode = ProcTree.Nodes[i].Nodes[j].Name;
                            break;
                        }
                    }
                    if (scrollNode != null) break;
                }
            }
            ProcTree.BeginUpdate();
            ProcTree.Nodes.Clear();

            TreeNode rootNode;
            foreach (var s in TREEPROCEDURES) {
                rootNode = ProcTree.Nodes.Add(s, s);
                rootNode.ForeColor = Color.DodgerBlue;
                rootNode.NodeFont = new Font("Arial", 9F, FontStyle.Bold, GraphicsUnit.Point);
            }
            ProcTree.Nodes[0].ToolTipText = "Procedures declared and located in headers files." + treeTipProcedure;
            ProcTree.Nodes[0].Tag = 0; // global tag
            ProcTree.Nodes[1].ToolTipText = "Procedures declared and located in this script." + treeTipProcedure;
            ProcTree.Nodes[1].Tag = 1; // local tag

            foreach (Procedure p in currentTab.parseInfo.procs) {
                if (!Settings.enableParser && p.d.end == -1)
                    continue; //skip imported or broken procedures
                TreeNode tn = new TreeNode((!ViewArgsStripButton.Checked)? p.name : p.ToString(false));
                tn.Name = p.name;
                tn.Tag = p;
                foreach (Variable var in p.variables) {
                    TreeNode tn2 = new TreeNode(var.name);
                    tn2.Name = var.name;
                    tn2.Tag = var;
                    tn2.ToolTipText = var.ToString();
                    tn.Nodes.Add(tn2);
                }
                if (p.filename.ToLower() != currentTab.filename.ToLower() || p.IsImported) {
                    tn.ToolTipText = p.ToString() + "\ndeclarate file: " + p.filename;
                    ProcTree.Nodes[0].Nodes.Add(tn);
                    ProcTree.Nodes[0].Expand();
                } else {
                    tn.ToolTipText = p.ToString();
                    ProcTree.Nodes[1].Nodes.Add(tn);
                    ProcTree.Nodes[1].Expand();
                }
            }

            if (!Settings.enableParser && !currentTab.parseInfo.parseData) {
                ProcTree.Nodes.RemoveAt(0);
                if (tabControl3.TabPages.Count > 2) // удалить и вкладку если отсутсвует информация
                    tabControl3.TabPages.RemoveAt(1);
            } else {
                VarTree.BeginUpdate();
                VarTree.Nodes.Clear();

                foreach (var s in TREEVARIABLES) {
                    rootNode = VarTree.Nodes.Add(s);
                    rootNode.ForeColor = Color.DodgerBlue;
                    rootNode.NodeFont = new Font("Arial", 9F, FontStyle.Bold, GraphicsUnit.Point);
                }
                VarTree.Nodes[0].ToolTipText = "Variables declared and located in headers files." + treeTipVariable;
                VarTree.Nodes[1].ToolTipText = "Variables declared and located in this script." + treeTipVariable;

                foreach (Variable var in currentTab.parseInfo.vars) {
                    TreeNode tn = new TreeNode(var.name);
                    tn.Tag = var;
                    if (var.filename.ToLower() != currentTab.filename.ToLower()) {
                        tn.ToolTipText = var.ToString() + "\ndeclarate file: " + var.filename;
                        VarTree.Nodes[0].Nodes.Add(tn);
                        VarTree.Nodes[0].Expand();
                    } else {
                        tn.ToolTipText = var.ToString();
                        VarTree.Nodes[1].Nodes.Add(tn);
                        VarTree.Nodes[1].Expand();
                    }
                }
                if (VarTree.Nodes[0].Nodes.Count == 0) VarTree.Nodes[0].ForeColor = Color.Gray;
                if (VarTree.Nodes[1].Nodes.Count == 0) VarTree.Nodes[1].ForeColor = Color.Gray;

                foreach (TreeNode node in VarTree.Nodes)
                    SetNodeCollapseStatus(node);

                VarTree.EndUpdate();
            }
            foreach (TreeNode node in ProcTree.Nodes)
                SetNodeCollapseStatus(node);

            if (ProcTree.Nodes[0].Nodes.Count == 0) ProcTree.Nodes[0].ForeColor = Color.Gray;
            if (ProcTree.Nodes.Count > 1) {
                if (ProcTree.Nodes[1].Nodes.Count == 0)
                    ProcTree.Nodes[1].ForeColor = Color.Gray;
                //ProcTree.Nodes[1].EnsureVisible();
            }

            if (selectedNode != null) {
                TreeNode[] nodes = null;
                if (selectedNode is Procedure)
                    nodes = ProcTree.Nodes.Find(((Procedure)selectedNode).name, true);
                else if (selectedNode is Variable)
                    nodes = ProcTree.Nodes.Find(((Variable)selectedNode).name, true);
                if (nodes != null && nodes.Length > 0)
                    ProcTree.SelectedNode = nodes[0];
            }
            ProcTree.EndUpdate();
            HighlightCurrentPocedure((currentHighlightProc == null) ? currentActiveTextAreaCtrl.Caret.Line : -2);

            // scroll to node
            if (scrollNode != null) {
                foreach (TreeNode nodes in ProcTree.Nodes) {
                    foreach (TreeNode node in nodes.Nodes) {
                        if (node.Name == scrollNode) {
                            if (node.PrevNode != null) node.PrevNode.EnsureVisible();
                            scrollNode = null;
                            break;
                        }
                    }
                    if (scrollNode == null) break;
                }
                if (scrollNode != null && currentHighlightNode != null) currentHighlightNode.EnsureVisible();
            }
            ProcTree.Tag = TreeStatus.idle;
        }

        private string GetCorrectNodeKeyName(TreeNode node)
        {
            string nodeKey = node.FullPath;
            int n = nodeKey.IndexOf('\\');
            if (n != -1) nodeKey = nodeKey.Remove(n + 1) + node.Name;
            return nodeKey;
        }

        private void SetNodeCollapseStatus(TreeNode node)
        {
            string nodeKey = GetCorrectNodeKeyName(node);
            if (currentTab.treeExpand.ContainsKey(nodeKey)) {
                    if (currentTab.treeExpand[nodeKey])
                        node.Collapse();
                    else
                        node.Expand();
            }
            foreach (TreeNode nd in node.Nodes) SetNodeCollapseStatus(nd);
        }

        bool treeExpandCollapse = false;
        private void TreeExpandCollapse(TreeViewEventArgs e)
        {
            TreeNode tn = e.Node;
            if (tn == null) return;

            bool collapsed = (e.Action == TreeViewAction.Collapse);
            string nodeKey = GetCorrectNodeKeyName(tn);
            if (!currentTab.treeExpand.ContainsKey(nodeKey))
                currentTab.treeExpand.Add(nodeKey, collapsed);
            else
                currentTab.treeExpand[nodeKey] = collapsed;
            if (tn.Parent == null) treeExpandCollapse = true;
        }

        private void TreeView_DClickMouse(object sender, MouseEventArgs e) {
            if (e.X <= 20) return;
            TreeNode node = (!treeExpandCollapse) ? ((TreeView)sender).GetNodeAt(e.Location) : null;
            treeExpandCollapse = false;
            if (node != null) TreeView_ClickBehavior(node);
        }

        // Click on node tree Procedures/Variables
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!ctrlKeyPress || e.Action == TreeViewAction.Unknown) return;
            TreeView_ClickBehavior(e.Node);
        }

        private void TreeView_ClickBehavior(TreeNode node)
        {
            string file = null, name = null;
            int line = 0;
            bool pSelect = false;
            if (node.Tag is Variable) {
                Variable var = (Variable)node.Tag;
                if (!ctrlKeyPress) {
                    file = var.fdeclared;
                    line = var.d.declared;
                } else {
                    name = var.name;
                }
            } else if (node.Tag is Procedure) {
                Procedure proc = (Procedure)node.Tag;
                if (!ctrlKeyPress) {
                    file = proc.fstart;
                    line = proc.d.start;
                    if (line == -1 || file == null) { // goto declared
                        file = proc.fdeclared;
                        line = proc.d.declared;
                    }
                    pSelect = true;
                } else {
                    name = proc.name;
                }
            }
            if (file != null) {
                SelectLine(file, line, pSelect);
            } else if (name != null) {
                Utilities.InsertText(name, currentActiveTextAreaCtrl);
            }
        }

        void Tree_AfterExpandCollapse(object sender, TreeViewEventArgs e)
        {
            if ((TreeStatus)ProcTree.Tag == TreeStatus.idle)
                TreeExpandCollapse(e);
        }

        private void ProcTree_Leave(object sender, EventArgs e)
        {
            ProcTree.SelectedNode = null;
        }

        private void ProcTree_MouseDown(object sender, MouseEventArgs e)
        {
            dbClick = false;
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left && e.Clicks == 2) {
                TreeNode tn = ProcTree.GetNodeAt(e.X, e.Y);
                if (tn != null) dbClick = true;
                if (e.Button == MouseButtons.Right) {
                    ProcTree.SelectedNode = tn;
                }
            }
        }

        private void ProcTree_BeforeExpandCollapse(object sender, TreeViewCancelEventArgs e) {
            if (e.Action == TreeViewAction.Expand || e.Action == TreeViewAction.Collapse) {
                 if (dbClick || ctrlKeyPress) {
                    if (e.Node.Tag is Procedure) e.Cancel = true;
                    dbClick = false;
                }
            }
        }
        #endregion

        // Goto script text of selected Variable or Procedure in treeview
        public void SelectLine(string file, int line, bool pselect = false, int column = -1, int sLen = -1)
        {
            if (line <= 0) return;

            bool not_this = false;
            if (currentTab == null || file != currentTab.filepath) {
                if (Open(file, OpenType.File, false, alreadyOpen: false) == null) {
                    MessageBox.Show("Could not open file '" + file + "'", "Error");
                    return;
                }
                not_this = true;
            }
            LineSegment ls;
            if (line > currentDocument.TotalNumberOfLines)
                ls = currentDocument.GetLineSegment(currentDocument.TotalNumberOfLines - 1);
            else
                ls = currentDocument.GetLineSegment(line - 1);

            TextLocation start, end;
            if (column == -1 || column > ls.Length) {
                start = new TextLocation(0, ls.LineNumber);
                if (column == -1)
                    end = new TextLocation(ls.Length, ls.LineNumber);
                else
                    end = new TextLocation(0, ls.LineNumber);
            } else {
                column--;
                if (sLen == -1) {
                    foreach (var w in ls.Words)
                    {
                        if (w.Type != TextWordType.Word)
                            continue;
                        int pos = w.Offset + w.Length;
                        if ((column >= w.Offset) && (column <= pos)) {
                            column = w.Offset;
                            sLen = w.Length;
                            break;
                        }
                    }
                }
                start = new TextLocation(column, ls.LineNumber);
                end = new TextLocation(start.Column + sLen, ls.LineNumber);
            }
            // Expand or Collapse folding
            foreach (FoldMarker fm in currentDocument.FoldingManager.FoldMarker) {
                if (OnlyProcStripButton.Checked) {
                    if (fm.FoldType == FoldType.MemberBody || fm.FoldType == FoldType.Region) {
                        if (fm.StartLine == start.Line)
                            fm.IsFolded = false;
                        else if (fm.FoldType != FoldType.Region)
                            fm.IsFolded = true;
                    }
                } else {
                    if (fm.StartLine == start.Line) {
                        fm.IsFolded = false;
                        break;
                    }
                }
            }
            // Scroll and select
            currentActiveTextAreaCtrl.Caret.Position = start;
            if (not_this || !pselect || !OnlyProcStripButton.Checked)
                currentActiveTextAreaCtrl.SelectionManager.SetSelection(start, end);
            else
                currentActiveTextAreaCtrl.SelectionManager.ClearSelection();

            if (!not_this) {
                if (pselect)
                    currentActiveTextAreaCtrl.TextArea.TextView.FirstVisibleLine = start.Line - 1;
                else
                    currentActiveTextAreaCtrl.CenterViewOn(start.Line + 10, 0);
            } else
                currentActiveTextAreaCtrl.CenterViewOn(start.Line - 15, 0);
            currentTab.textEditor.Refresh();
        }

        private void TextArea_KeyPressed(object sender, KeyPressEventArgs e)
        {
            var caret = currentActiveTextAreaCtrl.Caret;

            if (Settings.autoInputPaired && e.KeyChar == '"') {
                List<Utilities.Quote> quotes = new  List<Utilities.Quote>();
                Utilities.GetQuotesPosition(TextUtilities.GetLineAsString(currentDocument, caret.Line), quotes);
                // skiping quotes "..." region
                bool inQuotes = false;
                foreach (Utilities.Quote q in quotes)
                {
                    if (caret.Column > q.Open && caret.Column < q.Close) {
                        inQuotes = true;
                        break;
                    }
                }
                if (!inQuotes) {
                    char chR = currentDocument.GetCharAt(caret.Offset);
                    char chL = currentDocument.GetCharAt(caret.Offset - 1);
                    if ((chL == '(' && chR == ')') || (chL != '"' && (chR == ' ' || chR == '\r') && !Char.IsLetterOrDigit(chL)))
                        currentDocument.Insert(caret.Offset, "\"");
                    else if (chL == '"' && chR == '"')
                        currentDocument.Remove(caret.Offset, 1);
                }
            }
            else if (e.KeyChar == '(' || e.KeyChar == '[' || e.KeyChar == '{') {
                if (autoComplete.IsVisible) autoComplete.Close();
                if (e.KeyChar == '{') return;

                if (Settings.showTips && currentTab.parseInfo != null && e.KeyChar == '(') {
                    string word = TextUtilities.GetWordAt(currentDocument, caret.Offset - 1);
                    if (word != String.Empty) {
                        string item = ProgramInfo.LookupOpcodesToken(word);
                        if (item != null) {
                            int z = item.IndexOf('\n');
                            if (z > 0)
                                item = item.Remove(z);
                        }
                        if (item == null)
                            item = currentTab.parseInfo.LookupToken(word, null, 0, true);
                        if (item != null)
                            ShowCodeTips(item, caret, 50000, true);
                    }
                }

                if (Settings.autoInputPaired && Char.IsWhiteSpace(currentDocument.GetCharAt(caret.Offset))) {
                    string bracket = (e.KeyChar == '[') ? "]" : ")";
                    currentDocument.Insert(caret.Offset, bracket);
                }
            } else if (e.KeyChar == ')' || e.KeyChar == ']' || e.KeyChar == '}') {
                if (toolTips.Active) ToolTipsHide();
                if (e.KeyChar == '}') return;

                if (Settings.autoInputPaired) {
                    char bracket = (e.KeyChar == ']') ? '[' : '(';
                    if (currentDocument.GetCharAt(caret.Offset -1) == bracket && currentDocument.GetCharAt(caret.Offset) == e.KeyChar) {
                        currentDocument.Remove(caret.Offset, 1);
                        // TODO BUG: В контроле баг при использовании TextBuffer - стирается строка символов.
                        //currentDocument.TextBufferStrategy.Remove(caret.Offset, 1);
                        //currentActiveTextAreaCtrl.TextArea.Refresh();
                    }
                }
            } else {
                if (Settings.autocomplete) {
                    if (!ColorTheme.CheckColorPosition(currentDocument, caret.Position))
                        autoComplete.GenerateList(e.KeyChar.ToString(), currentTab, caret.Offset - 1, toolTips.Tag);
                }
            }
        }

        void TextArea_KeyUp(object sender, KeyEventArgs e)
        {
            updateHighlightPocedure = true;
            if (e.KeyCode == Keys.OemSemicolon) Utilities.FormattingCodeSmart(currentActiveTextAreaCtrl);

            if (!Settings.showTips || toolTips.Active || !Char.IsLetter(Convert.ToChar(e.KeyValue))) return;

            var caret = currentActiveTextAreaCtrl.Caret;
            string word = TextUtilities.GetWordAt(currentDocument, caret.Offset - 1);
            if (word != String.Empty) {
                switch (word) {
                    case "for":
                    case "foreach":
                    case "while":
                    case "switch":
                    case "if":
                    case "ifel":
                    case "elif":
                        ShowCodeTips("Press the TAB key to insert the autocode.", caret, 5000);
                        break;
                }
            }
        }

        private void ShowCodeTips(string tipText, Caret caret, int duration, bool tag = false)
        {
            int offset = TextUtilities.FindWordStart(currentDocument, caret.Offset - 1);
            offset = caret.Offset - offset;
            Point pos = caret.GetScreenPosition(caret.Line, caret.Column - offset);
            pos.Offset(currentActiveTextAreaCtrl.FindForm().PointToClient(
                       currentActiveTextAreaCtrl.Parent.PointToScreen(currentActiveTextAreaCtrl.Location)));
            offset = (autoComplete.IsVisible) ? -25 : 20;
            pos.Offset(0, offset);

            if (tag) showTipsColumn = caret.Offset;

            toolTips.Active = true;
            toolTips.Tag = tag;
            toolTips.Show(tipText, panel1, pos, duration);
        }

        // Tooltip for opcodes and macros
        void TextArea_ToolTipRequest(object sender, ToolTipRequestEventArgs e)
        {
            if (currentTab == null || !e.InDocument)
                return;

            ToolTipRequest.Show(currentTab, currentDocument, e);
        }

        private void UpdateEditorToolStripMenu()
        {
            TextLocation tl = currentActiveTextAreaCtrl.Caret.Position;
            //editorMenuStrip.Tag = tl;
            // includes
            string line = TextUtilities.GetLineAsString(currentDocument, tl.Line).Trim();
            if (!line.TrimStart().StartsWith(ParserInternal.INCLUDE)) {
                openIncludeToolStripMenuItem.Enabled = false;
            }

            // skip for specific color text
            if (ColorTheme.CheckColorPosition(currentDocument, tl))
                return;

            //Refactor name
            if (!Settings.enableParser) {
                renameToolStripMenuItem.Text += ": Disabled";
                renameToolStripMenuItem.ToolTipText = "It is required to enable the parser in the settings.";
                return;
            }
            if (currentTab.parseInfo != null) {
                NameType nt = NameType.None;
                IParserInfo item = null;
                string word = TextUtilities.GetWordAt(currentDocument, currentDocument.PositionToOffset(tl));
                item = currentTab.parseInfo.Lookup(word, currentTab.filepath, tl.Line + 1);
                if (item != null) {
                    nt = item.Type();
                    renameToolStripMenuItem.Tag = item;
                    if (!currentTab.needsParse)
                        renameToolStripMenuItem.Enabled = true;
                }
                switch (nt) {
                    case NameType.LVar: // variable procedure
                    case NameType.GVar: // variable script
                        findReferencesToolStripMenuItem.Enabled = true;
                        findDeclerationToolStripMenuItem.Enabled = true;
                        findDefinitionToolStripMenuItem.Enabled = false;
                        renameToolStripMenuItem.Text += (nt == NameType.LVar)
                                                        ? (((Variable)item).IsArgument ? ": Argument Variable" : ": Local Variable")
                                                        : ": Script Variable";
                        if (item.IsExported)
                            renameToolStripMenuItem.ToolTipText = "Note: Renaming exported variables will result in an error in the scripts using this variable.";
                        break;
                    case NameType.Proc:
                        findReferencesToolStripMenuItem.Enabled = currentTab.parseInfo.parseData; //true;
                        findDeclerationToolStripMenuItem.Enabled = true;
                        findDefinitionToolStripMenuItem.Enabled = !item.IsImported;
                        renameToolStripMenuItem.Text += ": Procedure";
                        if (item.IsExported)
                            renameToolStripMenuItem.ToolTipText = "Note: Renaming exported procedures will result in an error in the scripts using this procedure.";
                        break;
                    case NameType.Macro:
                        findReferencesToolStripMenuItem.Enabled = false;
                        findDeclerationToolStripMenuItem.Enabled = true;
                        findDefinitionToolStripMenuItem.Enabled = false;
                        Macro mcr = (Macro)item;
                        if (!ProgramInfo.macrosGlobal.ContainsKey(mcr.name) && mcr.fdeclared == currentTab.filepath)
                            renameToolStripMenuItem.Text += ": Local Macros";
                        else {
                            renameToolStripMenuItem.Text += ": Global Macros";
                            renameToolStripMenuItem.Enabled = false; // TODO: for next version
                            renameToolStripMenuItem.ToolTipText = "The feature is disabled, will be available in future versions.";
                        }
                        break;
                    default:
                        if (!currentTab.parseInfo.parseData) {
                            renameToolStripMenuItem.Text += ": Out of data";
                            renameToolStripMenuItem.ToolTipText = "The parser data is missing.";
                        } else
                            renameToolStripMenuItem.Text += ": None";
                        break;
                }
                if (item != null && item.IsImported) {
                    renameToolStripMenuItem.Enabled = !item.IsImported;
                    renameToolStripMenuItem.ToolTipText = "The feature is disabled, will be available in future versions.";
                }
            } else {
                renameToolStripMenuItem.Text += ": Out of data";
                renameToolStripMenuItem.ToolTipText = "The parser data is missing.";
            }
        }

        private void editorMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            findReferencesToolStripMenuItem.Enabled = true;
            findDeclerationToolStripMenuItem.Enabled = true;
            findDefinitionToolStripMenuItem.Enabled = true;
            openIncludeToolStripMenuItem.Enabled = true;
        }

        private void recentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int count = Open_toolStripSplitButton.DropDownItems.Count;
            if (count < 4 || MessageBox.Show("Do you want to clear the list of recent files ?",
                                             "Recent files", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            for (int i = 3; i < count; i++)
                Open_toolStripSplitButton.DropDownItems.RemoveAt(3);

            Settings.ClearRecent();
        }

        #region Control set states
        private void InitControlEvent()
        {   // Parser
            parserLabel = new ToolStripLabel((Settings.enableParser) ? "Parser: No file" : parseoff);
            parserLabel.Alignment = ToolStripItemAlignment.Right;
            parserLabel.Click += delegate(object sender, EventArgs e) { ParseScript(0); };
            parserLabel.ToolTipText = "Click - Run update parser info.";
            parserLabel.TextChanged += delegate(object sender, EventArgs e) { parserLabel.ForeColor = Color.Black; };
            ToolStrip.Items.Add(parserLabel);

            // Parser timer
            extParserTimer = new Timer();
            extParserTimer.Interval = 100;
            extParserTimer.Tick += new EventHandler(ExternalParser_Tick);
            intParserTimer = new Timer();
            intParserTimer.Interval = 10;
            intParserTimer.Tick += new EventHandler(InternalParser_Tick);

            // Tabs Swapped
            tabControl1.tabsSwapped += delegate(object sender, TabsSwappedEventArgs e) {
                TabInfo tmp = tabs[e.aIndex];
                tabs[e.aIndex] = tabs[e.bIndex];
                tabs[e.aIndex].index = e.aIndex;
                tabs[e.bIndex] = tmp;
                tabs[e.bIndex].index = e.bIndex;
            };

            // Create Variable Tab
            VarTree.HotTracking = true;
            VarTree.ShowNodeToolTips = true;
            VarTree.ShowRootLines = false;
            VarTree.Indent = 16;
            VarTree.ItemHeight = 14;
            VarTree.MouseDoubleClick += TreeView_DClickMouse;
            VarTree.AfterSelect += TreeView_AfterSelect;
            VarTree.AfterCollapse += Tree_AfterExpandCollapse;
            VarTree.AfterExpand += Tree_AfterExpandCollapse;
            VarTree.Dock = DockStyle.Fill;
            VarTree.BackColor = Color.FromArgb(250, 250, 255);
            VarTree.Cursor = Cursors.Hand;
            VarTab.Padding = new Padding(0, 2, 2, 2);
            VarTab.BackColor = SystemColors.ControlLightLight;
            VarTab.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            VarTab.Controls.Add(VarTree);
        }

        private void SetTabTextChange(int i) { tabControl1.TabPages[i].ImageIndex = (tabs[i].changed ? 1 : 0); }

        private void SetActiveAreaEvents(TextEditorControl te)
        {
            te.ActiveTextAreaControl.TextArea.MouseDown += delegate(object a1, MouseEventArgs a2) {
                //if (a2.Button == MouseButtons.Left)
                //    Utilities.SelectedTextColorRegion(currentActiveTextAreaCtrl);
                autoComplete.Close();
            };
            te.ActiveTextAreaControl.TextArea.KeyUp += TextArea_KeyUp;
            te.ActiveTextAreaControl.TextArea.KeyPress += TextArea_KeyPressed;
            te.ActiveTextAreaControl.TextArea.MouseEnter += TextArea_SetFocus;
            te.ActiveTextAreaControl.TextArea.PreviewKeyDown += TextArea_PreviewKeyDown;
            te.ActiveTextAreaControl.TextArea.MouseWheel += TextArea_MouseWheel;
            te.ActiveTextAreaControl.VScrollBar.Scroll += delegate(object sender, ScrollEventArgs e) {
                var e1 = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, e.OldValue - e.NewValue);
                TextArea_MouseWheel(sender, e1);
            };
            te.ActiveTextAreaControl.TextArea.MouseClick += delegate(object sender, MouseEventArgs e) {
                if (e.Button == MouseButtons.Middle) {
                    Utilities.HighlightingSelectedText(currentActiveTextAreaCtrl);
                    currentTab.textEditor.Refresh();
                } else if (toolTips.Active && e.Button == MouseButtons.Left) {
                     ToolTipsHide();
                }
            };
            te.ActiveTextAreaControl.TextArea.ToolTipRequest += new ToolTipRequestEventHandler(TextArea_ToolTipRequest);
            te.ActiveTextAreaControl.Caret.PositionChanged += new EventHandler(Caret_PositionChanged);
            te.ActiveTextAreaControl.TextArea.MouseDoubleClick += new MouseEventHandler(TextArea_MouseDoubleClick);
        }

        void TextArea_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            Utilities.SelectedTextColorRegion(currentActiveTextAreaCtrl.Caret.Position, currentActiveTextAreaCtrl);
        }

        bool setOnlyOnce = false;
        private void EnableFormControls()
        {
            TabClose_button.Visible = true;
            Split_button.Visible = true;
            splitDocumentToolStripMenuItem.Enabled = true;
            openAllIncludesScriptToolStripMenuItem.Enabled = true;
            GotoProc_StripButton.Enabled = true;
            //Search_toolStripButton.Enabled = true;
            CommentStripButton.Enabled = true;
            if (Settings.showLog)
                splitContainer1.Panel2Collapsed = false;
            includeFileToCodeToolStripMenuItem.Enabled = true;

            // set buttons position
            if (setOnlyOnce) return;
            setOnlyOnce = true;

            int xLocation = tabControl1.DisplayRectangle.Right;
            TabClose_button.Left = xLocation - TabClose_button.Width + 1;
            TabClose_button.Top = tabControl1.DisplayRectangle.Top - 1;

            Split_button.Left = xLocation - Split_button.Width;
            Split_button.Top = tabControl1.DisplayRectangle.Bottom - Split_button.Height;

            minimizelog_button.Left = tabControl2.DisplayRectangle.Right - minimizelog_button.Width + 2;
            minimizelog_button.Top = tabControl2.Top - 1;
        }

        private void ControlFormStateOn_Off()
        {
            autoComplete.Close();

            ShowTabsSpaces();
            ShowLineNumbers(null, null);

            if (currentTab.parseInfo != null && currentDocument.FoldingManager.FoldMarker.Count > 0) //currentTab.parseInfo.procs.Length
                Outline_toolStripButton.Enabled = true;
            else
                Outline_toolStripButton.Enabled = false;

            SetBackForwardButtonState();

            if (currentTab.shouldParse) {
                DecIndentStripButton.Enabled = true;
                //CommentStripButton.Enabled = true;
                AlignToLeftToolStripMenuItem.Enabled = true;
                ToggleBlockCommentToolStripMenuItem.Enabled = true;
                formatingCodeToolStripMenuItem.Enabled = true;
            } else {
                DecIndentStripButton.Enabled = false;
                //CommentStripButton.Enabled = false;
                AlignToLeftToolStripMenuItem.Enabled = false;
                ToggleBlockCommentToolStripMenuItem.Enabled = false;
                formatingCodeToolStripMenuItem.Enabled = false;
            }
        }

        // No selected text tabs
        private void SetFormControlsOff() {
            Outline_toolStripButton.Enabled = false;
            splitContainer2.Panel2Collapsed = true;
            TabClose_button.Visible = false;
            openAllIncludesScriptToolStripMenuItem.Enabled = false;
            Split_button.Visible = false;
            splitDocumentToolStripMenuItem.Enabled = false;
            Back_toolStripButton.Enabled = false;
            Forward_toolStripButton.Enabled = false;
            GotoProc_StripButton.Enabled = false;
            //Search_toolStripButton.Enabled = false;
            if (SearchToolStrip.Visible)
                Search_Panel(null, null);
            DecIndentStripButton.Enabled = false;
            CommentStripButton.Enabled = false;
            Text = SSE.Remove(SSE.Length - 2);
            autoComplete.Close();
            includeFileToCodeToolStripMenuItem.Enabled = false;
        }
        #endregion

        #region Dialog System
        private void dialogNodesDiagramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;

            var tag = tabControl1.TabPages[currentTab.index].Tag;
            if (tag != null) {
                NodeDiagram ndForm = ((NodeDiagram)tag);
                if (ndForm.WindowState == FormWindowState.Minimized)
                    ndForm.WindowState = FormWindowState.Maximized;
                ndForm.Activate();
                return;
            }

            string msgPath;
            if (!MessageFile.GetAssociatePath(currentTab, false, out msgPath)) {
                MessageBox.Show(MessageFile.MissingFile, "Nodes Flowchart Editor");
                return;
            }
            ScriptEditor.TextEditorUI.Function.DialogFunctionsRules.BuildOpcodesDictionary();

            currentTab.msgFilePath = msgPath;

            NodeDiagram NodesView = new NodeDiagram(currentTab);
            NodesView.FormClosed += delegate { tabControl1.TabPages[currentTab.index].Tag = null; };
            NodesView.ChangeNodes += delegate { ForceParseScript(); }; //Force Parse Script;
            NodesView.Show();

            tabControl1.TabPages[currentTab.index].Tag = NodesView;

            this.ParserUpdatedInfo += delegate
            {
                if (NodesView != null)
                    NodesView.NeedUpdate = true;
            };
        }

        private void previewDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null) return;

            string msgPath;
            if (!MessageFile.GetAssociatePath(currentTab, false, out msgPath)) {
                MessageBox.Show(MessageFile.MissingFile, "Dialog Preview");
                return;
            }

            ScriptEditor.TextEditorUI.Function.DialogFunctionsRules.BuildOpcodesDictionary();

            currentTab.msgFilePath = msgPath;
            DialogPreview DialogView = new DialogPreview(currentTab);
            if (!DialogView.InitReady) {
                DialogView.Dispose();
                MessageBox.Show("This script does not contain dialog procedures.", "Dialog Preview");
            }
            else
                DialogView.Show(this);
        }

        private void dialogFunctionConfigToolStripMenuItem_Click(object sender, EventArgs e) {
            ScriptEditor.TextEditorUI.Function.DialogFunctionsRules.BuildOpcodesDictionary();
            new ScriptEditor.TextEditorUI.Function.FunctionsRules().ShowDialog(this);
        }

        private void editNodeCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Procedure proc = (Procedure)ProcTree.SelectedNode.Tag;

            if (currentTab.messages.Count == 0) {
                string msgPath;
                if (!MessageFile.GetAssociatePath(currentTab, false, out msgPath)) {
                    MessageBox.Show(MessageFile.MissingFile, "Node Editor");
                    return;
                }
                currentTab.msgFilePath = msgPath;
                MessageFile.ParseMessages(currentTab, File.ReadAllLines(currentTab.msgFilePath, Settings.EncCodePage));
            }

            foreach (var nodeTE in currentTab.nodeFlowchartTE)
            {
                if (nodeTE.NodeName == proc.name) {
                    nodeTE.Activate();
                    return;
                }
            }

            FlowchartTE nodeEditor = new FlowchartTE(proc, currentTab);
            nodeEditor.Disposed += delegate(object s, EventArgs e1) { currentTab.nodeFlowchartTE.Remove((FlowchartTE)s); };
            nodeEditor.ApplyCode += new EventHandler<FlowchartTE.CodeArgs>(nodeEditor_ApplyCode);
            nodeEditor.ShowEditor(this);

            currentTab.nodeFlowchartTE.Add(nodeEditor);
        }

        private void nodeEditor_ApplyCode(object sender, FlowchartTE.CodeArgs e)
        {
            if (e.Change) {
                if (Utilities.ReplaceProcedureCode(currentDocument, currentTab.parseInfo, e.Name, e.Code)) {
                    MessageBox.Show("In the source script, there is no dialog node with this name.", "Apply code error");
                    return;
                }
                e.Change = false;
                ForceParseScript();
            }
        }
        #endregion

        #region Static Functions
        internal static TabInfo CheckTabs(List<TabInfo> tabs, string checkFile)
        {
            foreach (TabInfo tab in tabs) {
                if (string.Compare(tab.filepath, checkFile, true) == 0)
                    return tab;
            }
            return null;
        }
        #endregion
    }
}