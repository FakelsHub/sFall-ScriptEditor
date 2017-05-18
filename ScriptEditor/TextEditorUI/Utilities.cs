using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ICSharpCode.TextEditor;

namespace ScriptEditor.TextEditorUI
{    
    /// <summary>
    /// Class for text functions.
    /// </summary>
    class Utilities
    {
        // for selected code
        public static void FormattingCode(TextEditorControl TE) 
        {
            if (!TE.ActiveTextAreaControl.SelectionManager.HasSomethingSelected) return;
            string textCode = TE.ActiveTextAreaControl.SelectionManager.SelectedText;
            int offset = TE.ActiveTextAreaControl.SelectionManager.SelectionCollection[0].Offset;
            TE.Document.Replace(offset, textCode.Length, FormattingCode(textCode));
        }
                
        public static string FormattingCode(string textCode) 
        {
            string[] pattern = { ":=", "!=", "==", ">=", "<=", "+=", "-=", "*=", "/=", "%=", ",", ">", "<", "+", "-", "*", "/", "%" };
            char[] excludeR = { ' ', '=', '+', '-', '*', '/' };
            char[] excludeL = { ' ', '=', '>', '<', '+', '-', '!', ':', '*', '/', '%' };
            char[] excludeD = { ' ', ',' };
            const string space = " ";

            string[] linecode = textCode.Split('\n');
            for (int i = 0; i < linecode.Length; i++) {
                string tmp = linecode[i].TrimStart();
                if (tmp.Length < 3 || tmp.StartsWith("//") || tmp.StartsWith("/*")) continue;
                int openQuotes = linecode[i].IndexOf('"');
                int closeQuotes = (openQuotes != -1) ? linecode[i].IndexOf('"', openQuotes + 1) : -1; 
                foreach (string p in pattern) {
                    int n = 0;
                    do {
                        n = linecode[i].IndexOf(p, n);
                        // skip string "..."
                        if (openQuotes > 0 && (n > openQuotes && n < closeQuotes)) {
                            n = closeQuotes + 1;
                            if (n < linecode[i].Length) {
                                openQuotes = linecode[i].IndexOf('"', n);
                                closeQuotes = (openQuotes != -1) ? linecode[i].IndexOf('"', openQuotes + 1) : -1;
                            } else openQuotes = -1;
                            continue;
                        }
                        if (n > 0) {
                            // insert right space
                            if (linecode[i].Substring(n + p.Length, 1) != space) {
                                if (p.Length == 2)
                                    linecode[i] = linecode[i].Insert(n + 2, space);
                                else {
                                    if (linecode[i].Substring(n + 1, 1).IndexOfAny(excludeR) == -1) {
                                        if ((p == "-" && Char.IsDigit(char.Parse(linecode[i].Substring(n + 1, 1)))
                                        && linecode[i].Substring(n - 1, 1).IndexOfAny(excludeD) != -1) == false       // check NegDigit
                                        && ((p == "+" || p == "-") && linecode[i].Substring(n - 1, 1) == p) == false) // check '++/--'
                                            linecode[i] = linecode[i].Insert(n + 1, space);
                                    }
                                }
                            }
                            // insert left space
                            if (p != "," && linecode[i].Substring(n - 1, 1) != space) {
                                if (p.Length == 2)
                                    linecode[i] = linecode[i].Insert(n, space);
                                else {
                                    if (linecode[i].Substring(n - 1, 1).IndexOfAny(excludeL) == -1) {
                                        if (((p == "+" || p == "-") && (linecode[i].Substring(n + 1, 1)) == p) == false) // check '++/--'
                                            linecode[i] = linecode[i].Insert(n, space);
                                    }
                                }
                            }
                        } else break;
                        n += p.Length;
                    } while (n < linecode[i].Length);
                }
            }
            return string.Join("\n", linecode);
        }
    }
}
