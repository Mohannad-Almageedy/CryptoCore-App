using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.Controls
{
    /// <summary>
    /// Factory helpers — create consistently styled controls.
    /// Use these everywhere instead of inline styling.
    /// </summary>
    public static class ControlFactory
    {
        // ── Section label ──────────────────────────────────────────
        public static Label SectionLabel(string text)
            => new Label
            {
                Text      = text,
                Font      = AppTheme.FontH3,
                ForeColor = AppTheme.TextSecondary,
                AutoSize  = true,
                Margin    = new Padding(0, 0, 0, 4),
                BackColor = Color.Transparent
            };

        // ── Page title ─────────────────────────────────────────────
        public static Label PageTitle(string text)
            => new Label
            {
                Text      = text,
                Font      = AppTheme.FontH1,
                ForeColor = AppTheme.TextPrimary,
                AutoSize  = true,
                Margin    = new Padding(0, 0, 0, 2),
                BackColor = Color.Transparent
            };

        // ── Subtitle ───────────────────────────────────────────────
        public static Label SubTitle(string text)
            => new Label
            {
                Text      = text,
                Font      = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                AutoSize  = true,
                Margin    = new Padding(0, 0, 0, 16),
                BackColor = Color.Transparent
            };

        // ── Rich text input ───────────────────────────────────────
        public static RichTextBox MultiLineInput(int height = 120)
            => new RichTextBox
            {
                Height      = height,
                Dock        = DockStyle.Fill,
                Font        = AppTheme.FontBody,
                BorderStyle = BorderStyle.None,
                BackColor   = Color.FromArgb(248, 249, 253),
                ForeColor   = AppTheme.TextPrimary,
                ScrollBars  = RichTextBoxScrollBars.Vertical
            };

        // ── Single-line text input ────────────────────────────────
        public static TextBox SingleLineInput(bool readOnly = false)
            => new TextBox
            {
                Height      = 32,
                Dock        = DockStyle.Fill,
                Font        = AppTheme.FontBody,
                BorderStyle = BorderStyle.None,
                BackColor   = Color.FromArgb(248, 249, 253),
                ForeColor   = AppTheme.TextPrimary,
                ReadOnly    = readOnly
            };

        // ── Monospace output ──────────────────────────────────────
        public static RichTextBox MonoOutput(int height = 100)
            => new RichTextBox
            {
                Height      = height,
                Dock        = DockStyle.Fill,
                Font        = new Font("Cascadia Code", 9, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                BackColor   = Color.FromArgb(240, 242, 253),
                ForeColor   = Color.FromArgb(50, 55, 160),
                ReadOnly    = true,
                ScrollBars  = RichTextBoxScrollBars.Vertical
            };

        // ── Step trace output ─────────────────────────────────────
        public static RichTextBox StepTrace()
            => new RichTextBox
            {
                Dock        = DockStyle.Fill,
                Font        = new Font("Cascadia Code", 8),
                BorderStyle = BorderStyle.None,
                BackColor   = Color.FromArgb(248, 249, 253),
                ForeColor   = AppTheme.TextPrimary,
                ReadOnly    = true,
                ScrollBars  = RichTextBoxScrollBars.Vertical
            };

        // ── Pill button ───────────────────────────────────────────
        public static Button Pill(string text, Color bg, Color? fg = null, int width = 0)
        {
            var b = new Button
            {
                Text      = text,
                FlatStyle = FlatStyle.Flat,
                Font      = AppTheme.FontBodyBold,
                BackColor = bg,
                ForeColor = fg ?? Color.White,
                Cursor    = Cursors.Hand,
                Height    = 36,
                AutoSize  = width == 0,
                Width     = width > 0 ? width : 100,
                Padding   = new Padding(14, 0, 14, 0),
                Margin    = new Padding(0, 0, 8, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(bg, 0.12f);
            return b;
        }

        // ── Status label ──────────────────────────────────────────
        public static Label StatusLabel()
            => new Label
            {
                Text      = "Ready",
                Font      = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Margin    = new Padding(4, 8, 0, 0),
                BackColor = Color.Transparent
            };

        // ── Input box with background padding ────────────────────
        public static Panel InputBox(Control inner, int padding = 10, int height = 0)
        {
            var p = new Panel
            {
                BackColor = Color.FromArgb(248, 249, 253),
                Padding   = new Padding(padding),
                Dock      = DockStyle.Fill,
                Height    = height > 0 ? height : inner.Height + padding * 2
            };
            inner.Dock = DockStyle.Fill;
            p.Controls.Add(inner);
            return p;
        }

        // ── Flow row (horizontal buttons) ─────────────────────────
        public static FlowLayoutPanel ButtonRow(params Control[] controls)
        {
            var panel = new FlowLayoutPanel
            {
                FlowDirection   = FlowDirection.LeftToRight,
                AutoSize        = true,
                WrapContents    = false,
                Margin          = new Padding(0, 8, 0, 8),
                BackColor       = Color.Transparent
            };
            panel.Controls.AddRange(controls);
            return panel;
        }

        // ── Card container (rounded look applied via RoundedPanel) ─
        public static RoundedPanel Card()
            => new RoundedPanel
            {
                Dock        = DockStyle.Fill,
                Padding     = new Padding(20),
                ShowShadow  = true
            };
    }
}
