﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor.CodeTranslation
{
    /// <summary>
    /// Class for parsing SSL code. Interacts with SSLC compiler via DLL imports.
    /// </summary>
    internal class ParserDLL
    {
        #region Imports from SSLC DLL

        [DllImport("resources\\parser.dll")]
        private static extern int parse_main(string file, string orig, string dir, string def);
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

        private static readonly string parserPath = Path.Combine(Settings.scriptTempPath, "parser.ssl");

        private bool firstParse;
        private int lastStatus = 1;
        
        public int LastStatus 
        {
            get { return lastStatus; }
        }

        public ParserDLL(bool firstPass)
        {
            this.firstParse = firstPass;
        }
        
        public ProgramInfo Parse(string text, string filepath, ProgramInfo prev_pi)
        {
            // Parse disabled, get only macros
            if (Settings.enableParser && filepath != null) {
                ParseOverrideIncludes(text);
                string includePath = (Settings.overrideIncludesPath) ? Settings.pathHeadersFiles : null;
                try {
                    lastStatus = parse_main(parserPath, filepath, (includePath ?? Path.GetDirectoryName(filepath)), 
                                            Settings.preprocDef ?? String.Empty);
                } catch {
                    lastStatus = 3;
                    MessageBox.Show("An unexpected error occurred while parsing text of the script.\n" +
                                    "It is recommended that you save all unsaved documents and restart application,\n" +
                                    "in order to avoid further incorrect operation of the application.", "Error: Parser.dll");
                };
            }

            ProgramInfo pi = (lastStatus >= 1)
                              ? prev_pi //new ProgramInfo(0, 0)
                              : new ProgramInfo(numProcs(), numVars());
            if (lastStatus >= 1 && prev_pi != null) { // preprocess error - store previous data Procs/Vars
                if (prev_pi.parseData)
                    pi = Parser.UpdateProcsPI(prev_pi, text, filepath);
                else {
                    if (firstParse)
                        pi.RebuildProcedureDictionary();
                    //pi = prev_pi;
                    //pi.parsed = false;
                }
                pi.macros.Clear();
            }

            pi.parseError = (lastStatus != 0 & Settings.enableParser);
            // Macros
            new GetMacros(text.Split('\n'), filepath, Path.GetDirectoryName(filepath), pi.macros);
            if (lastStatus >= 1)
                return pi; // parse failed, return macros and previous parsed data Procs/Vars
            //
            // Getting data of variables/procedures
            //
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

        private string ParseName(byte[] namelist, int name)
        {
            int strlen = (namelist[name - 5] << 8) + namelist[name - 6];
            return Encoding.ASCII.GetString(namelist, name - 4, strlen).TrimEnd('\0');
        }

        private void ParseOverrideIncludes(string text)
        {
            /*  
             *  Пререопределение include путей для парсера более не требутся, 
             *  т.к. путь для поиска include файлов указывается непосредственно парсеру через аргумент строки.
             *  возможность оставленна только для переопределения неотносительных include путей.
             */
            if (Settings.overrideIncludesPath && Settings.pathHeadersFiles != null) {
                string[] linetext = text.Split('\n');
                for (int i = 0; i < linetext.Length; i++)
                {
                    if (linetext[i].ToLower().TrimStart().StartsWith(Parser.INCLUDE)) {
                        string[] str = linetext[i].Split('"');
                        if (str.Length < 2)
                            continue;
                        if (str[1].IndexOfAny(Path.GetInvalidPathChars()) != -1)
                            continue;

                        if (Parser.OverrideIncludePath(ref str[1]))
                            linetext[i] = str[0] + '"' + str[1] + '"';
                    }
                }
                text = String.Join("\n", linetext);
            }
            File.WriteAllText(parserPath, text, Encoding.Default);
        }
    }
}