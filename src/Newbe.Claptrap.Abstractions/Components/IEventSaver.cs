using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IEventSaver : IClaptrapComponent
    {
        /// <summary>
        ///     save event to event store, do not throw exception
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        /// <exception cref="EventSavingException">throw if save with a exception</exception>
        Task<EventSavingResult> SaveEventAsync(IEvent @event);
    }
}