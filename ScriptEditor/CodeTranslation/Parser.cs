using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ScriptEditor.TextEditorUI;

namespace ScriptEditor.CodeTranslation
{
    public struct ProcBlock
    {
        public int begin;
        public int end;
        public bool copy;
    }
    
    /// <summary>
    /// Class for parsing procedures SSL code w/o external parser.dll
    /// </summary>
    public static class Parser
    {
        private static List<string> procNameList = new List<string>();
        private static TextEditor scrptEditor;

        const int PROC_LEN = 10; 
        const string PROCEDURE = "procedure ";
        const string BEGIN = "begin";
        const string END = "end";
        public const string INCLUDE = "#include ";
        public const string DEFINE = "#define ";
        public const string COMMENT = "//";

        public static List<string> ProceduresListName
        {
            get { 
                GetNamesProcedures();
                return procNameList; 
            }
        }

        public static void InternalParser(TabInfo _ti, Form frm)
        {
            scrptEditor = frm as TextEditor;
            TextEditor.parserRunning = true; // internal parse work
            File.WriteAllText(Compiler.parserPath, _ti.textEditor.Text);
            ProgramInfo _pi = new ProgramInfo(CountProcedures, 0);
            _ti.parseInfo = InternalProcParse(_pi, _ti.textEditor.Text, _ti.filepath);
            TextEditor.parserRunning = false;
        }

        // Update to data location of procedures
        public static ProgramInfo UpdateProcsPI(ProgramInfo _pi, string file, string filepath)
        {
            ProgramInfo update_pi = InternalProcParse(new ProgramInfo(CountProcedures, 0), file, filepath);
            for (int i = 0; i < _pi.procs.Length; i++)
            {
                foreach (Procedure p in update_pi.procs)
                {
                    if (String.Equals(_pi.procs[i].name, p.name, StringComparison.OrdinalIgnoreCase) && _pi.procs[i].fdeclared == p.fdeclared) {
                        _pi.procs[i].d.start = p.d.start;
                        _pi.procs[i].d.end = p.d.end;
                        _pi.procs[i].d.declared = p.d.declared;
                        break;
                    }
                }
            }
            return _pi;
        }

        private static int CountProcedures
        {
            get {
                GetNamesProcedures();
                return procNameList.Count;
            }
        }

        // Get procedure data
        private static ProgramInfo InternalProcParse(ProgramInfo _pi, string text, string scriptFile)
        {
            #region Procedures data
            /*  pi.procs[].d.start        - номер строки начала тела процедуры
             *  pi.procs[].d.end          - номер строки конца тела процедуры
             *  pi.procs[].d.declared     - номер строки с объявление процедуры
             *  pi.procs[].name           - имя процедуры
             *  pi.procs[].fdeclared      - путь и имя файла к скрипту (pi.procs[].fstart тоже самое)
             *  pi.procs[].filename       - имя файла скрипта */
            #endregion

            ProcBlock be_block = new ProcBlock();
            UpdateParseSSL(text, false);
            for (int i = 0; i < _pi.procs.Length; i++)
            {
                _pi.procs[i] = new Procedure();
                _pi.procs[i].name = procNameList[i];
                _pi.procs[i].d.declared = GetDeclarationProcedureLine(_pi.procs[i].name) + 1;
                be_block = GetProcBeginEndBlock(_pi.procs[i].name, _pi.procs[i].d.declared - 1);
                _pi.procs[i].d.start = be_block.begin + 1;
                _pi.procs[i].d.end = be_block.end + 1;
                _pi.procs[i].fdeclared = Path.GetFullPath(scriptFile);
                _pi.procs[i].fstart = _pi.procs[i].fdeclared;
                _pi.procs[i].filename = Path.GetFileName(scriptFile).ToLowerInvariant();
                _pi.procs[i].references = new Reference[0];  // empty not used
                _pi.procs[i].variables = new Variable[0];    // empty not used
            }
            _pi.parsed = true;
            return _pi;
        }

