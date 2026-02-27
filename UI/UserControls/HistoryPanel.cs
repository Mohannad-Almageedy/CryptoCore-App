using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Services;
using CryptoEdu.UI.Controls;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.UserControls
{
    public class HistoryPanel : UserControl
    {
        private DataGridView _grid = null!;

        public HistoryPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = AppTheme.ContentBg;
            BuildUI();
        }

        private void BuildUI()
        {
            var card = new RoundedPanel { Dock = DockStyle.Fill };

            // Header row
            var lbl = new Label { Text = "Operation History", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, Location = new Point(16, 12) };
            var sub = new Label { Text = "Session log of all cryptographic operations performed", Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, Location = new Point(16, 40) };

            var btnRefresh = MakePill("↻  Refresh", Color.FromArgb(238, 239, 255), new Point(580, 12), AppTheme.Accent);
            var btnClear   = MakePill("✕  Clear",   Color.FromArgb(255, 238, 238), new Point(700, 12), AppTheme.AccentDanger);

            // Grid
            _grid = new DataGridView
            {
                Bounds          = new Rectangle(16, 70, card.Width - 32, card.Height - 90),
                Anchor          = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly        = true,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White,
                BorderStyle     = BorderStyle.None,
                RowHeadersVisible = false,
                SelectionMode   = DataGridViewSelectionMode.FullRowSelect,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 249, 255) },
                Font = AppTheme.FontBody,
                GridColor = Color.FromArgb(230, 232, 245)
            };

            _grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.Accent,
                ForeColor = Color.White,
                Font      = AppTheme.FontBodyBold,
                Padding   = new Padding(6, 0, 6, 0)
            };
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            _grid.ColumnHeadersHeight = 38;

            _grid.Columns.Add("Timestamp", "Timestamp");
            _grid.Columns.Add("Operation", "Operation");
            _grid.Columns.Add("Algorithm", "Algorithm");
            _grid.Columns.Add("Details",   "Details");

            // Tag each row with a colour indicator in the Operation cell
            _grid.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == 1 && e.Value != null)
                {
                    e.CellStyle.ForeColor = e.Value.ToString() switch
                    {
                        "Encrypt" => AppTheme.Accent,
                        "Decrypt" => AppTheme.AccentSuccess,
                        "Hash"    => Color.FromArgb(250, 144, 30),
                        "KeyGen"  => AppTheme.AccentInfo,
                        _         => AppTheme.TextPrimary
                    };
                    e.CellStyle.Font = AppTheme.FontBodyBold;
                }
            };

            btnRefresh.Click += (s, e) => Refresh();
            btnClear.Click   += (s, e) => { HistoryService.ClearHistory(); Refresh(); };

            card.Controls.AddRange(new Control[] { lbl, sub, btnRefresh, btnClear, _grid });
            Controls.Add(card);

            Refresh();
        }

        public new void Refresh()
        {
            _grid.Rows.Clear();
            foreach (var e in HistoryService.GetHistory())
                _grid.Rows.Add(e.Timestamp.ToString("HH:mm:ss"), e.OperationType, e.Title, e.Description);
        }

        private static Button MakePill(string text, Color bg, Point loc, Color? fg = null)
        {
            var b = new Button { Text = text, Location = loc, AutoSize = true, FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = fg ?? Color.White, Font = AppTheme.FontBodyBold, Cursor = Cursors.Hand, Height = 34, Padding = new Padding(12, 0, 12, 0) };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(bg, 0.1f);
            return b;
        }
    }
}
