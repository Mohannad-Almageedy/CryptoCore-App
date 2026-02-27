using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Services;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.UserControls
{
    public class CipherDetailControl : UserControl
    {
        private readonly Core.Interfaces.IClassicalCipher _cipher;

        private RichTextBox _txtInput  = null!;
        private TextBox     _txtKey    = null!;
        private RichTextBox _txtOutput = null!;
        private RichTextBox? _txtSteps;
        private Label       _lblStatus = null!;

        public CipherDetailControl(Core.Interfaces.IClassicalCipher cipher)
        {
            _cipher = cipher;
            Dock    = DockStyle.Fill;
            BuildLayout();
        }

        private void BuildLayout()
        {
            var root = new TableLayoutPanel
            {
                Dock      = DockStyle.Fill,
                RowCount  = 2, ColumnCount = 1,
                BackColor = Color.Transparent, Padding = Padding.Empty
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Banner for rule
            var ruleBanner = new Panel
            {
                Dock      = DockStyle.Fill, AutoSize = true,
                BackColor = AppTheme.InputWrapBg, Margin = new Padding(0, 0, 0, 10),
                Padding   = new Padding(14, 10, 14, 10)
            };
            var lblRule = new Label
            {
                Text      = $"📖 Rule: {_cipher.GetMathematicalRule()}",
                Font      = AppTheme.FontBodyBold, ForeColor = AppTheme.Accent,
                Dock      = DockStyle.Top, AutoSize = true, BackColor = Color.Transparent
            };
            ruleBanner.Controls.Add(lblRule);

            // Split Container
            var split = new SplitContainer
            {
                Dock        = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                BackColor   = AppTheme.ContentBg, SplitterWidth = 8
            };
            split.Panel1.BackColor = AppTheme.ContentBg;
            split.Panel2.BackColor = AppTheme.ContentBg;

            // Left panel — inputs
            split.Panel1.Controls.Add(BuildLeftPanel());
            // Right panel — step trace
            split.Panel2.Controls.Add(BuildRightPanel());

            // Defer splitter until the control is actually visible
            split.VisibleChanged += (s, e) =>
            {
                if (split.Visible && split.Width > 100 && split.SplitterDistance != (int)(split.Width * 0.5))
                    split.SplitterDistance = (int)(split.Width * 0.5);
            };

            root.Controls.Add(ruleBanner, 0, 0);
            root.Controls.Add(split, 0, 1);

            var card = ControlFactory.Card();
            card.Controls.Add(root);
            Controls.Add(card);
        }

        private Control BuildLeftPanel()
        {
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 7,
                BackColor = Color.Transparent, Padding = Padding.Empty
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // label In
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 40)); // box In
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // label Key
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));// key row
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));// button row
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // status
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // label Out
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 60)); // box out

            _txtInput  = ControlFactory.MultiLineInput();
            _txtOutput = ControlFactory.MonoOutput();
            _txtKey    = ControlFactory.SingleLineInput();
            _lblStatus = ControlFactory.StatusLabel();

            var keyRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.Transparent, Padding = Padding.Empty
            };
            keyRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            keyRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var btnClear = ControlFactory.Pill("Clear", AppTheme.ButtonMutedBg, AppTheme.ButtonMutedFg);
            keyRow.Controls.Add(WrapInput(_txtKey), 0, 0);
            keyRow.Controls.Add(btnClear,           1, 0);

            var btnEnc = ControlFactory.Pill("▶  Encrypt", AppTheme.Accent);
            var btnDec = ControlFactory.Pill("◀  Decrypt", AppTheme.AccentSuccess);
            var btnCopyOut = ControlFactory.Pill("⎘ Copy", AppTheme.ButtonMutedBg, AppTheme.ButtonMutedFg);
            var btnRow = ControlFactory.ButtonRow(btnEnc, btnDec, btnCopyOut);

            tbl.Controls.Add(ControlFactory.SectionLabel("✏️  Input Plaintext / Ciphertext"), 0, 0);
            tbl.Controls.Add(WrapInput(_txtInput), 0, 1);
            tbl.Controls.Add(ControlFactory.SectionLabel("🔑  Key / Parameter"), 0, 2);
            tbl.Controls.Add(keyRow, 0, 3);
            tbl.Controls.Add(btnRow, 0, 4);
            tbl.Controls.Add(_lblStatus, 0, 5);
            tbl.Controls.Add(ControlFactory.SectionLabel("📤  Output"), 0, 6);
            tbl.Controls.Add(WrapInput(_txtOutput), 0, 7);

            btnEnc.Click += (s, e) =>
            {
                try
                {
                    _txtOutput.Text = _cipher.Encrypt(_txtInput.Text, _txtKey.Text);
                    UpdateSteps();
                    SetStatus("Encrypted ✓", AppTheme.AccentSuccess);
                    HistoryService.LogOperation("Encrypt", _cipher.Name, "OK");
                }
                catch (Exception ex) { SetStatus(ex.Message, AppTheme.AccentDanger); }
            };

            btnDec.Click += (s, e) =>
            {
                try
                {
                    _txtOutput.Text = _cipher.Decrypt(_txtInput.Text, _txtKey.Text);
                    UpdateSteps();
                    SetStatus("Decrypted ✓", AppTheme.AccentSuccess);
                    HistoryService.LogOperation("Decrypt", _cipher.Name, "OK");
                }
                catch (Exception ex) { SetStatus(ex.Message, AppTheme.AccentDanger); }
            };

            btnClear.Click += (s, e) =>
            {
                _txtInput.Clear(); _txtKey.Clear();
                _txtOutput.Clear(); _txtSteps?.Clear();
                SetStatus("Cleared.", AppTheme.TextMuted);
            };

            btnCopyOut.Click += (s, e) => ClipboardService.CopyToClipboard(_txtOutput.Text);

            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 6, 0) };
            p.Controls.Add(tbl);
            return p;
        }

        private Control BuildRightPanel()
        {
            var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(6, 0, 0, 0), BackColor = Color.Transparent };
            var tbl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Color.Transparent };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            tbl.Controls.Add(ControlFactory.SectionLabel("🔍  Step-by-Step Trace"), 0, 0);
            _txtSteps = ControlFactory.StepTrace();
            tbl.Controls.Add(WrapInput(_txtSteps), 0, 1);
            p.Controls.Add(tbl);
            return p;
        }

        private void UpdateSteps()
        {
            if (_txtSteps != null)
            {
                try
                {
                    var steps = _cipher.GetEncryptionSteps(_txtInput.Text, _txtKey.Text);
                    var sb = new System.Text.StringBuilder();
                    for (int i = 0; i < steps.Count; i++)
                    {
                        sb.AppendLine($"[{i + 1}] {steps[i].Title}");
                        if (!string.IsNullOrEmpty(steps[i].Description)) sb.AppendLine(steps[i].Description);
                        if (!string.IsNullOrEmpty(steps[i].FormulaApplied)) sb.AppendLine($"Formula: {steps[i].FormulaApplied}");
                        sb.AppendLine($"In : {steps[i].InputState}");
                        sb.AppendLine($"Out: {steps[i].OutputState}");
                        sb.AppendLine("────────────────────────────────────────────────");
                    }
                    _txtSteps.Text = sb.ToString();
                }
                catch
                {
                    _txtSteps.Text = "Not available.";
                }
            }
        }

        private void SetStatus(string msg, Color color)
        {
            _lblStatus.Text = msg;
            _lblStatus.ForeColor = color;
        }

        private static Panel WrapInput(Control c)
        {
            var p = new Panel
            {
                BackColor = AppTheme.InputWrapBg,
                Dock      = DockStyle.Fill,
                Padding   = new Padding(6),
                Margin    = new Padding(0, 0, 4, 4)
            };
            c.Dock = DockStyle.Fill;
            p.Controls.Add(c);
            return p;
        }
    }
}
