using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using CryptoEdu.UI.Theme;

namespace CryptoEdu.UI.Controls
{
    /// <summary>
    /// Elegant text box with a styled bottom-line focus indicator and placeholder text.
    /// </summary>
    public class StyledTextBox : UserControl
    {
        private readonly TextBox _inner = new TextBox();
        private readonly Label   _lblPlaceholder = new Label();

        private string _placeholder = "";
        private bool   _isFocused;
        private bool   _isPassword;

        public string Placeholder
        {
            get => _placeholder;
            set { _placeholder = value; _lblPlaceholder.Text = value; }
        }

        public bool IsPassword
        {
            get => _isPassword;
            set { _isPassword = value; _inner.UseSystemPasswordChar = value; }
        }

        public override string Text
        {
            get => _inner.Text;
            set { _inner.Text = value; }
        }

        public StyledTextBox()
        {
            Height = 46;
            BackColor = Color.White;
            Padding = new Padding(0);
            DoubleBuffered = true;

            _inner.BorderStyle = BorderStyle.None;
            _inner.Font = AppTheme.FontBody;
            _inner.ForeColor = AppTheme.TextPrimary;
            _inner.BackColor = Color.Transparent;
            _inner.Location = new Point(4, 20);
            _inner.Size = new Size(Width - 8, 20);
            _inner.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            _lblPlaceholder.ForeColor = AppTheme.TextMuted;
            _lblPlaceholder.Font = AppTheme.FontBody;
            _lblPlaceholder.AutoSize = false;
            _lblPlaceholder.Size = new Size(Width - 8, 20);
            _lblPlaceholder.Location = new Point(4, 20);
            _lblPlaceholder.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            _lblPlaceholder.Visible = true;

            Controls.Add(_lblPlaceholder);
            Controls.Add(_inner);

            // Wire events
            _inner.GotFocus  += (s, e) => { _isFocused = true;  _lblPlaceholder.Visible = false; Invalidate(); };
            _inner.LostFocus += (s, e) => { _isFocused = false; _lblPlaceholder.Visible = string.IsNullOrEmpty(_inner.Text); Invalidate(); };
            _inner.TextChanged += (s, e) => { _lblPlaceholder.Visible = !_isFocused && string.IsNullOrEmpty(_inner.Text); };
            _lblPlaceholder.Click += (s, e) => _inner.Focus();

            Resize += (s, e) =>
            {
                _inner.Width = Width - 8;
                _lblPlaceholder.Width = Width - 8;
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            int lineY = Height - 2;
            var baseColor = Color.FromArgb(220, 222, 235);
            using (var pen = new Pen(_isFocused ? AppTheme.Accent : baseColor, _isFocused ? 2 : 1))
                e.Graphics.DrawLine(pen, 4, lineY, Width - 4, lineY);
        }
    }
}
