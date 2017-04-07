using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor.CodeTranslation
{
    public struct ProcBlock
    {
        public int begin;
        public int end;
        public bool copy;
    }
    
    public class Parser
    {
        const int PROC_LEN = 10; 
        const string PROCEDURE = "procedure ";
        const string BEGIN = "begin";
        const string END = "end";
        
        public static void UpdateParseSSL(string sText) {
            File.WriteAllText(Compiler.parserPath, sText.ToLower());
            //return File.ReadAllLines(Compiler.parserPath);
        }

        // проверить объявлена ли процедура, и вернуть номер строки объявления
        public static int CheckDeclarationProcedure(string name)
        {
            string[] file = File.ReadAllLines(Compiler.parserPath);
            for (int i = 0; i < file.Length; i++) {
                file[i] = file[i].TrimStart();
                // TODO: нужна проверка на закоментированный блок /* */
                if (file[i].StartsWith(PROCEDURE + name.ToLower())){
                    string s = file[i].Substring(PROC_LEN + name.Length, 1);
                    if (s == ";" || s == "(") return i; //found
                }
            }
            return -1; // not found
        }

        // найти строку конца списка 'procedure declaration'
        public static int GetEndLineProcDeclaration()
        {
            string[] file = File.ReadAllLines(Compiler.parserPath);
            for (int i = 0; i < file.Length; i++) 
            {
                file[i] = file[i].Trim();
                if (file[i].StartsWith("//")) continue;
                // TODO: нужна проверка на закоментированный блок /* */
                int z = file[i].IndexOf("//"); // убираем лишнее
                if (z == 0) file[i].IndexOf("/*");
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

        // Получить для заданной процедуры номер ее строки
        public static int GetLinePpocedureBegin(string pName, int startline = 0)
        {
            int lProc = -1, _proc = 0, _comm = 0; 
            string[] file = File.ReadAllLines(Compiler.parserPath);
            for (int i = startline; i < file.Length; i++)
            {
                file[i] = file[i].Trim();
                if (file[i].StartsWith("//")) continue;
                if (file[i].StartsWith("/*") && _comm == 0) {
                    if ( file[i].IndexOf("*/") < 0) _comm++;
                    continue;
                } else if (_comm > 0) {
                    if (file[i].IndexOf("*/") > 0 || file[i].StartsWith("*/")) {
                        _comm--;
                    } else continue;
                }
                // ищем начало процедуры с нашим именем
                if (_proc == 0 && file[i].StartsWith(PROCEDURE + pName.ToLower()))
                {   // нашли, имя 100% совпадает c искомым? 
                    string s = " ";
                    if (file[i].Length > PROC_LEN + pName.Length) s = file[i].Substring(PROC_LEN + pName.Length, 1);
                    if (s != " " && s != "(") continue; //нет это не совпадает, ищем дальше
                    _proc++; //совпадает, проверяем это процедура или ее объявление
                    
                    // убираем лишнее
                    int z = file[i].IndexOf("//", PROC_LEN + pName.Length);
                    if (z == 0) file[i].IndexOf("/*", PROC_LEN + pName.Length);
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

        // Получить для заданной процедуры номера строк с ее блоком Begin...End, или же только для Begin
        public static ProcBlock GetProcBeginEndBlock(string pName, int startline = 0, bool onlybegin = false)
        {
            ProcBlock block = new ProcBlock();
            int _begin = 0, _proc = 0, _comm = 0 ; 
            string[] file = File.ReadAllLines(Compiler.parserPath);
            for (int i = startline; i < file.Length; i++)
            {
                file[i] = file[i].Trim();
                if (file[i].StartsWith("//")) continue;
                if (file[i].StartsWith("/*") && _comm == 0) {
                    if (file[i].IndexOf("*/") < 0) _comm++;
                    _comm++;
                    continue;
                } else if (_comm > 0) {
                    if (file[i].IndexOf("*/") > 0 || file[i].StartsWith("*/")) {
                    _comm--;
                    } else continue;  
                }
                // ищем начало процедуры с искомым именем
                if (_proc == 0 && file[i].StartsWith(PROCEDURE + pName.ToLower()))
                {   // нашли, имя 100% совпадает c искомым?
                    string s = " ";
                    if (file[i].Length > PROC_LEN + pName.Length) s = file[i].Substring(PROC_LEN + pName.Length, 1);
                    if (s != " " && s != "(") continue; //нет не совпадает, ищем дальше
                    _proc++; //совпадает, проверяем это процедура или ее объявление

                    // убираем лишнее
                    int z = file[i].IndexOf("//", PROC_LEN + pName.Length);
                    if (z == -1) file[i].IndexOf("/*", PROC_LEN + pName.Length);
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
                        if (z == 0) file[i].IndexOf("/*");
                        if (z > 0) file[i] = file[i].Remove(z).TrimEnd();
                        if (file[j].StartsWith(END) && file[j].Length <= 3)
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
            return block; 
        }
        
        public static bool CheckExistsProcedureName(string name)
        {
            if (CheckDeclarationProcedure(name) != -1) {
                MessageBox.Show("This procedure has already declared.", "Info");
                return true;
            }
            return false;
        }
    }
}
