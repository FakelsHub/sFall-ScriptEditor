using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;

namespace ScriptEditor.TextEditorUtilities
{
    internal sealed class Refactor
    {
        internal static void Rename(IParserInfo item, IDocument document, ProgramInfo pi)
        {
            string newName;
            switch ((NameType)item.Type()) {
                case NameType.LVar: // local procedure variable 
                    Variable lvar = (Variable)item;
                    newName = lvar.name;
                    if (!ProcForm.CreateRenameForm(ref newName, "Local Variable") || newName == lvar.name)
                        return;
                    if (pi.CheckExistsName(newName, NameType.LVar, lvar.fdeclared, lvar.d.declared)) {
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
                    if (pi.CheckExistsName(newName, NameType.GVar)) {
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
                    Procedure proc = (Procedure)item;
                    RenameProcedure(proc.name, document, pi);
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
                    if (pi.CheckExistsName(newName, NameType.Macro)) {
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

        // Search and replace procedure name in script text
        internal static string RenameProcedure(string oldName, IDocument document, ProgramInfo pi)
        {
            string newName = oldName;
            // form ini
            if (!ProcForm.CreateRenameForm(ref newName, "Procedure") || newName == oldName)
                return null;
            
            if (pi.CheckExistsName(newName, NameType.Proc)) {
                MessageBox.Show("The procedure/variable or declared macro with this name already exists.", "Unable to rename");
                return null;  
            }
            int differ = newName.Length - oldName.Length; 
            
            string search = "[=,@ ]" + oldName + "[ ,;()\\s]";
            RegexOptions option = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            Regex s_regex = new Regex(search, option);
            MatchCollection matches = s_regex.Matches(document.TextContent);
            int replace_count = 0;
            document.UndoStack.StartUndoGroup();
            foreach (Match m in matches)
            {
                int offset = (differ * replace_count) + (m.Index + 1);
                document.Replace(offset, (m.Length - 2), newName);
                replace_count++;
            }
            document.UndoStack.EndUndoGroup();

            return newName;
        }
    }
}
