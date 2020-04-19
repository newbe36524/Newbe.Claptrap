namespace Newbe.Claptrap.EventStore
{
    /// <summary>
    /// result of saving a event to event store
    /// </summary>
    public enum EventSavingResult
    {
        /// <summary>
        /// success
        /// </summary>
        Success = 0,

        /// <summary>
        /// the event has been added before.
        /// that means the event existed which has contains the same uid 
        /// </summary>
        AlreadyAdded = 1,
    }
}