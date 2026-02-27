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
            var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3,
                BackColor = Color.Transparent, Padding = Padding.Empty
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // header row
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // subtitle
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // grid

            // Header row: title + buttons
            var hdrRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1,
                BackColor = Color.Transparent, AutoSize = true
            };
            hdrRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            hdrRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            hdrRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            hdrRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            hdrRow.Controls.Add(new Label { Text = "Operation History", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, AutoSize = true, BackColor = Color.Transparent }, 0, 0);
            var btnRefresh = ControlFactory.Pill("↻  Refresh", Color.FromArgb(238, 239, 255), AppTheme.Accent, 110);
            var btnClear   = ControlFactory.Pill("✕  Clear",   Color.FromArgb(255, 238, 238), AppTheme.AccentDanger, 90);
            hdrRow.Controls.Add(btnRefresh, 1, 0);
            hdrRow.Controls.Add(btnClear,   2, 0);

            var lblSub = new Label { Text = "Session log of all cryptographic operations", Font = AppTheme.FontBody, ForeColor = AppTheme.TextSecondary, AutoSize = true, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 10) };

            // Grid
            _grid = new DataGridView
            {
                Dock                    = DockStyle.Fill,
                AutoSizeColumnsMode      = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly                = true,
                AllowUserToAddRows      = false,
                AllowUserToDeleteRows   = false,
                BackgroundColor         = Color.White,
                BorderStyle             = BorderStyle.None,
                RowHeadersVisible       = false,
                SelectionMode           = DataGridViewSelectionMode.FullRowSelect,
                Font                    = AppTheme.FontBody,
                GridColor               = Color.FromArgb(230, 232, 245),
                RowTemplate             = { Height = 34 },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 249, 255) }
            };
            _grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.Accent, ForeColor = Color.White, Font = AppTheme.FontBodyBold,
                Padding = new Padding(8, 0, 0, 0)
            };
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            _grid.ColumnHeadersHeight = 38;
            _grid.Columns.Add("Time",      "Time");
            _grid.Columns.Add("Operation", "Operation");
            _grid.Columns.Add("Algorithm", "Algorithm");
            _grid.Columns.Add("Details",   "Details");
            _grid.Columns["Time"].FillWeight      = 18;
            _grid.Columns["Operation"].FillWeight = 18;
            _grid.Columns["Algorithm"].FillWeight = 20;
            _grid.Columns["Details"].FillWeight   = 44;

            _grid.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == 1 && e.Value != null)
                {
                    e.CellStyle.ForeColor = e.Value.ToString() switch
                    {
                        "Encrypt" => AppTheme.Accent,
                        "Decrypt" => AppTheme.AccentSuccess,
                        "Hash"    => Color.FromArgb(251, 140, 0),
                        "KeyGen"  => AppTheme.AccentInfo,
                        _         => AppTheme.TextPrimary
                    };
                    e.CellStyle.Font = AppTheme.FontBodyBold;
                }
            };

            btnRefresh.Click += (s, e) => Reload();
            btnClear.Click   += (s, e) => { HistoryService.ClearHistory(); Reload(); };

            tbl.Controls.Add(hdrRow, 0, 0);
            tbl.Controls.Add(lblSub, 0, 1);
            tbl.Controls.Add(_grid,  0, 2);

            card.Controls.Add(tbl);
            Controls.Add(card);
            Reload();
        }

        public void Reload()
        {
            _grid.Rows.Clear();
            foreach (var e in HistoryService.GetHistory())
                _grid.Rows.Add(e.Timestamp.ToString("HH:mm:ss"), e.OperationType, e.Title, e.Description);
        }
    }
}
