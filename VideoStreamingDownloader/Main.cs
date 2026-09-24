using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoStreamingDownloader._3CAT;
using VideoStreamingDownloader.Common;
using VideoStreamingDownloader.ETB;
using VideoStreamingDownloader.RTVE;

namespace VideoStreamingDownloader
{
    internal partial class Main : Form
    {
        IStreamingIdentificator _streamingDownloader;

        internal Main()
        {
            InitializeComponent();
            SetTexts();
            Program.DownloadManager.FirstLoadDownloadsCue();
            UpdateDownloadButton();
        }

        private void SetTexts()
        {
            preferenciesToolStripMenuItem.Text = Resources.Translations.Strings.Preferences;
            DownloaderViewerToolStripMenuItem.Text = Resources.Translations.Strings.DownloadViewer;
            DownloadButton.Text = Resources.Translations.Strings.Download;
            Search.Text = Resources.Translations.Strings.Search;
            InputUrl.Text = Resources.Translations.Strings.LinkHere;
        }

        private async void Download_Click(object sender, EventArgs e)
        {
            DownloadButton.Enabled = false;
            await Task.Run(() => Program.DownloadManager.StartDownload());
            DownloadButton.Enabled = Program.DownloadManager.DownloadEnable;
        }

        private void ProgressBarUpdate()
        {
            progressBar.Value = Program.DownloadManager.FinishedDownloads;
            progressBar.Maximum = Program.DownloadManager.TotalDownloads;
            double percentage = progressBar.Value > 0 ? (progressBar.Value * 1.0 / progressBar.Maximum * 100) : 0;
            ProgressLabel.Text = $"{progressBar.Value.ToString("000")}/{progressBar.Maximum.ToString("000")} " +
                $"({percentage.ToString("0.0")}%)";
        }
        private async Task BackgroundMonitor()
        {
            while (true)
            {
                await Task.Delay(500);
                ProgressBarUpdate();
            }
        }

        private async void Search_Click(object sender, EventArgs e)
        {
            await SearchAction();
        }

        private void setMainPanel(Form form)
        {
            MainPanel.Controls.Clear();
            form.TopLevel = false;
            MainPanel.Controls.Add(form);
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.Show();
        }

        private async Task SearchAction()
        {
            Search.Enabled = false;
            _streamingDownloader = await IdentifyProvider(InputUrl.Text);
            if (_streamingDownloader == null)
                setMainPanel(new IdentificationFailed());
            else
                setMainPanel(_streamingDownloader.ShowForm());
            Search.Enabled = true;
        }

        private async Task<IStreamingIdentificator> IdentifyProvider(string url)
        {
            List<IStreamingIdentificator> linkInfos = new List<IStreamingIdentificator>() {
                new _3CATIdentificator(),
                new RTVEIdentificator(),
                new ETBIdentificator(),
            };

            var results = await Task.WhenAll(
                linkInfos.Select(async x => new
                {
                    Identificator = x,
                    MatchScore = await x.IdentifyUrl(url)
                })
            );

            return results.OrderByDescending(x => x.MatchScore).FirstOrDefault(x => x.MatchScore > 0)?.Identificator;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.DownloadManager.Abort();
        }

        private void MainPanel_ControlRemoved(object sender, ControlEventArgs e)
        {
            UpdateDownloadButton();
        }

        private void UpdateDownloadButton()
        {
            DownloadButton.Enabled = Program.DownloadManager.DownloadEnable;
        }

        private void preferenciesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new UserSettingsForm().ShowDialog();
        }

        private void DownloaderViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Common.Downloads.Viewer().ShowDialog();
            DownloadButton.Enabled = Program.DownloadManager.DownloadEnable;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            _ = BackgroundMonitor();
        }
    }
}

