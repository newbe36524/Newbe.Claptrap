using System.Threading.Tasks;

namespace Newbe.Claptrap.Core
{
    public interface IActor
    {
        IState State { get; }

        Task ActivateAsync();

        Task DeactivateAsync();

        /// <summary>
        ///     handle new event
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task HandleEvent(IEvent @event);
    }
}