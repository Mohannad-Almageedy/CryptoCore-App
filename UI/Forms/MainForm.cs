using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;
using CryptoEdu.UI.UserControls;

namespace CryptoEdu.UI.Forms
{
    /// <summary>
    /// Application shell.
    /// Layout: sidebar (left) | main area (right)
    ///         - header bar on top of main area
    ///         - content panel that swaps child views
    /// </summary>
    public class MainForm : Form
    {
        private Panel          _contentArea  = null!;
        private Label          _lblPageTitle = null!;
        private Label          _lblPageSub   = null!;

        public MainForm()
        {
            SetupForm();
            BuildLayout();
        }

        // ───────────────────────────────────────── form settings ───
        private void SetupForm()
        {
            Text            = "CryptoEdu";
            Size            = new Size(1280, 820);
            MinimumSize     = new Size(1100, 700);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = AppTheme.ContentBg;
            Font            = AppTheme.FontBody;

            // Remove default title bar so we can draw a fully custom header
            FormBorderStyle = FormBorderStyle.None;
            // Allow dragging the form by the header area
        }

        // ───────────────────────────────────────── layout ── ────────
        private void BuildLayout()
        {
            // ── 1. Sidebar ───────────────────────────────────────────
            var sidebar = new SidebarNav();

            // Logo area
            var logoPanel = new Panel
            {
                Height    = 68,
                Width     = AppTheme.SidebarWidth,
                BackColor = Color.Transparent
            };
            var logoIcon = new Label
            {
                Text      = "🔐",
                Font      = new Font("Segoe UI", 18),
                ForeColor = Color.White,
                AutoSize  = true,
                Location  = new Point(16, 18)
            };
            var logoText = new Label
            {
                Text      = "CryptoEdu",
                Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize  = true,
                Location  = new Point(50, 20)
            };
            var logoSub = new Label
            {
                Text      = "Cryptography Suite",
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(120, 200, 200, 200),
                AutoSize  = true,
                Location  = new Point(50, 38)
            };
            logoPanel.Controls.AddRange(new Control[] { logoIcon, logoText, logoSub });
            sidebar.Controls.Add(logoPanel);

            // Divider
            sidebar.Controls.Add(MakeDivider());

            // Nav items
            sidebar.AddSection("Education");
            sidebar.AddItem(new NavItem { Icon = "◈", Label = "Classical Ciphers" });

            sidebar.AddSection("Professional");
            sidebar.AddItem(new NavItem { Icon = "⬡", Label = "Modern Encryption" });
            sidebar.AddItem(new NavItem { Icon = "⬢", Label = "File Encryption" });
            sidebar.AddItem(new NavItem { Icon = "#", Label = "Hashing" });

            sidebar.AddSection("Tools");
            sidebar.AddItem(new NavItem { Icon = "⊕", Label = "Key Generator" });
            sidebar.AddItem(new NavItem { Icon = "≡", Label = "History" });

            // Divider bottom
            sidebar.Controls.Add(MakeDivider());

            // Version info
            var verLabel = new Label
            {
                Text      = "v1.0.0   •   .NET 8",
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(70, 200, 200, 220),
                AutoSize  = false,
                Width     = AppTheme.SidebarWidth,
                Height    = 28,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Padding   = new Padding(0, 5, 0, 0)
            };
            sidebar.Controls.Add(verLabel);

            // ── 2. Right panel ───────────────────────────────────────
            var rightPanel = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = AppTheme.ContentBg
            };

            // ── 3. Header bar ────────────────────────────────────────
            var header = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = AppTheme.HeaderHeight,
                BackColor = AppTheme.Header
            };

            // Dragging support via header
            bool _dragging = false;
            Point _dragStart = Point.Empty;
            header.MouseDown += (s, e) => { _dragging = true;  _dragStart = e.Location; };
            header.MouseMove += (s, e) => { if (_dragging) Location = new Point(Location.X + e.X - _dragStart.X, Location.Y + e.Y - _dragStart.Y); };
            header.MouseUp   += (s, e) => { _dragging = false; };

