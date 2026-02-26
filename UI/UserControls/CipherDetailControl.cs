using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CryptoEdu.Core.Interfaces;
using CryptoEdu.Core.Models;
using MaterialSkin;
using MaterialSkin.Controls;

namespace CryptoEdu.UI.UserControls
{
    public partial class CipherDetailControl : UserControl
    {
        private readonly IClassicalCipher _cipher;

        public CipherDetailControl(IClassicalCipher cipher)
        {
            _cipher = cipher;
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
            // Title and Math Rule
            var lblTitle = new MaterialLabel
            {
                Text = _cipher.Name,
                FontType = MaterialSkinManager.fontType.H4,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var txtRule = new RichTextBox
            {
                Text = "Mathematical Rule:\n" + _cipher.GetMathematicalRule(),
                ReadOnly = true,
                Location = new Point(20, 60),
                Size = new Size(800, 80),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            // Input Area
            var lblInput = new MaterialLabel { Text = "Input Text:", Location = new Point(20, 160), AutoSize = true };
            var _txtInput = new RichTextBox {
                Location = new Point(20, 190), Size = new Size(380, 100)
            };

            var lblKey = new MaterialLabel { Text = "Key:", Location = new Point(420, 160), AutoSize = true };
            var _txtKey = new TextBox { Location = new Point(420, 190), Size = new Size(380, 50), Multiline = true };

            // Buttons
            var btnEncrypt = new MaterialButton { Text = "Encrypt", Location = new Point(20, 310) };
            var btnDecrypt = new MaterialButton { Text = "Decrypt", Location = new Point(120, 310) };
            var btnClear = new MaterialButton { Text = "Clear", Type = MaterialButton.MaterialButtonType.Outlined, Location = new Point(220, 310) };

            // Output Area
            var lblOutput = new MaterialLabel { Text = "Output Text:", Location = new Point(20, 360), AutoSize = true };
            var _txtOutput = new RichTextBox {
                Location = new Point(20, 390), Size = new Size(780, 100), ReadOnly = true
            };

            // Steps Area
            var lblSteps = new MaterialLabel { Text = "Step-by-Step Breakdown:", Location = new Point(20, 510), AutoSize = true };
            var _txtSteps = new RichTextBox {
                Location = new Point(20, 540), Size = new Size(780, 200), ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Font = new Font("Consolas", 10)
            };

            // Event Handlers
            btnEncrypt.Click += (s, e) => {
                try {
                    string inputText = _txtInput.Text?.ToString() ?? "";
                    string keyText = _txtKey.Text?.ToString() ?? "";
                    _txtOutput.Text = _cipher.Encrypt(inputText, keyText);
                    DisplaySteps(_txtSteps, _cipher.GetEncryptionSteps(inputText, keyText));
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnDecrypt.Click += (s, e) => {
                try {
                    string inputText = _txtInput.Text?.ToString() ?? "";
                    string keyText = _txtKey.Text?.ToString() ?? "";
                    _txtOutput.Text = _cipher.Decrypt(inputText, keyText);
                    DisplaySteps(_txtSteps, _cipher.GetDecryptionSteps(inputText, keyText));
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnClear.Click += (s, e) => {
                _txtInput.Clear();
                _txtKey.Clear();
                _txtOutput.Clear();
                _txtSteps.Clear();
            };

            // Add Controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(txtRule);
            this.Controls.Add(lblInput);
            this.Controls.Add(_txtInput);
            this.Controls.Add(lblKey);
            this.Controls.Add(_txtKey);
            this.Controls.Add(btnEncrypt);
            this.Controls.Add(btnDecrypt);
            this.Controls.Add(btnClear);
            this.Controls.Add(lblOutput);
            this.Controls.Add(_txtOutput);
            this.Controls.Add(lblSteps);
            this.Controls.Add(_txtSteps);
        }

        private void DisplaySteps(RichTextBox textBox, List<CipherStep> steps)
        {
            var sb = new StringBuilder();
            foreach (var step in steps)
            {
                sb.AppendLine("--------------------------------------------------");
                sb.AppendLine($"[ {step.Title} ]");
                sb.AppendLine(step.Description);
                string visData = step.VisualizationData?.ToString() ?? "";
                if (!string.IsNullOrEmpty(visData))
                {
                    sb.AppendLine("Visualization:");
                    sb.AppendLine(visData);
                }
                if (!string.IsNullOrEmpty(step.FormulaApplied))
                {
                    sb.AppendLine($"Formula: {step.FormulaApplied}");
                }
                if (!string.IsNullOrEmpty(step.InputState))
                {
                    sb.AppendLine($"Input:  {step.InputState}");
                    sb.AppendLine($"Output: {step.OutputState}");
                }
                sb.AppendLine();
            }
            textBox.Text = sb.ToString();
        }
    }
}
