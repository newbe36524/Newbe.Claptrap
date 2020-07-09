using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IEventLoader : IClaptrapComponent
    {
        /// <summary>
        /// get events where version in range [startVersion, endVersion)
        /// </summary>
        /// <param name="startVersion"></param>
        /// <param name="endVersion"></param>
        /// <returns></returns>
        Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion);
    }
}