using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ScriptEditor.TextEditorUI;
using System.Windows.Forms;

namespace ScriptEditor.CodeTranslation
{
    /// <summary>
    /// Class for compiling and parsing SSL code. Interacts with SSLC compiler via command line (EXE version) and DLL imports.
    /// </summary>
    public class Compiler
    {
        private static readonly string decompilationPath = Path.Combine(Settings.scriptTempPath, "decomp.ssl");
        public static readonly string parserPath = Path.Combine(Settings.scriptTempPath, "parser.ssl");
        private static readonly string preprocessPath = Path.Combine(Settings.scriptTempPath, "preprocess.ssl");

        #region Imports from SSLC DLL

        [DllImport("resources\\parser.dll")]
        private static extern int parse_main(string file, string orig, string dir);
        [DllImport("resources\\parser.dll")]
        private static extern int numProcs();
        [DllImport("resources\\parser.dll")]
        private static extern void getProc(int proc, out ProcedureData data);
        [DllImport("resources\\parser.dll")]
        private static extern int getProcNamespaceSize(int proc);
        [DllImport("resources\\parser.dll")]
        private static extern void getProcNamespace(int proc, byte[] names);
        [DllImport("resources\\parser.dll")]
        private static extern int numVars();
        [DllImport("resources\\parser.dll")]
        private static extern void getVar(int var, out VariableData data);
        //[DllImport("resources\\parser.dll")]
        //private static extern int numExternals();
        //[DllImport("resources\\parser.dll")]
        //private static extern void getExternal(int var, out VariableData data);
        [DllImport("resources\\parser.dll")]
        private static extern void getProcVar(int proc, int var, out VariableData data);
        [DllImport("resources\\parser.dll")]
        private static extern int namespaceSize();
        [DllImport("resources\\parser.dll")]
        private static extern void getNamespace(byte[] names);
        [DllImport("resources\\parser.dll")]
        private static extern int stringspaceSize();
        [DllImport("resources\\parser.dll")]
        private static extern void getStringspace(byte[] names);
        [DllImport("resources\\parser.dll")]
        private static extern void getProcRefs(int proc, int[] refs);
        [DllImport("resources\\parser.dll")]
        private static extern void getVarRefs(int var, int[] refs);
        [DllImport("resources\\parser.dll")]
        private static extern void getProcVarRefs(int proc, int var, int[] refs);

        #endregion

        private int lastStatus = 1;

