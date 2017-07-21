using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEditor.CodeTranslation
{
    public enum OpcodeType
    { 
        Option, 
        giq_option, 
        gsay_option,
 
        Reply,
        gsay_reply,
 
        Message,
        gsay_message,

        call,
    } 
    
    class DialogueParser
    {
        //private static readonly char[] digit = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' }; 
        private static readonly char[] trimming = new char[] { ' ', '(', ')' };

        public int msgNum;
        public string toNode;
        public string iq;
        public string code;

        /*
         * gsay_message({int msg_list}, {int msg_num}, {int reaction});
         * gsay_reply({int msg_list}, {int msg_num});
         * Reply/Message
         */
        public DialogueParser(string param, int offset, OpcodeType opcode)
        {
            int x = param.IndexOf(";", offset) - 1;
            param = param.Substring(++offset, x - offset);
            code = "<..." + param + "...>";
            int z;
            if (opcode == OpcodeType.gsay_message) {
                z = param.LastIndexOf(',');
                param = param.Remove(z);
            }
            if (opcode == OpcodeType.gsay_reply || opcode == OpcodeType.gsay_message) {
                z = param.LastIndexOf(',') + 1;
                param = param.Substring(z, param.Length - z);
            }
            if (!int.TryParse(param.Trim(trimming), out msgNum))
                msgNum = -1;

            toNode = (opcode == OpcodeType.Reply || opcode == OpcodeType.gsay_reply) ? "[Reply]" : "[Message]";
        }
            
        /*
         * gsay_option(msg_list, msg_num,  procedure, reaction);
         * giq_option (iq_test,  msg_list, msg_num,   procedure, reaction);
         * NOption    (msg_num, procedure, iq_test);
         * NLowOption (msg_num, procedure);
         */
        public DialogueParser(string param, OpcodeType opcode)
        {
            int z = param.LastIndexOf(',') + 1;
            switch (opcode) {
                case OpcodeType.gsay_option :
                case OpcodeType.giq_option :
                    // node param parse
                    code = param.Remove(z - 1); 
                    z = code.LastIndexOf(',') + 1;
                    toNode = code.Substring(z, code.Length - z).Trim(trimming).TrimStart('@');

                    // msg number parse
                    code = code.Remove(z - 1); 
                    z = code.LastIndexOf(',') + 1;
                    code = code.Substring(z, code.Length - z).Trim(trimming);
                    if (!int.TryParse(code, out msgNum)) {
                        msgNum = -1;
                        code = "<..." + code + "...>";
                    }

                    // iq param parse
                    if (opcode == OpcodeType.giq_option) {
                        z = param.IndexOf(',');
                        iq = param.Remove(z).Trim(trimming);
                    } else
                        iq = "-";
                    break;
                
                // for all macros
                case OpcodeType.Option :
                    // iq/node param parse
                    int result;
                    string str = param.Substring(z, param.Length - z).Trim(trimming);
                    if (int.TryParse(str, out result))
                        iq = result.ToString();
                    else
                        toNode = str.TrimStart('@');

                    // node param parse
                    int y = param.IndexOf(',') + 1;
                    if (toNode == null)
                        toNode = param.Substring(y, --z - y).Trim(trimming).TrimStart('@');

                    // msg number parse
                    str = param.Remove(y).Trim(trimming);
                    for (int i = 0; i < str.Length; i++) {
                        if (!Char.IsDigit(Convert.ToChar(str.Substring(i, 1)))) {
                            str = str.Substring(0, i);
                            break;
                        }
                    }
                    if (!int.TryParse(str, out msgNum)) {
                        msgNum = -1;
                        code = "<" + param.Remove(y - 1) + "...>";
                    }
                    break;
            }

        }

        //for Call
        public DialogueParser(string param, int offset)
        {
            try
            {
                int m = param.IndexOf(";", offset);
                toNode = param.Substring(offset, m - offset).Trim();
            }
            catch {}
            code = "<" + param + ">";
        }

        public static void ReplySubParse(List<DialogueParser> Args, string incode, OpcodeType _opcode)
        {
            int n = 0;
            do {
                OpcodeType opcode = _opcode;
                n = incode.IndexOf(opcode.ToString(), n, StringComparison.OrdinalIgnoreCase);
                
                if (n > -1) {
                    if (n > 4 && incode.Substring(n - 5, 5).Equals("gsay_", StringComparison.OrdinalIgnoreCase))
                        opcode = (opcode == OpcodeType.Reply) ? OpcodeType.gsay_reply : OpcodeType.gsay_message;
                    
                    n += (_opcode == OpcodeType.Reply) ? 5 : 7;
                    Args.Add(new DialogueParser(incode, n, opcode));
                };
            } while (n > -1); 
        }

        public static void OptionSubParse(List<DialogueParser> Args, string incode)
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
                    int y = incode.IndexOf("(", n) + 1;
                    int x = incode.IndexOf(";", y) - 1;
                    Args.Add(new DialogueParser(incode.Substring(y, x - y), opcode));
                };
            } while (n > -1); 
        }
    }
}
