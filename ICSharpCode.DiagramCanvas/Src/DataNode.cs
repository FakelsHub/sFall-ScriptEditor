using System.Collections.Generic;

namespace ICSharpCode.ClassDiagram
{
    public class ContentBody
    {
        public string scrText;
        public string msgText;
        public opCodeType type;

        public ContentBody(string scrText, string msgText, opCodeType type)
        {
            this.scrText = scrText;
            this.type = type;
            this.msgText = msgText;
        }
    }  
 
    public class DataNode : INode
	{
        string name;                                            // this node name
        NodesType nodeType;                                     // визуальный тип ноды
        List<LinkTo> linkedTo;                                  // список нод на которые ссылается эта нода  
        List<string> linkedForm;                                // список имен нод которые ссылается на эту ноду
        List<ContentBody> content = new List<ContentBody> ();   // контент ноды (построковое содержимое процедуры) 

        // конструктор
		public DataNode(string name, List<LinkTo> linkedTo, List<string> linkedForm , List<ContentBody> content, NodesType nodeType)
		{
			this.name = name;
            this.nodeType = nodeType;
            this.linkedTo = linkedTo;
            this.linkedForm = linkedForm;
            this.content = content;
		}

        public string Name {
			get {
				return name;
			}
		}
		
        public NodesType NodeType {
			get {
				return nodeType;
			}
            //set {
            //    nodeType = value;
            //}
		}

        public List<LinkTo> LinkedToNodes {
			get {
				return linkedTo;
			}
			set {
				linkedTo = value;
			}
		}

        public List<string> LinkedFromNodes {
			get {
				return linkedForm;
			}
            set {
                linkedForm = value;
            }
		}

        public List<ContentBody> NodeContent {
			get {
				return content;
			}
            set {
                content = value;
            }
		}
	}
}
