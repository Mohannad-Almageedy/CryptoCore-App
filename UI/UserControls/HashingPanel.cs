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
            var card = ControlFactory.Card();
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 9,
                BackColor = Color.Transparent, Padding = Padding.Empty
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // title
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // subtitle
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // "Input" label
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 45)); // input box
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // HMAC key label
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));// HMAC key input
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));// algorithm buttons
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // "Result" label + status
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 55)); // result output

            var txtIn  = ControlFactory.MultiLineInput();
            var txtOut = ControlFactory.MonoOutput();
            var txtKey = ControlFactory.SingleLineInput();
            var lblStat = ControlFactory.StatusLabel();

            var hmacRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent, Padding = Padding.Empty };
            hmacRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            hmacRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            hmacRow.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            hmacRow.Controls.Add(new Label { Text = "HMAC Key:", Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextSecondary, AutoSize = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, BackColor = Color.Transparent, Margin = new Padding(0, 0, 10, 0) }, 0, 0);
            var keyWrap = new Panel { BackColor = AppTheme.InputWrapBg, Padding = new Padding(6), Dock = DockStyle.Fill };
            txtKey.Dock = DockStyle.Fill;
            keyWrap.Controls.Add(txtKey);
            hmacRow.Controls.Add(keyWrap, 1, 0);

            var algoRow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.Transparent, WrapContents = false, Padding = new Padding(0, 8, 0, 0) };
            var btnMD5    = ControlFactory.Pill("MD5",         Color.FromArgb(234, 67, 53));
            var btnSHA1   = ControlFactory.Pill("SHA-1",       Color.FromArgb(251, 140, 0));
            var btnSHA256 = ControlFactory.Pill("SHA-256",     AppTheme.Accent);
            var btnSHA512 = ControlFactory.Pill("SHA-512",     AppTheme.AccentInfo);
            var btnHMAC   = ControlFactory.Pill("HMAC-SHA256", AppTheme.AccentSuccess);
            var btnCopy   = ControlFactory.Pill("⎘ Copy",      AppTheme.ButtonMutedBg, AppTheme.ButtonMutedFg);
            algoRow.Controls.AddRange(new Control[] { btnMD5, btnSHA1, btnSHA256, btnSHA512, btnHMAC, btnCopy });

            var resultHeader = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent, AutoSize = true, Padding = Padding.Empty };
            resultHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            resultHeader.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            resultHeader.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            resultHeader.Controls.Add(ControlFactory.SectionLabel("📋  Hash Result (Hex)"), 0, 0);
            resultHeader.Controls.Add(lblStat, 1, 0);

            tbl.Controls.Add(ControlFactory.PageTitle("Cryptographic Hash Functions"), 0, 0);
            tbl.Controls.Add(ControlFactory.SubTitle("One-way transformation  •  Cannot be reversed or decrypted"), 0, 1);
            tbl.Controls.Add(ControlFactory.SectionLabel("✏️  Input Text"), 0, 2);
            tbl.Controls.Add(WrapBox(txtIn), 0, 3);
            tbl.Controls.Add(ControlFactory.SectionLabel("🔑  HMAC Secret Key  (only for HMAC-SHA256)"), 0, 4);
            tbl.Controls.Add(hmacRow, 0, 5);
            tbl.Controls.Add(algoRow, 0, 6);
            tbl.Controls.Add(resultHeader, 0, 7);
            tbl.Controls.Add(WrapBox(txtOut, AppTheme.OutputBg), 0, 8);

            void Run(Func<string> fn, string name)
            {
                try { txtOut.Text = fn(); lblStat.Text = name + " ✓"; lblStat.ForeColor = AppTheme.AccentSuccess; HistoryService.LogOperation("Hash", name, "OK"); }
                catch (Exception ex) { lblStat.Text = ex.Message; lblStat.ForeColor = AppTheme.AccentDanger; }
            }

            btnMD5.Click    += (s, e) => Run(() => HashingService.ComputeMD5(txtIn.Text), "MD5");
            btnSHA1.Click   += (s, e) => Run(() => HashingService.ComputeSHA1(txtIn.Text), "SHA-1");
            btnSHA256.Click += (s, e) => Run(() => HashingService.ComputeSHA256(txtIn.Text), "SHA-256");
            btnSHA512.Click += (s, e) => Run(() => HashingService.ComputeSHA512(txtIn.Text), "SHA-512");
            btnHMAC.Click   += (s, e) => Run(() => HashingService.ComputeHMACSHA256(txtIn.Text, txtKey.Text), "HMAC-SHA256");
            btnCopy.Click   += (s, e) => ClipboardService.CopyToClipboard(txtOut.Text);

            card.Controls.Add(tbl);
            Controls.Add(card);
        }

        private static Panel WrapBox(Control inner, Color? bg = null)
        {
            var p = new Panel { BackColor = bg ?? AppTheme.InputWrapBg, Dock = DockStyle.Fill, Padding = new Padding(6), Margin = new Padding(0, 0, 0, 6) };
            inner.Dock = DockStyle.Fill;
            p.Controls.Add(inner);
            return p;
        }
    }
}
