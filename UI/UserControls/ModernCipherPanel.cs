using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Services;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.UserControls
{
    public class ModernCipherPanel : UserControl
    {
        private readonly Core.Modern.AesCipher _aes = new();
        private readonly Core.Modern.DesCipher _des = new();
        private readonly Core.Modern.RsaCipher _rsa = new();

        public ModernCipherPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = AppTheme.ContentBg;
            BuildUI();
        }

        private void BuildUI()
        {
            var tabs = MakeTabBar(out var content);

            // Tab pages
            var pgAes  = BuildSymmetric("AES-256 · CBC Mode", _aes,
                "Military-grade symmetric encryption.", "AES");
            var pgDes  = BuildSymmetric("Triple DES · CBC Mode", _des,
                "Legacy 3-key DES cipher (educational).", "3DES");
            var pgRsa  = BuildRsa();

            tabs.Controls[0].Tag = pgAes;
            tabs.Controls[1].Tag = pgDes;
            tabs.Controls[2].Tag = pgRsa;

            // Wire first tab
            content.Controls.Add(pgAes);

            Controls.Add(content);
            Controls.Add(tabs);
        }

        private Panel MakeTabBar(out Panel content)
        {
            content = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.ContentBg };
            var localContent = content; // capture for lambda

            var bar = new Panel
            {
                Dock = DockStyle.Top, Height = 48, BackColor = AppTheme.ContentBg
            };

            string[] labels = { "AES-256", "Triple DES", "RSA" };
            Button? active = null;

            for (int i = 0; i < labels.Length; i++)
            {
                var btn = new Button
                {
                    Text      = labels[i],
                    FlatStyle = FlatStyle.Flat,
                    Font      = AppTheme.FontBodyBold,
                    Height    = 34,
                    Width     = 120,
                    Left      = i * 128,
                    Top       = 7,
                    BackColor = Color.FromArgb(238, 239, 255),
                    ForeColor = AppTheme.TextSecondary,
                    Cursor    = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                bar.Controls.Add(btn);

                var capturedBtn = btn;
                btn.Click += (s, e) =>
                {
                    foreach (Button b in bar.Controls) { b.BackColor = Color.FromArgb(238, 239, 255); b.ForeColor = AppTheme.TextSecondary; }
                    capturedBtn.BackColor = AppTheme.Accent;
                    capturedBtn.ForeColor = Color.White;
                    active = capturedBtn;

                    localContent.Controls.Clear();
                    if (capturedBtn.Tag is Control ct) { ct.Dock = DockStyle.Fill; localContent.Controls.Add(ct); }
                };

                if (i == 0) { btn.BackColor = AppTheme.Accent; btn.ForeColor = Color.White; active = btn; }
            }

            return bar;
        }

        private Panel BuildSymmetric(string title, Core.Interfaces.ICipher cipher, string desc, string histLabel)
        {
            var card = new RoundedPanel { Dock = DockStyle.Fill };
            var p = card;

            var lbl = new Label { Text = title, Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(16, 12) };
            var sub = new Label { Text = desc, Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 38) };

            // Input
            var lblIn = new Label { Text = "Plaintext / Ciphertext", Font = AppTheme.FontH3, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 72) };
            var txtIn = new RichTextBox { Bounds = new Rectangle(16, 96, 580, 120), Font = AppTheme.FontBody, BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(248, 249, 255) };

            // Key
            var lblK = new Label { Text = "Password / Secret Key", Font = AppTheme.FontH3, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 230) };
            var txtK = new TextBox { Bounds = new Rectangle(16, 254, 400, 32), Font = AppTheme.FontBody, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 249, 255) };
            var btnGenK = MakePill("⟳ Generate", AppTheme.AccentInfo, new Point(424, 254));
            btnGenK.Click += (s, e) => txtK.Text = KeyGeneratorService.GenerateSecurePassword(16);

            // Buttons
            var btnEnc = MakePill("▶  Encrypt", AppTheme.Accent, new Point(16, 304));
            var btnDec = MakePill("◀  Decrypt", AppTheme.AccentSuccess, new Point(btnEnc.Right + 8, 304));
            var lblStat = new Label { Text = "Ready", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(btnDec.Right + 14, 316) };

            // Output
            var lblOut = new Label { Text = "Output (Base64 / Plaintext)", Font = AppTheme.FontH3, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 354) };
            var txtOut = new RichTextBox
            {
                Bounds = new Rectangle(16, 378, 730, 120),
                Font = new Font("Cascadia Code", 9), BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(240, 242, 255), ForeColor = Color.FromArgb(50, 50, 160), ReadOnly = true
            };
            var btnCopy = MakePill("⎘ Copy", Color.FromArgb(238, 239, 255), new Point(16, 510), AppTheme.Accent);
            btnCopy.Click += (s, e) => ClipboardService.CopyToClipboard(txtOut.Text);

            btnEnc.Click += (s, e) => {
                try { txtOut.Text = cipher.Encrypt(txtIn.Text, txtK.Text); lblStat.Text = "Encrypted ✓"; lblStat.ForeColor = AppTheme.AccentSuccess; HistoryService.LogOperation("Encrypt", histLabel, "OK"); }
                catch (Exception ex) { lblStat.Text = ex.Message; lblStat.ForeColor = AppTheme.AccentDanger; }
            };
            btnDec.Click += (s, e) => {
                try { txtOut.Text = cipher.Decrypt(txtIn.Text, txtK.Text); lblStat.Text = "Decrypted ✓"; lblStat.ForeColor = AppTheme.AccentSuccess; HistoryService.LogOperation("Decrypt", histLabel, "OK"); }
                catch (Exception ex) { lblStat.Text = ex.Message; lblStat.ForeColor = AppTheme.AccentDanger; }
            };

            p.Controls.AddRange(new Control[] { lbl, sub, lblIn, txtIn, lblK, txtK, btnGenK, btnEnc, btnDec, lblStat, lblOut, txtOut, btnCopy });
            return card;
        }

        private Panel BuildRsa()
        {
            var card = new RoundedPanel { Dock = DockStyle.Fill };

            var lbl = new Label { Text = "RSA Asymmetric Encryption", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(16, 12) };
            var sub = new Label { Text = "2048-bit OAEP · Public key encrypts · Private key decrypts", Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 38) };

            var btnGen = MakePill("⟳  Generate New 2048-bit Key Pair", AppTheme.Accent, new Point(16, 70));

            var lblPub  = new Label { Text = "Public Key (XML — share this)", Font = AppTheme.FontH3, ForeColor = Color.FromArgb(34, 150, 94), AutoSize = true, Location = new Point(16, 118) };
            var txtPub  = new RichTextBox { Bounds = new Rectangle(16, 142, 370, 90), Font = new Font("Cascadia Code", 7), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(240, 255, 248), ReadOnly = true };

            var lblPriv = new Label { Text = "Private Key (XML — keep secret!)", Font = AppTheme.FontH3, ForeColor = AppTheme.AccentDanger, AutoSize = true, Location = new Point(400, 118) };
            var txtPriv = new RichTextBox { Bounds = new Rectangle(400, 142, 370, 90), Font = new Font("Cascadia Code", 7), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(255, 242, 242), ReadOnly = true };

            var lblIn  = new Label { Text = "Input Text", Font = AppTheme.FontH3, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 248) };
            var txtIn  = new RichTextBox { Bounds = new Rectangle(16, 272, 755, 80), Font = AppTheme.FontBody, BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(248, 249, 255) };

            var btnEnc = MakePill("▶  Encrypt (Public Key)", AppTheme.Accent, new Point(16, 366));
            var btnDec = MakePill("◀  Decrypt (Private Key)", AppTheme.AccentSuccess, new Point(btnEnc.Right + 8, 366));

            var lblOut = new Label { Text = "Output", Font = AppTheme.FontH3, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 416) };
            var txtOut = new RichTextBox { Bounds = new Rectangle(16, 440, 755, 80), Font = new Font("Cascadia Code", 9), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(240, 242, 255), ForeColor = Color.FromArgb(50, 50, 160), ReadOnly = true };

            btnGen.Click += (s, e) => { KeyGeneratorService.GenerateRsaKeyPair(out string pub, out string prv); txtPub.Text = pub; txtPriv.Text = prv; };
            btnEnc.Click += (s, e) => { try { txtOut.Text = _rsa.Encrypt(txtIn.Text, txtPub.Text); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            btnDec.Click += (s, e) => { try { txtOut.Text = _rsa.Decrypt(txtIn.Text, txtPriv.Text); } catch (Exception ex) { MessageBox.Show(ex.Message); } };

            card.Controls.AddRange(new Control[] { lbl, sub, btnGen, lblPub, txtPub, lblPriv, txtPriv, lblIn, txtIn, btnEnc, btnDec, lblOut, txtOut });
            return card;
        }

        private static Button MakePill(string text, Color bg, Point loc, Color? fg = null)
        {
            var b = new Button
            {
                Text = text, Location = loc, AutoSize = true, FlatStyle = FlatStyle.Flat,
                BackColor = bg, ForeColor = fg ?? Color.White, Font = AppTheme.FontBodyBold,
                Cursor = Cursors.Hand, Height = 34, Padding = new Padding(10, 0, 10, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(bg, 0.1f);
            return b;
        }
    }
}
