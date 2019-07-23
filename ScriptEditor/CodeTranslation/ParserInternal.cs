using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ScriptEditor.TextEditorUI;

using ICSharpCode.TextEditor.Document;

namespace ScriptEditor.CodeTranslation
{
    public struct ProcBlock
    {
        public int declar;      // Строка декларации процедуры
        public int begin;       // Строка указывающая на begin процедуры
        public int end;         // Строка указывающая на end процедуры
        public bool copy;
    }
    
    /// <summary>
    /// Class for parsing procedures SSL code w/o external parser.dll
    /// </summary>
    public class ParserInternal
    {
        private static List<string> procNameList = new List<string>();
        private static TextEditor scrptEditor;

        private static string[] bufferSSL; // text buffer for ssl script code

        public const int PROC_LEN = 10;
        public const string PROCEDURE = "procedure ";
        public const string BEGIN = "begin";
        const string END = "end";
        public const string INCLUDE = "#include ";
        public const string DEFINE = "#define ";
        const string COMMENT = "//";
        public const string VARIABLE = "variable ";

        public static List<string> ProceduresListName
        {
            get { 
                GetNamesProcedures();
                return procNameList; 
            }
        }

        private static int CountProcedures
        {
            get {
                GetNamesProcedures();
                return procNameList.Count;
            }
        }

        /// <summary>
        /// Internal parse script
        /// </summary>
        /// <param name="_ti"></param>
        /// <param name="frm"></param>
        public ParserInternal(TabInfo _ti, Form frm)
        {
            scrptEditor = frm as TextEditor;
            TextEditor.parserRunning = true; // internal parse work
            
            UpdateParseSSL(_ti.textEditor.Text, false);
            
            ProgramInfo _pi = new ProgramInfo(CountProcedures, 0);
            _ti.parseInfo = InternalProcParse(_pi, _ti.textEditor.Text, _ti.filepath);
            
            TextEditor.parserRunning = false;
        }

        /// <summary>
        /// Обновляет данные о процедурах, удаляет устаревшие или добавляет новые
        /// </summary>
        /// <param name="_pi"></param>
        /// <param name="textscript"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static ProgramInfo UpdateProcsPI(ProgramInfo _pi, string textscript, string filepath)
        {
            List<Procedure> _proc = _pi.procs.ToList();

            UpdateParseSSL(textscript, false);
            ProgramInfo update_pi = InternalProcParse(new ProgramInfo(CountProcedures, 0), textscript, filepath);
            
            for (int i = 0; i < _proc.Count; i++)
            {
                bool exist = false;
                foreach (Procedure p in update_pi.procs)
                {
                    if (String.Equals(_proc[i].name, p.name, StringComparison.OrdinalIgnoreCase)
                        && _proc[i].fdeclared == p.fdeclared) {
                        _proc[i].d.start = p.d.start;
                        _proc[i].d.end = p.d.end;
                        _proc[i].d.declared = p.d.declared;
                        exist = true;
                        break;
                    }
                }
                if (!exist) // remove unused procedure
                    _proc.RemoveAt(i--);
            }

            for (int i = 0; i < update_pi.procs.Length; i++)
            {
                bool exist = false;
                for (int j = 0; j < _proc.Count; j++)
                {
                    if (String.Equals(update_pi.procs[i].name, _proc[j].name, StringComparison.OrdinalIgnoreCase)
                        && update_pi.procs[i].fdeclared == _proc[j].fdeclared) {
                            exist = true;
                            break;
                    }
                }
                if (!exist) // Add new procedure
                    _proc.Insert(i, update_pi.procs[i]);
            }

            _pi.procs = _proc.ToArray();
            _pi.RebuildProcedureDictionary();
            
            return _pi;
        }

