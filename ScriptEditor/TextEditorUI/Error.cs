using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using System.Text.RegularExpressions;

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
        public int len;
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

        public Error(string message, string fileName, int line, int column = -1, int len = -1)
        {
            this.message = message;
            this.fileName = fileName;
            this.line = line;
            this.column = column;
            this.len = len;
        }

        public override string ToString()
        {
            return message;
        }

        public static void BuildLog(List<Error> errors, string output, string srcfile)
        {
            foreach (string s in output.Split(new char[] { '\n' })) {
                if (s.StartsWith("[Error]") || s.StartsWith("[Warning]") || s.StartsWith("[Message]")) {
                    var error = new Error();
                    if (s[1] == 'E') {
                        error.type = ErrorType.Error;
                    } else if (s[1] == 'W') {
                        error.type = ErrorType.Warning;
                    } else {
                        error.type = ErrorType.Message;
                    }
                    Match m = Regex.Match(s, @"\[\w+\]\s*\<([^\>]+)\>\s*\:(\-?\d+):?(\-?\d+)?\:\s*(.*)");
                    error.fileName = m.Groups[1].Value;
                    error.line = int.Parse(m.Groups[2].Value);
                    if (m.Groups[3].Value.Length > 0) {
                        error.column = int.Parse(m.Groups[3].Value);
                    }
                    error.message = m.Groups[4].Value.TrimEnd();
                    if (error.fileName != "none" && !Path.IsPathRooted(error.fileName)) {
                        error.fileName = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(srcfile), error.fileName));
                    }
                    errors.Add(error);
                }
            }
        }

        public static string ParserLog(string log, TabInfo tab)
        {
            if (tab.error) {
                List<TextMarker> marker = tab.textEditor.Document.MarkerStrategy.GetMarkers(0, tab.textEditor.Document.TextLength);
                foreach (TextMarker m in marker) { 
                    if (m.TextMarkerType == TextMarkerType.WaveLine) 
                        tab.textEditor.Document.MarkerStrategy.RemoveMarker(m); 
                }
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
            tab.textEditor.Refresh();
            if (log.Length > 2)
                log = "------ Script: " + tab.filename
                + " < Parse Time: " + DateTime.Now.ToString("HH:mm:ss")
                + " > ------" + Environment.NewLine + log;
            return log;
        }

        private static void HighlightError(string error, TabInfo tab)
        {
            string[] str = error.Split(new char[] {':'}, 4);
            if (str.Length < 3 || Path.GetFileName(str[1].TrimEnd('>')) != tab.filename) return; 
            TextLocation ErrorPosition = new Error(str[2], "1").ErrorPosition;
            int offset = tab.textEditor.Document.PositionToOffset(ErrorPosition);
            int len = TextUtilities.GetLineAsString(tab.textEditor.Document, ErrorPosition.Line).Length;
            TextMarker tm = new TextMarker(offset, len, TextMarkerType.WaveLine, System.Drawing.Color.Red);
            tm.ToolTip = str[str.Length - 1];
            tab.textEditor.Document.MarkerStrategy.AddMarker(tm);
            tab.error = true;
        }
    }
}
