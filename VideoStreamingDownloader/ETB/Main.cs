using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VideoStreamingDownloader.ETB.Downloads;
using VideoStreamingDownloader.ETB.Media;
using VideoStreamingDownloader.ETB.Media.Extensions;

namespace VideoStreamingDownloader.ETB
{
    internal partial class Main : Form
    {
        private ETBService _RTVEService;
        private Common.EpisodeSelector.Main _episodeSelector;
        private Media.Options _mediaOptions;

        internal Main(ETBService RTVEService)
        {
            InitializeComponent();
            SetText();
            _RTVEService = RTVEService;
            LoadImage.Visible = true;
        }

        private void SetText()
        {
            VideoLabel.Text = Resources.Translations.Strings.Video;
            AudioLabel.Text = Resources.Translations.Strings.Audio;
            SubsLabel.Text = Resources.Translations.Strings.Subtitles;
            AddToDownloadButton.Text = Resources.Translations.Strings.AddToDownloads;
        }

        private void LoadOptions(Media.Options mediaOptions)
        {
            flpVideo.Controls.Clear();
            foreach (var option in mediaOptions.Videos)
            {
                flpVideo.Controls.Add(option.BuildControl());
            }
            flpAudio.Controls.Clear();
            foreach (var option in mediaOptions.Audios)
            {
                flpAudio.Controls.Add(option.BuildControl());
            }
            flpSub.Controls.Clear();
            foreach (var option in mediaOptions.Subtitles)
            {
                flpSub.Controls.Add(option.BuildControl());
            }
        }

        private async void Main_Load(object sender, System.EventArgs e)
        {
            _mediaOptions = await _RTVEService.GetMediaOptions();
            InitializeEpisodeSelector(_mediaOptions.FileInfos);
            if (_mediaOptions.FileInfos.Count > 1)
            {
                Title.Text = $"ETB: {_mediaOptions.FileInfos.Last().ProgramName} ({_mediaOptions.FileInfos.Count})";
                Select.Enabled = true;
            }
            else
            {
                Title.Text = $"ETB: {_mediaOptions.FileInfos.First().Title}";
            }
            LoadOptions(_mediaOptions);
            LoadImage.Visible = false;
            AddToDownloadButton.Enabled = true;
        }

        private void InitializeEpisodeSelector(FileInfos fileInfos)
        {
            var items = new Common.EpisodeSelector.Items();

            foreach (var fileInfo in fileInfos)
            {
                items.Add(new Common.EpisodeSelector.Item(fileInfo.Id, fileInfo.Title, fileInfo.Season));
            }

            _episodeSelector = new Common.EpisodeSelector.Main(items);
        }

        #region Download

        private IEnumerable<Item> GetDownloadItems()
        {
            List<Item> items = new List<Item>();
            Track.Video video;
            Tracks.Audio audios;
            Tracks.Subtitle subtitles;

            try
            {
                video = ReadVideoOption();
                audios = ReadAudioOption();
                subtitles = ReadSubsOption();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new List<Item>();
            }

            foreach (var episode in _mediaOptions.FileInfos)
            {
                items.Add(new Item(video, audios, subtitles, episode));
            }

            return items;
        }

        private Track.Video ReadVideoOption()
        {
            foreach (Control control in flpVideo.Controls)
            {
                if (control is RadioButton rb && rb.Checked)
                    return (Track.Video)rb.Tag;
            }

            throw new Exception(Resources.Translations.Strings.MustChooseVideo);
        }

        private Tracks.Audio ReadAudioOption()
        {
            Tracks.Audio audios = new Tracks.Audio();
            foreach (Control control in flpAudio.Controls)
            {
                if (control is CheckBox rb && rb.Checked)
                    audios.Add((Track.Audio)rb.Tag);
            }

            if (audios.Count > 0)
                return audios;

            throw new Exception(Resources.Translations.Strings.MustChooseAudio);
        }

        private Tracks.Subtitle ReadSubsOption()
        {
            Tracks.Subtitle subtitles = new Tracks.Subtitle();
            foreach (Control control in flpSub.Controls)
            {
                if (control is CheckBox rb && rb.Checked)
                    subtitles.Add((Track.Subtitle)rb.Tag);
            }

            return subtitles;
        }

        private bool GetDownloadPath()
        {
            folderBrowserDialog1.SelectedPath = Program.UserSettings.DefaultDownloadPath;
            return folderBrowserDialog1.ShowDialog() == DialogResult.OK;
        }

        private void Download_Click(object sender, System.EventArgs e)
        {
            AddToDownloadButton.Enabled = false;
            Select.Enabled = false;

            var items = GetDownloadItems();

            if (items.Any() && GetDownloadPath())
            {
                foreach (var item in items)
                {
                    item.DestinationPath = folderBrowserDialog1.SelectedPath;
                }

                Program.DownloadManager.BatchAdd(items);
                Close();
            }

            AddToDownloadButton.Enabled = true;
            Select.Enabled = true;
        }


        #endregion

        private void Select_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == _episodeSelector.ShowDialog(this))
            {
                Select.Text = $"{Resources.Translations.Strings.Selected} ({_episodeSelector.CheckedIds.Count})";
            }
        }
    }
}