            _lblPageTitle = new Label
            {
                Text      = "Classical Ciphers",
                Font      = AppTheme.FontH2,
                ForeColor = Color.White,
                AutoSize  = true,
                Location  = new Point(20, 10)
            };
            _lblPageSub = new Label
            {
                Text      = "Educational encryption with step-by-step tracing",
                Font      = AppTheme.FontSmall,
                ForeColor = Color.FromArgb(160, 200, 210, 255),
                AutoSize  = true,
                Location  = new Point(21, 32)
            };

            // Window control buttons (close / maximize / minimize)
            var btnClose = MakeWindowBtn("✕", Color.FromArgb(239, 68, 68));
            var btnMax   = MakeWindowBtn("□", Color.FromArgb(250, 204, 21));
            var btnMin   = MakeWindowBtn("─", Color.FromArgb(34, 197, 94));
            btnClose.Location = new Point(header.Width - 40,  14);
            btnMax.Location   = new Point(header.Width - 80,  14);
            btnMin.Location   = new Point(header.Width - 120, 14);
            btnClose.Anchor   = AnchorStyles.Top | AnchorStyles.Right;
            btnMax.Anchor     = AnchorStyles.Top | AnchorStyles.Right;
            btnMin.Anchor     = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Click   += (s, e) => Application.Exit();
            btnMax.Click     += (s, e) => WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            btnMin.Click     += (s, e) => WindowState = FormWindowState.Minimized;

            header.Controls.AddRange(new Control[] { _lblPageTitle, _lblPageSub, btnClose, btnMax, btnMin });

            // ── 4. Content area ──────────────────────────────────────
            _contentArea = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = AppTheme.ContentBg,
                Padding   = new Padding(20, 16, 20, 20)
            };

            rightPanel.Controls.Add(_contentArea);
            rightPanel.Controls.Add(header);

            // ── 5. Assemble ──────────────────────────────────────────
            Controls.Add(rightPanel);
            Controls.Add(sidebar);

            // Wire navigation
            var panels = new Control?[]
            {
                new ClassicalCipherPanel(),
                new ModernCipherPanel(),
                new FileEncryptionPanel(),
                new HashingPanel(),
                new KeyGeneratorPanel(),
                new HistoryPanel()
            };

            var pageInfo = new (string title, string sub)[]
            {
                ("Classical Ciphers",  "Educational encryption with step-by-step tracing"),
                ("Modern Encryption",  "AES-256 · 3DES · RSA  — Professional grade algorithms"),
                ("File Encryption",    "Securely encrypt any file using AES-256 streaming"),
                ("Hashing",            "One-way cryptographic hash functions · MD5 · SHA · HMAC"),
                ("Key Generator",      "Generate secure passwords and RSA key pairs"),
                ("History",            "Session log of all cryptographic operations")
            };

            int idx = 0;
            sidebar.NavItemClicked += nav =>
            {
                string[] labels = { "Classical Ciphers", "Modern Encryption", "File Encryption", "Hashing", "Key Generator", "History" };
                idx = Array.IndexOf(labels, nav.Label);
                if (idx < 0) return;

                _contentArea.Controls.Clear();
                if (panels[idx] != null)
                {
                    panels[idx]!.Dock = DockStyle.Fill;
                    _contentArea.Controls.Add(panels[idx]);
                }

                _lblPageTitle.Text = pageInfo[idx].title;
                _lblPageSub.Text   = pageInfo[idx].sub;
            };

            sidebar.SelectFirst();
        }

        // ── helpers ─────────────────────────────────────────────────
        private static Panel MakeDivider() => new Panel
        {
            Height    = 1,
            Width     = AppTheme.SidebarWidth - 32,
            BackColor = Color.FromArgb(35, 50, 60),
            Margin    = new Padding(16, 4, 16, 4)
        };

        private static Button MakeWindowBtn(string text, Color hover)
        {
            var b = new Button
            {
                Text      = text,
                Size      = new Size(26, 26),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(180, 200, 220),
                BackColor = Color.Transparent,
                Font      = new Font("Segoe UI", 8),
                Cursor    = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = hover;
            return b;
        }
    }
}
