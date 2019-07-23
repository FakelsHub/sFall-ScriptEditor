using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScriptEditor.CodeTranslation
{
    internal class GetMacros
    {
        public static string[] GetHeadersFiles(string dirHeaders)
        {
            if (dirHeaders == null || !Directory.Exists(dirHeaders)) {
                Program.printLog("   <GetHeaders> Directory of headers not found: '" + (dirHeaders ?? "<unset>") + "'");
                return null;
            }
            return Directory.GetFiles(dirHeaders, "*.h", SearchOption.AllDirectories);
        }

        public static void GetGlobalMacros(string dirHeaders)
        {
            string[] headerFiles = GetHeadersFiles(dirHeaders);
            if (headerFiles == null) return;

            ProgramInfo.macrosGlobal.Clear();
            foreach (string file in headerFiles)
            {
                new GetMacros(File.ReadAllLines(file, (Settings.saveScriptUTF8)
                                                       ? Encoding.UTF8 
                                                       : Encoding.Default),
                                                       file, "", ProgramInfo.macrosGlobal, false);
            }
        }

        public GetMacros(string file, string dir, SortedDictionary<string, Macro> macros)
        { 
            if (!File.Exists(file)) {
                Program.printLog("   <GetMacros> File not found: '" + file + "'");
                return;
            }
            new GetMacros(File.ReadAllLines(file, (Settings.saveScriptUTF8)
                                                   ? Encoding.UTF8 
                                                   : Encoding.Default),
                                                   file, dir, macros);
        }

        public GetMacros(string[] lines, string file, string dir, SortedDictionary<string, Macro> macros, bool include = true)
        {
            if (dir == null)
                dir = Path.GetDirectoryName(file);
            for (int i = 0; i < lines.Length; i++) {
                lines[i] = lines[i].Replace('\t', ' ').TrimStart();
                if (include && lines[i].StartsWith(ParserInternal.INCLUDE)) {
                    string[] text = lines[i].Split('"');
                    if (text.Length < 2)
                        continue;
                    if (text[1].IndexOfAny(Path.GetInvalidPathChars()) != -1)
                        continue;
                    ParserInternal.OverrideIncludePath(ref text[1], dir);
                    new GetMacros(text[1], null, macros);
                } else if (lines[i].StartsWith(ParserInternal.DEFINE)) {
                    if (lines[i].TrimEnd().EndsWith(@"\")) {
                        var sb = new StringBuilder();
                        int lineno = i;
                        lines[i] = lines[i].Substring(8);
                        do {
                            sb.Append(lines[i].Remove(lines[i].Length - 1).TrimEnd());
                            sb.Append(Environment.NewLine);
                            i++;
                        } while (lines[i].TrimEnd().EndsWith(@"\"));
                        sb.Append(lines[i]);
                        AddMacro(sb.ToString(), macros, file, lineno);
                    } else
                        AddMacro(lines[i].Substring(8), macros, file, i);
                }
            }
        }
   
        private void AddMacro(string line, SortedDictionary<string, Macro> macros, string file, int lineno)
        {
            string token, macro, def;
            line = line.Trim();
            int firstspace = line.IndexOf(' ');
            if (firstspace == -1)
                return;
            int firstbracket = line.IndexOf('(');
            if (firstbracket != -1 && firstbracket < firstspace) {
                int closebracket = line.IndexOf(')');
                if (line.Length == closebracket + 1)
                    return; //second check, because spaces are allowed between macro arguments
                macro = line.Remove(closebracket + 1);
                token = line.Remove(firstbracket); //.ToLowerInvariant(); //макросы записываются в том регистре в котором они объявлены
                def = MacroFormat(line.Substring(closebracket + 1), macro.Length);
            } else {
                macro = line.Remove(firstspace);
                token = macro; //.ToLowerInvariant();
                def = MacroFormat(line.Substring(firstspace), macro.Length);
            }
            macros[token] = new Macro(macro, def, file, lineno + 1);
        }

        private string MacroFormat(string defmacro, int macrolen)
        { 
            string[] macroline = defmacro.Split('\n');
            if (macroline.Length > 1) {
                int indent = -1;
                macroline[0] = macroline[0].TrimEnd();
                if (macroline[0].Length > 1) {
                    indent = (8 + macrolen) + macroline[0].Length - macroline[0].TrimStart().Length;
                }
                for (int i = 1; i < macroline.Length; i++)
                {
                    macroline[i] = macroline[i].TrimEnd();
                    if (indent == -1 && macroline[i].Length > 1) {
                        indent = macroline[i].Length - macroline[i].TrimStart().Length;
                    } else if (indent == -1 || macroline[i].Length == 0 )
                                continue;
                    try {
                        int adjust = macroline[i].Length - macroline[i].TrimStart().Length;
                        if (adjust > indent) adjust = indent;
                        else if (i == 1) indent = adjust;
                        macroline[i] = macroline[i].Remove(0, adjust); 
                    }
                    catch { Program.printLog("   <MacroFormat> Exception in line " + macroline[i] + " | Macros: " + defmacro); }
                    if (i > 40 && macroline.Length > 42) { // tip text size
                        macroline[i++] = " continue...";
                        Array.Resize(ref macroline, i);
                        break;
                    }
                }
                return String.Join("\n", macroline).TrimStart();
            }
            return defmacro.TrimStart();
        }
    }
}
