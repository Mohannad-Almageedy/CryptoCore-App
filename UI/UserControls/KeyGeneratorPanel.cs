using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Services;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.UserControls
{
    public class KeyGeneratorPanel : UserControl
    {
        public KeyGeneratorPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = AppTheme.ContentBg;
            BuildUI();
        }

        private void BuildUI()
        {
            // ── Password Generator Card ───────────────────────────────
            var pwCard = new RoundedPanel { Bounds = new Rectangle(0, 0, 780, 240), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            var pwTitle = new Label { Text = "Password Generator", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(16, 12) };
            var pwSub   = new Label { Text = "Cryptographically-secure random passwords", Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 40) };

            var lblLen = new Label { Text = "Length:", Font = AppTheme.FontH3, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 78) };
            var numLen = new NumericUpDown { Minimum = 6, Maximum = 128, Value = 20, Location = new Point(80, 74), Width = 70, Font = AppTheme.FontBody };
            var chkSp  = new CheckBox { Text = "Include special characters  (!@#$&*)", Location = new Point(160, 76), AutoSize = true, Checked = true, Font = AppTheme.FontBody };

            var btnGen = MakePill("⟳  Generate", AppTheme.Accent, new Point(16, 116));

            var txtPw = new TextBox
            {
                Bounds = new Rectangle(16, 162, 640, 38),
                Font = new Font("Cascadia Code", 14, FontStyle.Bold),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(248, 249, 255),
                ReadOnly = true,
                TextAlign = HorizontalAlignment.Center,
                ForeColor = AppTheme.Accent
            };
            var btnCopyPw = MakePill("⎘ Copy", Color.FromArgb(238, 239, 255), new Point(666, 166), AppTheme.Accent);
            btnCopyPw.Click += (s, e) => ClipboardService.CopyToClipboard(txtPw.Text);
            btnGen.Click    += (s, e) => txtPw.Text = KeyGeneratorService.GenerateSecurePassword((int)numLen.Value, chkSp.Checked);

            pwCard.Controls.AddRange(new Control[] { pwTitle, pwSub, lblLen, numLen, chkSp, btnGen, txtPw, btnCopyPw });

            // ── RSA Key Pair Card ─────────────────────────────────────
            var rsaCard = new RoundedPanel { Bounds = new Rectangle(0, 252, 780, 400), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };

            var rsaTitle = new Label { Text = "RSA 2048-bit Key Pair", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(16, 12) };
            var rsaSub   = new Label { Text = "Asymmetric key pair generation for RSA encryption/decryption", Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 40) };

            var btnGenRsa = MakePill("⟳  Generate New Key Pair", AppTheme.Accent, new Point(16, 74));

            var lblPub = new Label { Text = "Public Key (share this):", Font = AppTheme.FontH3, ForeColor = Color.FromArgb(34, 150, 94), AutoSize = true, Location = new Point(16, 120) };
            var txtPub = new RichTextBox { Bounds = new Rectangle(16, 144, 370, 90), Font = new Font("Cascadia Code", 7), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(240, 255, 248), ReadOnly = true };

            var lblPrv = new Label { Text = "Private Key (keep secret!):", Font = AppTheme.FontH3, ForeColor = AppTheme.AccentDanger, AutoSize = true, Location = new Point(400, 120) };
            var txtPrv = new RichTextBox { Bounds = new Rectangle(400, 144, 370, 90), Font = new Font("Cascadia Code", 7), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(255, 242, 242), ReadOnly = true };

            var btnCpPub = MakePill("⎘ Copy Public",  Color.FromArgb(238, 255, 248), new Point(16, 248),  Color.FromArgb(34, 150, 94));
            var btnCpPrv = MakePill("⎘ Copy Private", Color.FromArgb(255, 238, 238), new Point(200, 248), AppTheme.AccentDanger);

            btnGenRsa.Click += (s, e) => { KeyGeneratorService.GenerateRsaKeyPair(out string pub, out string prv); txtPub.Text = pub; txtPrv.Text = prv; HistoryService.LogOperation("KeyGen", "RSA", "2048-bit pair generated."); };
            btnCpPub.Click  += (s, e) => ClipboardService.CopyToClipboard(txtPub.Text);
            btnCpPrv.Click  += (s, e) => ClipboardService.CopyToClipboard(txtPrv.Text);

            rsaCard.Controls.AddRange(new Control[] { rsaTitle, rsaSub, btnGenRsa, lblPub, txtPub, lblPrv, txtPrv, btnCpPub, btnCpPrv });

            Controls.AddRange(new Control[] { rsaCard, pwCard });
        }

        private static Button MakePill(string text, Color bg, Point loc, Color? fg = null)
        {
            var b = new Button { Text = text, Location = loc, AutoSize = true, FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = fg ?? Color.White, Font = AppTheme.FontBodyBold, Cursor = Cursors.Hand, Height = 34, Padding = new Padding(12, 0, 12, 0) };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(bg, 0.1f);
            return b;
        }
    }
}
