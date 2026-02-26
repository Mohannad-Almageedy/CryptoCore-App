using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Services;
using MaterialSkin;
using MaterialSkin.Controls;

namespace CryptoEdu.UI.UserControls
{
    public partial class KeyGeneratorPanel : UserControl
    {
        public KeyGeneratorPanel()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(20);
        }

        private void SetupUI()
        {
            #region Password Generator Section
            var lblPassTitle = new MaterialLabel
            {
                Text = "Secure Password Generator",
                FontType = MaterialSkinManager.fontType.H5,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var lblLen = new MaterialLabel { Text = "Password Length:", Location = new Point(20, 75), AutoSize = true };
            var numLength = new NumericUpDown
            {
                Minimum = 6,
                Maximum = 128,
                Value = 20,
                Location = new Point(160, 70),
                Width = 70,
                Font = new Font("Segoe UI", 11)
            };

            var chkSpecial = new CheckBox
            {
                Text = "Include Special Characters (!@#$%...)",
                Location = new Point(20, 110),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 10)
            };

            var btnGen = new MaterialButton { Text = "Generate Password", Location = new Point(20, 150) };

            var txtPassword = new TextBox
            {
                Location = new Point(20, 200),
                Size = new Size(600, 40),
                Font = new Font("Segoe UI", 14),
                ReadOnly = true
            };
            var btnCopyPass = new MaterialButton { Text = "Copy", Type = MaterialButton.MaterialButtonType.Outlined, Location = new Point(640, 200) };

            btnGen.Click += (s, e) =>
            {
                txtPassword.Text = KeyGeneratorService.GenerateSecurePassword((int)numLength.Value, chkSpecial.Checked);
            };
            btnCopyPass.Click += (s, e) => ClipboardService.CopyToClipboard(txtPassword.Text);
            #endregion

            #region RSA Key Pair Section
            var lblRsaTitle = new MaterialLabel
            {
                Text = "RSA 2048-bit Key Pair Generator",
                FontType = MaterialSkinManager.fontType.H5,
                AutoSize = true,
                Location = new Point(20, 280)
            };

            var btnGenRsa = new MaterialButton { Text = "Generate New RSA Key Pair", Location = new Point(20, 325) };

            var lblPub = new MaterialLabel { Text = "Public Key (share this to encrypt):", Location = new Point(20, 375), AutoSize = true };
            var txtPub = new RichTextBox
            {
                Location = new Point(20, 400),
                Size = new Size(840, 100),
                Font = new Font("Consolas", 8),
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                BackColor = Color.FromArgb(245, 250, 255)
            };

            var lblPriv = new MaterialLabel { Text = "Private Key (keep this secret!):", Location = new Point(20, 510), AutoSize = true, ForeColor = Color.DarkRed };
            var txtPriv = new RichTextBox
            {
                Location = new Point(20, 535),
                Size = new Size(840, 100),
                Font = new Font("Consolas", 8),
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                BackColor = Color.FromArgb(255, 245, 245)
            };

            var btnCopyPub = new MaterialButton { Text = "Copy Public Key", Type = MaterialButton.MaterialButtonType.Outlined, Location = new Point(20, 640) };
            var btnCopyPriv = new MaterialButton { Text = "Copy Private Key", Type = MaterialButton.MaterialButtonType.Outlined, Location = new Point(200, 640) };

            btnGenRsa.Click += (s, e) =>
            {
                KeyGeneratorService.GenerateRsaKeyPair(out string pub, out string priv);
                txtPub.Text = pub;
                txtPriv.Text = priv;
                HistoryService.LogOperation("KeyGen", "RSA", "Generated new RSA 2048-bit key pair.");
            };

            btnCopyPub.Click += (s, e) => ClipboardService.CopyToClipboard(txtPub.Text);
            btnCopyPriv.Click += (s, e) => ClipboardService.CopyToClipboard(txtPriv.Text);
            #endregion

            this.Controls.AddRange(new Control[] {
                lblPassTitle, lblLen, numLength, chkSpecial, btnGen, txtPassword, btnCopyPass,
                lblRsaTitle, btnGenRsa, lblPub, txtPub, lblPriv, txtPriv, btnCopyPub, btnCopyPriv
            });
        }
    }
}
