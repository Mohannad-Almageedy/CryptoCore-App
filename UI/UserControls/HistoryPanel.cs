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
            var card = ControlFactory.Card();

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3,
                BackColor = Color.Transparent, Padding = Padding.Empty
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // header row
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));     // subtitle
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // grid

            var hdrRow = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1,
                BackColor = Color.Transparent, AutoSize = true
            };
            hdrRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            hdrRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            hdrRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            hdrRow.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            hdrRow.Controls.Add(ControlFactory.PageTitle("Operation History"), 0, 0);
            var btnRefresh = ControlFactory.Pill("↻  Refresh", AppTheme.ButtonMutedBg, AppTheme.ButtonMutedFg, 110);
            var btnClear   = ControlFactory.Pill("✕  Clear",   AppTheme.ButtonMutedBg, AppTheme.AccentDanger, 90);
            hdrRow.Controls.Add(btnRefresh, 1, 0);
            hdrRow.Controls.Add(btnClear,   2, 0);

            _grid = new DataGridView
            {
                Dock                    = DockStyle.Fill,
                AutoSizeColumnsMode      = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly                = true,
                AllowUserToAddRows      = false,
                AllowUserToDeleteRows   = false,
                BackgroundColor         = AppTheme.InputWrapBg,
                BorderStyle             = BorderStyle.None,
                RowHeadersVisible       = false,
                SelectionMode           = DataGridViewSelectionMode.FullRowSelect,
                Font                    = AppTheme.FontBody,
                GridColor               = AppTheme.CardBorder,
                DefaultCellStyle        = new DataGridViewCellStyle { BackColor = AppTheme.InputWrapBg, ForeColor = AppTheme.TextPrimary, SelectionBackColor = AppTheme.Accent, SelectionForeColor = Color.White },
                RowTemplate             = { Height = 34 },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = AppTheme.CardBg, ForeColor = AppTheme.TextPrimary }
            };
            _grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppTheme.CardBorder, ForeColor = AppTheme.TextPrimary, Font = AppTheme.FontBodyBold,
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
                        "Hash"    => AppTheme.AccentWarning,
                        "KeyGen"  => AppTheme.AccentInfo,
                        _         => AppTheme.TextPrimary
                    };
                    e.CellStyle.Font = AppTheme.FontBodyBold;
                }
            };

            btnRefresh.Click += (s, e) => Reload();
            btnClear.Click   += (s, e) => { HistoryService.ClearHistory(); Reload(); };

            tbl.Controls.Add(hdrRow, 0, 0);
            tbl.Controls.Add(ControlFactory.SubTitle("Session log of all cryptographic operations"), 0, 1);
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
