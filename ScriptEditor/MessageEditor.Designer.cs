namespace ScriptEditor {
    partial class MessageEditor {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageEditor));
            this.dgvMessage = new System.Windows.Forms.DataGridView();
            this.EntryCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playerMarkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.NewStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.msgOpenButton = new System.Windows.Forms.ToolStripSplitButton();
            this.msgSaveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveAsStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.SendStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.MoveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.IncAddStripButton = new System.Windows.Forms.ToolStripButton();
            this.InsertEmptyStripButton = new System.Windows.Forms.ToolStripButton();
            this.InsertCommentStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.DeleteLineStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.SearchStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.NextStripButton = new System.Windows.Forms.ToolStripButton();
            this.BackStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.StripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.alwaysOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLIPColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.encodingTextDOSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessage)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvMessage
            // 
            this.dgvMessage.AllowUserToAddRows = false;
            this.dgvMessage.AllowUserToDeleteRows = false;
            this.dgvMessage.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.dgvMessage.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvMessage.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dgvMessage.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvMessage.ColumnHeadersHeight = 22;
            this.dgvMessage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvMessage.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.EntryCol,
            this.cLine,
            this.cDescription,
            this.cLip});
            this.dgvMessage.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvMessage.DefaultCellStyle = dataGridViewCellStyle11;
            this.dgvMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMessage.EnableHeadersVisualStyles = false;
            this.dgvMessage.GridColor = System.Drawing.Color.Silver;
            this.dgvMessage.Location = new System.Drawing.Point(4, 17);
            this.dgvMessage.MultiSelect = false;
            this.dgvMessage.Name = "dgvMessage";
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dgvMessage.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dgvMessage.RowHeadersVisible = false;
            this.dgvMessage.RowTemplate.Height = 18;
            this.dgvMessage.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMessage.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvMessage.Size = new System.Drawing.Size(819, 541);
            this.dgvMessage.TabIndex = 0;
            this.dgvMessage.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvMessage_CellBeginEdit);
            this.dgvMessage.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMessage_CellEndEdit);
            this.dgvMessage.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMessage_CellValueChanged);
            this.dgvMessage.SelectionChanged += new System.EventHandler(this.dgvMessage_SelectionChanged);
            this.dgvMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvMessage_KeyPress);
            // 
            // EntryCol
            // 
            this.EntryCol.HeaderText = "Entry";
            this.EntryCol.Name = "EntryCol";
            this.EntryCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EntryCol.Visible = false;
            this.EntryCol.Width = 47;
            // 
            // cLine
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.cLine.DefaultCellStyle = dataGridViewCellStyle8;
            this.cLine.HeaderText = "Line";
            this.cLine.Name = "cLine";
            this.cLine.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cLine.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cLine.ToolTipText = "Msg line number";
            this.cLine.Width = 40;
            // 
            // cDescription
            // 
            this.cDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.cDescription.DefaultCellStyle = dataGridViewCellStyle9;
            this.cDescription.HeaderText = "Message or comment text";
            this.cDescription.Name = "cDescription";
            this.cDescription.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cLip
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.GrayText;
            this.cLip.DefaultCellStyle = dataGridViewCellStyle10;
            this.cLip.HeaderText = "Lip File";
            this.cLip.Name = "cLip";
            this.cLip.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cLip.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cLip.Width = 60;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playerMarkerToolStripMenuItem,
            this.sendLineToolStripMenuItem,
            this.toolStripSeparator11,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(176, 98);
            // 
            // playerMarkerToolStripMenuItem
            // 
            this.playerMarkerToolStripMenuItem.Name = "playerMarkerToolStripMenuItem";
            this.playerMarkerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
            this.playerMarkerToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.playerMarkerToolStripMenuItem.Text = "Player Marker";
            this.playerMarkerToolStripMenuItem.ToolTipText = "Set player options marker";
            this.playerMarkerToolStripMenuItem.Click += new System.EventHandler(this.playerMarkerToolStripMenuItem_Click);
            // 
            // sendLineToolStripMenuItem
            // 
            this.sendLineToolStripMenuItem.Name = "sendLineToolStripMenuItem";
            this.sendLineToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.sendLineToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.sendLineToolStripMenuItem.Text = "Send Line";
            this.sendLineToolStripMenuItem.ToolTipText = "Send current line number to an open script.";
            this.sendLineToolStripMenuItem.Click += new System.EventHandler(this.SendStripButton_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(172, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.dgvMessage);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(0, 26);
            this.groupBox.Name = "groupBox";
            this.groupBox.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox.Size = new System.Drawing.Size(827, 562);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Messages";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewStripButton,
            this.toolStripSeparator7,
            this.msgOpenButton,
            this.msgSaveButton,
            this.toolStripSeparator8,
            this.SaveAsStripButton,
            this.toolStripSeparator1,
            this.SendStripButton,
            this.toolStripSeparator9,
            this.MoveToolStripButton,
            this.toolStripSeparator4,
            this.IncAddStripButton,
            this.InsertEmptyStripButton,
            this.InsertCommentStripButton,
            this.toolStripSeparator5,
            this.DeleteLineStripButton,
            this.toolStripSeparator2,
            this.SearchStripTextBox,
            this.NextStripButton,
            this.BackStripButton,
            this.toolStripSeparator3,
            this.StripComboBox,
            this.toolStripSeparator10,
            this.toolStripDropDownButton1,
            this.toolStripDropDownButton2});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(827, 26);
            this.toolStrip.TabIndex = 7;
            this.toolStrip.Text = "toolStrip1";
            // 
            // NewStripButton
            // 
            this.NewStripButton.AutoSize = false;
            this.NewStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NewStripButton.Image = ((System.Drawing.Image)(resources.GetObject("NewStripButton.Image")));
            this.NewStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NewStripButton.Name = "NewStripButton";
            this.NewStripButton.Size = new System.Drawing.Size(25, 23);
            this.NewStripButton.ToolTipText = "Clear & New";
            this.NewStripButton.Click += new System.EventHandler(this.NewStripButton_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.AutoSize = false;
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(10, 26);
            // 
            // msgOpenButton
            // 
            this.msgOpenButton.Image = ((System.Drawing.Image)(resources.GetObject("msgOpenButton.Image")));
            this.msgOpenButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.msgOpenButton.Name = "msgOpenButton";
            this.msgOpenButton.Size = new System.Drawing.Size(65, 23);
            this.msgOpenButton.Text = "Open";
            this.msgOpenButton.ToolTipText = "Open message file";
            this.msgOpenButton.ButtonClick += new System.EventHandler(this.msgOpenButton_ButtonClick);
            // 
            // msgSaveButton
            // 
            this.msgSaveButton.Enabled = false;
            this.msgSaveButton.Image = ((System.Drawing.Image)(resources.GetObject("msgSaveButton.Image")));
            this.msgSaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.msgSaveButton.Name = "msgSaveButton";
            this.msgSaveButton.Size = new System.Drawing.Size(51, 23);
            this.msgSaveButton.Text = "Save";
            this.msgSaveButton.ToolTipText = "Save messageg file";
            this.msgSaveButton.Click += new System.EventHandler(this.msgSaveButton_ButtonClick);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.AutoSize = false;
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(10, 26);
            // 
            // SaveAsStripButton
            // 
            this.SaveAsStripButton.AutoSize = false;
            this.SaveAsStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveAsStripButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveAsStripButton.Image")));
            this.SaveAsStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveAsStripButton.Name = "SaveAsStripButton";
            this.SaveAsStripButton.Size = new System.Drawing.Size(25, 23);
            this.SaveAsStripButton.Text = "Save As...";
            this.SaveAsStripButton.Click += new System.EventHandler(this.SaveAsStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AutoSize = false;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(10, 25);
            // 
            // SendStripButton
            // 
            this.SendStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SendStripButton.Image = ((System.Drawing.Image)(resources.GetObject("SendStripButton.Image")));
            this.SendStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SendStripButton.Name = "SendStripButton";
            this.SendStripButton.Size = new System.Drawing.Size(23, 23);
            this.SendStripButton.ToolTipText = "Send current line number to an open script [Alt+S]";
            this.SendStripButton.Click += new System.EventHandler(this.SendStripButton_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 26);
            // 
            // MoveToolStripButton
            // 
            this.MoveToolStripButton.CheckOnClick = true;
            this.MoveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MoveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("MoveToolStripButton.Image")));
            this.MoveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MoveToolStripButton.Name = "MoveToolStripButton";
            this.MoveToolStripButton.Size = new System.Drawing.Size(23, 23);
            this.MoveToolStripButton.ToolTipText = "Enable multi selected for move and deleting rows.";
            this.MoveToolStripButton.Click += new System.EventHandler(this.MoveToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.AutoSize = false;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(10, 26);
            // 
            // IncAddStripButton
            // 
            this.IncAddStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.IncAddStripButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IncAddStripButton.Image = ((System.Drawing.Image)(resources.GetObject("IncAddStripButton.Image")));
            this.IncAddStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.IncAddStripButton.Name = "IncAddStripButton";
            this.IncAddStripButton.Size = new System.Drawing.Size(23, 23);
            this.IncAddStripButton.ToolTipText = "Add next number line [Alt+A]";
            this.IncAddStripButton.Click += new System.EventHandler(this.IncAddStripButton_Click);
            // 
            // InsertEmptyStripButton
            // 
            this.InsertEmptyStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.InsertEmptyStripButton.Image = ((System.Drawing.Image)(resources.GetObject("InsertEmptyStripButton.Image")));
            this.InsertEmptyStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.InsertEmptyStripButton.Name = "InsertEmptyStripButton";
            this.InsertEmptyStripButton.Size = new System.Drawing.Size(23, 23);
            this.InsertEmptyStripButton.ToolTipText = "Add empty line [Enter]";
            this.InsertEmptyStripButton.Click += new System.EventHandler(this.InsertEmptyStripButton_Click);
            // 
            // InsertCommentStripButton
            // 
            this.InsertCommentStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.InsertCommentStripButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.InsertCommentStripButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.InsertCommentStripButton.Image = ((System.Drawing.Image)(resources.GetObject("InsertCommentStripButton.Image")));
            this.InsertCommentStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.InsertCommentStripButton.Name = "InsertCommentStripButton";
            this.InsertCommentStripButton.Size = new System.Drawing.Size(26, 23);
            this.InsertCommentStripButton.Text = "#";
            this.InsertCommentStripButton.ToolTipText = "Insert comment line";
            this.InsertCommentStripButton.Click += new System.EventHandler(this.InsertCommentStripButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.AutoSize = false;
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(10, 26);
            // 
            // DeleteLineStripButton
            // 
            this.DeleteLineStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.DeleteLineStripButton.Image = ((System.Drawing.Image)(resources.GetObject("DeleteLineStripButton.Image")));
            this.DeleteLineStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DeleteLineStripButton.Name = "DeleteLineStripButton";
            this.DeleteLineStripButton.Size = new System.Drawing.Size(23, 23);
            this.DeleteLineStripButton.ToolTipText = "Delete current line [Delete]";
            this.DeleteLineStripButton.Click += new System.EventHandler(this.DeleteLineStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(10, 25);
            // 
            // SearchStripTextBox
            // 
            this.SearchStripTextBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.SearchStripTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.SearchStripTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SearchStripTextBox.Name = "SearchStripTextBox";
            this.SearchStripTextBox.Size = new System.Drawing.Size(250, 26);
            this.SearchStripTextBox.ToolTipText = "Search text";
            // 
            // NextStripButton
            // 
            this.NextStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.NextStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NextStripButton.Image = ((System.Drawing.Image)(resources.GetObject("NextStripButton.Image")));
            this.NextStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NextStripButton.Name = "NextStripButton";
            this.NextStripButton.Size = new System.Drawing.Size(23, 23);
            this.NextStripButton.ToolTipText = "Next find";
            this.NextStripButton.Click += new System.EventHandler(this.Downbutton_Click);
            // 
            // BackStripButton
            // 
            this.BackStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.BackStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BackStripButton.Image = ((System.Drawing.Image)(resources.GetObject("BackStripButton.Image")));
            this.BackStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BackStripButton.Name = "BackStripButton";
            this.BackStripButton.Size = new System.Drawing.Size(23, 23);
            this.BackStripButton.ToolTipText = "Back find";
            this.BackStripButton.Click += new System.EventHandler(this.Upbutton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 26);
            // 
            // StripComboBox
            // 
            this.StripComboBox.AutoSize = false;
            this.StripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.StripComboBox.Items.AddRange(new object[] {
            "1",
            "5",
            "10",
            "20",
            "30",
            "40",
            "50"});
            this.StripComboBox.Name = "StripComboBox";
            this.StripComboBox.Size = new System.Drawing.Size(40, 21);
            this.StripComboBox.ToolTipText = "The line number after the comment is increased by this number.";
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.AutoSize = false;
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(10, 26);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alwaysOnTopToolStripMenuItem,
            this.showLIPColumnToolStripMenuItem,
            this.toolStripSeparator6,
            this.encodingTextDOSToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(57, 23);
            this.toolStripDropDownButton1.Text = "Options";
            // 
            // alwaysOnTopToolStripMenuItem
            // 
            this.alwaysOnTopToolStripMenuItem.CheckOnClick = true;
            this.alwaysOnTopToolStripMenuItem.Name = "alwaysOnTopToolStripMenuItem";
            this.alwaysOnTopToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.alwaysOnTopToolStripMenuItem.Text = "Editor On Top";
            this.alwaysOnTopToolStripMenuItem.CheckedChanged += new System.EventHandler(this.alwaysOnTopToolStripMenuItem_CheckedChanged);
            // 
            // showLIPColumnToolStripMenuItem
            // 
            this.showLIPColumnToolStripMenuItem.Checked = true;
            this.showLIPColumnToolStripMenuItem.CheckOnClick = true;
            this.showLIPColumnToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showLIPColumnToolStripMenuItem.Name = "showLIPColumnToolStripMenuItem";
            this.showLIPColumnToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.showLIPColumnToolStripMenuItem.Text = "Show LIP Column";
            this.showLIPColumnToolStripMenuItem.Click += new System.EventHandler(this.showLIPColumnToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(175, 6);
            // 
            // encodingTextDOSToolStripMenuItem
            // 
            this.encodingTextDOSToolStripMenuItem.CheckOnClick = true;
            this.encodingTextDOSToolStripMenuItem.Name = "encodingTextDOSToolStripMenuItem";
            this.encodingTextDOSToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.encodingTextDOSToolStripMenuItem.Text = "Encoding: OEM 866";
            this.encodingTextDOSToolStripMenuItem.ToolTipText = "Read and write Msg files in cyrillic encoding OEM 866.";
            this.encodingTextDOSToolStripMenuItem.Click += new System.EventHandler(this.encodingTextDOSToolStripMenuItem_Click);
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(29, 23);
            this.toolStripDropDownButton2.Text = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Visible = false;
            this.toolStripDropDownButton2.Click += new System.EventHandler(this.SendStripButton_Click);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.addToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.addToolStripMenuItem.Text = "add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.IncAddStripButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "msg";
            this.openFileDialog.Filter = "Message files|*.msg";
            this.openFileDialog.InitialDirectory = "D:\\";
            this.openFileDialog.RestoreDirectory = true;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Message files|*.msg";
            // 
            // MessageEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(827, 588);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.toolStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(250, 250);
            this.Name = "MessageEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = " - Message Editor";
            this.Text = " - Message Editor";
            this.Deactivate += new System.EventHandler(this.MessageEditor_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MessageEditor_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MessageEditor_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMessage)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMessage;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton IncAddStripButton;
        private System.Windows.Forms.ToolStripButton InsertCommentStripButton;
        private System.Windows.Forms.ToolStripButton InsertEmptyStripButton;
        private System.Windows.Forms.ToolStripButton SendStripButton;
        private System.Windows.Forms.ToolStripButton DeleteLineStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton NextStripButton;
        private System.Windows.Forms.ToolStripButton BackStripButton;
        private System.Windows.Forms.ToolStripTextBox SearchStripTextBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem showLIPColumnToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton msgSaveButton;
        private System.Windows.Forms.ToolStripButton SaveAsStripButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn EntryCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLip;
        private System.Windows.Forms.ToolStripButton NewStripButton;
        private System.Windows.Forms.ToolStripSplitButton msgOpenButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripComboBox StripComboBox;
        private System.Windows.Forms.ToolStripMenuItem encodingTextDOSToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton MoveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem playerMarkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alwaysOnTopToolStripMenuItem;
    }
}