/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;

using System.Xml;
using System.Xml.XPath;

using ICSharpCode.Diagrams;
using ICSharpCode.Diagrams.Drawables;

namespace ICSharpCode.ClassDiagram
{
    /// <summary>
    /// This class was built from ClassCanvasItem 
    /// </summary>
    public class NodeCanvasItem : CanvasItem, IDisposable
	{
        INode dataNode;

		#region Graphics related variables
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font TitleFont = new Font (FontFamily.GenericSansSerif, 16, FontStyle.Bold, GraphicsUnit.Pixel);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font SubtextFont = new Font (FontFamily.GenericSansSerif, 10, FontStyle.Regular, GraphicsUnit.Pixel);
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font GroupTitleFont = new Font (FontFamily.GenericSansSerif, 12, FontStyle.Bold, GraphicsUnit.Pixel);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly Font LinkedFont = new Font (FontFamily.GenericSansSerif, 8, FontStyle.Bold, GraphicsUnit.Pixel);
		public static readonly Font ScriptFont = new Font (FontFamily.GenericSansSerif, 12, FontStyle.Regular, GraphicsUnit.Pixel);
		
		LinearGradientBrush grad;
		GraphicsPath shadowpath;
		DrawableRectangle containingShape;
		
		#endregion

		InteractiveHeaderedItem classItemHeaderedContent;
		DrawableItemsStack classItemContainer = new DrawableItemsStack();

		CollapseExpandShape collapseExpandShape = new CollapseExpandShape();

		DrawableItemsStack titles = new DrawableItemsStack();
		DrawableItemsStack titlesCollapsed = new DrawableItemsStack();
		DrawableItemsStack titlesExpanded = new DrawableItemsStack();
        
        InteractiveItemsStack items = new InteractiveItemsStack();		
		
        DrawableRectangle titlesBackgroundCollapsed;
		DrawableRectangle titlesBackgroundExpanded;
        
        //DrawableItemsStack<InteractiveHeaderedItem> groups = new DrawableItemsStack<InteractiveHeaderedItem>();
        Dictionary<InteractiveHeaderedItem, string> groupNames = new Dictionary<InteractiveHeaderedItem, string>(); // TODO - this is really an ugly patch
		Dictionary<string, InteractiveHeaderedItem> groupsByName = new Dictionary<string, InteractiveHeaderedItem>(); // TODO - this is really an ugly patch
 
        // unused
        //DrawableItemsStack<TextSegment> interfaces = new DrawableItemsStack<TextSegment>();
		
        #region Graphics related members
		
		static Color titlesBG = Color.FromArgb(255,217, 225, 241);
		protected virtual Color TitleBackground
		{
			get { return titlesBG; }
		}

		protected virtual LinearGradientBrush TitleBG
		{
			get { return grad; }
		}

		//Reply Background
		static Brush replyTitlesBG = new SolidBrush(Color.FromArgb(255, 230, 232, 239));
		protected virtual Brush ReplyTitlesBackground
		{
			get { return replyTitlesBG; }
		}

        static Brush replyTextTitlesBG = new SolidBrush(Color.FromArgb(255, 235, 238, 244));
		protected virtual Brush ReplyTextTitlesBackground
		{
			get { return replyTextTitlesBG; }
		}

        //Options Background
        static Brush optionTitlesBG = new SolidBrush(Color.PaleGoldenrod);
		protected virtual Brush OptionTitlesBackground
		{
			get { return optionTitlesBG; }
		}

        static Brush optionTextTitlesBG = new SolidBrush(Color.FromArgb(255, 243, 237, 175));
		protected virtual Brush OptionTextTitlesBackground
		{
			get { return optionTextTitlesBG; }
		}

		static Brush contentBG = new SolidBrush(Color.FromArgb(255, 250, 250, 255));
		protected virtual Brush ContentBG
		{
			get { return contentBG; }
		}
		
		protected virtual bool RoundedCorners
		{
			get { return true; }
		}
		
		protected virtual int CornerRadius
		{
			get { return 15; }
		}
		#endregion
		
		protected override bool AllowHeightModifications()
		{
            return false;
		}
		
		public override float Width
		{
			set
			{
				base.Width = Math.Max (value, 100.0f);
				PrepareFrame();
			}
		}
		
