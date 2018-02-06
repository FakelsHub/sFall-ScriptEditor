using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.TextEditor.Document;

namespace ScriptEditor.CodeTranslation
{    
    public class DialogueParser
    {
        private static readonly char[] trimming = new char[] { ' ', '(', ')' };
        private static readonly char[] digit = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        
        private static int nLine;
        private static ProgramInfo pi;

        public int numberMsgLine;       // msg number line
        public int numberMsgFile = -1;  // msg number file, -1 - если используется msg файл по умолчанию
        public string toNode;           // name goto node
        public string iq;               // player IQ
        public string shortcode;        // short code for test dialog
        public string code;             // code for diagram dialog
        public int codeNumLine;         // номер строки в коде ноды
        public OpcodeType opcode;

        public DialogueParser(OpcodeType opcode, string code, int offset = -1)
        {
            this.opcode = opcode;
            this.code = code;

            switch (opcode) {
                case OpcodeType.Reply:
                case OpcodeType.Message:
                case OpcodeType.gsay_reply:
                case OpcodeType.gsay_message:
                    ReplyOpcode(code, offset, opcode);
                    break;
                case OpcodeType.Option:
                case OpcodeType.gsay_option:
                case OpcodeType.giq_option:
                    OptionOpcode(offset, code, opcode);
                    break;
                case OpcodeType.call:
                    CallOpcode(code, offset);
                    break;
                default: //OpcodeType.None:
                    break;
            }
        }
        
        /*
         * gsay_message({int msg_list}, {int msg_num}, {int reaction});
         * gsay_reply({int msg_list}, {int msg_num});
         * Reply/Message
         */
        private void ReplyOpcode(string code, int offset, OpcodeType opcode)
        {
            this.codeNumLine = nLine;
            
            int x = code.IndexOf(";", offset) - 1;
            int sOffset = offset - opcode.ToString().Length;
            try {
                this.shortcode = code.Substring(sOffset, x + 1  - sOffset);
                code = code.Substring(++offset, x - offset);
            } catch {
                this.opcode = OpcodeType.None;
                return;
            }

            int z;
            if (opcode == OpcodeType.gsay_message) {
                z = code.LastIndexOf(',');
                code = code.Remove(z);
            }
            if (opcode == OpcodeType.gsay_reply || opcode == OpcodeType.gsay_message) {
                z = code.LastIndexOf(',');
                if (!int.TryParse(code.Remove(z++).Trim(trimming), out numberMsgFile)) // тут номер файла msg
                    numberMsgFile = -1; 
                code = code.Substring(z, code.Length - z); // тут номер строки
            }
            // msg line number parse
            if (!int.TryParse(code.Trim(trimming), out numberMsgLine))
                numberMsgLine = messageSubCode(code); //неудалось получить номер строки, распарсить доп код.

            if (opcode == OpcodeType.Reply || opcode == OpcodeType.gsay_reply) {
                toNode = "[Reply]";
                this.opcode = OpcodeType.Reply;
            } else {
                toNode = "[Message]";
                this.opcode = OpcodeType.Message;
            }
        }
            
        /*
         * gsay_option(msg_list, msg_num,  procedure, reaction);
         * giq_option (iq_test,  msg_list, msg_num,   procedure, reaction);
         * NOption    (msg_num, procedure, iq_test);
         * NLowOption (msg_num, procedure);
         */
        private void OptionOpcode(int offset, string code, OpcodeType opcode)
        {
            this.codeNumLine = nLine;

            //prepare code
            int z = code.IndexOf("(", offset) + 1;
            int x = code.IndexOf(";", z) - 1;
            try {                
                int sOffset = offset - opcode.ToString().Length;
                this.shortcode = code.Substring(sOffset, x + 1 - sOffset);
                code = code.Substring(z, x - z);
            } catch {
                this.opcode = OpcodeType.None;
                return;
            }

            z = code.LastIndexOf(',') + 1;
            switch (opcode) {
                case OpcodeType.gsay_option :
                case OpcodeType.giq_option :
                    // iq param parse
                    if (opcode == OpcodeType.giq_option) {
                        x = code.IndexOf(',');
                        iq = code.Remove(x).Trim(trimming);
                        code = code.Substring(++x); // удаляем IQ параметр
                        z -= x;
                    } else
                        iq = "-";
                    
                    // node param parse
                    code = code.Remove(z - 1); // удаляем Reaction параметр
                    z = code.LastIndexOf(',') + 1;
                    toNode = code.Substring(z, code.Length - z).Trim(trimming).TrimStart('@');
                    code = code.Remove(z - 1); // удаляем Node параметр

                    // msg file number parse
                    z = code.IndexOf(',');
                    string fileNum = code.Remove(z);
                    x = fileNum.IndexOf(',') + 1;
                    if (!int.TryParse(fileNum.Substring(x).Trim(trimming), out numberMsgFile)) {
                        numberMsgFile = -1;
                    } else
                        code = code.Substring(z + 1); // удаляем 
                    
                    // msg line number parse
                    string num = code;
                    z = num.IndexOf(',');
                    if (z > 0)
                        num = num.Remove(z).Trim(trimming);
                    if (!int.TryParse(num, out numberMsgLine))
                        numberMsgLine = messageSubCode(code); // распарсить доп код.
                    break;
                
                // for all macros
                case OpcodeType.Option :
                    // iq/node param parse
                    int result;
                    string str = code.Substring(z).Trim(trimming);
                    if (int.TryParse(str, out result)){
                        iq = result.ToString();
                        code = code.Remove(z - 1);
                    } else
                        toNode = str.TrimStart('@');

                    // node param parse
                    if (toNode == null) {
                        int y = code.LastIndexOf(',') + 1;
                        toNode = code.Substring(y, --z - y).Trim(trimming).TrimStart('@');
                        z = y;
                    }
                    code = code.Remove(z - 1); // удаляем Node параметр

                    // msg line number parse
                    if (!int.TryParse(code.Trim(trimming), out numberMsgLine))
                        numberMsgLine = messageSubCode(code); // распарсить доп код.
                    break;
            }
        }

