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
            var card = ControlFactory.Card();
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 9,
                BackColor = Color.Transparent, Padding = Padding.Empty
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Title
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Subtitle
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // File label
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // File picker row
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Password label
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // Password row
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 56)); // Action buttons
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Progress + status
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Info card

            var txtFile = ControlFactory.SingleLineInput(readOnly: true);
            var btnBrowse = ControlFactory.Pill("Browse…", AppTheme.ButtonMutedBg, AppTheme.ButtonMutedFg, 100);
            var fileRow = MakeInputRow(txtFile, btnBrowse, 6);

            var txtPw = ControlFactory.SingleLineInput();
            txtPw.UseSystemPasswordChar = true;
            var btnShow = ControlFactory.Pill("Show", AppTheme.ButtonMutedBg, AppTheme.TextSecondary, 70);
            var btnGenPw = ControlFactory.Pill("⟳ Generate", AppTheme.AccentInfo, null, 110);
            var pwFlow = new FlowLayoutPanel { Dock = DockStyle.None, AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.Transparent };
            pwFlow.Controls.AddRange(new Control[] { btnShow, btnGenPw });
            var pwRow = MakeInputRow(txtPw, pwFlow, 6);

            var btnEnc  = ControlFactory.Pill("🔒  Encrypt File", AppTheme.Accent, null, 160);
            var btnDec  = ControlFactory.Pill("🔓  Decrypt File", AppTheme.AccentSuccess, null, 160);
            var lblStat = ControlFactory.StatusLabel();
            var btnFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 0) };
            btnFlow.Controls.AddRange(new Control[] { btnEnc, btnDec, lblStat });

            var progress = new ProgressBar { Dock = DockStyle.Top, Height = 8, Minimum = 0, Maximum = 100, Style = ProgressBarStyle.Continuous };
            var progWrap = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(0, 4, 0, 0) };
            progWrap.Controls.Add(progress);

            var infoPanel = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.InputWrapBg, Padding = new Padding(14, 12, 14, 12) };
            infoPanel.Controls.Add(new Label
            {
                Text = "ℹ️  How it works:\n" +
                       "•  A random 128-bit IV is generated and prepended to the output file\n" +
                       "•  Your password is derived to a 32-byte AES-256 key via SHA-256\n" +
                       "•  Data is processed in 64 KB chunks — safe for multi-GB files",
                Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, Dock = DockStyle.Fill, BackColor = Color.Transparent
            });

            tbl.Controls.Add(ControlFactory.PageTitle("File Encryption"), 0, 0);
            tbl.Controls.Add(ControlFactory.SubTitle("AES-256 stream cipher  •  Any file type  •  RAM-safe for large files"), 0, 1);
            tbl.Controls.Add(ControlFactory.SectionLabel("📂  Source File"), 0, 2);
            tbl.Controls.Add(fileRow,   0, 3);
            tbl.Controls.Add(ControlFactory.SectionLabel("🔑  Password"), 0, 4);
            tbl.Controls.Add(pwRow,     0, 5);
            tbl.Controls.Add(btnFlow,   0, 6);
            tbl.Controls.Add(progWrap,  0, 7);
            tbl.Controls.Add(infoPanel, 0, 8);

            bool vis = false;
            btnBrowse.Click += (s, e) => { using var d = new OpenFileDialog { Filter = "All Files (*.*)|*.*" }; if (d.ShowDialog() == DialogResult.OK) { _filePath = d.FileName; txtFile.Text = _filePath; } };
            btnShow.Click  += (s, e) => { vis = !vis; txtPw.UseSystemPasswordChar = !vis; btnShow.Text = vis ? "Hide" : "Show"; };
            btnGenPw.Click += (s, e) => txtPw.Text = KeyGeneratorService.GenerateSecurePassword(20);

            btnEnc.Click += async (s, e) =>
            {
                if (!Validate(_filePath, txtPw.Text)) return;
                using var sd = new SaveFileDialog { FileName = _filePath + ".enc", Filter = "Encrypted (*.enc)|*.enc|All Files|*.*" };
                if (sd.ShowDialog() != DialogResult.OK) return;
                SetBusy(btnEnc, btnDec, true);
                lblStat.Text = "Encrypting…"; lblStat.ForeColor = AppTheme.AccentInfo;
                var prog = new Progress<int>(v => progress.Value = v);
                try { await FileEncryptionService.EncryptFileAsync(_filePath!, sd.FileName, txtPw.Text, prog); lblStat.Text = "Done ✓"; lblStat.ForeColor = AppTheme.AccentSuccess; HistoryService.LogOperation("Encrypt", "File", System.IO.Path.GetFileName(_filePath)); }
                catch (Exception ex) { lblStat.Text = ex.Message; lblStat.ForeColor = AppTheme.AccentDanger; }
                finally { SetBusy(btnEnc, btnDec, false); }
            };

            btnDec.Click += async (s, e) =>
            {
                if (!Validate(_filePath, txtPw.Text)) return;
                using var sd = new SaveFileDialog { FileName = _filePath!.Replace(".enc", ""), Filter = "All Files (*.*)|*.*" };
                if (sd.ShowDialog() != DialogResult.OK) return;
                SetBusy(btnEnc, btnDec, true);
                lblStat.Text = "Decrypting…"; lblStat.ForeColor = AppTheme.AccentInfo;
                var prog = new Progress<int>(v => progress.Value = v);
                try { await FileEncryptionService.DecryptFileAsync(_filePath!, sd.FileName, txtPw.Text, prog); lblStat.Text = "Done ✓"; lblStat.ForeColor = AppTheme.AccentSuccess; HistoryService.LogOperation("Decrypt", "File", System.IO.Path.GetFileName(_filePath)); }
                catch (Exception ex) { lblStat.Text = ex.Message; lblStat.ForeColor = AppTheme.AccentDanger; }
                finally { SetBusy(btnEnc, btnDec, false); }
            };

            card.Controls.Add(tbl);
            Controls.Add(card);
        }

        private static Panel MakeInputRow(Control main, Control extra, int pad = 6)
        {
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
                BackColor = AppTheme.InputWrapBg, Padding = new Padding(pad), Margin = new Padding(0, 0, 0, 4)
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            row.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            main.Dock = DockStyle.Fill;
            extra.Dock = DockStyle.Fill;
            row.Controls.Add(main,  0, 0);
            row.Controls.Add(extra, 1, 0);
            return row;
        }

        private static bool Validate(string? f, string p)
        {
            if (string.IsNullOrWhiteSpace(f)) { MessageBox.Show("Select a file first.", "Validation"); return false; }
            if (string.IsNullOrWhiteSpace(p)) { MessageBox.Show("Enter a password.",    "Validation"); return false; }
            return true;
        }

        private static void SetBusy(Button a, Button b, bool busy) { a.Enabled = !busy; b.Enabled = !busy; }
    }
}
