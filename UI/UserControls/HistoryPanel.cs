using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.Services;
using MaterialSkin;
using MaterialSkin.Controls;

namespace CryptoEdu.UI.UserControls
{
    public partial class HistoryPanel : UserControl
    {
        private DataGridView? _grid;

        public HistoryPanel()
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
            var lblTitle = new MaterialLabel
            {
                Text = "Operation History",
                FontType = MaterialSkinManager.fontType.H5,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var btnRefresh = new MaterialButton { Text = "Refresh", Location = new Point(700, 15) };
            var btnClear = new MaterialButton { Text = "Clear History", Type = MaterialButton.MaterialButtonType.Outlined, Location = new Point(800, 15) };

            _grid = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(900, 500),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(103, 58, 183),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                },
                Font = new Font("Segoe UI", 10)
            };

            _grid.Columns.Add("Timestamp", "Timestamp");
            _grid.Columns.Add("Operation", "Operation");
            _grid.Columns.Add("Algorithm", "Algorithm");
            _grid.Columns.Add("Description", "Description");

            btnRefresh.Click += (s, e) => RefreshHistory();
            btnClear.Click += (s, e) =>
            {
                HistoryService.ClearHistory();
                RefreshHistory();
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnClear);
            this.Controls.Add(_grid);

            RefreshHistory();
        }

        public void RefreshHistory()
        {
            if (_grid == null) return;
            _grid.Rows.Clear();
            foreach (var entry in HistoryService.GetHistory())
            {
                _grid.Rows.Add(
                    entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    entry.OperationType,
                    entry.Title,
                    entry.Description
                );
            }
        }
    }
}