        /// <summary>
        /// Получает новые данные о процедурах из кода скрипта
        /// </summary>
        /// <param name="_pi"></param>
        /// <param name="text">Текущий текст скрипта</param>
        /// <param name="scriptFile">Файл скрипта</param>
        /// <returns></returns>
        private static ProgramInfo InternalProcParse(ProgramInfo _pi, string text, string scriptFile, bool bufferUpdate = true)
        {
            #region Procedures info data
            /*  pi.procs[].d.start        - номер строки начала тела процедуры
             *  pi.procs[].d.end          - номер строки конца тела процедуры
             *  pi.procs[].d.declared     - номер строки с объявление процедуры
             *  pi.procs[].name           - имя процедуры
             *  pi.procs[].fdeclared      - путь и имя к файлу где объявлена процедура 
             *  pi.procs[].fstart         - путь и имя к файлу где расположана процедура    
             *  pi.procs[].filename       - имя файла скрипта */
            #endregion
            
            if (bufferUpdate) UpdateParseSSL(text);

            ProcBlock be_block = new ProcBlock();
            for (int i = 0; i < _pi.procs.Length; i++)
            {
                _pi.procs[i] = new Procedure();
                _pi.procs[i].name = procNameList[i];
                _pi.procs[i].d.declared = GetDeclarationProcedureLine(_pi.procs[i].name) + 1;
                be_block = GetProcBeginEndBlock(_pi.procs[i].name, _pi.procs[i].d.declared - 1);
                _pi.procs[i].d.start = (be_block.begin >= 0) ? be_block.begin + 1 : -1;
                _pi.procs[i].d.end = (be_block.end >= 0) ? be_block.end + 1 : -1;
                _pi.procs[i].fdeclared = Path.GetFullPath(scriptFile);
                _pi.procs[i].fstart = _pi.procs[i].fdeclared;
                _pi.procs[i].filename = Path.GetFileName(scriptFile).ToLowerInvariant();
                _pi.procs[i].references = new Reference[0];  // empty not used
                _pi.procs[i].variables = new Variable[0];    // empty not used
            }
            _pi.parsed = true;
            
            bufferSSL = null;
            
            return _pi;
        }

        /// <summary>
        /// Обновляет данные для процедур позицию строк деклараций и блока begin/end из кода скрипта
        /// </summary>
        /// <param name="_pi">Данные которые требуется обновить</param>
        /// <param name="text">Текущий текст скрипта</param>
        /// <param name="scriptFile">Файл скрипта</param>
        public static void UpdateProcInfo(ref ProgramInfo _pi, string text, string scriptFile)
        {
            UpdateParseSSL(text);
            
            ProcBlock be_block = new ProcBlock();
            for (int i = 0; i < _pi.procs.Length; i++)
            {
                if (_pi.procs[i].fdeclared != scriptFile) 
                    continue;
                _pi.procs[i].d.declared = GetDeclarationProcedureLine(_pi.procs[i].name) + 1;
                be_block = GetProcBeginEndBlock(_pi.procs[i].name, _pi.procs[i].d.declared - 1);
                _pi.procs[i].d.start = (be_block.begin >= 0) ? be_block.begin + 1 : -1;
                _pi.procs[i].d.end = (be_block.end >= 0) ? be_block.end + 1 : -1;
            }
            _pi.RebuildProcedureDictionary();

            bufferSSL = null;
        }

        // Get new procedure data of script
        public static Procedure[] GetProcsData(string textscript, string filepath)
        {
            UpdateParseSSL(textscript, false); // Не переводить в нижний регист для правильных имен процедур
            
            ProgramInfo _pi = InternalProcParse(new ProgramInfo(CountProcedures, 0), textscript, filepath);
            
            return _pi.procs;
        }

        // Получить список всех процедур из текущего буфера
        private static void GetNamesProcedures()
        {
            int _comm = 0;
            procNameList.Clear();
            
            for (int i = 0; i < bufferSSL.Length; i++)
            {
                bool _add = true;
                bufferSSL[i] = bufferSSL[i].TrimStart();
                
                if (CommentBlockParse(ref bufferSSL[i], ref _comm))
                    continue;
                
                if (bufferSSL[i].StartsWith("import", StringComparison.OrdinalIgnoreCase)
                    || bufferSSL[i].StartsWith("export", StringComparison.OrdinalIgnoreCase)) {
                    bufferSSL[i] = bufferSSL[i].Remove(0, 7).TrimStart();
                }

                if (bufferSSL[i].StartsWith(PROCEDURE, StringComparison.OrdinalIgnoreCase)) {
                    // get name procedure
                    string pName = bufferSSL[i].Substring(PROC_LEN, bufferSSL[i].Length - PROC_LEN);
                    
                    // delete Begin or other information from procedure name
                    int z = pName.IndexOf(';');
                    if (z > 0)
                        pName = pName.Remove(z);

                    z = pName.IndexOf('(');
                    if (z > 0)
                        pName = pName.Remove(z);
                    
                    z = pName.IndexOf(BEGIN);
                    if (z > 0)
                        pName = pName.Remove(z);

                    RemoveCommentLine(ref pName, 0);
                    
                    pName = pName.Trim();
                    //
                    foreach (string name in procNameList)
                    {
                        if (String.Equals(pName, name, StringComparison.OrdinalIgnoreCase)) {
                            _add = false;
                            break;
                        }
                    }
                    if (_add)
                        procNameList.Add(pName);  
                }            
            }
        }

