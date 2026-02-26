using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Core.Modern;
using CryptoEdu.Services;
using MaterialSkin;
using MaterialSkin.Controls;

namespace CryptoEdu.UI.UserControls
{
    public partial class ModernCipherPanel : UserControl
    {
        private readonly AesCipher _aesCipher = new AesCipher();
        private readonly DesCipher _desCipher = new DesCipher();
        private readonly RsaCipher _rsaCipher = new RsaCipher();

        public ModernCipherPanel()
        {
            InitializeComponent();
            SetupUI();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
        }

        private void SetupUI()
        {
            var tabControl = new MaterialTabControl
            {
                Dock = DockStyle.Fill,
                Depth = 0,
                MouseState = MouseState.HOVER
            };

            tabControl.TabPages.Add(CreateSymmetricTab("AES-256", _aesCipher));
            tabControl.TabPages.Add(CreateSymmetricTab("Triple DES", _desCipher));
            tabControl.TabPages.Add(CreateRsaTab());

            var tabSelector = new MaterialTabSelector
            {
                BaseTabControl = tabControl,
                Dock = DockStyle.Top,
                Height = 48
            };

            this.Controls.Add(tabControl);
            this.Controls.Add(tabSelector);
        }

        private TabPage CreateSymmetricTab(string title, Core.Interfaces.ICipher cipher)
        {
            var page = new TabPage(title) { BackColor = Color.White };

            var lblTitle = new MaterialLabel { Text = $"{cipher.Name} Encryption", FontType = MaterialSkinManager.fontType.H5, Location = new Point(20, 20), AutoSize = true };
            
            var lblInput = new MaterialLabel { Text = "Plaintext / Ciphertext:", Location = new Point(20, 70), AutoSize = true };
            var txtInput = new RichTextBox { Location = new Point(20, 100), Size = new Size(500, 150), Font = new Font("Segoe UI", 10) };

            var lblKey = new MaterialLabel { Text = "Secret Key / Password:", Location = new Point(540, 70), AutoSize = true };
            var txtKey = new TextBox { Location = new Point(540, 100), Size = new Size(300, 30), Font = new Font("Segoe UI", 10) };
            
            var btnGenKey = new MaterialButton { Text = "Generate Secure Key", Type = MaterialButton.MaterialButtonType.Outlined, Location = new Point(540, 140) };

            var btnEncrypt = new MaterialButton { Text = "Encrypt", Location = new Point(20, 270) };
            var btnDecrypt = new MaterialButton { Text = "Decrypt", Location = new Point(120, 270) };
            
            var lblOutput = new MaterialLabel { Text = "Output Base64 / Plaintext:", Location = new Point(20, 320), AutoSize = true };
            var txtOutput = new RichTextBox { Location = new Point(20, 350), Size = new Size(820, 150), Font = new Font("Consolas", 10), ReadOnly = true };

            var btnCopy = new MaterialButton { Text = "Copy Output", Type = MaterialButton.MaterialButtonType.Text, Location = new Point(20, 520) };

            btnGenKey.Click += (s, e) => txtKey.Text = KeyGeneratorService.GenerateSecurePassword(16);
            
            btnEncrypt.Click += (s, e) => {
                try {
                    txtOutput.Text = cipher.Encrypt(txtInput.Text, txtKey.Text);
                    HistoryService.LogOperation("Encrypt", cipher.Name, "Successfully encrypted data.");
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Encryption Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnDecrypt.Click += (s, e) => {
                try {
                    txtOutput.Text = cipher.Decrypt(txtInput.Text, txtKey.Text);
                    HistoryService.LogOperation("Decrypt", cipher.Name, "Successfully decrypted data.");
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Decryption Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnCopy.Click += (s, e) => ClipboardService.CopyToClipboard(txtOutput.Text);

            page.Controls.Add(lblTitle);
            page.Controls.Add(lblInput);
            page.Controls.Add(txtInput);
            page.Controls.Add(lblKey);
            page.Controls.Add(txtKey);
            page.Controls.Add(btnGenKey);
            page.Controls.Add(btnEncrypt);
            page.Controls.Add(btnDecrypt);
            page.Controls.Add(lblOutput);
            page.Controls.Add(txtOutput);
            page.Controls.Add(btnCopy);

            return page;
        }

        private TabPage CreateRsaTab()
        {
            var page = new TabPage("RSA") { BackColor = Color.White };

            var lblTitle = new MaterialLabel { Text = "RSA Asymmetric Cryptography", FontType = MaterialSkinManager.fontType.H5, Location = new Point(20, 20), AutoSize = true };
            
            var btnGenerate = new MaterialButton { Text = "Generate New 2048-bit RSA Key Pair", Location = new Point(20, 60) };
            
            var lblPub = new MaterialLabel { Text = "Public Key (XML):", Location = new Point(20, 110), AutoSize = true };
            var txtPub = new RichTextBox { Location = new Point(20, 140), Size = new Size(400, 100), Font = new Font("Consolas", 8) };

            var lblPriv = new MaterialLabel { Text = "Private Key (XML):", Location = new Point(440, 110), AutoSize = true };
            var txtPriv = new RichTextBox { Location = new Point(440, 140), Size = new Size(400, 100), Font = new Font("Consolas", 8) };

            var lblInput = new MaterialLabel { Text = "Input Text:", Location = new Point(20, 260), AutoSize = true };
            var txtInput = new RichTextBox { Location = new Point(20, 290), Size = new Size(820, 100), Font = new Font("Segoe UI", 10) };

            var btnEncrypt = new MaterialButton { Text = "Encrypt (Needs Public Key)", Location = new Point(20, 410) };
            var btnDecrypt = new MaterialButton { Text = "Decrypt (Needs Private Key)", Location = new Point(270, 410) };

            var lblOutput = new MaterialLabel { Text = "Output Base64 / Plaintext:", Location = new Point(20, 460), AutoSize = true };
            var txtOutput = new RichTextBox { Location = new Point(20, 490), Size = new Size(820, 100), Font = new Font("Consolas", 10), ReadOnly = true };

            btnGenerate.Click += (s, e) => {
                KeyGeneratorService.GenerateRsaKeyPair(out string pub, out string priv);
                txtPub.Text = pub;
                txtPriv.Text = priv;
            };

            btnEncrypt.Click += (s, e) => {
                try {
                    txtOutput.Text = _rsaCipher.Encrypt(txtInput.Text, txtPub.Text);
                    HistoryService.LogOperation("Encrypt", "RSA", "Encrypted using Public Key.");
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnDecrypt.Click += (s, e) => {
                try {
                    txtOutput.Text = _rsaCipher.Decrypt(txtInput.Text, txtPriv.Text);
                    HistoryService.LogOperation("Decrypt", "RSA", "Decrypted using Private Key.");
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            page.Controls.Add(lblTitle);
            page.Controls.Add(btnGenerate);
            page.Controls.Add(lblPub);
            page.Controls.Add(txtPub);
            page.Controls.Add(lblPriv);
            page.Controls.Add(txtPriv);
            page.Controls.Add(lblInput);
            page.Controls.Add(txtInput);
            page.Controls.Add(btnEncrypt);
            page.Controls.Add(btnDecrypt);
            page.Controls.Add(lblOutput);
            page.Controls.Add(txtOutput);

            return page;
        }
    }
}
