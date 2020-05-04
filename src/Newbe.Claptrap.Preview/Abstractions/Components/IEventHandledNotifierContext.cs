using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Components
{
    public interface IEventHandledNotifierContext
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
        IState EarlierState { get; }
    }
}