using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Models;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.UserControls
{
    /// <summary>
    /// Professional card-based panel for a single classical cipher.
    /// Left column: input / output.  Right panel: math rule + step trace.
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
            Dock = DockStyle.Fill;
            Padding = new Padding(0);
            BuildUI();
        }

        private void BuildUI()
        {
            // ── Math Rule Card ───────────────────────────────────────
            var ruleCard = new RoundedPanel
            {
                Dock    = DockStyle.Top,
                Height  = 90,
                Margin  = new Padding(0, 0, 0, 10)
            };
            var lblRuleTitle = new Label
            {
                Text      = "📐  Mathematical Rule",
                Font      = AppTheme.FontH3,
                ForeColor = AppTheme.Accent,
                AutoSize  = true,
                Location  = new Point(16, 12)
            };
            var lblRule = new Label
            {
                Text      = _cipher.GetMathematicalRule(),
                Font      = new Font("Cascadia Code", 9),
                ForeColor = AppTheme.TextSecondary,
                AutoSize  = false,
                Bounds    = new Rectangle(16, 36, ruleCard.Width - 32, 50),
                Anchor    = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ruleCard.Controls.AddRange(new Control[] { lblRuleTitle, lblRule });

            // ── Main two-column layout ───────────────────────────────
            var split = new SplitContainer
            {
                Dock             = DockStyle.Fill,
                SplitterDistance = 480,
                SplitterWidth    = 8,
                BackColor        = AppTheme.ContentBg
            };
            split.Panel1.BackColor = AppTheme.ContentBg;
            split.Panel2.BackColor = AppTheme.ContentBg;

            // ── LEFT: Input ──────────────────────────────────────────
            var leftCard = new RoundedPanel { Dock = DockStyle.Fill };

            var lblIn = MakeLabel("✏️  Plaintext / Ciphertext", AppTheme.TextSecondary, AppTheme.FontH3, new Point(14, 12));

            _txtInput = new RichTextBox
            {
                Bounds      = new Rectangle(14, 38, leftCard.Width - 28, 120),
                Anchor      = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Font        = AppTheme.FontBody,
                BorderStyle = BorderStyle.None,
                BackColor   = Color.FromArgb(248, 249, 255),
                ForeColor   = AppTheme.TextPrimary,
                ScrollBars  = RichTextBoxScrollBars.Vertical
            };

            var lblKeyLbl = MakeLabel("🔑  Key / Parameter", AppTheme.TextSecondary, AppTheme.FontH3, new Point(14, 172));
            _txtKey = new TextBox
            {
                Bounds      = new Rectangle(14, 196, leftCard.Width - 100, 32),
                Anchor      = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                Font        = AppTheme.FontBody,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor   = Color.FromArgb(248, 249, 255)
            };

            // Buttons row
            var btnEnc = MakePill("  Encrypt  ▶", AppTheme.Accent, new Point(14, 242));
            var btnDec = MakePill("  Decrypt  ◀", Color.FromArgb(34, 197, 94), new Point(btnEnc.Right + 8, 242));
            var btnClr = MakePill("  Clear", Color.FromArgb(200, 202, 220), new Point(btnDec.Right + 8, 242), AppTheme.TextSecondary);

            _lblStatus = new Label
            {
                Text      = "Ready.",
                Font      = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                AutoSize  = true,
                Location  = new Point(14, 292)
            };

            var lblOutLbl = MakeLabel("📤  Result", AppTheme.TextSecondary, AppTheme.FontH3, new Point(14, 318));
            _txtOutput = new RichTextBox
            {
                Bounds      = new Rectangle(14, 344, leftCard.Width - 28, 110),
                Anchor      = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top,
                Font        = new Font("Cascadia Code", 9),
                BorderStyle = BorderStyle.None,
                BackColor   = Color.FromArgb(240, 242, 255),
                ForeColor   = Color.FromArgb(50, 50, 160),
                ReadOnly    = true
            };

            var btnCopy = MakePill("  ⎘ Copy Result", Color.FromArgb(238, 239, 255), new Point(14, leftCard.Height - 50), AppTheme.Accent);
            btnCopy.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

            leftCard.Controls.AddRange(new Control[] { lblIn, _txtInput, lblKeyLbl, _txtKey, btnEnc, btnDec, btnClr, _lblStatus, lblOutLbl, _txtOutput, btnCopy });

            // ── RIGHT: Step trace ────────────────────────────────────
            var rightCard = new RoundedPanel { Dock = DockStyle.Fill };

            var lblStepTitle = MakeLabel("🔍  Step-by-Step Breakdown", AppTheme.TextSecondary, AppTheme.FontH3, new Point(14, 12));

            _txtSteps = new RichTextBox
            {
                Bounds      = new Rectangle(14, 38, rightCard.Width - 28, rightCard.Height - 55),
                Anchor      = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                Font        = new Font("Cascadia Code", 8),
                BorderStyle = BorderStyle.None,
                BackColor   = Color.FromArgb(248, 249, 255),
                ForeColor   = AppTheme.TextPrimary,
                ReadOnly    = true,
                ScrollBars  = RichTextBoxScrollBars.Vertical
            };

            rightCard.Controls.AddRange(new Control[] { lblStepTitle, _txtSteps });

            split.Panel1.Controls.Add(leftCard);
            split.Panel2.Controls.Add(rightCard);

            // ── Event handlers ────────────────────────────────────────
            btnEnc.Click += (s, e) => Run(() =>
            {
                _txtOutput.Text = _cipher.Encrypt(_txtInput.Text, _txtKey.Text);
                DisplaySteps(_cipher.GetEncryptionSteps(_txtInput.Text, _txtKey.Text));
                SetStatus("Encryption complete ✓", AppTheme.AccentSuccess);
            });

            btnDec.Click += (s, e) => Run(() =>
            {
                _txtOutput.Text = _cipher.Decrypt(_txtInput.Text, _txtKey.Text);
                DisplaySteps(_cipher.GetDecryptionSteps(_txtInput.Text, _txtKey.Text));
                SetStatus("Decryption complete ✓", AppTheme.AccentSuccess);
            });

            btnClr.Click += (s, e) =>
            {
                _txtInput.Clear(); _txtKey.Clear();
                _txtOutput.Clear(); _txtSteps.Clear();
                SetStatus("Ready.", AppTheme.TextMuted);
            };

            btnCopy.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_txtOutput.Text))
                    Services.ClipboardService.CopyToClipboard(_txtOutput.Text);
            };

            // ── Wire to main panel ────────────────────────────────────
            Controls.Add(split);
            Controls.Add(ruleCard);
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

        private void SetStatus(string text, Color color)
        {
            _lblStatus.Text = text;
            _lblStatus.ForeColor = color;
        }

        private void DisplaySteps(List<CipherStep> steps)
        {
            var sb = new StringBuilder();
            int n = 1;
            foreach (var step in steps)
            {
                sb.AppendLine($"━━━  Step {n++}: {step.Title}  ━━━");
                sb.AppendLine($"  {step.Description}");
                if (!string.IsNullOrEmpty(step.FormulaApplied))
                    sb.AppendLine($"  Formula : {step.FormulaApplied}");
                if (!string.IsNullOrEmpty(step.InputState))
                {
                    sb.AppendLine($"  In  → {step.InputState}");
                    sb.AppendLine($"  Out → {step.OutputState}");
                }
                var vis = step.VisualizationData?.ToString();
                if (!string.IsNullOrEmpty(vis))
                {
                    sb.AppendLine();
                    sb.AppendLine(vis);
                }
                sb.AppendLine();
            }
            _txtSteps.Text = sb.ToString();
        }

        // ── tiny helpers ─────────────────────────────────────────────
        private static Label MakeLabel(string text, Color fg, Font font, Point loc)
        {
            return new Label { Text = text, ForeColor = fg, Font = font, AutoSize = true, Location = loc, BackColor = Color.Transparent };
        }

        private static Button MakePill(string text, Color bg, Point loc, Color? fg = null)
        {
            var b = new Button
            {
                Text      = text,
                Location  = loc,
                AutoSize  = true,
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = fg ?? Color.White,
                Font      = AppTheme.FontBodyBold,
                Cursor    = Cursors.Hand,
                Height    = 34,
                Padding   = new Padding(10, 0, 10, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(bg, 0.1f);
            return b;
        }
    }
}
