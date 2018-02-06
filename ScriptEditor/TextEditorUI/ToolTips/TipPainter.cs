﻿using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScriptEditor.TextEditorUI.ToolTips
{
    static class TipPainter
    {
        // Specify custom text formatting flags
        static TextFormatFlags sf = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;

        public static void DrawMessage(DrawToolTipEventArgs e)
        {
            LinearGradientBrush gradientDefault = new LinearGradientBrush(e.Bounds,
                                                        Color.White, Color.Lavender,
                                                        LinearGradientMode.Vertical);

            // Draw the custom background
            e.Graphics.FillRectangle(gradientDefault, e.Bounds);

            // Draw the custom border to appear 3-dimensional
            e.Graphics.DrawLines(new Pen(Color.Gray), new Point[] {
                new Point (0, e.Bounds.Height - 1), 
                new Point (e.Bounds.Width - 1, e.Bounds.Height - 1), 
                new Point (e.Bounds.Width - 1, 0)
            });
            e.Graphics.DrawLines(new Pen(Color.DarkGray), new Point[] {
                new Point (0, e.Bounds.Height - 1), 
                new Point (0, 0), 
                new Point (e.Bounds.Width - 1, 0)
            });

            // Draw the standard text with customized formatting options
            e.DrawText(sf);
        }

        public static void DrawInfo(DrawToolTipEventArgs e)
        {
            LinearGradientBrush gradientInfo = new LinearGradientBrush(e.Bounds,
                                                    Color.White, Color.FromArgb(255, 245, 190),
                                                    LinearGradientMode.Vertical);
            // Draw the custom background
            e.Graphics.FillRectangle(gradientInfo, e.Bounds);
            e.DrawBorder();

            // Draw the standard text with customized formatting options
            e.DrawText(sf);
        }

    }
}