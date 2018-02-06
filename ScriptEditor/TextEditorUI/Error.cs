using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.TextEditor.Document;

namespace ScriptEditor.TextEditorUI
{
    public enum ErrorType { None, Error, Warning, Message, Search, Parser }

    public class Error
    {                                //@"\[\w+\]\s*\<([^\>]+)\>\s*\:(\-?\d+):?(\-?\d+)?\:\s*(.*)"
        private const string pattern = @"(\[\w+\])?\s*\<?([^\>?]+)\>?\s*\:(\-?\d+):?(\-?\d+|\s\w+)?\:\s*(.*)";
        
        public ErrorType type = ErrorType.None;
        public string message;
        public string fileName;
        public int line;
        public int column = -1;
        public int len = -1;

        public Error(ErrorType type)
        {
            this.type = type;
        }
        
        public Error(int line, int len)
        {
            this.line = line;
            this.len = len;
        }

        public Error(string line, string column)
        {
            this.line = int.Parse(line) - 1;
            int col;
            int.TryParse(column, out col);
            this.column = col - 1;
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
        // for compile
        public static void BuildLog(List<Error> errors, string output, string srcfile)
        {
            errors.Clear();
            string[] log = output.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            for (int s = 0; s < log.Length; s++)
            {
                bool warning = (log[s].IndexOf(": warning:") > 0);
                if (log[s].StartsWith("[Error]") || log[s].StartsWith("[Warning]") || log[s].StartsWith("[Message]") || warning) {
                    var error = new Error(ErrorType.Message);
                    if (warning || log[s][1] == 'W')
                        error.type = ErrorType.Warning;
                    else if (log[s][1] == 'E')
                        error.type = ErrorType.Error;

                    GetLogText(log, s, error);

                    // File path correct
                    if ((Settings.useMcpp || Settings.useWatcom) && error.fileName != "none") {
                        string scrName = Path.GetFileName(srcfile);
                        if (error.fileName.IndexOf(scrName) > 0)
                            error.fileName = srcfile;
                    }
                    if (error.fileName != "none" && !Path.IsPathRooted(error.fileName)) {
                        error.fileName = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(srcfile), error.fileName));
                    }
                    errors.Add(error);
                }
            }
        }

        private static void GetLogText(string[] log, int s, Error error)
        {
            Match m = Regex.Match(log[s], pattern);
            error.fileName = m.Groups[2].Value.Replace('/', '\\');
            error.line = int.Parse(m.Groups[3].Value);
            if (m.Groups[4].Value.Length > 0 && !char.IsWhiteSpace(m.Groups[4].Value[0]))
                error.column = int.Parse(m.Groups[4].Value);

            error.message = m.Groups[5].Value.TrimEnd();
            if (error.type == ErrorType.Warning)
                error.message += ": " + log[s + 1].Trim();
        }

        // for parser
        public static string ParserLog(string log, TabInfo tab)
        {
            if (tab.parserErrors.Count > 0) {
                List<TextMarker> marker = tab.textEditor.Document.MarkerStrategy.GetMarkers(0, tab.textEditor.Document.TextLength);
                foreach (TextMarker m in marker) { 
                    if (m.TextMarkerType == TextMarkerType.WaveLine) 
                        tab.textEditor.Document.MarkerStrategy.RemoveMarker(m); 
                }
                tab.parserErrors.Clear();
            }
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("------ Script: {0} < Parse Time: {1} > ------\r\n", 
                            tab.filename, DateTime.Now.ToString("HH:mm:ss"));
            bool warn = false;
            string[] sLog = log.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < sLog.Length; i++)
            {
                if (sLog[i].StartsWith("[Error]")) {
                    sb.AppendLine();
                    warn = false;
                    if (TextEditor.ParsingErrors)
                        HighlightError(sLog[i], tab);
                }
                if (sLog[i].StartsWith("[Warning]")) {
                    if (!Settings.parserWarn) {
                        warn = true;
                        continue;
                    }
                    var error = new Error(ErrorType.Warning);
                    GetLogText(sLog, i, error);
                    //error.type = ErrorType.Parser;
                    tab.parserErrors.Add(error);
                    sb.AppendLine();
                }
                if (!warn) 
                    sb.AppendLine(sLog[i]);
            }

            tab.textEditor.Refresh();

            return sb.ToString();
        }

        private static void HighlightError(string error, TabInfo tab)
        {
            Match m = Regex.Match(error, pattern);
            Error ePosition = new Error(m.Groups[3].Value, m.Groups[4].Value);
            string message = m.Groups[5].Value.TrimEnd();
            string fpath = m.Groups[2].Value;

            if (Path.GetFileName(fpath) == tab.filename) {
                LineSegment ls = tab.textEditor.Document.GetLineSegment(ePosition.line);
                TextMarker tm = new TextMarker(ls.Offset, ls.Length, TextMarkerType.WaveLine, System.Drawing.Color.FromArgb(160, 255, 0, 0));
                tm.ToolTip = message;
                tab.textEditor.Document.MarkerStrategy.AddMarker(tm);
                fpath = tab.filepath;
            }
            // add to error tab
            tab.parserErrors.Add(new Error(ErrorType.Error, message, fpath, ePosition.line + 1, ePosition.column));
        }
    }
}
