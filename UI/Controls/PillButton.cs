using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.Controls
{
    public enum ButtonVariant { Primary, Secondary, Danger, Success, Ghost }

    /// <summary>
    /// A smooth pill-shaped button with hover/press animation and variant colours.
    /// </summary>
    public class PillButton : Button
    {
        private Color   _normalBg;
        private Color   _hoverBg;
        private Color   _pressBg;
        private Color   _fg;
        private Color   _currentBg;
        private bool    _hovered;
        private ButtonVariant _variant = ButtonVariant.Primary;

        public ButtonVariant Variant
        {
            get => _variant;
            set { _variant = value; ApplyVariant(); Invalidate(); }
        }

        public PillButton()
        {
            FlatStyle    = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            UseVisualStyleBackColor = false;
            Height  = 38;
            Padding = new Padding(18, 0, 18, 0);
            Font    = AppTheme.FontBodyBold;
            Cursor  = Cursors.Hand;
            ApplyVariant();
        }

        private void ApplyVariant()
        {
            switch (_variant)
            {
                case ButtonVariant.Primary:
                    _normalBg = AppTheme.Accent;
                    _hoverBg  = AppTheme.AccentHover;
                    _pressBg  = Color.FromArgb(60, 63, 200);
                    _fg       = Color.White;
                    break;
                case ButtonVariant.Secondary:
                    _normalBg = Color.FromArgb(238, 239, 255);
                    _hoverBg  = Color.FromArgb(224, 225, 255);
                    _pressBg  = Color.FromArgb(210, 212, 255);
                    _fg       = AppTheme.Accent;
                    break;
                case ButtonVariant.Danger:
                    _normalBg = AppTheme.AccentDanger;
                    _hoverBg  = Color.FromArgb(220, 50, 50);
                    _pressBg  = Color.FromArgb(200, 35, 35);
                    _fg       = Color.White;
                    break;
                case ButtonVariant.Success:
                    _normalBg = AppTheme.AccentSuccess;
                    _hoverBg  = Color.FromArgb(22, 170, 75);
                    _pressBg  = Color.FromArgb(15, 145, 60);
                    _fg       = Color.White;
                    break;
                case ButtonVariant.Ghost:
                    _normalBg = Color.Transparent;
                    _hoverBg  = Color.FromArgb(20, 99, 102, 241);
                    _pressBg  = Color.FromArgb(35, 99, 102, 241);
                    _fg       = AppTheme.Accent;
                    break;
            }
            _currentBg = _normalBg;
            ForeColor  = _fg;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hovered = true; _currentBg = _hoverBg; Invalidate(); base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            _hovered = false; _currentBg = _normalBg; Invalidate(); base.OnMouseLeave(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _currentBg = _pressBg; Invalidate(); base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _currentBg = _hovered ? _hoverBg : _normalBg; Invalidate(); base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int r    = Height / 2;
            using var path = RoundedPath(rect, r);
            using (var brush = new SolidBrush(_currentBg))
                e.Graphics.FillPath(brush, path);

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            using (var brush = new SolidBrush(ForeColor))
                e.Graphics.DrawString(Text, Font, brush, rect, sf);
        }

        private static GraphicsPath RoundedPath(Rectangle b, int r)
        {
            var p = new GraphicsPath();
            p.AddArc(b.X, b.Y, r * 2, r * 2, 180, 90);
            p.AddArc(b.Right - r * 2, b.Y, r * 2, r * 2, 270, 90);
            p.AddArc(b.Right - r * 2, b.Bottom - r * 2, r * 2, r * 2, 0, 90);
            p.AddArc(b.X, b.Bottom - r * 2, r * 2, r * 2, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
