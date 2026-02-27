using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Models;
using CryptoEdu.Services;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.UserControls
{
    /// <summary>
    /// Educational cipher UI — uses TableLayoutPanel + SplitContainer, no absolute coords.
    /// Layout:
    ///   Row 0: Math rule banner  (auto height)
    ///   Row 1: SplitContainer    (fill)
    ///            Left : Input → Key → Buttons → Status → Output
    ///            Right: Step-by-step trace
    /// </summary>
    public class CipherDetailControl : UserControl
    {
        private readonly IClassicalCipher _cipher;
        private RichTextBox _txtInput  = null!;
        private TextBox     _txtKey    = null!;
        private RichTextBox _txtOutput = null!;
        private RichTextBox _txtSteps  = null!;
        private Label       _lblStatus = null!;

        public CipherDetailControl(IClassicalCipher cipher)
        {
            _cipher = cipher;
            DoubleBuffered = true;
            BackColor = AppTheme.ContentBg;
            Dock   = DockStyle.Fill;
            BuildUI();
        }

        private void BuildUI()
        {
            // ── Root layout: 2 rows ────────────────────────────────
            var root = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 1,
                RowCount    = 2,
                BackColor   = AppTheme.ContentBg,
                Padding     = new Padding(0)
            };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // ── Row 0: Math rule banner ────────────────────────────
            var ruleBanner = new Panel
            {
                BackColor = Color.FromArgb(240, 242, 253),
                Height    = 70,
                Dock      = DockStyle.Fill,
                Padding   = new Padding(16, 10, 16, 10)
            };
            var ruleTitle = new Label
            {
                Text      = "📐  Mathematical Rule",
                Font      = AppTheme.FontBodyBold,
                ForeColor = AppTheme.Accent,
                AutoSize  = true,
                Dock      = DockStyle.Top
            };
            var ruleBody = new Label
            {
                Text      = _cipher.GetMathematicalRule(),
                Font      = new Font("Cascadia Code", 8),
                ForeColor = AppTheme.TextSecondary,
                AutoSize  = true,
                Dock      = DockStyle.Fill
            };
            ruleBanner.Controls.Add(ruleBody);
            ruleBanner.Controls.Add(ruleTitle);

            // ── Row 1: SplitContainer ─────────────────────────────
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterWidth = 6,
                Orientation   = Orientation.Vertical
            };
            split.Panel1.BackColor = AppTheme.ContentBg;
            split.Panel2.BackColor = AppTheme.ContentBg;

            // Left panel — input / output
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
            Controls.Add(root);
        }

        // ── Left: Input / Key / Buttons / Output ─────────────────
        private Control BuildLeftPanel()
        {
            var tbl = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                ColumnCount = 1,
                RowCount    = 8,
                BackColor   = AppTheme.ContentBg,
                Padding     = new Padding(12, 12, 8, 12),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Label "Input"
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 38));  // Input textbox
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Label "Key"
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 46)); // Key TextBox row
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52)); // Buttons row
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Status
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // Label "Output"
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 62));  // Output textbox

            _txtInput = ControlFactory.MultiLineInput();
            _txtKey   = ControlFactory.SingleLineInput();
            _txtOutput = ControlFactory.MonoOutput();

            _lblStatus = ControlFactory.StatusLabel();

            // Key row: key input + copy button together
            var keyRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
                BackColor = Color.Transparent, Margin = Padding.Empty, Padding = Padding.Empty
            };
            keyRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            keyRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            keyRow.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var keyInput = WrapInput(_txtKey, 36);
            var btnClear = ControlFactory.Pill("Clear", Color.FromArgb(230, 232, 245), AppTheme.TextSecondary);
            keyRow.Controls.Add(keyInput, 0, 0);
            keyRow.Controls.Add(btnClear, 1, 0);

            // Buttons row
            var btnEnc = ControlFactory.Pill("▶  Encrypt", AppTheme.Accent);
            var btnDec = ControlFactory.Pill("◀  Decrypt", AppTheme.AccentSuccess);
            var btnCopyOut = ControlFactory.Pill("⎘ Copy", Color.FromArgb(238, 239, 255), AppTheme.Accent);
            var btnRow = ControlFactory.ButtonRow(btnEnc, btnDec, btnCopyOut);

            tbl.Controls.Add(ControlFactory.SectionLabel("✏️  Input Plaintext / Ciphertext"), 0, 0);
            tbl.Controls.Add(WrapInput(_txtInput), 0, 1);
            tbl.Controls.Add(ControlFactory.SectionLabel("🔑  Key / Parameter"), 0, 2);
            tbl.Controls.Add(keyRow, 0, 3);
            tbl.Controls.Add(btnRow, 0, 4);
            tbl.Controls.Add(_lblStatus, 0, 5);
            tbl.Controls.Add(ControlFactory.SectionLabel("📤  Output"), 0, 6);
            tbl.Controls.Add(WrapInput(_txtOutput), 0, 7);

            // Events
            btnEnc.Click += (s, e) => Run(() =>
            {
                _txtOutput.Text = _cipher.Encrypt(_txtInput.Text, _txtKey.Text);
                DisplaySteps(_cipher.GetEncryptionSteps(_txtInput.Text, _txtKey.Text));
                SetStatus("Encrypted ✓", AppTheme.AccentSuccess);
                HistoryService.LogOperation("Encrypt", _cipher.Name, "OK");
            });
            btnDec.Click += (s, e) => Run(() =>
            {
                _txtOutput.Text = _cipher.Decrypt(_txtInput.Text, _txtKey.Text);
                DisplaySteps(_cipher.GetDecryptionSteps(_txtInput.Text, _txtKey.Text));
                SetStatus("Decrypted ✓", AppTheme.AccentSuccess);
                HistoryService.LogOperation("Decrypt", _cipher.Name, "OK");
            });
            btnClear.Click += (s, e) =>
            {
                _txtInput.Clear(); _txtKey.Clear();
                _txtOutput.Clear(); _txtSteps?.Clear();
                SetStatus("Cleared.", AppTheme.TextMuted);
            };
            btnCopyOut.Click += (s, e) => ClipboardService.CopyToClipboard(_txtOutput.Text);

            return tbl;
        }

        // ── Right: Step-by-step trace ─────────────────────────────
        private Control BuildRightPanel()
        {
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2,
                BackColor = AppTheme.ContentBg,
                Padding = new Padding(8, 12, 12, 12)
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _txtSteps = ControlFactory.StepTrace();

            tbl.Controls.Add(ControlFactory.SectionLabel("🔍  Step-by-Step Breakdown"), 0, 0);
            tbl.Controls.Add(WrapInput(_txtSteps), 0, 1);
            return tbl;
        }

        // ── Helpers ───────────────────────────────────────────────
        private static Panel WrapInput(Control inner, int? fixedHeight = null)
        {
            var p = new Panel
            {
                BackColor = Color.FromArgb(248, 249, 253),
                Dock      = DockStyle.Fill,
                Padding   = new Padding(8),
                Margin    = new Padding(0, 0, 0, 8)
            };
            if (fixedHeight.HasValue) p.Height = fixedHeight.Value;
            inner.Dock = DockStyle.Fill;
            p.Controls.Add(inner);
            return p;
        }

        private void SetStatus(string text, Color color)
        {
            _lblStatus.Text      = text;
            _lblStatus.ForeColor = color;
        }

        private void Run(Action action)
        {
            try { action(); }
            catch (Exception ex)
            {
                SetStatus("Error: " + ex.Message, AppTheme.AccentDanger);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DisplaySteps(List<CipherStep> steps)
        {
            var sb = new StringBuilder();
            int n = 1;
            foreach (var step in steps)
            {
                sb.AppendLine($"── Step {n++}: {step.Title}");
                sb.AppendLine($"   {step.Description}");
                if (!string.IsNullOrEmpty(step.FormulaApplied))
                    sb.AppendLine($"   Formula : {step.FormulaApplied}");
                if (!string.IsNullOrEmpty(step.InputState))
                {
                    sb.AppendLine($"   In  → {step.InputState}");
                    sb.AppendLine($"   Out → {step.OutputState}");
                }
                var vis = step.VisualizationData?.ToString();
                if (!string.IsNullOrEmpty(vis)) { sb.AppendLine(); sb.AppendLine(vis); }
                sb.AppendLine();
            }
            _txtSteps.Text = sb.ToString();
        }
    }
}
