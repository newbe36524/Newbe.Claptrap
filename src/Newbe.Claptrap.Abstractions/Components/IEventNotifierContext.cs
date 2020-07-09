namespace Newbe.Claptrap
{
    public interface IEventNotifierContext
    {
        /// <summary>
        /// what event handled
        /// </summary>
        IEvent Event { get; }

        /// <summary>
        /// state ofter event handled
        /// </summary>
        IState CurrentState { get; }

        /// <summary>
        /// state before event handled
        /// </summary>
        IState OldState { get; }
    }
}