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
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterWidth = 8,
                BackColor = AppTheme.ContentBg
            };
            split.Panel1.BackColor = AppTheme.ContentBg;
            split.Panel2.BackColor = AppTheme.ContentBg;
            split.VisibleChanged += (s, e) =>
            {
                if (split.Visible && split.Height > 100 && split.SplitterDistance != split.Height / 2)
                    split.SplitterDistance = split.Height / 2;
            };
            split.Panel1.Controls.Add(BuildPasswordSection());
            split.Panel2.Controls.Add(BuildRsaSection());
            Controls.Add(split);
        }

        private Control BuildPasswordSection()
        {
            var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            var tbl  = MakeTbl(6);
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var lblTitle = new Label { Text = "Password Generator", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, BackColor = Color.Transparent };
            var lblSub   = new Label { Text = "Cryptographically-secure passwords via System.Security.Cryptography", Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 6) };

            var optRow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, AutoSize = true, BackColor = Color.Transparent, WrapContents = false, Padding = new Padding(0, 0, 0, 8) };
            var numLen = new NumericUpDown { Minimum = 6, Maximum = 128, Value = 20, Width = 76, Font = AppTheme.FontBody, Margin = new Padding(0, 3, 14, 0) };
            var chkSp  = new CheckBox { Text = "Include special chars (!@#$&*%)", Checked = true, Font = AppTheme.FontBody, AutoSize = true, ForeColor = AppTheme.TextSecondary, Margin = new Padding(0, 4, 0, 0), BackColor = Color.Transparent };
            optRow.Controls.AddRange(new Control[] { new Label { Text = "Length:", Font = AppTheme.FontBodyBold, ForeColor = AppTheme.TextSecondary, AutoSize = true, Margin = new Padding(0, 5, 8, 0), BackColor = Color.Transparent }, numLen, chkSp });

            var btnGen  = ControlFactory.Pill("⟳  Generate Password", AppTheme.Accent, null, 200);
            var btnFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            btnFlow.Controls.Add(btnGen);

            var txtPw = new TextBox { Dock = DockStyle.Fill, Font = new Font("Cascadia Code", 16, FontStyle.Bold), BorderStyle = BorderStyle.None, ReadOnly = true, TextAlign = HorizontalAlignment.Center, ForeColor = AppTheme.Accent, BackColor = Color.FromArgb(240, 242, 253) };
            var btnCopy = ControlFactory.Pill("⎘ Copy", Color.FromArgb(238, 239, 255), AppTheme.Accent, 90);
            var outRow  = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.FromArgb(240, 242, 253), Padding = new Padding(10) };
            outRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            outRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            outRow.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            outRow.Controls.Add(txtPw, 0, 0);
            outRow.Controls.Add(btnCopy, 1, 0);

            tbl.Controls.Add(lblTitle, 0, 0);
            tbl.Controls.Add(lblSub,   0, 1);
            tbl.Controls.Add(optRow,   0, 2);
            tbl.Controls.Add(btnFlow,  0, 3);
            tbl.Controls.Add(ControlFactory.SectionLabel("Generated Password"), 0, 4);
            tbl.Controls.Add(outRow,   0, 5);

            btnGen.Click  += (s, e) => txtPw.Text = KeyGeneratorService.GenerateSecurePassword((int)numLen.Value, chkSp.Checked);
            btnCopy.Click += (s, e) => ClipboardService.CopyToClipboard(txtPw.Text);

            card.Controls.Add(tbl);
            return card;
        }

        private Control BuildRsaSection()
        {
            var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(20) };
            var tbl  = MakeTbl(4);
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var hdrRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent, AutoSize = true };
            hdrRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            hdrRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            hdrRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            hdrRow.Controls.Add(new Label { Text = "RSA 2048-bit Key Pair", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, BackColor = Color.Transparent }, 0, 0);
            var btnGen = ControlFactory.Pill("⟳  Generate", AppTheme.Accent, null, 130);
            hdrRow.Controls.Add(btnGen, 1, 0);

            var lblSub = new Label { Text = "Asymmetric keys for RSA encryption operations", Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 8) };

            var lblsRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent, AutoSize = true };
            lblsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            lblsRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            lblsRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var btnCpPub  = ControlFactory.Pill("⎘ Copy Public",  Color.FromArgb(238, 255, 248), Color.FromArgb(24, 140, 84), 120);
            var btnCpPriv = ControlFactory.Pill("⎘ Copy Private", Color.FromArgb(255, 238, 238), AppTheme.AccentDanger,       120);
            var pubHdr = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.Transparent, Padding = Padding.Empty };
            pubHdr.Controls.Add(new Label { Text = "Public Key  (share this)", Font = AppTheme.FontBodyBold, ForeColor = AppTheme.AccentSuccess, AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0, 5, 8, 0) });
            pubHdr.Controls.Add(btnCpPub);
            var prvHdr = new FlowLayoutPanel { AutoSize = true, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.Transparent, Padding = Padding.Empty };
            prvHdr.Controls.Add(new Label { Text = "Private Key  (keep secret!)", Font = AppTheme.FontBodyBold, ForeColor = AppTheme.AccentDanger, AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0, 5, 8, 0) });
            prvHdr.Controls.Add(btnCpPriv);
            lblsRow.Controls.Add(pubHdr, 0, 0);
            lblsRow.Controls.Add(prvHdr, 1, 0);

            var txtPub  = new RichTextBox { Dock = DockStyle.Fill, Font = new Font("Cascadia Code", 7), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(240, 255, 248), ReadOnly = true };
            var txtPriv = new RichTextBox { Dock = DockStyle.Fill, Font = new Font("Cascadia Code", 7), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(255, 242, 242), ReadOnly = true };
            var boxRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent };
            boxRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            boxRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            boxRow.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            boxRow.Controls.Add(WrapBox(txtPub,  Color.FromArgb(240, 255, 248)), 0, 0);
            boxRow.Controls.Add(WrapBox(txtPriv, Color.FromArgb(255, 242, 242)), 1, 0);

            tbl.Controls.Add(hdrRow,  0, 0);
            tbl.Controls.Add(lblSub,  0, 1);
            tbl.Controls.Add(lblsRow, 0, 2);
            tbl.Controls.Add(boxRow,  0, 3);

            btnGen.Click    += (s, e) => { KeyGeneratorService.GenerateRsaKeyPair(out string pub, out string prv); txtPub.Text = pub; txtPriv.Text = prv; HistoryService.LogOperation("KeyGen", "RSA", "OK"); };
            btnCpPub.Click  += (s, e) => ClipboardService.CopyToClipboard(txtPub.Text);
            btnCpPriv.Click += (s, e) => ClipboardService.CopyToClipboard(txtPriv.Text);

            card.Controls.Add(tbl);
            return card;
        }

        private static TableLayoutPanel MakeTbl(int rows) => new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = rows, BackColor = Color.Transparent, Padding = Padding.Empty };

        private static Panel WrapBox(Control inner, Color? bg = null)
        {
            var p = new Panel { BackColor = bg ?? Color.FromArgb(248, 249, 253), Dock = DockStyle.Fill, Padding = new Padding(6), Margin = new Padding(0, 0, 4, 0) };
            inner.Dock = DockStyle.Fill;
            p.Controls.Add(inner);
            return p;
        }
    }
}
