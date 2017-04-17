﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor
{
    public enum SavedWindows { Main, Count }

    public static class Settings
    {
        public static readonly string ProgramFolder = Path.GetDirectoryName(Application.ExecutablePath);
        public static readonly string SettingsFolder = Path.Combine(ProgramFolder, "settings");
        public static readonly string ResourcesFolder = Path.Combine(ProgramFolder, "resources");

        private static readonly string SettingsPath = Path.Combine(SettingsFolder, "settings.dat");
        private static readonly string PreprocessPath = Path.Combine(SettingsFolder, "preprocess.ssl");
        public static readonly string SearchHistoryPath = Path.Combine(SettingsFolder, "SearchHistory.ini");

        const int MAX_RECENT = 30;

        public static byte optimize = 1;
        public static bool showWarnings = true;
        public static bool showDebug = true;
        public static bool overrideIncludesPath = true;
        public static bool warnOnFailedCompile = true;
        public static bool multiThreaded = true;
        public static bool autoOpenMsgs = true;
        public static string outputDir;
        public static string PathScriptsHFile;
        public static string lastMassCompile;
        public static string lastSearchPath;
        private static readonly List<string> recent = new List<string>();
        private static readonly WindowPos[] windowPositions = new WindowPos[(int)SavedWindows.Count];
        public static int editorSplitterPosition = -1;
        public static int editorSplitterPosition2 = -1;
        public static string language = "english";
        public static bool tabsToSpaces = true;
        public static int tabSize = 3;
        public static bool enableParser = true;
        public static bool shortCircuit = false;
        public static bool autocomplete = true;
        public static bool showLog = true;
        public static byte hintsLang = 0;
        public static byte highlight = 0;
        public static byte encoding = 0; // 0 = DEFAULT, 1 = DOS(cp866)
        public static bool allowDefine = true;

        public static void SetupWindowPosition(SavedWindows window, System.Windows.Forms.Form f)
        {
            WindowPos wp = windowPositions[(int)window];
            if (wp.width == 0)
                return;
            f.Location = new System.Drawing.Point(wp.x, wp.y);
            if (wp.maximized)
                f.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            else
                f.ClientSize = new System.Drawing.Size(wp.width, wp.height);
            f.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        }

        public static void SaveWindowPosition(SavedWindows window, System.Windows.Forms.Form f)
        {
            WindowPos wp = new WindowPos();
            wp.maximized = f.WindowState == System.Windows.Forms.FormWindowState.Maximized;
            wp.x = f.Location.X;
            wp.y = f.Location.Y;
            wp.width = f.ClientSize.Width;
            wp.height = f.ClientSize.Height;
            windowPositions[(int)window] = wp;
        }

        public static void AddRecentFile(string s, bool b = false)
        {
            for (int i = 0; i < recent.Count; i++) {
                if (string.Compare(recent[i], s, true) == 0)
                    recent.RemoveAt(i--);
            }
            if (!b && recent.Count >= MAX_RECENT)
                recent.RemoveAt(0);
            if (!b) recent.Add(s);
        }

        public static string[] GetRecent() { return recent.ToArray(); }

        private static void LoadWindowPos(BinaryReader br, int i)
        {
            windowPositions[i].maximized = br.ReadBoolean();
            windowPositions[i].x = br.ReadInt32();
            windowPositions[i].y = br.ReadInt32();
            windowPositions[i].width = br.ReadInt32();
            windowPositions[i].height = br.ReadInt32();
        }

        private static void LoadInternal(BinaryReader br)
        {
            try {
                allowDefine = br.ReadBoolean();
                hintsLang = br.ReadByte();
                highlight = br.ReadByte();
                encoding = br.ReadByte();
                optimize = br.ReadByte();
                showWarnings = br.ReadBoolean();
                showDebug = br.ReadBoolean();
                overrideIncludesPath = br.ReadBoolean();
                outputDir = br.ReadString();
                if (outputDir.Length == 0)
                    outputDir = null;
                int recentItems = br.ReadByte();
                for (int i = 0; i < recentItems; i++)
                    recent.Add(br.ReadString());
                warnOnFailedCompile = br.ReadBoolean();
                multiThreaded = br.ReadBoolean();
                lastMassCompile = br.ReadString();
                if (lastMassCompile.Length == 0)
                    lastMassCompile = null;
                lastSearchPath = br.ReadString();
                if (lastSearchPath.Length == 0)
                    lastSearchPath = null;
                LoadWindowPos(br, 0);
                editorSplitterPosition = br.ReadInt32(); // reserved
                autoOpenMsgs = br.ReadBoolean();
                editorSplitterPosition2 = br.ReadInt32();
                PathScriptsHFile = br.ReadString();
                if (PathScriptsHFile.Length == 0)
                    PathScriptsHFile = null;
                language = br.ReadString();
                if (language.Length == 0)
                    language = "english";
                tabsToSpaces = br.ReadBoolean();
                tabSize = br.ReadInt32();
                enableParser = br.ReadBoolean();
                shortCircuit = br.ReadBoolean();
                autocomplete = br.ReadBoolean();
                showLog = br.ReadBoolean();
            } catch {
                return;
            }
        }

        public static void Load()
        {
            if (!Directory.Exists(SettingsFolder)) {
                Directory.CreateDirectory(SettingsFolder);
            }
            if (!Directory.Exists(ResourcesFolder)) {
                Directory.CreateDirectory(ResourcesFolder);
            }
            var templatesFolder = Path.Combine(ResourcesFolder, "templates");
            if (!Directory.Exists(templatesFolder)) {
                Directory.CreateDirectory(templatesFolder);
            }
            if (File.Exists(SettingsPath)) {
                recent.Clear();
                BinaryReader br = new BinaryReader(File.OpenRead(SettingsPath));
                LoadInternal(br);
                br.Close();
            }
        }

        private static void WriteWindowPos(BinaryWriter bw, int i)
        {
            bw.Write(windowPositions[i].maximized);
            bw.Write(windowPositions[i].x);
            bw.Write(windowPositions[i].y);
            bw.Write(windowPositions[i].width);
            bw.Write(windowPositions[i].height);
        }

        public static void Save()
        {
            if (!Directory.Exists(SettingsFolder))
                Directory.CreateDirectory(SettingsFolder);
            BinaryWriter bw = new BinaryWriter(File.Create(SettingsPath));
            bw.Write(allowDefine);
            bw.Write(hintsLang);
            bw.Write(highlight);
            bw.Write(encoding);
            bw.Write(optimize);
            bw.Write(showWarnings);
            bw.Write(showDebug);
            bw.Write(overrideIncludesPath);
            bw.Write(outputDir == null ? "" : outputDir);
            bw.Write((byte)recent.Count);
            for (int i = 0; i < recent.Count; i++)
                bw.Write(recent[i]);
            bw.Write(warnOnFailedCompile);
            bw.Write(multiThreaded);
            bw.Write(lastMassCompile == null ? "" : lastMassCompile);
            bw.Write(lastSearchPath == null ? "" : lastSearchPath);
            WriteWindowPos(bw, 0);
            bw.Write(editorSplitterPosition); // reserved
            bw.Write(autoOpenMsgs);
            bw.Write(editorSplitterPosition2);
            bw.Write(PathScriptsHFile == null ? "" : PathScriptsHFile);
            bw.Write(language == null ? "english" : language);
            bw.Write(tabsToSpaces);
            bw.Write(tabSize);
            bw.Write(enableParser);
            bw.Write(shortCircuit);
            bw.Write(autocomplete);
            bw.Write(showLog);
            bw.Close();
        }

        public static string GetPreprocessedFile()
        {
            if (!File.Exists(PreprocessPath))
                return null;
            return File.ReadAllText(PreprocessPath);
        }

        public static string GetOutputPath(string infile)
        {
            return Path.Combine(outputDir, Path.GetFileNameWithoutExtension(infile)) + ".int";
        }

#if DLL_COMPILER
        public static string[] GetSslcCommandLine(string infile, bool preprocess) {
            return new string[] {
                "--", "-q",
                preprocess?"-P":"-p",
                optimize?"-O":"--",
                showWarnings?"--":"-n ",
                showDebug?"-d":"--",
                "-l", /* no logo */
                Path.GetFileName(infile),
                "-o",
                preprocess?preprocessPath:GetOutputPath(infile),
                null
            };
#else
        public static string GetSslcCommandLine(string infile, bool preprocess)
        {
            return (preprocess ? "-P " : "-p ")
                + ("-O" + optimize + " ")
                + (showWarnings ? "" : "-n ")
                + (showDebug ? "-d " : "")
                + ("-l ") /* always no logo */
                + (shortCircuit ? "-s " : "")
                + "\"" + Path.GetFileName(infile) + "\" -o \"" + (preprocess ? PreprocessPath : GetOutputPath(infile)) + "\"";
#endif
        }

        struct WindowPos
        {
            public bool maximized;
            public int x, y, width, height;
        } 
    }
}
