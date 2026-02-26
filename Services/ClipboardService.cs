using System.Windows.Forms;
using System.Threading;

namespace CryptoEdu.Services
{
    /// <summary>
    /// Clipboard wrapper ensuring STA thread safety in Windows Forms.
    /// </summary>
    public static class ClipboardService
    {
        public static void CopyToClipboard(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            Thread t = new Thread(() => Clipboard.SetText(text));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }
    }
}
