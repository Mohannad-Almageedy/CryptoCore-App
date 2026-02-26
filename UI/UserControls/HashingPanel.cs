using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Services;
using MaterialSkin;
using MaterialSkin.Controls;

namespace CryptoEdu.UI.UserControls
{
    public partial class HashingPanel : UserControl
    {
        public HashingPanel()
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
            int y = 20;

            var lblTitle = new MaterialLabel
            {
                Text = "Cryptographic Hash Functions",
                FontType = MaterialSkinManager.fontType.H5,
                AutoSize = true,
                Location = new Point(20, y)
            };
            y += 50;

            var lblNote = new Label
            {
                Text = "Hashing is a one-way mathematical function. It cannot be reversed/decrypted.",
                Location = new Point(20, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            };
            y += 40;

            var lblInput = new MaterialLabel { Text = "Input Text:", Location = new Point(20, y), AutoSize = true };
            y += 30;
            var txtInput = new RichTextBox
            {
                Location = new Point(20, y),
                Size = new Size(820, 100),
                Font = new Font("Segoe UI", 10),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            y += 120;

            // HMAC Key
            var lblHmacKey = new MaterialLabel { Text = "HMAC Secret Key (only for HMAC):", Location = new Point(20, y), AutoSize = true };
            y += 30;
            var txtHmacKey = new TextBox
            {
                Location = new Point(20, y),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 10)
            };
            y += 50;

            // Buttons Row
            var btnMD5 = new MaterialButton { Text = "MD5", Location = new Point(20, y) };
            var btnSHA1 = new MaterialButton { Text = "SHA-1", Location = new Point(90, y) };
            var btnSHA256 = new MaterialButton { Text = "SHA-256", Location = new Point(170, y) };
            var btnSHA512 = new MaterialButton { Text = "SHA-512", Location = new Point(270, y) };
            var btnHMAC = new MaterialButton { Text = "HMAC-SHA256", Location = new Point(380, y) };
            y += 60;

            // Output
            var lblOutput = new MaterialLabel { Text = "Hash Result (Hex):", Location = new Point(20, y), AutoSize = true };
            y += 30;
            var txtOutput = new RichTextBox
            {
                Location = new Point(20, y),
                Size = new Size(820, 80),
                Font = new Font("Consolas", 11),
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 245, 245)
            };
            y += 100;

            var btnCopy = new MaterialButton
            {
                Text = "Copy Hash",
                Type = MaterialButton.MaterialButtonType.Text,
                Location = new Point(20, y)
            };

            // Handlers
            btnMD5.Click += (s, e) => TryHash(() => HashingService.ComputeMD5(txtInput.Text), txtOutput, "MD5");
            btnSHA1.Click += (s, e) => TryHash(() => HashingService.ComputeSHA1(txtInput.Text), txtOutput, "SHA-1");
            btnSHA256.Click += (s, e) => TryHash(() => HashingService.ComputeSHA256(txtInput.Text), txtOutput, "SHA-256");
            btnSHA512.Click += (s, e) => TryHash(() => HashingService.ComputeSHA512(txtInput.Text), txtOutput, "SHA-512");
            btnHMAC.Click += (s, e) => {
                try {
                    txtOutput.Text = HashingService.ComputeHMACSHA256(txtInput.Text, txtHmacKey.Text);
                    HistoryService.LogOperation("Hash", "HMAC-SHA256", "Computed HMAC-SHA256 hash.");
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "HMAC Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            btnCopy.Click += (s, e) => ClipboardService.CopyToClipboard(txtOutput.Text);

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblNote);
            this.Controls.Add(lblInput);
            this.Controls.Add(txtInput);
            this.Controls.Add(lblHmacKey);
            this.Controls.Add(txtHmacKey);
            this.Controls.Add(btnMD5);
            this.Controls.Add(btnSHA1);
            this.Controls.Add(btnSHA256);
            this.Controls.Add(btnSHA512);
            this.Controls.Add(btnHMAC);
            this.Controls.Add(lblOutput);
            this.Controls.Add(txtOutput);
            this.Controls.Add(btnCopy);
        }

        private void TryHash(Func<string> hashFunc, RichTextBox output, string algoName)
        {
            try
            {
                output.Text = hashFunc();
                HistoryService.LogOperation("Hash", algoName, $"Computed {algoName} hash.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hash Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
