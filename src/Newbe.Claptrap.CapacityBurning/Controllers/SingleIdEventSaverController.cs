using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Newbe.Claptrap.CapacityBurning.Controllers
{
    [Route("api/[controller]")]
    public class SingleIdEventSaverController : Controller
    {
        private readonly EventSavingBurningService.Factory _factory;

        public SingleIdEventSaverController(
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
            return Content(DateTime.Now.ToString("h:mm:ss tt zz"));
        }
    }
}