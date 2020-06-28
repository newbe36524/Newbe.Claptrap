using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newbe.Claptrap.CapacityBurning.Services;

namespace Newbe.Claptrap.CapacityBurning.Controllers
{
    [Route("api/[controller]")]
    public class EventSaverController : Controller
    {
        private readonly EventSavingBurningService.Factory _factory;

        public EventSaverController(
            EventSavingBurningService.Factory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public async Task<ContentResult> Get([FromQuery] EventSavingBurningOptions options)
        {
            var service = (IBurningService) _factory.Invoke(options);
            await service.PrepareAsync();
            var sw = Stopwatch.StartNew();
            await service.StartAsync();
            sw.Stop();
            var cost = sw.ElapsedMilliseconds;
            await service.CleanAsync();
            return Content($"{cost} ms");
        }
    }
}