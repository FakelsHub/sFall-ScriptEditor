/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ICSharpCode.ClassDiagram
{
	public class NodeCircleShape : VectorShape
	{
        static Pen Pen1 = new Pen(Color.SteelBlue, 1.0f);

        static LinearGradientBrush shade = new LinearGradientBrush(
				new PointF(0, 0), new PointF(0, 22),
				Color.White, Color.LightSteelBlue);
        
        static GraphicsPath path = InitializePath();
		
		static GraphicsPath InitializePath ()
		{
			GraphicsPath path = new GraphicsPath();
			path.StartFigure();
			path.AddPolygon(new PointF[]{
				new PointF(5.75f, 3.75f),
				new PointF(9.25f, 6.75f),
				new PointF(5.75f, 9.75f),
			});
			path.CloseFigure();
			return path;
		}
		public override void Draw(Graphics graphics)
		{
			if (graphics == null) return;
			graphics.FillEllipse(shade, 2.0f, 2.0f, 9.5f, 9.5f);
            graphics.DrawEllipse(Pen1, 1.5f, 1.5f, 10.5f, 10.5f);
            graphics.FillPath(Brushes.DarkBlue, path);
		}
		
		public override float ShapeWidth
		{
			get { return 12.0f; }
		}
		
		public override float ShapeHeight
		{
			get { return 14.0f; }
		}
	}
}
