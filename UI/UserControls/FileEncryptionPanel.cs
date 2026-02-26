using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Services;
using MaterialSkin;
using MaterialSkin.Controls;

namespace CryptoEdu.UI.UserControls
{
    public partial class FileEncryptionPanel : UserControl
    {
        private string? _selectedFilePath;

        public FileEncryptionPanel()
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
                Text = "File Encryption (AES-256 Streaming)",
                FontType = MaterialSkinManager.fontType.H5,
                AutoSize = true,
                Location = new Point(20, y)
            };
            y += 50;

            var lblInfo = new Label
            {
                Text = "Encrypt/decrypt any file type (JPG, PDF, MP4, ZIP...) using AES-256. Large files are processed in chunks to avoid RAM overload.",
                Location = new Point(20, y),
                Size = new Size(820, 40),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            y += 60;

            // File Selection
            var lblFile = new MaterialLabel { Text = "Selected File:", Location = new Point(20, y), AutoSize = true };
            y += 30;
            var txtFilePath = new TextBox
            {
                Location = new Point(20, y),
                Size = new Size(620, 30),
                Font = new Font("Segoe UI", 10),
                ReadOnly = true,
                BackColor = Color.White
            };
            var btnBrowse = new MaterialButton
            {
                Text = "Browse...",
                Location = new Point(660, y - 5)
            };
            y += 60;

            // Password
            var lblKey = new MaterialLabel { Text = "Encryption Password:", Location = new Point(20, y), AutoSize = true };
            y += 30;
            var txtKey = new TextBox
            {
                Location = new Point(20, y),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 10),
                UseSystemPasswordChar = true
            };
            var btnShowPass = new MaterialButton
            {
                Text = "Show",
                Type = MaterialButton.MaterialButtonType.Text,
                Location = new Point(440, y - 5)
            };
            var btnGenKey = new MaterialButton
            {
                Text = "Generate Password",
                Type = MaterialButton.MaterialButtonType.Outlined,
                Location = new Point(530, y - 5)
            };
            y += 60;

            // Action Buttons
            var btnEncrypt = new MaterialButton { Text = "Encrypt File", Location = new Point(20, y) };
            var btnDecrypt = new MaterialButton { Text = "Decrypt File", Location = new Point(150, y) };
            y += 60;

            // Progress
            var lblStatus = new MaterialLabel { Text = "Status:", Location = new Point(20, y), AutoSize = true };
            y += 30;
            var progressBar = new MaterialProgressBar
            {
                Location = new Point(20, y),
                Size = new Size(820, 10),
                Value = 0
            };
            y += 30;
            var lblPercent = new MaterialLabel { Text = "0%", Location = new Point(20, y), AutoSize = true };

            // Handlers
            btnBrowse.Click += (s, e) =>
            {
                using var dialog = new OpenFileDialog { Filter = "All Files (*.*)|*.*" };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _selectedFilePath = dialog.FileName;
                    txtFilePath.Text = _selectedFilePath;
                }
            };

            bool passVisible = false;
            btnShowPass.Click += (s, e) =>
            {
                passVisible = !passVisible;
                txtKey.UseSystemPasswordChar = !passVisible;
                btnShowPass.Text = passVisible ? "Hide" : "Show";
            };

            btnGenKey.Click += (s, e) => txtKey.Text = KeyGeneratorService.GenerateSecurePassword(20);

            btnEncrypt.Click += async (s, e) =>
            {
                if (!ValidateInputs(txtFilePath.Text, txtKey.Text)) return;

                using var dialog = new SaveFileDialog { FileName = _selectedFilePath + ".enc", Filter = "Encrypted Files (*.enc)|*.enc|All Files|*.*" };
                if (dialog.ShowDialog() != DialogResult.OK) return;

                btnEncrypt.Enabled = btnDecrypt.Enabled = false;
                var progress = new Progress<int>(p => { progressBar.Value = p; lblPercent.Text = $"{p}%"; });
                try
                {
                    await FileEncryptionService.EncryptFileAsync(_selectedFilePath!, dialog.FileName, txtKey.Text, progress);
                    HistoryService.LogOperation("Encrypt", "File", $"Encrypted: {System.IO.Path.GetFileName(_selectedFilePath)}");
                    MessageBox.Show("File encrypted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to encrypt: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnEncrypt.Enabled = btnDecrypt.Enabled = true;
                }
            };

            btnDecrypt.Click += async (s, e) =>
            {
                if (!ValidateInputs(txtFilePath.Text, txtKey.Text)) return;

                using var dialog = new SaveFileDialog { FileName = _selectedFilePath!.Replace(".enc", ""), Filter = "All Files (*.*)|*.*" };
                if (dialog.ShowDialog() != DialogResult.OK) return;

                btnEncrypt.Enabled = btnDecrypt.Enabled = false;
                var progress = new Progress<int>(p => { progressBar.Value = p; lblPercent.Text = $"{p}%"; });
                try
                {
                    await FileEncryptionService.DecryptFileAsync(_selectedFilePath!, dialog.FileName, txtKey.Text, progress);
                    HistoryService.LogOperation("Decrypt", "File", $"Decrypted: {System.IO.Path.GetFileName(_selectedFilePath)}");
                    MessageBox.Show("File decrypted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to decrypt: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnEncrypt.Enabled = btnDecrypt.Enabled = true;
                }
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblInfo);
            this.Controls.Add(lblFile);
            this.Controls.Add(txtFilePath);
            this.Controls.Add(btnBrowse);
            this.Controls.Add(lblKey);
            this.Controls.Add(txtKey);
            this.Controls.Add(btnShowPass);
            this.Controls.Add(btnGenKey);
            this.Controls.Add(btnEncrypt);
            this.Controls.Add(btnDecrypt);
            this.Controls.Add(lblStatus);
            this.Controls.Add(progressBar);
            this.Controls.Add(lblPercent);
        }

        private bool ValidateInputs(string filePath, string key)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("Please select a file first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                MessageBox.Show("Please enter an encryption password.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
}
