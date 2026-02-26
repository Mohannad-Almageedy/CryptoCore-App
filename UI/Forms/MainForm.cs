using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace CryptoEdu.UI.Forms
{
    public partial class MainForm : MaterialForm
    {
        private readonly MaterialSkinManager _materialSkinManager;

        public MainForm()
        {
            InitializeComponent();

            // Initialize MaterialSkinManager
            _materialSkinManager = MaterialSkinManager.Instance;
            _materialSkinManager.AddFormToManage(this);
            _materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            
            // Apply professional Purple/Indigo theme to match "aesthetic" requirements
            _materialSkinManager.ColorScheme = new ColorScheme(
                Primary.DeepPurple600, Primary.DeepPurple700,
                Primary.DeepPurple200, Accent.LightBlue200,
                TextShade.WHITE
            );

            SetupTabs();
        }

        private void InitializeComponent()
        {
            this.Text = "CryptoEdu - Educational Cryptography Suite";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Sizable = true;
        }

        private void SetupTabs()
        {
            // Main TabControl driven by MaterialSkin
            MaterialTabControl tabControl = new MaterialTabControl
            {
                Dock = DockStyle.Fill,
                Depth = 0,
                MouseState = MouseState.HOVER
            };

            // Classic Ciphers Tab
            TabPage tabClassical = new TabPage("Classical Ciphers");
            tabClassical.BackColor = Color.White;
            tabClassical.Controls.Add(new Label { Text = "Classical Ciphers Panel Placeholder", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            
            // Modern Ciphers Tab
            TabPage tabModern = new TabPage("Modern Ciphers");
            tabModern.BackColor = Color.White;
            tabModern.Controls.Add(new Label { Text = "Modern Ciphers Panel Placeholder", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });

            // File Encryption Tab
            TabPage tabFile = new TabPage("File Encryption");
            tabFile.BackColor = Color.White;
            tabFile.Controls.Add(new Label { Text = "File Encryption Panel Placeholder", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });

            // Hashing Tab
            TabPage tabHash = new TabPage("Hashing");
            tabHash.BackColor = Color.White;
            tabHash.Controls.Add(new Label { Text = "Hashing Panel Placeholder", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });

            // History Tab
            TabPage tabHistory = new TabPage("History");
            tabHistory.BackColor = Color.White;
            tabHistory.Controls.Add(new Label { Text = "History Panel Placeholder", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });

            tabControl.TabPages.Add(tabClassical);
            tabControl.TabPages.Add(tabModern);
            tabControl.TabPages.Add(tabFile);
            tabControl.TabPages.Add(tabHash);
            tabControl.TabPages.Add(tabHistory);

            // A MaterialTabSelector to display the tabs natively in the form header
            MaterialTabSelector tabSelector = new MaterialTabSelector
            {
                BaseTabControl = tabControl,
                Dock = DockStyle.Top,
                Height = 48
            };

            this.Controls.Add(tabControl);
            this.Controls.Add(tabSelector);
        }
    }
}
