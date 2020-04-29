using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Exceptions;

namespace Newbe.Claptrap.Preview.Abstractions.Components
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