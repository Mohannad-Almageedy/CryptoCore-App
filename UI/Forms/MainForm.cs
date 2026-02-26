using System;
using System.Drawing;
using System.Windows.Forms;
using CryptoEdu.UI.UserControls;
using MaterialSkin;
using MaterialSkin.Controls;

namespace CryptoEdu.UI.Forms
{
    /// <summary>
    /// The main application shell. Hosts a MaterialTabSelector and all panel UserControls.
    /// Uses MaterialSkin for a modern, professional look.
    /// </summary>
    public partial class MainForm : MaterialForm
    {
        private readonly MaterialSkinManager _materialSkinManager;

        public MainForm()
        {
            InitializeComponent();

            _materialSkinManager = MaterialSkinManager.Instance;
            _materialSkinManager.AddFormToManage(this);
            _materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            // Professional deep purple & indigo color scheme
            _materialSkinManager.ColorScheme = new ColorScheme(
                Primary.DeepPurple600,
                Primary.DeepPurple700,
                Primary.DeepPurple200,
                Accent.LightBlue200,
                TextShade.WHITE
            );

            SetupTabs();
        }

        private void InitializeComponent()
        {
            this.Text = "CryptoEdu — Educational Cryptography Suite";
            this.Size = new Size(1200, 850);
            this.MinimumSize = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Sizable = true;
        }

        private void SetupTabs()
        {
            var tabControl = new MaterialTabControl
            {
                Dock = DockStyle.Fill,
                Depth = 0,
                MouseState = MouseState.HOVER
            };

            // Tab 1: Classical Ciphers
            var tabClassical = new TabPage("Classical Ciphers") { BackColor = Color.White };
            tabClassical.Controls.Add(new ClassicalCipherPanel { Dock = DockStyle.Fill });

            // Tab 2: Modern Encryption
            var tabModern = new TabPage("Modern Encryption") { BackColor = Color.White };
            tabModern.Controls.Add(new ModernCipherPanel { Dock = DockStyle.Fill });

            // Tab 3: File Encryption
            var tabFile = new TabPage("File Encryption") { BackColor = Color.White };
            tabFile.Controls.Add(new FileEncryptionPanel { Dock = DockStyle.Fill });

            // Tab 4: Hashing
            var tabHash = new TabPage("Hashing") { BackColor = Color.White };
            tabHash.Controls.Add(new HashingPanel { Dock = DockStyle.Fill });

            // Tab 5: Key Generator
            var tabKeys = new TabPage("Key Generator") { BackColor = Color.White };
            tabKeys.Controls.Add(new KeyGeneratorPanel { Dock = DockStyle.Fill });

            // Tab 6: History
            var tabHistory = new TabPage("History") { BackColor = Color.White };
            tabHistory.Controls.Add(new HistoryPanel { Dock = DockStyle.Fill });

            tabControl.TabPages.AddRange(new TabPage[] {
                tabClassical, tabModern, tabFile, tabHash, tabKeys, tabHistory
            });

            // MaterialTabSelector renders the tabs inside the MaterialForm header
            var tabSelector = new MaterialTabSelector
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
