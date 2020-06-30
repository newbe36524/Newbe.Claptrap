using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Newbe.Claptrap.EventCenter
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