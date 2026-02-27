using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.Controls
{
    public class RoundedPanel : Panel
    {
        public bool ShowShadow { get; set; } = true;

        public RoundedPanel()
        {
            DoubleBuffered = true;
            BackColor = AppTheme.CardBg;
            Padding = new Padding(20);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Parent.BackColor); // paint transparently over parent

            int r = AppTheme.CornerRadius;
            var w = Width - 1;
            var h = Height - 1;

            using var path = new GraphicsPath();
            path.AddArc(0, 0, r, r, 180, 90);
            path.AddArc(w - r, 0, r, r, 270, 90);
            path.AddArc(w - r, h - r, r, r, 0, 90);
            path.AddArc(0, h - r, r, r, 90, 90);
            path.CloseFigure();

            // Draw shadow fake outline
            if (ShowShadow)
            {
                using var pS = new Pen(Color.FromArgb(10, 0, 0, 0), 3);
                e.Graphics.DrawPath(pS, path);
            }

            // Fill card
            using var brush = new SolidBrush(BackColor);
            e.Graphics.FillPath(brush, path);

            // Border for dark mode (makes cards pop against contentbg)
            using var pOutline = new Pen(AppTheme.CardBorder, 1);
            e.Graphics.DrawPath(pOutline, path);
        }
    }
}
