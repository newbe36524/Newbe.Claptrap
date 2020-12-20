using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newbe.Claptrap.StorageTestWebApi.Services;

namespace Newbe.Claptrap.StorageTestWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ITestService _testService;

        public EventController(
            ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet]
        public async Task<IActionResult> InsertAsync()
        {
            var count = await _testService.RunAsync();
            return Content(count.ToString());
        }
    }
}