        //for Call
        private void CallOpcode(string code, int offset)
        {
            int m = code.IndexOf(";", offset);
            if (m == -1)
                m = code.Length;
            int sOffset = offset - 4;
            this.shortcode = code.Substring(sOffset, m - sOffset);

            int x = code.IndexOf('(', offset);
            if (x > 0)
                m = x;
            try 
            { toNode = code.Substring(offset, m - offset).Trim(); }
            catch 
            { toNode = "<Failed get call node name>"; } 

            numberMsgLine = -1;
        }
                
        private int messageSubCode(string scode)
        {
            int result = -1;
            int macroValue = -1;
            string token = null;
            bool m_str = false;
            
            int i = scode.IndexOf('(');
            if (i != -1) {
                token = scode.Remove(i).Trim(); //.ToLower();

                switch (token.ToLower())
                {
                    case "message_str":
                        m_str = true;
                        break;
                    case "random":
                        int j = scode.IndexOf(',') - 1;
                        token = scode.Substring(i + 1, j - i).Trim(trimming);
                        return CheckMacrosValue(token);
                }
            }

            if (token != null && (m_str || pi.macros.ContainsKey(token))) { 
                string def = m_str ? scode : pi.macros[token].def; //message_str(NAME,x)
                int x = def.IndexOf("message_str", StringComparison.OrdinalIgnoreCase);
                int z = def.IndexOf('(');
                int y = def.IndexOf(',', z);
                    
                if (x != -1) { //Берем макрос номера файла сообщения
                    string argMsgNum = def.Substring(z + 1, y - z - 1).Trim(trimming); //.ToLower(); //macros NAME
                    macroValue = CheckMacrosValue(argMsgNum);
                    if (macroValue == -1)
                        return macroValue;
                }
                // возвращаем номер msg строки
                if (m_str)
                    z = scode.IndexOf(',');
                else
                    z = scode.IndexOf('(');
                if (z == -1)
                    return result;
                
                numberMsgFile = macroValue; // number msg file
                
                /* do // ишем первое вхожднение числа
                {
                    z++;
                    if (z == scode.Length)
                        return result;
                } while (!Char.IsDigit(scode[z])); */
                
                z++;
                if (!m_str)
                    y = scode.IndexOf(',', z);
                if (y == -1 || m_str)
                    y = scode.IndexOf(')', z);
                string argMsgLine = scode.Substring(z, y - z).Trim(trimming);
                if (!int.TryParse(argMsgLine, out result)) {
                    result = scode.LastIndexOf(')');
                    if (result > -1)
                        result = messageSubCode(scode.Substring(z, result - z));   
                }
            } else {
                // получить первое найденное число
                int d = scode.IndexOfAny(digit);
                if (d != -1)
                    result = CheckDigitValue(d, scode);
            }

            return result;              // return number msg line
        }

        private int CheckDigitValue(int d, string code)
        {
            int i;
            for (i = d; i < code.Length; i++) 
            {
                if (!Char.IsDigit(code[i]))
                    break;
            }

            code = code.Substring(d, i - d);
            return Convert.ToInt32(code);
        }


        private int CheckMacrosValue(string argToken)
        {
            int result = -1;
        loop:
            if (pi.macros.ContainsKey(argToken)) {
                pi.MacrosGetValue(ref argToken);
                if (!int.TryParse(argToken, out result)) // проверяем значение макроса
                    goto loop;
            } else { // такого макроса нет, проверяем на явно указаный номер
                if (!int.TryParse(argToken, out result))
                    result = -1; // выход, не удалось получить номер
            }
            return result;
        }