        // Обновить данные begin...end блоков процедур
        public static void UpdateProcInfo(ref ProgramInfo _pi, string text, string scriptFile)
        {
            ProcBlock be_block = new ProcBlock();
            UpdateParseSSL(text);
            for (int i = 0; i < _pi.procs.Length; i++)
            {
                if (_pi.procs[i].fdeclared != scriptFile) continue;
                _pi.procs[i].d.declared = GetDeclarationProcedureLine(_pi.procs[i].name) + 1;
                be_block = GetProcBeginEndBlock(_pi.procs[i].name, _pi.procs[i].d.declared - 1);
                _pi.procs[i].d.start = be_block.begin + 1;
                _pi.procs[i].d.end = be_block.end + 1;
            }
        } 

        // Получить список всех процедур из скрипта
        private static void GetNamesProcedures()
        {
            int _comm = 0;
            procNameList.Clear();
            string[] file = File.ReadAllLines(Compiler.parserPath);
            for (int i = 0; i < file.Length; i++)
            {
                bool _add = true;
                file[i] = file[i].TrimStart();
                if (CommentBlockParse(ref file[i], ref _comm)) continue;
                if (file[i].StartsWith(PROCEDURE, StringComparison.OrdinalIgnoreCase)) {
                    // get name procedure
                    string pName = file[i].Substring(PROC_LEN, file[i].Length - PROC_LEN);
                    // delete Begin or other information from procedure name
                    int z = pName.IndexOf(';');
                    if (z > 0) pName = pName.Remove(z);
                    z = pName.IndexOf('(');
                    if (z > 0) pName = pName.Remove(z);
                    z = pName.IndexOf(BEGIN);
                    if (z > 0) pName = pName.Remove(z);
                    z = pName.IndexOf(COMMENT);
                    if (z < 0) z = pName.IndexOf("/*");
                    if (z > 0) pName = pName.Remove(z);
                    pName = pName.Trim();
                    //
                    foreach (string name in procNameList) {
                        if (String.Equals(pName, name, StringComparison.OrdinalIgnoreCase)) {
                            _add = false;
                            break;
                        }
                    }
                    if (_add) procNameList.Add(pName);  
                }            
            }
        }

        // Получить номер строки с объявленой процедурой
        public static int GetDeclarationProcedureLine(string pName)
        {
            string[] file = File.ReadAllLines(Compiler.parserPath);
            pName = pName.ToLowerInvariant();
            int pLen = pName.Length;
            for (int i = 0; i < file.Length; i++) {
                file[i] = file[i].Trim();
                // TODO: возможно тут нужна проверка на закоментированный блок /* */
                if (IsProcedure(ref file[i], pName)) {
                    if (file[i].Length <= (PROC_LEN + pLen)) continue; // broken declare
                    RemoveDebrisLine(file, pLen, i);
                    if (file[i].LastIndexOf(';') >= (PROC_LEN + pLen)) return i; //found
                }
            }
            return -1; // not found
        }

        // Получить последнию строку в списке 'procedure declaration'
        public static int GetEndLineProcDeclaration()
        {
            int _comm = 0;
            string[] file = File.ReadAllLines(Compiler.parserPath);
            for (int i = 0; i < file.Length; i++) 
            {
                file[i] = file[i].Trim();
                if (CommentBlockParse(ref file[i], ref _comm)) continue;
                // убираем лишнее
                RemoveDebrisLine(file, 0-PROC_LEN, i);
                if (file[i].EndsWith(BEGIN)) {
                    if (!file[i].StartsWith(PROCEDURE) && !file[i - 1].StartsWith(PROCEDURE)) 
                        continue; 
                    for (int j = i - 1; j > 0; j--) {
                       if (file[j].StartsWith(PROCEDURE)) return j + 1;
                    }
                    if (++i <= file.Length) continue;
                    return -1;  // procedure block is broken
                }
            }
            return 0; // not found procedure declaration
        }

        // Получить последнию строку declaration
        public static int GetEndRegionDeclaration(string text, int line)
        {
            int _comm = 0;
            string[] file = text.Split('\n');
            for (int i = line; i > 0; i--)
            {
                file[i] = file[i].TrimStart();
                if (CommentBlockParse(ref file[i], ref _comm)) continue;
                RemoveDebrisLine(file, 0-PROC_LEN, i);
                if (file[i].StartsWith(PROCEDURE, StringComparison.OrdinalIgnoreCase))
                    return i;  // found end declaration
            }
            return -1; // not found
        }

