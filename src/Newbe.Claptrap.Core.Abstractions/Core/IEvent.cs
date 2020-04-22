namespace Newbe.Claptrap.Core
{
    public interface IEvent
    {
        /// <summary>
        /// actor identity
        /// </summary>
        IActorIdentity ActorIdentity { get; }

        /// <summary>
        /// version of event, this is a increasing number.
        /// </summary>
        ulong Version { get; set; }

        /// <summary>
        /// unique id of event, events with the same uid will be process only once.
        /// </summary>
        IEventUid Uid { get; }

        /// <summary>
        /// type of event
        /// </summary>
        string EventTypeCode { get; }

        IEventData Data { get; }
    }
}