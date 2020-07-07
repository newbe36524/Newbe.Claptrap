using System.Threading.Tasks;
using HelloClaptrap.IActor;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace HelloClaptrap.Web.Controllers
{
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly IGrainFactory _grainFactory;

        public CartController(
            IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemsAsync(int id)
        {
            var cartGrain = _grainFactory.GetGrain<ICartGrain>(id.ToString());
            var items = await cartGrain.GetItemsAsync();
            return Json(items);
        }
    }
}