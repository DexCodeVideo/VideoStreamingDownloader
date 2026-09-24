using System.Drawing;
using System.Windows.Forms;

namespace VideoStreamingDownloader.RTVE.Media.Extensions
{
    internal static class TrackExtension
    {
        internal static RadioButton BuildControl(this Track.Video video)
        {
            return new RadioButton()
            {
                Text = video.Resolution,
                Font = new Font("Microsoft Sans Serif", 15),
                AutoSize = true,
                Tag = video
            };
        }

        internal static CheckBox BuildControl(this Track.Audio audio)
        {
            return new CheckBox()
            {
                Text = audio.Language.Description,
                Font = new Font("Microsoft Sans Serif", 15),
                AutoSize = true,
                Tag = audio
            };
        }

        internal static CheckBox BuildControl(this Track.Subtitle subtitle)
        {
            return new CheckBox()
            {
                Text = subtitle.Language.Description,
                Font = new Font("Microsoft Sans Serif", 15),
                AutoSize = true,
                Tag = subtitle
            };
        }
    }
}
