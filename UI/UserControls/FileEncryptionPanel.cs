using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Services;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.UserControls
{
    public class FileEncryptionPanel : UserControl
    {
        private string? _filePath;

        public FileEncryptionPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = AppTheme.ContentBg;
            BuildUI();
        }

        private void BuildUI()
        {
            var card = new RoundedPanel { Dock = DockStyle.Fill };

            // Header
            var lbl = new Label { Text = "File Encryption", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(16, 12) };
            var sub = new Label { Text = "AES-256 stream cipher · Works on any file type · RAM-safe for large files", Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 40) };

            // File picker
            var lblFile = new Label { Text = "Source File", Font = AppTheme.FontH3, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 80) };
            var txtFile = new TextBox { Bounds = new Rectangle(16, 104, 600, 32), Font = AppTheme.FontBody, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 249, 255), ReadOnly = true };
            var btnBrowse = MakePill("Browse…", Color.FromArgb(238, 239, 255), new Point(626, 104), AppTheme.Accent);
            btnBrowse.Click += (s, e) => {
                using var d = new OpenFileDialog { Filter = "All Files (*.*)|*.*", Title = "Select file to encrypt / decrypt" };
                if (d.ShowDialog() == DialogResult.OK) { _filePath = d.FileName; txtFile.Text = _filePath; }
            };

            // Password
            var lblPw = new Label { Text = "Password", Font = AppTheme.FontH3, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 152) };
            var txtPw = new TextBox { Bounds = new Rectangle(16, 176, 380, 32), Font = AppTheme.FontBody, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 249, 255), UseSystemPasswordChar = true };
            var btnShow = MakePill("Show", Color.FromArgb(238, 239, 255), new Point(404, 176), AppTheme.TextSecondary);
            var btnGenPw = MakePill("⟳ Generate", AppTheme.AccentInfo, new Point(btnShow.Right + 8, 176));
            bool vis = false;
            btnShow.Click += (s, e) => { vis = !vis; txtPw.UseSystemPasswordChar = !vis; btnShow.Text = vis ? "Hide" : "Show"; };
            btnGenPw.Click += (s, e) => txtPw.Text = KeyGeneratorService.GenerateSecurePassword(20);

            // Action buttons
            var btnEnc = MakePill("🔒  Encrypt File", AppTheme.Accent, new Point(16, 228));
            var btnDec = MakePill("🔓  Decrypt File", AppTheme.AccentSuccess, new Point(btnEnc.Right + 8, 228));

            // Status / progress
            var lblStat = new Label { Text = "Ready.", Font = AppTheme.FontBody, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(16, 280) };
            var progress = new ProgressBar { Bounds = new Rectangle(16, 306, 750, 12), Style = ProgressBarStyle.Continuous, Minimum = 0, Maximum = 100 };
            var lblPct = new Label { Text = "", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(16, 322) };

            // Info card
            var infoCard = new RoundedPanel
            {
                Bounds = new Rectangle(16, 360, 750, 90),
                BackColor = Color.FromArgb(240, 244, 255),
                ShowShadow = false,
                Padding = new Padding(14)
            };
            infoCard.Controls.Add(new Label
            {
                Text = "ℹ️  How it works:\n" +
                       "• Encryption appends .enc and stores IV in file header\n" +
                       "• Decryption reads IV, derives key from password via SHA-256, then decrypts the stream chunk-by-chunk",
                Font = AppTheme.FontBody,
                ForeColor = AppTheme.TextSecondary,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            });

            // Handlers
            btnEnc.Click += async (s, e) =>
            {
                if (!Validate(txtFile.Text, txtPw.Text)) return;
                using var sd = new SaveFileDialog { FileName = _filePath + ".enc", Filter = "Encrypted File (*.enc)|*.enc|All Files|*.*" };
                if (sd.ShowDialog() != DialogResult.OK) return;
                btnEnc.Enabled = btnDec.Enabled = false;
                var prog = new Progress<int>(v => { progress.Value = v; lblPct.Text = $"{v}%"; });
                lblStat.Text = "Encrypting…"; lblStat.ForeColor = AppTheme.AccentInfo;
                try { await FileEncryptionService.EncryptFileAsync(_filePath!, sd.FileName, txtPw.Text, prog); lblStat.Text = "Encryption complete ✓"; lblStat.ForeColor = AppTheme.AccentSuccess; HistoryService.LogOperation("Encrypt", "File", System.IO.Path.GetFileName(_filePath)); }
                catch (Exception ex) { lblStat.Text = ex.Message; lblStat.ForeColor = AppTheme.AccentDanger; }
                finally { btnEnc.Enabled = btnDec.Enabled = true; }
            };

            btnDec.Click += async (s, e) =>
            {
                if (!Validate(txtFile.Text, txtPw.Text)) return;
                using var sd = new SaveFileDialog { FileName = _filePath!.Replace(".enc", ""), Filter = "All Files (*.*)|*.*" };
                if (sd.ShowDialog() != DialogResult.OK) return;
                btnEnc.Enabled = btnDec.Enabled = false;
                var prog = new Progress<int>(v => { progress.Value = v; lblPct.Text = $"{v}%"; });
                lblStat.Text = "Decrypting…"; lblStat.ForeColor = AppTheme.AccentInfo;
                try { await FileEncryptionService.DecryptFileAsync(_filePath!, sd.FileName, txtPw.Text, prog); lblStat.Text = "Decryption complete ✓"; lblStat.ForeColor = AppTheme.AccentSuccess; HistoryService.LogOperation("Decrypt", "File", System.IO.Path.GetFileName(_filePath)); }
                catch (Exception ex) { lblStat.Text = ex.Message; lblStat.ForeColor = AppTheme.AccentDanger; }
                finally { btnEnc.Enabled = btnDec.Enabled = true; }
            };

            card.Controls.AddRange(new Control[] { lbl, sub, lblFile, txtFile, btnBrowse, lblPw, txtPw, btnShow, btnGenPw, btnEnc, btnDec, lblStat, progress, lblPct, infoCard });
            Controls.Add(card);
        }

        private static bool Validate(string file, string pw)
        {
            if (string.IsNullOrWhiteSpace(file)) { MessageBox.Show("Please select a file first."); return false; }
            if (string.IsNullOrWhiteSpace(pw))   { MessageBox.Show("Please enter a password.");   return false; }
            return true;
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
