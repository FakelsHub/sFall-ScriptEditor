using System.Windows.Forms;

namespace ScriptEditor.TextEditorUI
{
    public class DataGridViewEx : DataGridView
    {
        protected override bool DoubleBuffered
        {
            get { return this.GetStyle(ControlStyles.OptimizedDoubleBuffer); }
            set {
                if (value != this.DoubleBuffered) {
                    if (value) {
                        this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, value);
                    } else {
                        this.SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
                    }
                }
            }
        }

        public DataGridViewEx()
        {
            DoubleBuffered = true;
        }
    }
}
