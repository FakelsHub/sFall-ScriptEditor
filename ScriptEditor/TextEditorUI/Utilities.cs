using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ScriptEditor.CodeTranslation;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ScriptEditor.TextEditorUI
{    
    /// <summary>
    /// Class for text editor functions.
    /// </summary>
    internal static class Utilities
    {
    #region Formating text functions
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
            TE.ActiveTextAreaControl.TextArea.SelectionManager.ClearSelection();
        }

        public static void DecIndent(TextEditorControl TE)
        {
            int indent = -1;
            if (TE.ActiveTextAreaControl.SelectionManager.HasSomethingSelected) {
                ISelection position = TE.ActiveTextAreaControl.SelectionManager.SelectionCollection[0];
                for (int i = position.StartPosition.Line; i <= position.EndPosition.Line; i++)
                {
                    CheckSpacesIndent(i, ref indent, TE.Document);
                }
                if (indent <= 0) return;
                TE.Document.UndoStack.StartUndoGroup();
                for (int i = position.StartPosition.Line; i <= position.EndPosition.Line; i++)
                {
                    SubDecIndent(i, indent, TE.Document);
                }
                TE.Document.UndoStack.EndUndoGroup();
                TextLocation srtSel = TE.ActiveTextAreaControl.SelectionManager.SelectionCollection[0].StartPosition;
                TextLocation endSel = TE.ActiveTextAreaControl.SelectionManager.SelectionCollection[0].EndPosition;
                srtSel.Column -= indent;
                endSel.Column -= indent;
                TE.ActiveTextAreaControl.SelectionManager.SetSelection(srtSel, endSel);
            } else {
                int line = TE.ActiveTextAreaControl.Caret.Line;
                CheckSpacesIndent(line, ref indent, TE.Document);
                if (indent <= 0 || SubDecIndent(line, indent, TE.Document)) return;
            }
            TE.ActiveTextAreaControl.Caret.Column -= indent;
            TE.Refresh();
        }

        private static void CheckSpacesIndent(int line, ref int indent, IDocument document)
        {
            string LineText = TextUtilities.GetLineAsString(document, line);
            int len = LineText.Length;
            int trimlen = LineText.TrimStart().Length;
            if (len == 0 || trimlen == 0) return;

            int spacesLen = (len - trimlen);
            if (indent == -1) {
                // Adjust indent
                int adjust = spacesLen % Settings.tabSize;
                indent = (adjust > 0) ? adjust : Settings.tabSize; 
            }
            if (spacesLen < indent) indent = spacesLen;
        }

        private static bool SubDecIndent(int line, int indent, IDocument document)
        {
            if (TextUtilities.GetLineAsString(document, line).TrimStart().Length == 0) return true;
            document.Remove(document.LineSegmentCollection[line].Offset, indent);
            return false;
        }

        public static void CommentText(TextEditorControl TE)
        {
            if (TE.ActiveTextAreaControl.SelectionManager.HasSomethingSelected) {
                TE.Document.UndoStack.StartUndoGroup();
                ISelection position = TE.ActiveTextAreaControl.SelectionManager.SelectionCollection[0];
                for (int i = position.StartPosition.Line; i <= position.EndPosition.Line; i++) 
                {
                    string LineText = TextUtilities.GetLineAsString(TE.Document, i);
                    if (LineText.TrimStart().StartsWith(Parser.COMMENT)) continue;
                    int offset = TE.Document.LineSegmentCollection[i].Offset;
                    TE.Document.Insert(offset, Parser.COMMENT); 
                }
                TE.Document.UndoStack.EndUndoGroup();
                TE.ActiveTextAreaControl.SelectionManager.ClearSelection();
            } else {
                string LineText = TextUtilities.GetLineAsString(TE.Document, TE.ActiveTextAreaControl.Caret.Line);
                if (LineText.TrimStart().StartsWith(Parser.COMMENT)) return;
                int offset_str = TE.Document.LineSegmentCollection[TE.ActiveTextAreaControl.Caret.Line].Offset;
                TE.Document.Insert(offset_str, Parser.COMMENT);
            }
            TE.ActiveTextAreaControl.Caret.Column += 2;
        }

        public static void UnCommentText(TextEditorControl TE)
        {
            if (TE.ActiveTextAreaControl.SelectionManager.HasSomethingSelected) {
                TE.Document.UndoStack.StartUndoGroup();
                ISelection position = TE.ActiveTextAreaControl.SelectionManager.SelectionCollection[0];
                for (int i = position.StartPosition.Line; i <= position.EndPosition.Line; i++)
                {
                    string LineText = TextUtilities.GetLineAsString(TE.Document, i);
                    if (!LineText.TrimStart().StartsWith(Parser.COMMENT)) continue;
                    int n = LineText.IndexOf(Parser.COMMENT);
                    int offset_str = TE.Document.LineSegmentCollection[i].Offset;
                    TE.Document.Remove(offset_str + n, 2);
                }
                TE.Document.UndoStack.EndUndoGroup();
                TE.ActiveTextAreaControl.SelectionManager.ClearSelection();
            } else {
                string LineText = TextUtilities.GetLineAsString(TE.Document, TE.ActiveTextAreaControl.Caret.Line);
                if (!LineText.TrimStart().StartsWith(Parser.COMMENT)) return;
                int n = LineText.IndexOf(Parser.COMMENT);
                int offset_str = TE.Document.LineSegmentCollection[TE.ActiveTextAreaControl.Caret.Line].Offset;
                TE.Document.Remove(offset_str + n, 2);
            }
            TE.ActiveTextAreaControl.Caret.Column -= 2;
        }

        public static void AlignToLeft(TextEditorControl TE)
        {
            if (TE.ActiveTextAreaControl.SelectionManager.HasSomethingSelected) {
                ISelection position = TE.ActiveTextAreaControl.SelectionManager.SelectionCollection[0];
                string LineText = TextUtilities.GetLineAsString(TE.Document, position.StartPosition.Line);
                int Align = LineText.Length - LineText.TrimStart().Length; // узнаем длину отступа
                TE.Document.UndoStack.StartUndoGroup();
                for (int i = position.StartPosition.Line + 1; i <= position.EndPosition.Line; i++)
                {
                    LineText = TextUtilities.GetLineAsString(TE.Document, i);
                    int len = LineText.Length - LineText.TrimStart().Length;
                    if (len == 0 || len <= Align) continue;
                    int offset = TE.Document.LineSegmentCollection[i].Offset;
                    TE.Document.Remove(offset, len-Align);
                }
                TE.Document.UndoStack.EndUndoGroup();
            }
        }
    #endregion

    # region Search Function
        public static bool Search(string text, string str, Regex regex, int start, bool restart, bool mcase, out int mstart, out int mlen)
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
                int i = text.IndexOf(str, start, (mcase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
                if (i != -1) {
                    mstart = i;
                    return true;
                }
                if (!restart) return false;
                i = text.IndexOf(str, (mcase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
                if (i != -1) {
                    mstart = i;
                    return true;
                }
            }
            return false;
        }

        public static bool Search(string text, string str, Regex regex, bool mcase)
        {
            if (regex != null) {
                if (regex.IsMatch(text))
                    return true;
            } else {
                if (text.IndexOf(str, (mcase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) != -1)
                    return true;
            }
            return false;
        }

        public static bool SearchAndScroll(TabInfo tab, Regex regex, string searchText, bool mcase, ref ScriptEditor.TextEditor.PositionType type)
        {
            int start, len;
            if (Search(tab.textEditor.Text, searchText, regex, tab.textEditor.ActiveTextAreaControl.Caret.Offset + 1, true, mcase, out start, out len)) {
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

        public static void SearchForAll(TabInfo tab, string searchText, Regex regex, bool mcase, DataGridView dgv, List<int> offsets, List<int> lengths)
        {
            int start, len, line, lastline = -1;
            int offset = 0;
            while (Search(tab.textEditor.Text, searchText, regex, offset, false, mcase, out start, out len))
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

        public static void SearchForAll(string[] text, string file, string searchText, Regex regex, bool mcase, DataGridView dgv)
        {
            bool matched;
            for (int i = 0; i < text.Length; i++)
            {
                if (regex != null)
                    matched = regex.IsMatch(text[i]);
                else
                    matched = text[i].IndexOf(searchText, (mcase) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) != -1;
                if (matched) {
                    Error error = new Error(text[i].Trim(), file, i + 1);
                    dgv.Rows.Add(Path.GetFileName(file), (i + 1).ToString(), error);
                }
            }
        }

        public static int SearchPanel(string text, string find, int start, bool icase, bool wholeword, bool back = false)
        {
            int z;
            if (wholeword) {
                RegexOptions option = RegexOptions.Multiline;
                if (!icase) option |= RegexOptions.IgnoreCase;
                if (back) option |= RegexOptions.RightToLeft;
                z = SearchWholeWord(text, find, start, option);
            } else {            
                if (!icase) {
                    if (back) z = text.LastIndexOf(find, start, StringComparison.OrdinalIgnoreCase);
                    else z = text.IndexOf(find, start, StringComparison.OrdinalIgnoreCase);
                } else {
                    if (back) z = text.LastIndexOf(find, start);
                    else z= text.IndexOf(find, start);
                }
            }
            return z;
        }

        private static int SearchWholeWord(string text, string find, int start, RegexOptions option)
        {
            int z, x;
            string search = @"\b" + find + @"\b";
            Regex s_regex = new Regex(search, option);
            if (!Search(text, find, s_regex, start, false, false, out z, out x)) return -1;
            return z;
        }
    #endregion

        internal static void RefactorRename(IParserInfo item, TextEditorControl TE)
        {
            string newName;
            switch ((NameType)item.Type()) {
                case NameType.LVar: // local procedure variable 
                    Variable lvar = (Variable)item;
                    newName = lvar.name;
                    if (!ProcForm.CreateRenameForm(ref newName, "Local Variable") || newName == lvar.name) return;
                    TE.Document.UndoStack.StartUndoGroup();
                    subRenameVariable(lvar, newName, RegexOptions.IgnoreCase, TE);
                    break;
                case NameType.GVar: // script variable
                    Variable gvar = (Variable)item;
                    newName = gvar.name;
                    if (!ProcForm.CreateRenameForm(ref newName, "Script Variable") || newName == gvar.name) return;
                    TE.Document.UndoStack.StartUndoGroup();
                    // rename only references
                    subRenameVariable(gvar, newName, RegexOptions.IgnoreCase, TE);
                    // for all script text
                    //subRenameMacros(gvar.name, newName, RegexOptions.IgnoreCase, TE); 
                    break;
                case NameType.Proc:
                    Procedure proc = (Procedure)item;
                    RenameProcedure(proc.name, TE);
                    return;
                case NameType.Macro:
                    Macro macros = (Macro)item;
                    int offset = macros.name.IndexOf('(');
                    if (offset != -1)
                        newName = macros.name.Remove(offset);
                    else 
                        newName = macros.name;
                    string name = newName;
                    if (!ProcForm.CreateRenameForm(ref newName, "Local Macros") || newName == macros.name) return;
                    TE.Document.UndoStack.StartUndoGroup();
                    subRenameMacros(name, newName, RegexOptions.None, TE);
                    // insert/delete spaces
                    int diff = name.Length - newName.Length;
                    if (diff != 0) {
                        offset = TE.Document.PositionToOffset(new TextLocation(0, macros.declared - 1));
                        offset += (macros.name.Length + 8) - diff;
                        if (diff > 0)
                            TE.Document.Insert(offset, new string(' ', diff));
                        else {
                            diff = diff * -1;
                            for (int i = 0; i < diff; i++)  
                            {
                                if (!Char.IsWhiteSpace(TE.Document.GetCharAt(offset + 1))) break; 
                                TE.Document.Remove(offset, 1);
                            }
                        }
                    }
                    break;   
            }
            TE.Document.UndoStack.EndUndoGroup();
        }

        private static void subRenameMacros(string find, string newName, RegexOptions option ,TextEditorControl TE)
        {
            int offset = 0;
            while (offset < TE.Text.Length)
            {
                offset = SearchWholeWord(TE.Text, find, offset, option);
                if (offset == -1) break; 
                TE.Document.Replace(offset, find.Length, newName);
                offset += newName.Length; 
            }
        }

        private static void subRenameVariable(Variable var, string newName, RegexOptions option, TextEditorControl TE)
        {
            int z, offset;
            int nameLen = var.name.Length;
            foreach (var refs in var.references)
            {
                LineSegment ls = TE.Document.GetLineSegment(refs.line - 1);
                offset = 0;
                while (offset < ls.Length)
                {
                    z = SearchWholeWord(TextUtilities.GetLineAsString(TE.Document, refs.line - 1), var.name, offset, option);
                    if (z == -1) break; 
                    TE.Document.Replace(ls.Offset + z, nameLen, newName);
                    offset = z + newName.Length;
                }
            }
            int decline = var.d.declared - 1;
            for (int i = decline; i > 0; i--)
            {
                z = SearchWholeWord(TextUtilities.GetLineAsString(TE.Document, i), var.name, 0, option);
                if (z == -1) continue; 
                LineSegment ls = TE.Document.GetLineSegment(i);
                TE.Document.Replace(ls.Offset + z, nameLen, newName);
                break;
            }
        }

        // Search and replace procedure name in script text
        internal static void RenameProcedure(string oldName, TextEditorControl TE)
        {
            string newName = oldName;
            // form ini
            if (!ProcForm.CreateRenameForm(ref newName, "Procedure") 
                || newName == oldName || Parser.CheckExistsProcedureName(newName)) {
                return;
            }
            int differ = newName.Length - oldName.Length; 
            string search = "[=, ]" + oldName + "[ ,;()\\s]";
            RegexOptions option = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            Regex s_regex = new Regex(search, option);
            MatchCollection matches = s_regex.Matches(TE.Text);
            int replace_count = 0;
            TE.Document.UndoStack.StartUndoGroup();
            foreach (Match m in matches) {
                int offset = (differ * replace_count) + (m.Index + 1);
                TE.Document.Replace(offset, (m.Length - 2), newName);
                replace_count++;
            }
            TE.Document.UndoStack.EndUndoGroup();

        }

        // auto selected text color region  
        internal static void SelectedTextColorRegion(TextEditorControl TE)
        {
            TextLocation tl = TE.ActiveTextAreaControl.Caret.Position;
            HighlightColor hc = TE.Document.GetLineSegment(tl.Line).GetColorForPosition(tl.Column);
            if (hc == null) return; 
            if (hc.BackgroundColor == Color.LightGray) {
                int sStart= tl.Column, sEnd = tl.Column + 1;
                for (int i = sEnd; i < (sEnd + 32); i++)
                {
                    hc = TE.Document.GetLineSegment(tl.Line).GetColorForPosition(i);
                    if (hc == null || hc.BackgroundColor != Color.LightGray) {
                        sEnd = i;
                        break;
                    }
                }
                for (int i = sStart; i > 0; i--)
                {
                    hc = TE.Document.GetLineSegment(tl.Line).GetColorForPosition(i);
                    if (hc == null || hc.BackgroundColor != Color.LightGray) {
                        sStart = i + 1;
                        break;
                    }
                }
                TextLocation sSel = new TextLocation(sStart, tl.Line);
                TextLocation eSel = new TextLocation(sEnd, tl.Line);
                TE.ActiveTextAreaControl.SelectionManager.SetSelection(sSel, eSel);
            }
        }
    }
}
