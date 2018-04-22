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
        private const string parseoff = "Parser: Disabled";
        private const string unsaved = "unsaved.ssl";

        private static readonly string[] TREEPROCEDURES = new string[] { "Global Procedures", "Local Procedures" };
        private static readonly string[] TREEVARIABLES = new string[] { "Global Variables", "Script Variables" };
        private static readonly System.Media.SoundPlayer DontFind = new System.Media.SoundPlayer(Properties.Resources.DontFind);
        private static readonly System.Media.SoundPlayer CompileFail = new System.Media.SoundPlayer(Properties.Resources.CompileError);

        private DateTime extParser_TimeNext, intParser_TimeNext;
        private Timer extParserTimer, intParserTimer;
        private readonly List<TabInfo> tabs = new List<TabInfo>();
        private TabInfo currentTab;
        private ToolStripLabel parserLabel;

        public static volatile bool parserRunning;

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
        internal static bool ParsingErrors = true;

        private int showTipsColumn;

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
           //Program.SetDoubleBuffered(ProcTree);
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
            
            autoComplete = new AutoComplete(panel1, Settings.autocompleteColor);

            if (Settings.encoding == (byte)EncodingType.OEM866) {
                EncodingDOSmenuItem.Checked = true;
                windowsDefaultMenuItem.Checked = false;
            }
            
            // Highlighting
            FileSyntaxModeProvider fsmProvider = new FileSyntaxModeProvider(SyntaxFile.SyntaxFolder); // Create new provider with the highlighting directory.
            HighlightingManager.Manager.AddSyntaxModeFileProvider(fsmProvider); // Attach to the text editor.
            ColorTheme.InitTheme(Settings.highlight == 2, this);

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

            tbOutput.Text = "***** " +  AboutBox.appName + " v." + AboutBox.appVersion + " *****";
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

            if (bwSyntaxParser.IsBusy) {
                e.Cancel = true;
                return;
            }

            if (sf != null)
                sf.Close();

            splitContainer3.Panel1Collapsed = true;
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
                            bool seltab = true, bool commandline = false, bool fcdOpen = false, bool alreadyOpen = true)
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
                    string decomp = new Compiler().Decompile(file);
                    if (decomp == null) {
                        MessageBox.Show("Decompilation of '" + file + "' was not successful", "Error");
                        return null;
                    } else {
                        file = decomp;
                        // fix for procedure begin
                        Parser.FixProcedureBegin(file);
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
            
            te.TextEditorProperties.AllowCaretBeyondEOL = true;
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
                te.TextEditorProperties.Font = new Font("Verdana", 10, FontStyle.Regular, GraphicsUnit.Point);
            } else {
                te.SetHighlighting(ColorTheme.HighlightingScheme); // Activate the highlighting, use the name from the SyntaxDefinition node.
                te.Document.FoldingManager.FoldingStrategy = new CodeFolder();
                te.TextEditorProperties.ConvertTabsToSpaces = Settings.tabsToSpaces;
                te.TextEditorProperties.ShowSpaces = Settings.showTabsChar;
                te.TextEditorProperties.IndentStyle = IndentStyle.Smart;
                te.TextEditorProperties.ShowVerticalRuler = Settings.showVRuler;
                te.TextEditorProperties.VerticalRulerRow = Settings.tabSize;
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

                    if (!createNew && Settings.storeLastPosition)
                        te.ActiveTextAreaControl.JumpTo(Settings.GetLastScriptPosition(ti.filename.ToLowerInvariant()));

                    if (Settings.autoOpenMsgs && ti.filepath != null) 
                        AssossciateMsg(ti, false);
                }
            }
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

        private void Save(TabInfo tab, bool close = false)
        {
            if (tab != null) {
                if (tab.filepath == null) {
                    SaveAs(tab, close);
                    return;
                }
                while (parserRunning) {
                    System.Threading.Thread.Sleep(10); //Avoid stomping on files while the parser is running
                }

                bool msg = false;
                if (Path.GetExtension(tab.filename) == ".msg") 
                    msg = true;

                if (Settings.autoTrailingSpaces && !msg)
                    new ICSharpCode.TextEditor.Actions.RemoveTrailingWS().Execute(currentActiveTextAreaCtrl.TextArea);

                if (close && tab.textEditor.Document.FoldingManager.FoldMarker.Count > 0)
                    CodeFolder.SaveMarkFoldCollapsed(tab.textEditor.Document);

                string saveText = tab.textEditor.Text;
                if (msg && Settings.EncCodePage.CodePage == 866) 
                    saveText = saveText.Replace('\u0425', '\u0058'); //Replacement russian letter "X", to english letter

                Utilities.NormalizeDelimiter(ref saveText);

                File.WriteAllText(tab.filepath, saveText, msg ? Settings.EncCodePage 
                                                              : (Settings.saveScriptUTF8) ? new UTF8Encoding(false) 
                                                              : Encoding.Default);
                tab.changed = false;
                SetTabTextChange(tab.index);
                this.Text = SSE + tab.filepath + ((pDefineStripComboBox.SelectedIndex > 0) ? " [" + pDefineStripComboBox.Text + "]" : "");
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

            bool skip = tab.changed;
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

        private static void KeepScriptSetting(TabInfo tab, bool skip)
        {
            if (!skip && tab.filepath != null && tab.textEditor.Document.FoldingManager.FoldMarker.Count > 0) {
                CodeFolder.SaveMarkFoldCollapsed(tab.textEditor.Document);
                File.WriteAllText(tab.filepath, tab.textEditor.Text, (Settings.saveScriptUTF8) ? new UTF8Encoding(false) : Encoding.Default);
            }

            // store last script position
            if (Path.GetExtension(tab.filepath).ToLowerInvariant() == ".ssl" && tab.filename != unsaved)
                Settings.SetLastScriptPosition(tab.filename.ToLowerInvariant(), tab.textEditor.ActiveTextAreaControl.Caret.Line);
        }

        private void AssossciateMsg(TabInfo tab, bool create)
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

        private bool Compile(TabInfo tab, out string msg, bool showMessages = true, bool preprocess = false)
        {
            msg = String.Empty;
            if (string.Compare(Path.GetExtension(tab.filename), ".ssl", true) != 0) {
                if (showMessages)
                    MessageBox.Show("You cannot compile this file.", "Compile Error");
                return false;
            }
            if (!Settings.ignoreCompPath && !preprocess && Settings.outputDir == null) {
                if (showMessages)
                    MessageBox.Show("No output path selected.\nPlease select your scripts directory before compiling", "Compile Error");
                return false;
            }
            if (tab.changed)
                Save(tab);
            if (tab.changed || tab.filepath == null)
                return false;

            bool success = new Compiler().Compile(tab.filepath, out msg, tab.buildErrors, preprocess, tab.parseInfo.ShortCircuitEvaluation);
            
            foreach (ErrorType et in new ErrorType[] { ErrorType.Error, ErrorType.Warning, ErrorType.Message }) {
                foreach (Error e in tab.buildErrors) {
                    if (e.type == et) {
                        dgvErrors.Rows.Add(e.type.ToString(), Path.GetFileName(e.fileName), e.line, e);
                        if (et == ErrorType.Error)
                            dgvErrors.Rows[dgvErrors.Rows.Count - 1].Cells[0].Style.ForeColor = Color.Red;
                    }
                }
            }

            if (dgvErrors.RowCount > 0)
                dgvErrors.Rows[0].Cells[0].Selected = false;
            
            if (preprocess)
                return success;

            if (!success) {
                parserLabel.Text = "Failed to compiled: " + tab.filename;
                parserLabel.ForeColor = Color.Firebrick;
                msg += "\r\n Compilation Failed! (See the output build and errors window log for details).";
                CompileFail.Play();

                if (showMessages) {
                    if (Settings.warnOnFailedCompile) {
                        tabControl2.SelectedIndex = 2 - Convert.ToInt32(Settings.userCmdCompile);
                        maximize_log();
                    } else 
                        new CompiledStatus(false, this).ShowCompileStatus();
                }
            } else {
                if (showMessages)
                    new CompiledStatus(true, this).ShowCompileStatus();
                parserLabel.Text = "Compiled: " + tab.filename + " at " + DateTime.Now.ToString("HH:mm:ss");
                parserLabel.ForeColor = Color.DarkGreen;
                msg += (!preprocess)? "\r\n Compilation Successfully!": string.Empty;
            }
            return success;
        }

        // Called when creating a new document and when switching tabs
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
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
                UpdateNames();
                // text editor set focus 
                currentActiveTextAreaCtrl.Select();
                ControlFormStateOn_Off();
                this.Text = SSE + currentTab.filepath + ((pDefineStripComboBox.SelectedIndex > 0) ? " [" + pDefineStripComboBox.Text + "]" : ""); 
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

        // Create names for procedures and variables in treeview
        private void UpdateNames()
        {
            if (currentTab == null || !currentTab.shouldParse || currentTab.parseInfo == null)
                return;
            
            object selectedNode = null;
            if (ProcTree.SelectedNode != null)
                selectedNode = ProcTree.SelectedNode.Tag;

            ProcTree.Tag = TreeStatus.update;
            ProcTree.BeginUpdate();
            ProcTree.Nodes.Clear();

            TreeNode rootNode;
            foreach (var s in TREEPROCEDURES) {
                rootNode = ProcTree.Nodes.Add(s);
                rootNode.ForeColor = Color.DodgerBlue;
                rootNode.NodeFont = new Font("Arial", 9, FontStyle.Bold);
            }
            ProcTree.Nodes[0].ToolTipText = "Procedures declared and located in headers files";
            ProcTree.Nodes[0].Tag = 0; // global tag
            ProcTree.Nodes[1].ToolTipText = "Procedures declared and located in this script";
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
                    rootNode.NodeFont = new Font("Arial", 9, FontStyle.Bold);
                }
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
            ProcTree.Tag = TreeStatus.idle;
        }

        private void SetNodeCollapseStatus(TreeNode node)
        {
            if (currentTab.treeExpand.ContainsKey(node.FullPath)) {
                    if (currentTab.treeExpand[node.FullPath])
                        node.Collapse();
                    else
                        node.Expand();
            }
            foreach (TreeNode nd in node.Nodes)
                SetNodeCollapseStatus(nd);
        }

        private void TreeExpandCollapse(TreeViewEventArgs e)
        {
            TreeNode tn = e.Node;
            if (tn == null)
                return;

            bool collapsed = (e.Action == TreeViewAction.Collapse);
            if (!currentTab.treeExpand.ContainsKey(tn.FullPath))
                currentTab.treeExpand.Add(tn.FullPath, collapsed);
            else
                currentTab.treeExpand[tn.FullPath] = collapsed;
        }

        // Click on node tree Procedures/Variables
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
                return;

            string file = null;
            int line = 0;
            bool pSelect = false;
            if (e.Node.Tag is Variable) {
                Variable var = (Variable)e.Node.Tag;
                file = var.fdeclared;
                line = var.d.declared;
            } else if (e.Node.Tag is Procedure) {
                Procedure proc = (Procedure)e.Node.Tag;
                file = proc.fstart;
                line = proc.d.start;
                if (file == null) { // goto declared
                    file = proc.fdeclared;
                    line = proc.d.declared;
                }
                pSelect = true;
            }
            if (file != null)
                SelectLine(file, line, pSelect);
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
            if (e.Button == MouseButtons.Right)
                ProcTree.SelectedNode = ProcTree.GetNodeAt(e.X, e.Y);
        }
        #endregion

        // Goto script text of selected Variable or Procedure in treeview
        public void SelectLine(string file, int line, bool pselect = false, int column = -1, int sLen = -1)
        {
            if (line <= 0)
                return;
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
                char chR = currentDocument.GetCharAt(caret.Offset);
                char chL = currentDocument.GetCharAt(caret.Offset - 1);
                if ((chR == ' ' || chR == '\r') && !Char.IsLetterOrDigit(chL)) 
                    currentDocument.Insert(caret.Offset, "\"");
                else if (chL == '"' && chR == '"')
                        currentDocument.Remove(caret.Offset, 1);
            } 
            else if (e.KeyChar == '(' || e.KeyChar == '[' || e.KeyChar == '{') {
                if (autoComplete.IsVisible)
                    autoComplete.Close();

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
                    string bracket = ")";
                    if (e.KeyChar == '[')
                        bracket = "]";
                    else if (e.KeyChar == '{')
                        bracket = "}";
                    currentDocument.Insert(caret.Offset, bracket);
                }
            } else if (e.KeyChar == ')' || e.KeyChar == ']' || e.KeyChar == '}') {
                if (toolTips.Active) ToolTipsHide();

                if (Settings.autoInputPaired) {
                    char bracket = '(';
                    if (e.KeyChar == ']') 
                        bracket = '[';
                    else if (e.KeyChar == '}')
                        bracket = '{';
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
            if (e.KeyCode == Keys.OemSemicolon)
                Utilities.FormattingCodeSmart(currentActiveTextAreaCtrl);

            if (!Settings.showTips || toolTips.Active || !Char.IsLetter(Convert.ToChar(e.KeyValue)))
               return;

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
            if (!line.TrimStart().StartsWith(Parser.INCLUDE)) {
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
            parserLabel.Click += delegate(object sender, EventArgs e) { ParseScript(); };
            parserLabel.ToolTipText = "Click - Run update parser info.";
            parserLabel.TextChanged += delegate(object sender, EventArgs e) { parserLabel.ForeColor = Color.Black; };
            ToolStrip.Items.Add(parserLabel);

            // Parser timer
            extParserTimer = new Timer();
            extParserTimer.Interval = 100;
            extParserTimer.Tick += new EventHandler(ExternalParser_Tick);
            intParserTimer = new Timer();
            intParserTimer.Interval = 50;
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
            VarTree.AfterSelect += TreeView_AfterSelect;
            VarTree.AfterCollapse += Tree_AfterExpandCollapse;
            VarTree.AfterExpand += Tree_AfterExpandCollapse;
            VarTree.Dock = DockStyle.Fill;
            VarTree.BackColor = Color.FromArgb(250, 250, 255);
            VarTree.Cursor = Cursors.Hand;
            VarTab.Padding = new Padding(0, 2, 2, 2);
            VarTab.BackColor = SystemColors.ControlLightLight;
            VarTab.Controls.Add(VarTree);            
        }

        private void SetTabTextChange(int i) { tabControl1.TabPages[i].ImageIndex = (tabs[i].changed ? 1 : 0); }

        private void SetActiveAreaEvents(TextEditorControl te)
        {
            te.ActiveTextAreaControl.TextArea.MouseDown += delegate(object a1, MouseEventArgs a2) {
                if (a2.Button == MouseButtons.Left)
                    Utilities.SelectedTextColorRegion(currentActiveTextAreaCtrl);
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
        }

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

        #region Menu control events
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool p = Settings.enableParser; //save prev.state
            int f = Settings.selectFont;
            (new SettingsDialog()).ShowDialog();
            if (currentTab != null)
                tabControl1_Selected(null, null);

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
                parserLabel.Text = "Parser: Enabled (click here for get updated parsing data)";
                parserLabel.ForeColor = Color.Green;
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
            ApplySettingsTabs(f != Settings.selectFont);
            if (Settings.pathHeadersFiles != null)
                Headers_toolStripSplitButton.Enabled = true;
            
            autoComplete.Colored = Settings.autocompleteColor;
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
                    customHighlight.HighlightWordClear();
                    customHighlight.ProceduresHighlight(ct.textEditor.Document, ct.parseInfo.procs);
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
                    if (tabControl1.GetTabRect(i).Contains(e.X, e.Y)) {
                        if (e.Button == MouseButtons.Middle)
                            Close(tabs[i]);
                        else if (e.Button == MouseButtons.Right) {
                            cmsTabControls.Tag = i;
                            
                            foreach (ToolStripItem item in cmsTabControls.Items)
                                item.Visible = true;

                            cmsTabControls.Show(tabControl1, e.X, e.Y);
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
                    if (tabControl2.GetTabRect(i).Contains(e.X, e.Y)) {
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
                            
                            cmsTabControls.Show(tabControl2, e.X, e.Y);
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
            if (!result)
                return;
            
            string file = Compiler.GetPreprocessedFile(currentTab.filename);
            if (file != null)
                Open(file, OpenType.File, false);
            else
                MessageBox.Show("Failed to fetch preprocessed file");
        }

        private void roundtripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;

            if (Settings.userCmdCompile) {
                MessageBox.Show("It is required to turn off the compilation option via a user cmd file.");
                return;
            }
            dgvErrors.Rows.Clear();
            string msg;
            bool result = Compile(currentTab, out msg);
            tbOutput.Text = currentTab.buildLog = msg;
            if (result) {
                Open(new Compiler().GetOutputPath(currentTab.filepath), OpenType.File, false);
            }
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
                AssossciateMsg(currentTab, true);
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
                renameToolStripMenuItem.ToolTipText = (currentTab.needsParse)? "Waiting get parsing data..." : ""; 
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
            if (minimizelogsize == 0)
                return;
            if (Settings.editorSplitterPosition == -1)
                Settings.editorSplitterPosition = Size.Height - (Size.Height / 4);
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
                foreach (string s in Parser.GetAllIncludes(currentTab))
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
            if (e.Button != MouseButtons.Left)
                return;

            if (e.Node.Tag != null && currentTab != null) {
                if (!Functions.NodeHitCheck(e.Location, e.Node.Bounds))
                    return;

                string space = new string(' ', currentActiveTextAreaCtrl.Caret.Column);
                string code = e.Node.Tag.ToString();
                if (code.Contains("<cr>"))
                    code = code.Replace("<cr>", Environment.NewLine + space);
                else if (code.EndsWith(")"))
                    code += " ";
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

        private void TextEditor_Deactivate(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;
            currentActiveTextAreaCtrl.TextArea.MouseEnter -= TextArea_SetFocus;
        }

        private void TextEditor_Activated(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;
            currentActiveTextAreaCtrl.TextArea.MouseEnter += TextArea_SetFocus;
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
            ParsingErrors = ParsingErrorsToolStripMenuItem.Checked;
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "/select, " + tabs[(int)cmsTabControls.Tag].filepath);
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
            if (currentTab != null)
                OutputErrorLog(currentTab);
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
            if (++Settings.sizeFont > 3)
                Settings.sizeFont = -3;

            SizeFontToString();
        }

        private void SizeFontToString()
        {
            string str = Settings.sizeFont.ToString();
            str = (str[0] == '-') ? str : '+' + str;
            FontSizeStripStatusLabel.Text = str;
        }

        private void decompileF1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.decompileF1 = decompileF1ToolStripMenuItem.Checked;
        }

        private void win32RenderTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.winAPITextRender = win32RenderTextToolStripMenuItem.Checked;
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
            Utilities.PasteIncludeFile(currentActiveTextAreaCtrl, Headers_toolStripSplitButton.Bounds.Location);
        }

        private void oldDecompileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.oldDecompile = oldDecompileToolStripMenuItem.Checked;
        }
        #endregion

        #region Dialog System
        private void dialogNodesDiagramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentTab == null)
                return;

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
            if (currentTab == null)
                return;

            string msgPath;
            if (!MessageFile.GetAssociatePath(currentTab, false, out msgPath)) {
                MessageBox.Show(MessageFile.MissingFile, "Dialog Preview");
                return;
            }
            currentTab.msgFilePath = msgPath;
            DialogPreview DialogView = new DialogPreview(currentTab);
            if (!DialogView.InitReady) {
                DialogView.Dispose();
                MessageBox.Show("This script does not contain dialog procedures.", "Dialog Preview");
            }
            else
                DialogView.Show(this);
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