        private string indentFormat(string defmacro, int macrolen)
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
                        } else if (indent == -1 || macroline[i].Length == 0 ) continue;
                        try {
                            int adjust = macroline[i].Length - macroline[i].TrimStart().Length;
                            if (adjust > indent) adjust = indent;
                            else if (i == 1) indent = adjust;
                            macroline[i] = macroline[i].Remove(0, adjust); 
                        }
                        catch { Program.printLog("indentFormat. Line " + macroline[i] + "\r\nMacros: " + defmacro); }
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
                token = line.Remove(firstbracket).ToLowerInvariant();
                def = indentFormat(line.Substring(closebracket + 1), macro.Length);
            } else {
                macro = line.Remove(firstspace);
                token = macro.ToLowerInvariant();
                def = indentFormat(line.Substring(firstspace), macro.Length);
            }
            macros[token] = new Macro(macro, def, file, lineno + 1);
        }

        private void GetMacros(string file, string dir, SortedDictionary<string, Macro> macros)
        {
            if (!File.Exists(file))
                return;
            string[] lines = File.ReadAllLines(file, Encoding.Default);
            if (dir == null)
                dir = Path.GetDirectoryName(file);
            for (int i = 0; i < lines.Length; i++) {
                lines[i] = lines[i].Replace('\t', ' ').TrimStart();
                if (lines[i].StartsWith(Parser.INCLUDE)) {
                    string[] text = lines[i].Split('"');
                    if (text.Length < 2)
                        continue;
                    if (text[1].IndexOfAny(Path.GetInvalidPathChars()) != -1)
                        continue;
                    Parser.includePath(ref text[1], dir);
                    GetMacros(text[1], null, macros);
                } else if (lines[i].StartsWith(Parser.DEFINE)) {
                    if (lines[i].EndsWith("\\")) {
                        var sb = new StringBuilder();
                        int lineno = i;
                        lines[i] = lines[i].Substring(8);
                        do {
                            sb.Append(lines[i].Remove(lines[i].Length - 1).TrimEnd());
                            sb.Append(Environment.NewLine);
                            i++;
                        } while (lines[i].EndsWith("\\"));
                        sb.Append(lines[i]);
                        AddMacro(sb.ToString(), macros, file, lineno);
                    } else
                        AddMacro(lines[i].Substring(8), macros, file, i);
                }
            }
        }

        private static string ParseName(byte[] namelist, int name)
        {
            int strlen = (namelist[name - 5] << 8) + namelist[name - 6];
            return Encoding.ASCII.GetString(namelist, name - 4, strlen).TrimEnd('\0');
        }

        public void ParseOverrideIncludes(string text)
        {
            if (Settings.overrideIncludesPath && Settings.PathScriptsHFile != null) {
                string[] linetext = text.Split('\n');
                for (int i = 0; i < linetext.Length; i++)
                {
                    if (linetext[i].ToLower().TrimStart().StartsWith(Parser.INCLUDE)) {
                        string[] str = linetext[i].Split('"');
                        if (str.Length < 2)
                            continue;
                        if (str[1].IndexOfAny(Path.GetInvalidPathChars()) != -1)
                            continue;
                        Parser.includePath(ref str[1], parserPath);
                        linetext[i] = str[0] + '"' + str[1] + '"';
                    }
                }
                text = String.Join("\n",linetext);
            }
            File.WriteAllText(parserPath, text, Encoding.Default);
        }

        public ProgramInfo Parse(string text, string filepath, ProgramInfo prev_pi)
        {
            // Parse disabled, get only macros
            ParseOverrideIncludes(text);
            if (Settings.enableParser && filepath != null) {
                try {
                    lastStatus = parse_main(parserPath, filepath, Path.GetDirectoryName(filepath));
                } catch {
                    MessageBox.Show("An unexpected error occurred while parsing text of the script.\nIt is recommended that you save all unsaved documents and restart application,\nin order to avoid further incorrect operation of the application.", "Error: Parser.dll");
                    lastStatus = 2;
                };
            }
            ProgramInfo pi = (lastStatus >= 1)
                            ? new ProgramInfo(0, 0)
                            : new ProgramInfo(numProcs(), numVars());
            if (lastStatus >= 1 && prev_pi != null) { // preprocess error - store previous data Procs/Vars
                if (prev_pi.parseData) pi = Parser.UpdateProcsPI(prev_pi, text, filepath);
                else pi = prev_pi;
                pi.parsed = false;
                pi.macros.Clear();
            }
            if (Settings.overrideIncludesPath) File.WriteAllText(parserPath, text, Encoding.Default); // restore
            // Macros
            GetMacros(parserPath, Path.GetDirectoryName(filepath), pi.macros);
            if (lastStatus >= 1) return pi; // parse failed, return macros and previous data Procs/Vars
            //
            // Getting data of variables/procedures
            pi.parsed = true;
            pi.parseData = true; // flag - received data from parser.dll
            byte[] names = new byte[namespaceSize()];
            int stringsSize = stringspaceSize();
            getNamespace(names);
            byte[] strings = null;
            if (stringsSize > 0) {
                strings = new byte[stringsSize];
                getStringspace(strings);
            }
            //Variables
            for (int i = 0; i < pi.vars.Length; i++) {
                pi.vars[i] = new Variable();
                getVar(i, out pi.vars[i].d);
                pi.vars[i].name = ParseName(names, pi.vars[i].d.name);
                if (pi.vars[i].d.initialized != 0) {
                    switch (pi.vars[i].d.initialType) {
                        case ValueType.Int:
                            pi.vars[i].initialValue = pi.vars[i].d.intValue.ToString();
                            break;
                        case ValueType.Float:
                            pi.vars[i].initialValue = pi.vars[i].d.floatValue.ToString();
                            break;
                        case ValueType.String:
                            pi.vars[i].initialValue = '"' + ParseName(strings, pi.vars[i].d.intValue) + '"';
                            break;
                    }
                }
                if (pi.vars[i].d.fdeclared != IntPtr.Zero) {
                    pi.vars[i].fdeclared = Path.GetFullPath(Marshal.PtrToStringAnsi(pi.vars[i].d.fdeclared));
                    pi.vars[i].filename = Path.GetFileName(pi.vars[i].fdeclared).ToLowerInvariant();
                }
                if (pi.vars[i].d.numRefs == 0) {
                    pi.vars[i].references = new Reference[0];
                } else {
                    int[] tmp = new int[pi.vars[i].d.numRefs * 2];
                    getVarRefs(i, tmp);
                    pi.vars[i].references = new Reference[pi.vars[i].d.numRefs];
                    for (int j = 0; j < pi.vars[i].d.numRefs; j++)
                        pi.vars[i].references[j] = Reference.FromPtr(tmp[j * 2], tmp[j * 2 + 1]);
                }
            }
            //Procedures
            for (int i = 0; i < pi.procs.Length; i++) {
                pi.procs[i] = new Procedure();
                getProc(i, out pi.procs[i].d);
                pi.procs[i].name = ParseName(names, pi.procs[i].d.name);
                if (pi.procs[i].d.fdeclared != IntPtr.Zero) {
                    //pi.procs[i].fdeclared=Marshal.PtrToStringAnsi(pi.procs[i].d.fdeclared);
                    pi.procs[i].fdeclared = Path.GetFullPath(Marshal.PtrToStringAnsi(pi.procs[i].d.fdeclared));
                    pi.procs[i].filename = Path.GetFileName(pi.procs[i].fdeclared).ToLowerInvariant();
                }
                if (pi.procs[i].d.fstart != IntPtr.Zero) {
                    //pi.procs[i].fstart = Marshal.PtrToStringAnsi(pi.procs[i].d.fstart);
                    pi.procs[i].fstart = Path.GetFullPath(Marshal.PtrToStringAnsi(pi.procs[i].d.fstart));
                }
                //pi.procs[i].fend=Marshal.PtrToStringAnsi(pi.procs[i].d.fend);
                if (pi.procs[i].d.numRefs == 0) {
                    pi.procs[i].references = new Reference[0];
                } else {
                    int[] tmp = new int[pi.procs[i].d.numRefs * 2];
                    getProcRefs(i, tmp);
                    pi.procs[i].references = new Reference[pi.procs[i].d.numRefs];
                    for (int j = 0; j < pi.procs[i].d.numRefs; j++)
                        pi.procs[i].references[j] = Reference.FromPtr(tmp[j * 2], tmp[j * 2 + 1]);
                }
                //Procedure variables
                if (getProcNamespaceSize(i) == -1) {
                    pi.procs[i].variables = new Variable[0];
                } else {
                    byte[] procnames = new byte[getProcNamespaceSize(i)];
                    getProcNamespace(i, procnames);
                    pi.procs[i].variables = new Variable[pi.procs[i].d.numVariables];
                    for (int j = 0; j < pi.procs[i].variables.Length; j++) {
                        Variable var = pi.procs[i].variables[j] = new Variable();
                        getProcVar(i, j, out var.d);
                        var.name = ParseName(procnames, var.d.name);
                        if (var.d.initialized != 0) {
                            switch (var.d.initialType) {
                                case ValueType.Int:
                                    var.initialValue = var.d.intValue.ToString();
                                    break;
                                case ValueType.Float:
                                    var.initialValue = var.d.floatValue.ToString();
                                    break;
                                case ValueType.String:
                                    var.initialValue = '"' + ParseName(strings, var.d.intValue) + '"';
                                    break;
                            }
                        }
                        var.fdeclared = Marshal.PtrToStringAnsi(var.d.fdeclared);
                        if (var.d.numRefs == 0) {
                            var.references = new Reference[0];
                        } else {
                            int[] tmp = new int[var.d.numRefs * 2];
                            getProcVarRefs(i, j, tmp);
                            var.references = new Reference[var.d.numRefs];
                            for (int k = 0; k < var.d.numRefs; k++)
                                var.references[k] = Reference.FromPtr(tmp[k * 2], tmp[k * 2 + 1]);
                        }
                    }
                }
            }
            pi.BuildDictionaries();
            return pi;
        }

        public string OverrideIncludeSSLCompile(string file)
        { 
            if (Settings.overrideIncludesPath && Settings.PathScriptsHFile != null){
                string[] text = File.ReadAllLines(file);
                for (int i = 0; i < text.Length; i++)
                {
                    text[i] = text[i].TrimStart();
                    if (text[i].ToLower().StartsWith(Parser.INCLUDE)) {
                        string[] str = text[i].Split('"');
                        if (str.Length < 2)
                            continue;
                        if (str[1].IndexOfAny(Path.GetInvalidPathChars()) != -1)
                            continue;
                        Parser.includePath(ref str[1], file);
                        text[i]= str[0] + '"' + str[1] + '"';
                    }
                }
                string cfile = Settings.scriptTempPath + '\\' + Path.GetFileName(file);
                File.WriteAllLines(cfile, text);
                file = cfile;
            } 
            return file;
        }

        public static string GetPreprocessedFile(string sName)
        {
            if (!File.Exists(preprocessPath)) return null;
            sName = Path.Combine(Settings.scriptTempPath, Path.GetFileNameWithoutExtension(sName) + "_[preproc].ssl");
            File.Delete(sName);
            File.Move(preprocessPath, sName);
            return sName;   
        }

        public static string GetOutputPath(string infile, string sourceDir = "")
        { 
            string outputFile = Path.GetFileNameWithoutExtension(infile);
            if (sourceDir.Length != 0 && Settings.useWatcom) outputFile = outputFile.Remove(outputFile.Length - 6);
            outputFile = outputFile + ".int";
            if (Settings.ignoreCompPath && sourceDir.Length == 0) sourceDir = Path.GetDirectoryName(infile);
            return (Settings.ignoreCompPath) ? Path.Combine(sourceDir, outputFile) : Path.Combine(Settings.outputDir, outputFile);
        }

