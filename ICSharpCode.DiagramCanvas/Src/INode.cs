using System.Collections.Generic;

namespace ICSharpCode.ClassDiagram
{
	public interface INode
	{
 		string Name {
			get;
		}

        List<LinkTo> LinkedToNodes {
			get;
            set;
		}
		
        List<string> LinkedFromNodes {
			get;
		}
                
        List<ContentBody> NodeContent {
			get;
		}
        		
        NodesType NodeType {
			get;
		}
	}
}
