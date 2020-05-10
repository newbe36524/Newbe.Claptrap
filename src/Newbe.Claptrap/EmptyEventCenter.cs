using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    [ExcludeFromCodeCoverage]
    public class EmptyEventCenter : IEventCenter
    {
        public Task SendToMinionsAsync(IClaptrapIdentity identity, IEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}