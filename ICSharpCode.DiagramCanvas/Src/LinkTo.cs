namespace ICSharpCode.ClassDiagram
{
	public class LinkTo
	{
		private string name;    // имя ноды к которой идет соединение
		private int line;       // номер строки в контенте переданной процедуры

		private float point;    // смещение от которой будет начинанться соединяющая линия

		public LinkTo(string name, int line)
		{
			this.name = name;
			this.line = line;
		}

		public string NameTo
		{
			get { return name; }
		}

		public int ContentLine
		{
			get { return line; }
		}

		public float PointTo
		{
			get { return point; }
			set { point = value; }
		}
	}
}
