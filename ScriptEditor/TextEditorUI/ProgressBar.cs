using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ScriptEditor.TextEditorUI
{
    internal class ProgressBarForm
    {
        Form progressForm;
        ProgressBar bar;

        public ProgressBarForm(Form owner, int max)
        {
            bar = new ProgressBar() { Width = 305, Height = 15, Top = 10, Maximum = max};
            
            progressForm = new Form()
            {
                MinimumSize = new Size(200, 10), Width = 312, Height = 15, 
                ControlBox = false, ShowIcon = false, ShowInTaskbar = false,
                StartPosition = FormStartPosition.Manual,
                FormBorderStyle = FormBorderStyle.FixedSingle
            };

            progressForm.Controls.Add(bar);

            progressForm.Location = new Point(owner.Location.X + (owner.Width - progressForm.Width) / 2,
                                              owner.Location.Y + (owner.Height - progressForm.Height) / 2);
            progressForm.Show();
        }

        public int SetProgress
        {
            set { bar.Value = value; }
        }

        public void Dispose()
        {
            progressForm.Dispose();
        }
    }
}
