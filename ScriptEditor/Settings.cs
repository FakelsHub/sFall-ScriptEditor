﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ScriptEditor
{
    public enum SavedWindows { Main, Count }
    public enum  EncodingType : byte { Default, OEM866 }

    public static class Settings
    {
        public static Encoding EncCodePage;
        public static Size HeadersFormSize;

        public static readonly string ProgramFolder = Application.StartupPath;
        public static readonly string SettingsFolder = Path.Combine(ProgramFolder, "settings");
        public static readonly string ResourcesFolder = Path.Combine(ProgramFolder, "resources");
        public static readonly string DescriptionsFolder = Path.Combine(ProgramFolder, "descriptions");
        public static readonly string scriptTempPath = Path.Combine(ProgramFolder, "scrTemp");

        private static readonly string RecentPath = Path.Combine(SettingsFolder, "recent.dat");
        private static readonly string SettingsPath = Path.Combine(SettingsFolder, "settings.dat");
        public static readonly string SearchHistoryPath = Path.Combine(SettingsFolder, "SearchHistory.ini");
        public static readonly string PreprocDefPath = Path.Combine(SettingsFolder, "PreprocDefine.ini");

        public static PrivateFontCollection Fonts = new PrivateFontCollection();
        public static readonly Dictionary<string, float> FontAdjustSize = new Dictionary<string, float>() {
            {"Anonymous Pro", 10.5f},       {"Consolas", 10.5f},            {"Cousine", 10.5f},
            {"InconsolataCyr", 11.0f},      {"InputMono", 9.5f},            {"InputMonoCondensed", 9.5f},
            {"Liberation Mono", 10.25f},    {"Meslo LG S DZ", 9.75f},       {"Ubuntu Mono",  11.75f}
        };
        
        const int MAX_RECENT = 40;

        public static byte optimize = 1;
        public static bool showWarnings = true;
        public static bool showDebug = true; //show additional information when compiling script
        public static bool overrideIncludesPath = true;
        public static bool warnOnFailedCompile = true;
        public static bool multiThreaded = true;
        public static bool autoOpenMsgs = false;
        public static bool openMsgEditor = false;
        public static string outputDir;
        public static string pathScriptsHFile;
        public static string lastMassCompile;
        public static string lastSearchPath;
        public static readonly List<string> msgListPath = new List<string>();
        private static List<string> recent = new List<string>();
        private static List<string> recentMsg = new List<string>();
        private static Dictionary<string, ushort> scriptPosition = new Dictionary<string, ushort>();
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
        public static byte highlight = 1; // 0 = Original, 1 = FGeck
        public static byte encoding = (byte)EncodingType.Default; // 0 = DEFAULT, 1 = DOS(cp866)
        public static bool allowDefine = false;
        public static bool parserWarn = true;
        public static bool useWatcom = false;
        public static string preprocDef = null;
        public static bool ignoreCompPath = true;
        public static bool userCmdCompile = false;
        public static bool msgHighlightComment = true;
        public static byte msgHighlightColor = 0;
        public static byte msgFontSize = 0;
        public static string pathHeadersFiles;
        public static bool associateID = false;
        public static bool useMcpp = true;
        public static bool autocompleteColor = true;
        public static bool autoInputPaired = true;
        public static bool showTabsChar = false;
        public static bool autoTrailingSpaces = true;
        public static bool showTips = true;
        public static bool shortDesc = false;
        public static byte selectFont = 11; // 0 - default
        public static sbyte sizeFont = 0; // 0 - default
        public static bool showVRuler = true;
        public static bool storeLastPosition = true;
        public static bool saveScriptUTF8 = false;
        public static bool decompileF1 = false;
        public static bool winAPITextRender = true;

        // for Flowchart
        public static bool autoUpdate = false;
        public static bool autoSaveChart = true;
        public static bool autoArrange = false;
        public static bool woExitNode = true;
        public static bool autoHideNodes = false;

        // no saved settings
        public static bool msgLipColumn = true;
        public static bool firstRun = false;

        public static void SetupWindowPosition(SavedWindows window, Form f)
        {
            WindowPos wp = windowPositions[(int)window];
            if (wp.width == 0)
                return;
            f.Location = new System.Drawing.Point(wp.x, wp.y);
            if (wp.maximized)
                f.WindowState = FormWindowState.Maximized;
            else
                f.ClientSize = new System.Drawing.Size(wp.width, wp.height);
            f.StartPosition = FormStartPosition.Manual;
        }

        public static void SaveWindowPosition(SavedWindows window, Form f)
        {
            WindowPos wp = new WindowPos();
            wp.maximized = f.WindowState == FormWindowState.Maximized;
            wp.x = f.Location.X;
            wp.y = f.Location.Y;
            wp.width = f.ClientSize.Width;
            wp.height = f.ClientSize.Height;
            windowPositions[(int)window] = wp;
        }

        public static void SetLastScriptPosition(string script, int line)
        {
            if (!storeLastPosition || line < 10)
                return;
            if (scriptPosition.ContainsKey(script))
                scriptPosition[script] = (ushort)line;
            else
                scriptPosition.Add(script, (ushort)line);
        }

        public static int GetLastScriptPosition(string script)
        {
            if (storeLastPosition && scriptPosition.ContainsKey(script))
                return (ushort)scriptPosition[script];
            else 
                return 0;
        }

        public static void AddMsgRecentFile(string s, bool b = false)
        {
            SubRecentFile(ref recentMsg, s, b);
        }

        public static void AddRecentFile(string s, bool b = false)
        {
            SubRecentFile(ref recent, s, b, true);
        }

        public static void SubRecentFile(ref List<string> recent, string s, bool b, bool p = false)
        {
            for (int i = 0; i < recent.Count; i++) {
                if (string.Compare(recent[i], s, true) == 0)
                    recent.RemoveAt(i--);
            }
            if (!b && recent.Count >= MAX_RECENT) {
                if (p) scriptPosition.Remove(Path.GetFileName(recent[0]));
                recent.RemoveAt(0);
            }
            if (!b) recent.Add(s);
        }

        public static string[] GetRecent() { return recent.ToArray(); }
        public static string[] GetMsgRecent() { return recentMsg.ToArray(); }

        private static void LoadWindowPos(BinaryReader br, int i)
        {
            windowPositions[i].maximized = br.ReadBoolean();
            windowPositions[i].x = br.ReadInt32();
            windowPositions[i].y = br.ReadInt32();
            windowPositions[i].width = br.ReadInt32();
            windowPositions[i].height = br.ReadInt32();
        }

        private static void LoadInternal(BinaryReader br, BinaryReader brRecent)
        {
            if (br != null) {
                try {
                    firstRun = br.ReadBoolean();
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
                    warnOnFailedCompile = br.ReadBoolean();
                    multiThreaded = br.ReadBoolean();
                    lastMassCompile = br.ReadString();
                    if (lastMassCompile.Length == 0)
                        lastMassCompile = null;
                    lastSearchPath = br.ReadString();
                    if (lastSearchPath.Length == 0)
                        lastSearchPath = null;
                    LoadWindowPos(br, 0);
                    editorSplitterPosition = br.ReadInt32();
                    autoOpenMsgs = br.ReadBoolean();
                    editorSplitterPosition2 = br.ReadInt32();
                    pathHeadersFiles = br.ReadString();
                    if (pathHeadersFiles.Length == 0)
                        pathHeadersFiles = null;
                    language = br.ReadString();
                    if (language.Length == 0)
                        language = "english";
                    tabsToSpaces = br.ReadBoolean();
                    tabSize = br.ReadInt32();
                    enableParser = br.ReadBoolean();
                    shortCircuit = br.ReadBoolean();
                    autocomplete = br.ReadBoolean();
                    showLog = br.ReadBoolean();
                    parserWarn = br.ReadBoolean();
                    useWatcom = br.ReadBoolean();
                    preprocDef = br.ReadString();
                    if (preprocDef.Length == 0)
                        preprocDef = null;
                    ignoreCompPath = br.ReadBoolean();
                    byte MsgItems = br.ReadByte();
                    for (byte i = 0; i < MsgItems; i++)
                        msgListPath.Add(br.ReadString());
                    openMsgEditor = br.ReadBoolean();
                    userCmdCompile = br.ReadBoolean();
                    msgHighlightComment = br.ReadBoolean();
                    msgHighlightColor = br.ReadByte();
                    msgFontSize = br.ReadByte();
                    pathScriptsHFile = br.ReadString();
                    if (pathScriptsHFile.Length == 0)
                        pathScriptsHFile = null;
                    associateID = br.ReadBoolean();
                    //
                    autoUpdate = br.ReadBoolean();
                    autoSaveChart = br.ReadBoolean();
                    autoArrange = br.ReadBoolean();
                    woExitNode = br.ReadBoolean();
                    useMcpp = br.ReadBoolean();
                    autocompleteColor = br.ReadBoolean();
                    autoInputPaired = br.ReadBoolean();
                    showTabsChar = br.ReadBoolean();
                    autoTrailingSpaces = br.ReadBoolean();
                    showTips = br.ReadBoolean();
                    shortDesc = br.ReadBoolean();
                    autoHideNodes = br.ReadBoolean();
                    selectFont = br.ReadByte();
                    sizeFont = br.ReadSByte();
                    showVRuler = br.ReadBoolean();
                    storeLastPosition = br.ReadBoolean();
                    saveScriptUTF8 = br.ReadBoolean();
                    decompileF1 = br.ReadBoolean();
                    winAPITextRender = br.ReadBoolean();
                }
                catch { MessageBox.Show("An error occurred while reading configuration file.\n"
                                        + "File setting.dat may be in wrong format.", "Setting read error"); 
                }
                br.Close();
            }
            // Recent files
            if (brRecent == null) return;
            int recentItems = brRecent.ReadByte();
            int recentMsgItems = brRecent.ReadByte();
            for (int i = 0; i < recentItems; i++)
                recent.Add(brRecent.ReadString());
            for (int i = 0; i < recentMsgItems; i++)
                recentMsg.Add(brRecent.ReadString());
            //
            int positionItems = brRecent.ReadByte();
            for (int i = 0; i < positionItems; i++)
                scriptPosition.Add(brRecent.ReadString(), brRecent.ReadUInt16());
            brRecent.Close();
        }

        public static void Load()
        {
            Program.printLog("   Load configuration setting.");

            if (!Directory.Exists(scriptTempPath)) {
                Directory.CreateDirectory(scriptTempPath);
            } else 
                foreach (string file in Directory.GetFiles(scriptTempPath))
                    File.Delete(file);

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
            
            BinaryReader brRecent = null, brSettings = null;
            if (File.Exists(RecentPath))
                brRecent = new BinaryReader(File.OpenRead(RecentPath));
            if (File.Exists(SettingsPath))
                brSettings = new BinaryReader(File.OpenRead(SettingsPath));
            LoadInternal(brSettings, brRecent);
            
            if (!File.Exists(SearchHistoryPath))
                File.Create(SearchHistoryPath).Close();
            if (!File.Exists(PreprocDefPath))
                File.Create(PreprocDefPath).Close();
            if (!firstRun)
                FileAssociation.Associate();
            
            EncCodePage = (encoding == (byte)EncodingType.OEM866) ? Encoding.GetEncoding("cp866") : Encoding.Default;

            //Load custom fonts
            try {
                foreach (string file in Directory.GetFiles(Settings.ResourcesFolder + @"\fonts\", "*.ttf"))
                    Fonts.AddFontFile(file);
            } catch (System.IO.DirectoryNotFoundException ) { }
        }
            
        
        public static void SetTextAreaFont(ICSharpCode.TextEditor.TextEditorControl TE)
        {
            if (Fonts.Families.Length == 0)
                return;

            int indexFont = selectFont - 1;
            Font font;
            if (indexFont > -1) {
                FontFamily family = Fonts.Families[indexFont];
                float sz;
                if (!FontAdjustSize.TryGetValue(family.Name, out sz))
                    sz = 10.0f;
                font = new Font(family, sz + sizeFont, FontStyle.Regular, GraphicsUnit.Point);
            } else
                font = new Font("Courier New", 10.0f + sizeFont, FontStyle.Regular);
            TE.TextEditorProperties.Font = font;
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
            bw.Write((byte)255);
            bw.Write(allowDefine);
            bw.Write(hintsLang);
            bw.Write(highlight);
            bw.Write(encoding);
            bw.Write(optimize);
            bw.Write(showWarnings);
            bw.Write(showDebug);
            bw.Write(overrideIncludesPath);
            bw.Write(outputDir == null ? "" : outputDir);
            bw.Write(warnOnFailedCompile);
            bw.Write(multiThreaded);
            bw.Write(lastMassCompile == null ? "" : lastMassCompile);
            bw.Write(lastSearchPath == null ? "" : lastSearchPath);
            WriteWindowPos(bw, 0);
            bw.Write(editorSplitterPosition);
            bw.Write(autoOpenMsgs);
            bw.Write(editorSplitterPosition2);
            bw.Write(pathHeadersFiles == null ? "" : pathHeadersFiles);
            bw.Write(language == null ? "english" : language);
            bw.Write(tabsToSpaces);
            bw.Write(tabSize);
            bw.Write(enableParser);
            bw.Write(shortCircuit);
            bw.Write(autocomplete);
            bw.Write(showLog);
            bw.Write(parserWarn);
            bw.Write(useWatcom);
            bw.Write(preprocDef ?? string.Empty);
            bw.Write(ignoreCompPath);
            bw.Write((byte)msgListPath.Count);
            for (int i = 0; i < msgListPath.Count; i++)
                bw.Write(msgListPath[i]);
            bw.Write(openMsgEditor);
            bw.Write(userCmdCompile);
            bw.Write(msgHighlightComment);
            bw.Write(msgHighlightColor);
            bw.Write(msgFontSize);
            bw.Write(pathScriptsHFile == null ? "" : pathScriptsHFile);
            bw.Write(associateID);
            //
            bw.Write(autoUpdate);
            bw.Write(autoSaveChart);
            bw.Write(autoArrange);
            bw.Write(woExitNode);
            bw.Write(useMcpp);
            bw.Write(autocompleteColor);
            bw.Write(autoInputPaired);
            bw.Write(showTabsChar);
            bw.Write(autoTrailingSpaces);
            bw.Write(showTips);
            bw.Write(shortDesc);
            bw.Write(autoHideNodes);
            bw.Write(selectFont);
            bw.Write(sizeFont);
            bw.Write(showVRuler);
            bw.Write(storeLastPosition);
            bw.Write(saveScriptUTF8);
            bw.Write(decompileF1);
            bw.Write(winAPITextRender);
            bw.Close();

            // Recent files
            BinaryWriter bwRecent = new BinaryWriter(File.Create(RecentPath));
            bwRecent.Write((byte)recent.Count);
            bwRecent.Write((byte)recentMsg.Count);
            for (int i = 0; i < recent.Count; i++)
                bwRecent.Write(recent[i]); 
            for (int i = 0; i < recentMsg.Count; i++)
                bwRecent.Write(recentMsg[i]);
            //
            string[] key = new string[scriptPosition.Count];
            ushort[] value = new ushort[scriptPosition.Count];
            scriptPosition.Keys.CopyTo(key, 0);
            scriptPosition.Values.CopyTo(value, 0);
            bwRecent.Write((byte)scriptPosition.Count);
            for (int i = 0; i < scriptPosition.Count; i++) {
                bwRecent.Write(key[i]);
                bwRecent.Write(value[i]);
            }
            bwRecent.Close();
        }

        public static void SaveSettingData(Form mainfrm)
        {
            TextEditor frm = mainfrm as TextEditor;
            StreamWriter sw = new StreamWriter(Settings.SearchHistoryPath);
            foreach (var item in frm.SearchTextComboBox.Items)
                sw.WriteLine(item.ToString());
            sw.Close();
            openMsgEditor = frm.msgAutoOpenEditorStripMenuItem.Checked;
            if (frm.WindowState != FormWindowState.Minimized) SaveWindowPosition(SavedWindows.Main, mainfrm);
            Save();
            Directory.Delete(scriptTempPath, true);
        }

        struct WindowPos
        {
            public bool maximized;
            public int x, y, width, height;
        } 
    }
}
