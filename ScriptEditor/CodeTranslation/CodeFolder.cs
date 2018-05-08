using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.TextEditor.Document;

namespace ScriptEditor.CodeTranslation
{
    /// <summary>
    /// SSL code folding strategy for ICSharpCode.TextEditor.
    /// </summary>
    public class CodeFolder : IFoldingStrategy
    {
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            Procedure[] procs = (Procedure[])parseInformation;

            List<FoldMarker> list = new List<FoldMarker>(procs.Length);
            int minStart = -1;
            fileName = fileName.ToLowerInvariant();
            for (int i = 0; i < procs.Length; i++) {
                string fstart = Path.GetFileName(procs[i].fstart ?? string.Empty).ToLowerInvariant();
                if (fstart != fileName || procs[i].d.start >= procs[i].d.end)
                    continue;
                int dstart = procs[i].d.start - 1;
                if (minStart > dstart || minStart == -1)
                    minStart = dstart;
                int len = document.GetLineSegment(procs[i].d.end - 1).Length;
                list.Add(new FoldMarker(document, dstart, 0, procs[i].d.end - 1, len, FoldType.MemberBody, " " + procs[i].name.ToUpperInvariant() + " "));
            }

            if (list.Count > 0 && Path.GetExtension(fileName) == ".ssl") {
                ProcBlock dRegion = Parser.GetRegionDeclaration(document.TextContent, minStart);
                if (dRegion.end < 0)
                    dRegion.end = minStart - 2;
                if (dRegion.end > dRegion.begin)
                    list.Add(new FoldMarker(document, dRegion.begin, 0, dRegion.end, 1000, FoldType.Region, " - Declaration Region - "));
            }
            // Get variable and #if foldings block
            List<ProcBlock> blockList = Parser.GetFoldingBlock(document.TextContent);
            foreach (ProcBlock block in blockList) {
                string str = block.copy ? TextUtilities.GetLineAsString(document, block.begin) + " " 
                                        : " - Variables - ";
                list.Add(new FoldMarker(document, block.begin, 0, block.end, 1000, FoldType.TypeBody, str));
            }
            return list;
        }

        /// <summary>
        /// Update Foldings
        /// </summary>
        internal static void UpdateFolding(IDocument document, string filename, Procedure[] parseInformation)
        {
            document.FoldingManager.UpdateFoldings(filename, parseInformation);
            document.FoldingManager.NotifyFoldingsChanged(null);
        }

        /// <summary>
        /// Get information about the location of procedures for updating Foldings
        /// </summary>
        internal static void UpdateFolding(IDocument document, string filepath)
        {
            Procedure[] parseInformation = Parser.GetProcsData(document.TextContent, filepath);
            UpdateFolding(document, Path.GetFileName(filepath), parseInformation);
        }

        const string FLDC = " //_FLDC_";

        internal static bool SaveMarkFoldCollapsed(IDocument document)
        {
            bool result = false;
            foreach (FoldMarker fm in document.FoldingManager.FoldMarker)
            {
                if (fm.FoldType > FoldType.Region || !fm.IsFolded)
                    continue;

                LineSegment ls = document.GetLineSegment(fm.StartLine);
                string line = document.GetText(ls);
                if (!line.EndsWith(FLDC)) 
                    document.Insert(ls.Offset + ls.Length, FLDC);
                result = true;
            }
            return result;
        }

        internal static void LoadFoldCollapse(IDocument document)
        {
            foreach (FoldMarker fm in document.FoldingManager.FoldMarker)
            {
                if (fm.FoldType > FoldType.Region)
                    continue;
                
                LineSegment ls = document.GetLineSegment(fm.StartLine);
                if (document.GetText(ls).EndsWith(FLDC)) {
                    fm.IsFolded = true;
                    int len = FLDC.Length;
                    document.Remove(ls.Offset + ls.Length - len, len);
                }
            }
            if (document.UndoStack.CanUndo) document.UndoStack.ClearAll();
        }
    }
}
