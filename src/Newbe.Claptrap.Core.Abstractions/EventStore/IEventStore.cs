using System.Collections.Generic;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventStore
{
    /// <summary>
    ///     to store events for actor. each actor has it`s own event store
    /// </summary>
    public interface IEventStore
    {
        IActorIdentity Identity { get; }

        /// <summary>
        ///     save event to event store, do not throw exception
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        /// <exception cref="EventSavingException">throw if save with a exception</exception>
        Task<EventSavingResult> SaveEvent(IEvent @event);

        /// <summary>
        /// get events where version in range [startVersion, endVersion)
        /// </summary>
        /// <param name="startVersion"></param>
        /// <param name="endVersion"></param>
        /// <returns></returns>
        Task<IEnumerable<IEvent>> GetEvents(ulong startVersion, ulong endVersion);
    }
}