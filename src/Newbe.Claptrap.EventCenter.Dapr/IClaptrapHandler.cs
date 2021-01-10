using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Newbe.Claptrap.EventCenter.Dapr
{
    public interface IClaptrapHandler
    {
        Task HandleAsync(HttpContext context);
    }
}