using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;

using ICSharpCode.TextEditor.Document;
using System.Text.RegularExpressions;

namespace ScriptEditor
{
    public partial class DialogPreview : Form
    {
        private List<DialogueParser> Arguments = new List<DialogueParser>();

        private List<int> nodesNavigation = new List<int>();
        private int currentNavigation = 0;

        private TabInfo sourceTab;
        private IDocument document;
        private Procedure[] scrProc;

        private string msgPath;
        private string[] MessagesData;
        
        private bool allow;
        private bool user;

        public bool InitReady 
        {
            get { return (MessagesData != null); }
        }

        public DialogPreview(TabInfo sourceTab, string msgPath)
        {
            InitializeComponent();
            
            this.Text += sourceTab.filename;
            this.sourceTab = sourceTab;
            this.document = sourceTab.textEditor.Document;
            this.scrProc = sourceTab.parseInfo.procs;
            this.msgPath = msgPath;
            
            Parser.UpdateParseSSL(document.TextContent, true, false);
            foreach (string name in Parser.ProceduresListName)
            {
                if (name.StartsWith("node", StringComparison.OrdinalIgnoreCase) 
                    || name.Equals("talk_p_proc", StringComparison.OrdinalIgnoreCase))
                    NodesComboBox.Items.Add(name);
            }

            Procedure curProc = Parser.GetProcedurePosition(scrProc, sourceTab.textEditor.ActiveTextAreaControl.Caret.Line);
            if (curProc == null || !NodesComboBox.Items.Contains(curProc.name)) {
                int indx = GetProcedureIndex("talk_p_proc");
                if (indx == -1) return;
                curProc = scrProc[indx];
            }
            
            MessagesData = File.ReadAllLines(msgPath, Settings.EncCodePage);

            NodesComboBox.Text = curProc.name;
            nodesNavigation.Add(GetProcedureIndex(curProc.name));
            GotoNode(curProc);
        }

        private void GotoNode(Procedure curProc)
        {
            int offset = document.GetLineSegment(curProc.d.start).Offset;
            LineSegment end = document.GetLineSegment(curProc.d.end - 2);
            int length = (end.Offset + end.Length) - offset;
            Arguments.Clear();
            dgvMessages.Rows.Clear();
            if (length < 10) return;
            ParseNode(document.GetText(offset, length));
            BuildMessageDialog();
        }
        
        private void ParseNode(string text)
        {
            Regex regex = new Regex(OpcodeType.call.ToString(), RegexOptions.IgnoreCase);

            string[] bodyNode = text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in bodyNode)
            {
                string str = line.Trim();

                DialogueParser.ReplySubParse(Arguments, str, OpcodeType.Message);
                DialogueParser.ReplySubParse(Arguments, str, OpcodeType.Reply);
                DialogueParser.OptionSubParse(Arguments, str);
                
                MatchCollection matches = regex.Matches(str);
                foreach (Match m in matches)
                    Arguments.Add(new DialogueParser(str, m.Index + 4));
            }
        }
      
        private void BuildMessageDialog()
        {
            int number = 0;
            if (femaleToolStripMenuItem.Checked)
                int.TryParse(toolStripTextBox.Text, out number);
            
            string msg;
            foreach (DialogueParser line in Arguments)
            {
                int n = number;
                if (line.msgNum > 0) {
                    msg = MessageFile.GetMessages(MessagesData, number + line.msgNum);
                    if (msg == null && number > 0) {
                        msg = MessageFile.GetMessages(MessagesData, line.msgNum);
                        n = 0;
                    }
                    if (msg == null) 
                        msg = "Error: <Text was not found in msg file>";
                } else
                    msg = line.code;

                if (line.iq != null)
                    msg = (char)0x25CF + " " + msg;

                dgvMessages.Rows.Add(line.toNode.Trim('"', ' '), msg, line.iq, (line.msgNum > 0) ? n + line.msgNum : line.msgNum);
                if (!line.toNode.StartsWith("["))
                    dgvMessages.Rows[dgvMessages.Rows.Count - 1].Cells[1].Style.ForeColor = Color.Blue;
            }
        }

        private void dgvMessages_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string node;
            if (e.ColumnIndex == 1) {
                node = dgvMessages.CurrentRow.Cells[0].Value.ToString();
                if (!node.StartsWith("[")) {
                    OptionsTextLabel.Text = dgvMessages.CurrentRow.Cells[1].Value.ToString();
                    user = false;
                    NodesComboBox.Text = node;
                    AddToNavigation(node);
                    if (JumpToolStripMenuItem.Checked) 
                        JumpProcedure(node);
                }
            } else if (e.ColumnIndex == 0) {
                node = dgvMessages.CurrentRow.Cells[0].Value.ToString();

                if (node.StartsWith("["))
                    node = NodesComboBox.Text;

                JumpProcedure(node);
            }
        }
        
        private void JumpProcedure(string nodeName)
        {                
            int index = GetProcedureIndex(nodeName);
            if (index == -1) return;

            TextEditor te = this.Owner as TextEditor;  
            te.SelectLine(scrProc[index].fstart, scrProc[index].d.start, true);
        }

        private void dgvMessages_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
                MessageEditor.MessageEditorInit(msgPath, (int)dgvMessages.CurrentRow.Cells[3].Value);
        }
        
        private void AddToNavigation(string node)
        {
            int count = nodesNavigation.Count;
            if (++currentNavigation < count) {   
                nodesNavigation.RemoveRange(currentNavigation, count - currentNavigation);
            }
            nodesNavigation.Add(GetProcedureIndex(node));
        }

        private int GetProcedureIndex(string name)
        {
            for (int i = 0; i < scrProc.Length;  i++)
            {
                if (scrProc[i].name == name)
                    return i;
            }
            return -1;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (currentNavigation == 0) return;
            int index = nodesNavigation[--currentNavigation];
            allow = false;
            NodesComboBox.Text = scrProc[index].name;
            GotoNode(scrProc[index]);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (currentNavigation >= nodesNavigation.Count - 1) return; 
            int index = nodesNavigation[++currentNavigation];
            allow = false;
            NodesComboBox.Text = scrProc[index].name;
            GotoNode(scrProc[index]);
        }

        private void NodesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!scrProc.Equals(sourceTab.parseInfo.procs))
                scrProc = sourceTab.parseInfo.procs;        //Update procedures info

            if (allow) {
                int index = GetProcedureIndex(NodesComboBox.Text);
                if (user) {
                    nodesNavigation.Clear();
                    currentNavigation = 0;
                    nodesNavigation.Add(index);
                }
                GotoNode(scrProc[index]);
            } else 
                allow = true;
            user = false;
        }

        private void NodesComboBox_DropDownClosed(object sender, EventArgs e)
        {
            user = true;
        }

        private void femaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripTextBox.Enabled = femaleToolStripMenuItem.Checked;
            //update messages
            GotoNode(scrProc[GetProcedureIndex(NodesComboBox.Text)]);
        }
    }
}