		public override float GetAbsoluteContentWidth()
		{
			return classItemHeaderedContent.GetAbsoluteContentWidth();
		}
		
		public override float GetAbsoluteContentHeight()
		{
			return classItemHeaderedContent.GetAbsoluteContentHeight();
		}
		
		public INode GetNodeData
		{
			get { return dataNode; }
		}
        
		#region Constructors
		// —оздание ноды (start)
		public NodeCanvasItem(INode dataNode)
		{
			this.dataNode = dataNode;
            
            // Header gradient
			grad = new LinearGradientBrush(new PointF(0, 0), new PointF(1, 0), TitleBackground, Color.White);
					
			classItemHeaderedContent = new InteractiveHeaderedItem(titlesCollapsed, titlesExpanded, InitContentContainer(InitContent()));

			classItemContainer.Container = this;
			classItemContainer.Add(classItemHeaderedContent);
			
            Pen outlinePen = GetNodeOutlinePen();

            if (RoundedCorners) {
				int radius = CornerRadius;
				containingShape = new DrawableRectangle(null, outlinePen, radius, radius, radius, radius);
                
                titlesBackgroundCollapsed = new DrawableRectangle(grad, null, radius, radius, radius, radius);
				titlesBackgroundExpanded = new DrawableRectangle(grad, null, radius, radius, 0, 0);

			} else {
				containingShape = new DrawableRectangle(null, outlinePen, 0, 0, 0, 0);
			    
                titlesBackgroundCollapsed = new DrawableRectangle(grad, null, 0, 0, 0, 0);
				titlesBackgroundExpanded = new DrawableRectangle(grad, null, 0, 0, 0, 0);
            }

			classItemContainer.Add(containingShape);
			classItemContainer.OrientationAxis = Axis.Z;
			
			titles.Border = 5;
			
			titlesCollapsed.Add(titlesBackgroundCollapsed);
			titlesCollapsed.Add(titles);
			titlesCollapsed.OrientationAxis = Axis.Z;
			
			titlesExpanded.Add(titlesBackgroundExpanded);
			titlesExpanded.Add(titles);
			titlesExpanded.OrientationAxis = Axis.Z;
		}
		#endregion
		
        // Stroke type for node
		private Pen GetNodeOutlinePen()
		{
			Pen pen = new Pen(Color.Gray);
            pen.Width = 2;

			if (dataNode.NodeType == NodesType.DialogStart) {
				pen.DashStyle = DashStyle.Dash;
                pen.Color = Color.DarkGreen;
			}
			else if (dataNode.NodeType == NodesType.DialogEnd) {
				pen.DashStyle = DashStyle.Dash;
                pen.Color = Color.DarkRed;
			}

			return pen;
		}
		
		protected virtual DrawableRectangle InitContentBackground()
		{
			if (RoundedCorners) {
				int radius = CornerRadius;
				return new DrawableRectangle(ContentBG, null, 0, 0, radius, radius);
			} else
				return new DrawableRectangle(ContentBG, null, 0, 0, 0, 0);
		}
		
        // помещение содержимого контента в отрисованный контейнер под заголовком
        // определнение цвета заливки дл€ фона контейнера
		protected virtual DrawableItemsStack InitContentContainer(params IDrawableRectangle[] items)
		{
			DrawableItemsStack content = new DrawableItemsStack();
			content.OrientationAxis = Axis.Z;
			content.Add(InitContentBackground());
			
			foreach (IDrawableRectangle item in items)
				content.Add(item);

            return content;
		}
		
		protected virtual IDrawableRectangle InitContent ()
		{
            items.MinWidth = 80;
            items.Border = 5;
            items.Spacing = 2;
            //Items.Padding = 1;

            return items;
		}
		
		public void Initialize ()
		{
            PrepareNodeContent();
			PrepareTitles();
            Width = Math.Min(GetAbsoluteContentWidth(), 350.0f);
            OffsetPointTo();
		}
		
