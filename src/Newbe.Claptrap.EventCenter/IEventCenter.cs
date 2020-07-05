using System.Threading.Tasks;

namespace Newbe.Claptrap.EventCenter
{
    public interface IEventCenter
    {
        Task SendToMinionsAsync(IClaptrapIdentity masterId, IEvent @event);
    }
}