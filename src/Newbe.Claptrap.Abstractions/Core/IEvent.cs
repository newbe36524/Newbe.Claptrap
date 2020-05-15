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
        /// type of event
        /// </summary>
        string EventTypeCode { get; }

        IEventData Data { get; }
    }
}