using System.Threading.Tasks;

namespace Newbe.Claptrap.Preview.Abstractions.Components
{
    public interface IEventHandledNotifier : IClaptrapComponent
    {
        Task Notify(IEventHandledNotifierContext context);
    }
}