		#region Preparations
        //создание заголовока дл€ ноды
		protected virtual void PrepareTitles ()
		{
			if (dataNode == null) return;
			
			DrawableItemsStack title = new DrawableItemsStack();
			title.OrientationAxis = Axis.X;
			
            //Ќазвание
			TextSegment titleString = new TextSegment(base.Graphics, dataNode.Name, TitleFont, true, StringAlignment.Center);
			title.Add(new NodeCircleShape());
            title.Add(titleString);

            title.Add(collapseExpandShape);
			collapseExpandShape.Collapsed = Collapsed;
			titles.OrientationAxis = Axis.Y;
			titles.Add(title);
			titles.Add(new TextSegment(base.Graphics, "Linked from:", LinkedFont, true));

            //—писок Ќод которые ссылаютс€ на эту ноду
            DrawableItemsStack linkedFrom = new DrawableItemsStack();
			linkedFrom.OrientationAxis = Axis.Y;
            foreach (string linkF in dataNode.LinkedFromNodes)
            {
                linkedFrom.Add(new TextSegment(base.Graphics, linkF, SubtextFont, true));
            }
            titles.Add(linkedFrom);

            //добавление интерфейсов /* неиспользуетс€ */
            //interfaces.Add(new TextSegment(base.Graphics, "Node999", SubtextFont, true));
		}

        // создание заголовка раскрывающихс€ полей, и кнопки +/- дл€ раскрыти€ Reply/Option
		protected InteractiveHeaderedItem PrepareMessagesHeader (string title, IDrawableRectangle content, bool reply)
		{
			#region Prepare Container
			DrawableItemsStack headerPlus = new DrawableItemsStack();
			DrawableItemsStack headerMinus = new DrawableItemsStack();
			
			headerPlus.OrientationAxis = Axis.X;
			headerMinus.OrientationAxis = Axis.X;
			#endregion
			
			#region Create Header
			TextSegment titleSegment = new TextSegment(Graphics, title, GroupTitleFont, true);
			
			PlusShape plus = new PlusShape();
			plus.Border = 3;
			headerPlus.Add(plus);
			headerPlus.Add(titleSegment);

			MinusShape minus = new MinusShape();
			minus.Border = 3;
			headerMinus.Add(minus);
			headerMinus.Add(titleSegment);
			
            //пиктограмма
            //if (!reply) {
            //    DrawableItemsStack<VectorShape> image = new DrawableItemsStack<VectorShape>();
            //    image.OrientationAxis = Axis.X;
            //    image.KeepAspectRatio = true;
            //    image.Border = -3.0f;
            //    image.Add(new OptionsCircleShape()); 

            //    headerPlus.Add(image);
            //    headerMinus.Add(image);
            //}

			DrawableItemsStack headerCollapsed = new DrawableItemsStack();
			DrawableItemsStack headerExpanded = new DrawableItemsStack();
			
			headerCollapsed.OrientationAxis = Axis.Z;
			headerExpanded.OrientationAxis = Axis.Z;

			headerCollapsed.Add (new DrawableRectangle((reply) ? ReplyTitlesBackground : OptionTitlesBackground,null, 5, 5, 5, 5));
			headerCollapsed.Add (headerPlus);
            			
			headerExpanded.Add (new DrawableRectangle((reply) ? ReplyTitlesBackground : OptionTitlesBackground, null, 5, 5, 0, 0));
			headerExpanded.Add (headerMinus);
           
			#endregion

			InteractiveHeaderedItem tg = new InteractiveHeaderedItem(headerCollapsed, headerExpanded, content);
            
            tg.Collapsed = true;

            //событи€
            tg.HeaderClicked += delegate { tg.Collapsed = !tg.Collapsed; };
			IMouseInteractable interactive = content as IMouseInteractable;
			if (interactive != null)
				tg.ContentClicked += delegate (object sender, PointF pos) {
                    /*interactive.HandleMouseClick(pos);*/ //оригинальное событие
                    foreach (var i in (InteractiveItemsStack)interactive)
				    {
                        if (!(i is InteractiveItemsStack)) continue;
                        foreach (var z in (InteractiveItemsStack)i)
                        {
                            if (z is TextSegment) {
                                string msg = ((TextSegment)z).Text;
                            } 
                        }
				    }
                };
			tg.RedrawNeeded += HandleRedraw;

			return tg;
		}

