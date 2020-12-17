using System.IO;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageTestConsole.Services
{
    public interface IReportManager
    {
        Task InitAsync();
        FileStream CreateFile(string filename);
    }
}