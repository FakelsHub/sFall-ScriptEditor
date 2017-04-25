﻿using System;
using System.Windows.Forms;

namespace ScriptEditor
{
    public partial class SettingsDialog : Form
    {
        private string outpath;
        private string scriptshpath;

        public SettingsDialog()
        {
            outpath = Settings.outputDir;
            scriptshpath = Settings.PathScriptsHFile;
            InitializeComponent();
            cbDebug.Checked = Settings.showDebug;
            cbIncludePath.Checked = Settings.overrideIncludesPath;
            cbOptimize.SelectedIndex = (Settings.optimize == 255 ? 1 : Settings.optimize);
            cbWarnings.Checked = Settings.showWarnings;
            cbWarnFailedCompile.Checked = Settings.warnOnFailedCompile;
            cbMultiThread.Checked = Settings.multiThreaded;
            cbAutoOpenMessages.Checked = Settings.autoOpenMsgs;
            tbLanguage.Text = Settings.language;
            cbTabsToSpaces.Checked = Settings.tabsToSpaces;
            tbTabSize.Text = Convert.ToString(Settings.tabSize);
            cbEnableParser.Checked = Settings.enableParser;
            cbShortCircuit.Checked = Settings.shortCircuit;
            cbAutocomplete.Checked = Settings.autocomplete;
            Highlight_comboBox.SelectedIndex = Settings.highlight;
            HintLang_comboBox.SelectedIndex = Settings.hintsLang;
            if (!Settings.enableParser) cbParserWarn.Enabled = false;
            cbParserWarn.Checked = Settings.parserWarn;
            SetLabelText();
        }

        private void SetLabelText()
        {
            textBox2.Text = outpath == null ? "<unset>" : outpath;
            textBox1.Text = scriptshpath == null ? "<unset>" : scriptshpath;
        }

        private void SettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.showDebug = cbDebug.Checked;
            Settings.overrideIncludesPath = cbIncludePath.Checked;
            Settings.optimize = (byte)cbOptimize.SelectedIndex;
            Settings.showWarnings = cbWarnings.Checked;
            Settings.warnOnFailedCompile = cbWarnFailedCompile.Checked;
            Settings.multiThreaded = cbMultiThread.Checked;
            Settings.outputDir = outpath;
            Settings.autoOpenMsgs = cbAutoOpenMessages.Checked;
            Settings.PathScriptsHFile = scriptshpath;
            Settings.language = tbLanguage.Text.Length == 0 ? "english" : tbLanguage.Text;
            Settings.tabsToSpaces = cbTabsToSpaces.Checked;
            try {
                Settings.tabSize = Convert.ToInt32(tbTabSize.Text);
            } catch (System.FormatException) {
                Settings.tabSize = 3;
            }
            if (Settings.tabSize < 1 || Settings.tabSize > 30) {
                Settings.tabSize = 3;
            }
            Settings.enableParser = cbEnableParser.Checked;
            Settings.shortCircuit = cbShortCircuit.Checked;
            Settings.autocomplete = cbAutocomplete.Checked;
            Settings.highlight = (byte)Highlight_comboBox.SelectedIndex;
            Settings.hintsLang = (byte)HintLang_comboBox.SelectedIndex;
            Settings.parserWarn = cbParserWarn.Checked;
            Settings.Save();
        }

        private void bChange_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                outpath = folderBrowserDialog1.SelectedPath;
                SetLabelText();
            }
        }

        private void bScriptsH_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                scriptshpath = System.IO.Path.GetDirectoryName(openFileDialog1.FileName);
                SetLabelText();
            }
        }
        void TbTabSizeKeyPress(object sender, KeyPressEventArgs e)
        {

        }
        void TbTabSizeTextChanged(object sender, EventArgs e)
        {
            int n;
            try {
                n = Convert.ToInt32(tbTabSize.Text);
            } catch (System.FormatException) {
                n = 3;
            }
            tbTabSize.Text = Convert.ToString(n);
        }

        private void cbEnableParser_CheckedChanged(object sender, EventArgs e)
        {
            cbParserWarn.Enabled = cbEnableParser.Checked;
        }
    }
}
