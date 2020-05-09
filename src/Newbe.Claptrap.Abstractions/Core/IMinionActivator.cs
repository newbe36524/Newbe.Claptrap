using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IMinionActivator
    {
        Task WakeAsync(IClaptrapIdentity identity);
    }
}