using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Core;
using CryptoEdu.Services;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;
using System.Linq;

namespace CryptoEdu.UI.UserControls
{
    public class ClassicalCipherPanel : UserControl
    {
        public ClassicalCipherPanel()
        {
            Dock      = DockStyle.Fill;
            BackColor = AppTheme.ContentBg;
            BuildUI();
        }

        private void BuildUI()
        {
            var ciphers = CipherRegistry.GetClassicalCiphers().ToList();

            var chipBar = new FlowLayoutPanel
            {
                Dock          = DockStyle.Top,
                Height        = 54,
                BackColor     = AppTheme.ContentBg,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = false,
                Padding       = new Padding(8, 10, 8, 4)
            };

            var host = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.ContentBg };
            Button? activeChip = null;

            for (int i = 0; i < ciphers.Count; i++)
            {
                var cipher = ciphers[i];
                var detail = new CipherDetailControl(cipher);

                var chip = new Button
                {
                    Text      = cipher.Name,
                    FlatStyle = FlatStyle.Flat,
                    Font      = AppTheme.FontBodyBold,
                    Height    = 32,
                    AutoSize  = true,
                    Padding   = new Padding(14, 0, 14, 0),
                    BackColor = AppTheme.ChipBg,
                    ForeColor = AppTheme.TextSecondary,
                    Cursor    = Cursors.Hand,
                    Margin    = new Padding(0, 0, 6, 0),
                    Tag       = detail
                };
                chip.FlatAppearance.BorderSize = 0;
                chip.FlatAppearance.MouseOverBackColor = AppTheme.ChipHoverBg;

                chip.Click += (s, e) =>
                {
                    if (activeChip != null)
                    {
                        activeChip.BackColor = AppTheme.ChipBg;
                        activeChip.ForeColor = AppTheme.TextSecondary;
                    }
                    chip.BackColor = AppTheme.Accent;
                    chip.ForeColor = Color.White;
                    activeChip = chip;

                    host.Controls.Clear();
                    if (chip.Tag is Control c) { c.Dock = DockStyle.Fill; host.Controls.Add(c); }
                };

                chipBar.Controls.Add(chip);

                if (i == 0)
                {
                    chip.BackColor = AppTheme.Accent;
                    chip.ForeColor = Color.White;
                    activeChip     = chip;
                    detail.Dock    = DockStyle.Fill;
                    host.Controls.Add(detail);
                }
            }

            Controls.Add(host);
            Controls.Add(chipBar);
        }
    }
}
