using System.Threading.Tasks;

namespace Newbe.Claptrap.EventCenter
{
    public interface IEventCenter
    {
        Task SendToMinionsAsync(IClaptrapIdentity identity, IEvent @event);
    }
}