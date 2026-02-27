using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.Controls
{
    /// <summary>
    /// A Panel with rounded corners and an optional shadow / border.
    /// Drop it anywhere as a card container.
    /// </summary>
    public class RoundedPanel : Panel
    {
        private int   _radius      = AppTheme.CornerRadius;
        private bool  _showBorder  = true;
        private bool  _showShadow  = true;
        private Color _borderColor = AppTheme.CardBorder;

        public int   Radius      { get => _radius;      set { _radius      = value; Invalidate(); } }
        public bool  ShowBorder  { get => _showBorder;  set { _showBorder  = value; Invalidate(); } }
        public bool  ShowShadow  { get => _showShadow;  set { _showShadow  = value; Invalidate(); } }
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        public RoundedPanel()
        {
            DoubleBuffered = true;
            BackColor = AppTheme.CardBg;
            Padding = new Padding(AppTheme.CardPadding);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect  = new Rectangle(2, 2, Width - 5, Height - 5);
            using var path = RoundedRect(rect, _radius);

            if (_showShadow)
            {
                using var shadowPen = new Pen(Color.FromArgb(18, 0, 0, 0), 4);
                var shadowRect = new Rectangle(3, 4, Width - 6, Height - 6);
                using var shadowPath = RoundedRect(shadowRect, _radius);
                e.Graphics.DrawPath(shadowPen, shadowPath);
            }

            using (var brush = new SolidBrush(BackColor))
                e.Graphics.FillPath(brush, path);

            if (_showBorder)
            {
                using var pen = new Pen(_borderColor, 1);
                e.Graphics.DrawPath(pen, path);
            }

            base.OnPaint(e);
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0,   90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
