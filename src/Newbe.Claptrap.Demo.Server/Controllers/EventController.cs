using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newbe.Claptrap.Demo.Server.Services;

namespace Newbe.Claptrap.Demo.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IDaprActorTestService _daprActorTestService;

        public EventController(
            IDaprActorTestService daprActorTestService)
        {
            _daprActorTestService = daprActorTestService;
        }

        [HttpGet]
        public async Task<IActionResult> InsertAsync()
        {
            var count = await _daprActorTestService.RunAsync();
            return Content(count.ToString());
        }
    }
}