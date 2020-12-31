using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageTestConsole.Services
{
    public interface IReportFormat<in T>
    {
        Task<string> FormatAsync(T result);
    }
}