using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IEventNotifierHandler
    {
        Task Notify(IEventNotifierContext context);
    }
}