namespace ScriptEditor
{
    partial class ProcForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbAfterSelProcedure = new System.Windows.Forms.RadioButton();
            this.rbPasteAtEnd = new System.Windows.Forms.RadioButton();
            this.cbCopyBodyProc = new System.Windows.Forms.CheckBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.Cancel = new System.Windows.Forms.Button();
            this.Create = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbAfterSelProcedure);
            this.groupBox1.Controls.Add(this.rbPasteAtEnd);
            this.groupBox1.Controls.Add(this.cbCopyBodyProc);
            this.groupBox1.Location = new System.Drawing.Point(12, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(265, 65);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Procedure options";
            // 
            // rbAfterSelProcedure
            // 
            this.rbAfterSelProcedure.AutoSize = true;
            this.rbAfterSelProcedure.Location = new System.Drawing.Point(141, 19);
            this.rbAfterSelProcedure.Name = "rbAfterSelProcedure";
            this.rbAfterSelProcedure.Size = new System.Drawing.Size(118, 17);
            this.rbAfterSelProcedure.TabIndex = 2;
            this.rbAfterSelProcedure.Text = "Insert after selected";
            this.rbAfterSelProcedure.UseVisualStyleBackColor = true;
            // 
            // rbPasteAtEnd
            // 
            this.rbPasteAtEnd.AutoSize = true;
            this.rbPasteAtEnd.Checked = true;
            this.rbPasteAtEnd.Location = new System.Drawing.Point(16, 19);
            this.rbPasteAtEnd.Name = "rbPasteAtEnd";
            this.rbPasteAtEnd.Size = new System.Drawing.Size(113, 17);
            this.rbPasteAtEnd.TabIndex = 1;
            this.rbPasteAtEnd.TabStop = true;
            this.rbPasteAtEnd.Text = "Paste at end script";
            this.rbPasteAtEnd.UseVisualStyleBackColor = true;
            // 
            // cbCopyBodyProc
            // 
            this.cbCopyBodyProc.AutoSize = true;
            this.cbCopyBodyProc.Location = new System.Drawing.Point(16, 42);
            this.cbCopyBodyProc.Name = "cbCopyBodyProc";
            this.cbCopyBodyProc.Size = new System.Drawing.Size(203, 17);
            this.cbCopyBodyProc.TabIndex = 0;
            this.cbCopyBodyProc.Text = "Copy from current selected procedure";
            this.cbCopyBodyProc.UseVisualStyleBackColor = true;
            // 
            // tbName
            // 
            this.tbName.HideSelection = false;
            this.tbName.Location = new System.Drawing.Point(12, 12);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(346, 20);
            this.tbName.TabIndex = 1;
            this.tbName.WordWrap = false;
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(283, 80);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 3;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            // 
            // Create
            // 
            this.Create.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Create.Location = new System.Drawing.Point(283, 38);
            this.Create.Name = "Create";
            this.Create.Size = new System.Drawing.Size(75, 23);
            this.Create.TabIndex = 2;
            this.Create.Text = "Create";
            this.Create.UseVisualStyleBackColor = true;
            // 
            // ProcForm
            // 
            this.AcceptButton = this.Create;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(370, 114);
            this.Controls.Add(this.Create);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProcForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Procedure";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProcForm_FormClosing);
            this.Shown += new System.EventHandler(this.ProcForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancel;
        protected internal System.Windows.Forms.GroupBox groupBox1;
        protected internal System.Windows.Forms.Button Create;
        private System.Windows.Forms.RadioButton rbPasteAtEnd;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.RadioButton rbAfterSelProcedure;
        private System.Windows.Forms.CheckBox cbCopyBodyProc;
    }
}