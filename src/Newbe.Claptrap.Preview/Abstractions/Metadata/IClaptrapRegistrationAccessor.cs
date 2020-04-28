using System;
using Newbe.Claptrap.Preview.EventStore;

namespace Newbe.Claptrap.Preview.Metadata
{
    public interface IClaptrapRegistrationAccessor
    {
        Type FindEventDataType(string actorTypeCode, string eventTypeCode);

        Type? FindEventHandlerType(string actorTypeCode, string eventTypeCode);

        /// <summary>
        /// find state type
        /// </summary>
        /// <param name="actorTypeCode"></param>
        Type FindStateDataType(string actorTypeCode);

        /// <summary>
        /// find actor type code
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string FindActorTypeCode(Type type);

        /// <summary>
        /// Find event store type
        /// </summary>
        /// <param name="actorTypeCode"></param>
        /// <returns></returns>
        EventStoreProvider FindEventStoreProvider(string actorTypeCode);

        /// <summary>
        /// Find state store type
        /// </summary>
        /// <param name="actorTypeCode"></param>
        /// <returns></returns>
        StateStoreProvider FindStateStoreProvider(string actorTypeCode);
    }
}