        // Получить для заданной процедуры номера строк блока Begin...End
        public static ProcBlock GetProcBeginEndBlock(string pName, int startline = 0, bool procBegin = false)
        {
            ProcBlock block = new ProcBlock();
            int _begin = 0, _proc = 0, _comm = 0, lineProc = 0;
            string[] file = File.ReadAllLines(Compiler.parserPath);
            pName = pName.ToLowerInvariant();
            int pLen = pName.Length;
            if (startline < 0) startline = 0;
            for (int i = startline; i < file.Length; i++)
            {
                file[i] = file[i].Trim();
                if (CommentBlockParse(ref file[i], ref _comm)) continue;

                // ищем начало процедуры с искомым именем
                if (_proc == 0 && IsProcedure(ref file[i], pName))
                {   // нашли procedure name, проверяем 100% совпадает c искомым?
                    string s = " ";
                    if (file[i].Length > PROC_LEN + pLen) s = file[i].Substring(PROC_LEN + pLen, 1);
                    if (s != " " && s != "(") continue; //не совпадает, ищем дальше
                    _proc++; // совпадает, проверяем это процедура или ее объявление
                    // убираем лишнее
                    RemoveDebrisLine(file, pLen, i);
                    if (file[i].EndsWith(BEGIN)) {
                        block.begin = i; //да это процедура, присваеваем значение строки в begin
                        _begin++;
                        continue;
                    } else { // нет, продолжаем искать begin
                        lineProc = i; // save for procBegin
                        continue;
                    }
                } // ищем begin от процедуры.
                else if (_proc > 0 && _begin == 0) {
                    if (file[i].StartsWith(BEGIN)) {
                        _begin++; // нашли begin
                        block.begin = (procBegin) ? lineProc : i; // возвращаем номер строки с процедурой
                        continue;
                    }
                    // в процессе проверяем и любое объявление процедур
                    if (file[i].StartsWith(PROCEDURE)) {
                        // нашли - откат назад, будем продолжать искать нашу "procedure Name"
                        _proc--;
                        i--;
                        continue;
                    }
                }
                // нашли begin, теперь ищем начало следующей процедуры
                // и от ее позиции будем искать 'END' принадлежащий к искомой "procedure Name"
                if (_proc > 0 && _begin > 0 && file[i].StartsWith(PROCEDURE)) {
                    // нашли следующую процедуруh);
                    for (int j = i - 1; j > 0; j--) // back find 
                    {   
                        if (file[j].StartsWith(END)) {
                            // found "end"
                            block.end = j;
                            return block ; // return
                        } else if (j <= block.begin) {
                            scrptEditor.intParserPrint("[Parsing Error] Line#" + (block.begin + 1) + ": When parsing of procedure '" + pName + "' construct keyword 'End' not found.\r\n");
                            block.end = block.begin + 1;
                            return block; // procedure block is broken
                        } 
                    }
                } 
            }
            // обработка вслучае последней процедуры в скрипте
            if (block.end == 0 && _proc > 0 && _begin > 0) {
                for (int i = file.Length - 1; i > 0; i--) // back find 
                {
                    if (file[i].StartsWith(END)) {
                        block.end = i;
                        return block;
                    }
                }
            }
            scrptEditor.intParserPrint("[Parsing Error] When parsing of procedure '" + pName + "' construct Begin...End, an unexpected error occurred.\r\n");
            block.begin = -1;
            block.end = -1;
            return block; // что-то пошло не так, достигнут конец файла
        }

        private static void RemoveDebrisLine(string[] file, int pLen, int i)
        {
            int z = file[i].IndexOf(COMMENT, PROC_LEN + pLen);
            if (z < 0) z = file[i].IndexOf("/*", PROC_LEN + pLen);
            if (z > 0) file[i] = file[i].Remove(z).TrimEnd();
        }

