namespace ScriptEditor {
    partial class SettingsDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            this.cbWarnings = new System.Windows.Forms.CheckBox();
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.cbIncludePath = new System.Windows.Forms.CheckBox();
            this.bChange = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.cbWarnFailedCompile = new System.Windows.Forms.CheckBox();
            this.cbMultiThread = new System.Windows.Forms.CheckBox();
            this.cbAutoOpenMessages = new System.Windows.Forms.CheckBox();
            this.bScriptsH = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tbLanguage = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbOptimize = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbTabSize = new System.Windows.Forms.TextBox();
            this.cbTabsToSpaces = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbEnableParser = new System.Windows.Forms.CheckBox();
            this.cbShortCircuit = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cbAutocomplete = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.cbParserWarn = new System.Windows.Forms.CheckBox();
            this.cbWatcom = new System.Windows.Forms.CheckBox();
            this.cbCompilePath = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.msgPathlistView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MsgcontextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bAssociate = new System.Windows.Forms.Button();
            this.cbUserCompile = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.HintLang_comboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Highlight_comboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            label1 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.MsgcontextMenu.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(115, 13);
            label1.TabIndex = 5;
            label1.Text = "Compiled scripts folder:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(6, 55);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(154, 13);
            label4.TabIndex = 11;
            label4.Text = "Location folder of headers files:";
            // 
            // cbWarnings
            // 
            this.cbWarnings.AutoSize = true;
            this.cbWarnings.Location = new System.Drawing.Point(6, 44);
            this.cbWarnings.Name = "cbWarnings";
            this.cbWarnings.Size = new System.Drawing.Size(101, 17);
            this.cbWarnings.TabIndex = 0;
            this.cbWarnings.Text = "Show Warnings";
            this.toolTip.SetToolTip(this.cbWarnings, "Show compiler warning messages.");
            this.cbWarnings.UseVisualStyleBackColor = true;
            // 
            // cbDebug
            // 
            this.cbDebug.AutoSize = true;
            this.cbDebug.Location = new System.Drawing.Point(6, 67);
            this.cbDebug.Name = "cbDebug";
            this.cbDebug.Size = new System.Drawing.Size(119, 17);
            this.cbDebug.TabIndex = 1;
            this.cbDebug.Text = "Show debug output";
            this.cbDebug.UseVisualStyleBackColor = true;
            // 
            // cbIncludePath
            // 
            this.cbIncludePath.AutoSize = true;
            this.cbIncludePath.Location = new System.Drawing.Point(303, 54);
            this.cbIncludePath.Name = "cbIncludePath";
            this.cbIncludePath.Size = new System.Drawing.Size(132, 17);
            this.cbIncludePath.TabIndex = 3;
            this.cbIncludePath.Text = "Override includes path";
            this.toolTip.SetToolTip(this.cbIncludePath, "Override path of included header files in script, to this selected path.");
            this.cbIncludePath.UseVisualStyleBackColor = true;
            // 
            // bChange
            // 
            this.bChange.Image = ((System.Drawing.Image)(resources.GetObject("bChange.Image")));
            this.bChange.Location = new System.Drawing.Point(446, 29);
            this.bChange.Name = "bChange";
            this.bChange.Size = new System.Drawing.Size(30, 25);
            this.bChange.TabIndex = 4;
            this.toolTip.SetToolTip(this.bChange, "Change folder");
            this.bChange.UseVisualStyleBackColor = true;
            this.bChange.Click += new System.EventHandler(this.bChange_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select compiled scripts folder";
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // cbWarnFailedCompile
            // 
            this.cbWarnFailedCompile.AutoSize = true;
            this.cbWarnFailedCompile.Location = new System.Drawing.Point(6, 90);
            this.cbWarnFailedCompile.Name = "cbWarnFailedCompile";
            this.cbWarnFailedCompile.Size = new System.Drawing.Size(134, 17);
            this.cbWarnFailedCompile.TabIndex = 7;
            this.cbWarnFailedCompile.Text = "Warn on failed compile";
            this.toolTip.SetToolTip(this.cbWarnFailedCompile, "Show message that script compilation was not successful.");
            this.cbWarnFailedCompile.UseVisualStyleBackColor = true;
            // 
            // cbMultiThread
            // 
            this.cbMultiThread.AutoSize = true;
            this.cbMultiThread.Location = new System.Drawing.Point(146, 90);
            this.cbMultiThread.Name = "cbMultiThread";
            this.cbMultiThread.Size = new System.Drawing.Size(156, 17);
            this.cbMultiThread.TabIndex = 8;
            this.cbMultiThread.Text = "Multithreaded mass compile";
            this.cbMultiThread.UseVisualStyleBackColor = true;
            // 
            // cbAutoOpenMessages
            // 
            this.cbAutoOpenMessages.AutoSize = true;
            this.cbAutoOpenMessages.Location = new System.Drawing.Point(18, 251);
            this.cbAutoOpenMessages.Name = "cbAutoOpenMessages";
            this.cbAutoOpenMessages.Size = new System.Drawing.Size(141, 17);
            this.cbAutoOpenMessages.TabIndex = 9;
            this.cbAutoOpenMessages.Text = "Auto-open message files";
            this.cbAutoOpenMessages.UseVisualStyleBackColor = true;
            // 
            // bScriptsH
            // 
            this.bScriptsH.Image = ((System.Drawing.Image)(resources.GetObject("bScriptsH.Image")));
            this.bScriptsH.Location = new System.Drawing.Point(446, 68);
            this.bScriptsH.Name = "bScriptsH";
            this.bScriptsH.Size = new System.Drawing.Size(30, 25);
            this.bScriptsH.TabIndex = 10;
            this.toolTip.SetToolTip(this.bScriptsH, "Change folder");
            this.bScriptsH.UseVisualStyleBackColor = true;
            this.bScriptsH.Click += new System.EventHandler(this.bScriptsH_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "h";
            this.openFileDialog1.FileName = "scripts.h";
            this.openFileDialog1.Filter = "Header files|*.h";
            this.openFileDialog1.RestoreDirectory = true;
            this.openFileDialog1.Title = "Select Headers folder";
            // 
            // tbLanguage
            // 
            this.tbLanguage.Location = new System.Drawing.Point(224, 249);
            this.tbLanguage.MaxLength = 8;
            this.tbLanguage.Name = "tbLanguage";
            this.tbLanguage.Size = new System.Drawing.Size(90, 20);
            this.tbLanguage.TabIndex = 13;
            this.toolTip.SetToolTip(this.tbLanguage, "Msg files folder language, default \'english\'.");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(165, 252);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Msg lang:";
            // 
            // cbOptimize
            // 
            this.cbOptimize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOptimize.FormattingEnabled = true;
            this.cbOptimize.Items.AddRange(new object[] {
            "None",
            "Basic",
            "Full",
            "Experimental"});
            this.cbOptimize.Location = new System.Drawing.Point(146, 17);
            this.cbOptimize.Name = "cbOptimize";
            this.cbOptimize.Size = new System.Drawing.Size(95, 21);
            this.cbOptimize.TabIndex = 15;
            this.toolTip.SetToolTip(this.cbOptimize, resources.GetString("cbOptimize.ToolTip"));
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(245, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Optimization";
            this.toolTip.SetToolTip(this.label6, "Compile optimization script");
            // 
            // tbTabSize
            // 
            this.tbTabSize.Location = new System.Drawing.Point(343, 104);
            this.tbTabSize.MaxLength = 8;
            this.tbTabSize.Name = "tbTabSize";
            this.tbTabSize.Size = new System.Drawing.Size(40, 20);
            this.tbTabSize.TabIndex = 17;
            this.tbTabSize.TextChanged += new System.EventHandler(this.TbTabSizeTextChanged);
            // 
            // cbTabsToSpaces
            // 
            this.cbTabsToSpaces.AutoSize = true;
            this.cbTabsToSpaces.Location = new System.Drawing.Point(343, 83);
            this.cbTabsToSpaces.Name = "cbTabsToSpaces";
            this.cbTabsToSpaces.Size = new System.Drawing.Size(135, 17);
            this.cbTabsToSpaces.TabIndex = 18;
            this.cbTabsToSpaces.Text = "Convert tabs to spaces";
            this.cbTabsToSpaces.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(389, 107);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(91, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "tab size in spaces";
            // 
            // cbEnableParser
            // 
            this.cbEnableParser.AutoSize = true;
            this.cbEnableParser.Location = new System.Drawing.Point(6, 17);
            this.cbEnableParser.Name = "cbEnableParser";
            this.cbEnableParser.Size = new System.Drawing.Size(65, 17);
            this.cbEnableParser.TabIndex = 8;
            this.cbEnableParser.Text = "Enabled";
            this.toolTip.SetToolTip(this.cbEnableParser, "Enable parsing currently opened scripts.\r\nThis includes \"Find declaration\", \"Find" +
                    " references\" and similar functions,\r\nas well as right panel with program globals" +
                    ".");
            this.cbEnableParser.UseVisualStyleBackColor = true;
            this.cbEnableParser.CheckedChanged += new System.EventHandler(this.cbEnableParser_CheckedChanged);
            // 
            // cbShortCircuit
            // 
            this.cbShortCircuit.AutoSize = true;
            this.cbShortCircuit.Location = new System.Drawing.Point(146, 44);
            this.cbShortCircuit.Name = "cbShortCircuit";
            this.cbShortCircuit.Size = new System.Drawing.Size(134, 17);
            this.cbShortCircuit.TabIndex = 20;
            this.cbShortCircuit.Text = "Short-circuit evaluation";
            this.toolTip.SetToolTip(this.cbShortCircuit, resources.GetString("cbShortCircuit.ToolTip"));
            this.cbShortCircuit.UseVisualStyleBackColor = true;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 30000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.IsBalloon = true;
            this.toolTip.ReshowDelay = 100;
            // 
            // cbAutocomplete
            // 
            this.cbAutocomplete.AutoSize = true;
            this.cbAutocomplete.Location = new System.Drawing.Point(343, 60);
            this.cbAutocomplete.Name = "cbAutocomplete";
            this.cbAutocomplete.Size = new System.Drawing.Size(126, 17);
            this.cbAutocomplete.TabIndex = 21;
            this.cbAutocomplete.Text = "Enable autocomplete";
            this.toolTip.SetToolTip(this.cbAutocomplete, "Enable displaying box with a list of suggested procedure names, \r\nvariables, cons" +
                    "tants and macros as you type.");
            this.cbAutocomplete.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(419, 342);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 24;
            this.button1.Text = "OK";
            this.toolTip.SetToolTip(this.button1, "Close and save settings");
            this.button1.UseVisualStyleBackColor = true;
            // 
            // cbParserWarn
            // 
            this.cbParserWarn.AutoSize = true;
            this.cbParserWarn.BackColor = System.Drawing.Color.Transparent;
            this.cbParserWarn.Location = new System.Drawing.Point(85, 17);
            this.cbParserWarn.Name = "cbParserWarn";
            this.cbParserWarn.Size = new System.Drawing.Size(66, 17);
            this.cbParserWarn.TabIndex = 30;
            this.cbParserWarn.Text = "Warning";
            this.toolTip.SetToolTip(this.cbParserWarn, "Show parser warnings in log messages.");
            this.cbParserWarn.UseVisualStyleBackColor = false;
            // 
            // cbWatcom
            // 
            this.cbWatcom.AutoSize = true;
            this.cbWatcom.Location = new System.Drawing.Point(146, 67);
            this.cbWatcom.Name = "cbWatcom";
            this.cbWatcom.Size = new System.Drawing.Size(152, 17);
            this.cbWatcom.TabIndex = 21;
            this.cbWatcom.Text = "Use Watcom preprocessor";
            this.toolTip.SetToolTip(this.cbWatcom, "Use preprocessor OpenWatcom before compiling script.");
            this.cbWatcom.UseVisualStyleBackColor = true;
            // 
            // cbCompilePath
            // 
            this.cbCompilePath.AutoSize = true;
            this.cbCompilePath.Location = new System.Drawing.Point(303, 15);
            this.cbCompilePath.Name = "cbCompilePath";
            this.cbCompilePath.Size = new System.Drawing.Size(134, 17);
            this.cbCompilePath.TabIndex = 14;
            this.cbCompilePath.Text = "Not use path compiling";
            this.toolTip.SetToolTip(this.cbCompilePath, "Compile scripts into same folder where source ssl file.");
            this.cbCompilePath.UseVisualStyleBackColor = true;
            this.cbCompilePath.CheckedChanged += new System.EventHandler(this.cbCompilePath_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.msgPathlistView);
            this.groupBox3.Location = new System.Drawing.Point(12, 271);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(400, 100);
            this.groupBox3.TabIndex = 27;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MSG Files Path";
            this.toolTip.SetToolTip(this.groupBox3, "Paths to folders in which editor will search for associated message files.");
            // 
            // msgPathlistView
            // 
            this.msgPathlistView.BackColor = System.Drawing.SystemColors.Window;
            this.msgPathlistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.msgPathlistView.ContextMenuStrip = this.MsgcontextMenu;
            this.msgPathlistView.FullRowSelect = true;
            this.msgPathlistView.GridLines = true;
            this.msgPathlistView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.msgPathlistView.Location = new System.Drawing.Point(6, 19);
            this.msgPathlistView.MultiSelect = false;
            this.msgPathlistView.Name = "msgPathlistView";
            this.msgPathlistView.ShowGroups = false;
            this.msgPathlistView.ShowItemToolTips = true;
            this.msgPathlistView.Size = new System.Drawing.Size(388, 75);
            this.msgPathlistView.TabIndex = 15;
            this.toolTip.SetToolTip(this.msgPathlistView, "Paths for search to message files.");
            this.msgPathlistView.UseCompatibleStateImageBehavior = false;
            this.msgPathlistView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 380;
            // 
            // MsgcontextMenu
            // 
            this.MsgcontextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPathToolStripMenuItem,
            this.deletePathToolStripMenuItem,
            this.toolStripSeparator1,
            this.moveUpToolStripMenuItem,
            this.modeDownToolStripMenuItem});
            this.MsgcontextMenu.Name = "MsgcontextMenu";
            this.MsgcontextMenu.ShowImageMargin = false;
            this.MsgcontextMenu.Size = new System.Drawing.Size(176, 98);
            // 
            // addPathToolStripMenuItem
            // 
            this.addPathToolStripMenuItem.Name = "addPathToolStripMenuItem";
            this.addPathToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.addPathToolStripMenuItem.Text = "Add path";
            this.addPathToolStripMenuItem.Click += new System.EventHandler(this.addPathToolStripMenuItem_Click);
            // 
            // deletePathToolStripMenuItem
            // 
            this.deletePathToolStripMenuItem.Name = "deletePathToolStripMenuItem";
            this.deletePathToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deletePathToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.deletePathToolStripMenuItem.Text = "Delete path";
            this.deletePathToolStripMenuItem.Click += new System.EventHandler(this.deletePathToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // modeDownToolStripMenuItem
            // 
            this.modeDownToolStripMenuItem.Name = "modeDownToolStripMenuItem";
            this.modeDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.modeDownToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.modeDownToolStripMenuItem.Text = "Move Down";
            this.modeDownToolStripMenuItem.Click += new System.EventHandler(this.modeDownToolStripMenuItem_Click);
            // 
            // bAssociate
            // 
            this.bAssociate.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.bAssociate.Location = new System.Drawing.Point(419, 318);
            this.bAssociate.Name = "bAssociate";
            this.bAssociate.Size = new System.Drawing.Size(16, 16);
            this.bAssociate.TabIndex = 32;
            this.bAssociate.Text = "A";
            this.toolTip.SetToolTip(this.bAssociate, "Associate SSL, INT and MSG files with the sfall editor.");
            this.bAssociate.UseVisualStyleBackColor = true;
            this.bAssociate.Click += new System.EventHandler(this.bAssociate_Click);
            // 
            // cbUserCompile
            // 
            this.cbUserCompile.AutoSize = true;
            this.cbUserCompile.Location = new System.Drawing.Point(6, 20);
            this.cbUserCompile.Name = "cbUserCompile";
            this.cbUserCompile.Size = new System.Drawing.Size(125, 17);
            this.cbUserCompile.TabIndex = 22;
            this.cbUserCompile.Text = "Compile from cmd-file";
            this.toolTip.SetToolTip(this.cbUserCompile, "To use custom command batch file (UserComp.bat from resources folder) to compile " +
                    "scripts files.");
            this.cbUserCompile.UseVisualStyleBackColor = true;
            this.cbUserCompile.CheckedChanged += new System.EventHandler(this.cbUserCompile_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbUserCompile);
            this.groupBox1.Controls.Add(this.cbWatcom);
            this.groupBox1.Controls.Add(this.cbShortCircuit);
            this.groupBox1.Controls.Add(this.cbWarnings);
            this.groupBox1.Controls.Add(this.cbDebug);
            this.groupBox1.Controls.Add(this.cbWarnFailedCompile);
            this.groupBox1.Controls.Add(this.cbMultiThread);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cbOptimize);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(319, 120);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Compiling";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbCompilePath);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.cbIncludePath);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(label1);
            this.groupBox2.Controls.Add(this.bChange);
            this.groupBox2.Controls.Add(this.bScriptsH);
            this.groupBox2.Controls.Add(label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 138);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(482, 104);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Path";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.textBox2.Location = new System.Drawing.Point(6, 32);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(434, 20);
            this.textBox2.TabIndex = 13;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.textBox1.Location = new System.Drawing.Point(6, 71);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(434, 20);
            this.textBox1.TabIndex = 13;
            // 
            // HintLang_comboBox
            // 
            this.HintLang_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HintLang_comboBox.FormattingEnabled = true;
            this.HintLang_comboBox.Items.AddRange(new object[] {
            "English",
            "Russian",
            "Chinese"});
            this.HintLang_comboBox.Location = new System.Drawing.Point(419, 248);
            this.HintLang_comboBox.Name = "HintLang_comboBox";
            this.HintLang_comboBox.Size = new System.Drawing.Size(75, 21);
            this.HintLang_comboBox.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(355, 252);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Language:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Highlight_comboBox
            // 
            this.Highlight_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Highlight_comboBox.FormattingEnabled = true;
            this.Highlight_comboBox.Items.AddRange(new object[] {
            "Original",
            "F-Geck"});
            this.Highlight_comboBox.Location = new System.Drawing.Point(418, 290);
            this.Highlight_comboBox.Name = "Highlight_comboBox";
            this.Highlight_comboBox.Size = new System.Drawing.Size(76, 21);
            this.Highlight_comboBox.TabIndex = 28;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(418, 274);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "SSL Highlight:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbParserWarn);
            this.groupBox4.Controls.Add(this.cbEnableParser);
            this.groupBox4.Location = new System.Drawing.Point(337, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(157, 40);
            this.groupBox4.TabIndex = 31;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Parser";
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 376);
            this.Controls.Add(this.bAssociate);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Highlight_comboBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.tbLanguage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.HintLang_comboBox);
            this.Controls.Add(this.cbAutoOpenMessages);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbAutocomplete);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cbTabsToSpaces);
            this.Controls.Add(this.tbTabSize);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsDialog_FormClosing);
            this.groupBox3.ResumeLayout(false);
            this.MsgcontextMenu.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.CheckBox cbAutocomplete;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox cbShortCircuit;
        private System.Windows.Forms.CheckBox cbEnableParser;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbTabSize;
        private System.Windows.Forms.CheckBox cbTabsToSpaces;

        #endregion

        private System.Windows.Forms.CheckBox cbWarnings;
        private System.Windows.Forms.CheckBox cbDebug;
        private System.Windows.Forms.CheckBox cbIncludePath;
        private System.Windows.Forms.Button bChange;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox cbWarnFailedCompile;
        private System.Windows.Forms.CheckBox cbMultiThread;
        private System.Windows.Forms.CheckBox cbAutoOpenMessages;
        private System.Windows.Forms.Button bScriptsH;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox tbLanguage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbOptimize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox HintLang_comboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView msgPathlistView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ComboBox Highlight_comboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbParserWarn;
        private System.Windows.Forms.CheckBox cbWatcom;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbCompilePath;
        private System.Windows.Forms.ContextMenuStrip MsgcontextMenu;
        private System.Windows.Forms.ToolStripMenuItem addPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deletePathToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeDownToolStripMenuItem;
        private System.Windows.Forms.Button bAssociate;
        private System.Windows.Forms.CheckBox cbUserCompile;
    }
}