        /// <summary>
        /// Получить номер строки объявления для процедуры
        /// </summary>
        /// <param name="pName">Имя процедуры</param>
        /// <returns>Номер строки в коде скрипта (-1 если строка декларация процедуры не найдена)</returns>
        public static int GetDeclarationProcedureLine(string pName)
        {
            pName = pName.ToLowerInvariant();
            int pLen = pName.Length;
            
            for (int i = 0; i < bufferSSL.Length; i++)
            {
                bufferSSL[i] = bufferSSL[i].Trim();
                
                // TODO: возможно тут нужна проверка на закоментированный блок /* */
                
                if (bufferSSL[i].StartsWith("import") || bufferSSL[i].StartsWith("export"))
                    bufferSSL[i] = bufferSSL[i].Remove(0, 7).TrimStart();

                if (IsProcedure(ref bufferSSL[i], pName)) {
                    if (bufferSSL[i].Length <= (PROC_LEN + pLen))
                        continue; // broken declare

                    RemoveCommentLine(ref bufferSSL[i], PROC_LEN + pLen);
                    
                    if (bufferSSL[i].LastIndexOf(';') >= (PROC_LEN + pLen))
                        return i; //found
                }
            }
            return -1; // not found
        }

        /// <summary>
        /// Получить номер последней строки в списке процедурных деклараций
        /// </summary>
        /// <returns>Номер строки в коде (0 если строку не удалось получить)</returns>
        public static int GetEndLineProcDeclaration()
        {
            int _comm = 0;
            
            for (int i = 0; i < bufferSSL.Length; i++) 
            {
                bufferSSL[i] = bufferSSL[i].Trim();
                if (CommentBlockParse(ref bufferSSL[i], ref _comm))
                    continue;
                
                // убираем лишнее
                RemoveCommentLine(ref bufferSSL[i], 0);
                
                if (bufferSSL[i].EndsWith(BEGIN)) {
                    if (!bufferSSL[i].StartsWith(PROCEDURE) && !bufferSSL[i - 1].StartsWith(PROCEDURE)) 
                        continue; 
                    for (int j = i - 1; j > 0; j--) {
                       if (bufferSSL[j].StartsWith(PROCEDURE)) 
                           return j + 1;
                    }
                    if (++i <= bufferSSL.Length)
                        continue;
                    return -1;  // procedure block is broken
                }
            }
            return 0; // not found procedure declaration
        }