        public static void PrepareNodeCode(string nodeProcedureText, List<DialogueParser> args, ProgramInfo pi, bool excludeComment)
        {
            string[] preNodeBody = nodeProcedureText.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < preNodeBody.Length; i++)
            {
                preNodeBody[i] = preNodeBody[i].TrimEnd().Replace("\t", new string(' ', 3));

                int n = 0;
                do {
                    n = preNodeBody[i].IndexOf(";", n);
                    if (n > 0) {
                        if (++n >= preNodeBody[i].Length)
                            break; // выходим достигнут конец строки
                        
                        int z = preNodeBody[i].IndexOf("else", n);
                        if (excludeComment && z != -1)
                            z = (preNodeBody[i].IndexOf("//") != -1) ? -1 : z; //исключаем перенос для закоменнированных строк кода
                        if (z < 0)
                            break; // в строке нет ключевого слова 'else'
                        
                        int x;
                        for (x = 0; x < preNodeBody[i].Length; x++)
                        {
                            if (!Char.IsWhiteSpace(preNodeBody[i][x]))
                                break;
                        }

                        preNodeBody[i] = preNodeBody[i].Insert(z, Environment.NewLine + new string(' ', x));
                        n = z + 6;
                    }
                } while (n > -1);
            }

            ParseNodeCode(String.Join("\n", preNodeBody), args, pi, true, excludeComment);
        }

        public static void ParseNodeCode(string text, List<DialogueParser> args, ProgramInfo pi, bool diagram = false, 
                                         bool excludeComment = true, StringSplitOptions splitOption = StringSplitOptions.RemoveEmptyEntries)
        {
            DialogueParser.pi = pi;
            int _comm = 0, _count = 0;
            
            Regex regex = new Regex(@"\b" + OpcodeType.call.ToString() + @"\b", RegexOptions.IgnoreCase);
            string[] bodyNode = text.Split(new char[] {'\n'}, splitOption);

            for (int i = 0; i < bodyNode.Length; i++)
            {
                nLine = i;
                string str = bodyNode[i].TrimEnd();
                
                if (excludeComment || !diagram) {
                    string _str = str.TrimStart();
                    if (Parser.CommentBlockParse(ref _str, ref _comm))
                        continue;
                }
                if (excludeComment || diagram)
                    _count = args.Count;

                ReplySubParse(args, str, OpcodeType.Message);
                ReplySubParse(args, str, OpcodeType.Reply);
                OptionSubParse(args, str);
                
                // for call opcode
                MatchCollection matches = regex.Matches(str);
                foreach (Match m in matches)
                    args.Add(new DialogueParser(OpcodeType.call, str, m.Index + 4));

                // для диаграмм добавляем код из ноды
                if (diagram && _count == args.Count)
                    args.Add(new DialogueParser(OpcodeType.None, str));
            }
        }

        private static void ReplySubParse(List<DialogueParser> Args, string incode, OpcodeType _opcode)
        {
            int n = 0;
            do {
                OpcodeType opcode = _opcode;
                n = incode.IndexOf(opcode.ToString(), n, StringComparison.OrdinalIgnoreCase);
                if (n > -1) {
                    if (_opcode == OpcodeType.Message)
                        if (incode[n + 7].Equals('_')) //проверка на опкод message_str
                            break;
                    if (n > 4 && incode.Substring(n - 5, 5).Equals("gsay_", StringComparison.OrdinalIgnoreCase))
                        opcode = (opcode == OpcodeType.Reply) ? OpcodeType.gsay_reply : OpcodeType.gsay_message;

                    n += (_opcode == OpcodeType.Reply) ? 5 : 7;
                    if ((n + 2) < incode.Length) Args.Add(new DialogueParser(opcode, incode, n));
                };
            } while (n > -1); 
        }

        private static void OptionSubParse(List<DialogueParser> Args, string incode)
        {
            int n = 0;
            do {
                OpcodeType opcode = OpcodeType.Option;
                n = incode.IndexOf(opcode.ToString(), n, StringComparison.OrdinalIgnoreCase);
                if (n > -1) {
                    if (n > 3 && incode.Substring(n - 4, 4).Equals("giq_", StringComparison.OrdinalIgnoreCase))
                        opcode = OpcodeType.giq_option;
                    else if (n > 4 && incode.Substring(n - 5, 5).Equals("gsay_", StringComparison.OrdinalIgnoreCase))
                            opcode = OpcodeType.gsay_option;

                    n += 6;
                    Args.Add(new DialogueParser(opcode, incode, n));
                };
            } while (n > -1); 
        }

        public static List<string> GetAllNodesName(Procedure[] procedures)
        {
            List<string> nodesName = new List<string>();
            foreach (var p in procedures)
            {
                if ((p.name.IndexOf("node", StringComparison.OrdinalIgnoreCase) > -1)
                    || p.name.Equals("talk_p_proc", StringComparison.OrdinalIgnoreCase))
                    nodesName.Add(p.name);
            }
            return nodesName;
        }
    }
}
