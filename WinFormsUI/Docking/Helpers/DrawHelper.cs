using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    public static class DrawHelper
    {
        public static Point RtlTransform(Control control, Point point)
        {
            if (control.RightToLeft != RightToLeft.Yes)
                return point;
            else
                return new Point(control.Right - point.X, point.Y);
        }

        public static Rectangle RtlTransform(Control control, Rectangle rectangle)
        {
            if (control.RightToLeft != RightToLeft.Yes)
                return rectangle;
            else
                return new Rectangle(control.ClientRectangle.Right - rectangle.Right, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        /// <summary>Width of the bar <see cref="DrawTabAccent"/> draws.</summary>
        public const int TabAccentWidth = 3;

        /// <summary>
        /// MCS: draws <see cref="DockContentHandler.TabAccentColor"/> of <paramref name="content"/> as a bar along
        /// the leading edge of its document tab; nothing when the colour is empty.
        /// </summary>
        /// <param name="tabRect">The tab, already transformed for right-to-left.</param>
        /// <param name="inset">Distance of the bar from the edge, for tab shapes whose edge is not vertical.</param>
        /// <param name="clip">The tab outline, so the bar cannot leak out of a slanted tab; null for a rectangle.</param>
        /// <remarks>
        /// The bar sits in the gap left of the icon (8 px in both document strips), so the tab layout and the
        /// room for the text stay as they were.
        /// </remarks>
        public static void DrawTabAccent(Graphics g, Control strip, IDockContent content, Rectangle tabRect, int inset, GraphicsPath clip)
        {
            Color color = content.DockHandler.TabAccentColor;
            if (color.IsEmpty)
                return;

            int x = strip.RightToLeft == RightToLeft.Yes
                ? tabRect.Right - inset - TabAccentWidth
                : tabRect.Left + inset;
            var bar = new Rectangle(x, tabRect.Top, TabAccentWidth, tabRect.Height);

            GraphicsState state = g.Save();
            try
            {
                if (clip != null)
                    g.SetClip(clip, CombineMode.Intersect);
                using (var brush = new SolidBrush(color))
                    g.FillRectangle(brush, bar);
            }
            finally
            {
                g.Restore(state);
            }
        }

        /// <summary>
        /// MCS: the entry for <paramref name="content"/> in the document list behind the tab strip's drop-down
        /// button, with the same accent bar as its tab.
        /// </summary>
        /// <remarks>
        /// The list is how an operator finds a document whose tab no longer fits the strip, so it has to show
        /// the mark too; otherwise the marked documents can only be told apart while their tabs are visible.
        /// </remarks>
        public static ToolStripItem CreateWindowListItem(IDockContent content)
        {
            return new AccentMenuItem(content.DockHandler.TabText, content.DockHandler.Icon.ToBitmap(),
                content.DockHandler.TabAccentColor);
        }

        // Paints after the renderer so the bar stays on top of the highlight of the hovered entry.
        private sealed class AccentMenuItem : ToolStripMenuItem
        {
            private readonly Color _accent;

            public AccentMenuItem(string text, Image image, Color accent) : base(text, image)
            {
                _accent = accent;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                if (_accent.IsEmpty)
                    return;

                int x = RightToLeft == RightToLeft.Yes ? Width - TabAccentWidth : 0;
                using (var brush = new SolidBrush(_accent))
                    e.Graphics.FillRectangle(brush, x, 0, TabAccentWidth, Height);
            }
        }

        public static GraphicsPath GetRoundedCornerTab(GraphicsPath graphicsPath, Rectangle rect, bool upCorner)
        {
            if (graphicsPath == null)
                graphicsPath = new GraphicsPath();
            else
                graphicsPath.Reset();
            
            int curveSize = 6;
            if (upCorner)
            {
                graphicsPath.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top + curveSize / 2);
                graphicsPath.AddArc(new Rectangle(rect.Left, rect.Top, curveSize, curveSize), 180, 90);
                graphicsPath.AddLine(rect.Left + curveSize / 2, rect.Top, rect.Right - curveSize / 2, rect.Top);
                graphicsPath.AddArc(new Rectangle(rect.Right - curveSize, rect.Top, curveSize, curveSize), -90, 90);
                graphicsPath.AddLine(rect.Right, rect.Top + curveSize / 2, rect.Right, rect.Bottom);
            }
            else
            {
                graphicsPath.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom - curveSize / 2);
                graphicsPath.AddArc(new Rectangle(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize), 0, 90);
                graphicsPath.AddLine(rect.Right - curveSize / 2, rect.Bottom, rect.Left + curveSize / 2, rect.Bottom);
                graphicsPath.AddArc(new Rectangle(rect.Left, rect.Bottom - curveSize, curveSize, curveSize), 90, 90);
                graphicsPath.AddLine(rect.Left, rect.Bottom - curveSize / 2, rect.Left, rect.Top);
            }

            return graphicsPath;
        }

        public static GraphicsPath CalculateGraphicsPathFromBitmap(Bitmap bitmap)
        {
            return CalculateGraphicsPathFromBitmap(bitmap, Color.Empty);
        }

        // From http://edu.cnzz.cn/show_3281.html
        public static GraphicsPath CalculateGraphicsPathFromBitmap(Bitmap bitmap, Color colorTransparent) 
        { 
            GraphicsPath graphicsPath = new GraphicsPath(); 
            if (colorTransparent == Color.Empty)
                colorTransparent = bitmap.GetPixel(0, 0); 

            for(int row = 0; row < bitmap.Height; row ++) 
            { 
                int colOpaquePixel = 0;
                for(int col = 0; col < bitmap.Width; col ++) 
                { 
                    if(bitmap.GetPixel(col, row) != colorTransparent) 
                    { 
                        colOpaquePixel = col; 
                        int colNext = col; 
                        for(colNext = colOpaquePixel; colNext < bitmap.Width; colNext ++) 
                            if(bitmap.GetPixel(colNext, row) == colorTransparent) 
                                break;
 
                        graphicsPath.AddRectangle(new Rectangle(colOpaquePixel, row, colNext - colOpaquePixel, 1)); 
                        col = colNext; 
                    } 
                } 
            } 
            return graphicsPath; 
        }

        public static int Balance(int length, int margin, int input, int lower, int upper)
        {
            return Max(Min(input, upper - length - margin), lower + margin);
        }

        private static int Min(int one, int other)
        {
            return one > other ? other : one;
        }

        private static int Max(int one, int other)
        {
            return one < other ? other : one;
        }
    }
}
