using System;
using System.Collections.Generic;
using System.Text;

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

        public Error() { }

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
        public static string ParserLog(string log, string filename)
        {
            bool warn = false;
            string[] sLog = log.Split('\n');
            log = string.Empty;
            for (int i = 0; i < sLog.Length; i++)
            {
                sLog[i] = sLog[i].Replace("\r", string.Empty);
                if (sLog[i].StartsWith("[Error]")) {
                    if (log.Length > 0) log += Environment.NewLine;
                    warn = false;
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
                log = "------ Script: " + filename
                + " < Parse Time: " + DateTime.Now.ToString("HH:mm:ss")
                + " > ------" + Environment.NewLine + log;
            return log;
        }
    }
}
