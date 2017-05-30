using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

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
            string textCode;
            int offset; 
            if (TE.ActiveTextAreaControl.SelectionManager.HasSomethingSelected) {
                textCode = TE.ActiveTextAreaControl.SelectionManager.SelectedText;
                offset = TE.ActiveTextAreaControl.SelectionManager.SelectionCollection[0].Offset;
            } else {
                textCode = TextUtilities.GetLineAsString(TE.Document, TE.ActiveTextAreaControl.Caret.Line);
                offset = TE.ActiveTextAreaControl.Caret.Offset - TE.ActiveTextAreaControl.Caret.Column;
            }
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

        public static void HighlightingSelectedText(TextEditorControl TE)
        {
            List<TextMarker> marker = TE.Document.MarkerStrategy.GetMarkers(0, TE.Document.TextLength);
            foreach (TextMarker m in marker) {
                if (m.TextMarkerType == TextMarkerType.SolidBlock)
                    TE.Document.MarkerStrategy.RemoveMarker(m); 
            }
            if (!TE.ActiveTextAreaControl.SelectionManager.HasSomethingSelected) return;
            string sWord = TE.ActiveTextAreaControl.SelectionManager.SelectedText.Trim();
            int wordLen = sWord.Length;
            if (wordLen == 0 || (wordLen < 3 && !Char.IsLetterOrDigit(Convert.ToChar((sWord.Substring(0,1)))))) return;
            int seek = 0;
            while (seek < TE.Document.TextLength) {
                seek = TE.Text.IndexOf(sWord, seek);
                if (seek == -1) break;
                char chS = (seek > 0) ? TE.Document.GetCharAt(seek - 1) : ' ';
                char chE = ((seek + wordLen) < TE.Document.TextLength) ? TE.Document.GetCharAt(seek + wordLen): ' ';
                if (!(Char.IsLetter(chS) || chS == '_') && !(Char.IsLetter(chE) || chE == '_'))
                    TE.Document.MarkerStrategy.AddMarker(new TextMarker(seek, sWord.Length, TextMarkerType.SolidBlock, Color.GreenYellow, Color.Black));
                seek += wordLen;
            }
        }

    # region Search Function
        public static bool Search(string text, string str, Regex regex, int start, bool restart, out int mstart, out int mlen)
        {
            if (start >= text.Length) start = 0;
            mstart = 0;
            mlen = str.Length;
            if (regex != null) {
                Match m = regex.Match(text, start);
                if (m.Success) {
                    mstart = m.Index;
                    mlen = m.Length;
                    return true;
                }
                if (!restart) return false;
                m = regex.Match(text);
                if (m.Success) {
                    mstart = m.Index;
                    mlen = m.Length;
                    return true;
                }
            } else {
                int i = text.IndexOf(str, start, StringComparison.OrdinalIgnoreCase);
                if (i != -1) {
                    mstart = i;
                    return true;
                }
                if (!restart) return false;
                i = text.IndexOf(str, StringComparison.OrdinalIgnoreCase);
                if (i != -1) {
                    mstart = i;
                    return true;
                }
            }
            return false;
        }

        public static bool Search(string text, string str, Regex regex)
        {
            if (regex != null) {
                if (regex.IsMatch(text))
                    return true;
            } else {
                if (text.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1)
                    return true;
            }
            return false;
        }

        public static bool SearchAndScroll(TabInfo tab, Regex regex, string searchText, ref ScriptEditor.TextEditor.PositionType type)
        {
            int start, len;
            if (Search(tab.textEditor.Text, searchText, regex, tab.textEditor.ActiveTextAreaControl.Caret.Offset + 1, true, out start, out len)) {
                FindSelected(tab, start, len, ref type);
                return true;
            }
            return false;
        }

        public static void FindSelected(TabInfo tab, int start, int len, ref ScriptEditor.TextEditor.PositionType type, string replace = null)
        {
            type = ScriptEditor.TextEditor.PositionType.NoSave;
            TextLocation locstart = tab.textEditor.Document.OffsetToPosition(start);
            TextLocation locend = tab.textEditor.Document.OffsetToPosition(start + len);
            tab.textEditor.ActiveTextAreaControl.SelectionManager.SetSelection(locstart, locend);
            if (replace != null) {
                tab.textEditor.ActiveTextAreaControl.Document.Replace(start, len, replace);
                locend = tab.textEditor.Document.OffsetToPosition(start + replace.Length);
                tab.textEditor.ActiveTextAreaControl.SelectionManager.SetSelection(locstart, locend);
            }
            tab.textEditor.ActiveTextAreaControl.Caret.Position = locstart;
            tab.textEditor.ActiveTextAreaControl.CenterViewOn(locstart.Line, 0);
        }

        public static void SearchForAll(TabInfo tab, string searchText, Regex regex, DataGridView dgv, List<int> offsets, List<int> lengths)
        {
            int start, len, line, lastline = -1;
            int offset = 0;
            while (Search(tab.textEditor.Text, searchText, regex, offset, false, out start, out len))
            {
                offset = start + 1;
                line = tab.textEditor.Document.OffsetToPosition(start).Line;
                if (offsets != null) {
                    offsets.Add(start);
                    lengths.Add(len);
                }
                if (line != lastline) {
                    lastline = line;
                    string message = TextUtilities.GetLineAsString(tab.textEditor.Document, line).Trim();
                    Error error = new Error(message, tab.filepath, line + 1, tab.textEditor.Document.OffsetToPosition(start).Column + 1, len);
                    dgv.Rows.Add(tab.filename, error.line.ToString(), error);
                }
            }
        }

        public static void SearchForAll(string[] text, string file, string searchText, Regex regex, DataGridView dgv)
        {
            bool matched;
            for (int i = 0; i < text.Length; i++)
            {
                if (regex != null)
                    matched = regex.IsMatch(text[i]);
                else
                    matched = text[i].IndexOf(searchText, StringComparison.OrdinalIgnoreCase) != -1;
                if (matched) {
                    Error error = new Error(text[i].Trim(), file, i + 1);
                    dgv.Rows.Add(Path.GetFileName(file), (i + 1).ToString(), error);
                }
            }
        }

        public static int SearchPanel(string text, string find, int start, bool icase, bool back = false)
        {
            int z; // = -1;
            if (!icase) {
                if (back) z = text.LastIndexOf(find, start, StringComparison.OrdinalIgnoreCase);
                else z = text.IndexOf(find, start, StringComparison.OrdinalIgnoreCase);
            } else {
                if (back) z = text.LastIndexOf(find, start);
                else z= text.IndexOf(find, start);
            }
            return z;
        }
    #endregion
    }
}
