/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 11/9/2006
 * Time: 4:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using ICSharpCode.Diagrams;

namespace ICSharpCode.ClassDiagram
{
	/// <summary>
	/// Description of RouteStartShape.
	/// </summary>
	public class RouteStartShape : RouteShape
	{
        static Pen stroke = new Pen(Color.FromArgb(128, 0, 0, 0), 1);
        static Brush fill = new SolidBrush(Color.FromArgb(200, 0, 0, 0));

		protected override void Paint (Graphics graphics)
		{
            graphics.FillEllipse(fill, -2.8f, 6, 6, 6);
            graphics.DrawLine(stroke, 0, 0, 0, 6.25f);
		}
	}
}
