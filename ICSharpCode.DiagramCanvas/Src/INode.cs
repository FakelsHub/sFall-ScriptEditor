using System.Collections.Generic;

namespace ICSharpCode.ClassDiagram
{
	public interface INode
	{
		string Name {
			get;
			set;
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
