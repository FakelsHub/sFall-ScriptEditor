using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using ScriptEditor.CodeTranslation;

using ICSharpCode.TextEditor.Document;

namespace ScriptEditor.SyntaxRules
{
    internal class HighlightWord
    {
        List<string> procedures = new List<string>();

        Color color = Color.Empty;
        bool noColor;

        public void ProceduresHighlight(IDocument document, Procedure[] procs)
        {
            if (color.IsEmpty) {
                if (noColor) return;

                string clr;
                if (document.HighlightingStrategy.Properties.TryGetValue("ProceduresColor", out clr))
                    color = Color.FromName(clr);
                else {
                    noColor = true;
                    return;
                }
            }
            
            bool needUpdate = false;
            foreach (var p in procs)
            {
                if (!procedures.Contains(p.name) && !ProgramInfo.opcodes_list.ContainsKey(p.name)) {
                    procedures.Add(p.name);
                    document.HighlightingStrategy.AddHighlightWord(p.name, color, Color.Empty, false, false);
                    needUpdate = true;
                }
            }
            if (needUpdate)
                document.HighlightingStrategy.MarkTokens(document);
        }
    }
}
