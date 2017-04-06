namespace ScriptEditor {
    partial class TextEditor {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEditor));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbAutocomplete = new System.Windows.Forms.ListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.Split_button = new System.Windows.Forms.Button();
            this.TabClose_button = new System.Windows.Forms.Button();
            this.tabControl1 = new DraggableTabControl();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.minimizelog_button = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgvErrors = new System.Windows.Forms.DataGridView();
            this.cType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tbOutputParse = new System.Windows.Forms.TextBox();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.ProcTree = new System.Windows.Forms.TreeView();
            this.ProcMnContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createProcedureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameProcedureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.moveProcedureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteProcedureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.FunctionsTree = new System.Windows.Forms.TreeView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.LineStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ColStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.New_toolStripDropDownButton = new System.Windows.Forms.ToolStripSplitButton();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TemplateScript_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.Open_toolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.Save_toolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.Save_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAs_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAll_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.saveAsTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.Outline_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.Undo_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Redo_ToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.Searh_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.Back_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Forward_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.GotoProc_toolStripButton = new System.Windows.Forms.ToolStripSplitButton();
            this.gotoToLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.Script_toolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.editRegisteredScriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Headers_toolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.openAllIncludesScriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openHeaderFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.MSG_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.qCompile_toolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.Compile_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CompileAllOpen_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MassCompile_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Preprocess_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.roundtripToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.About_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Help_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.splitDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Settings_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.showLogWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsTabControls = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllButThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdScripts = new System.Windows.Forms.OpenFileDialog();
            this.sfdScripts = new System.Windows.Forms.SaveFileDialog();
            this.fbdMassCompile = new System.Windows.Forms.FolderBrowserDialog();
            this.bwSyntaxParser = new System.ComponentModel.BackgroundWorker();
            this.editorMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.UpperCaseToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.LowerCaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.findReferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findDeclerationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openIncludeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTipAC = new System.Windows.Forms.ToolTip(this.components);
            this.panel1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.ProcMnContext.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.ToolStrip.SuspendLayout();
            this.cmsTabControls.SuspendLayout();
            this.editorMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbAutocomplete);
            this.panel1.Controls.Add(this.splitContainer2);
            this.panel1.Controls.Add(this.ToolStrip);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(896, 707);
            this.panel1.TabIndex = 2;
            // 
            // lbAutocomplete
            // 
            this.lbAutocomplete.BackColor = System.Drawing.SystemColors.Info;
            this.lbAutocomplete.Cursor = System.Windows.Forms.Cursors.Help;
            this.lbAutocomplete.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbAutocomplete.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbAutocomplete.FormattingEnabled = true;
            this.lbAutocomplete.ItemHeight = 16;
            this.lbAutocomplete.Location = new System.Drawing.Point(631, -6);
            this.lbAutocomplete.Name = "lbAutocomplete";
            this.lbAutocomplete.Size = new System.Drawing.Size(132, 20);
            this.lbAutocomplete.TabIndex = 5;
            this.lbAutocomplete.Visible = false;
            this.lbAutocomplete.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lbAutocomplete_PasteOpcode);
            this.lbAutocomplete.SelectedIndexChanged += new System.EventHandler(this.LbAutocompleteSelectedIndexChanged);
            this.lbAutocomplete.VisibleChanged += new System.EventHandler(this.LbAutocompleteVisibleChanged);
            this.lbAutocomplete.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LbAutocompleteKeyDown);
            this.lbAutocomplete.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbAutocomplete_MouseMove);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 25);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer1);
            this.splitContainer2.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.Panel1MinSize = 680;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tabControl3);
            this.splitContainer2.Panel2.Controls.Add(this.statusStrip);
            this.splitContainer2.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.Panel2MinSize = 125;
            this.splitContainer2.Size = new System.Drawing.Size(896, 682);
            this.splitContainer2.SplitterDistance = 701;
            this.splitContainer2.SplitterWidth = 3;
            this.splitContainer2.TabIndex = 4;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.Split_button);
            this.splitContainer1.Panel1.Controls.Add(this.TabClose_button);
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Panel1MinSize = 500;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.minimizelog_button);
            this.splitContainer1.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Size = new System.Drawing.Size(701, 682);
            this.splitContainer1.SplitterDistance = 550;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 3;
            // 
            // Split_button
            // 
            this.Split_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Split_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Split_button.ForeColor = System.Drawing.Color.DarkRed;
            this.Split_button.Location = new System.Drawing.Point(681, 530);
            this.Split_button.Name = "Split_button";
            this.Split_button.Size = new System.Drawing.Size(16, 16);
            this.Split_button.TabIndex = 2;
            this.Split_button.Text = "^";
            this.toolTipAC.SetToolTip(this.Split_button, "Split document viewer");
            this.Split_button.UseVisualStyleBackColor = true;
            this.Split_button.Visible = false;
            this.Split_button.Click += new System.EventHandler(this.SplitDoc_Click);
            // 
            // TabClose_button
            // 
            this.TabClose_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TabClose_button.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.TabClose_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TabClose_button.ForeColor = System.Drawing.Color.DarkRed;
            this.TabClose_button.Image = ((System.Drawing.Image)(resources.GetObject("TabClose_button.Image")));
            this.TabClose_button.Location = new System.Drawing.Point(680, 22);
            this.TabClose_button.Name = "TabClose_button";
            this.TabClose_button.Size = new System.Drawing.Size(18, 18);
            this.TabClose_button.TabIndex = 0;
            this.TabClose_button.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTipAC.SetToolTip(this.TabClose_button, "Close this document");
            this.TabClose_button.UseVisualStyleBackColor = true;
            this.TabClose_button.Visible = false;
            this.TabClose_button.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.AllowDrop = true;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.ShowToolTips = true;
            this.tabControl1.Size = new System.Drawing.Size(701, 550);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            this.tabControl1.DragDrop += new System.Windows.Forms.DragEventHandler(this.TextEditorDragDrop);
            this.tabControl1.DragEnter += new System.Windows.Forms.DragEventHandler(this.TextEditorDragEnter);
            this.tabControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "save.png");
            this.imageList1.Images.SetKeyName(1, "nosave.png");
            // 
            // minimizelog_button
            // 
            this.minimizelog_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimizelog_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.minimizelog_button.Image = ((System.Drawing.Image)(resources.GetObject("minimizelog_button.Image")));
            this.minimizelog_button.Location = new System.Drawing.Point(678, 0);
            this.minimizelog_button.Name = "minimizelog_button";
            this.minimizelog_button.Size = new System.Drawing.Size(20, 20);
            this.minimizelog_button.TabIndex = 6;
            this.minimizelog_button.Tag = "0";
            this.toolTipAC.SetToolTip(this.minimizelog_button, "Minimize Log");
            this.minimizelog_button.UseVisualStyleBackColor = true;
            this.minimizelog_button.Click += new System.EventHandler(this.minimize_log_button_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Controls.Add(this.tabPage2);
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Multiline = true;
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(701, 130);
            this.tabControl2.TabIndex = 1;
            this.tabControl2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabControl2_MouseClick);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbOutput);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(693, 104);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Build output";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbOutput
            // 
            this.tbOutput.AcceptsReturn = true;
            this.tbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbOutput.Location = new System.Drawing.Point(0, 0);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutput.Size = new System.Drawing.Size(693, 104);
            this.tbOutput.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvErrors);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(693, 104);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Errors";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvErrors
            // 
            this.dgvErrors.AllowUserToAddRows = false;
            this.dgvErrors.AllowUserToDeleteRows = false;
            this.dgvErrors.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvErrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvErrors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cType,
            this.cFile,
            this.cLine,
            this.cMessage});
            this.dgvErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvErrors.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvErrors.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dgvErrors.Location = new System.Drawing.Point(0, 0);
            this.dgvErrors.MultiSelect = false;
            this.dgvErrors.Name = "dgvErrors";
            this.dgvErrors.ReadOnly = true;
            this.dgvErrors.RowHeadersVisible = false;
            this.dgvErrors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvErrors.Size = new System.Drawing.Size(693, 104);
            this.dgvErrors.TabIndex = 0;
            this.dgvErrors.DoubleClick += new System.EventHandler(this.dgvErrors_DoubleClick);
            // 
            // cType
            // 
            this.cType.HeaderText = "Type";
            this.cType.Name = "cType";
            this.cType.ReadOnly = true;
            this.cType.Width = 80;
            // 
            // cFile
            // 
            this.cFile.HeaderText = "File";
            this.cFile.Name = "cFile";
            this.cFile.ReadOnly = true;
            // 
            // cLine
            // 
            this.cLine.HeaderText = "Line";
            this.cLine.Name = "cLine";
            this.cLine.ReadOnly = true;
            this.cLine.Width = 40;
            // 
            // cMessage
            // 
            this.cMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cMessage.HeaderText = "Message";
            this.cMessage.Name = "cMessage";
            this.cMessage.ReadOnly = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tbOutputParse);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(693, 104);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Parser output";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tbOutputParse
            // 
            this.tbOutputParse.AcceptsReturn = true;
            this.tbOutputParse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbOutputParse.Location = new System.Drawing.Point(0, 0);
            this.tbOutputParse.Margin = new System.Windows.Forms.Padding(0);
            this.tbOutputParse.Multiline = true;
            this.tbOutputParse.Name = "tbOutputParse";
            this.tbOutputParse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutputParse.Size = new System.Drawing.Size(693, 104);
            this.tbOutputParse.TabIndex = 1;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tabPage4);
            this.tabControl3.Controls.Add(this.tabPage6);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl3.Location = new System.Drawing.Point(0, 0);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(192, 660);
            this.tabControl3.TabIndex = 1;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage4.Controls.Add(this.ProcTree);
            this.tabPage4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabPage4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(184, 634);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "Procedures";
            // 
            // ProcTree
            // 
            this.ProcTree.ContextMenuStrip = this.ProcMnContext;
            this.ProcTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ProcTree.HideSelection = false;
            this.ProcTree.Location = new System.Drawing.Point(3, 3);
            this.ProcTree.Name = "ProcTree";
            this.ProcTree.ShowNodeToolTips = true;
            this.ProcTree.ShowRootLines = false;
            this.ProcTree.Size = new System.Drawing.Size(178, 628);
            this.ProcTree.TabIndex = 0;
            this.ProcTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);
            // 
            // ProcMnContext
            // 
            this.ProcMnContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createProcedureToolStripMenuItem,
            this.renameProcedureToolStripMenuItem,
            this.toolStripSeparator20,
            this.moveProcedureToolStripMenuItem,
            this.deleteProcedureToolStripMenuItem});
            this.ProcMnContext.Name = "ProcMnContext";
            this.ProcMnContext.Size = new System.Drawing.Size(177, 120);
            this.ProcMnContext.Opening += new System.ComponentModel.CancelEventHandler(this.ProcMnContext_Opening);
            // 
            // createProcedureToolStripMenuItem
            // 
            this.createProcedureToolStripMenuItem.Name = "createProcedureToolStripMenuItem";
            this.createProcedureToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.createProcedureToolStripMenuItem.Text = "Create Procedure";
            this.createProcedureToolStripMenuItem.Click += new System.EventHandler(this.createProcedureToolStripMenuItem_Click);
            // 
            // renameProcedureToolStripMenuItem
            // 
            this.renameProcedureToolStripMenuItem.Enabled = false;
            this.renameProcedureToolStripMenuItem.Name = "renameProcedureToolStripMenuItem";
            this.renameProcedureToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.renameProcedureToolStripMenuItem.Text = "Rename Procedure";
            this.renameProcedureToolStripMenuItem.Click += new System.EventHandler(this.renameProcedureToolStripMenuItem_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(173, 6);
            // 
            // moveProcedureToolStripMenuItem
            // 
            this.moveProcedureToolStripMenuItem.Enabled = false;
            this.moveProcedureToolStripMenuItem.Name = "moveProcedureToolStripMenuItem";
            this.moveProcedureToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.moveProcedureToolStripMenuItem.Text = "Move Procedure";
            // 
            // deleteProcedureToolStripMenuItem
            // 
            this.deleteProcedureToolStripMenuItem.Enabled = false;
            this.deleteProcedureToolStripMenuItem.Name = "deleteProcedureToolStripMenuItem";
            this.deleteProcedureToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.deleteProcedureToolStripMenuItem.Text = "Delete Procedure";
            this.deleteProcedureToolStripMenuItem.Click += new System.EventHandler(this.deleteProcedureToolStripMenuItem_Click);
            // 
            // tabPage6
            // 
            this.tabPage6.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage6.Controls.Add(this.FunctionsTree);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(184, 634);
            this.tabPage6.TabIndex = 2;
            this.tabPage6.Text = "Functions";
            // 
            // FunctionsTree
            // 
            this.FunctionsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FunctionsTree.Location = new System.Drawing.Point(3, 3);
            this.FunctionsTree.Name = "FunctionsTree";
            this.FunctionsTree.Size = new System.Drawing.Size(178, 628);
            this.FunctionsTree.TabIndex = 0;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel3,
            this.LineStripStatusLabel,
            this.ColStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 660);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(192, 22);
            this.statusStrip.TabIndex = 2;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.AutoSize = false;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(10, 17);
            // 
            // LineStripStatusLabel
            // 
            this.LineStripStatusLabel.AutoSize = false;
            this.LineStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.LineStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.LineStripStatusLabel.Name = "LineStripStatusLabel";
            this.LineStripStatusLabel.Size = new System.Drawing.Size(83, 17);
            this.LineStripStatusLabel.Spring = true;
            this.LineStripStatusLabel.Text = "Line: 1";
            this.LineStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // ColStripStatusLabel
            // 
            this.ColStripStatusLabel.AutoSize = false;
            this.ColStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.ColStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.ColStripStatusLabel.Name = "ColStripStatusLabel";
            this.ColStripStatusLabel.Size = new System.Drawing.Size(83, 17);
            this.ColStripStatusLabel.Spring = true;
            this.ColStripStatusLabel.Text = "Col: 1";
            this.ColStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // ToolStrip
            // 
            this.ToolStrip.AutoSize = false;
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.New_toolStripDropDownButton,
            this.toolStripSeparator7,
            this.Open_toolStripSplitButton,
            this.Save_toolStripSplitButton,
            this.toolStripSeparator8,
            this.Outline_toolStripButton,
            this.toolStripSeparator17,
            this.Undo_toolStripButton,
            this.Redo_ToolStripButton,
            this.toolStripSeparator11,
            this.Searh_toolStripButton,
            this.toolStripSeparator12,
            this.Back_toolStripButton,
            this.Forward_toolStripButton,
            this.GotoProc_toolStripButton,
            this.toolStripSeparator10,
            this.Script_toolStripSplitButton,
            this.Headers_toolStripSplitButton,
            this.toolStripSeparator16,
            this.toolStripButton3,
            this.MSG_toolStripButton,
            this.toolStripSeparator9,
            this.qCompile_toolStripSplitButton,
            this.toolStripSeparator13,
            this.About_toolStripButton,
            this.Help_toolStripButton,
            this.toolStripSeparator4,
            this.toolStripDropDownButton2,
            this.toolStripSeparator14});
            this.ToolStrip.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(896, 25);
            this.ToolStrip.TabIndex = 2;
            this.ToolStrip.Text = "toolStrip2";
            // 
            // New_toolStripDropDownButton
            // 
            this.New_toolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.New_toolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.TemplateScript_ToolStripMenuItem,
            this.toolStripSeparator19});
            this.New_toolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("New_toolStripDropDownButton.Image")));
            this.New_toolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.New_toolStripDropDownButton.Name = "New_toolStripDropDownButton";
            this.New_toolStripDropDownButton.Size = new System.Drawing.Size(32, 22);
            this.New_toolStripDropDownButton.Text = "CreateNew";
            this.New_toolStripDropDownButton.ToolTipText = "Create new script [Ctrl+N]";
            this.New_toolStripDropDownButton.ButtonClick += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.newToolStripMenuItem.Text = "New Script";
            this.newToolStripMenuItem.Visible = false;
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // TemplateScript_ToolStripMenuItem
            // 
            this.TemplateScript_ToolStripMenuItem.Enabled = false;
            this.TemplateScript_ToolStripMenuItem.Name = "TemplateScript_ToolStripMenuItem";
            this.TemplateScript_ToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.TemplateScript_ToolStripMenuItem.Text = "Tepmplates Scripts";
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(172, 6);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // Open_toolStripSplitButton
            // 
            this.Open_toolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.toolStripSeparator18});
            this.Open_toolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("Open_toolStripSplitButton.Image")));
            this.Open_toolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Open_toolStripSplitButton.Name = "Open_toolStripSplitButton";
            this.Open_toolStripSplitButton.Size = new System.Drawing.Size(65, 22);
            this.Open_toolStripSplitButton.Text = "Open";
            this.Open_toolStripSplitButton.ToolTipText = "Quick open script [Ctrl+O]";
            this.Open_toolStripSplitButton.ButtonClick += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Visible = false;
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.Enabled = false;
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.recentToolStripMenuItem.Text = "Recent Files";
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(148, 6);
            // 
            // Save_toolStripSplitButton
            // 
            this.Save_toolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Save_toolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Save_ToolStripMenuItem,
            this.SaveAs_ToolStripMenuItem,
            this.SaveAll_ToolStripMenuItem,
            this.toolStripSeparator15,
            this.saveAsTemplateToolStripMenuItem});
            this.Save_toolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("Save_toolStripSplitButton.Image")));
            this.Save_toolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Save_toolStripSplitButton.Name = "Save_toolStripSplitButton";
            this.Save_toolStripSplitButton.Size = new System.Drawing.Size(32, 22);
            this.Save_toolStripSplitButton.Text = "Quick Save";
            this.Save_toolStripSplitButton.ToolTipText = "Quick save script [Ctrl+S]";
            this.Save_toolStripSplitButton.ButtonClick += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // Save_ToolStripMenuItem
            // 
            this.Save_ToolStripMenuItem.Name = "Save_ToolStripMenuItem";
            this.Save_ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.Save_ToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.Save_ToolStripMenuItem.Text = "Save";
            this.Save_ToolStripMenuItem.Visible = false;
            this.Save_ToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // SaveAs_ToolStripMenuItem
            // 
            this.SaveAs_ToolStripMenuItem.Name = "SaveAs_ToolStripMenuItem";
            this.SaveAs_ToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.SaveAs_ToolStripMenuItem.Text = "Save Script as";
            this.SaveAs_ToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // SaveAll_ToolStripMenuItem
            // 
            this.SaveAll_ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SaveAll_ToolStripMenuItem.Image")));
            this.SaveAll_ToolStripMenuItem.Name = "SaveAll_ToolStripMenuItem";
            this.SaveAll_ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.SaveAll_ToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.SaveAll_ToolStripMenuItem.Text = "Save All";
            this.SaveAll_ToolStripMenuItem.Click += new System.EventHandler(this.saveAllToolStripMenuItem_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(188, 6);
            // 
            // saveAsTemplateToolStripMenuItem
            // 
            this.saveAsTemplateToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveAsTemplateToolStripMenuItem.Image")));
            this.saveAsTemplateToolStripMenuItem.Name = "saveAsTemplateToolStripMenuItem";
            this.saveAsTemplateToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.saveAsTemplateToolStripMenuItem.Text = "Save as Template";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
            // 
            // Outline_toolStripButton
            // 
            this.Outline_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Outline_toolStripButton.Enabled = false;
            this.Outline_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Outline_toolStripButton.Image")));
            this.Outline_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Outline_toolStripButton.Name = "Outline_toolStripButton";
            this.Outline_toolStripButton.Size = new System.Drawing.Size(23, 22);
            this.Outline_toolStripButton.Text = "Outline";
            this.Outline_toolStripButton.ToolTipText = "Outline Expand/Collapse";
            this.Outline_toolStripButton.Click += new System.EventHandler(this.outlineToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(6, 25);
            // 
            // Undo_toolStripButton
            // 
            this.Undo_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Undo_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Undo_toolStripButton.Image")));
            this.Undo_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Undo_toolStripButton.Name = "Undo_toolStripButton";
            this.Undo_toolStripButton.Size = new System.Drawing.Size(23, 22);
            this.Undo_toolStripButton.Text = "Undo";
            this.Undo_toolStripButton.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // Redo_ToolStripButton
            // 
            this.Redo_ToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Redo_ToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Redo_ToolStripButton.Image")));
            this.Redo_ToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Redo_ToolStripButton.Name = "Redo_ToolStripButton";
            this.Redo_ToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.Redo_ToolStripButton.Text = "Redo";
            this.Redo_ToolStripButton.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 25);
            // 
            // Searh_toolStripButton
            // 
            this.Searh_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Searh_toolStripButton.Image")));
            this.Searh_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Searh_toolStripButton.Name = "Searh_toolStripButton";
            this.Searh_toolStripButton.Size = new System.Drawing.Size(47, 22);
            this.Searh_toolStripButton.Text = "Find";
            this.Searh_toolStripButton.ToolTipText = "Search & Replace";
            this.Searh_toolStripButton.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 25);
            // 
            // Back_toolStripButton
            // 
            this.Back_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Back_toolStripButton.Enabled = false;
            this.Back_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Back_toolStripButton.Image")));
            this.Back_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Back_toolStripButton.Name = "Back_toolStripButton";
            this.Back_toolStripButton.Size = new System.Drawing.Size(23, 22);
            this.Back_toolStripButton.Text = "Back Position";
            this.Back_toolStripButton.Click += new System.EventHandler(this.Back_toolStripButton_Click);
            // 
            // Forward_toolStripButton
            // 
            this.Forward_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Forward_toolStripButton.Enabled = false;
            this.Forward_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Forward_toolStripButton.Image")));
            this.Forward_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Forward_toolStripButton.Name = "Forward_toolStripButton";
            this.Forward_toolStripButton.Size = new System.Drawing.Size(23, 22);
            this.Forward_toolStripButton.Text = "Forward Position";
            this.Forward_toolStripButton.Click += new System.EventHandler(this.Forward_toolStripButton_Click);
            // 
            // GotoProc_toolStripButton
            // 
            this.GotoProc_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.GotoProc_toolStripButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gotoToLineToolStripMenuItem});
            this.GotoProc_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("GotoProc_toolStripButton.Image")));
            this.GotoProc_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.GotoProc_toolStripButton.Name = "GotoProc_toolStripButton";
            this.GotoProc_toolStripButton.Size = new System.Drawing.Size(32, 22);
            this.GotoProc_toolStripButton.Text = "Goto Procedure";
            this.GotoProc_toolStripButton.ToolTipText = "Goto procedure under cursor";
            // 
            // gotoToLineToolStripMenuItem
            // 
            this.gotoToLineToolStripMenuItem.Name = "gotoToLineToolStripMenuItem";
            this.gotoToLineToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gotoToLineToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.gotoToLineToolStripMenuItem.Text = "Goto Line";
            this.gotoToLineToolStripMenuItem.ToolTipText = "Goto line document";
            this.gotoToLineToolStripMenuItem.Click += new System.EventHandler(this.GoToLineToolStripMenuItemClick);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
            // 
            // Script_toolStripSplitButton
            // 
            this.Script_toolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Script_toolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editRegisteredScriptsToolStripMenuItem});
            this.Script_toolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("Script_toolStripSplitButton.Image")));
            this.Script_toolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Script_toolStripSplitButton.Name = "Script_toolStripSplitButton";
            this.Script_toolStripSplitButton.Size = new System.Drawing.Size(32, 22);
            this.Script_toolStripSplitButton.Text = "Register Script";
            this.Script_toolStripSplitButton.ButtonClick += new System.EventHandler(this.registerScriptToolStripMenuItem_Click);
            // 
            // editRegisteredScriptsToolStripMenuItem
            // 
            this.editRegisteredScriptsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editRegisteredScriptsToolStripMenuItem.Image")));
            this.editRegisteredScriptsToolStripMenuItem.Name = "editRegisteredScriptsToolStripMenuItem";
            this.editRegisteredScriptsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.editRegisteredScriptsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.editRegisteredScriptsToolStripMenuItem.Text = "Edit registered Scripts";
            this.editRegisteredScriptsToolStripMenuItem.Click += new System.EventHandler(this.editRegisteredScriptsToolStripMenuItem_Click);
            // 
            // Headers_toolStripSplitButton
            // 
            this.Headers_toolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Headers_toolStripSplitButton.DropDownButtonWidth = 12;
            this.Headers_toolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openAllIncludesScriptToolStripMenuItem,
            this.openHeaderFileToolStripMenuItem});
            this.Headers_toolStripSplitButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Headers_toolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("Headers_toolStripSplitButton.Image")));
            this.Headers_toolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Headers_toolStripSplitButton.MergeAction = System.Windows.Forms.MergeAction.Remove;
            this.Headers_toolStripSplitButton.Name = "Headers_toolStripSplitButton";
            this.Headers_toolStripSplitButton.Size = new System.Drawing.Size(33, 22);
            this.Headers_toolStripSplitButton.Text = "Headers";
            this.Headers_toolStripSplitButton.ToolTipText = "Quick open header files";
            this.Headers_toolStripSplitButton.ButtonClick += new System.EventHandler(this.Headers_toolStripSplitButton_ButtonClick);
            // 
            // openAllIncludesScriptToolStripMenuItem
            // 
            this.openAllIncludesScriptToolStripMenuItem.Enabled = false;
            this.openAllIncludesScriptToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openAllIncludesScriptToolStripMenuItem.Image")));
            this.openAllIncludesScriptToolStripMenuItem.Name = "openAllIncludesScriptToolStripMenuItem";
            this.openAllIncludesScriptToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.openAllIncludesScriptToolStripMenuItem.Text = "Open all include files";
            this.openAllIncludesScriptToolStripMenuItem.ToolTipText = "Open all include file in this script.";
            this.openAllIncludesScriptToolStripMenuItem.Click += new System.EventHandler(this.openIncludesScriptToolStripMenuItem_Click);
            // 
            // openHeaderFileToolStripMenuItem
            // 
            this.openHeaderFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openHeaderFileToolStripMenuItem.Image")));
            this.openHeaderFileToolStripMenuItem.Name = "openHeaderFileToolStripMenuItem";
            this.openHeaderFileToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.openHeaderFileToolStripMenuItem.Text = "Open Header file";
            this.openHeaderFileToolStripMenuItem.Click += new System.EventHandler(this.openHeaderFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton3.Text = "View Dialog";
            // 
            // MSG_toolStripButton
            // 
            this.MSG_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MSG_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("MSG_toolStripButton.Image")));
            this.MSG_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MSG_toolStripButton.Name = "MSG_toolStripButton";
            this.MSG_toolStripButton.Size = new System.Drawing.Size(23, 22);
            this.MSG_toolStripButton.Text = "View MSG File";
            this.MSG_toolStripButton.Click += new System.EventHandler(this.associateMsgToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
            // 
            // qCompile_toolStripSplitButton
            // 
            this.qCompile_toolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Compile_ToolStripMenuItem,
            this.CompileAllOpen_ToolStripMenuItem,
            this.MassCompile_ToolStripMenuItem,
            this.toolStripSeparator3,
            this.Preprocess_ToolStripMenuItem,
            this.roundtripToolStripMenuItem});
            this.qCompile_toolStripSplitButton.Image = ((System.Drawing.Image)(resources.GetObject("qCompile_toolStripSplitButton.Image")));
            this.qCompile_toolStripSplitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.qCompile_toolStripSplitButton.Name = "qCompile_toolStripSplitButton";
            this.qCompile_toolStripSplitButton.Size = new System.Drawing.Size(76, 22);
            this.qCompile_toolStripSplitButton.Text = "Compile";
            this.qCompile_toolStripSplitButton.ToolTipText = "Quick Compile Script [F8]";
            this.qCompile_toolStripSplitButton.ButtonClick += new System.EventHandler(this.compileToolStripMenuItem1_Click);
            // 
            // Compile_ToolStripMenuItem
            // 
            this.Compile_ToolStripMenuItem.Name = "Compile_ToolStripMenuItem";
            this.Compile_ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.Compile_ToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.Compile_ToolStripMenuItem.Text = "Compile";
            this.Compile_ToolStripMenuItem.Visible = false;
            this.Compile_ToolStripMenuItem.Click += new System.EventHandler(this.compileToolStripMenuItem1_Click);
            // 
            // CompileAllOpen_ToolStripMenuItem
            // 
            this.CompileAllOpen_ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CompileAllOpen_ToolStripMenuItem.Image")));
            this.CompileAllOpen_ToolStripMenuItem.Name = "CompileAllOpen_ToolStripMenuItem";
            this.CompileAllOpen_ToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.CompileAllOpen_ToolStripMenuItem.Text = "Compile All Open";
            this.CompileAllOpen_ToolStripMenuItem.Click += new System.EventHandler(this.compileAllOpenToolStripMenuItem_Click);
            // 
            // MassCompile_ToolStripMenuItem
            // 
            this.MassCompile_ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("MassCompile_ToolStripMenuItem.Image")));
            this.MassCompile_ToolStripMenuItem.Name = "MassCompile_ToolStripMenuItem";
            this.MassCompile_ToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.MassCompile_ToolStripMenuItem.Text = "Mass Compile";
            this.MassCompile_ToolStripMenuItem.Click += new System.EventHandler(this.massCompileToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(162, 6);
            // 
            // Preprocess_ToolStripMenuItem
            // 
            this.Preprocess_ToolStripMenuItem.Name = "Preprocess_ToolStripMenuItem";
            this.Preprocess_ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.Preprocess_ToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.Preprocess_ToolStripMenuItem.Text = "Preprocess";
            this.Preprocess_ToolStripMenuItem.Click += new System.EventHandler(this.preprocessToolStripMenuItem_Click);
            // 
            // roundtripToolStripMenuItem
            // 
            this.roundtripToolStripMenuItem.Name = "roundtripToolStripMenuItem";
            this.roundtripToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.roundtripToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.roundtripToolStripMenuItem.Text = "Roundtrip";
            this.roundtripToolStripMenuItem.Click += new System.EventHandler(this.roundtripToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 25);
            // 
            // About_toolStripButton
            // 
            this.About_toolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.About_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.About_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("About_toolStripButton.Image")));
            this.About_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.About_toolStripButton.Name = "About_toolStripButton";
            this.About_toolStripButton.Size = new System.Drawing.Size(23, 22);
            this.About_toolStripButton.Text = "About";
            this.About_toolStripButton.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // Help_toolStripButton
            // 
            this.Help_toolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Help_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Help_toolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Help_toolStripButton.Image")));
            this.Help_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Help_toolStripButton.Name = "Help_toolStripButton";
            this.Help_toolStripButton.Size = new System.Drawing.Size(23, 22);
            this.Help_toolStripButton.Text = "Help";
            this.Help_toolStripButton.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.splitDocumentToolStripMenuItem,
            this.toolStripSeparator1,
            this.Settings_ToolStripMenuItem,
            this.toolStripSeparator5,
            this.showLogWindowToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(73, 22);
            this.toolStripDropDownButton2.Text = "Options";
            // 
            // splitDocumentToolStripMenuItem
            // 
            this.splitDocumentToolStripMenuItem.Enabled = false;
            this.splitDocumentToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("splitDocumentToolStripMenuItem.Image")));
            this.splitDocumentToolStripMenuItem.Name = "splitDocumentToolStripMenuItem";
            this.splitDocumentToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.splitDocumentToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.splitDocumentToolStripMenuItem.Text = "Split Document";
            this.splitDocumentToolStripMenuItem.ToolTipText = "Split document viewer";
            this.splitDocumentToolStripMenuItem.Click += new System.EventHandler(this.SplitDoc_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(187, 6);
            // 
            // Settings_ToolStripMenuItem
            // 
            this.Settings_ToolStripMenuItem.Name = "Settings_ToolStripMenuItem";
            this.Settings_ToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.Settings_ToolStripMenuItem.Text = "Settings";
            this.Settings_ToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(187, 6);
            // 
            // showLogWindowToolStripMenuItem
            // 
            this.showLogWindowToolStripMenuItem.Checked = true;
            this.showLogWindowToolStripMenuItem.CheckOnClick = true;
            this.showLogWindowToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showLogWindowToolStripMenuItem.Name = "showLogWindowToolStripMenuItem";
            this.showLogWindowToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.showLogWindowToolStripMenuItem.Text = "Log Window";
            this.showLogWindowToolStripMenuItem.ToolTipText = "Show/Hide log window";
            this.showLogWindowToolStripMenuItem.Click += new System.EventHandler(this.showLogWindowToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 25);
            // 
            // cmsTabControls
            // 
            this.cmsTabControls.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem,
            this.closeAllToolStripMenuItem,
            this.closeAllButThisToolStripMenuItem});
            this.cmsTabControls.Name = "cmsTabControls";
            this.cmsTabControls.Size = new System.Drawing.Size(167, 70);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem1_Click);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.closeAllToolStripMenuItem.Text = "Close All";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.CloseAllToolStripMenuItemClick);
            // 
            // closeAllButThisToolStripMenuItem
            // 
            this.closeAllButThisToolStripMenuItem.Name = "closeAllButThisToolStripMenuItem";
            this.closeAllButThisToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.closeAllButThisToolStripMenuItem.Text = "Close All But This";
            this.closeAllButThisToolStripMenuItem.Click += new System.EventHandler(this.CloseAllButThisToolStripMenuItemClick);
            // 
            // ofdScripts
            // 
            this.ofdScripts.Filter = "All supported files|*.ssl;*.int;*.h;*.msg|Script files|*.ssl|Header files|*.h|Com" +
                "piled scripts|*.int|Message files|*.msg|All files|*.*";
            this.ofdScripts.Multiselect = true;
            this.ofdScripts.RestoreDirectory = true;
            this.ofdScripts.Title = "Select script or header to open";
            // 
            // sfdScripts
            // 
            this.sfdScripts.DefaultExt = "ssl";
            this.sfdScripts.Filter = "Script file (.ssl)|*.ssl|Header file (.h)|*.h|All files|*.*";
            this.sfdScripts.RestoreDirectory = true;
            this.sfdScripts.Title = "Save script as";
            // 
            // fbdMassCompile
            // 
            this.fbdMassCompile.Description = "Select folder to compile";
            this.fbdMassCompile.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // bwSyntaxParser
            // 
            this.bwSyntaxParser.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwSyntaxParser_DoWork);
            this.bwSyntaxParser.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwSyntaxParser_RunWorkerCompleted);
            // 
            // editorMenuStrip
            // 
            this.editorMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem1,
            this.copyToolStripMenuItem1,
            this.pasteToolStripMenuItem1,
            this.toolStripSeparator6,
            this.UpperCaseToolStripMenuItem1,
            this.LowerCaseToolStripMenuItem,
            this.toolStripSeparator2,
            this.findReferencesToolStripMenuItem,
            this.findDeclerationToolStripMenuItem,
            this.findDefinitionToolStripMenuItem,
            this.openIncludeToolStripMenuItem});
            this.editorMenuStrip.Name = "editorMenuStrip1";
            this.editorMenuStrip.Size = new System.Drawing.Size(212, 214);
            this.editorMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.editorMenuStrip_Opening);
            // 
            // cutToolStripMenuItem1
            // 
            this.cutToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem1.Image")));
            this.cutToolStripMenuItem1.Name = "cutToolStripMenuItem1";
            this.cutToolStripMenuItem1.Size = new System.Drawing.Size(211, 22);
            this.cutToolStripMenuItem1.Text = "Cut";
            this.cutToolStripMenuItem1.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem1
            // 
            this.copyToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem1.Image")));
            this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
            this.copyToolStripMenuItem1.Size = new System.Drawing.Size(211, 22);
            this.copyToolStripMenuItem1.Text = "Copy";
            this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem1
            // 
            this.pasteToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem1.Image")));
            this.pasteToolStripMenuItem1.Name = "pasteToolStripMenuItem1";
            this.pasteToolStripMenuItem1.Size = new System.Drawing.Size(211, 22);
            this.pasteToolStripMenuItem1.Text = "Paste";
            this.pasteToolStripMenuItem1.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(208, 6);
            // 
            // UpperCaseToolStripMenuItem1
            // 
            this.UpperCaseToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("UpperCaseToolStripMenuItem1.Image")));
            this.UpperCaseToolStripMenuItem1.Name = "UpperCaseToolStripMenuItem1";
            this.UpperCaseToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.A)));
            this.UpperCaseToolStripMenuItem1.Size = new System.Drawing.Size(211, 22);
            this.UpperCaseToolStripMenuItem1.Text = "Upper Case";
            this.UpperCaseToolStripMenuItem1.Click += new System.EventHandler(this.UPPERCASEToolStripMenuItemClick);
            // 
            // LowerCaseToolStripMenuItem
            // 
            this.LowerCaseToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("LowerCaseToolStripMenuItem.Image")));
            this.LowerCaseToolStripMenuItem.Name = "LowerCaseToolStripMenuItem";
            this.LowerCaseToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Z)));
            this.LowerCaseToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.LowerCaseToolStripMenuItem.Text = "Lower Case";
            this.LowerCaseToolStripMenuItem.Click += new System.EventHandler(this.LowecaseToolStripMenuItemClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(208, 6);
            // 
            // findReferencesToolStripMenuItem
            // 
            this.findReferencesToolStripMenuItem.Name = "findReferencesToolStripMenuItem";
            this.findReferencesToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.findReferencesToolStripMenuItem.Text = "Find references";
            this.findReferencesToolStripMenuItem.Click += new System.EventHandler(this.findReferencesToolStripMenuItem_Click);
            // 
            // findDeclerationToolStripMenuItem
            // 
            this.findDeclerationToolStripMenuItem.Name = "findDeclerationToolStripMenuItem";
            this.findDeclerationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F12)));
            this.findDeclerationToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.findDeclerationToolStripMenuItem.Text = "Find declaration";
            this.findDeclerationToolStripMenuItem.Click += new System.EventHandler(this.findDeclerationToolStripMenuItem_Click);
            // 
            // findDefinitionToolStripMenuItem
            // 
            this.findDefinitionToolStripMenuItem.Name = "findDefinitionToolStripMenuItem";
            this.findDefinitionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.findDefinitionToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.findDefinitionToolStripMenuItem.Text = "Find definition";
            this.findDefinitionToolStripMenuItem.Click += new System.EventHandler(this.findDefinitionToolStripMenuItem_Click);
            // 
            // openIncludeToolStripMenuItem
            // 
            this.openIncludeToolStripMenuItem.Name = "openIncludeToolStripMenuItem";
            this.openIncludeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.openIncludeToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.openIncludeToolStripMenuItem.Text = "Open include";
            this.openIncludeToolStripMenuItem.Click += new System.EventHandler(this.openIncludeToolStripMenuItem_Click);
            // 
            // toolTipAC
            // 
            this.toolTipAC.AutoPopDelay = 50000;
            this.toolTipAC.InitialDelay = 500;
            this.toolTipAC.IsBalloon = true;
            this.toolTipAC.ReshowDelay = 100;
            // 
            // TextEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 707);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TextEditor";
            this.Text = "sfall Script Editor";
            this.Activated += new System.EventHandler(this.TextEditor_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextEditor_FormClosing);
            this.Load += new System.EventHandler(this.TextEditor_Load);
            this.panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabControl3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ProcMnContext.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.cmsTabControls.ResumeLayout(false);
            this.editorMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.ToolTip toolTipAC;
        private System.Windows.Forms.ListBox lbAutocomplete;
        private System.Windows.Forms.TextBox tbOutputParse;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllButThisToolStripMenuItem;

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.OpenFileDialog ofdScripts;
        private System.Windows.Forms.SaveFileDialog sfdScripts;
        private DraggableTabControl tabControl1;
        private System.Windows.Forms.FolderBrowserDialog fbdMassCompile;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dgvErrors;
        private System.Windows.Forms.DataGridViewTextBoxColumn cType;
        private System.Windows.Forms.DataGridViewTextBoxColumn cFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn cMessage;
        private System.ComponentModel.BackgroundWorker bwSyntaxParser;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TreeView ProcTree;
        private System.Windows.Forms.ContextMenuStrip editorMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem findReferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findDeclerationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findDefinitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openIncludeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip cmsTabControls;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem UpperCaseToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem LowerCaseToolStripMenuItem;
        private System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button minimizelog_button;
        private System.Windows.Forms.ToolStripButton Undo_toolStripButton;
        private System.Windows.Forms.ToolStripButton Redo_ToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSplitButton Open_toolStripSplitButton;
        private System.Windows.Forms.ToolStripSplitButton Save_toolStripSplitButton;
        private System.Windows.Forms.ToolStripMenuItem SaveAs_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveAll_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripButton Searh_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton Back_toolStripButton;
        private System.Windows.Forms.ToolStripButton Forward_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripSplitButton New_toolStripDropDownButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripSplitButton qCompile_toolStripSplitButton;
        private System.Windows.Forms.ToolStripMenuItem CompileAllOpen_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MassCompile_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripButton About_toolStripButton;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem Save_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem Settings_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem Preprocess_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem showLogWindowToolStripMenuItem;
        private System.Windows.Forms.TreeView FunctionsTree;
        private System.Windows.Forms.ToolStripButton Help_toolStripButton;
        private System.Windows.Forms.ToolStripMenuItem Compile_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton Script_toolStripSplitButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem editRegisteredScriptsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem saveAsTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton MSG_toolStripButton;
        private System.Windows.Forms.ToolStripMenuItem roundtripToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton Outline_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem TemplateScript_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button TabClose_button;
        private System.Windows.Forms.ToolStripSplitButton Headers_toolStripSplitButton;
        private System.Windows.Forms.ToolStripMenuItem openAllIncludesScriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openHeaderFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Button Split_button;
        private System.Windows.Forms.ToolStripMenuItem splitDocumentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.ToolStripSplitButton GotoProc_toolStripButton;
        private System.Windows.Forms.ToolStripMenuItem gotoToLineToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel LineStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel ColStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ContextMenuStrip ProcMnContext;
        private System.Windows.Forms.ToolStripMenuItem createProcedureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameProcedureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveProcedureToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
        private System.Windows.Forms.ToolStripMenuItem deleteProcedureToolStripMenuItem;
    }
}