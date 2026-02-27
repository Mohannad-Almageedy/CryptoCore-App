using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Services;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.UserControls
{
    /// <summary>
    /// Modern ciphers — AES / 3DES / RSA.
    /// Layout root: TableLayoutPanel (2 rows)
    ///   Row 0: FlowLayoutPanel tab chips (auto-height)
    ///   Row 1: content host (fill)
    /// Each sub-panel is also built with TableLayoutPanel — no absolute coords.
    /// </summary>
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
            var host = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.ContentBg };
            var chipBar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top, Height = 54, BackColor = AppTheme.ContentBg,
                FlowDirection = FlowDirection.LeftToRight, WrapContents = false,
                Padding = new Padding(8, 10, 8, 4)
            };

            var pages = new Control[]
            {
                BuildSymmetricPage("AES-256  •  CBC Mode", _aes,
                    "Military-grade symmetric encryption. Key is derived from your password via SHA-256.", "AES"),
                BuildSymmetricPage("Triple DES  •  CBC Mode", _des,
                    "3-key Triple-DES block cipher. Included for educational comparison with AES.", "3DES"),
                BuildRsaPage()
            };

            string[] chipLabels = { "AES-256", "Triple DES", "RSA" };
            Button? activeChip = null;

            for (int i = 0; i < chipLabels.Length; i++)
            {
                int idx   = i;
                var chip  = MakeChip(chipLabels[i]);
                chipBar.Controls.Add(chip);

                chip.Click += (s, e) =>
                {
                    foreach (Button b in chipBar.Controls) ResetChip(b);
                    ActivateChip(chip);
                    activeChip = chip;
                    host.Controls.Clear();
                    var pg = pages[idx];
                    pg.Dock = DockStyle.Fill;
                    host.Controls.Add(pg);
                };

                if (i == 0)
                {
                    ActivateChip(chip); activeChip = chip;
                    pages[0].Dock = DockStyle.Fill;
                    host.Controls.Add(pages[0]);
                }
            }

            Controls.Add(host);
            Controls.Add(chipBar);
        }

        // ── Symmetric (AES / DES) page ─────────────────────────────
        private Control BuildSymmetricPage(string title, Core.Interfaces.ICipher cipher, string desc, string tag)
        {
            // Root card
            var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 9,
                BackColor = Color.Transparent, Padding = Padding.Empty
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // title
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // subtitle
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));// spacer
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // "Input" label
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 40)); // input box
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // key zone
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));// button row
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // "Output" label
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 60)); // output box

            var lblTitle = new Label { Text = title, Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, BackColor = Color.Transparent };
            var lblSub   = new Label { Text = desc,  Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 4) };

            var txtIn  = ControlFactory.MultiLineInput();
            var txtOut = ControlFactory.MonoOutput();
            var txtKey = ControlFactory.SingleLineInput();
            var lblStat = ControlFactory.StatusLabel();

            // Key zone: [label][key input] [gen btn]
            var keyZone = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2,
                BackColor = Color.Transparent, Margin = Padding.Empty, Padding = Padding.Empty,
                AutoSize = true
            };
            keyZone.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            keyZone.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            keyZone.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            keyZone.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            keyZone.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblKey = ControlFactory.SectionLabel("🔑  Password / Key");
            var btnGenKey = ControlFactory.Pill("⟳ Auto-generate", AppTheme.AccentInfo, null, 140);
            var keyInputWrap = new Panel
            {
                BackColor = Color.FromArgb(248, 249, 253), Padding = new Padding(8), Dock = DockStyle.Fill, Height = 40, Margin = new Padding(0, 0, 8, 0)
            };
            txtKey.Dock = DockStyle.Fill;
            keyInputWrap.Controls.Add(txtKey);

            keyZone.Controls.Add(new Panel { Width = 0, BackColor = Color.Transparent }, 0, 0); // spacer
            keyZone.Controls.Add(lblKey, 1, 0);
            keyZone.Controls.Add(new Panel { Width = 0, BackColor = Color.Transparent }, 2, 0);
            keyZone.Controls.Add(keyInputWrap, 1, 1);
            keyZone.Controls.Add(btnGenKey, 2, 1);

            // Buttons row
            var btnEnc  = ControlFactory.Pill("▶  Encrypt", AppTheme.Accent);
            var btnDec  = ControlFactory.Pill("◀  Decrypt", AppTheme.AccentSuccess);
            var btnCopy = ControlFactory.Pill("⎘ Copy Output", Color.FromArgb(238, 239, 255), AppTheme.Accent);

            var btnFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent, WrapContents = false, Padding = new Padding(0, 8, 0, 0)
            };
            btnFlow.Controls.AddRange(new Control[] { btnEnc, btnDec, btnCopy, lblStat });

            tbl.Controls.Add(lblTitle,  0, 0);
            tbl.Controls.Add(lblSub,    0, 1);
            tbl.Controls.Add(new Panel { BackColor = Color.Transparent }, 0, 2);
            tbl.Controls.Add(ControlFactory.SectionLabel("✏️  Plaintext / Ciphertext"), 0, 3);
            tbl.Controls.Add(WrapBox(txtIn), 0, 4);
            tbl.Controls.Add(keyZone, 0, 5);
            tbl.Controls.Add(btnFlow, 0, 6);
            tbl.Controls.Add(ControlFactory.SectionLabel("📤  Output"), 0, 7);
            tbl.Controls.Add(WrapBox(txtOut), 0, 8);

            btnGenKey.Click += (s, e) => txtKey.Text = KeyGeneratorService.GenerateSecurePassword(16);
            btnEnc.Click += (s, e) => RunSym(cipher, tag, "Encrypt", () => cipher.Encrypt(txtIn.Text, txtKey.Text), txtOut, lblStat);
            btnDec.Click += (s, e) => RunSym(cipher, tag, "Decrypt", () => cipher.Decrypt(txtIn.Text, txtKey.Text), txtOut, lblStat);
            btnCopy.Click += (s, e) => ClipboardService.CopyToClipboard(txtOut.Text);

            card.Controls.Add(tbl);
            return card;
        }

        // ── RSA page ───────────────────────────────────────────────
        private Control BuildRsaPage()
        {
            var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 9,
                BackColor = Color.Transparent
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // title
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // subtitle
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // gen button
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 28)); // key boxes
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // "Input" label
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 20)); // input box
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));// button row
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));    // "Output" label
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 52)); // output box

            var lblTitle = new Label { Text = "RSA Asymmetric Encryption", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, BackColor = Color.Transparent };
            var lblSub   = new Label { Text = "2048-bit OAEP  •  Public key encrypts  •  Private key decrypts", Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 4) };

            var btnGen  = ControlFactory.Pill("⟳  Generate New 2048-bit RSA Key Pair", AppTheme.Accent, null, 320);
            var btnFlow0 = new FlowLayoutPanel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(0, 6, 0, 6), AutoSize = true };
            btnFlow0.Controls.Add(btnGen);

            // Key pair boxes side by side
            var keyTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2,
                BackColor = Color.Transparent, Padding = Padding.Empty
            };
            keyTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            keyTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            keyTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            keyTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var lblPub  = new Label { Text = "Public Key  (share this)", Font = AppTheme.FontBodyBold, ForeColor = AppTheme.AccentSuccess, AutoSize = true, BackColor = Color.Transparent };
            var lblPriv = new Label { Text = "Private Key  (keep secret!)", Font = AppTheme.FontBodyBold, ForeColor = AppTheme.AccentDanger, AutoSize = true, BackColor = Color.Transparent };
            var txtPub  = new RichTextBox { Dock = DockStyle.Fill, Font = new Font("Cascadia Code", 7), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(240, 255, 248), ReadOnly = true };
            var txtPriv = new RichTextBox { Dock = DockStyle.Fill, Font = new Font("Cascadia Code", 7), BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(255, 242, 242), ReadOnly = true };

            keyTable.Controls.Add(lblPub,  0, 0);
            keyTable.Controls.Add(lblPriv, 1, 0);
            keyTable.Controls.Add(WrapBox(txtPub,  Color.FromArgb(240, 255, 248)), 0, 1);
            keyTable.Controls.Add(WrapBox(txtPriv, Color.FromArgb(255, 242, 242)), 1, 1);

            var txtIn   = ControlFactory.MultiLineInput(80);
            var txtOut  = ControlFactory.MonoOutput(80);
            var lblStat = ControlFactory.StatusLabel();

            var btnEnc  = ControlFactory.Pill("▶  Encrypt  (needs Public Key)", AppTheme.Accent);
            var btnDec  = ControlFactory.Pill("◀  Decrypt  (needs Private Key)", AppTheme.AccentSuccess);
            var btnFlow1 = new FlowLayoutPanel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 0) };
            btnFlow1.Controls.AddRange(new Control[] { btnEnc, btnDec, lblStat });

            root.Controls.Add(lblTitle, 0, 0);
            root.Controls.Add(lblSub,   0, 1);
            root.Controls.Add(btnFlow0, 0, 2);
            root.Controls.Add(keyTable, 0, 3);
            root.Controls.Add(ControlFactory.SectionLabel("✏️  Input Text"), 0, 4);
            root.Controls.Add(WrapBox(txtIn), 0, 5);
            root.Controls.Add(btnFlow1, 0, 6);
            root.Controls.Add(ControlFactory.SectionLabel("📤  Output"), 0, 7);
            root.Controls.Add(WrapBox(txtOut), 0, 8);

            btnGen.Click  += (s, e) => { KeyGeneratorService.GenerateRsaKeyPair(out string pub, out string prv); txtPub.Text = pub; txtPriv.Text = prv; };
            btnEnc.Click  += (s, e) => Try(() => { txtOut.Text = _rsa.Encrypt(txtIn.Text, txtPub.Text); lblStat.Text = "Encrypted ✓"; lblStat.ForeColor = AppTheme.AccentSuccess; }, lblStat);
            btnDec.Click  += (s, e) => Try(() => { txtOut.Text = _rsa.Decrypt(txtIn.Text, txtPriv.Text); lblStat.Text = "Decrypted ✓"; lblStat.ForeColor = AppTheme.AccentSuccess; }, lblStat);

            card.Controls.Add(root);
            return card;
        }

        // ── Helpers ───────────────────────────────────────────────
        private static Panel WrapBox(Control inner, Color? bg = null)
        {
            var p = new Panel
            {
                BackColor = bg ?? Color.FromArgb(248, 249, 253),
                Dock = DockStyle.Fill, Padding = new Padding(6), Margin = new Padding(0, 0, 4, 0)
            };
            inner.Dock = DockStyle.Fill;
            p.Controls.Add(inner);
            return p;
        }

        private static void RunSym(Core.Interfaces.ICipher cipher, string tag, string op,
            Func<string> fn, RichTextBox output, Label status)
        {
            try
            {
                output.Text = fn();
                status.Text = op + " ✓";
                status.ForeColor = AppTheme.AccentSuccess;
                HistoryService.LogOperation(op, tag, "OK");
            }
            catch (Exception ex) { status.Text = ex.Message; status.ForeColor = AppTheme.AccentDanger; }
        }

        private static void Try(Action a, Label status)
        {
            try { a(); }
            catch (Exception ex) { status.Text = ex.Message; status.ForeColor = AppTheme.AccentDanger; }
        }

        private static Button MakeChip(string text)
        {
            var b = new Button { Text = text, FlatStyle = FlatStyle.Flat, Font = AppTheme.FontBodyBold, Height = 32, AutoSize = true, Padding = new Padding(14, 0, 14, 0), BackColor = Color.FromArgb(236, 237, 252), ForeColor = AppTheme.TextSecondary, Cursor = Cursors.Hand, Margin = new Padding(0, 0, 6, 0) };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 221, 252);
            return b;
        }
        private static void ActivateChip(Button b) { b.BackColor = AppTheme.Accent; b.ForeColor = Color.White; }
        private static void ResetChip(Button b)    { b.BackColor = Color.FromArgb(236, 237, 252); b.ForeColor = AppTheme.TextSecondary; }
    }
}
