using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IClaptrap
    {
        IState State { get; }

        /// <summary>
        /// activate actor
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ActivateFailException">thrown if activated failed</exception>
        Task ActivateAsync();

        Task DeactivateAsync();

        /// <summary>
        ///     handle new event
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task HandleEventAsync(IEvent @event);
    }
}