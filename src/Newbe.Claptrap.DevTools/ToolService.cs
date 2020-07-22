using System.Threading.Tasks;

namespace Newbe.Claptrap.DevTools
{
    public class ToolService : IToolService
    {
        public Task RunAsync()
        {
            return Task.CompletedTask;
        }
    }
}