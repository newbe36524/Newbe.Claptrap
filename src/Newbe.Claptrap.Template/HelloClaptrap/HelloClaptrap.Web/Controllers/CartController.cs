using System.Threading.Tasks;
using Dapr.Actors.Client;
using HelloClaptrap.IActor;
using Microsoft.AspNetCore.Mvc;
using Newbe.Claptrap.Dapr;

namespace HelloClaptrap.Web.Controllers
{
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public CartController(
            IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemsAsync(int id)
        {
            var cartActor = _actorProxyFactory.GetClaptrap<ICartGrain>(id.ToString());
            var items = await cartActor.GetItemsAsync();
            return Json(items);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> AddItemAsync(int id, [FromBody] AddItemInput input)
        {
            var cartActor = _actorProxyFactory.GetClaptrap<ICartGrain>(id.ToString());
            var items = await cartActor.AddItemAsync(input.SkuId, input.Count);
            return Json(items);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveItemAsync(int id, string skuId, int count)
        {
            var cartActor = _actorProxyFactory.GetClaptrap<ICartGrain>(id.ToString());
            var items = await cartActor.RemoveItemAsync(skuId, count);
            return Json(items);
        }

        public class AddItemInput
        {
            public string SkuId { get; set; }
            public int Count { get; set; }
        }
    }
}