        /// <summary>
        /// Получить для указанной процедуры номера ее строк Begin...End блока 
        /// </summary>
        /// <param name="pName">Имя процедуры</param>
        /// <param name="startline">Строка в коде с которой необходимо начать поиск</param>
        /// <param name="procBegin">procBegin=True - получить номер строки процедуры, а не строки Begin блока</param>
        public static ProcBlock GetProcBeginEndBlock(string pName, int startline = 0, bool procBegin = false)
        {
            int _begin = 0, _proc = 0, _comm = 0, lineProc = 0;
            ProcBlock block = new ProcBlock() { declar = -1 };
            
            pName = pName.ToLowerInvariant();
            int pLen = pName.Length;
            if (startline < 0)
                startline = 0;
            
            for (int i = startline; i < bufferSSL.Length; i++)
            {
                bufferSSL[i] = bufferSSL[i].Trim();
                if (CommentBlockParse(ref bufferSSL[i], ref _comm))
                    continue;
                // ищем начало процедуры с искомым именем
                if (_proc == 0 && IsProcedure(ref bufferSSL[i], pName))
                {   // нашли procedure name, проверяем 100% совпадает c искомым?
                    block.declar = i;
                    string s = " ";
                    if (bufferSSL[i].Length > PROC_LEN + pLen)
                        s = bufferSSL[i].Substring(PROC_LEN + pLen, 1);
                    if (s != " " && s != "(")
                        continue; //не совпадает, ищем дальше
                    _proc++; // совпадает, проверяем это процедура или ее объявление
                    // убираем лишнее
                    RemoveCommentLine(ref bufferSSL[i], PROC_LEN + pLen);
                    if (bufferSSL[i].EndsWith(BEGIN)) {
                        block.begin = i; //да это процедура, присваеваем значение строки в begin
                        _begin++;
                        continue;
                    } else { // нет, продолжаем искать begin
                        lineProc = i; // save for procBegin, строка Procedure name
                        continue;
                    }
                } // ищем begin от процедуры.
                else if (_proc > 0 && _begin == 0) {
                    if (bufferSSL[i].StartsWith(BEGIN)) {
                        _begin++; // нашли begin
                        block.begin = (procBegin) ? lineProc : i; // возвращаем номер строки с процедурой
                        continue;
                    }
                    // в процессе проверяем и любое объявление процедур
                    if (bufferSSL[i].StartsWith(PROCEDURE)) {
                        // нашли - откат назад, будем продолжать искать нашу "procedure Name"
                        _proc--;
                        i--;
                        continue;
                    }
                }
                // нашли begin, теперь ищем начало следующей процедуры
                // и от ее позиции будем искать 'END' принадлежащий к искомой "procedure Name"
                if (_proc > 0 && _begin > 0 && bufferSSL[i].StartsWith(PROCEDURE)) {
                    // нашли следующую процедуру
                    for (int j = i - 1; j > 0; j--) // back find 
                    {   
                        if (bufferSSL[j].StartsWith(END)) {
                            // found "end"
                            block.end = j;
                            return block ; // return
                        } else if (j <= block.begin) {
                            block.end = -1; //i - 
                            scrptEditor.intParserPrint(String.Format("[Error] <Internal Parser> Line: {0} : When parsing of procedure '{1}'" + 
                                                                     " of the keyword 'end' was not found.\r\n", block.end + 1, pName));
                            return block; // procedure block end is broken
                        } 
                    }
                } 
            }
            // обработка вслучае последней процедуры в скрипте
            if (block.end == 0 && _proc > 0 && _begin > 0) {
                for (int i = bufferSSL.Length - 1; i > 0; i--) // back find 
                {
                    if (bufferSSL[i].StartsWith(END)) {
                        block.end = i;
                        return block;
                    }
                }
            }
            block.begin = -1;
            block.end = -1;
            scrptEditor.intParserPrint(String.Format("[Error] <Internal Parser> Line: {0} : When parsing the procedure '{1}' an unexpected error occurred," +
                                                     " the construction of the code 'begin...end' was not determined.\r\n", block.declar + 1, pName));
            return block;
        }

        /// <summary>
        /// Удаляет все комментарии из текстовой строки
        /// </summary>
        /// <param name="pLen">Позиция в строке откуда будет начат поиск</param>
        private static void RemoveCommentLine(ref string buff, int pLen)
        {
            int z = CheckAtComment(buff, pLen);
            if (z > 0)
                buff = buff.Remove(z).TrimEnd();
        }

        private static int CheckAtComment(string buff, int pLen)
        {
            int z = buff.IndexOf(COMMENT, pLen);
            int y = buff.IndexOf("/*", pLen);
            if (z > 0 && y > 0)
                z = Math.Min(z, y);
            else
                z = Math.Max(z, y);

            return z;
        }

        // Comment block parse
        public static bool CommentBlockParse(ref string sLine, ref int _comm)
        {
            if (sLine.Length < 2 || sLine.StartsWith(COMMENT))
                return true;

            int cStart = sLine.IndexOf("/*");
            if (cStart != -1 && _comm == 0) {
                // удаление из строки закомментированного блока '/* ... */'
                int cEnd = sLine.IndexOf("*/");
                if (cEnd < 0) {
                    _comm++;
                    sLine = sLine.Remove(cStart); // clear comment line
                    return true;
                } else 
                    sLine = sLine.Remove(cStart, (cEnd + 2) - cStart).Insert(cStart, " ").TrimStart();
            }
            else if (_comm > 0) {
                int cEnd = sLine.IndexOf("*/");
                if (cEnd != -1) {
                    _comm--;
                    if (sLine.Length > 2) {
                        // удаление комментария из строки с закрывающим тэгом '*/'
                        sLine = sLine.Remove(0, cEnd + 2).TrimStart();
                    }
                } else {
                    sLine = string.Empty; // clear comment line
                    return true;
                }
            }
            return false;
        }

