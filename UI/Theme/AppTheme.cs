using System.Drawing;

namespace CryptoEdu.UI.Theme
{
    /// <summary>
    /// Premium Dark Mode design tokens.
    /// Uses a Slate-inspired palette with vibrant accents.
    /// </summary>
    public static class AppTheme
    {
        // ─── Core dark palette (Slate-inspired) ────────────────────────────────
        public static readonly Color Sidebar        = Color.FromArgb(15, 23, 42);   // Slate-900
        public static readonly Color SidebarHover   = Color.FromArgb(30, 41, 59);   // Slate-800
        public static readonly Color SidebarActive  = Color.FromArgb(99, 102, 241); // Indigo-500
        public static readonly Color Header         = Color.FromArgb(15, 23, 42);   // Slate-900
        public static readonly Color ContentBg      = Color.FromArgb(2, 6, 23);     // Slate-950
        public static readonly Color CardBg         = Color.FromArgb(30, 41, 59);   // Slate-800
        public static readonly Color CardBorder     = Color.FromArgb(51, 65, 85);   // Slate-700
        public static readonly Color Divider        = Color.FromArgb(51, 65, 85);   // Slate-700
        
        // Inputs & outputs
        public static readonly Color InputBg        = Color.FromArgb(15, 23, 42);   // Slate-900
        public static readonly Color OutputBg       = Color.FromArgb(2, 6, 23);     // Slate-950
        public static readonly Color OutputText     = Color.FromArgb(129, 140, 248); // Indigo-400
        public static readonly Color InputWrapBg    = Color.FromArgb(15, 23, 42);   // Slate-900

        // Buttons & Chips
        public static readonly Color ButtonMutedBg  = Color.FromArgb(51, 65, 85);   // Slate-700
        public static readonly Color ButtonMutedFg  = Color.FromArgb(241, 245, 249);// Slate-100
        public static readonly Color ChipBg         = Color.FromArgb(30, 41, 59);   // Slate-800
        public static readonly Color ChipHoverBg    = Color.FromArgb(51, 65, 85);   // Slate-700

        // Spec alerts
        public static readonly Color AlertSuccessBg = Color.FromArgb(6, 78, 59);    // Emerald-900 
        public static readonly Color AlertDangerBg  = Color.FromArgb(127, 29, 29);  // Red-900

        // ─── Accent / brand ────────────────────────────────────────────────────
        public static readonly Color Accent         = Color.FromArgb(99, 102, 241); // Indigo-500
        public static readonly Color AccentHover    = Color.FromArgb(79, 70, 229);  // Indigo-600
        public static readonly Color AccentSuccess  = Color.FromArgb(16, 185, 129); // Emerald-500
        public static readonly Color AccentWarning  = Color.FromArgb(245, 158, 11); // Amber-500
        public static readonly Color AccentDanger   = Color.FromArgb(239, 68, 68);  // Red-500
        public static readonly Color AccentInfo     = Color.FromArgb(14, 165, 233); // Sky-500

        // ─── Text ──────────────────────────────────────────────────────────────
        public static readonly Color TextPrimary    = Color.FromArgb(241, 245, 249);// Slate-100
        public static readonly Color TextSecondary  = Color.FromArgb(160, 174, 192);// Slate-400
        public static readonly Color TextMuted      = Color.FromArgb(100, 116, 139);// Slate-500
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
        public const int SidebarWidth    = 240;
        public const int HeaderHeight    = 58;
        public const int CornerRadius    = 10;
        public const int CardPadding     = 20;
        public const int SidebarItemH   = 44;
    }
}