        protected virtual void PrepareNodeContent ()
		{
			if (dataNode == null) return;
			
            InteractiveItemsStack itemReply;
            InteractiveItemsStack itemOptions;
            DrawableItemsStack<InteractiveHeaderedItem> tileHeader;

            foreach (ContentBody content in dataNode.NodeContent)
            {
                switch (content.type) 
                {
                    case opCodeType.Reply:
                        itemReply = new InteractiveItemsStack();
                        itemReply = PrepareMessageContent(content.msgText, true);

                        tileHeader = new DrawableItemsStack<InteractiveHeaderedItem>();
                        tileHeader.Add(MessageToContent(content.scrText, itemReply, true));

                        items.Add(tileHeader);
                        break;
                    case opCodeType.Options:
                        itemOptions = new InteractiveItemsStack();
                        itemOptions = PrepareMessageContent(content.msgText, false);

                        tileHeader = new DrawableItemsStack<InteractiveHeaderedItem>();
                        tileHeader.Add(MessageToContent(content.scrText, itemOptions, false));

                        items.Add(tileHeader);
                        break;
                    default:
                        items.Add(new TextSegment(Graphics, content.scrText, ScriptFont, true));
                        break;
                }
            }
		}

        protected virtual InteractiveItemsStack PrepareMessageContent(string message, bool reply)
		{
            // пиктограмма
            DrawableItemsStack<VectorShape> image = new DrawableItemsStack<VectorShape>();
            image.OrientationAxis = Axis.X; // stack image components one on top of the other
            image.KeepAspectRatio = true;
            image.Add(new OptionsCircleShape());
            image.Border = 1;
            
            // текст
            InteractiveItemsStack replyText = new InteractiveItemsStack();
			replyText.OrientationAxis = Axis.X;
            replyText.Add(image);
            replyText.Add(new TextSegment(Graphics, string.Format("\"{0}\"", message), MessagesFont, true));
          
            // контейнер
            InteractiveItemsStack content = new InteractiveItemsStack();
			content.OrientationAxis = Axis.Z;
			content.Add(new DrawableRectangle((reply)? ReplyTextTitlesBackground : OptionTextTitlesBackground, null, 0, 0, 5, 5));
            content.Add(replyText);
            
			return content;
		}
        
        private InteractiveHeaderedItem MessageToContent(string title, InteractiveItemsStack content, bool reply)
		{
				InteractiveHeaderedItem headerContent = PrepareMessagesHeader(title, content, reply);
			    //groupNames.Add(headerContent, title);
				//groupsByName.Add(title, headerContent);
                return headerContent;
		}

		protected virtual void PrepareFrame ()
		{
			ActualHeight = classItemContainer.GetAbsoluteContentHeight();

			if (Container != null) return;
			
			shadowpath = new GraphicsPath();
			if (RoundedCorners)
			{
				int radius = CornerRadius;
				shadowpath.AddArc(ActualWidth-radius + 4, 3, radius, radius, 300, 60);
				shadowpath.AddArc(ActualWidth-radius + 4, ActualHeight - radius + 3, radius, radius, 0, 90);
				shadowpath.AddArc(4, ActualHeight-radius + 3, radius, radius, 90, 45);
				shadowpath.AddArc(ActualWidth-radius, ActualHeight - radius, radius, radius, 90, -90);
			}
			else
			{
				shadowpath.AddPolygon(new PointF[] {
				                      	new PointF(ActualWidth, 3),
				                      	new PointF(ActualWidth + 4, 3),
				                      	new PointF(ActualWidth + 4, ActualHeight + 3),
				                      	new PointF(4, ActualHeight + 3),
				                      	new PointF(4, ActualHeight),
				                      	new PointF(ActualWidth, ActualHeight)
				                      });
			}
			shadowpath.CloseFigure();
		}
		#endregion

		// вычисление положени€ точки дл€ начала соедин€ющей линии
        private void OffsetPointTo()
        {
            if (dataNode.LinkedToNodes.Count == 0) return;
            int line = 1;
            float offsetY = (!classItemHeaderedContent.Collapsed) ? titles.GetAbsoluteContentHeight() + 5 : 0;
            foreach (var item in items)
			{
                if (!classItemHeaderedContent.Collapsed) offsetY += item.GetAbsoluteContentHeight() + items.Spacing;
                SetPointTo(line++, offsetY); 
			}
        }
        
        private void SetPointTo(int line, float offset)
        {
            List<LinkTo> pointList = dataNode.LinkedToNodes;
            for (int i = 0; i < pointList.Count; i++)
            {
                if (pointList[i].ContentLine == line) {
                    pointList[i].PointTo = offset;
                    return;
                }
            }
        }

