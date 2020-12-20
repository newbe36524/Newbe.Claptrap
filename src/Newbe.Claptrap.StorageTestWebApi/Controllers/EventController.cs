using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newbe.Claptrap.StorageTestWebApi.Services;

namespace Newbe.Claptrap.StorageTestWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IInMemoryActorTestService _inMemoryActorTestService;

        public EventController(
            IInMemoryActorTestService inMemoryActorTestService)
        {
            _inMemoryActorTestService = inMemoryActorTestService;
        }

        [HttpGet]
        public async Task<IActionResult> InsertAsync()
        {
            var count = await _inMemoryActorTestService.RunAsync();
            return Content(count.ToString());
        }
    }
}