using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEditor.CodeTranslation
{
    public class Macro : IParserInfo
    {
        public readonly string name;
        public readonly string def;
        public readonly int declared;
        public readonly string fdeclared;

        public NameType Type() { return NameType.Macro; }
        public Reference[] References() { return null; }
        public void Deceleration(out string file, out int line)
        {
            file = fdeclared;
            line = declared;
        }

        public Macro(string name, string def, string file, int line)
        {
            this.name = name;
            this.def = def;
            this.fdeclared = file;
            this.declared = line;
        }

        public override string ToString()
        {
            string declare = fdeclared.Remove(0, fdeclared.LastIndexOf('\\') + 1);
            if (declare == "parser.ssl")
                declare = string.Empty;
            else
                declare = "\n\nDeclare file: " + declare;
            return "Define: " + name + "\n" + def + declare;
        }
    }
}
