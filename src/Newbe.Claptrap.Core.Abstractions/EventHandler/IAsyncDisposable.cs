using System.Threading.Tasks;

namespace Newbe.Claptrap.EventHandler
{
    /// <summary>
    /// TODO this interface will be replace in netcoreapp 3.0
    /// </summary>
    public interface IAsyncDisposable
    {
        ValueTask DisposeAsync();
    }
}