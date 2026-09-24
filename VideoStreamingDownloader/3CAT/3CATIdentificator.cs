using System.Threading.Tasks;
using System.Windows.Forms;
using VideoStreamingDownloader.Common;

namespace VideoStreamingDownloader._3CAT
{
    internal class _3CATIdentificator : IStreamingIdentificator
    {
        private _3CATService _3catService = new _3CATService();

        public async Task<int> IdentifyUrl(string url)
        {
            return await _3catService.IdentifyUrl(url);
        }

        public Form ShowForm()
        {
            return new Main(_3catService);
        }
    }
}
