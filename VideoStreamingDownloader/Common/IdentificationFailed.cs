using System.Windows.Forms;

namespace VideoStreamingDownloader.Common
{
    internal partial class IdentificationFailed : Form
    {
        internal IdentificationFailed(string url)
        {
            InitializeComponent();
            label1.Text = "Enllaç no acceptat / no detectat";
            Program.LoggerService.LogWarning($"Link detection failed: {url}");
        }
    }
}
