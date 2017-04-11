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
    /// Class for parsing SSL code w/o external parser.dll [WIP]
    /// </summary>
    public class Parser
    {
        private static List<string> ProcNameList = new List<string>();

        const int PROC_LEN = 10; 
        const string PROCEDURE = "procedure ";
        const string BEGIN = "begin";
        const string END = "end";
        const string INCLUDE = "#include ";
        
        public static void InternalParser(TabInfo _ti)
        {
            UpdateParseSSL(_ti.textEditor.Text);
            ProgramInfo _pi = new ProgramInfo(countProcs(), 0);
            _ti.parseInfo = InternalProcParse(_pi, _ti.textEditor.Text, _ti.filepath);
        }

        // обновить данные о расположенных процедурах 
        public static ProgramInfo UpdateProcsPI(ProgramInfo _pi, string file, string filepath)
        {
            ProgramInfo update_pi = InternalProcParse(new ProgramInfo(countProcs(), 0), file, filepath);
            for (int i = 0; i < _pi.procs.Length; i++)
            {
                foreach (Procedure p in update_pi.procs)
                {
                    if (_pi.procs[i].name.ToLower() == p.name && _pi.procs[i].fdeclared == p.fdeclared) {
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

        // Заполнить данные о продедурах
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
                be_block = GetProcBeginEndBlock(_pi.procs[i].name);
                _pi.procs[i].d.start = be_block.begin + 1;
                _pi.procs[i].d.end = be_block.end + 1;
                _pi.procs[i].fdeclared = Path.GetFullPath(scriptFile);
                _pi.procs[i].fstart = _pi.procs[i].fdeclared;
                _pi.procs[i].filename = Path.GetFileName(scriptFile).ToLowerInvariant();
                _pi.procs[i].references = new Reference[0];  // empty not used
                _pi.procs[i].variables = new Variable[0];    // empty not used
                _pi.parsed = true;
            }
            return _pi;
        } 

        // Получить список всех процедур из скрипта
        private static void GetNamesProcedures()
        {
            int _comm = 0, m = 0;
            string[] NameProcedures = new string[0];
            string[] file = File.ReadAllLines(Compiler.parserPath);
            for (int i = 0; i < file.Length; i++)
            {
                file[i] = file[i].TrimStart().ToLower();
                if (CommentBlockParse(file[i], ref _comm)) continue;
                if (file[i].StartsWith(PROCEDURE))
                {
                    //извлеч имя процедуры
                    string s = file[i].Substring(PROC_LEN, file[i].Length - PROC_LEN);
                    // удалить Begin и другую информацию из имени процедуры
                    int z = s.IndexOf(";");
                    if (z > 0) s = s.Remove(z).TrimEnd();
                    z = s.IndexOf("(");
                    if (z > 0) s = s.Remove(z).TrimEnd();
                    z = s.IndexOf(BEGIN);
                    if (z > 0) s = s.Remove(z).TrimEnd();
                    z = s.IndexOf("//");
                    if (z < 0) z = s.IndexOf("/*");
                    if (z > 0) s = s.Remove(z).TrimEnd();
                    //
                    Array.Resize(ref NameProcedures, m + 1);
                    NameProcedures[m++] = s;
                }            
              }
            // Clear & Delеte duplicates 
            ProcNameList.Clear();
            for (int i = 0; i < NameProcedures.Length; i++)
            {
                bool _add = true;
                foreach (string a in ProcNameList)
                {
                    if (NameProcedures[i] == a) _add = false;
                }
                if (_add) ProcNameList.Add(NameProcedures[i]);
            }
        }

        // Получить номер строки с объявленой процедурой
        public static int GetDeclarationProcedureLine(string pName)
        {
            string[] file = File.ReadAllLines(Compiler.parserPath);
            pName = pName.ToLower();
            int pLen = pName.Length;
            for (int i = 0; i < file.Length; i++) {
                file[i] = file[i].TrimStart();
                // TODO: возможна тут нужна проверка на закоментированный блок /* */
                if (file[i].StartsWith(PROCEDURE + pName)) {
                    string s = file[i].Substring(PROC_LEN + pLen, 1);
                    if (s == ";" || s == "(") return i; //found
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
                if (CommentBlockParse(file[i], ref _comm)) continue;
                
                int z = file[i].IndexOf("//"); // убираем лишнее
                if (z < 0) z = file[i].IndexOf("/*");
                if (z > 0) file[i] = file[i].Remove(z).TrimEnd();
                
                if (file[i].EndsWith(BEGIN)) {
                    if (file[i].StartsWith(PROCEDURE) || file[--i].StartsWith(PROCEDURE)) {
                        for (int j = i - 1; j > 0; j--) {
                            if (file[j].StartsWith(PROCEDURE)) return j + 1;
                        }
                        return i - 1;  // if not found procedure declaration
                    }
                    else return -1;    // procedure block is broken
                }
            }
            return 0; // not found
        }

        // Получить для процедуры номер ее строки
        public static int GetPocedureLine(string pName, int startline = 0)
        {
            int lProc = -1, _proc = 0, _comm = 0; 
            string[] file = File.ReadAllLines(Compiler.parserPath);
            pName = pName.ToLower();
            int pLen = pName.Length;
            for (int i = startline; i < file.Length; i++)
            {
                file[i] = file[i].Trim();
                if (CommentBlockParse(file[i], ref _comm)) continue;
                // ищем начало процедуры с нашим именем
                if (_proc == 0 && file[i].StartsWith(PROCEDURE + pName))
                {   // нашли, имя 100% совпадает c искомым? 
                    string s = " ";
                    if (file[i].Length > PROC_LEN + pLen) s = file[i].Substring(PROC_LEN + pLen, 1);
                    if (s != " " && s != "(") continue; //нет это не совпадает, ищем дальше
                    _proc++; //совпадает, проверяем это процедура или ее объявление
                    
                    // убираем лишнее
                    int z = file[i].IndexOf("//", PROC_LEN + pLen);
                    if (z < 0) z = file[i].IndexOf("/*", PROC_LEN + pLen);
                    if (z > 0) file[i] = file[i].Remove(z).TrimEnd();

                    if (file[i].EndsWith(BEGIN)) return i;  // да это процедура
                    else {
                        lProc = i;
                        continue; //нет, продолжаем искать begin
                    }
                } // ищем begin от процедуры.
                else if (_proc > 0) {
                    if (file[i].StartsWith(BEGIN)) return lProc; // нашли begin, возвращаем номер строки процедуры
                    // в процессе проверяем и объявление процедур. 
                    if (file[i].StartsWith(PROCEDURE))
                    {   // если нашли - откат назад, будем продолжать искать нашу "procedure Name"
                        _proc--;
                        i--;
                        continue;
                    }
                }
            }
            return -1;
        }

        // Получить для заданной процедуры номера строк блока Begin...End
        public static ProcBlock GetProcBeginEndBlock(string pName, int startline = 0, bool onlybegin = false)
        {
            ProcBlock block = new ProcBlock();
            int _begin = 0, _proc = 0, _comm = 0 ; 
            string[] file = File.ReadAllLines(Compiler.parserPath);
            pName = pName.ToLower();
            int pLen = pName.Length;
            for (int i = startline; i < file.Length; i++)
            {
                file[i] = file[i].Trim();
                if (CommentBlockParse(file[i], ref _comm)) continue;
                // ищем начало процедуры с искомым именем
                if (_proc == 0 && file[i].StartsWith(PROCEDURE + pName))
                {   // нашли, имя 100% совпадает c искомым?
                    string s = " ";
                    if (file[i].Length > PROC_LEN + pLen) s = file[i].Substring(PROC_LEN + pLen, 1);
                    if (s != " " && s != "(") continue; //нет не совпадает, ищем дальше
                    _proc++; //совпадает, проверяем это процедура или ее объявление

                    // убираем лишнее
                    int z = file[i].IndexOf("//", PROC_LEN + pLen);
                    if (z < 0) z = file[i].IndexOf("/*", PROC_LEN + pLen);
                    if (z > 0) file[i] = file[i].Remove(z).TrimEnd();

                    if (file[i].EndsWith(BEGIN))
                    {   // да это процедура
                        _begin++;
                        block.begin = i;   // присваеваем значение в start
                        if (onlybegin) return block;
                        else continue;
                    }
                    else continue; //нет, продолжаем искать begin
                } // ищем begin от процедуры.
                else if (_proc > 0 && _begin == 0)
                {
                    if (file[i].StartsWith(BEGIN))
                    {   // нашли begin
                        _begin++;
                        block.begin = i;   // присваеваем значение в start
                        if (onlybegin) return block;
                        else continue;
                    }
                    // в процессе проверяем и объявление процедур. 
                    if (file[i].StartsWith(PROCEDURE))
                    {   // если нашли - откат назад, будем продолжать искать нашу "procedure Name"
                        _proc--;
                        i--;
                        continue;
                    }
                }
                // нашли begin, теперь ищем начало следующей процедуры
                // и от ее позиции будем искать END принадлежащий к искомой "procedure Name"
                if (_proc > 0 && _begin > 0 && file[i].StartsWith(PROCEDURE))
                { // нашли следующую процедуру
                    for (int j = i - 1; j > 0; j--) // back find 
                    {   // убираем лишнее
                        int z = file[i].IndexOf("//");
                        if (z < 0) z = file[i].IndexOf("/*");
                        if (z > 0) file[i] = file[i].Remove(z).TrimEnd();
                        if (file[j].StartsWith(END))
                        { // found "end"
                            block.end = j;
                            return block ; // return
                        } else if (j == 0) {
                            block.begin = -1;
                            block.end = -1;
                            return block; // procedure block is broken
                        } 
                    }
                } 
            }
            // для последней процедуры
            if (block.end == 0 && _proc > 0 && _begin > 0) {
                for (int i = file.Length - 1; i > 0; i--) // back find 
                {
                    if (file[i].StartsWith(END)) {
                        block.end = i;
                        return block;
                    }
                }
            }
            MessageBox.Show("When parsing the Begin...End block of the procedure " + pName + "\n an unexpected error occurred.", "Internal Parse Error");
            return block; // что-то пошло не так, достигнут конец файла
        }
        
        // Comment block parse
        private static bool CommentBlockParse(string sLine, ref int _comm)
        {
            if (sLine.StartsWith("//")) return true;
            if (sLine.StartsWith("/*") && _comm == 0) {
                if (sLine.IndexOf("*/") < 0) _comm++;
                return true;
            }
            else if (_comm > 0) {
                if (sLine.IndexOf("*/") > 0 || sLine.StartsWith("*/")) {
                    _comm--;
                }
                else return true;
            }
            return false;
        }

        public static void UpdateParseSSL(string sText)
        {
            File.WriteAllText(Compiler.parserPath, sText.ToLower());
        }

        public static bool CheckExistsProcedureName(string name)
        {
            if (GetDeclarationProcedureLine(name) != -1) {
                MessageBox.Show("This procedure has already declared.", "Info");
                return true;
            }
            return false;
        }

        public static string[] GetAllIncludes(string file) {
            int n = 0;
            string[] include = new string[0];
            string[] lines = File.ReadAllLines(file);
            string dir = Path.GetDirectoryName(file);
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim().ToLower();
                if (lines[i].StartsWith(INCLUDE)) {
                    string[] text = lines[i].Split('"');
                    if (text.Length < 2)
                        continue;
                    if (text[1].IndexOfAny(Path.GetInvalidPathChars()) != -1)
                        continue;
                    if (!Path.IsPathRooted(text[1])) {
                        text[1] = Path.Combine(dir, text[1]);
                    }
                    Array.Resize(ref include, n + 1);
                    include[n++] = text[1];
                }
            }
            return include;
        }
    }
}
