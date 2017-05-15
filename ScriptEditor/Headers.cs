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
        private TextEditor TE;
        private string hFile;

        public Headers(Form te)
        {
            TE = te as TextEditor;
            InitializeComponent();
            foreach (string file in Directory.GetFiles(Settings.PathScriptsHFile, "*.h", SearchOption.AllDirectories)) {
                ListViewItem lw = new ListViewItem();
                lw.ImageIndex=0;
                lw.Text = Path.GetFileName(file).ToLower();
                lw.Tag = file;
                listView1.Items.Add(lw); 
            }
        }
        
        private void Headers_Load(object sender, EventArgs e)
        {
            this.Location = new Point(xy_pos.X + TextEditor.ActiveForm.Bounds.X - 100, (xy_pos.Y + TextEditor.ActiveForm.Bounds.Y + 60));
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            hFile = e.Item.Tag.ToString();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listView1.FocusedItem.Selected) {
                Close();
                TE.AcceptHeaderFile(hFile);
            }
        }

        private void Headers_Deactivate(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