		public override void DrawToGraphics (Graphics graphics)
		{
			grad.ResetTransform();
			grad.TranslateTransform(AbsoluteX, AbsoluteY);
			grad.ScaleTransform(ActualWidth, 1);
			
			GraphicsState state = graphics.Save();
			graphics.TranslateTransform (AbsoluteX, AbsoluteY);
			
			if (Container == null) {
	            //Draw Shadow
				graphics.FillPath(CanvasItem.ShadowBrush, shadowpath);
			}
			
			classItemContainer.Width = Width;
			classItemContainer.Height = Height;
				
			graphics.Restore(state);
			classItemContainer.DrawToGraphics(graphics);
			
			#region Draw interfaces lollipops
			//TODO - should be converted to an headered item.
			/*if (interfaces.Count > 0)
			{
				interfaces.X = AbsoluteX + 15;
				interfaces.Y = AbsoluteY - interfaces.ActualHeight - 1;
				interfaces.DrawToGraphics(graphics);
				
				graphics.DrawEllipse(Pens.Black, AbsoluteX + 9, AbsoluteY - interfaces.ActualHeight - 11, 10, 10);
				graphics.DrawLine(Pens.Black, AbsoluteX + 14, AbsoluteY - interfaces.ActualHeight - 1, AbsoluteX + 14, AbsoluteY);
			}*/
			#endregion

			base.DrawToGraphics(graphics);
		}
		
        #region Behaviour

		public bool Collapsed
		{
			get { return classItemHeaderedContent.Collapsed; }
			set
			{
				classItemHeaderedContent.Collapsed = value;
				collapseExpandShape.Collapsed = value;
				PrepareFrame();
				EmitLayoutUpdate();
			}
		}
		
		private void HandleRedraw (object sender, EventArgs args)
		{
			PrepareFrame();
			EmitLayoutUpdate();
		}
		
		public override void HandleMouseClick (PointF pos)
		{
			base.HandleMouseClick(pos);

			if (collapseExpandShape.IsInside(pos.X, pos.Y)) {
				Collapsed = !Collapsed;
                OffsetPointTo();
			} else {
                foreach (var item in items)
                {
                    if ((item is DrawableItemsStack<InteractiveHeaderedItem>) == false) continue;
                    foreach (InteractiveHeaderedItem header in (DrawableItemsStack<InteractiveHeaderedItem>)item)
                    {
                        if (header.HitTest(pos)) {
                            header.HandleMouseClick(pos);
                            OffsetPointTo();
                        }
                    }
                }
			}
		}
		#endregion
		
		#region Storage
		
		protected override XmlElement CreateXmlElement(XmlDocument doc)
		{
			return doc.CreateElement("Class");
		}
		
		protected override void FillXmlElement(XmlElement element, XmlDocument document)
		{
			base.FillXmlElement(element, document);
			element.SetAttribute("Name", dataNode.Name);
			element.SetAttribute("Collapsed", Collapsed.ToString());
			
			//<Compartments>
			//XmlElement compartments = document.CreateElement("Compartments");
            //foreach (InteractiveHeaderedItem tg in groups)
            //{
            //    XmlElement grp = document.CreateElement("Compartment");
            //    grp.SetAttribute("Name", groupNames[tg]); //groupNames
            //    grp.SetAttribute("Collapsed", tg.Collapsed.ToString());
            //    compartments.AppendChild(grp);
            //}
			//element.AppendChild(compartments);
		}
		
		public override void LoadFromXml (XPathNavigator navigator)
		{
			base.LoadFromXml(navigator);
			
			Collapsed = bool.Parse(navigator.GetAttribute("Collapsed", ""));
			
			XPathNodeIterator compNI = navigator.Select("Compartments/Compartment");
			while (compNI.MoveNext())
			{
				XPathNavigator compNav = compNI.Current;
				InteractiveHeaderedItem grp;
				if (groupsByName.TryGetValue(compNav.GetAttribute("Name", ""), out grp))
				{
					grp.Collapsed = bool.Parse(compNav.GetAttribute("Collapsed", ""));
				}
			}
		}
		#endregion
		
		public void Dispose()
		{
			grad.Dispose();
			if (shadowpath != null)
				shadowpath.Dispose();
		}
		
		public override string ToString()
		{
			return "NodeCanvasItem: " + dataNode.Name;
		}
	}
}
