using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEditor.TextEditorUI
{
    class WorkerArgs
    {
        public readonly string text;
        public readonly TabInfo tab;

        public WorkerArgs(string text, TabInfo tab)
        {
            this.text = text;
            this.tab = tab;
        }
    }
}
