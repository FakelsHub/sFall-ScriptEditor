using System;
using System.Windows.Forms;
using Path = System.IO.Path;
using Directory = System.IO.Directory;
using SearchOption = System.IO.SearchOption;
using System.Drawing;

namespace ScriptEditor
{
    public partial class Headers : Form
    {
        public Point xy_pos;

        public Headers()
        {
            InitializeComponent();
            foreach (string file in Directory.GetFiles(Settings.PathScriptsHFile, "*.h", SearchOption.AllDirectories)) {
                ListViewItem lw = new ListViewItem();
                lw.ImageIndex=0;
                lw.Text = Path.GetFileName(file).ToLower();
                listView1.Items.Add(lw); 
            }
        }
        
        private void Headers_Load(object sender, EventArgs e)
        {
            this.Location = new Point(xy_pos.X + TextEditor.ActiveForm.Bounds.X, (xy_pos.Y + TextEditor.ActiveForm.Bounds.Y + 60));
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            TextEditor.sHeaderfile = Path.Combine(Settings.PathScriptsHFile, e.Item.Text);
            Headers_Deactivate(null, null);
        }

        private void Headers_Deactivate(object sender, EventArgs e)
        {
            listView1.Invalidate();
            this.Dispose();
        }
    }
}
