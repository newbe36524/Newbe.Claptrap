namespace Newbe.Claptrap
{
    public interface IEvent
    {
        /// <summary>
        /// claptrap identity
        /// </summary>
        IClaptrapIdentity ClaptrapIdentity { get; }

        /// <summary>
        /// version of event, this is a increasing number.
        /// </summary>
        long Version { get; set; }

        /// <summary>
        /// unique id of event, events with the same uid will be process only once.
        /// </summary>
        string? Uid { get; }

        /// <summary>
        /// type of event
        /// </summary>
        string EventTypeCode { get; }

        IEventData Data { get; }
    }
}