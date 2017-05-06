namespace ScriptEditor {
    partial class SearchForm {
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
            System.Windows.Forms.Label label1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.cbRegular = new System.Windows.Forms.CheckBox();
            this.rbCurrent = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.rbFolder = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.bChange = new System.Windows.Forms.Button();
            this.cbSearchSubfolders = new System.Windows.Forms.CheckBox();
            this.bSearch = new System.Windows.Forms.Button();
            this.fbdSearchFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.cbFindAll = new System.Windows.Forms.CheckBox();
            this.tbReplace = new System.Windows.Forms.TextBox();
            this.bReplace = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(13, 141);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(86, 13);
            label1.TabIndex = 5;
            label1.Text = "Folder to search:";
            // 
            // tbSearch
            // 
            this.tbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearch.Location = new System.Drawing.Point(13, 23);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(379, 20);
            this.tbSearch.TabIndex = 0;
            // 
            // cbRegular
            // 
            this.cbRegular.AutoSize = true;
            this.cbRegular.Location = new System.Drawing.Point(13, 49);
            this.cbRegular.Name = "cbRegular";
            this.cbRegular.Size = new System.Drawing.Size(116, 17);
            this.cbRegular.TabIndex = 1;
            this.cbRegular.Text = "Regular expression";
            this.cbRegular.UseVisualStyleBackColor = true;
            // 
            // rbCurrent
            // 
            this.rbCurrent.AutoSize = true;
            this.rbCurrent.Checked = true;
            this.rbCurrent.Location = new System.Drawing.Point(149, 49);
            this.rbCurrent.Name = "rbCurrent";
            this.rbCurrent.Size = new System.Drawing.Size(112, 17);
            this.rbCurrent.TabIndex = 2;
            this.rbCurrent.TabStop = true;
            this.rbCurrent.Text = "Current  document";
            this.rbCurrent.UseVisualStyleBackColor = true;
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Location = new System.Drawing.Point(149, 72);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(113, 17);
            this.rbAll.TabIndex = 3;
            this.rbAll.Text = "All open document";
            this.rbAll.UseVisualStyleBackColor = true;
            // 
            // rbFolder
            // 
            this.rbFolder.AutoSize = true;
            this.rbFolder.Location = new System.Drawing.Point(149, 134);
            this.rbFolder.Name = "rbFolder";
            this.rbFolder.Size = new System.Drawing.Size(86, 17);
            this.rbFolder.TabIndex = 4;
            this.rbFolder.Text = "Files in folder";
            this.rbFolder.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Replace Text:";
            // 
            // bChange
            // 
            this.bChange.Enabled = false;
            this.bChange.Image = ((System.Drawing.Image)(resources.GetObject("bChange.Image")));
            this.bChange.Location = new System.Drawing.Point(13, 183);
            this.bChange.Name = "bChange";
            this.bChange.Size = new System.Drawing.Size(104, 23);
            this.bChange.TabIndex = 7;
            this.bChange.Text = "Change";
            this.bChange.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bChange.UseVisualStyleBackColor = true;
            // 
            // cbSearchSubfolders
            // 
            this.cbSearchSubfolders.AutoSize = true;
            this.cbSearchSubfolders.Enabled = false;
            this.cbSearchSubfolders.Location = new System.Drawing.Point(149, 187);
            this.cbSearchSubfolders.Name = "cbSearchSubfolders";
            this.cbSearchSubfolders.Size = new System.Drawing.Size(111, 17);
            this.cbSearchSubfolders.TabIndex = 8;
            this.cbSearchSubfolders.Text = "Search subfolders";
            this.cbSearchSubfolders.UseVisualStyleBackColor = true;
            // 
            // bSearch
            // 
            this.bSearch.Image = ((System.Drawing.Image)(resources.GetObject("bSearch.Image")));
            this.bSearch.Location = new System.Drawing.Point(288, 49);
            this.bSearch.Name = "bSearch";
            this.bSearch.Size = new System.Drawing.Size(104, 23);
            this.bSearch.TabIndex = 10;
            this.bSearch.Text = "Search";
            this.bSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bSearch.UseVisualStyleBackColor = true;
            // 
            // fbdSearchFolder
            // 
            this.fbdSearchFolder.Description = "Pick folder to search";
            this.fbdSearchFolder.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // cbFindAll
            // 
            this.cbFindAll.AutoSize = true;
            this.cbFindAll.Location = new System.Drawing.Point(13, 72);
            this.cbFindAll.Name = "cbFindAll";
            this.cbFindAll.Size = new System.Drawing.Size(102, 17);
            this.cbFindAll.TabIndex = 11;
            this.cbFindAll.Text = "Find all matches";
            this.cbFindAll.UseVisualStyleBackColor = true;
            // 
            // tbReplace
            // 
            this.tbReplace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbReplace.Location = new System.Drawing.Point(13, 108);
            this.tbReplace.Name = "tbReplace";
            this.tbReplace.Size = new System.Drawing.Size(379, 20);
            this.tbReplace.TabIndex = 12;
            // 
            // bReplace
            // 
            this.bReplace.Location = new System.Drawing.Point(288, 79);
            this.bReplace.Name = "bReplace";
            this.bReplace.Size = new System.Drawing.Size(104, 23);
            this.bReplace.TabIndex = 13;
            this.bReplace.Text = "Find && Replace";
            this.bReplace.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.textBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBox1.Location = new System.Drawing.Point(13, 157);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ShortcutsEnabled = false;
            this.textBox1.Size = new System.Drawing.Size(379, 20);
            this.textBox1.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Find Text:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(288, 183);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 215);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bReplace);
            this.Controls.Add(this.tbReplace);
            this.Controls.Add(this.cbFindAll);
            this.Controls.Add(this.bSearch);
            this.Controls.Add(this.cbSearchSubfolders);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.bChange);
            this.Controls.Add(this.label2);
            this.Controls.Add(label1);
            this.Controls.Add(this.rbFolder);
            this.Controls.Add(this.rbAll);
            this.Controls.Add(this.rbCurrent);
            this.Controls.Add(this.cbRegular);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.tbSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "SearchForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search & Replace";
            this.Activated += new System.EventHandler(this.SearchForm_Activated);
            this.Deactivate += new System.EventHandler(this.SearchForm_Deactivate);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox tbSearch;
        internal System.Windows.Forms.CheckBox cbRegular;
        internal System.Windows.Forms.RadioButton rbCurrent;
        internal System.Windows.Forms.RadioButton rbAll;
        internal System.Windows.Forms.RadioButton rbFolder;
        internal System.Windows.Forms.Button bChange;
        internal System.Windows.Forms.CheckBox cbSearchSubfolders;
        internal System.Windows.Forms.Button bSearch;
        internal System.Windows.Forms.FolderBrowserDialog fbdSearchFolder;
        internal System.Windows.Forms.CheckBox cbFindAll;
        internal System.Windows.Forms.TextBox tbReplace;
        internal System.Windows.Forms.Button bReplace;
        internal System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;

    }
}