        // Comment block parse
        private static bool CommentBlockParse(ref string sLine, ref int _comm)
        {
            if (sLine.StartsWith(COMMENT) || sLine.Length < 2) return true;
            int cStart = sLine.IndexOf("/*");
            if (cStart != -1 && _comm == 0) { // sLine.StartsWith("/*")
                // удаление из строки закомментированного блока '/* ... */'
                int cEnd = sLine.IndexOf("*/");
                if (cEnd < 0) {
                    _comm++;
                    sLine = string.Empty; // clear comment line
                    return true;
                } else sLine = sLine.Remove(cStart, (cEnd + 2) - cStart).Insert(cStart, " ").TrimStart();
            }
            else if (_comm > 0) {
                if (sLine.IndexOf("*/") > 0 || sLine.StartsWith("*/")) {
                    _comm--;
                    if (sLine.Length > 2) {
                        // удаление комментария из строки с закрывающим тэгом '*/'
                        cStart = sLine.IndexOf("*/");
                        sLine = sLine.Remove(0, cStart + 2).TrimStart();
                    }
                } else {
                    sLine = string.Empty; // clear comment line
                    return true;
                }
            }
            return false;
        }

        private static bool IsProcedure(ref string sLine, string pName)
        {
            if (sLine.StartsWith(PROCEDURE)) {
                // удаление двойных пробелов в строке процедуры
                char[] ch  = sLine.ToCharArray();
                for (int i = 9; i < ch.Length; i++)
                    if (ch[i] == ' ' && ch[i + 1] == ' ') ch[i] = '\0';
                sLine = new string(ch).Replace("\0", "");
                if (sLine.StartsWith(PROCEDURE + pName)) return true;
            }
            return false;
        }

        public static void UpdateParseSSL(string sText, bool check = true, bool lower = true)
        {
            while (check && TextEditor.parserRunning) System.Threading.Thread.Sleep(1); //Avoid stomping on files while the parser is running
            File.WriteAllText(Compiler.parserPath, (lower) ? sText.ToLowerInvariant(): sText);
        }

        public static bool CheckExistsProcedureName(string pName)
        {
            string[] file = File.ReadAllLines(Compiler.parserPath);
            pName = pName.ToLowerInvariant();
            for (int i = 0; i < file.Length; i++)
            {
                file[i] = file[i].Trim();
                // TODO: возможна тут нужна проверка на закоментированный блок /* */
                if (IsProcedure(ref file[i], pName)) {
                    MessageBox.Show("This procedure has already declared.", "Info");
                    return true; // found
                }
            }
            return false; // not found
        }

        public static Procedure GetProcedurePosition(Procedure[] procScript, int linePosition)
        {
            linePosition++;
            foreach (Procedure proc in procScript)
            {
                if (linePosition >= proc.d.start & linePosition <= proc.d.end)
                    return proc;
            }
            return null;
        }

        public static List<string> GetAllIncludes(TabInfo tab)
        {
            List<string> include = new List<string>();
            string[] lines = tab.textEditor.Document.TextContent.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
            string dir = Path.GetDirectoryName(tab.filepath);
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
                if (lines[i].StartsWith(INCLUDE, StringComparison.OrdinalIgnoreCase)) {
                    string[] text = lines[i].Split('"');
                    if (text.Length < 2)
                        continue;
                    if (text[1].IndexOfAny(Path.GetInvalidPathChars()) != -1)
                        continue;
                    includePath(ref text[1], dir);
                    include.Add(text[1]);
                }
            }
            return include;
        }

        // Override includes path
        public static void includePath(ref string iPath, string dir)
        {
            if (!Path.IsPathRooted(iPath)) {
                if (Settings.overrideIncludesPath && Settings.PathScriptsHFile != null)
                    iPath = Path.GetFullPath(Path.Combine(Settings.PathScriptsHFile, iPath));
                else
                    iPath = Path.GetFullPath(Path.Combine(dir, iPath));
            } // переопределять и неотносительные пути (все headers файлы должны лежать в одной папке)
            else if (Settings.overrideIncludesPath && Settings.PathScriptsHFile != null) {
                iPath = Path.Combine(Settings.PathScriptsHFile, Path.GetFileName(iPath));
            }
        }
    }
}