        // Определить содержит ли проверяемая строка коючевое слово "procedure " c указанным именем
        private static bool IsProcedure(ref string sLine, string pName)
        {
            if (sLine.StartsWith(PROCEDURE)) {
                // удаление двойных пробелов между словом процедура и ее именем
                int z = sLine.IndexOf("  ", 9);
                if (z > 0) {
                    int x = CheckAtComment(sLine, 9);
                    
                    int y = sLine.IndexOfAny(new char[] {';', ')'}, 9);
                    if (y > 0) { // определяем наименьшее значение x и y
                        if (x == -1 || y < x) // позиция скобки не находится в комментариях
                            x = y;        
                    }
                    if (x == -1 || z < x) // двойные пробелы расположены не за пределами проверки
                        sLine = RemoveDoubleWhiteSpaces(sLine, z, x);
                }
                if (sLine.StartsWith(PROCEDURE + pName)) {
                    int ePos = PROC_LEN + pName.Length;
                    // проверяем следующий символ за именем процедуры
                    if (ePos >= sLine.Length || sLine[ePos] == ';' || sLine[ePos] == '('
                        || (!char.IsLetterOrDigit(sLine[ePos]) && !char.IsPunctuation(sLine[ePos])))
                        return true; // строка с именем процедуры совпадает
                }
            }
            return false;
        }

        internal static string RemoveDoubleWhiteSpaces(string sLine, int start, int end)
        {
            char[] ch  = sLine.ToCharArray();
            end = (end > 0) ? end : ch.Length -1;
            for (int i = start; i < end; i++)
            {
                if (char.IsWhiteSpace(ch[i]) && char.IsWhiteSpace(ch[i + 1]))
                    ch[i] = '\0';
            }
            return new string(ch).Replace("\0", string.Empty);
        }

        /// <summary>
        /// Занести во временный буфер текст скрипта
        /// </summary>
        /// <param name="sText">Код скрипта</param>
        /// <param name="lower"></param>
        public static void UpdateParseSSL(string sText, bool lower = true)
        {
            char delimeter = '\n';
            bufferSSL = (lower) ? sText.ToLowerInvariant().Split(delimeter)
                                : sText.Split(delimeter);
        }

        public static void FixProcedureBegin(string file)
        {
            List<string> script = File.ReadAllLines(file, Encoding.Default).ToList();
            for (int i = 0; i < script.Count; i++)
            {
                if (script[i].StartsWith(ParserInternal.PROCEDURE, StringComparison.OrdinalIgnoreCase)) {
                    if (script[i + 1].StartsWith(ParserInternal.BEGIN, StringComparison.OrdinalIgnoreCase)) {
                        script[i] += '\u0020' + script[i + 1];
                        script.RemoveAt(i + 1);
                    }
                }
            }
            File.WriteAllLines(file, script, Encoding.Default);
        }

        #region Get declaration region
        /// <summary>
        /// Получить номера строк для региона деклараций
        /// </summary>
        public static ProcBlock GetRegionDeclaration(string text, int lineStart)
        {
            bufferSSL = text.Split(new char[]{'\n'}, lineStart + 1);
            int lenBuff = bufferSSL.Length - 1;
            
            int _comm = 0;
            bool ret = false;
            bool check = false;

            ProcBlock block = new ProcBlock() { begin = 0, end = -1 };

            for (int i = 0; i < lenBuff; i++)
            {
                bufferSSL[i] = bufferSSL[i].Trim();
                if (ret) {
                    if (bufferSSL[i].Length == 0) {
                        check = false;
                        continue;
                    } else {
                        block.begin = (check) ? i - 1 : i; // found begin declaration
                        break;
                    }
                }
                check = (bufferSSL[i].Length > 0);
                if (CommentBlockParse(ref bufferSSL[i], ref _comm))
                    continue;
                ret = true;
            }
                        
            for (int i = lineStart - 1; i > block.begin; i--)
            {
                if (bufferSSL[i].Trim().Length > 0) {
                    block.end = i;  // found end declaration
                    break;
                }
            }
            return block;
        }

