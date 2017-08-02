using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace ScriptEditor
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "SFALL_SCRIPT_EDITOR_4");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Settings.Load();
            // reset working folder to EXE directory (to resolve possible issues in parse_main)
            Directory.SetCurrentDirectory(Settings.ProgramFolder);

            if (args.Length > 0 && mutex.WaitOne(TimeSpan.Zero, true) 
                && Path.GetExtension(args[0]).ToLower() == ".msg") {
                
                mutex.Close();
                // run only Messages editor
                MessageEditor me = new MessageEditor(args[0].ToString());
                Application.Run(me);
            } else {
                // check if another instance is already running
                if (mutex.WaitOne(TimeSpan.Zero, true)) {
                    // pass arguments of command line to opening
                    TextEditor te = new TextEditor(args);
                    Application.Run(te);
                    mutex.ReleaseMutex();
                } else {
                    // only show message if opened normally without command line arguments
                    if (args.Length == 0) 
                        MessageBox.Show("Another instance is already running!", "Sfall Script Editor");
                    else {
                        // pass command line arguments via file
                        SingleInstanceManager.SaveCommandLine(args);
                    }
                    // send message to other instance
                    SingleInstanceManager.SendEditorOpenMessage();
                }
                SingleInstanceManager.DeleteCommandLine();
            }
        }

        public static void printLog(string log) { File.AppendAllText(Settings.ProgramFolder + "\\sse.log", "Еxception in " + log + "\r\n"); }
    }
}
