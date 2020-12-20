using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newbe.Claptrap.Demo.Server.Services;

namespace Newbe.Claptrap.Demo.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IOrleansActorTestService _orleansActorTestService;

        public EventController(
            IOrleansActorTestService orleansActorTestService)
        {
            _orleansActorTestService = orleansActorTestService;
        }

        [HttpGet]
        public async Task<IActionResult> InsertAsync()
        {
            var count = await _orleansActorTestService.RunAsync();
            return Content(count.ToString());
        }
    }
}