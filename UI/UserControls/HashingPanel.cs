using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Services;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.UserControls
{
    public class HashingPanel : UserControl
    {
        public HashingPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = AppTheme.ContentBg;
            BuildUI();
        }

        private void BuildUI()
        {
            var card = new RoundedPanel { Dock = DockStyle.Fill };
            var c = card;

            // Header
            var lbl = new Label { Text = "Cryptographic Hash Functions", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(16, 12) };
            var sub = new Label { Text = "One-way transformation · Cannot be reversed or decrypted", Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 40) };

            // Input
            var lblIn = new Label { Text = "Input Text", Font = AppTheme.FontH3, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 78) };
            var txtIn = new RichTextBox { Bounds = new Rectangle(16, 104, 780, 100), Font = AppTheme.FontBody, BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(248, 249, 255) };

            // HMAC Key
            var lblHK = new Label { Text = "HMAC Secret Key (only required for HMAC-SHA256)", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(16, 218) };
            var txtHK = new TextBox { Bounds = new Rectangle(16, 238, 380, 30), Font = AppTheme.FontBody, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 249, 255) };

            // Algorithm buttons row
            int bx = 16, by = 284;
            var btnMD5    = MakePill("MD5",          Color.FromArgb(234, 67, 53),   new Point(bx, by));
            bx += btnMD5.PreferredSize.Width + 8;
            var btnSHA1   = MakePill("SHA-1",        Color.FromArgb(250, 144, 30),  new Point(bx, by));
            bx += btnSHA1.PreferredSize.Width + 8;
            var btnSHA256 = MakePill("SHA-256",      AppTheme.Accent,               new Point(bx, by));
            bx += btnSHA256.PreferredSize.Width + 8;
            var btnSHA512 = MakePill("SHA-512",      Color.FromArgb(30, 136, 229),  new Point(bx, by));
            bx += btnSHA512.PreferredSize.Width + 8;
            var btnHMAC   = MakePill("HMAC-SHA256",  Color.FromArgb(0, 150, 136),   new Point(bx, by));

            // Status
            var lblStat = new Label { Text = "", Font = AppTheme.FontSmall, ForeColor = AppTheme.AccentSuccess, AutoSize = true, Location = new Point(16, 332) };

            // Output card
            var lblOut = new Label { Text = "Hash Result (Hexadecimal)", Font = AppTheme.FontH3, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 350) };
            var txtOut = new TextBox
            {
                Bounds = new Rectangle(16, 376, 780, 42), Font = new Font("Cascadia Code", 11, FontStyle.Bold),
                BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(240, 242, 255),
                ForeColor = AppTheme.Accent, ReadOnly = true
            };
            var btnCopy = MakePill("⎘  Copy", Color.FromArgb(238, 239, 255), new Point(16, 430), AppTheme.Accent);
            btnCopy.Click += (s, e) => ClipboardService.CopyToClipboard(txtOut.Text);

            // Handlers
            btnMD5.Click    += (s, e) => Run(() => HashingService.ComputeMD5(txtIn.Text),           txtOut, lblStat, "MD5");
            btnSHA1.Click   += (s, e) => Run(() => HashingService.ComputeSHA1(txtIn.Text),          txtOut, lblStat, "SHA-1");
            btnSHA256.Click += (s, e) => Run(() => HashingService.ComputeSHA256(txtIn.Text),        txtOut, lblStat, "SHA-256");
            btnSHA512.Click += (s, e) => Run(() => HashingService.ComputeSHA512(txtIn.Text),        txtOut, lblStat, "SHA-512");
            btnHMAC.Click   += (s, e) => Run(() => HashingService.ComputeHMACSHA256(txtIn.Text, txtHK.Text), txtOut, lblStat, "HMAC-SHA256");

            c.Controls.AddRange(new Control[] { lbl, sub, lblIn, txtIn, lblHK, txtHK, btnMD5, btnSHA1, btnSHA256, btnSHA512, btnHMAC, lblStat, lblOut, txtOut, btnCopy });
            Controls.Add(card);
        }

        private static void Run(Func<string> fn, TextBox output, Label status, string algo)
        {
            try
            {
                output.Text = fn();
                status.Text = $"{algo} computed ✓";
                status.ForeColor = AppTheme.AccentSuccess;
                HistoryService.LogOperation("Hash", algo, "OK");
            }
            catch (Exception ex)
            {
                status.Text = ex.Message;
                status.ForeColor = AppTheme.AccentDanger;
            }
        }

        private static Button MakePill(string text, Color bg, Point loc, Color? fg = null)
        {
            var b = new Button
            {
                Text = text, Location = loc, AutoSize = true, FlatStyle = FlatStyle.Flat,
                BackColor = bg, ForeColor = fg ?? Color.White, Font = AppTheme.FontBodyBold,
                Cursor = Cursors.Hand, Height = 34, Padding = new Padding(12, 0, 12, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(bg, 0.1f);
            return b;
        }
    }
}