#if DLL_COMPILER
        public static string[] GetSslcCommandLine(string infile, bool preprocess) {
            return new string[] {
                "--", "-q",
                Settings.preprocess?"-P":"-p",
                Settings.optimize?"-O":"--",
                Settings.showWarnings?"--":"-n ",
                Settings.showDebug?"-d":"--",
                "-l", /* no logo */
                Path.GetFileName(infile),
                "-o",
                preprocess?preprocessPath:GetOutputPath(infile),
                null
            };
        }
#else
        private static string GetSslcCommandLine(string infile, bool preprocess, string sourceDir, bool shortCircuit)
        {
            string usePreprocess = string.Empty;
            if (!Settings.useWatcom) usePreprocess = preprocess ? "-P " : "-p ";
            return (usePreprocess)
                + ("-O" + Settings.optimize + " ")
                + (Settings.showWarnings ? "" : "-n ")
                + (Settings.showDebug ? "-d " : "")
                + ("-l ") /* always no logo */
                + ((Settings.shortCircuit || shortCircuit) ? "-s " : "")
                + "\"" + Path.GetFileName(infile) + "\" -o \"" + (preprocess ? preprocessPath : GetOutputPath(infile, sourceDir)) + "\"";
        }

        private static string GetCommandLine(string infile, string outfile) {
            return ("\"" + infile + "\" ..\\scrTemp\\" + outfile 
                 + ((Settings.preprocDef != "---") ? " /d" + Settings.preprocDef : string.Empty));
        }

        private static string GetCommandLine(string infile, bool shortCircuit) { 
            return ("\"" + infile + "\" /d"
                 + ((Settings.preprocDef != "---") ? Settings.preprocDef : string.Empty) 
                 + ((Settings.shortCircuit || shortCircuit) ? " -s" : string.Empty));
        }
