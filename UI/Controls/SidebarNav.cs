using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.Controls
{
    public class NavItem
    {
        public string  Icon    { get; set; } = "";
        public string  Label   { get; set; } = "";
        public string? Badge   { get; set; }
        public Control? Target { get; set; }
    }

    /// <summary>
    /// Dark sidebar navigation panel.  
    /// Handles active-item highlighting, hover animation, and section headers.
    /// </summary>
    public class SidebarNav : Panel
    {
        private readonly List<(Panel item, NavItem nav)> _items = new();
        private Panel? _activeItem;

        public event Action<NavItem>? NavItemClicked;

        public SidebarNav()
        {
            DoubleBuffered = true;
            BackColor      = AppTheme.Sidebar;
            Width          = AppTheme.SidebarWidth;
            Dock           = DockStyle.Left;
            Padding        = new Padding(0);
        }

        /// <summary>Add a section header (non-clickable label).</summary>
        public void AddSection(string title)
        {
            var lbl = new Label
            {
                Text      = title.ToUpperInvariant(),
                Font      = AppTheme.FontBadge,
                ForeColor = Color.FromArgb(80, 170, 175, 200),
                AutoSize  = false,
                Width     = AppTheme.SidebarWidth,
                Height    = 32,
                Padding   = new Padding(16, 12, 0, 0),
                BackColor = Color.Transparent
            };
            Controls.Add(lbl);
        }

        /// <summary>Add a clickable navigation item.</summary>
        public void AddItem(NavItem nav)
        {
            var panel = new Panel
            {
                Width      = AppTheme.SidebarWidth,
                Height     = AppTheme.SidebarItemH,
                BackColor  = Color.Transparent,
                Cursor     = Cursors.Hand,
                Tag        = nav
            };

            // ── indicator bar (hidden until active) ──
            var indicator = new Panel
            {
                Width     = 3,
                Height    = 24,
                BackColor = AppTheme.SidebarActive,
                Location  = new Point(0, (AppTheme.SidebarItemH - 24) / 2),
                Visible   = false
            };

            // ── label ──
            var lbl = new Label
            {
                Text      = $"  {nav.Icon}  {nav.Label}",
                Font      = AppTheme.FontSidebar,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = false,
                Bounds    = new Rectangle(8, 0, AppTheme.SidebarWidth - 50, AppTheme.SidebarItemH),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                Cursor    = Cursors.Hand
            };

            panel.Controls.Add(indicator);
            panel.Controls.Add(lbl);

            // badge
            if (!string.IsNullOrEmpty(nav.Badge))
            {
                var badge = new Label
                {
                    Text      = nav.Badge,
                    Font      = AppTheme.FontBadge,
                    ForeColor = Color.White,
                    BackColor = AppTheme.Accent,
                    AutoSize  = true,
                    Padding   = new Padding(5, 2, 5, 2),
                    Location  = new Point(AppTheme.SidebarWidth - 45, 12),
                    Cursor    = Cursors.Hand
                };
                panel.Controls.Add(badge);
            }

            // hover effects
            void Enter(object? s, EventArgs e)
            {
                if (panel != _activeItem)
                    panel.BackColor = AppTheme.SidebarHover;
            }
            void Leave(object? s, EventArgs e)
            {
                if (panel != _activeItem)
                    panel.BackColor = Color.Transparent;
            }
            void Click(object? s, EventArgs e)
            {
                SetActive(panel, lbl, indicator);
                NavItemClicked?.Invoke(nav);
            }

            panel.MouseEnter += Enter;
            panel.MouseLeave += Leave;
            panel.Click      += Click;
            lbl.MouseEnter   += Enter;
            lbl.MouseLeave   += Leave;
            lbl.Click        += Click;

            foreach (Control c in panel.Controls)
            {
                c.MouseEnter += Enter;
                c.MouseLeave += Leave;
                c.Click      += Click;
            }

            Controls.Add(panel);
            _items.Add((panel, nav));
        }

        public void SelectFirst()
        {
            if (_items.Count == 0) return;
            var (panel, nav) = _items.First();
            var indicator = panel.Controls.OfType<Panel>().FirstOrDefault();
            var lbl       = panel.Controls.OfType<Label>().FirstOrDefault();
            if (lbl != null && indicator != null)
            {
                SetActive(panel, lbl, indicator);
                NavItemClicked?.Invoke(nav);
            }
        }

        private void SetActive(Panel panel, Label lbl, Panel indicator)
        {
            // Reset all
            foreach (var (p, _) in _items)
            {
                p.BackColor = Color.Transparent;
                var ind = p.Controls.OfType<Panel>().FirstOrDefault();
                var lb  = p.Controls.OfType<Label>().FirstOrDefault();
                if (ind != null) ind.Visible   = false;
                if (lb  != null)
                {
                    lb.ForeColor = AppTheme.TextMuted;
                    lb.Font      = AppTheme.FontSidebar;
                }
            }

            // Activate
            panel.BackColor     = Color.FromArgb(20, 99, 102, 241);
            indicator.Visible   = true;
            lbl.ForeColor       = Color.White;
            lbl.Font            = AppTheme.FontSidebarSel;
            _activeItem         = panel;
        }
    }
}
