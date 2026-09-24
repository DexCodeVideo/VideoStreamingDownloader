using System.Threading.Tasks;
using System.Windows.Forms;
using VideoStreamingDownloader.Common;

namespace VideoStreamingDownloader.RTVE
{
    internal class RTVEIdentificator : IStreamingIdentificator
    {
        private RTVEService _RTVEService = new RTVEService();

        public async Task<int> IdentifyUrl(string url)
        {
            return await _RTVEService.IdentifyUrl(url);
        }

        public Form ShowForm()
        {
            return new Main(_RTVEService);
        }
    }
}
