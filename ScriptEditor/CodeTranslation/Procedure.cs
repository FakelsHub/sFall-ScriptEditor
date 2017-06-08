using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEditor.CodeTranslation
{
    public class Procedure : IParserInfo
    {
        public string name;
        public string fdeclared;
        public string fstart;
        //public string fend; //just assume this is the same file as fstart
        public string filename;
        public ProcedureData d;
        public Variable[] variables;
        public Reference[] references;

        public NameType Type() { return NameType.Proc; }
        public Reference[] References() { return references; }
        public void Deceleration(out string file, out int line)
        {
            file = fdeclared;
            line = d.declared;
        }

        public string ToString(bool fullSpec = true)
        {
            string s = string.Empty;
            if (fullSpec) {
                if ((d.type & ProcType.Import) != ProcType.None)
                    s += "imported ";
                if ((d.type & ProcType.Export) != ProcType.None)
                    s += "exported ";
                if ((d.type & ProcType.Pure) != ProcType.None)
                    s += "pure ";
                if ((d.type & ProcType.Inline) != ProcType.None)
                    s += "inline ";
                if ((d.type & ProcType.Timed) != ProcType.None)
                    s += "timed ";
                if ((d.type & ProcType.Conditional) != ProcType.None)
                    s += "conditional ";
                if ((d.type & ProcType.Critical) != ProcType.None)
                    s += "critical ";
                s += "procedure ";
            }
            s += name;
            string args = "(";
            for (int i = 0; i < d.args; i++) {
                if (i > 0)
                    args += ", ";
                args += (i >= variables.Length) ? "x" : variables[i].name;
                if (fullSpec && i < variables.Length && variables[i].initialValue != null)
                    args += " := " + variables[i].initialValue;
            }
            args += ")";
            if (fullSpec || args.Length > 2) s += args;
            return s;
        }
        public bool IsImported()
        {
            return (d.type & ProcType.Import) > 0;
        }
        public override string ToString()
        {
            return ToString(true);
        }
    }
}
