using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<ContentResult> Get()
        {
            var service = _factory.Invoke(new EventSavingBurningOptions
            {
                UserIdCount = 10,
                BatchCount = 100,
                BatchSize = 1000
            });
            await service.StartAsync();
            return Content(DateTime.Now.ToString("f"));
        }
    }
}