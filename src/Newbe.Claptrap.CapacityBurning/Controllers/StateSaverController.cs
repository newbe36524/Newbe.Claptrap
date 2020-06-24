using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Newbe.Claptrap.CapacityBurning.Controllers
{
    [Route("api/[controller]")]
    public class StateSaverController : Controller
    {
        private readonly StateSavingBurningService.Factory _factory;

        public StateSaverController(
            StateSavingBurningService.Factory factory)
        {
            _factory = factory;
        }

        public async Task<ContentResult> Get()
        {
            var service = _factory.Invoke(new StateSavingBurningOptions
            {
                UserIdCount = 10000,
                BatchCount = 100,
                BatchSize = 1000
            });
            await service.StartAsync();
            return Content(DateTime.Now.ToString("f"));
        }
    }
}