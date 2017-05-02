using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor.TextEditorUI
{
    partial class HandlerProcedure
    {
        const string phFile = "\\ProcHandlers";
        public const string def = ".ini";
        public const string lng = "_rus.ini";
        public const byte English = 0;
        
        private static TextEditor TE;

        public static void CreateProcHandlers(ContextMenuStrip Menu, TextEditor handle)
        {
            TE = handle;
            bool sub = false;
            string parentName = "";
            string file = phFile + ((Settings.hintsLang == English) ? def : lng);
            string[] lines = File.ReadAllLines(Settings.ResourcesFolder + file, Encoding.Default);
            ToolStripMenuItem SubMenuItem = new ToolStripMenuItem();
            Menu.Items.Add(new ToolStripSeparator());
            for (int i = 0; i < lines.Length; i++) {
                lines[i] = lines[i].Trim();
                if (lines[i].StartsWith("//")) continue;
                if (lines[i].StartsWith("<m>")) {
                    if (sub) {
                        Menu.Items.Add(SubMenuItem);
                        Menu.Items[Menu.Items.Count - 1].Text = parentName;
                        SubMenuItem = new ToolStripMenuItem();
                    } else sub = true;
                    parentName = lines[i].Substring(3, lines[i].Length - 3);
                    continue;
                }
                int n = lines[i].IndexOf("<d>");
                if (n > 0 && !sub) {
                    int cnt = Menu.Items.Count;
                    Menu.Items.Add(lines[i].Substring(0, n));
                    Menu.Items[cnt].ToolTipText = lines[i].Substring(n + 3, lines[i].Length - (n + 3));
                    Menu.Items[cnt].Click += new EventHandler(HandlerProcedure_Click);
                    continue;
                } else if (n > 0) {
                    int cnt = SubMenuItem.DropDownItems.Count;
                    SubMenuItem.DropDownItems.Add(lines[i].Substring(0, n));
                    SubMenuItem.DropDownItems[cnt].ToolTipText = lines[i].Substring(n + 3, lines[i].Length - (n + 3));
                    SubMenuItem.DropDownItems[cnt].Click += new EventHandler(HandlerProcedure_Click);
                    continue;
                }
                if (lines[i] == "<-m>") {
                    if (sub) {
                        Menu.Items.Add(SubMenuItem);
                        Menu.Items[Menu.Items.Count - 1].Text = parentName;
                        SubMenuItem = new ToolStripMenuItem();
                        sub = false;
                    }
                    continue;
                }
                if (lines[i] == "<->") Menu.Items.Add(new ToolStripSeparator());
            }
        }

        static void HandlerProcedure_Click(object sender, EventArgs e)
        {
            TE.CreateProcBlock(((ToolStripMenuItem)sender).Text);
        }
     }

    partial class Functions 
    {
        const string fnFile = "\\Functions";

        public static void CreateTree(TreeView Tree)
        {
            TreeNode ND;
            string codeName;
            int Node = -1, mNode = -1, sNode = -1, ssNode = -1, aNode = 0;
            string file = fnFile + ((Settings.hintsLang == HandlerProcedure.English) ? HandlerProcedure.def : HandlerProcedure.lng);
            string[] lines = File.ReadAllLines(Settings.ResourcesFolder + file, Encoding.Default);
            for (int i = 0; i < lines.Length; i++) 
            {
                lines[i] = lines[i].Trim();
                if (lines[i].StartsWith("//")) continue;
                if (lines[i].StartsWith("<m>")) {
                    codeName = lines[i].Substring(3, lines[i].Length - 3);
                    ND = new TreeNode(codeName);
                    Tree.Nodes.Add(ND);
                    Node++;
                    sNode = -1;
                    mNode = -1;
                    aNode = 0;
                    continue;
                }
                else if (lines[i].StartsWith("<m+>")) {
                    codeName = lines[i].Substring(4, lines[i].Length - 4);
                    ND = new TreeNode(codeName);
                    mNode++;
                    if (mNode <= 0) {
                        Tree.Nodes[Node].Nodes.Add(ND);
                        sNode++;
                        if (aNode > 0) {
                            sNode += aNode;
                            aNode = 0;
                        }
                    } else Tree.Nodes[Node].Nodes[sNode].Nodes.Add(ND);
                    continue;
                }
                else if (lines[i].StartsWith("<m->")) {
                    mNode--;
                    ssNode = 0;
                    continue;
                } else {
                    int n = lines[i].IndexOf("<d>");
                    if (n > 0) {
                        ND = new TreeNode(lines[i].Substring(0, n));
                        int m =  lines[i].IndexOf("<s>");
                        ND.ToolTipText = lines[i].Substring(n + 3, m - (n + 3));
                        ND.Tag = lines[i].Substring(m + 3, lines[i].Length - (m + 3));
                        ND.NodeFont = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
                        ND.ForeColor = System.Drawing.Color.FromArgb(100, 0, 100); // DarkPurple
                        switch (mNode) {
                            case -1:
                                Tree.Nodes[Node].Nodes.Add(ND);
                                aNode++;
                                break;
                            case 0:
                                Tree.Nodes[Node].Nodes[sNode].Nodes.Add(ND);
                                ssNode++;
                                break;
                            case 1:
                                Tree.Nodes[Node].Nodes[sNode].Nodes[ssNode].Nodes.Add(ND);
                                break;
                            default:
                                break;
                        } 
                    }
                }
                if (lines[i] == "<->") {
                    Tree.Nodes.Add("------------------------------------------------");
                    Node++;
                }   
            }
        }
    }
}
