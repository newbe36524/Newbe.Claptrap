using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions
{
    public interface IMinionActivator
    {
        Task WakeAsync(IClaptrapIdentity identity);
    }
}