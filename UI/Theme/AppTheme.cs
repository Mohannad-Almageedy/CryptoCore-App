using System.Drawing;

namespace CryptoEdu.UI.Theme
{
    /// <summary>
    /// Centralised design token repository.
    /// All colours, fonts and sizes are defined here — never inline.
    /// </summary>
    public static class AppTheme
    {
        // ─── Core palette ──────────────────────────────────────────────────────
        public static readonly Color Sidebar        = Color.FromArgb(15, 17, 26);
        public static readonly Color SidebarHover   = Color.FromArgb(30, 33, 50);
        public static readonly Color SidebarActive  = Color.FromArgb(99, 102, 241);   // Indigo-500
        public static readonly Color Header         = Color.FromArgb(21, 24, 38);
        public static readonly Color ContentBg      = Color.FromArgb(248, 249, 255);
        public static readonly Color CardBg         = Color.White;
        public static readonly Color CardBorder     = Color.FromArgb(230, 232, 245);

        // ─── Accent / brand ────────────────────────────────────────────────────
        public static readonly Color Accent         = Color.FromArgb(99, 102, 241);   // Indigo
        public static readonly Color AccentHover    = Color.FromArgb(79,  82, 221);
        public static readonly Color AccentSuccess  = Color.FromArgb(34, 197,  94);   // Green
        public static readonly Color AccentWarning  = Color.FromArgb(251, 191,  36);  // Amber
        public static readonly Color AccentDanger   = Color.FromArgb(239,  68,  68);  // Red
        public static readonly Color AccentInfo     = Color.FromArgb( 56, 189, 248);  // Sky

        // ─── Text ──────────────────────────────────────────────────────────────
        public static readonly Color TextPrimary    = Color.FromArgb( 15,  17,  26);
        public static readonly Color TextSecondary  = Color.FromArgb(100, 106, 140);
        public static readonly Color TextMuted      = Color.FromArgb(170, 175, 200);
        public static readonly Color TextOnDark     = Color.White;
        public static readonly Color TextOnAccent   = Color.White;

        // ─── Fonts ─────────────────────────────────────────────────────────────
        public static readonly Font  FontTitle      = new Font("Segoe UI", 22, FontStyle.Bold);
        public static readonly Font  FontH1         = new Font("Segoe UI", 18, FontStyle.Bold);
        public static readonly Font  FontH2         = new Font("Segoe UI", 14, FontStyle.Bold);
        public static readonly Font  FontH3         = new Font("Segoe UI", 11, FontStyle.Bold);
        public static readonly Font  FontBody       = new Font("Segoe UI",  9, FontStyle.Regular);
        public static readonly Font  FontBodyBold   = new Font("Segoe UI",  9, FontStyle.Bold);
        public static readonly Font  FontSmall      = new Font("Segoe UI",  8, FontStyle.Regular);
        public static readonly Font  FontMono       = new Font("Cascadia Code", 9, FontStyle.Regular);
        public static readonly Font  FontMonoSmall  = new Font("Cascadia Code", 8, FontStyle.Regular);
        public static readonly Font  FontSidebar    = new Font("Segoe UI",  9, FontStyle.Regular);
        public static readonly Font  FontSidebarSel = new Font("Segoe UI",  9, FontStyle.Bold);
        public static readonly Font  FontBadge      = new Font("Segoe UI",  7, FontStyle.Bold);

        // ─── Sizes ─────────────────────────────────────────────────────────────
        public const int SidebarWidth    = 220;
        public const int HeaderHeight    = 58;
        public const int CornerRadius    = 10;
        public const int CardPadding     = 20;
        public const int SidebarItemH   = 44;
    }
}
