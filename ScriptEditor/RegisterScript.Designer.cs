namespace ScriptEditor {
    partial class RegisterScript {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RegisterScript));
            this.dgvScripts = new System.Windows.Forms.DataGridView();
            this.EntryCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLine = new System.Windows.Forms.DataGridViewButtonColumn();
            this.cScript = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cVars = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.Addbutton = new System.Windows.Forms.Button();
            this.FindtextBox = new System.Windows.Forms.TextBox();
            this.Downbutton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Save_button = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.Delbutton = new System.Windows.Forms.Button();
            this.Upbutton = new System.Windows.Forms.Button();
            this.label = new System.Windows.Forms.Label();
            this.DefinetextBox = new System.Windows.Forms.TextBox();
            this.AllowCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScripts)).BeginInit();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvScripts
            // 
            this.dgvScripts.AllowUserToAddRows = false;
            this.dgvScripts.AllowUserToDeleteRows = false;
            this.dgvScripts.AllowUserToResizeRows = false;
            this.dgvScripts.BackgroundColor = System.Drawing.SystemColors.MenuBar;
            this.dgvScripts.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvScripts.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.dgvScripts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScripts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.EntryCol,
            this.cLine,
            this.cScript,
            this.cDescription,
            this.cVars,
            this.cName});
            this.dgvScripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvScripts.Location = new System.Drawing.Point(6, 19);
            this.dgvScripts.MultiSelect = false;
            this.dgvScripts.Name = "dgvScripts";
            this.dgvScripts.RowHeadersVisible = false;
            this.dgvScripts.RowHeadersWidth = 30;
            this.dgvScripts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvScripts.Size = new System.Drawing.Size(515, 438);
            this.dgvScripts.TabIndex = 0;
            this.dgvScripts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvScripts_CellClick);
            // 
            // EntryCol
            // 
            this.EntryCol.HeaderText = "Entry";
            this.EntryCol.Name = "EntryCol";
            this.EntryCol.ReadOnly = true;
            this.EntryCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EntryCol.Visible = false;
            this.EntryCol.Width = 10;
            // 
            // cLine
            // 
            this.cLine.HeaderText = "Script #";
            this.cLine.Name = "cLine";
            this.cLine.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cLine.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.cLine.Width = 50;
            // 
            // cScript
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.cScript.DefaultCellStyle = dataGridViewCellStyle1;
            this.cScript.HeaderText = "Script File";
            this.cScript.Name = "cScript";
            this.cScript.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cScript.Width = 90;
            // 
            // cDescription
            // 
            this.cDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cDescription.FillWeight = 70F;
            this.cDescription.HeaderText = "Descriptions";
            this.cDescription.Name = "cDescription";
            this.cDescription.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // cVars
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.cVars.DefaultCellStyle = dataGridViewCellStyle2;
            this.cVars.HeaderText = "LVars:";
            this.cVars.Name = "cVars";
            this.cVars.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.cVars.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cVars.ToolTipText = "Local Variables";
            this.cVars.Width = 40;
            // 
            // cName
            // 
            this.cName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cName.FillWeight = 30F;
            this.cName.HeaderText = "Script Name";
            this.cName.Name = "cName";
            this.cName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cName.ToolTipText = "Script game name in scrname.msg";
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.dgvScripts);
            this.groupBox.Location = new System.Drawing.Point(9, 64);
            this.groupBox.Name = "groupBox";
            this.groupBox.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox.Size = new System.Drawing.Size(527, 463);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Scripts List";
            // 
            // Addbutton
            // 
            this.Addbutton.Enabled = false;
            this.Addbutton.Image = ((System.Drawing.Image)(resources.GetObject("Addbutton.Image")));
            this.Addbutton.Location = new System.Drawing.Point(93, 9);
            this.Addbutton.Name = "Addbutton";
            this.Addbutton.Size = new System.Drawing.Size(30, 24);
            this.Addbutton.TabIndex = 2;
            this.Addbutton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.Addbutton, "Add new line to end.");
            this.Addbutton.UseVisualStyleBackColor = true;
            this.Addbutton.Click += new System.EventHandler(this.Addbutton_Click);
            // 
            // FindtextBox
            // 
            this.FindtextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.FindtextBox.BackColor = System.Drawing.SystemColors.Info;
            this.FindtextBox.Location = new System.Drawing.Point(167, 12);
            this.FindtextBox.Name = "FindtextBox";
            this.FindtextBox.Size = new System.Drawing.Size(284, 20);
            this.FindtextBox.TabIndex = 3;
            // 
            // Downbutton
            // 
            this.Downbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Downbutton.Image = ((System.Drawing.Image)(resources.GetObject("Downbutton.Image")));
            this.Downbutton.Location = new System.Drawing.Point(457, 9);
            this.Downbutton.Name = "Downbutton";
            this.Downbutton.Size = new System.Drawing.Size(30, 24);
            this.Downbutton.TabIndex = 4;
            this.toolTip1.SetToolTip(this.Downbutton, "Find Down");
            this.Downbutton.UseVisualStyleBackColor = true;
            this.Downbutton.Click += new System.EventHandler(this.Downbutton_Click);
            // 
            // Save_button
            // 
            this.Save_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Save_button.ImageIndex = 0;
            this.Save_button.ImageList = this.imageList1;
            this.Save_button.Location = new System.Drawing.Point(9, 9);
            this.Save_button.Name = "Save_button";
            this.Save_button.Size = new System.Drawing.Size(78, 24);
            this.Save_button.TabIndex = 6;
            this.Save_button.Text = "Register";
            this.Save_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.Save_button, "Save all changes to files.");
            this.Save_button.UseVisualStyleBackColor = true;
            this.Save_button.Click += new System.EventHandler(this.Save_button_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "RegisterReady.png");
            this.imageList1.Images.SetKeyName(1, "RegisterNeed.png");
            // 
            // Delbutton
            // 
            this.Delbutton.Enabled = false;
            this.Delbutton.Image = ((System.Drawing.Image)(resources.GetObject("Delbutton.Image")));
            this.Delbutton.Location = new System.Drawing.Point(128, 9);
            this.Delbutton.Name = "Delbutton";
            this.Delbutton.Size = new System.Drawing.Size(30, 24);
            this.Delbutton.TabIndex = 2;
            this.Delbutton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.Delbutton, "Delete last line.");
            this.Delbutton.UseVisualStyleBackColor = true;
            this.Delbutton.Click += new System.EventHandler(this.Delbutton_Click);
            // 
            // Upbutton
            // 
            this.Upbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Upbutton.Image = ((System.Drawing.Image)(resources.GetObject("Upbutton.Image")));
            this.Upbutton.Location = new System.Drawing.Point(506, 9);
            this.Upbutton.Name = "Upbutton";
            this.Upbutton.Size = new System.Drawing.Size(30, 24);
            this.Upbutton.TabIndex = 4;
            this.Upbutton.UseVisualStyleBackColor = true;
            this.Upbutton.Click += new System.EventHandler(this.Upbutton_Click);
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label.BackColor = System.Drawing.Color.Transparent;
            this.label.Image = ((System.Drawing.Image)(resources.GetObject("label.Image")));
            this.label.Location = new System.Drawing.Point(483, 11);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(26, 22);
            this.label.TabIndex = 5;
            // 
            // DefinetextBox
            // 
            this.DefinetextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DefinetextBox.Enabled = false;
            this.DefinetextBox.Location = new System.Drawing.Point(167, 38);
            this.DefinetextBox.Name = "DefinetextBox";
            this.DefinetextBox.Size = new System.Drawing.Size(284, 20);
            this.DefinetextBox.TabIndex = 7;
            // 
            // AllowCheckBox
            // 
            this.AllowCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AllowCheckBox.AutoSize = true;
            this.AllowCheckBox.Enabled = false;
            this.AllowCheckBox.ForeColor = System.Drawing.Color.Firebrick;
            this.AllowCheckBox.Location = new System.Drawing.Point(457, 41);
            this.AllowCheckBox.Name = "AllowCheckBox";
            this.AllowCheckBox.Size = new System.Drawing.Size(51, 17);
            this.AllowCheckBox.TabIndex = 8;
            this.AllowCheckBox.Text = "Allow";
            this.AllowCheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label2.Location = new System.Drawing.Point(23, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Added #define to Scripts.h:";
            // 
            // RegisterScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 536);
            this.Controls.Add(this.AllowCheckBox);
            this.Controls.Add(this.DefinetextBox);
            this.Controls.Add(this.Save_button);
            this.Controls.Add(this.Upbutton);
            this.Controls.Add(this.Downbutton);
            this.Controls.Add(this.FindtextBox);
            this.Controls.Add(this.Delbutton);
            this.Controls.Add(this.Addbutton);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.label);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(250, 250);
            this.Name = "RegisterScript";
            this.Padding = new System.Windows.Forms.Padding(6);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Script Register Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RegisterScript_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RegisterScript_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScripts)).EndInit();
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvScripts;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Button Addbutton;
        private System.Windows.Forms.TextBox FindtextBox;
        private System.Windows.Forms.Button Downbutton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button Upbutton;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button Save_button;
        private System.Windows.Forms.TextBox DefinetextBox;
        private System.Windows.Forms.CheckBox AllowCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Delbutton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.DataGridViewTextBoxColumn EntryCol;
        private System.Windows.Forms.DataGridViewButtonColumn cLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn cScript;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn cVars;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
    }
}