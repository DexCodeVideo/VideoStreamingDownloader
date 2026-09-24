using System.Threading.Tasks;
using System.Windows.Forms;
using VideoStreamingDownloader.Common;

namespace VideoStreamingDownloader.ETB
{
    internal class ETBIdentificator : IStreamingIdentificator
    {
        private ETBService _RTVEService = new ETBService();

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
