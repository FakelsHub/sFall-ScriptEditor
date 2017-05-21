using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.TextEditor.Document;
using ScriptEditor.CodeTranslation;

namespace ScriptEditor.TextEditorUI
{
    /// <summary>
    /// SSL code folding strategy for ICSharpCode.TextEditor.
    /// </summary>
    public class CodeFolder : IFoldingStrategy
    {
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            ProgramInfo pi = (ProgramInfo)parseInformation;
            List<FoldMarker> list = new List<FoldMarker>(pi.procs.Length);
            int declare = -1;
            fileName = fileName.ToLowerInvariant();
            for (int i = 0; i < pi.procs.Length; i++) {
                if (pi.procs[i].filename != fileName || pi.procs[i].d.start == pi.procs[i].d.end)
                    continue;
                int dstart = pi.procs[i].d.start - 1;
                if (declare > dstart || declare == -1) declare = dstart;
                list.Add(new FoldMarker(document, dstart, 0, pi.procs[i].d.end - 1, 1000, FoldType.MemberBody, " " + pi.procs[i].name.ToUpperInvariant() + " "));
            }
            if (list.Count > 0 && System.IO.Path.GetExtension(fileName) == ".ssl") {
                int line = Parser.GetEndRegionDeclaration(document.TextContent, declare);
                if (line > 0) declare = line; 
                list.Add(new FoldMarker(document, 0, 0, declare - 2, 1000, FoldType.MemberBody, " - Declaration Region - "));
            } 
            return list;
        }
    }   
}