        public static List<ProcBlock> GetFoldingBlock(string text, int start = 0)
        {
            List<ProcBlock> list = new List<ProcBlock>();
            ProcBlock block = new ProcBlock() { begin = -1, end = -1 };
            ProcBlock ifblock = new ProcBlock() { begin = -1, end = -1, copy = true};

            int _comm = 0;
            bufferSSL = text.Split(new char[]{'\n'});
            int lenBuff = bufferSSL.Length;
            
            for (int i = start; i < lenBuff; i++)
            {
                string buffer = bufferSSL[i].TrimStart().ToLower();
                if (CommentBlockParse(ref buffer, ref _comm))
                    continue;
                RemoveCommentLine(ref buffer, 0);

                if (ifblock.begin == -1 && buffer.StartsWith("#if ")) {
                    ifblock.begin = i;
                    continue;
                } else if (ifblock.begin > -1 && buffer.StartsWith("#endif")) { 
                    ifblock.end = i;
                    list.Add(ifblock);
                    ifblock = new ProcBlock() { begin = -1, end = -1, copy = true};
                    continue;
                }

                if (block.begin == -1 && buffer.StartsWith(VARIABLE)) {
                    int boffset = buffer.IndexOf(" " + BEGIN) + 1;
                    if (boffset > 0) {
                        buffer = buffer.Remove(boffset + BEGIN.Length);
                        
                        int z = buffer.IndexOf("  ", 8);
                        if (z > 0)
                            buffer = RemoveDoubleWhiteSpaces(buffer, z, boffset);

                        if (buffer.StartsWith(VARIABLE + BEGIN)) {
                            block.begin = i;
                            continue;
                        }
                    }   
                }
                else if (block.begin > -1 && buffer.StartsWith(END) && (buffer.Length == 3 
                         || (buffer.Length > 3 && !TextUtilities.IsLetterDigitOrUnderscore(buffer[3])))) {
                    block.end = i;
                    list.Add(block);
                    block = new ProcBlock() { begin = -1, end = -1 };
                } 
                else if (block.begin > -1 && (buffer.StartsWith(VARIABLE) || buffer.StartsWith(PROCEDURE)))
                    block.begin = -1;
            }
            return list;
        }
        #endregion 

        #region Include files
        public static List<string> GetAllIncludes(TabInfo tab)
        {
            List<string> include = new List<string>();
            string[] lines = tab.textEditor.Document.TextContent.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
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
                    OverrideIncludePath(ref text[1], dir);
                    include.Add(text[1]);
                }
            }
            return include;
        }

        // Override includes path
        public static bool OverrideIncludePath(ref string iPath, string dir)
        {
            if (!Path.IsPathRooted(iPath)) {
                if (Settings.overrideIncludesPath && Settings.pathHeadersFiles != null) {
                    iPath = Path.GetFullPath(Path.Combine(Settings.pathHeadersFiles, iPath));
                    return true;
                } else { //если не указана папка c .h файлами то переопределять путь 
                    //относительно папки скрипта или каталога программы
                    string temp = Path.GetFullPath(Path.Combine(dir, iPath));
                    if (!File.Exists(temp))
                        iPath = Path.GetFullPath(Path.Combine(Settings.ProgramFolder, iPath));
                    else
                        iPath = temp; //source script dir
                    return true;
                }
            } // переопределить неотносительные пути в случае отсутвия такого .h файла
              // при переопрелелении пути в таком случае все .h файлы должны находится в одной папке
            else if (Settings.overrideIncludesPath && Settings.pathHeadersFiles != null) {
                if (!File.Exists(iPath)) {
                    iPath = Path.Combine(Settings.pathHeadersFiles, Path.GetFileName(iPath));
                    return true;
                }
            }
            return false;
        }

        public static bool OverrideIncludePath(ref string iPath)
        {
            if (Path.IsPathRooted(iPath) && !File.Exists(iPath)) {
                iPath = Path.Combine(Settings.pathHeadersFiles, Path.GetFileName(iPath));
                return true;
            }
            return false;
        }
        #endregion
    }
}
