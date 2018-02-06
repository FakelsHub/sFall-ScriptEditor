using System.Drawing;

using ScriptEditor.CodeTranslation;

namespace ScriptEditor.TextEditorUI.CompleteList
{
    public class AutoCompleteItem
    {
        private string name;
        private string hint = null;
        private string type = null;

        public AutoCompleteItem(string name)
        {
            int sep = name.IndexOf("|");
            if (sep != -1) {
                this.name = name.Substring(0, sep);
                this.hint = name.Substring(sep + 1);
                sep = hint.IndexOf("|");
                if (sep != -1) {
                    this.type = hint.Substring(sep + 1);
                    this.hint = hint.Remove(sep);
                }
            } else
                this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public string Hint
        {
            get { return hint; }
        }

        public int NameLength
        {
            get { return name.Length; }
        }

        public NameType GetType
        {
            get
            {   switch (type)
                {
                    case "M": // Macros
                        return NameType.Macro;
                    case "V": // Variable
                        return NameType.GVar;
                    case "P": // Procedure
                        return NameType.Proc;
                    default:  // Opcodes
                        return NameType.None;
                }
            }
        }

        public Brush GetBrush(bool colored)
        {
            if (!colored)
                return Brushes.Black;

            switch (type)
            {
                case "M": // Macros
                    return Brushes.MediumVioletRed;
                case "V": // Variable
                    return Brushes.RoyalBlue;
                case "P": // Procedure
                    return Brushes.Indigo;
                default:  // Opcodes
                    return Brushes.DarkMagenta;
            }
        }

        public override string ToString()
        {
            return name;
        }
    }
}
