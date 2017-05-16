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
    /// Class for parsing procedures SSL code w/o external parser.dll [WIP]
    /// </summary>
    public class Parser
    {
        private static List<string> ProcNameList = new List<string>();
        private static TextEditor scrptEditor;

        const int PROC_LEN = 10; 
        const string PROCEDURE = "procedure ";
        const string BEGIN = "begin";
        const string END = "end";
        public const string INCLUDE = "#include ";
        public const string DEFINE = "#define ";
        public const string COMMENT = "//";

        public static void InternalParser(TabInfo _ti, Form frm)
        {
            scrptEditor = frm as TextEditor;
            File.WriteAllText(Compiler.parserPath, _ti.textEditor.Text);
            ProgramInfo _pi = new ProgramInfo(countProcs(), 0);
            _ti.parseInfo = InternalProcParse(_pi, _ti.textEditor.Text, _ti.filepath);
        }

        // Update to data location of procedures
        public static ProgramInfo UpdateProcsPI(ProgramInfo _pi, string file, string filepath)
        {
            ProgramInfo update_pi = InternalProcParse(new ProgramInfo(countProcs(), 0), file, filepath);
            for (int i = 0; i < _pi.procs.Length; i++)
            {
                foreach (Procedure p in update_pi.procs)
                {
                    if (_pi.procs[i].name.ToLowerInvariant() == p.name && _pi.procs[i].fdeclared == p.fdeclared) {
                        _pi.procs[i].d.start = p.d.start;
                        _pi.procs[i].d.end = p.d.end;
                        _pi.procs[i].d.declared = p.d.declared;
                        break;
                    }
                }
            }
            return _pi;
        }

        private static int countProcs()
        {
            GetNamesProcedures();
            return ProcNameList.Count;
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
            UpdateParseSSL(text);
            for (int i = 0; i < _pi.procs.Length; i++)
            {
                _pi.procs[i] = new Procedure();
                _pi.procs[i].name = ProcNameList[i];
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
            int _comm = 0, m = 0;
            string[] NameProcedures = new string[0];
            string[] file = File.ReadAllLines(Compiler.parserPath);
            for (int i = 0; i < file.Length; i++)
            {
                file[i] = file[i].TrimStart();
                if (CommentBlockParse(ref file[i], ref _comm)) continue;
                if (file[i].ToLowerInvariant().StartsWith(PROCEDURE)) {
                    // get name procedure
                    string s = file[i].Substring(PROC_LEN, file[i].Length - PROC_LEN);
                    // удалить Begin или другую информацию из имени процедуры
                    int z = s.IndexOf(';');
                    if (z > 0) s = s.Remove(z);
                    z = s.IndexOf('(');
                    if (z > 0) s = s.Remove(z);
                    z = s.IndexOf(BEGIN);
                    if (z > 0) s = s.Remove(z);
                    z = s.IndexOf(COMMENT);
                    if (z < 0) z = s.IndexOf("/*");
                    if (z > 0) s = s.Remove(z);
                    s = s.Trim();
                    //
                    Array.Resize(ref NameProcedures, m + 1);
                    NameProcedures[m++] = s;
                }            
            }
            // Delеte duplicates
            ProcNameList.Clear();
            for (int i = 0; i < NameProcedures.Length; i++)
            {
                bool _add = true;
                foreach (string a in ProcNameList)
                {
                    if (NameProcedures[i].ToLowerInvariant() == a.ToLowerInvariant()) _add = false;
                }
                if (_add) ProcNameList.Add(NameProcedures[i]);
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
                // TODO: возможна тут нужна проверка на закоментированный блок /* */
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
                    if (file[i].StartsWith(PROCEDURE) || file[--i].StartsWith(PROCEDURE)) {
                        for (int j = i - 1; j > 0; j--) {
                            if (file[j].StartsWith(PROCEDURE)) return j + 1;
                        }
                        return i - 1;  // if not found procedure declaration
                    } else return -1;  // procedure block is broken
                }
            }
            return 0; // not found
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
                char[] m  = sLine.ToCharArray();
                for (int i = 9; i < m.Length; i++)
                {
                    if (m[i] == ' ' && m[i + 1] == ' ') m[i] = '\0';
                }
                sLine = new string(m).Replace("\0", "");
                if (sLine.StartsWith(PROCEDURE + pName)) return true;
            }
            return false;
        }

        public static void UpdateParseSSL(string sText)
        {
            File.WriteAllText(Compiler.parserPath, sText.ToLowerInvariant());
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

        public static List<string> GetAllIncludes(TabInfo tab)
        {
            List<string> include = new List<string>();
            string[] lines = tab.textEditor.Document.TextContent.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
            string dir = Path.GetDirectoryName(tab.filepath);
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim().ToLowerInvariant();
                if (lines[i].StartsWith(INCLUDE)) {
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
            } // переопределять и неотносительные пути, но тогда все header файлы должны лежать в одной папке.  
            else if (Settings.overrideIncludesPath && Settings.PathScriptsHFile != null) {
                iPath = Path.Combine(Settings.PathScriptsHFile, Path.GetFileName(iPath));
            }
        }
    }
}
