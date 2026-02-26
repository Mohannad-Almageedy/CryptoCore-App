using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CryptoEdu.Core;
using MaterialSkin;
using MaterialSkin.Controls;

namespace CryptoEdu.UI.UserControls
{
    public partial class ClassicalCipherPanel : UserControl
    {
        public ClassicalCipherPanel()
        {
            InitializeComponent();
            SetupClassicalTabs();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
        }

        private void SetupClassicalTabs()
        {
            var tabControl = new MaterialTabControl
            {
                Dock = DockStyle.Fill,
                Depth = 0,
                MouseState = MouseState.HOVER
            };

            var classicalCiphers = CipherRegistry.GetClassicalCiphers().ToList();

            foreach (var cipher in classicalCiphers)
            {
                var tabPage = new TabPage(cipher.Name)
                {
                    BackColor = Color.White
                };
                
                var detailControl = new CipherDetailControl(cipher)
                {
                    Dock = DockStyle.Fill
                };

                tabPage.Controls.Add(detailControl);
                tabControl.TabPages.Add(tabPage);
            }

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
