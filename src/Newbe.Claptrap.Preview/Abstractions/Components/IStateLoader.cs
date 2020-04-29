using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Components
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