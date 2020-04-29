using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Components
{
    public interface IStateSaver : IClaptrapComponent
    {
        /// <summary>
        /// save state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        Task SaveAsync(IState state);
    }
}