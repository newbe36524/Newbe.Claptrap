using System;
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

        [HttpPost("{id}")]
        public async Task<IActionResult> AddItemAsync(int id, [FromBody] AddItemInput input)
        {
            var cartGrain = _grainFactory.GetGrain<ICartGrain>(id.ToString());
            var items = await cartGrain.AddItemAsync(input.SkuId, input.Count);
            return Json(items);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveItemAsync(int id, string skuId, int count)
        {
            var cartGrain = _grainFactory.GetGrain<ICartGrain>(id.ToString());
            var items = await cartGrain.RemoveItemAsync(skuId, count);
            return Json(items);
        }

        public class AddItemInput
        {
            public string SkuId { get; set; }
            public int Count { get; set; }
        }
    }
}