using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IEventNotifier : IClaptrapComponent
    {
        Task Notify(IEventNotifierContext context);
    }
}