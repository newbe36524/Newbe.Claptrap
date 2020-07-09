using System.Threading.Tasks;

namespace Newbe.Claptrap
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