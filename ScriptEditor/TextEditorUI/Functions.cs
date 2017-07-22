﻿using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ScriptEditor.TextEditorUI
{
    public static class Functions
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
                        ND.NodeFont = new Font("Arial", 8, FontStyle.Bold);
                        ND.ForeColor = Color.FromArgb(100, 0, 100); // DarkPurple
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
                    Tree.Nodes.Add(new String('-', 50));
                    Node++;
                }   
            }
        }
    }
}