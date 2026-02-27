using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CryptoEdu.Core;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.UserControls
{
    /// <summary>
    /// Hosts a tab strip of all classical ciphers, each rendered by CipherDetailControl.
    /// </summary>
    public class ClassicalCipherPanel : UserControl
    {
        public ClassicalCipherPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = AppTheme.ContentBg;
            BuildUI();
        }

        private void BuildUI()
        {
            var ciphers = CipherRegistry.GetClassicalCiphers().ToList();

            // ── Cipher chips row ─────────────────────────────────────
            var chipBar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 48,
                BackColor = AppTheme.ContentBg
            };

            // ── Content host ─────────────────────────────────────────
            var content = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = AppTheme.ContentBg
            };

            Button? activeChip = null;

            for (int i = 0; i < ciphers.Count; i++)
            {
                var cipher = ciphers[i];
                var detail = new CipherDetailControl(cipher) { Dock = DockStyle.Fill };

                // chip button
                var chip = new Button
                {
                    Text      = cipher.Name,
                    FlatStyle = FlatStyle.Flat,
                    Font      = AppTheme.FontBodyBold,
                    Height    = 34,
                    AutoSize  = true,
                    Padding   = new Padding(14, 0, 14, 0),
                    BackColor = Color.FromArgb(238, 239, 255),
                    ForeColor = AppTheme.TextSecondary,
                    Cursor    = Cursors.Hand,
                    Tag       = detail
                };
                chip.FlatAppearance.BorderSize = 0;
                chip.Location = new Point(i * (chip.PreferredSize.Width + 6), 7);

                chip.Click += (s, e) =>
                {
                    // Reset previous
                    if (activeChip != null)
                    {
                        activeChip.BackColor = Color.FromArgb(238, 239, 255);
                        activeChip.ForeColor = AppTheme.TextSecondary;
                    }

                    chip.BackColor = AppTheme.Accent;
                    chip.ForeColor = Color.White;
                    activeChip = chip;

                    content.Controls.Clear();
                    if (chip.Tag is Control c)
                    {
                        c.Dock = DockStyle.Fill;
                        content.Controls.Add(c);
                    }
                };

                chipBar.Controls.Add(chip);

                // Auto-select first
                if (i == 0)
                {
                    chip.BackColor = AppTheme.Accent;
                    chip.ForeColor = Color.White;
                    activeChip = chip;
                    detail.Dock = DockStyle.Fill;
                    content.Controls.Add(detail);
                }
            }

            Controls.Add(content);
            Controls.Add(chipBar);
        }
    }
}