#endif

#if DLL_COMPILER
        [System.Runtime.InteropServices.DllImport("resources\\sslc.dll")]
        private static extern int compile_main(int argc, string[] argv);

        [System.Runtime.InteropServices.DllImport("resources\\sslc.dll")]
        private static extern IntPtr FetchBuffer();
#endif

        public bool Compile(string infile, out string output, List<Error> errors, bool preprocessOnly, bool shortCircuit = false)
        {
            if (errors != null)
                errors.Clear();
            if (infile == null) {
                output = "No filename specified";
                return false;
            }
            bool success;
            infile = Path.GetFullPath(infile);
            string srcfile = infile;
            string sourceDir = Path.GetDirectoryName(infile);
            infile = OverrideIncludeSSLCompile(infile);
            output = "****** " + DateTime.Now.ToString("HH:mm:ss") + " ******\r\n";
            if (Settings.userCmdCompile) {
                string userbat = Path.Combine(Settings.ResourcesFolder, "usercomp.bat");
                ProcessStartInfo upsi = new ProcessStartInfo(userbat, GetCommandLine(infile, shortCircuit));
                upsi.RedirectStandardOutput = true;
                upsi.RedirectStandardError = true;
                upsi.UseShellExecute = false;
                upsi.CreateNoWindow = true;
                upsi.WorkingDirectory = Settings.ResourcesFolder;
                Process wp = Process.Start(upsi);
                output += wp.StandardOutput.ReadToEnd();
                wp.WaitForExit(1000);
                success = wp.ExitCode == 0;
                wp.Dispose();
            } else {
                if (Settings.useWatcom) {
                    string wccPath = Path.Combine(Settings.ResourcesFolder, "wcc.bat");
                    string outfile = "preprocess.ssl";
                    ProcessStartInfo wpsi = new ProcessStartInfo(wccPath, GetCommandLine(infile, outfile));
                    wpsi.RedirectStandardOutput = true;
                    wpsi.UseShellExecute = false;
                    wpsi.CreateNoWindow = true;
                    wpsi.WorkingDirectory = Settings.ResourcesFolder;
                    Process wp = Process.Start(wpsi);
                    output = wp.StandardOutput.ReadToEnd();
                    wp.WaitForExit(1000);
                    success = wp.ExitCode == 0;
                    wp.Dispose();
                    if (!success || preprocessOnly) return success;
                    infile = Path.Combine(Settings.scriptTempPath, Path.GetFileNameWithoutExtension(infile) + "_[wcc].ssl");
                    File.Delete(infile);
                    File.Move(Path.Combine(Settings.scriptTempPath, outfile), infile);
                    output += Environment.NewLine;
                }

#if DLL_COMPILER
                string origpath=Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(Path.GetDirectoryName(infile));
                string[] args=Settings.GetSslcCommandLine(infile, preprocessOnly);
                bool success=compile_main(args.Length, args)==0;
                output=System.Runtime.InteropServices.Marshal.PtrToStringAnsi(FetchBuffer());
                Directory.SetCurrentDirectory(origpath);
#else

                var exePath = Path.Combine(Settings.ResourcesFolder, "compile.exe");
                ProcessStartInfo psi = new ProcessStartInfo(exePath, GetSslcCommandLine(infile, preprocessOnly, sourceDir, shortCircuit));
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardInput = true;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.WorkingDirectory = Path.GetDirectoryName(infile);
                Process p = Process.Start(psi);
                p.StandardInput.WriteLine();
                output += p.StandardOutput.ReadToEnd();
                p.StandardInput.WriteLine();
                p.WaitForExit(1000);
                success = p.ExitCode == 0;
                p.Dispose();
#endif
            }
            if (errors != null && !Settings.userCmdCompile) 
                Error.BuildLog(errors, output, (Settings.useWatcom) ? infile : srcfile);
            if (Settings.overrideIncludesPath) File.Delete(Settings.scriptTempPath + '\\' + Path.GetFileName(srcfile));
#if DLL_COMPILER
            output=output.Replace("\n", "\r\n");
#endif
            return success;
        }

        public string Decompile(string infile)
        {
            string[] program = { "int2ssl.exe", "int2ssl_v35.exe" };
            foreach (string exe in program) 
            {
                var exePath = Path.Combine(Settings.ResourcesFolder, exe);
                ProcessStartInfo psi = new ProcessStartInfo(exePath, "\"" + infile + "\" \"" + decompilationPath + "\"");
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                Process p = Process.Start(psi);
                p.WaitForExit(3000);
                if (!p.HasExited)
                    return null;
                if (p.ExitCode == 0) break;
                p.Dispose();
            }
            if (!File.Exists(decompilationPath)) {
                return null;
            }
            SaveFileDialog sfDecomp = new SaveFileDialog();
            sfDecomp.Title = "Enter name to save decompile file";
            sfDecomp.Filter = "Script files|*.ssl";
            sfDecomp.RestoreDirectory = true;
            sfDecomp.InitialDirectory = Path.GetDirectoryName(infile);
            sfDecomp.FileName = Path.GetFileNameWithoutExtension(infile);
            string result;
            if (sfDecomp.ShowDialog() == DialogResult.OK)
                result = sfDecomp.FileName;
            else
                result = Path.Combine(Settings.scriptTempPath, Path.GetFileNameWithoutExtension(infile) + "_[decomp].ssl");
            sfDecomp.Dispose();
            File.Delete(result);
            File.Move(decompilationPath, result);
            return result;
        }
    }
}
