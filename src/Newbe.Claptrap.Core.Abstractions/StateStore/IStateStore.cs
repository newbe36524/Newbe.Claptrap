using System.Threading.Tasks;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.StateStore
{
    /// <summary>
    /// to store state for actor. each actor has it`s own state store
    /// </summary>
    public interface IStateStore
    {
        IActorIdentity Identity { get; }

        /// <summary>
        /// get latest state
        /// </summary>
        /// <returns>return latest state of the actor. return null if there is no data</returns>
        Task<IState?> GetStateSnapshot();

        /// <summary>
        /// save state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task Save(IState state);
    }
}