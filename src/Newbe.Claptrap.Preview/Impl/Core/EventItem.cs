using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public struct EventItem
    {
        public IEvent Event { get; set; }
        public TaskCompletionSource<int> TaskCompletionSource { get; set; }
    }
}