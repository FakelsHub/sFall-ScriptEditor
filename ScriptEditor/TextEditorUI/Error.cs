using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ScriptEditor.TextEditorUI
{
    public enum ErrorType { Error, Warning, Message, Search }

    public class Error
    {
        public ErrorType type = ErrorType.Error;
        public string message;
        public string fileName;
        public int line;
        public int column;
        public TextLocation ErrorPosition;

        public Error() { }

        public Error(string line, string column)
        {
            ErrorPosition.Line = int.Parse(line) - 1;
            ErrorPosition.Column = int.Parse(column) - 1;
        }

        public Error(ErrorType type, string message, string fileName, int line, int column = -1)
        {
            this.type = type;
            this.message = message;
            this.fileName = fileName;
            this.line = line;
            this.column = column;
        }

        public Error(string message, string fileName, int line, int column = -1)
        {
            this.message = message;
            this.fileName = fileName;
            this.line = line;
            this.column = column;
        }

        public override string ToString()
        {
            return message;
        }
    }

    public class ParseError
    {
        public static string ParserLog(string log, TabInfo tab)
        {
            if (tab.error) {
                List<TextMarker> marker = tab.textEditor.Document.MarkerStrategy.GetMarkers(0, tab.textEditor.Document.TextLength);
                foreach (TextMarker m in marker) tab.textEditor.Document.MarkerStrategy.RemoveMarker(m); 
                tab.error = false;
            }
            bool warn = false;
            string[] sLog = log.Split('\n');
            log = string.Empty;
            for (int i = 0; i < sLog.Length; i++)
            {
                sLog[i] = sLog[i].TrimEnd();
                if (sLog[i].StartsWith("[Error]")) {
                    if (log.Length > 0) log += Environment.NewLine;
                    warn = false;
                    HighlightError(sLog[i], tab);
                }
                if (sLog[i].StartsWith("[Warning]")) {
                    if (!Settings.parserWarn){
                    warn = true;
                    continue;
                    }
                    if (log.Length > 0) log += Environment.NewLine;
                }
                if (!warn) log += sLog[i] + Environment.NewLine;
            }
            if (log.Length > 2)
                log = "------ Script: " + tab.filename
                + " < Parse Time: " + DateTime.Now.ToString("HH:mm:ss")
                + " > ------" + Environment.NewLine + log;
            return log;
        }

        private static void HighlightError(string error, TabInfo tab)
        {
            string[] str = error.Split(new char[] {':'}, 4);
            if (str.Length < 3) return; 
            TextLocation ErrorPosition = new Error(str[2], "1").ErrorPosition;
            int offset = tab.textEditor.Document.PositionToOffset(ErrorPosition);
            int len = TextUtilities.GetLineAsString(tab.textEditor.Document, ErrorPosition.Line).Length;
            TextMarker tm = new TextMarker(offset, len, TextMarkerType.WaveLine, System.Drawing.Color.Red);
            tm.ToolTip = str[str.Length - 1];
            tab.textEditor.Document.MarkerStrategy.AddMarker(tm);
            tab.textEditor.ActiveTextAreaControl.Refresh();
            tab.error = true;
        }
    }
}
