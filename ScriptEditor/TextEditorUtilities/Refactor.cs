using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;
using ScriptEditor.TextEditorUI;

namespace ScriptEditor.TextEditorUtilities
{
    internal sealed class Refactor
    {
        internal static void Rename(IParserInfo item, IDocument document, TabInfo cTab, List<TabInfo> tabs)
        {
            string newName;
            switch ((NameType)item.Type()) {
                case NameType.LVar: // local procedure variable 
                    Variable lvar = (Variable)item;
                    newName = lvar.name;
                    if (!ProcForm.CreateRenameForm(ref newName, "Local Variable") || newName == lvar.name)
                        return;
                    if (cTab.parseInfo.CheckExistsName(newName, NameType.LVar, lvar.fdeclared, lvar.d.declared)) {
                        MessageBox.Show("The local variable this name already exists.", "Unable to rename");
                        return;
                    }
                    document.UndoStack.StartUndoGroup();
                    RenameVariable(lvar, newName, RegexOptions.IgnoreCase, document);
                    break;
                case NameType.GVar: // script variable
                    Variable gvar = (Variable)item;
                    newName = gvar.name;
                    if (!ProcForm.CreateRenameForm(ref newName, "Script Variable") || newName == gvar.name)
                        return;
                    if (cTab.parseInfo.CheckExistsName(newName, NameType.GVar)) {
                        MessageBox.Show("The variable/procedure or declared macro this name already exists.", "Unable to rename");
                        return;
                    }
                    document.UndoStack.StartUndoGroup();
                    // rename only via references
                    RenameVariable(gvar, newName, RegexOptions.IgnoreCase, document);
                    // for all script text
                    //RenameMacros(gvar.name, newName, RegexOptions.IgnoreCase, document); 
                    break;
                case NameType.Proc:
                    RenameProcedure((Procedure)item, document, cTab, tabs);
                    return;
                case NameType.Macro:
                    Macro macros = (Macro)item;
                    int offset = macros.name.IndexOf('(');
                    if (offset != -1)
                        newName = macros.name.Remove(offset);
                    else 
                        newName = macros.name;
                    string name = newName;
                    if (!ProcForm.CreateRenameForm(ref newName, "Local Macros") || newName == macros.name)
                        return;
                    if (cTab.parseInfo.CheckExistsName(newName, NameType.Macro)) {
                        MessageBox.Show("The variable/procedure or declared macro this name already exists.", "Unable to rename");
                        return;
                    }
                    document.UndoStack.StartUndoGroup();
                    RenameMacros(name, newName, RegexOptions.None, document);
                    
                    // insert/delete spaces
                    int diff = name.Length - newName.Length;
                    if (diff != 0) {
                        offset = document.PositionToOffset(new TextLocation(0, macros.declared - 1));
                        offset += (macros.name.Length + 8) - diff;
                        if (diff > 0)
                            document.Insert(offset, new string(' ', diff));
                        else {
                            diff = diff * -1;
                            for (int i = 0; i < diff; i++)  
                            {
                                if (!Char.IsWhiteSpace(document.GetCharAt(offset + 1)))
                                    break; 
                                document.Remove(offset, 1);
                            }
                        }
                    }
                    break;   
            }
            document.UndoStack.EndUndoGroup();
        }

        private static void RenameMacros(string find, string newName, RegexOptions option , IDocument document)
        {
            int offset = 0;
            while (offset < document.TextLength)
            {
                offset = Utilities.SearchWholeWord(document.TextContent, find, offset, option);
                if (offset == -1)
                    break; 
                document.Replace(offset, find.Length, newName);
                offset += newName.Length; 
            }
        }

        private static void RenameVariable(Variable var, string newName, RegexOptions option, IDocument document)
        {
            int z, offset;
            int nameLen = var.name.Length;
            foreach (var refs in var.references)
            {
                LineSegment ls = document.GetLineSegment(refs.line - 1);
                offset = 0;
                while (offset < ls.Length)
                {
                    z = Utilities.SearchWholeWord(TextUtilities.GetLineAsString(document, refs.line - 1), var.name, offset, option);
                    if (z == -1)
                        break; 
                    document.Replace(ls.Offset + z, nameLen, newName);
                    offset = z + newName.Length;
                }
            }

            int decline = var.adeclared;
            if (decline > 0) {
                decline--;
                z = Utilities.SearchWholeWord(TextUtilities.GetLineAsString(document, decline), var.name, 0, option);
                document.Replace(document.GetLineSegment(decline).Offset + z, nameLen, newName);
            }

            decline = var.d.declared - 1;
            for (int i = decline; i > 0; i--)
            {
                z = Utilities.SearchWholeWord(TextUtilities.GetLineAsString(document, i), var.name, 0, option);
                if (z == -1)
                    continue;
                LineSegment ls = document.GetLineSegment(i);
                document.Replace(ls.Offset + z, nameLen, newName);
                break;
            }
        }

        internal static string RenameProcedure(string oldName, IDocument document, TabInfo cTab)
        {
            Procedure proc = new Procedure();
            proc.name = oldName;
            return RenameProcedure(proc, document, cTab);
        }

        // Search and replace procedure name in script text
        internal static string RenameProcedure(Procedure proc, IDocument document, TabInfo cTab, List<TabInfo> tabs = null)
        {
            string newName = proc.name;
            // form ini
            if (!ProcForm.CreateRenameForm(ref newName, "Procedure") || newName == proc.name)
                return null;
            
            if (cTab.parseInfo.CheckExistsName(newName, NameType.Proc)) {
                MessageBox.Show("The procedure/variable or declared macro with this name already exists.", "Unable to rename");
                return null;  
            }

            bool extFile = false;
            if (tabs != null && proc.filename != cTab.filename.ToLowerInvariant()) {
                switch (MessageBox.Show("Also renaming the procedure in a file: " + proc.filename.ToUpper() + " ?", "Procedure rename", MessageBoxButtons.YesNoCancel)) {
                    case DialogResult.Cancel :
                        return null;
                    case DialogResult.Yes :
                        extFile = true;
                        break;
                }
            }

            int differ = newName.Length - proc.name.Length;
            
            string search = "[=,@ ]" + proc.name + "[ ,;()\\s]";
            RegexOptions option = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            Regex s_regex = new Regex(search, option);
            Utilities.ReplaceDocumentText(s_regex, document, newName, differ);
            
            // replace to other file/tabs
            if (extFile) {
                string text = System.IO.File.ReadAllText(proc.fstart);
                Utilities.ReplaceCommonText(s_regex, ref text, newName, differ);
                System.IO.File.WriteAllText(proc.fstart, text);
                TabInfo tab = TextEditor.CheckTabs(tabs, proc.fstart);
                if (tab != null) {
                    Utilities.ReplaceDocumentText(s_regex, tab.textEditor.Document, newName, differ);
                    tab.FileTime = System.IO.File.GetLastWriteTime(proc.fstart);
                }
            }
            TextEditor.currentHighlightProc = null;
            return newName;
        }
    }
}
