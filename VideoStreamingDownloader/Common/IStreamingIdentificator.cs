using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoStreamingDownloader.Common
{
    internal interface IStreamingIdentificator
    {
        Task<int> IdentifyUrl(string url);
        Form ShowForm();
    }
}
