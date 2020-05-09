using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IStateLoader : IClaptrapComponent
    {
        /// <summary>
        /// get latest state
        /// </summary>
        /// <returns>return latest state of the actor. return null if there is no data</returns>
        Task<IState?> GetStateSnapshotAsync();
    }
}