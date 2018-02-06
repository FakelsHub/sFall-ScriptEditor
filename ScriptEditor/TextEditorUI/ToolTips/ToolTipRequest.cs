using System.Drawing;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ScriptEditor.CodeTranslation;

namespace ScriptEditor.TextEditorUI.ToolTips
{
    // Tooltip for opcodes/macros and message
    static class ToolTipRequest
    {
        public static void Show(TabInfo ti, IDocument document, ToolTipRequestEventArgs args)
        {
            HighlightColor hc = document.GetLineSegment(args.LogicalPosition.Line).GetColorForPosition(args.LogicalPosition.Column);
            
            if (hc == null || hc.Color == Color.Green || hc.Color == Color.Brown 
                || hc.Color == Color.DarkGreen || hc.BackgroundColor == Color.FromArgb(0xFF, 0xFF, 0xD0))
                return;
            
            string word = TextUtilities.GetWordAt(document, document.PositionToOffset(args.LogicalPosition));
            if (word.Length == 0 ) 
                return;
            
            if (ti.messages.Count > 0) {
                int msg;
                if (int.TryParse(word, out msg) && ti.messages.ContainsKey(msg)) {
                    args.ShowToolTip("\"" + ti.messages[msg] + "\"");
                    return;
                }
            }

            string lookup = ProgramInfo.LookupOpcodesToken(word); // show opcodes help
            if (lookup == null && ti.parseInfo != null )
                lookup = ti.parseInfo.LookupToken(word, ti.filepath, args.LogicalPosition.Line + 1);
            if (lookup != null) {
                args.ShowToolTip(lookup);
            }
        }
    }
}
