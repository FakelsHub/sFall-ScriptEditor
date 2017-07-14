using System;
using System.Windows.Forms;

using System.Collections.Generic;

using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.ClassDiagram
{
    public partial class NodeDiagram : Form
    {
        ClassCanvas nodesCanvas = new ClassCanvas();

        IDocument scriptText;

        public NodeDiagram(IDocument document)
        {
            InitializeComponent();

            scriptText = document;
            panel1.Controls.Add(nodesCanvas);
			nodesCanvas.Dock = DockStyle.Fill;
			//nodesCanvas.CanvasItemSelected += OnItemSelected;
			//nodesCanvas.LayoutChanged += HandleLayoutChange;

        }

        private void addNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NodeCanvasItem item;
            List<LinkTo> linkList;
            List<ContentBody> ProcedureBody;
            List<string> linkfrom = new List<string>();
            linkfrom.Add("Start");
            linkfrom.Add("Node001"); 

            linkList = new List<LinkTo>();
            linkList.Add(new LinkTo("Node001",4));
            linkList.Add(new LinkTo("Node002",6));

            ProcedureBody = new List<ContentBody>();
            ProcedureBody.Add(new ContentBody("If <condition>", null,  opCodeType.None));
            ProcedureBody.Add(new ContentBody("reply", "Просто откройте крышку саркофага, и вашим глазам предстанет великолепная мумия, в блеске своей древней славы. Я только прошу не трогать ее и, э-э, не фотографировать со вспышкой.",   opCodeType.Reply));
            ProcedureBody.Add(new ContentBody("else",  null,            opCodeType.None));
            ProcedureBody.Add(new ContentBody("options", "Как твое имя?",   opCodeType.Options));
            ProcedureBody.Add(new ContentBody("end",    null,           opCodeType.None));
            ProcedureBody.Add(new ContentBody("options", "Пошел нафиг с новым годом. Проваливай короче!", opCodeType.Options));

            item = ClassCanvas.CreateItemFromType(new DataNode("Node000", linkList, linkfrom, ProcedureBody, NodesType.DialogStart));
            item.X = 150;
			item.Y = 150;
 			nodesCanvas.AddCanvasItem(item);
        }

        private void addNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NoteCanvasItem note = new NoteCanvasItem();
			note.X = 40;
			note.Y = 40;
			note.Width = 100;
			note.Height = 100;
			nodesCanvas.AddCanvasItem(note);
        }

        private void Zoom_ValueChanged(object sender, EventArgs e)
        {
            nodesCanvas.Zoom = Zoom.Value / 100f;
        }
    }
}
